using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GifRGB565GUI
{
    internal class GifEncoder
    {
        private Stream stream;
        private BinaryWriter writer;

        public GifEncoder(Stream s)
        {
            stream = s;
            writer = new BinaryWriter(s);
        }

        public void WriteHeader(int width, int height, Color[] globalPalette)
        {
            writer.Write(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }); // GIF89a

            int paletteBits = GetPaletteBits(globalPalette.Length);
            int gctSize = 1 << (paletteBits + 1);

            writer.Write((short)width);
            writer.Write((short)height);

            byte packed = (byte)(0x80 | ((paletteBits) << 4) | paletteBits);
            writer.Write(packed);
            writer.Write((byte)0); // bg color index
            writer.Write((byte)0); // pixel aspect ratio

            for (int i = 0; i < gctSize; i++)
            {
                if (i < globalPalette.Length)
                {
                    writer.Write(globalPalette[i].R);
                    writer.Write(globalPalette[i].G);
                    writer.Write(globalPalette[i].B);
                }
                else
                {
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                }
            }
        }

        public void WriteNetscapeExtension(int loopCount)
        {
            writer.Write((byte)0x21); // Extension
            writer.Write((byte)0xFF); // Application Extension
            writer.Write((byte)0x0B); // Block size

            writer.Write(new byte[] {
                0x4E, 0x45, 0x54, 0x53, 0x43, 0x41, 0x50, 0x45, 0x32, 0x2E, 0x30 // NETSCAPE2.0
            });

            writer.Write((byte)0x03); // Sub-block size
            writer.Write((byte)0x01); // Sub-block ID
            writer.Write((short)loopCount);
            writer.Write((byte)0x00); // Block terminator
        }

        public void WriteFrame(Bitmap frame, int delayCentiseconds, Color[] palette)
        {
            int width = frame.Width;
            int height = frame.Height;

            // Graphic Control Extension
            writer.Write((byte)0x21); // Extension
            writer.Write((byte)0xF9); // GCE
            writer.Write((byte)0x04); // Block size

            byte disposalMethod = 2; // Restore to background
            byte packed = (byte)((disposalMethod << 2) | 0x00); // no transparency
            writer.Write(packed);
            writer.Write((short)delayCentiseconds);
            writer.Write((byte)0); // Transparent color index
            writer.Write((byte)0); // Block terminator

            // Image Descriptor
            writer.Write((byte)0x2C); // Image separator
            writer.Write((short)0); // Left
            writer.Write((short)0); // Top
            writer.Write((short)width);
            writer.Write((short)height);

            // Local Color Table
            int paletteBits = GetPaletteBits(palette.Length);
            int lctSize = 1 << (paletteBits + 1);
            byte lctPacked = (byte)(0x80 | paletteBits); // LCT flag + size
            writer.Write(lctPacked);

            for (int i = 0; i < lctSize; i++)
            {
                if (i < palette.Length)
                {
                    writer.Write(palette[i].R);
                    writer.Write(palette[i].G);
                    writer.Write(palette[i].B);
                }
                else
                {
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                }
            }

            // Indexed pixels
            int[] indexedPixels = QuantizeToPalette(frame, palette);

            // LZW compress
            int minCodeSize = Math.Max(2, paletteBits + 1);
            writer.Write((byte)minCodeSize);
            LzwCompress(indexedPixels, minCodeSize, writer);
        }

        public void WriteTrailer()
        {
            writer.Write((byte)0x3B);
            writer.Flush();
        }

        private int GetPaletteBits(int colorCount)
        {
            int bits = 0;
            int size = 2;
            while (size < colorCount && bits < 7)
            {
                bits++;
                size *= 2;
            }
            return bits;
        }

        private int[] QuantizeToPalette(Bitmap bmp, Color[] palette)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            var pixels = new int[w * h];
            var indexed = new int[w * h];

            var bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int stride = bmpData.Stride;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int offset = y * stride + x * 4;
                        byte b = ptr[offset];
                        byte g = ptr[offset + 1];
                        byte r = ptr[offset + 2];
                        byte a = ptr[offset + 3];
                        pixels[y * w + x] = Color.FromArgb(a, r, g, b).ToArgb();
                    }
                }
            }
            bmp.UnlockBits(bmpData);

            var colorMap = new Dictionary<int, int>();
            for (int i = 0; i < palette.Length; i++)
                colorMap[palette[i].ToArgb()] = i;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (colorMap.TryGetValue(pixels[i], out int idx))
                {
                    indexed[i] = idx;
                }
                else
                {
                    indexed[i] = FindNearest(pixels[i], palette);
                }
            }

            return indexed;
        }

        private int FindNearest(int argb, Color[] palette)
        {
            Color c = Color.FromArgb(argb);
            int bestIdx = 0;
            int bestDist = int.MaxValue;

            for (int i = 0; i < palette.Length; i++)
            {
                int dr = c.R - palette[i].R;
                int dg = c.G - palette[i].G;
                int db = c.B - palette[i].B;
                int dist = dr * dr + dg * dg + db * db;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIdx = i;
                    if (dist == 0) break;
                }
            }
            return bestIdx;
        }

        private void LzwCompress(int[] pixels, int minCodeSize, BinaryWriter writer)
        {
            int clearCode = 1 << minCodeSize;
            int eofCode = clearCode + 1;
            int codeSize = minCodeSize + 1;
            int nextCode = eofCode + 1;
            int maxCode = 1 << codeSize;

            var dict = new Dictionary<int, int>();
            for (int i = 0; i < clearCode; i++)
                dict[i] = i;

            var output = new List<int>();
            int bitBuffer = 0;
            int bitsInBuffer = 0;

            void EmitCode(int code)
            {
                bitBuffer |= (code << bitsInBuffer);
                bitsInBuffer += codeSize;
                while (bitsInBuffer >= 8)
                {
                    output.Add(bitBuffer & 0xFF);
                    bitBuffer >>= 8;
                    bitsInBuffer -= 8;
                }
            }

            EmitCode(clearCode);

            if (pixels.Length == 0)
            {
                EmitCode(eofCode);
                if (bitsInBuffer > 0) output.Add(bitBuffer & 0xFF);
                WriteSubBlocks(output.ConvertAll(x => (byte)x).ToArray(), writer);
                return;
            }

            int w = pixels[0];
            var prefix = new Dictionary<long, int>();
            int dictCounter = nextCode;
            int currentDictMax = maxCode;

            for (int i = 1; i < pixels.Length; i++)
            {
                int k = pixels[i];
                long key = ((long)w << 12) | k;

                if (prefix.ContainsKey(key))
                {
                    w = prefix[key];
                }
                else
                {
                    EmitCode(w);

                    if (dictCounter < 4096)
                    {
                        prefix[key] = dictCounter;
                        dictCounter++;

                        if (dictCounter > currentDictMax && codeSize < 12)
                        {
                            codeSize++;
                            currentDictMax = (1 << codeSize) - 1;
                        }
                    }
                    else
                    {
                        EmitCode(clearCode);
                        prefix.Clear();
                        dictCounter = nextCode;
                        codeSize = minCodeSize + 1;
                        currentDictMax = (1 << codeSize) - 1;
                    }

                    w = k;
                }
            }

            EmitCode(w);
            EmitCode(eofCode);

            if (bitsInBuffer > 0)
                output.Add(bitBuffer & 0xFF);

            WriteSubBlocks(output.ConvertAll(x => (byte)x).ToArray(), writer);
        }

        private void WriteSubBlocks(byte[] data, BinaryWriter writer)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int blockSize = Math.Min(255, data.Length - offset);
                writer.Write((byte)blockSize);
                writer.Write(data, offset, blockSize);
                offset += blockSize;
            }
            writer.Write((byte)0x00); // Block terminator
        }

        public static Color[] BuildPalette(Bitmap[] frames, int maxColors)
        {
            var colorCounts = new Dictionary<int, int>();

            foreach (var bmp in frames)
            {
                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    int stride = bmpData.Stride;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            int offset = y * stride + x * 4;
                            byte b = ptr[offset];
                            byte g = ptr[offset + 1];
                            byte r = ptr[offset + 2];
                            byte a = ptr[offset + 3];
                            int color = Color.FromArgb(a, r, g, b).ToArgb();
                            if (colorCounts.ContainsKey(color))
                                colorCounts[color]++;
                            else
                                colorCounts[color] = 1;
                        }
                    }
                }
                bmp.UnlockBits(bmpData);
            }

            var sorted = new List<KeyValuePair<int, int>>(colorCounts);
            sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

            var palette = new Color[Math.Min(maxColors, sorted.Count)];
            for (int i = 0; i < palette.Length; i++)
                palette[i] = Color.FromArgb(sorted[i].Key);

            return palette;
        }
    }
}
