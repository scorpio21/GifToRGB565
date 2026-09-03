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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CropForm));
            panelTop = new Panel();
            lblTitle = new Label();
            picPreview = new PictureBox();
            panelBottom = new Panel();
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
            lblInfo = new Label();
            btnCrop = new Button();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            panelBottom.SuspendLayout();
            panelCrop.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(50, 50, 60);
            panelTop.Controls.Add(lblTitle);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(780, 40);
            panelTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(780, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Recorte de imagen";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.Black;
            picPreview.BorderStyle = BorderStyle.FixedSingle;
            picPreview.Dock = DockStyle.Fill;
            picPreview.Location = new Point(0, 40);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(780, 400);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 0;
            picPreview.TabStop = false;
            picPreview.Paint += PicPreview_Paint;
            picPreview.MouseDown += PicPreview_MouseDown;
            picPreview.MouseMove += PicPreview_MouseMove;
            picPreview.MouseUp += PicPreview_MouseUp;
            // 
            // panelBottom
            // 
            panelBottom.BackColor = Color.FromArgb(35, 35, 45);
            panelBottom.Controls.Add(panelCrop);
            panelBottom.Controls.Add(lblInfo);
            panelBottom.Controls.Add(btnCrop);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 440);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(780, 140);
            panelBottom.TabIndex = 2;
            // 
            // panelCrop
            // 
            panelCrop.BackColor = Color.FromArgb(45, 45, 55);
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
            panelCrop.Location = new Point(10, 30);
            panelCrop.Name = "panelCrop";
            panelCrop.Size = new Size(750, 55);
            panelCrop.TabIndex = 1;
            // 
            // lblLeft
            // 
            lblLeft.AutoSize = true;
            lblLeft.ForeColor = Color.White;
            lblLeft.Location = new Point(5, 8);
            lblLeft.Name = "lblLeft";
            lblLeft.Size = new Size(58, 15);
            lblLeft.TabIndex = 0;
            lblLeft.Text = "Izquierda:";
            // 
            // txtLeft
            // 
            txtLeft.Location = new Point(80, 5);
            txtLeft.Name = "txtLeft";
            txtLeft.Size = new Size(60, 23);
            txtLeft.TabIndex = 1;
            txtLeft.TextChanged += TxtFields_TextChanged;
            // 
            // lblTop
            // 
            lblTop.AutoSize = true;
            lblTop.ForeColor = Color.White;
            lblTop.Location = new Point(155, 8);
            lblTop.Name = "lblTop";
            lblTop.Size = new Size(42, 15);
            lblTop.TabIndex = 2;
            lblTop.Text = "Arriba:";
            // 
            // txtTop
            // 
            txtTop.Location = new Point(210, 5);
            txtTop.Name = "txtTop";
            txtTop.Size = new Size(60, 23);
            txtTop.TabIndex = 3;
            txtTop.TextChanged += TxtFields_TextChanged;
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.ForeColor = Color.White;
            lblWidth.Location = new Point(290, 8);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(45, 15);
            lblWidth.TabIndex = 4;
            lblWidth.Text = "Ancho:";
            // 
            // txtWidth
            // 
            txtWidth.Location = new Point(345, 5);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(60, 23);
            txtWidth.TabIndex = 5;
            txtWidth.TextChanged += TxtFields_TextChanged;
            // 
            // lblHeight
            // 
            lblHeight.AutoSize = true;
            lblHeight.ForeColor = Color.White;
            lblHeight.Location = new Point(420, 8);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(42, 15);
            lblHeight.TabIndex = 6;
            lblHeight.Text = "Altura:";
            // 
            // txtHeight
            // 
            txtHeight.Location = new Point(475, 5);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(60, 23);
            txtHeight.TabIndex = 7;
            txtHeight.TextChanged += TxtFields_TextChanged;
            // 
            // lblAspectLock
            // 
            lblAspectLock.AutoSize = true;
            lblAspectLock.ForeColor = Color.White;
            lblAspectLock.Location = new Point(5, 35);
            lblAspectLock.Name = "lblAspectLock";
            lblAspectLock.Size = new Size(178, 15);
            lblAspectLock.TabIndex = 8;
            lblAspectLock.Text = "Relación de aspecto de bloqueo:";
            // 
            // cmbAspectLock
            // 
            cmbAspectLock.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAspectLock.Items.AddRange(new object[] { "No", "1:1", "4:3", "16:9", "3:2", "Libre" });
            cmbAspectLock.Location = new Point(210, 32);
            cmbAspectLock.Name = "cmbAspectLock";
            cmbAspectLock.Size = new Size(100, 23);
            cmbAspectLock.TabIndex = 9;
            // 
            // chkAutocrop
            // 
            chkAutocrop.AutoSize = true;
            chkAutocrop.ForeColor = Color.White;
            chkAutocrop.Location = new Point(330, 35);
            chkAutocrop.Name = "chkAutocrop";
            chkAutocrop.Size = new Size(234, 19);
            chkAutocrop.TabIndex = 10;
            chkAutocrop.Text = "Autocorte: recorta píxeles transparentes";
            // 
            // lblInfo
            // 
            lblInfo.Dock = DockStyle.Top;
            lblInfo.ForeColor = Color.LightGray;
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Padding = new Padding(10, 5, 0, 0);
            lblInfo.Size = new Size(780, 25);
            lblInfo.TabIndex = 2;
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCrop
            // 
            btnCrop.BackColor = Color.FromArgb(0, 120, 215);
            btnCrop.FlatStyle = FlatStyle.Flat;
            btnCrop.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCrop.ForeColor = Color.White;
            btnCrop.Image = Properties.Resources.icon_crop;
            btnCrop.ImageAlign = ContentAlignment.MiddleLeft;
            btnCrop.Location = new Point(10, 100);
            btnCrop.Name = "btnCrop";
            btnCrop.Size = new Size(180, 32);
            btnCrop.TabIndex = 3;
            btnCrop.Text = "¡Recorta la imagen!";
            btnCrop.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCrop.UseVisualStyleBackColor = false;
            btnCrop.Click += BtnCrop_Click;
            // 
            // CropForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 580);
            Controls.Add(picPreview);
            Controls.Add(panelTop);
            Controls.Add(panelBottom);
            Icon = (Icon)resources.GetObject("$this.Icon");
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
