using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ImageMagick;

namespace GifRGB565GUI
{
    public partial class CropForm : Form
    {
        private string currentFilePath = "";
        private MagickImage? magickImage;
        private MagickImageCollection? magickCollection;
        private bool isGif = false;
        private Image? previewImage;
        private int origW, origH;

        private bool isDragging = false;
        private bool isMoving = false;
        private Point dragStart;
        private Point dragEnd;
        private Rectangle selection;
        private bool hasSelection = false;

        private int moveOffsetX, moveOffsetY;

        public CropForm(string filePath)
        {
            InitializeComponent();
            currentFilePath = filePath;
            LoadImage(filePath);
        }

        private void LoadImage(string path)
        {
            try
            {
                currentFilePath = path;
                string ext = Path.GetExtension(path).ToLower();
                long size = new FileInfo(path).Length;
                string sizeStr = size > 1024 * 1024
                    ? $"{size / (1024.0 * 1024.0):F2}MiB"
                    : $"{size / 1024.0:F2}KiB";

                magickImage?.Dispose();
                magickImage = new MagickImage(path);
                origW = (int)magickImage.Width;
                origH = (int)magickImage.Height;

                if (ext == ".gif")
                {
                    isGif = true;
                    magickCollection?.Dispose();
                    magickCollection = new MagickImageCollection();
                    magickCollection.Read(path);

                    int frameCount = magickCollection.Count;
                    int totalCs = 0;
                    for (int i = 0; i < frameCount; i++)
                        totalCs += Math.Max(1, (int)magickCollection[i].AnimationDelay);
                    TimeSpan ts = TimeSpan.FromSeconds(totalCs / 100.0);

                    lblInfo.Text = $"Tamaño: {sizeStr}, ancho: {origW}px, altura: {origH}px, fotogramas: {frameCount}, tipo: gif, longitud: {ts.Minutes:D2}:{ts.Seconds:D2}";
                }
                else
                {
                    isGif = false;
                    magickCollection?.Dispose();
                    magickCollection = null;
                    lblInfo.Text = $"Tamaño: {sizeStr}, ancho: {origW}px, altura: {origH}px, tipo: {ext.TrimStart('.')}";
                }

                previewImage?.Dispose();
                previewImage = Image.FromFile(path);
                picPreview.Image = previewImage;

                txtLeft.Text = "0";
                txtTop.Text = "0";
                txtWidth.Text = origW.ToString();
                txtHeight.Text = origH.ToString();

                hasSelection = false;
                selection = new Rectangle(0, 0, origW, origH);
                picPreview.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void PicPreview_Paint(object? sender, PaintEventArgs e)
        {
            if (!hasSelection) return;
            if (previewImage == null) return;

            var g = e.Graphics;
            var imgRect = GetImageRect();
            if (imgRect.Width <= 0 || imgRect.Height <= 0) return;

            float scaleX = (float)imgRect.Width / origW;
            float scaleY = (float)imgRect.Height / origH;

            int sx = imgRect.X + (int)(selection.X * scaleX);
            int sy = imgRect.Y + (int)(selection.Y * scaleY);
            int sw = (int)(selection.Width * scaleX);
            int sh = (int)(selection.Height * scaleY);

            using var darkBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            using var borderPen = new Pen(Color.White, 2);

            g.SetClip(imgRect);
            g.FillRectangle(darkBrush, imgRect);

            g.ResetClip();

            using var clearBrush = new SolidBrush(Color.FromArgb(1, 0, 0, 0));
            g.SetClip(new Rectangle(sx, sy, sw, sh));
            g.DrawImage(previewImage, imgRect);
            g.ResetClip();

            using var borderPen2 = new Pen(Color.FromArgb(200, 255, 255, 255), 2);
            borderPen2.DashStyle = DashStyle.Dash;
            g.DrawRectangle(borderPen2, sx, sy, sw, sh);

            int handleSize = 6;
            using var handleBrush = new SolidBrush(Color.White);
            var handles = new Point[]
            {
                new(sx - handleSize / 2, sy - handleSize / 2),
                new(sx + sw - handleSize / 2, sy - handleSize / 2),
                new(sx - handleSize / 2, sy + sh - handleSize / 2),
                new(sx + sw - handleSize / 2, sy + sh - handleSize / 2),
                new(sx + sw / 2 - handleSize / 2, sy - handleSize / 2),
                new(sx + sw / 2 - handleSize / 2, sy + sh - handleSize / 2),
                new(sx - handleSize / 2, sy + sh / 2 - handleSize / 2),
                new(sx + sw - handleSize / 2, sy + sh / 2 - handleSize / 2),
            };
            foreach (var h in handles)
                g.FillRectangle(handleBrush, h.X, h.Y, handleSize, handleSize);
        }

        private Rectangle GetImageRect()
        {
            if (previewImage == null) return Rectangle.Empty;

            int pw = picPreview.ClientSize.Width;
            int ph = picPreview.ClientSize.Height;
            float ratioImg = (float)origW / origH;
            float ratioPic = (float)pw / ph;

            int drawW, drawH, drawX, drawY;
            if (ratioImg > ratioPic)
            {
                drawW = pw;
                drawH = (int)(pw / ratioImg);
                drawX = 0;
                drawY = (ph - drawH) / 2;
            }
            else
            {
                drawH = ph;
                drawW = (int)(ph * ratioImg);
                drawX = (pw - drawW) / 2;
                drawY = 0;
            }

            return new Rectangle(drawX, drawY, drawW, drawH);
        }

        private Point ScreenToImage(Point screenPt)
        {
            var imgRect = GetImageRect();
            if (imgRect.Width <= 0 || imgRect.Height <= 0) return Point.Empty;

            float scaleX = (float)origW / imgRect.Width;
            float scaleY = (float)origH / imgRect.Height;

            int ix = (int)((screenPt.X - imgRect.X) * scaleX);
            int iy = (int)((screenPt.Y - imgRect.Y) * scaleY);
            ix = Math.Clamp(ix, 0, origW);
            iy = Math.Clamp(iy, 0, origH);
            return new Point(ix, iy);
        }

        private void PicPreview_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || previewImage == null) return;

            var imgPt = ScreenToImage(e.Location);

            if (hasSelection && selection.Contains(imgPt))
            {
                isMoving = true;
                moveOffsetX = imgPt.X - selection.X;
                moveOffsetY = imgPt.Y - selection.Y;
            }
            else
            {
                isDragging = true;
                dragStart = imgPt;
                dragEnd = imgPt;
                hasSelection = true;
            }
        }

        private void PicPreview_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDragging && !isMoving) return;

            var imgPt = ScreenToImage(e.Location);

            if (isDragging)
            {
                dragEnd = imgPt;
                int x = Math.Min(dragStart.X, dragEnd.X);
                int y = Math.Min(dragStart.Y, dragEnd.Y);
                int w = Math.Abs(dragEnd.X - dragStart.X);
                int h = Math.Abs(dragEnd.Y - dragStart.Y);

                string lockRatio = cmbAspectLock.SelectedItem?.ToString() ?? "No";
                if (lockRatio != "No" && lockRatio.Contains(':'))
                {
                    var parts = lockRatio.Split(':');
                    if (int.TryParse(parts[0], out int rW) && int.TryParse(parts[1], out int rH) && rW > 0 && rH > 0)
                    {
                        float ratio = (float)rW / rH;
                        h = (int)(w / ratio);
                        if (y + h > origH)
                        {
                            h = origH - y;
                            w = (int)(h * ratio);
                        }
                    }
                }

                w = Math.Min(w, origW - x);
                h = Math.Min(h, origH - y);
                selection = new Rectangle(x, y, w, h);
            }
            else if (isMoving)
            {
                int nx = imgPt.X - moveOffsetX;
                int ny = imgPt.Y - moveOffsetY;
                nx = Math.Clamp(nx, 0, origW - selection.Width);
                ny = Math.Clamp(ny, 0, origH - selection.Height);
                selection.Location = new Point(nx, ny);
            }

            txtLeft.Text = selection.X.ToString();
            txtTop.Text = selection.Y.ToString();
            txtWidth.Text = selection.Width.ToString();
            txtHeight.Text = selection.Height.ToString();
            picPreview.Invalidate();
        }

        private void PicPreview_MouseUp(object? sender, MouseEventArgs e)
        {
            isDragging = false;
            isMoving = false;
        }

        private void TxtFields_TextChanged(object? sender, EventArgs e)
        {
            if (int.TryParse(txtLeft.Text, out int l) && int.TryParse(txtTop.Text, out int t)
                && int.TryParse(txtWidth.Text, out int w) && int.TryParse(txtHeight.Text, out int h))
            {
                l = Math.Clamp(l, 0, origW);
                t = Math.Clamp(t, 0, origH);
                w = Math.Clamp(w, 1, origW - l);
                h = Math.Clamp(h, 1, origH - t);
                selection = new Rectangle(l, t, w, h);
                hasSelection = true;
                picPreview.Invalidate();
            }
        }

        private void BtnCrop_Click(object? sender, EventArgs e)
        {
            if (magickImage == null) return;
            if (!hasSelection || selection.Width <= 0 || selection.Height <= 0)
            {
                MessageBox.Show("Selecciona un área para recortar.", "Recortar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool trimTransparent = chkAutocrop.Checked;

            if (isGif && magickCollection != null && magickCollection.Count > 1)
                CropAnimated(selection, trimTransparent);
            else
                CropStatic(selection, trimTransparent);
        }

        private void CropStatic(Rectangle area, bool trimTransparent)
        {
            if (magickImage == null) return;

            using var clone = (MagickImage)magickImage.Clone();

                    if (trimTransparent)
                    {
                        clone.Trim();
                    }
                    else
                    {
                        var cropGeo = new MagickGeometry($"{area.Width}x{area.Height}+{area.X}+{area.Y}");
                        clone.Crop(cropGeo);
                    }
                    clone.Page = new MagickGeometry($"{area.Width}x{area.Height}+0+0");

            var resultPath = Path.Combine(Path.GetTempPath(), $"crop_result_{Guid.NewGuid():N}.png");
            clone.Write(resultPath);

            var result = new CropResult
            {
                ResultPath = resultPath,
                IsGif = false,
                Width = area.Width,
                Height = area.Height
            };

            DialogResult = DialogResult.OK;
            Tag = result;
            Close();
        }

        private void CropAnimated(Rectangle area, bool trimTransparent)
        {
            if (magickCollection == null) return;

            try
            {
                using var collection = (MagickImageCollection)magickCollection.Clone();
                collection.Coalesce();

                foreach (IMagickImage<byte> frame in collection)
                {
                    var magick = (MagickImage)frame;
                    if (trimTransparent)
                    {
                        magick.Trim();
                        magick.Page = new MagickGeometry($"{magick.Width}x{magick.Height}+0+0");
                    }
                    else
                    {
                        var cropGeo = new MagickGeometry($"{area.Width}x{area.Height}+{area.X}+{area.Y}");
                        magick.Crop(cropGeo);
                        magick.Page = new MagickGeometry($"{area.Width}x{area.Height}+0+0");
                    }
                }

                var resultPath = Path.Combine(Path.GetTempPath(), $"crop_result_{Guid.NewGuid():N}.gif");
                collection.Write(resultPath);

                var result = new CropResult
                {
                    ResultPath = resultPath,
                    IsGif = true,
                    Width = area.Width,
                    Height = area.Height
                };

                DialogResult = DialogResult.OK;
                Tag = result;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recortando GIF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CropResult
    {
        public string ResultPath { get; set; } = "";
        public bool IsGif { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
