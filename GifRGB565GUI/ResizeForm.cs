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
        private Label lblWidth;
        private Label lblHeight;
        private Label lblPercent;
        private Label lblMethodTitle;
        private Label lblAspectTitle;

        private Bitmap? originalBmp;
        private Bitmap? resizedBmp;
        private string currentFilePath = "";

        public ResizeForm()
        {
            Text = "Redimensionar imágenes";
            Size = new Size(700, 750);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 500);
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

            // Options panel
            panelOptions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 220,
                BackColor = Color.FromArgb(35, 35, 45),
                Padding = new Padding(15)
            };

            // Width
            lblWidth = new Label { Text = "↔ Ancho:", AutoSize = true, Location = new Point(15, 15), ForeColor = Color.White };
            txtWidth = new TextBox { Location = new Point(95, 12), Width = 80 };
            var lblWidthHint = new Label { Text = "(Vacío = automático)", AutoSize = true, Location = new Point(185, 15), ForeColor = Color.Gray };

            // Height
            lblHeight = new Label { Text = "↑ Altura:", AutoSize = true, Location = new Point(15, 45), ForeColor = Color.White };
            txtHeight = new TextBox { Location = new Point(95, 42), Width = 80 };
            var lblHeightHint = new Label { Text = "(Vacío = automático)", AutoSize = true, Location = new Point(185, 45), ForeColor = Color.Gray };

            // Percentage
            lblPercent = new Label { Text = "Porcentaje:", AutoSize = true, Location = new Point(15, 75), ForeColor = Color.White };
            txtPercent = new TextBox { Location = new Point(95, 72), Width = 80 };

            // Resize method
            lblMethodTitle = new Label { Text = "Método de redimensionamiento:", AutoSize = true, Location = new Point(15, 110), ForeColor = Color.White };
            cmbMethod = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(15, 130),
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
            lblAspectTitle = new Label { Text = "Si la relación de aspecto no coincide:", AutoSize = true, Location = new Point(400, 110), ForeColor = Color.White };
            cmbAspect = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(400, 130),
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
                Location = new Point(15, 170),
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
                Location = new Point(210, 170),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCrop.Enabled = false;

            btnSave = new Button
            {
                Text = "Guardar",
                Location = new Point(320, 170),
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
                Location = new Point(440, 177),
                ForeColor = Color.White
            };

            // Wire events
            txtWidth.TextChanged += TxtWidth_TextChanged;
            txtHeight.TextChanged += TxtHeight_TextChanged;
            txtPercent.TextChanged += TxtPercent_TextChanged;

            // Add controls to options panel
            panelOptions.Controls.AddRange(new Control[] {
                lblWidth, txtWidth, lblWidthHint,
                lblHeight, txtHeight, lblHeightHint,
                lblPercent, txtPercent,
                lblMethodTitle, cmbMethod,
                lblAspectTitle, cmbAspect,
                btnResize, btnCrop, btnSave, chkRemember
            });

            // Add controls to form
            Controls.Add(picPreview);
            Controls.Add(panelToolbar);
            Controls.Add(lblFileInfo);
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
                picPreview.Image = originalBmp;

                string ext = Path.GetExtension(path).ToLower();
                long size = new FileInfo(path).Length;
                string sizeStr = size > 1024 * 1024 ? $"{size / (1024.0 * 1024.0):F2}MiB" : $"{size / 1024.0:F2}KiB";

                lblFileInfo.Text = $"Tamaño del archivo: {sizeStr}, ancho: {originalBmp.Width}px, altura: {originalBmp.Height}px, tipo: {ext.TrimStart('.')}";

                txtWidth.Text = originalBmp.Width.ToString();
                txtHeight.Text = originalBmp.Height.ToString();
                txtPercent.Text = "100";

                btnSave.Enabled = false;
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
                double ratio = (double)originalBmp.Height / originalBmp.Width;
                txtHeight.Text = ((int)(w * ratio)).ToString();
                double pct = (double)w / originalBmp.Width * 100;
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
                double ratio = (double)originalBmp.Width / originalBmp.Height;
                txtWidth.Text = ((int)(h * ratio)).ToString();
                double pct = (double)h / originalBmp.Height * 100;
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
                txtWidth.Text = ((int)(originalBmp.Width * pct / 100)).ToString();
                txtHeight.Text = ((int)(originalBmp.Height * pct / 100)).ToString();
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

            if (method <= 2) // Redimensionar con distintos métodos
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

            string ext = Path.GetExtension(currentFilePath).ToLower();
            lblFileInfo.Text = $"Redimensionado: {w}x{h} | Método: {cmbMethod.SelectedItem} | Tipo: {ext.TrimStart('.')}";

            Log($"Imagen redimensionada: {w}x{h} ({cmbMethod.SelectedItem})");
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
                Log($"Imagen guardada: {dlg.FileName}");
            }
        }

        private void Log(string msg)
        {
            lblFileInfo.Text = msg;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            originalBmp?.Dispose();
            resizedBmp?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
