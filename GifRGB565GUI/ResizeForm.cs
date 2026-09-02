using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    public class ResizeForm : Form
    {
        private PictureBox picPreview;
        private Label lblFileInfo;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private TextBox txtPercent;
        private ComboBox cmbMethod;
        private ComboBox cmbAspect;
        private Button btnResize;
        private Button btnSave;
        private Button btnCrop;
        private CheckBox chkRemember;
        private Panel panelToolbar;
        private Panel panelOptions;

        private Label lblResultTitle;
        private PictureBox picResult;
        private Label lblResultInfo;
        private Panel panelResult;

        private Bitmap? originalBmp;
        private Bitmap? resizedBmp;
        private string currentFilePath = "";
        private int originalWidth;
        private int originalHeight;

        public ResizeForm()
        {
            Text = "Redimensionar imágenes";
            Size = new Size(750, 850);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(650, 600);
            AllowDrop = true;
            DragEnter += ResizeForm_DragEnter;
            DragDrop += ResizeForm_DragDrop;

            // Toolbar
            panelToolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(50, 50, 60) };
            var btnOpen = MakeToolButton("Abrir imagen", 10);
            btnOpen.Click += (s, e) => OpenImage();
            panelToolbar.Controls.Add(btnOpen);

            // Preview
            picPreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };

            // File info
            lblFileInfo = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Padding = new Padding(5, 0, 0, 0),
                Text = "Arrastra una imagen aquí o haz clic en 'Abrir imagen'"
            };

            // Result panel (below options, like ezgif "Imagen redimensionada:")
            panelResult = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 160,
                BackColor = Color.FromArgb(35, 35, 45),
                Padding = new Padding(15),
                Visible = false
            };

            lblResultTitle = new Label
            {
                Text = "Imagen redimensionada:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            picResult = new PictureBox
            {
                Location = new Point(15, 30),
                Size = new Size(64, 64),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };

            lblResultInfo = new Label
            {
                Location = new Point(90, 35),
                AutoSize = false,
                Size = new Size(580, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Text = ""
            };

            panelResult.Controls.Add(lblResultTitle);
            panelResult.Controls.Add(picResult);
            panelResult.Controls.Add(lblResultInfo);

            // Options panel
            panelOptions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 210,
                BackColor = Color.FromArgb(35, 35, 45),
                Padding = new Padding(15)
            };

            // Width
            var lblW = new Label { Text = "↔ Ancho:", AutoSize = true, Location = new Point(15, 15), ForeColor = Color.White };
            txtWidth = new TextBox { Location = new Point(95, 12), Width = 80 };
            var lblWHint = new Label { Text = "(Vacío = automático)", AutoSize = true, Location = new Point(185, 15), ForeColor = Color.Gray };

            // Height
            var lblH = new Label { Text = "↑ Altura:", AutoSize = true, Location = new Point(15, 45), ForeColor = Color.White };
            txtHeight = new TextBox { Location = new Point(95, 42), Width = 80 };
            var lblHHint = new Label { Text = "(Vacío = automático)", AutoSize = true, Location = new Point(185, 45), ForeColor = Color.Gray };

            // Percentage
            var lblPct = new Label { Text = "Porcentaje:", AutoSize = true, Location = new Point(15, 75), ForeColor = Color.White };
            txtPercent = new TextBox { Location = new Point(95, 72), Width = 80 };

            // Resize method
            var lblM = new Label { Text = "Método de redimensionamiento:", AutoSize = true, Location = new Point(15, 105), ForeColor = Color.White };
            cmbMethod = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(15, 125),
                Width = 350,
                Items = {
                    "Redimensionar (Bicúbica, calidad)",
                    "Redimensionar (Bilineal)",
                    "Redimensionar (Vecino cercano, rápido)",
                    "Centrar y recortar para ajustarse",
                    "Estirar para ajustarse",
                    "Fuerza la relación de aspecto original",
                    "Añadir relleno transparente"
                },
                SelectedIndex = 0
            };

            // Aspect ratio
            var lblA = new Label { Text = "Si la relación de aspecto no coincide:", AutoSize = true, Location = new Point(400, 105), ForeColor = Color.White };
            cmbAspect = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(400, 125),
                Width = 250,
                Items = {
                    "Centro y recorte para ajustarse",
                    "Estirar para ajustarse",
                    "Fuerza la relación de aspecto original",
                    "Añadir relleno transparente"
                },
                SelectedIndex = 0
            };

            // Buttons
            btnResize = new Button
            {
                Text = "¡Redimensiona la imagen!",
                Location = new Point(15, 165),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnResize.Click += BtnResize_Click;

            btnCrop = new Button
            {
                Text = "Cortar",
                Location = new Point(210, 165),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };

            btnSave = new Button
            {
                Text = "Guardar",
                Location = new Point(320, 165),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSave.Click += BtnSave_Click;

            chkRemember = new CheckBox
            {
                Text = "Recuerda los ajustes",
                AutoSize = true,
                Location = new Point(440, 172),
                ForeColor = Color.White
            };

            // Wire events — Width/Height only update Percentage (no auto-linked)
            txtWidth.TextChanged += TxtWidth_TextChanged;
            txtHeight.TextChanged += TxtHeight_TextChanged;
            txtPercent.TextChanged += TxtPercent_TextChanged;

            // Add controls to options panel
            panelOptions.Controls.AddRange(new Control[] {
                lblW, txtWidth, lblWHint,
                lblH, txtHeight, lblHHint,
                lblPct, txtPercent,
                lblM, cmbMethod,
                lblA, cmbAspect,
                btnResize, btnCrop, btnSave, chkRemember
            });

            // Add controls to form (order matters for docking)
            Controls.Add(picPreview);
            Controls.Add(panelToolbar);
            Controls.Add(lblFileInfo);
            Controls.Add(panelResult);
            Controls.Add(panelOptions);
        }

        private Button MakeToolButton(string text, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 8),
                Size = new Size(120, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 70),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
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

        private void OpenImage()
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
                originalBmp?.Dispose();
                originalBmp = new Bitmap(path);
                currentFilePath = path;
                originalWidth = originalBmp.Width;
                originalHeight = originalBmp.Height;
                picPreview.Image = originalBmp;

                string ext = Path.GetExtension(path).ToLower();
                long size = new FileInfo(path).Length;
                string sizeStr = size > 1024 * 1024 ? $"{size / (1024.0 * 1024.0):F2}MiB" : $"{size / 1024.0:F2}KiB";

                lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {originalWidth}px, altura: {originalHeight}px, tipo: {ext.TrimStart('.')}";

                suppressEvents = true;
                txtWidth.Text = originalWidth.ToString();
                txtHeight.Text = originalHeight.ToString();
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

        private bool suppressEvents = false;

        private void TxtWidth_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents || originalBmp == null) return;
            if (int.TryParse(txtWidth.Text, out int w) && w > 0)
            {
                suppressEvents = true;
                double pct = (double)w / originalWidth * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void TxtHeight_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents || originalBmp == null) return;
            if (int.TryParse(txtHeight.Text, out int h) && h > 0)
            {
                suppressEvents = true;
                double pct = (double)h / originalHeight * 100;
                txtPercent.Text = ((int)pct).ToString();
                suppressEvents = false;
            }
        }

        private void TxtPercent_TextChanged(object? sender, EventArgs e)
        {
            if (suppressEvents || originalBmp == null) return;
            if (double.TryParse(txtPercent.Text, out double pct) && pct > 0)
            {
                suppressEvents = true;
                txtWidth.Text = ((int)(originalWidth * pct / 100)).ToString();
                txtHeight.Text = ((int)(originalHeight * pct / 100)).ToString();
                suppressEvents = false;
            }
        }

        private void BtnResize_Click(object? sender, EventArgs e)
        {
            if (originalBmp == null)
            {
                MessageBox.Show("No hay imagen cargada.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int w) || !int.TryParse(txtHeight.Text, out int h) || w <= 0 || h <= 0)
            {
                MessageBox.Show("Ancho y altura deben ser números válidos.", "Redimensionar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var method = cmbMethod.SelectedIndex;
            resizedBmp?.Dispose();

            if (method <= 2)
            {
                resizedBmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(resizedBmp);
                g.InterpolationMode = method switch
                {
                    1 => InterpolationMode.HighQualityBilinear,
                    2 => InterpolationMode.NearestNeighbor,
                    _ => InterpolationMode.HighQualityBicubic
                };
                g.DrawImage(originalBmp, 0, 0, w, h);
            }
            else if (method == 3) // Centrar y recortar
            {
                double scaleX = (double)w / originalBmp.Width;
                double scaleY = (double)h / originalBmp.Height;
                double scale = Math.Max(scaleX, scaleY);
                int sw = (int)(originalBmp.Width * scale);
                int sh = (int)(originalBmp.Height * scale);
                resizedBmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(resizedBmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalBmp, (w - sw) / 2, (h - sh) / 2, sw, sh);
            }
            else if (method == 4) // Estirar
            {
                resizedBmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(resizedBmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalBmp, 0, 0, w, h);
            }
            else if (method == 5) // Forzar aspect ratio
            {
                double ratio = (double)originalBmp.Width / originalBmp.Height;
                int nw = w;
                int nh = (int)(w / ratio);
                if (nh > h) { nh = h; nw = (int)(h * ratio); }
                resizedBmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(resizedBmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                g.DrawImage(originalBmp, (w - nw) / 2, (h - nh) / 2, nw, nh);
            }
            else if (method == 6) // Relleno transparente
            {
                resizedBmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(resizedBmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                int ox = (w - originalBmp.Width) / 2;
                int oy = (h - originalBmp.Height) / 2;
                g.DrawImage(originalBmp, ox, oy, originalBmp.Width, originalBmp.Height);
            }

            picPreview.Image = resizedBmp;
            btnSave.Enabled = true;

            // Show result panel like ezgif
            picResult.Image?.Dispose();
            picResult.Image = resizedBmp != null ? new Bitmap(resizedBmp) : null;

            string ext = Path.GetExtension(currentFilePath).ToLower();
            long origSize = currentFilePath.Length > 0 ? new FileInfo(currentFilePath).Length : 0;
            string origSizeStr = origSize > 1024 ? $"{origSize / 1024.0:F2}KiB" : $"{origSize}B";

            using var ms = new MemoryStream();
            resizedBmp?.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
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
                    ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                    _ => System.Drawing.Imaging.ImageFormat.Png
                };
                resizedBmp.Save(dlg.FileName, format);
                lblFileInfo.Text = $"Guardado: {dlg.FileName}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            originalBmp?.Dispose();
            resizedBmp?.Dispose();
            picResult.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
