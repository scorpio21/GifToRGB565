namespace GifRGB565GUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ListBox lstFrames;
        private System.Windows.Forms.PictureBox picPreview;

        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer animTimer;

        private System.Windows.Forms.TrackBar speedSlider;
        private System.Windows.Forms.Label lblSpeed;

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.CheckBox chkLoop;

        // NUEVO
        private System.Windows.Forms.CheckBox chkDither;
        private System.Windows.Forms.CheckBox chkNoise;
        private System.Windows.Forms.CheckBox chkSharpen;

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
            components = new System.ComponentModel.Container();
            btnSelectFolder = new Button();
            txtFolder = new TextBox();
            btnGenerate = new Button();
            progressBar = new ProgressBar();
            txtLog = new TextBox();
            lstFrames = new ListBox();
            picPreview = new PictureBox();
            btnPlay = new Button();
            btnStop = new Button();
            animTimer = new System.Windows.Forms.Timer(components);
            speedSlider = new TrackBar();
            lblSpeed = new Label();
            btnNext = new Button();
            btnPrev = new Button();
            chkLoop = new CheckBox();
            chkDither = new CheckBox();
            chkNoise = new CheckBox();
            chkSharpen = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            SuspendLayout();
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.BackColor = Color.FromArgb(45, 45, 45);
            btnSelectFolder.ForeColor = Color.White;
            btnSelectFolder.Location = new Point(12, 12);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(150, 30);
            btnSelectFolder.TabIndex = 16;
            btnSelectFolder.Text = "Seleccionar GIF/Carpeta";
            btnSelectFolder.UseVisualStyleBackColor = false;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // txtFolder
            // 
            txtFolder.BackColor = Color.FromArgb(45, 45, 45);
            txtFolder.ForeColor = Color.White;
            txtFolder.Location = new Point(170, 17);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(400, 23);
            txtFolder.TabIndex = 15;
            // 
            // btnGenerate
            // 
            btnGenerate.BackColor = Color.FromArgb(45, 45, 45);
            btnGenerate.ForeColor = Color.White;
            btnGenerate.Location = new Point(12, 55);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(150, 30);
            btnGenerate.TabIndex = 14;
            btnGenerate.Text = "Generar .h";
            btnGenerate.UseVisualStyleBackColor = false;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(170, 55);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(400, 30);
            progressBar.TabIndex = 13;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(45, 45, 45);
            txtLog.ForeColor = Color.White;
            txtLog.Location = new Point(12, 100);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(558, 250);
            txtLog.TabIndex = 12;
            // 
            // lstFrames
            // 
            lstFrames.BackColor = Color.FromArgb(45, 45, 45);
            lstFrames.ForeColor = Color.White;
            lstFrames.ItemHeight = 15;
            lstFrames.Location = new Point(12, 360);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(250, 199);
            lstFrames.TabIndex = 11;
            lstFrames.SelectedIndexChanged += lstFrames_SelectedIndexChanged;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.Black;
            picPreview.Location = new Point(270, 360);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(300, 200);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 10;
            picPreview.TabStop = false;
            // 
            // btnPlay
            // 
            btnPlay.BackColor = Color.FromArgb(45, 45, 45);
            btnPlay.ForeColor = Color.White;
            btnPlay.Location = new Point(12, 570);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(120, 30);
            btnPlay.TabIndex = 8;
            btnPlay.Text = "▶ Play";
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.FromArgb(45, 45, 45);
            btnStop.ForeColor = Color.White;
            btnStop.Location = new Point(150, 570);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(120, 30);
            btnStop.TabIndex = 9;
            btnStop.Text = "■ Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // speedSlider
            // 
            speedSlider.Location = new Point(300, 570);
            speedSlider.Maximum = 200;
            speedSlider.Minimum = 10;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(200, 45);
            speedSlider.TabIndex = 7;
            speedSlider.TickFrequency = 10;
            speedSlider.Value = 50;
            // Asociar evento Tick del timer y usar el valor del slider para el intervalo
            animTimer.Tick += animTimer_Tick;
            animTimer.Interval = speedSlider.Value;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // lblSpeed
            // 
            lblSpeed.ForeColor = Color.White;
            lblSpeed.Location = new Point(510, 570);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(60, 30);
            lblSpeed.TabIndex = 6;
            lblSpeed.Text = "50 ms";
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.FromArgb(45, 45, 45);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(150, 610);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(120, 30);
            btnNext.TabIndex = 4;
            btnNext.Text = "Next ⟩";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.FromArgb(45, 45, 45);
            btnPrev.ForeColor = Color.White;
            btnPrev.Location = new Point(12, 610);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(120, 30);
            btnPrev.TabIndex = 5;
            btnPrev.Text = "⟨ Prev";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += btnPrev_Click;
            // 
            // chkLoop
            // 
            chkLoop.BackColor = SystemColors.ActiveCaptionText;
            chkLoop.Checked = true;
            chkLoop.CheckState = CheckState.Checked;
            chkLoop.ForeColor = Color.IndianRed;
            chkLoop.Location = new Point(300, 610);
            chkLoop.Name = "chkLoop";
            chkLoop.Size = new Size(120, 30);
            chkLoop.TabIndex = 3;
            chkLoop.Text = "Loop ON";
            chkLoop.UseVisualStyleBackColor = false;
            // 
            // chkDither
            // 
            chkDither.BackColor = SystemColors.ActiveCaptionText;
            chkDither.Checked = true;
            chkDither.CheckState = CheckState.Checked;
            chkDither.ForeColor = Color.IndianRed;
            chkDither.Location = new Point(12, 320);
            chkDither.Name = "chkDither";
            chkDither.Size = new Size(200, 30);
            chkDither.TabIndex = 0;
            chkDither.Text = "Dithering ON";
            chkDither.UseVisualStyleBackColor = false;
            // 
            // chkNoise
            // 
            chkNoise.BackColor = SystemColors.ActiveCaptionText;
            chkNoise.ForeColor = Color.IndianRed;
            chkNoise.Location = new Point(220, 320);
            chkNoise.Name = "chkNoise";
            chkNoise.Size = new Size(200, 30);
            chkNoise.TabIndex = 1;
            chkNoise.Text = "Noise Reduction";
            chkNoise.UseVisualStyleBackColor = false;
            // 
            // chkSharpen
            // 
            chkSharpen.BackColor = SystemColors.ActiveCaptionText;
            chkSharpen.ForeColor = Color.IndianRed;
            chkSharpen.Location = new Point(430, 320);
            chkSharpen.Name = "chkSharpen";
            chkSharpen.Size = new Size(200, 30);
            chkSharpen.TabIndex = 2;
            chkSharpen.Text = "Sharpen";
            chkSharpen.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(582, 660);
            Controls.Add(chkDither);
            Controls.Add(chkNoise);
            Controls.Add(chkSharpen);
            Controls.Add(chkLoop);
            Controls.Add(btnNext);
            Controls.Add(btnPrev);
            Controls.Add(lblSpeed);
            Controls.Add(speedSlider);
            Controls.Add(btnPlay);
            Controls.Add(btnStop);
            Controls.Add(picPreview);
            Controls.Add(lstFrames);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnGenerate);
            Controls.Add(txtFolder);
            Controls.Add(btnSelectFolder);
            Name = "Form1";
            Text = "GIF → RGB565 Converter (AOUpdate Dark Theme)";
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
