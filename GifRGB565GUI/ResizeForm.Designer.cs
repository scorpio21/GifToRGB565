namespace GifRGB565GUI
{
    partial class ResizeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelToolbar = new Panel();
            btnOpen = new Button();
            picPreview = new PictureBox();
            lblFileInfo = new Label();
            panelResult = new Panel();
            lblResultTitle = new Label();
            picResult = new PictureBox();
            lblResultInfo = new Label();
            panelOptions = new Panel();
            lblWidth = new Label();
            txtWidth = new TextBox();
            lblWidthHint = new Label();
            lblHeight = new Label();
            txtHeight = new TextBox();
            lblHeightHint = new Label();
            lblPercent = new Label();
            txtPercent = new TextBox();
            lblMethodTitle = new Label();
            cmbMethod = new ComboBox();
            lblAspectTitle = new Label();
            cmbAspect = new ComboBox();
            btnResize = new Button();
            btnCrop = new Button();
            btnSave = new Button();
            chkRemember = new CheckBox();

            panelToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            panelResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picResult).BeginInit();
            panelOptions.SuspendLayout();
            SuspendLayout();

            // panelToolbar
            panelToolbar.BackColor = Color.FromArgb(50, 50, 60);
            panelToolbar.Controls.Add(btnOpen);
            panelToolbar.Dock = DockStyle.Top;
            panelToolbar.Location = new Point(0, 0);
            panelToolbar.Name = "panelToolbar";
            panelToolbar.Size = new Size(720, 50);
            panelToolbar.TabIndex = 0;

            // btnOpen
            btnOpen.BackColor = Color.FromArgb(60, 60, 70);
            btnOpen.FlatStyle = FlatStyle.Flat;
            btnOpen.ForeColor = Color.White;
            btnOpen.Location = new Point(10, 8);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(120, 34);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Abrir imagen";
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += BtnOpen_Click;

            // picPreview
            picPreview.BackColor = Color.Black;
            picPreview.BorderStyle = BorderStyle.FixedSingle;
            picPreview.Dock = DockStyle.Fill;
            picPreview.Name = "picPreview";
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;

            // lblFileInfo
            lblFileInfo.BackColor = Color.FromArgb(40, 40, 50);
            lblFileInfo.Dock = DockStyle.Bottom;
            lblFileInfo.ForeColor = Color.White;
            lblFileInfo.Location = new Point(0, 530);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Padding = new Padding(5, 0, 0, 0);
            lblFileInfo.Size = new Size(720, 25);
            lblFileInfo.TabIndex = 2;
            lblFileInfo.Text = "Arrastra una imagen aquí o haz clic en 'Abrir imagen'";
            lblFileInfo.TextAlign = ContentAlignment.MiddleLeft;

            // panelResult
            panelResult.BackColor = Color.FromArgb(35, 35, 45);
            panelResult.Controls.Add(lblResultTitle);
            panelResult.Controls.Add(picResult);
            panelResult.Controls.Add(lblResultInfo);
            panelResult.Dock = DockStyle.Bottom;
            panelResult.Location = new Point(0, 370);
            panelResult.Name = "panelResult";
            panelResult.Padding = new Padding(15);
            panelResult.Size = new Size(720, 160);
            panelResult.TabIndex = 3;
            panelResult.Visible = false;

            // lblResultTitle
            lblResultTitle.Dock = DockStyle.Top;
            lblResultTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblResultTitle.ForeColor = Color.White;
            lblResultTitle.Location = new Point(15, 15);
            lblResultTitle.Name = "lblResultTitle";
            lblResultTitle.Size = new Size(690, 25);
            lblResultTitle.TabIndex = 0;
            lblResultTitle.Text = "Imagen redimensionada:";
            lblResultTitle.TextAlign = ContentAlignment.MiddleLeft;

            // picResult
            picResult.BackColor = Color.Black;
            picResult.BorderStyle = BorderStyle.FixedSingle;
            picResult.Location = new Point(15, 45);
            picResult.Name = "picResult";
            picResult.Size = new Size(80, 80);
            picResult.SizeMode = PictureBoxSizeMode.Zoom;
            picResult.TabIndex = 1;
            picResult.TabStop = false;

            // lblResultInfo
            lblResultInfo.Font = new Font("Segoe UI", 9F);
            lblResultInfo.ForeColor = Color.White;
            lblResultInfo.Location = new Point(105, 45);
            lblResultInfo.Name = "lblResultInfo";
            lblResultInfo.Size = new Size(590, 100);
            lblResultInfo.TabIndex = 2;
            lblResultInfo.Text = "";

            // panelOptions
            panelOptions.BackColor = Color.FromArgb(35, 35, 45);
            panelOptions.Controls.Add(lblWidth);
            panelOptions.Controls.Add(txtWidth);
            panelOptions.Controls.Add(lblWidthHint);
            panelOptions.Controls.Add(lblHeight);
            panelOptions.Controls.Add(txtHeight);
            panelOptions.Controls.Add(lblHeightHint);
            panelOptions.Controls.Add(lblPercent);
            panelOptions.Controls.Add(txtPercent);
            panelOptions.Controls.Add(lblMethodTitle);
            panelOptions.Controls.Add(cmbMethod);
            panelOptions.Controls.Add(lblAspectTitle);
            panelOptions.Controls.Add(cmbAspect);
            panelOptions.Controls.Add(btnResize);
            panelOptions.Controls.Add(btnCrop);
            panelOptions.Controls.Add(btnSave);
            panelOptions.Controls.Add(chkRemember);
            panelOptions.Dock = DockStyle.Bottom;
            panelOptions.Location = new Point(0, 160);
            panelOptions.Name = "panelOptions";
            panelOptions.Size = new Size(720, 210);
            panelOptions.TabIndex = 4;

            // lblWidth
            lblWidth.AutoSize = true;
            lblWidth.ForeColor = Color.White;
            lblWidth.Location = new Point(15, 15);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(65, 15);
            lblWidth.TabIndex = 0;
            lblWidth.Text = "↔ Ancho:";

            // txtWidth
            txtWidth.Location = new Point(95, 12);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(80, 23);
            txtWidth.TabIndex = 1;

            // lblWidthHint
            lblWidthHint.AutoSize = true;
            lblWidthHint.ForeColor = Color.Gray;
            lblWidthHint.Location = new Point(185, 15);
            lblWidthHint.Name = "lblWidthHint";
            lblWidthHint.Size = new Size(115, 15);
            lblWidthHint.TabIndex = 2;
            lblWidthHint.Text = "(Vacío = automático)";

            // lblHeight
            lblHeight.AutoSize = true;
            lblHeight.ForeColor = Color.White;
            lblHeight.Location = new Point(15, 45);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(68, 15);
            lblHeight.TabIndex = 3;
            lblHeight.Text = "↑ Altura:";

            // txtHeight
            txtHeight.Location = new Point(95, 42);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(80, 23);
            txtHeight.TabIndex = 4;

            // lblHeightHint
            lblHeightHint.AutoSize = true;
            lblHeightHint.ForeColor = Color.Gray;
            lblHeightHint.Location = new Point(185, 45);
            lblHeightHint.Name = "lblHeightHint";
            lblHeightHint.Size = new Size(115, 15);
            lblHeightHint.TabIndex = 5;
            lblHeightHint.Text = "(Vacío = automático)";

            // lblPercent
            lblPercent.AutoSize = true;
            lblPercent.ForeColor = Color.White;
            lblPercent.Location = new Point(15, 75);
            lblPercent.Name = "lblPercent";
            lblPercent.Size = new Size(78, 15);
            lblPercent.TabIndex = 6;
            lblPercent.Text = "Porcentaje:";

            // txtPercent
            txtPercent.Location = new Point(95, 72);
            txtPercent.Name = "txtPercent";
            txtPercent.Size = new Size(80, 23);
            txtPercent.TabIndex = 7;

            // lblMethodTitle
            lblMethodTitle.AutoSize = true;
            lblMethodTitle.ForeColor = Color.White;
            lblMethodTitle.Location = new Point(15, 105);
            lblMethodTitle.Name = "lblMethodTitle";
            lblMethodTitle.Size = new Size(190, 15);
            lblMethodTitle.TabIndex = 8;
            lblMethodTitle.Text = "Método de redimensionamiento:";

            // cmbMethod
            cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMethod.Location = new Point(15, 125);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(350, 23);
            cmbMethod.TabIndex = 9;
            cmbMethod.Items.AddRange(new object[] {
                "Redimensionar (Bicúbica, calidad)",
                "Redimensionar (Bilineal)",
                "Redimensionar (Vecino cercano, rápido)",
                "Centrar y recortar para ajustarse",
                "Estirar para ajustarse",
                "Fuerza la relación de aspecto original",
                "Añadir relleno transparente"
            });

            // lblAspectTitle
            lblAspectTitle.AutoSize = true;
            lblAspectTitle.ForeColor = Color.White;
            lblAspectTitle.Location = new Point(390, 105);
            lblAspectTitle.Name = "lblAspectTitle";
            lblAspectTitle.Size = new Size(215, 15);
            lblAspectTitle.TabIndex = 10;
            lblAspectTitle.Text = "Si la relación de aspecto no coincide:";

            // cmbAspect
            cmbAspect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAspect.Location = new Point(390, 125);
            cmbAspect.Name = "cmbAspect";
            cmbAspect.Size = new Size(250, 23);
            cmbAspect.TabIndex = 11;
            cmbAspect.Items.AddRange(new object[] {
                "Centro y recorte para ajustarse",
                "Estirar para ajustarse",
                "Fuerza la relación de aspecto original",
                "Añadir relleno transparente"
            });

            // btnResize
            btnResize.BackColor = Color.FromArgb(0, 120, 215);
            btnResize.FlatStyle = FlatStyle.Flat;
            btnResize.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnResize.ForeColor = Color.White;
            btnResize.Location = new Point(15, 165);
            btnResize.Name = "btnResize";
            btnResize.Size = new Size(180, 35);
            btnResize.TabIndex = 12;
            btnResize.Text = "¡Redimensiona la imagen!";
            btnResize.UseVisualStyleBackColor = false;
            btnResize.Click += BtnResize_Click;

            // btnCrop
            btnCrop.BackColor = Color.FromArgb(60, 60, 70);
            btnCrop.Enabled = false;
            btnCrop.FlatStyle = FlatStyle.Flat;
            btnCrop.ForeColor = Color.White;
            btnCrop.Location = new Point(210, 165);
            btnCrop.Name = "btnCrop";
            btnCrop.Size = new Size(100, 35);
            btnCrop.TabIndex = 13;
            btnCrop.Text = "Cortar";
            btnCrop.UseVisualStyleBackColor = false;

            // btnSave
            btnSave.BackColor = Color.FromArgb(60, 60, 70);
            btnSave.Enabled = false;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(320, 165);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 35);
            btnSave.TabIndex = 14;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;

            // chkRemember
            chkRemember.AutoSize = true;
            chkRemember.ForeColor = Color.White;
            chkRemember.Location = new Point(440, 172);
            chkRemember.Name = "chkRemember";
            chkRemember.Size = new Size(138, 19);
            chkRemember.TabIndex = 15;
            chkRemember.Text = "Recuerda los ajustes";
            chkRemember.UseVisualStyleBackColor = true;

            // ResizeForm
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 555);
            Controls.Add(picPreview);
            Controls.Add(panelToolbar);
            Controls.Add(lblFileInfo);
            Controls.Add(panelResult);
            Controls.Add(panelOptions);
            Name = "ResizeForm";
            Text = "Redimensionar imágenes";
            DragDrop += ResizeForm_DragDrop;
            DragEnter += ResizeForm_DragEnter;

            panelToolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            panelResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picResult).EndInit();
            panelOptions.ResumeLayout(false);
            panelOptions.PerformLayout();
            ResumeLayout(false);
        }

        private Panel panelToolbar;
        private Button btnOpen;
        private PictureBox picPreview;
        private Label lblFileInfo;
        private Panel panelResult;
        private Label lblResultTitle;
        private PictureBox picResult;
        private Label lblResultInfo;
        private Panel panelOptions;
        private Label lblWidth;
        private TextBox txtWidth;
        private Label lblWidthHint;
        private Label lblHeight;
        private TextBox txtHeight;
        private Label lblHeightHint;
        private Label lblPercent;
        private TextBox txtPercent;
        private Label lblMethodTitle;
        private ComboBox cmbMethod;
        private Label lblAspectTitle;
        private ComboBox cmbAspect;
        private Button btnResize;
        private Button btnCrop;
        private Button btnSave;
        private CheckBox chkRemember;
    }
}
