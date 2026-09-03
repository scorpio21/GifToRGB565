using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ImageMagick;

namespace GifRGB565GUI
{
    public partial class ResizeForm : Form
    {
        private Bitmap? resizedBmp;
        private string currentFilePath = "";
        private string? originalGifPath;

        private MagickImage? magickImage;
        private MagickImageCollection? magickCollection;
        private bool isGif = false;
        private int[]? gifFrameDelays;
        private int gifFrameCount = 0;
        private Image? previewImage;

        public ResizeForm()
        {
            InitializeComponent();
            cmbMethod.SelectedIndex = 0;
            cmbAspect.SelectedIndex = 0;

            txtWidth.TextChanged += txtWidth_TextChanged;
            txtHeight.TextChanged += txtHeight_TextChanged;
            txtPercent.TextChanged += txtPercent_TextChanged;
            cmbPreset.SelectedIndex = 0;
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
                Cleanup();
                currentFilePath = path;
                string ext = Path.GetExtension(path).ToLower();
                long size = new FileInfo(path).Length;
                string sizeStr = size > 1024 * 1024
                    ? $"{size / (1024.0 * 1024.0):F2}MiB"
                    : $"{size / 1024.0:F2}KiB";

                if (ext == ".gif")
                {
                    isGif = true;
                    LoadGif(path, sizeStr);
                }
                else
                {
                    isGif = false;
                    originalGifPath = null;
                    magickImage = new MagickImage(path);

                    previewImage?.Dispose();
                    previewImage = Image.FromFile(path);
                    picPreview.Image = previewImage;

                    lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {magickImage.Width}px, altura: {magickImage.Height}px, tipo: {ext.TrimStart('.')}";

                    txtWidth.Text = magickImage.Width.ToString();
                    txtHeight.Text = magickImage.Height.ToString();
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

        private void LoadGif(string path, string sizeStr)
        {
            magickCollection = new MagickImageCollection();
            magickCollection.Read(path);

            gifFrameCount = magickCollection.Count;
            gifFrameDelays = new int[gifFrameCount];

            for (int i = 0; i < gifFrameCount; i++)
            {
                int delay = (int)magickCollection[i].AnimationDelay;
                gifFrameDelays[i] = Math.Max(1, delay);
            }

            int totalCs = 0;
            foreach (int d in gifFrameDelays) totalCs += d;
            TimeSpan ts = TimeSpan.FromSeconds(totalCs / 100.0);

            var first = (MagickImage)magickCollection[0];
            lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {first.Page.Width}px, altura: {first.Page.Height}px, fotogramas: {gifFrameCount}, tipo: gif, longitud: {ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds / 100}";

            magickImage = new MagickImage(path);
            originalGifPath = path;

            previewImage?.Dispose();
            previewImage = Image.FromFile(path);
            picPreview.Image = previewImage;

            txtWidth.Text = magickImage.Width.ToString();
            txtHeight.Text = magickImage.Height.ToString();
        }

        private bool suppressEvents = false;

        private void txtWidth_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents) return;
            int origW = GetOriginalWidth();
            int origH = GetOriginalHeight();
            if (origW <= 0) return;
            if (int.TryParse(txtWidth.Text, out int w) && w > 0)
            {
                suppressEvents = true;
                if (chkKeepAspect.Checked && origH > 0)
                {
                    int h = (int)((double)w / origW * origH);
                    txtHeight.Text = h.ToString();
                }
                double pct = (double)w / origW * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void txtHeight_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents) return;
            int origW = GetOriginalWidth();
            int origH = GetOriginalHeight();
            if (origH <= 0) return;
            if (int.TryParse(txtHeight.Text, out int h) && h > 0)
            {
                suppressEvents = true;
                if (chkKeepAspect.Checked && origW > 0)
                {
                    int w = (int)((double)h / origH * origW);
                    txtWidth.Text = w.ToString();
                }
                double pct = (double)h / origH * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void txtPercent_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents) return;
            int origW = GetOriginalWidth();
            int origH = GetOriginalHeight();
            if (origW <= 0 || origH <= 0) return;

            if (double.TryParse(txtPercent.Text, out double pct) && pct > 0)
            {
                suppressEvents = true;
                txtWidth.Text = ((int)(origW * pct / 100)).ToString();
                txtHeight.Text = ((int)(origH * pct / 100)).ToString();
                suppressEvents = false;
            }
        }

        private void cmbPreset_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (suppressEvents) return;
            int origW = GetOriginalWidth();
            int origH = GetOriginalHeight();
            if (origW <= 0 || origH <= 0) return;

            int idx = cmbPreset.SelectedIndex;
            suppressEvents = true;

            switch (idx)
            {
                case 1: txtPercent.Text = "50"; break;
                case 2: txtPercent.Text = "25"; break;
                case 3: txtPercent.Text = "10"; break;
                case 4: txtWidth.Text = "160"; txtHeight.Text = "120"; break;
                case 5: txtWidth.Text = "320"; txtHeight.Text = "240"; break;
                case 6: txtWidth.Text = "640"; txtHeight.Text = "480"; break;
                case 7: txtWidth.Text = "800"; txtHeight.Text = "600"; break;
                case 8: txtWidth.Text = "1024"; txtHeight.Text = "768"; break;
            }

            suppressEvents = false;
        }

        private int GetOriginalWidth()
        {
            if (magickCollection != null && gifFrameCount > 0)
                return (int)((MagickImage)magickCollection[0]).Page.Width;
            if (magickImage != null) return (int)magickImage.Width;
            return 0;
        }

        private int GetOriginalHeight()
        {
            if (magickCollection != null && gifFrameCount > 0)
                return (int)((MagickImage)magickCollection[0]).Page.Height;
            if (magickImage != null) return (int)magickImage.Height;
            return 0;
        }

        private void BtnResize_Click(object? sender, EventArgs e)
        {
            if (magickImage == null)
            {
                MessageBox.Show("No hay imagen cargada.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int w) || !int.TryParse(txtHeight.Text, out int h) || w <= 0 || h <= 0)
            {
                MessageBox.Show("Ancho y altura deben ser números válidos.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isGif && magickCollection != null && gifFrameCount > 1)
                ResizeAnimatedGif(w, h);
            else
                ResizeStaticImage(w, h);
        }

        private void ResizeStaticImage(int w, int h)
        {
            if (magickImage == null) return;

            resizedBmp?.Dispose();
            using var clone = (MagickImage)magickImage.Clone();
            ApplyResizeMagickInPlace(clone, w, h);
            resizedBmp = clone.ToBitmap();

            btnSave.Enabled = true;
            ShowResultPanel(w, h);
        }

        private void ResizeAnimatedGif(int w, int h)
        {
            if (magickCollection == null) return;

            try
            {
                using var collection = (MagickImageCollection)magickCollection.Clone();
                collection.Coalesce();

                foreach (IMagickImage<byte> frame in collection)
                {
                    ApplyResizeMagickInPlace((MagickImage)frame, w, h);
                }

                var resultPath = Path.Combine(Path.GetTempPath(), $"resize_result_{Guid.NewGuid():N}.gif");
                collection.Write(resultPath);

                resizedBmp?.Dispose();
                using var resultMagick = new MagickImage(resultPath);
                resizedBmp = resultMagick.ToBitmap();

                btnSave.Enabled = true;
                ShowResultPanel(w, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error redimensionando GIF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyResizeMagickInPlace(MagickImage img, int targetW, int targetH)
        {
            int method = cmbMethod.SelectedIndex;
            int aspect = cmbAspect.SelectedIndex;

            var filterType = method switch
            {
                1 => FilterType.Triangle,
                2 => FilterType.Point,
                _ => FilterType.Lanczos
            };

            if (aspect == 1)
            {
                var geo = new MagickGeometry((uint)targetW, (uint)targetH) { IgnoreAspectRatio = true };
                img.Resize(geo, filterType);
            }
            else if (aspect == 2)
            {
                var geo = new MagickGeometry((uint)targetW, (uint)targetH) { IgnoreAspectRatio = false };
                img.Resize(geo, filterType);
            }
            else if (aspect == 3)
            {
                var geo = new MagickGeometry($"{targetW}x{targetH}^");
                img.Resize(geo, filterType);
                img.Crop((uint)targetW, (uint)targetH, Gravity.Center);
                img.Page = new MagickGeometry(0, 0, (uint)targetW, (uint)targetH);
            }
            else
            {
                var geo = new MagickGeometry((uint)targetW, (uint)targetH) { IgnoreAspectRatio = false };
                img.Resize(geo, filterType);
            }
        }

        private void ShowResultPanel(int w, int h)
        {
            picResult.Image?.Dispose();
            picResult.Image = resizedBmp != null ? new Bitmap(resizedBmp) : null;

            string ext = Path.GetExtension(currentFilePath).ToLower();
            long origSize = currentFilePath.Length > 0 && File.Exists(currentFilePath)
                ? new FileInfo(currentFilePath).Length : 0;
            string origSizeStr = origSize > 1024 ? $"{origSize / 1024.0:F2}KiB" : $"{origSize}B";

            long newSize = 0;
            if (resizedBmp != null)
            {
                using var ms = new MemoryStream();
                resizedBmp.Save(ms, ImageFormat.Png);
                newSize = ms.Length;
            }
            string newSizeStr = newSize > 1024 ? $"{newSize / 1024.0:F2}KiB" : $"{newSize}B";
            double reduction = origSize > 0 ? (1.0 - (double)newSize / origSize) * 100 : 0;
            string reductionStr = reduction > 0 ? $"( {reduction:F1}% )" : "";

            lblResultInfo.Text = $"Tamaño del archivo: {newSizeStr} {reductionStr}  ancho: {w}px, altura: {h}px, tipo: {ext.TrimStart('.')}";
            panelResult.Visible = true;
        }

        private void BtnCrop_Click(object? sender, EventArgs e)
        {
            if (currentFilePath.Length == 0 || !File.Exists(currentFilePath))
            {
                MessageBox.Show("No hay imagen cargada.", "Recortar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var cropForm = new CropForm(currentFilePath);
            if (cropForm.ShowDialog(this) == DialogResult.OK && cropForm.Tag is CropResult result)
            {
                Cleanup();
                currentFilePath = result.ResultPath;
                isGif = result.IsGif;

                if (isGif)
                {
                    magickCollection = new MagickImageCollection();
                    magickCollection.Read(result.ResultPath);
                    gifFrameCount = magickCollection.Count;
                    gifFrameDelays = new int[gifFrameCount];
                    for (int i = 0; i < gifFrameCount; i++)
                        gifFrameDelays[i] = Math.Max(1, (int)magickCollection[i].AnimationDelay);
                }

                magickImage = new MagickImage(result.ResultPath);

                previewImage?.Dispose();
                previewImage = Image.FromFile(result.ResultPath);
                picPreview.Image = previewImage;

                long size = new FileInfo(result.ResultPath).Length;
                string sizeStr = size > 1024 * 1024 ? $"{size / (1024.0 * 1024.0):F2}MiB" : $"{size / 1024.0:F2}KiB";
                string info = $"Tamaño: {sizeStr}, ancho: {result.Width}px, altura: {result.Height}px";
                if (isGif) info += $", fotogramas: {gifFrameCount}, tipo: gif";
                lblFileInfo.Text = info;

                txtWidth.Text = result.Width.ToString();
                txtHeight.Text = result.Height.ToString();
                suppressEvents = true;
                txtPercent.Text = "100";
                suppressEvents = false;

                btnSave.Enabled = false;
                panelResult.Visible = false;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (resizedBmp == null)
            {
                MessageBox.Show("No hay imagen redimensionada para guardar.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog();
            dlg.Filter = isGif ? "GIF|*.gif|PNG|*.png|Todos|*.*" : "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|Todos|*.*";
            dlg.FileName = Path.GetFileNameWithoutExtension(currentFilePath) + "_resized";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(dlg.FileName).ToLower();
                if (ext == ".gif" && isGif && magickCollection != null && gifFrameCount > 1)
                    SaveResizedAnimatedGif(dlg.FileName);
                else
                {
                    var format = ext switch
                    {
                        ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                        ".bmp" => ImageFormat.Bmp,
                        ".gif" => ImageFormat.Gif,
                        _ => ImageFormat.Png
                    };
                    resizedBmp.Save(dlg.FileName, format);
                }
                lblFileInfo.Text = $"Guardado: {dlg.FileName}";
            }
        }

        private void SaveResizedAnimatedGif(string path)
        {
            if (magickCollection == null || !int.TryParse(txtWidth.Text, out int w) || !int.TryParse(txtHeight.Text, out int h))
                return;

            try
            {
                using var collection = (MagickImageCollection)magickCollection.Clone();
                collection.Coalesce();
                foreach (IMagickImage<byte> frame in collection)
                    ApplyResizeMagickInPlace((MagickImage)frame, w, h);
                collection.Write(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando GIF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Cleanup()
        {
            magickImage?.Dispose();
            magickImage = null;
            magickCollection?.Dispose();
            magickCollection = null;
            previewImage?.Dispose();
            previewImage = null;
            gifFrameDelays = null;
            gifFrameCount = 0;
            isGif = false;
            originalGifPath = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cleanup();
            resizedBmp?.Dispose();
            picResult.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
