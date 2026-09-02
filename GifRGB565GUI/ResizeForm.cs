using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    public partial class ResizeForm : Form
    {
        private Bitmap? originalBmp;
        private Bitmap? resizedBmp;
        private string currentFilePath = "";

        private Image? gifImage;
        private FrameDimension? gifFrameDimension;
        private int gifFrameCount = 0;
        private int[]? gifFrameDelays;
        private bool isGif = false;

        public ResizeForm()
        {
            InitializeComponent();
            cmbMethod.SelectedIndex = 0;
            cmbAspect.SelectedIndex = 0;
        }

        private void ResizeForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ResizeForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0) return;
            LoadImageFile(files[0]);
        }

        private void BtnOpen_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "Imágenes|*.gif;*.png;*.jpg;*.jpeg;*.bmp;*.webp;*.apng;*.heic;*.heif;*.avif|Todos|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
                LoadImageFile(dlg.FileName);
        }

        private void LoadImageFile(string path)
        {
            try
            {
                CleanupGif();

                currentFilePath = path;
                string ext = Path.GetExtension(path).ToLower();
                long size = new FileInfo(path).Length;
                string sizeStr = size > 1024 * 1024 ? $"{size / (1024.0 * 1024.0):F2}MiB" : $"{size / 1024.0:F2}KiB";

                if (ext == ".gif")
                {
                    isGif = true;
                    gifImage = Image.FromFile(path);
                    gifFrameDimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
                    gifFrameCount = gifImage.GetFrameCount(gifFrameDimension);

                    gifFrameDelays = GetGifFrameDelays(gifImage, gifFrameCount);

                    int totalMs = 0;
                    foreach (int d in gifFrameDelays) totalMs += d;
                    TimeSpan ts = TimeSpan.FromMilliseconds(totalMs * 10);

                    lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {gifImage.Width}px, altura: {gifImage.Height}px, fotogramas: {gifFrameCount}, tipo: gif, longitud: {ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds / 100}";

                    originalBmp?.Dispose();
                    originalBmp = new Bitmap(gifImage.Width, gifImage.Height);
                    using (var g = Graphics.FromImage(originalBmp))
                        g.DrawImage(gifImage, 0, 0, gifImage.Width, gifImage.Height);

                    picPreview.Image = gifImage;
                    txtWidth.Text = gifImage.Width.ToString();
                    txtHeight.Text = gifImage.Height.ToString();
                }
                else
                {
                    isGif = false;
                    originalBmp?.Dispose();
                    originalBmp = new Bitmap(path);
                    picPreview.Image = originalBmp;

                    lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {originalBmp.Width}px, altura: {originalBmp.Height}px, tipo: {ext.TrimStart('.')}";

                    txtWidth.Text = originalBmp.Width.ToString();
                    txtHeight.Text = originalBmp.Height.ToString();
                }

                suppressEvents = true;
                txtPercent.Text = "100";
                suppressEvents = false;

                btnSave.Enabled = false;
                panelResult.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int[] GetGifFrameDelays(Image img, int frameCount)
        {
            var delays = new int[frameCount];
            try
            {
                var prop = img.GetPropertyItem(0x5100);
                if (prop.Value != null)
                {
                    for (int i = 0; i < frameCount; i++)
                    {
                        delays[i] = BitConverter.ToInt32(prop.Value, i * 4);
                        if (delays[i] < 1) delays[i] = 1;
                    }
                }
                else
                {
                    for (int i = 0; i < frameCount; i++)
                        delays[i] = 10;
                }
            }
            catch
            {
                for (int i = 0; i < frameCount; i++)
                    delays[i] = 10;
            }
            return delays;
        }

        private bool suppressEvents = false;

        private void txtWidth_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents || originalBmp == null) return;
            int origW = isGif && gifImage != null ? gifImage.Width : originalBmp.Width;
            if (int.TryParse(txtWidth.Text, out int w) && w > 0)
            {
                suppressEvents = true;
                double pct = (double)w / origW * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void txtHeight_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents || originalBmp == null) return;
            int origH = isGif && gifImage != null ? gifImage.Height : originalBmp.Height;
            if (int.TryParse(txtHeight.Text, out int h) && h > 0)
            {
                suppressEvents = true;
                double pct = (double)h / origH * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void txtPercent_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents) return;
            int origW = 0, origH = 0;
            if (isGif && gifImage != null) { origW = gifImage.Width; origH = gifImage.Height; }
            else if (originalBmp != null) { origW = originalBmp.Width; origH = originalBmp.Height; }
            if (origW <= 0 || origH <= 0) return;

            if (double.TryParse(txtPercent.Text, out double pct) && pct > 0)
            {
                suppressEvents = true;
                txtWidth.Text = ((int)(origW * pct / 100)).ToString();
                txtHeight.Text = ((int)(origH * pct / 100)).ToString();
                suppressEvents = false;
            }
        }

        private void BtnResize_Click(object? sender, EventArgs e)
        {
            if (originalBmp == null && gifImage == null)
            {
                MessageBox.Show("No hay imagen cargada.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int w) || !int.TryParse(txtHeight.Text, out int h) || w <= 0 || h <= 0)
            {
                MessageBox.Show("Ancho y altura deben ser números válidos.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isGif && gifImage != null && gifFrameCount > 1)
            {
                ResizeAnimatedGif(w, h);
            }
            else
            {
                ResizeStaticImage(w, h);
            }
        }

        private void ResizeStaticImage(int w, int h)
        {
            if (originalBmp == null) return;
            int method = cmbMethod.SelectedIndex;

            resizedBmp?.Dispose();
            resizedBmp = ApplyResize(originalBmp, w, h, method);

            picPreview.Image = resizedBmp;
            btnSave.Enabled = true;
            ShowResultPanel(w, h);
        }

        private void ResizeAnimatedGif(int w, int h)
        {
            if (gifImage == null || gifFrameDimension == null || gifFrameDelays == null) return;

            int method = cmbMethod.SelectedIndex;
            int origW = gifImage.Width;
            int origH = gifImage.Height;

            var frames = new Bitmap[gifFrameCount];

            try
            {
                for (int i = 0; i < gifFrameCount; i++)
                {
                    gifImage.SelectActiveFrame(gifFrameDimension, i);
                    using var frameBmp = new Bitmap(origW, origH);
                    using (var g = Graphics.FromImage(frameBmp))
                        g.DrawImage(gifImage, 0, 0, origW, origH);

                    frames[i] = ApplyResize(frameBmp, w, h, method);
                }

                var resultPath = Path.Combine(Path.GetTempPath(), $"resize_preview_{Guid.NewGuid():N}.gif");
                SaveAnimatedGif(frames, gifFrameDelays, resultPath);

                CleanupGif();
                gifImage = Image.FromFile(resultPath);
                gifFrameDimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
                gifFrameCount = gifImage.GetFrameCount(gifFrameDimension);

                resizedBmp?.Dispose();
                resizedBmp = new Bitmap(w, h);
                using (var g = Graphics.FromImage(resizedBmp))
                    g.DrawImage(frames[0], 0, 0, w, h);

                picPreview.Image = gifImage;

                for (int i = 0; i < frames.Length; i++)
                    frames[i].Dispose();

                btnSave.Enabled = true;
                ShowResultPanel(w, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error redimensionando GIF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                for (int i = 0; i < frames.Length; i++)
                    frames[i]?.Dispose();
            }
        }

        private Bitmap ApplyResize(Bitmap source, int w, int h, int method)
        {
            if (method <= 2)
            {
                var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);
                g.InterpolationMode = method switch
                {
                    1 => InterpolationMode.HighQualityBilinear,
                    2 => InterpolationMode.NearestNeighbor,
                    _ => InterpolationMode.HighQualityBicubic
                };
                g.DrawImage(source, 0, 0, w, h);
                return bmp;
            }
            else if (method == 3) // Centrar y recortar
            {
                double scaleX = (double)w / source.Width;
                double scaleY = (double)h / source.Height;
                double scale = Math.Max(scaleX, scaleY);
                int sw = (int)(source.Width * scale);
                int sh = (int)(source.Height * scale);
                var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, (w - sw) / 2, (h - sh) / 2, sw, sh);
                return bmp;
            }
            else if (method == 4) // Estirar
            {
                var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, w, h);
                return bmp;
            }
            else if (method == 5) // Forzar aspect ratio
            {
                double ratio = (double)source.Width / source.Height;
                int nw = w;
                int nh = (int)(w / ratio);
                if (nh > h) { nh = h; nw = (int)(h * ratio); }
                var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                g.DrawImage(source, (w - nw) / 2, (h - nh) / 2, nw, nh);
                return bmp;
            }
            else // Relleno transparente
            {
                var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                int ox = (w - source.Width) / 2;
                int oy = (h - source.Height) / 2;
                g.DrawImage(source, ox, oy, source.Width, source.Height);
                return bmp;
            }
        }

        private void SaveAnimatedGif(Bitmap[] frames, int[] delays, string path)
        {
            if (frames.Length == 0) return;

            var tempFiles = new List<string>();

            try
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    string tempFrame = Path.Combine(Path.GetTempPath(), $"frame_{i}_{Guid.NewGuid():N}.gif");
                    frames[i].Save(tempFrame, ImageFormat.Gif);
                    tempFiles.Add(tempFrame);
                }

                using var mainImage = Image.FromFile(tempFiles[0]);

                if (frames.Length > 1)
                {
                    var eps = new EncoderParameters(1);
                    eps.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);

                    for (int i = 1; i < tempFiles.Count; i++)
                    {
                        using var frameImage = Image.FromFile(tempFiles[i]);
                        mainImage.SaveAdd(frameImage, eps);
                    }
                }

                try
                {
                    var prop = mainImage.GetPropertyItem(0x5100);
                    byte[] delayBytes = new byte[delays.Length * 4];
                    for (int i = 0; i < delays.Length; i++)
                        BitConverter.GetBytes(delays[i]).CopyTo(delayBytes, i * 4);
                    prop.Value = delayBytes;
                    prop.Len = delayBytes.Length;
                    mainImage.SetPropertyItem(prop);
                }
                catch { }

                mainImage.Save(path, ImageFormat.Gif);
            }
            finally
            {
                foreach (var f in tempFiles)
                    try { File.Delete(f); } catch { }
            }
        }

        private void ShowResultPanel(int w, int h)
        {
            picResult.Image?.Dispose();
            picResult.Image = resizedBmp != null ? new Bitmap(resizedBmp) : null;

            string ext = Path.GetExtension(currentFilePath).ToLower();
            long origSize = currentFilePath.Length > 0 ? new FileInfo(currentFilePath).Length : 0;
            string origSizeStr = origSize > 1024 ? $"{origSize / 1024.0:F2}KiB" : $"{origSize}B";

            using var ms = new MemoryStream();
            resizedBmp?.Save(ms, ImageFormat.Png);
            long newSize = ms.Length;
            string newSizeStr = newSize > 1024 ? $"{newSize / 1024.0:F2}KiB" : $"{newSize}B";
            double reduction = origSize > 0 ? (1.0 - (double)newSize / origSize) * 100 : 0;
            string reductionStr = reduction > 0 ? $"( {reduction:F1}% )" : "";

            lblResultInfo.Text = $"Tamaño del archivo: {newSizeStr} {reductionStr}  ancho: {w}px, altura: {h}px, tipo: {ext.TrimStart('.')}";
            panelResult.Visible = true;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (resizedBmp == null)
            {
                MessageBox.Show("No hay imagen redimensionada para guardar.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog();
            dlg.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|Todos|*.*";
            dlg.FileName = Path.GetFileNameWithoutExtension(currentFilePath) + "_resized";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var format = Path.GetExtension(dlg.FileName).ToLower() switch
                {
                    ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                    ".bmp" => ImageFormat.Bmp,
                    _ => ImageFormat.Png
                };
                resizedBmp.Save(dlg.FileName, format);
                lblFileInfo.Text = $"Guardado: {dlg.FileName}";
            }
        }

        private void CleanupGif()
        {
            if (gifImage != null)
            {
                gifImage.Dispose();
                gifImage = null;
            }
            gifFrameDimension = null;
            gifFrameCount = 0;
            gifFrameDelays = null;
            isGif = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupGif();
            originalBmp?.Dispose();
            resizedBmp?.Dispose();
            picResult.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
