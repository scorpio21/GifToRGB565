using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    public class RGB565DebugForm : Form
    {
        private Bitmap originalBmp;
        private Bitmap rgb565Bmp;
        private int[] rgb565Data;
        private int imgW, imgH;

        private PictureBox picLeft;
        private PictureBox picRight;
        private Label lblLeft;
        private Label lblRight;
        private Panel pnlControls;
        private ComboBox cmbChannel;
        private TrackBar opacitySlider;
        private Label lblOpacity;
        private CheckBox chkDiff;
        private Label lblPixelInfo;
        private Panel pnlZoom;
        private PictureBox picZoomLeft;
        private PictureBox picZoomRight;
        private Label lblZoomLeft;
        private Label lblZoomRight;
        private NumericUpDown numZoomLevel;
        private Label lblZoomLevel;
        private ToolTip tip;

        private int viewChannel = -1; // -1=all, 0=R, 1=G, 2=B
        private float opacity = 1.0f;
        private bool showDiff = false;
        private int zoomLevel = 16;
        private Point hoverPixel = new(-1, -1);
        private const int ZoomSize = 128;

        public RGB565DebugForm(Bitmap original, ushort[] rgb565Pixels, int width, int height)
        {
            tip = new ToolTip();
            imgW = width;
            imgH = height;
            rgb565Data = new int[rgb565Pixels.Length];
            for (int i = 0; i < rgb565Pixels.Length; i++)
                rgb565Data[i] = rgb565Pixels[i];

            originalBmp = new Bitmap(original, width, height);
            rgb565Bmp = RebuildRgb565Bitmap(rgb565Pixels, width, height);

            InitUI();
            this.Text = $"Debug RGB565 - {width}x{height} - {rgb565Pixels.Length} píxeles";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.DoubleBuffered = true;
        }

        private void InitUI()
        {
            pnlControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(8, 8, 8, 8)
            };

            var lblChannel = new Label { Text = "Canal:", Location = new Point(8, 14), AutoSize = true };
            cmbChannel = new ComboBox
            {
                Location = new Point(52, 10),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbChannel.Items.AddRange(new object[] { "RGB", "Rojo", "Verde", "Azul" });
            cmbChannel.SelectedIndex = 0;
            cmbChannel.SelectedIndexChanged += (s, e) =>
            {
                viewChannel = cmbChannel.SelectedIndex - 1;
                RenderImages();
            };

            chkDiff = new CheckBox
            {
                Text = "Modo Diff",
                Location = new Point(170, 12),
                AutoSize = true
            };
            chkDiff.CheckedChanged += (s, e) =>
            {
                showDiff = chkDiff.Checked;
                RenderImages();
            };

            var lblOp = new Label { Text = "Opacidad:", Location = new Point(280, 14), AutoSize = true };
            opacitySlider = new TrackBar
            {
                Location = new Point(345, 8),
                Width = 200,
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10
            };
            lblOpacity = new Label
            {
                Text = "100%",
                Location = new Point(550, 14),
                AutoSize = true
            };
            opacitySlider.Scroll += (s, e) =>
            {
                opacity = opacitySlider.Value / 100f;
                lblOpacity.Text = $"{opacitySlider.Value}%";
                RenderImages();
            };

            lblPixelInfo = new Label
            {
                Text = "Hover sobre la imagen para ver valores",
                Location = new Point(620, 14),
                AutoSize = true,
                Font = new Font("Consolas", 9f)
            };

            pnlControls.Controls.AddRange(new Control[] {
                lblChannel, cmbChannel, chkDiff, lblOp, opacitySlider, lblOpacity, lblPixelInfo
            });

            picLeft = new PictureBox
            {
                Dock = DockStyle.Left,
                Width = 500,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black,
                Cursor = Cursors.Cross
            };
            picLeft.MouseMove += PicZoom_MouseMove;

            picRight = new PictureBox
            {
                Dock = DockStyle.Right,
                Width = 500,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black,
                Cursor = Cursors.Cross
            };
            picRight.MouseMove += PicZoom_MouseMove;

            lblLeft = new Label
            {
                Text = "Original (24-bit)",
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };

            lblRight = new Label
            {
                Text = "RGB565 (16-bit)",
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };

            pnlZoom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = ZoomSize + 50,
                Padding = new Padding(8)
            };

            var lblZoomTitle = new Label
            {
                Text = "Zoom píxel a píxel:",
                Location = new Point(8, 4),
                AutoSize = true,
                Font = new Font("Consolas", 9f, FontStyle.Bold)
            };

            lblZoomLevel = new Label
            {
                Text = "Zoom:",
                Location = new Point(140, 6),
                AutoSize = true
            };
            numZoomLevel = new NumericUpDown
            {
                Location = new Point(180, 3),
                Width = 60,
                Minimum = 4,
                Maximum = 64,
                Value = 16
            };
            numZoomLevel.ValueChanged += (s, e) =>
            {
                zoomLevel = (int)numZoomLevel.Value;
                UpdateZoom();
            };

            picZoomLeft = new PictureBox
            {
                Location = new Point(8, 26),
                Width = ZoomSize,
                Height = ZoomSize,
                SizeMode = PictureBoxSizeMode.Normal,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            picZoomLeft.Paint += PicZoom_Paint;

            picZoomRight = new PictureBox
            {
                Location = new Point(ZoomSize + 20, 26),
                Width = ZoomSize,
                Height = ZoomSize,
                SizeMode = PictureBoxSizeMode.Normal,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            picZoomRight.Paint += PicZoom_Paint;

            lblZoomLeft = new Label
            {
                Location = new Point(8, ZoomSize + 30),
                Size = new Size(ZoomSize, 16),
                Font = new Font("Consolas", 8f),
                TextAlign = ContentAlignment.TopCenter
            };
            lblZoomRight = new Label
            {
                Location = new Point(ZoomSize + 20, ZoomSize + 30),
                Size = new Size(ZoomSize, 16),
                Font = new Font("Consolas", 8f),
                TextAlign = ContentAlignment.TopCenter
            };

            pnlZoom.Controls.AddRange(new Control[] {
                lblZoomTitle, lblZoomLevel, numZoomLevel,
                picZoomLeft, picZoomRight, lblZoomLeft, lblZoomRight
            });

            Controls.Add(picRight);
            Controls.Add(picLeft);
            Controls.Add(lblRight);
            Controls.Add(lblLeft);
            Controls.Add(pnlZoom);
            Controls.Add(pnlControls);

            RenderImages();
        }

        private void RenderImages()
        {
            if (originalBmp == null || rgb565Bmp == null) return;

            var leftBmp = new Bitmap(imgW, imgH);
            var rightBmp = new Bitmap(imgW, imgH);
            var diffBmp = new Bitmap(imgW, imgH);

            var origData = originalBmp.LockBits(new Rectangle(0, 0, imgW, imgH), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var rgbData = rgb565Bmp.LockBits(new Rectangle(0, 0, imgW, imgH), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var leftData = leftBmp.LockBits(new Rectangle(0, 0, imgW, imgH), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var rightData = rightBmp.LockBits(new Rectangle(0, 0, imgW, imgH), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var diffData = diffBmp.LockBits(new Rectangle(0, 0, imgW, imgH), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                for (int y = 0; y < imgH; y++)
                {
                    byte* oRow = (byte*)origData.Scan0 + y * origData.Stride;
                    byte* rRow = (byte*)rgbData.Scan0 + y * rgbData.Stride;
                    byte* lRow = (byte*)leftData.Scan0 + y * leftData.Stride;
                    byte* riRow = (byte*)rightData.Scan0 + y * rightData.Stride;
                    byte* dRow = (byte*)diffData.Scan0 + y * diffData.Stride;

                    for (int x = 0; x < imgW; x++)
                    {
                        int i = x * 4;
                        int oB = oRow[i], oG = oRow[i + 1], oR = oRow[i + 2];
                        int rB = rRow[i], rG = rRow[i + 1], rR = rRow[i + 2];

                        if (showDiff)
                        {
                            int dB = Math.Abs(oB - rB);
                            int dG = Math.Abs(oG - rG);
                            int dR = Math.Abs(oR - rR);
                            int diff = Math.Max(dB, Math.Max(dG, dR));
                            byte diffByte = (byte)Math.Min(255, diff * 3);

                            dRow[i] = diffByte;
                            dRow[i + 1] = (byte)(diff > 0 ? 255 : 0);
                            dRow[i + 2] = diffByte;
                            dRow[i + 3] = 255;

                            lRow[i] = (byte)oB;
                            lRow[i + 1] = (byte)oG;
                            lRow[i + 2] = (byte)oR;
                            lRow[i + 3] = 255;

                            riRow[i] = diffByte;
                            riRow[i + 1] = (byte)(diff > 0 ? 255 : 0);
                            riRow[i + 2] = diffByte;
                            riRow[i + 3] = 255;
                        }
                        else
                        {
                            int lB = oB, lG = oG, lR = oR;
                            int rB2 = rB, rG2 = rG, rR2 = rR;

                            if (viewChannel >= 0)
                            {
                                switch (viewChannel)
                                {
                                    case 0: lG = 0; lB = 0; rG2 = 0; rB2 = 0; break;
                                    case 1: lR = 0; lB = 0; rR2 = 0; rB2 = 0; break;
                                    case 2: lR = 0; lG = 0; rR2 = 0; rG2 = 0; break;
                                }
                            }

                            lRow[i] = (byte)lB;
                            lRow[i + 1] = (byte)lG;
                            lRow[i + 2] = (byte)lR;
                            lRow[i + 3] = 255;

                            if (opacity < 1.0f)
                            {
                                rB2 = (int)(rB2 * opacity + lB * (1 - opacity));
                                rG2 = (int)(rG2 * opacity + lG * (1 - opacity));
                                rR2 = (int)(rR2 * opacity + lR * (1 - opacity));
                            }

                            riRow[i] = (byte)rB2;
                            riRow[i + 1] = (byte)rG2;
                            riRow[i + 2] = (byte)rR2;
                            riRow[i + 3] = 255;
                        }
                    }
                }
            }

            originalBmp.UnlockBits(origData);
            rgb565Bmp.UnlockBits(rgbData);
            leftBmp.UnlockBits(leftData);
            rightBmp.UnlockBits(rightData);
            diffBmp.UnlockBits(diffData);

            picLeft.Image?.Dispose();
            picRight.Image?.Dispose();
            picLeft.Image = leftBmp;
            picRight.Image = rightBmp;
        }

        private void PicZoom_MouseMove(object? sender, MouseEventArgs e)
        {
            if (picLeft.Image == null) return;

            var pb = (PictureBox)sender!;
            float scaleX = (float)imgW / pb.ClientSize.Width;
            float scaleY = (float)imgH / pb.ClientSize.Height;

            int px = (int)(e.X * scaleX);
            int py = (int)(e.Y * scaleY);
            if (px < 0 || px >= imgW || py < 0 || py >= imgH) return;

            hoverPixel = new Point(px, py);

            int origIdx = (py * imgW + px) * 4;
            var origData = originalBmp.LockBits(new Rectangle(px, py, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int oB = Marshal.ReadByte(origData.Scan0, 0);
            int oG = Marshal.ReadByte(origData.Scan0, 1);
            int oR = Marshal.ReadByte(origData.Scan0, 2);
            originalBmp.UnlockBits(origData);

            int rgbVal = rgb565Data[py * imgW + px];
            int r5 = (rgbVal >> 11) & 0x1F;
            int g6 = (rgbVal >> 5) & 0x3F;
            int b5 = rgbVal & 0x1F;
            int r8 = (r5 << 3) | (r5 >> 2);
            int g8 = (g6 << 2) | (g6 >> 4);
            int b8 = (b5 << 3) | (b5 >> 2);

            lblPixelInfo.Text = $"({px},{py}) Original: #{oR:X2}{oG:X2}{oB:X2}  RGB565: 0x{rgbVal:X4} → #{r8:X2}{g8:X2}{b8:X2}  ΔR={Math.Abs(oR - r8)} ΔG={Math.Abs(oG - g8)} ΔB={Math.Abs(oB - b8)}";

            UpdateZoom();
        }

        private void UpdateZoom()
        {
            if (hoverPixel.X < 0 || hoverPixel.Y < 0) return;
            picZoomLeft.Invalidate();
            picZoomRight.Invalidate();
        }

        private void PicZoom_Paint(object? sender, PaintEventArgs e)
        {
            if (hoverPixel.X < 0 || hoverPixel.Y < 0) return;
            var pb = (PictureBox)sender!;
            bool isRight = pb == picZoomRight;

            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            int half = zoomLevel / 2;
            int sx = Math.Max(0, hoverPixel.X - half);
            int sy = Math.Max(0, hoverPixel.Y - half);
            int sw = Math.Min(zoomLevel, imgW - sx);
            int sh = Math.Min(zoomLevel, imgH - sy);

            var srcBmp = isRight ? rgb565Bmp : originalBmp;
            var srcRect = new Rectangle(sx, sy, sw, sh);
            var dstRect = new Rectangle(0, 0, ZoomSize, ZoomSize);

            using var brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, dstRect);
            g.DrawImage(srcBmp, dstRect, srcRect, GraphicsUnit.Pixel);

            int cellW = ZoomSize / zoomLevel;
            int cellH = ZoomSize / zoomLevel;
            using var gridPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
            using var centerPen = new Pen(Color.Yellow, 2);

            for (int gx = 0; gx <= zoomLevel; gx++)
                g.DrawLine(gridPen, gx * cellW, 0, gx * cellW, ZoomSize);
            for (int gy = 0; gy <= zoomLevel; gy++)
                g.DrawLine(gridPen, 0, gy * cellH, ZoomSize, gy * cellH);

            int cx = (hoverPixel.X - sx) * cellW;
            int cy = (hoverPixel.Y - sy) * cellH;
            g.DrawRectangle(centerPen, cx, cy, cellW, cellH);

            if (isRight)
                lblZoomRight.Text = $"0x{rgb565Data[hoverPixel.Y * imgW + hoverPixel.X]:X4}";
            else
            {
                var d = originalBmp.LockBits(new Rectangle(hoverPixel.X, hoverPixel.Y, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int b = Marshal.ReadByte(d.Scan0, 0);
                int g2 = Marshal.ReadByte(d.Scan0, 1);
                int r = Marshal.ReadByte(d.Scan0, 2);
                originalBmp.UnlockBits(d);
                lblZoomLeft.Text = $"#{r:X2}{g2:X2}{b:X2}";
            }
        }

        private static Bitmap RebuildRgb565Bitmap(ushort[] pixels, int w, int h)
        {
            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    int x = i % w;
                    int y = i / w;
                    byte* row = (byte*)data.Scan0 + y * data.Stride;
                    int idx = x * 4;
                    int val = pixels[i];
                    row[idx] = (byte)(((val & 0x1F) << 3) | ((val & 0x1F) >> 2));
                    row[idx + 1] = (byte)((((val >> 5) & 0x3F) << 2) | (((val >> 5) & 0x3F) >> 4));
                    row[idx + 2] = (byte)((((val >> 11) & 0x1F) << 3) | (((val >> 11) & 0x1F) >> 2));
                    row[idx + 3] = 255;
                }
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                originalBmp?.Dispose();
                rgb565Bmp?.Dispose();
                tip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
