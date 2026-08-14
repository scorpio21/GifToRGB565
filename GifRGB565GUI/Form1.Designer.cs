using System;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    partial class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compresionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compN64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compBinGzToolStripMenuItem;

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
        private System.Windows.Forms.CheckBox chkDither;
        private System.Windows.Forms.CheckBox chkNoise;
        private System.Windows.Forms.CheckBox chkSharpen;
        private System.Windows.Forms.CheckBox chkGzip;
        private System.Windows.Forms.Button btnSimulate;

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
            menuStrip1 = new MenuStrip();
            archivoToolStripMenuItem = new ToolStripMenuItem();
            salirToolStripMenuItem = new ToolStripMenuItem();
            compresionToolStripMenuItem = new ToolStripMenuItem();
            compN64ToolStripMenuItem = new ToolStripMenuItem();
            compBinToolStripMenuItem = new ToolStripMenuItem();
            compBinGzToolStripMenuItem = new ToolStripMenuItem();
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
            chkGzip = new CheckBox();
            btnSimulate = new Button();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { archivoToolStripMenuItem, compresionToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(582, 24);
            menuStrip1.TabIndex = 0;
            // 
            // archivoToolStripMenuItem
            // 
            archivoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { salirToolStripMenuItem });
            archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            archivoToolStripMenuItem.Size = new Size(60, 20);
            archivoToolStripMenuItem.Text = "Archivo";
            // 
            // salirToolStripMenuItem
            // 
            salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            salirToolStripMenuItem.Size = new Size(96, 22);
            salirToolStripMenuItem.Text = "Salir";
            salirToolStripMenuItem.Click += salirToolStripMenuItem_Click;
            // 
            // compresionToolStripMenuItem
            // 
            compresionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { compN64ToolStripMenuItem, compBinToolStripMenuItem, compBinGzToolStripMenuItem });
            compresionToolStripMenuItem.Name = "compresionToolStripMenuItem";
            compresionToolStripMenuItem.Size = new Size(84, 20);
            compresionToolStripMenuItem.Text = "Compresión";
            // 
            // compN64ToolStripMenuItem
            // 
            compN64ToolStripMenuItem.Checked = true;
            compN64ToolStripMenuItem.CheckOnClick = true;
            compN64ToolStripMenuItem.CheckState = CheckState.Checked;
            compN64ToolStripMenuItem.Name = "compN64ToolStripMenuItem";
            compN64ToolStripMenuItem.Size = new Size(154, 22);
            compN64ToolStripMenuItem.Text = "n64.h (original)";
            compN64ToolStripMenuItem.Click += compN64ToolStripMenuItem_Click;
            // 
            // compBinToolStripMenuItem
            // 
            compBinToolStripMenuItem.CheckOnClick = true;
            compBinToolStripMenuItem.Name = "compBinToolStripMenuItem";
            compBinToolStripMenuItem.Size = new Size(154, 22);
            compBinToolStripMenuItem.Text = "esp32.bin";
            compBinToolStripMenuItem.Click += compBinToolStripMenuItem_Click;
            // 
            // compBinGzToolStripMenuItem
            // 
            compBinGzToolStripMenuItem.CheckOnClick = true;
            compBinGzToolStripMenuItem.Name = "compBinGzToolStripMenuItem";
            compBinGzToolStripMenuItem.Size = new Size(154, 22);
            compBinGzToolStripMenuItem.Text = "esp32.bin.gz";
            compBinGzToolStripMenuItem.Click += compBinGzToolStripMenuItem_Click;
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(12, 36);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(150, 30);
            btnSelectFolder.TabIndex = 19;
            btnSelectFolder.Text = "Seleccionar GIF/Carpeta";
            btnSelectFolder.UseVisualStyleBackColor = true;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(170, 41);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(400, 23);
            txtFolder.TabIndex = 18;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(12, 79);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(150, 30);
            btnGenerate.TabIndex = 17;
            btnGenerate.Text = "Generar .h";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(170, 79);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(400, 30);
            progressBar.TabIndex = 16;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 124);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(558, 250);
            txtLog.TabIndex = 15;
            // 
            // lstFrames
            // 
            lstFrames.ItemHeight = 15;
            lstFrames.Location = new Point(12, 384);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(250, 199);
            lstFrames.TabIndex = 14;
            lstFrames.SelectedIndexChanged += lstFrames_SelectedIndexChanged;
            // 
            // picPreview
            // 
            picPreview.Location = new Point(270, 384);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(300, 200);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 13;
            picPreview.TabStop = false;
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(12, 594);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(120, 30);
            btnPlay.TabIndex = 11;
            btnPlay.Text = "▶ Play";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(150, 594);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(120, 30);
            btnStop.TabIndex = 12;
            btnStop.Text = "■ Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // speedSlider
            // 
            speedSlider.Location = new Point(300, 594);
            speedSlider.Maximum = 200;
            speedSlider.Minimum = 10;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(200, 45);
            speedSlider.TabIndex = 10;
            speedSlider.TickFrequency = 10;
            speedSlider.Value = 50;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // lblSpeed
            // 
            lblSpeed.Location = new Point(510, 594);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(60, 30);
            lblSpeed.TabIndex = 9;
            lblSpeed.Text = "50 ms";
            // 
            // btnNext
            // 
            btnNext.Location = new Point(150, 634);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(120, 30);
            btnNext.TabIndex = 7;
            btnNext.Text = "Next ⟩";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(12, 634);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(120, 30);
            btnPrev.TabIndex = 8;
            btnPrev.Text = "⟨ Prev";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += btnPrev_Click;
            // 
            // chkLoop
            // 
            chkLoop.Checked = true;
            chkLoop.CheckState = CheckState.Checked;
            chkLoop.Location = new Point(300, 634);
            chkLoop.Name = "chkLoop";
            chkLoop.Size = new Size(120, 30);
            chkLoop.TabIndex = 6;
            chkLoop.Text = "Loop ON";
            // 
            // chkDither
            // 
            chkDither.Checked = true;
            chkDither.CheckState = CheckState.Checked;
            chkDither.Location = new Point(12, 344);
            chkDither.Name = "chkDither";
            chkDither.Size = new Size(200, 30);
            chkDither.TabIndex = 5;
            chkDither.Text = "Dithering ON";
            // 
            // chkNoise
            // 
            chkNoise.Location = new Point(220, 344);
            chkNoise.Name = "chkNoise";
            chkNoise.Size = new Size(200, 30);
            chkNoise.TabIndex = 4;
            chkNoise.Text = "Noise Reduction";
            // 
            // chkSharpen
            // 
            chkSharpen.Location = new Point(430, 344);
            chkSharpen.Name = "chkSharpen";
            chkSharpen.Size = new Size(200, 30);
            chkSharpen.TabIndex = 3;
            chkSharpen.Text = "Sharpen";
            // 
            // chkGzip
            // 
            chkGzip.Location = new Point(220, 740);
            chkGzip.Name = "chkGzip";
            chkGzip.Size = new Size(137, 23);
            chkGzip.TabIndex = 2;
            chkGzip.Text = "GZip (if applicable)";
            // 
            // btnSimulate
            // 
            btnSimulate.Location = new Point(380, 733);
            btnSimulate.Name = "btnSimulate";
            btnSimulate.Size = new Size(120, 30);
            btnSimulate.TabIndex = 1;
            btnSimulate.Text = "Simular .h";
            btnSimulate.UseVisualStyleBackColor = true;
            btnSimulate.Click += btnSimulate_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(582, 780);
            Controls.Add(menuStrip1);
            Controls.Add(btnSimulate);
            Controls.Add(chkGzip);
            Controls.Add(chkSharpen);
            Controls.Add(chkNoise);
            Controls.Add(chkDither);
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
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "GIF → RGB565 Converter (AOUpdate Dark Theme)";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
