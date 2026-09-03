namespace GifRGB565GUI
{
    partial class CropForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                previewImage?.Dispose();
                magickImage?.Dispose();
                magickCollection?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelTop = new Panel();
            lblTitle = new Label();
            picPreview = new PictureBox();
            panelBottom = new Panel();
            lblInfo = new Label();
            panelCrop = new Panel();
            lblLeft = new Label();
            txtLeft = new TextBox();
            lblTop = new Label();
            txtTop = new TextBox();
            lblWidth = new Label();
            txtWidth = new TextBox();
            lblHeight = new Label();
            txtHeight = new TextBox();
            lblAspectLock = new Label();
            cmbAspectLock = new ComboBox();
            chkAutocrop = new CheckBox();
            btnCrop = new Button();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            panelBottom.SuspendLayout();
            panelCrop.SuspendLayout();
            SuspendLayout();

            panelTop.BackColor = Color.FromArgb(50, 50, 60);
            panelTop.Controls.Add(lblTitle);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(780, 40);
            panelTop.TabIndex = 0;

            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(780, 40);
            lblTitle.Text = "Recorte de imagen";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            picPreview.BackColor = Color.Black;
            picPreview.BorderStyle = BorderStyle.FixedSingle;
            picPreview.Dock = DockStyle.Fill;
            picPreview.Location = new Point(0, 40);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(780, 400);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabStop = false;
            picPreview.Paint += PicPreview_Paint;
            picPreview.MouseDown += PicPreview_MouseDown;
            picPreview.MouseMove += PicPreview_MouseMove;
            picPreview.MouseUp += PicPreview_MouseUp;

            panelBottom.BackColor = Color.FromArgb(35, 35, 45);
            panelBottom.Controls.Add(panelCrop);
            panelBottom.Controls.Add(lblInfo);
            panelBottom.Controls.Add(btnCrop);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 440);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(780, 140);
            panelBottom.TabIndex = 2;

            lblInfo.Dock = DockStyle.Top;
            lblInfo.ForeColor = Color.LightGray;
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new Padding(10, 5, 0, 0);
            lblInfo.Size = new Size(780, 25);
            lblInfo.Text = "";
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;

            panelCrop.BackColor = Color.FromArgb(45, 45, 55);
            panelCrop.Location = new Point(10, 30);
            panelCrop.Name = "panelCrop";
            panelCrop.Size = new Size(750, 55);
            panelCrop.TabIndex = 1;

            lblLeft.AutoSize = true;
            lblLeft.ForeColor = Color.White;
            lblLeft.Location = new Point(5, 8);
            lblLeft.Text = "Izquierda:";

            txtLeft.Location = new Point(80, 5);
            txtLeft.Size = new Size(60, 23);
            txtLeft.TextChanged += TxtFields_TextChanged;

            lblTop.AutoSize = true;
            lblTop.ForeColor = Color.White;
            lblTop.Location = new Point(155, 8);
            lblTop.Text = "Arriba:";

            txtTop.Location = new Point(210, 5);
            txtTop.Size = new Size(60, 23);
            txtTop.TextChanged += TxtFields_TextChanged;

            lblWidth.AutoSize = true;
            lblWidth.ForeColor = Color.White;
            lblWidth.Location = new Point(290, 8);
            lblWidth.Text = "Ancho:";

            txtWidth.Location = new Point(345, 5);
            txtWidth.Size = new Size(60, 23);
            txtWidth.TextChanged += TxtFields_TextChanged;

            lblHeight.AutoSize = true;
            lblHeight.ForeColor = Color.White;
            lblHeight.Location = new Point(420, 8);
            lblHeight.Text = "Altura:";

            txtHeight.Location = new Point(475, 5);
            txtHeight.Size = new Size(60, 23);
            txtHeight.TextChanged += TxtFields_TextChanged;

            lblAspectLock.AutoSize = true;
            lblAspectLock.ForeColor = Color.White;
            lblAspectLock.Location = new Point(5, 35);
            lblAspectLock.Text = "Relación de aspecto de bloqueo:";

            cmbAspectLock.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAspectLock.Items.AddRange(new object[] { "No", "1:1", "4:3", "16:9", "3:2", "自由" });
            cmbAspectLock.Location = new Point(210, 32);
            cmbAspectLock.Size = new Size(100, 23);
            cmbAspectLock.SelectedIndex = 0;

            chkAutocrop.AutoSize = true;
            chkAutocrop.ForeColor = Color.White;
            chkAutocrop.Location = new Point(330, 35);
            chkAutocrop.Text = "Autocorte: recorta píxeles transparentes";

            panelCrop.Controls.Add(lblLeft);
            panelCrop.Controls.Add(txtLeft);
            panelCrop.Controls.Add(lblTop);
            panelCrop.Controls.Add(txtTop);
            panelCrop.Controls.Add(lblWidth);
            panelCrop.Controls.Add(txtWidth);
            panelCrop.Controls.Add(lblHeight);
            panelCrop.Controls.Add(txtHeight);
            panelCrop.Controls.Add(lblAspectLock);
            panelCrop.Controls.Add(cmbAspectLock);
            panelCrop.Controls.Add(chkAutocrop);

            btnCrop.BackColor = Color.FromArgb(0, 120, 215);
            btnCrop.FlatStyle = FlatStyle.Flat;
            btnCrop.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCrop.ForeColor = Color.White;
            btnCrop.Location = new Point(10, 100);
            btnCrop.Size = new Size(180, 32);
            btnCrop.Text = "¡Recorta la imagen!";
            btnCrop.UseVisualStyleBackColor = false;
            btnCrop.Click += BtnCrop_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 580);
            Controls.Add(picPreview);
            Controls.Add(panelTop);
            Controls.Add(panelBottom);
            MinimumSize = new Size(600, 450);
            Name = "CropForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Recorte de imagen";

            panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            panelBottom.ResumeLayout(false);
            panelCrop.ResumeLayout(false);
            panelCrop.PerformLayout();
            ResumeLayout(false);
        }

        private Panel panelTop;
        private Label lblTitle;
        private PictureBox picPreview;
        private Panel panelBottom;
        private Label lblInfo;
        private Panel panelCrop;
        private Label lblLeft;
        private TextBox txtLeft;
        private Label lblTop;
        private TextBox txtTop;
        private Label lblWidth;
        private TextBox txtWidth;
        private Label lblHeight;
        private TextBox txtHeight;
        private Label lblAspectLock;
        private ComboBox cmbAspectLock;
        private CheckBox chkAutocrop;
        private Button btnCrop;
    }
}
