using System.Collections.Generic;
using System.Drawing;

namespace GifRGB565GUI
{
    public static class ImageConverter
    {
        // Opciones configurables
        public static bool EnableDithering = true;
        public static bool EnableNoiseReduction = false;
        public static bool EnableSharpen = false;

        public static List<ushort> ToRGB565(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            // Copia de trabajo
            Color[,] pixels = new Color[w, h];

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    pixels[x, y] = bmp.GetPixel(x, y);

            // -----------------------------
            // 1) Reducción de ruido (suavizado)
            // -----------------------------
            if (EnableNoiseReduction)
                ApplyNoiseReduction(pixels, w, h);

            // -----------------------------
            // 2) Sharpen (mejorar bordes)
            // -----------------------------
            if (EnableSharpen)
                ApplySharpen(pixels, w, h);

            // -----------------------------
            // 3) Conversión + dithering opcional
            // -----------------------------
            List<ushort> output = new List<ushort>(w * h);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color old = pixels[x, y];

                    // Convertir a RGB565
                    ushort rgb565 =
                        (ushort)(
                            ((old.R & 0xF8) << 8) |
                            ((old.G & 0xFC) << 3) |
                            (old.B >> 3)
                        );

                    // Si dithering está desactivado → salida directa
                    if (!EnableDithering)
                    {
                        output.Add(rgb565);
                        continue;
                    }

                    // Convertir RGB565 → RGB888 para calcular error
                    int r5 = (rgb565 >> 11) & 0x1F;
                    int g6 = (rgb565 >> 5) & 0x3F;
                    int b5 = rgb565 & 0x1F;

                    int newR = (r5 << 3) | (r5 >> 2);
                    int newG = (g6 << 2) | (g6 >> 4);
                    int newB = (b5 << 3) | (b5 >> 2);

                    // Error
                    int errR = old.R - newR;
                    int errG = old.G - newG;
                    int errB = old.B - newB;

                    output.Add(rgb565);

                    // Distribuir error Floyd–Steinberg
                    void AddError(int xx, int yy, float factor)
                    {
                        if (xx < 0 || xx >= w || yy < 0 || yy >= h) return;

                        Color c = pixels[xx, yy];
                        int nr = Clamp(c.R + (int)(errR * factor));
                        int ng = Clamp(c.G + (int)(errG * factor));
                        int nb = Clamp(c.B + (int)(errB * factor));
                        pixels[xx, yy] = Color.FromArgb(nr, ng, nb);
                    }

                    AddError(x + 1, y, 7f / 16f);
                    AddError(x - 1, y + 1, 3f / 16f);
                    AddError(x, y + 1, 5f / 16f);
                    AddError(x + 1, y + 1, 1f / 16f);
                }
            }

            return output;
        }

        // -----------------------------
        // Filtro de reducción de ruido
        // -----------------------------
        private static void ApplyNoiseReduction(Color[,] px, int w, int h)
        {
            int[,] kernel = {
                {1, 2, 1},
                {2, 4, 2},
                {1, 2, 1}
            };

            Color[,] temp = new Color[w, h];

            for (int y = 1; y < h - 1; y++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    int r = 0, g = 0, b = 0;
                    int sum = 16;

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color c = px[x + kx, y + ky];
                            int k = kernel[ky + 1, kx + 1];
                            r += c.R * k;
                            g += c.G * k;
                            b += c.B * k;
                        }
                    }

                    temp[x, y] = Color.FromArgb(r / sum, g / sum, b / sum);
                }
            }

            for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++)
                    px[x, y] = temp[x, y];
        }

        // -----------------------------
        // Filtro de sharpening
        // -----------------------------
        private static void ApplySharpen(Color[,] px, int w, int h)
        {
            int[,] kernel = {
                { 0, -1,  0},
                {-1,  5, -1},
                { 0, -1,  0}
            };

            Color[,] temp = new Color[w, h];

            for (int y = 1; y < h - 1; y++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    int r = 0, g = 0, b = 0;

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color c = px[x + kx, y + ky];
                            int k = kernel[ky + 1, kx + 1];
                            r += c.R * k;
                            g += c.G * k;
                            b += c.B * k;
                        }
                    }

                    temp[x, y] = Color.FromArgb(
                        Clamp(r),
                        Clamp(g),
                        Clamp(b)
                    );
                }
            }

            for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++)
                    px[x, y] = temp[x, y];
        }

        private static int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return v;
        }
    }
}
