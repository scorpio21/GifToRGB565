using System;
using System.Windows.Forms;
using System.Drawing;

namespace GifRGB565GUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearRecentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator recentSeparator;
        private System.Windows.Forms.ToolStripMenuItem cargarHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilidadesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compresionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compN64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compBinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compBinGzToolStripMenuItem;

        // Ayuda menu items
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaDitherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaNoiseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaSharpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaGzipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acercaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem claroToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oscuroToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusDims;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusFrames;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusSize;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusFormat;

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label lblOutName;
        private System.Windows.Forms.TextBox txtOutName;
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
        private System.Windows.Forms.ToolStripMenuItem exportarFramesToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpRescale;
        private System.Windows.Forms.ComboBox cmbRescalePreset;
        private System.Windows.Forms.NumericUpDown nudRescaleW;
        private System.Windows.Forms.NumericUpDown nudRescaleH;
        private System.Windows.Forms.CheckBox chkKeepRatio;

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
            recentToolStripMenuItem = new ToolStripMenuItem();
            clearRecentToolStripMenuItem = new ToolStripMenuItem();
            salirToolStripMenuItem = new ToolStripMenuItem();
            verToolStripMenuItem = new ToolStripMenuItem();
            temaToolStripMenuItem = new ToolStripMenuItem();
            claroToolStripMenuItem = new ToolStripMenuItem();
            oscuroToolStripMenuItem = new ToolStripMenuItem();
            compresionToolStripMenuItem = new ToolStripMenuItem();
            compN64ToolStripMenuItem = new ToolStripMenuItem();
            compBinToolStripMenuItem = new ToolStripMenuItem();
            compBinGzToolStripMenuItem = new ToolStripMenuItem();
            utilidadesToolStripMenuItem = new ToolStripMenuItem();
            cargarHeaderToolStripMenuItem = new ToolStripMenuItem();
            exportarFramesToolStripMenuItem = new ToolStripMenuItem();
            ayudaToolStripMenuItem = new ToolStripMenuItem();
            ayudaDitherToolStripMenuItem = new ToolStripMenuItem();
            ayudaNoiseToolStripMenuItem = new ToolStripMenuItem();
            ayudaSharpenToolStripMenuItem = new ToolStripMenuItem();
            ayudaGzipToolStripMenuItem = new ToolStripMenuItem();
            acercaToolStripMenuItem = new ToolStripMenuItem();
            grpRescale = new GroupBox();
            cmbRescalePreset = new ComboBox();
            nudRescaleW = new NumericUpDown();
            nudRescaleH = new NumericUpDown();
            chkKeepRatio = new CheckBox();
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
            recentSeparator = new ToolStripSeparator();
            lblOutName = new Label();
            txtOutName = new TextBox();
            statusStrip1 = new StatusStrip();
            lblStatusDims = new ToolStripStatusLabel();
            lblStatusFrames = new ToolStripStatusLabel();
            lblStatusSize = new ToolStripStatusLabel();
            lblStatusFormat = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            grpRescale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRescaleW).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRescaleH).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { archivoToolStripMenuItem, verToolStripMenuItem, compresionToolStripMenuItem, utilidadesToolStripMenuItem, ayudaToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(582, 24);
            menuStrip1.TabIndex = 0;
            // 
            // archivoToolStripMenuItem
            // 
            archivoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { recentToolStripMenuItem, salirToolStripMenuItem });
            archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            archivoToolStripMenuItem.Size = new Size(60, 20);
            archivoToolStripMenuItem.Text = "Archivo";
            // 
            // recentToolStripMenuItem
            // 
            recentToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clearRecentToolStripMenuItem });
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            recentToolStripMenuItem.Size = new Size(158, 22);
            recentToolStripMenuItem.Text = "Abierto reciente";
            // 
            // clearRecentToolStripMenuItem
            // 
            clearRecentToolStripMenuItem.Name = "clearRecentToolStripMenuItem";
            clearRecentToolStripMenuItem.Size = new Size(151, 22);
            clearRecentToolStripMenuItem.Text = "Borrar historial";
            clearRecentToolStripMenuItem.Click += clearRecentToolStripMenuItem_Click;
            // 
            // salirToolStripMenuItem
            // 
            salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            salirToolStripMenuItem.Size = new Size(158, 22);
            salirToolStripMenuItem.Text = "Salir";
            salirToolStripMenuItem.Click += salirToolStripMenuItem_Click;
            // 
            // verToolStripMenuItem
            // 
            verToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { temaToolStripMenuItem });
            verToolStripMenuItem.Name = "verToolStripMenuItem";
            verToolStripMenuItem.Size = new Size(35, 20);
            verToolStripMenuItem.Text = "Ver";
            // 
            // temaToolStripMenuItem
            // 
            temaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { claroToolStripMenuItem, oscuroToolStripMenuItem });
            temaToolStripMenuItem.Name = "temaToolStripMenuItem";
            temaToolStripMenuItem.Size = new Size(103, 22);
            temaToolStripMenuItem.Text = "Tema";
            // 
            // claroToolStripMenuItem
            // 
            claroToolStripMenuItem.Name = "claroToolStripMenuItem";
            claroToolStripMenuItem.Size = new Size(112, 22);
            claroToolStripMenuItem.Text = "Claro";
            claroToolStripMenuItem.Click += claroToolStripMenuItem_Click;
            // 
            // oscuroToolStripMenuItem
            // 
            oscuroToolStripMenuItem.Name = "oscuroToolStripMenuItem";
            oscuroToolStripMenuItem.Size = new Size(112, 22);
            oscuroToolStripMenuItem.Text = "Oscuro";
            oscuroToolStripMenuItem.Click += oscuroToolStripMenuItem_Click;
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
            // utilidadesToolStripMenuItem
            // 
            utilidadesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cargarHeaderToolStripMenuItem, exportarFramesToolStripMenuItem });
            utilidadesToolStripMenuItem.Name = "utilidadesToolStripMenuItem";
            utilidadesToolStripMenuItem.Size = new Size(71, 20);
            utilidadesToolStripMenuItem.Text = "Utilidades";
            // 
            // cargarHeaderToolStripMenuItem
            // 
            cargarHeaderToolStripMenuItem.Name = "cargarHeaderToolStripMenuItem";
            cargarHeaderToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.L;
            cargarHeaderToolStripMenuItem.Size = new Size(209, 22);
            cargarHeaderToolStripMenuItem.Text = "Cargar .h";
            cargarHeaderToolStripMenuItem.Click += cargarHeaderToolStripMenuItem_Click;
            // 
            // exportarFramesToolStripMenuItem
            // 
            exportarFramesToolStripMenuItem.Name = "exportarFramesToolStripMenuItem";
            exportarFramesToolStripMenuItem.Size = new Size(209, 22);
            exportarFramesToolStripMenuItem.Text = "Exportar todos los Frames";
            exportarFramesToolStripMenuItem.Click += exportarFramesToolStripMenuItem_Click;
            // 
            // ayudaToolStripMenuItem
            // 
            ayudaToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            ayudaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ayudaDitherToolStripMenuItem, ayudaNoiseToolStripMenuItem, ayudaSharpenToolStripMenuItem, ayudaGzipToolStripMenuItem, acercaToolStripMenuItem });
            ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            ayudaToolStripMenuItem.Size = new Size(53, 20);
            ayudaToolStripMenuItem.Text = "Ayuda";
            // 
            // ayudaDitherToolStripMenuItem
            // 
            ayudaDitherToolStripMenuItem.Name = "ayudaDitherToolStripMenuItem";
            ayudaDitherToolStripMenuItem.Size = new Size(174, 22);
            ayudaDitherToolStripMenuItem.Text = "Dithering ON";
            ayudaDitherToolStripMenuItem.Click += ayudaDitherToolStripMenuItem_Click;
            // 
            // ayudaNoiseToolStripMenuItem
            // 
            ayudaNoiseToolStripMenuItem.Name = "ayudaNoiseToolStripMenuItem";
            ayudaNoiseToolStripMenuItem.Size = new Size(174, 22);
            ayudaNoiseToolStripMenuItem.Text = "Noise Reduction";
            ayudaNoiseToolStripMenuItem.Click += ayudaNoiseToolStripMenuItem_Click;
            // 
            // ayudaSharpenToolStripMenuItem
            // 
            ayudaSharpenToolStripMenuItem.Name = "ayudaSharpenToolStripMenuItem";
            ayudaSharpenToolStripMenuItem.Size = new Size(174, 22);
            ayudaSharpenToolStripMenuItem.Text = "Sharpen";
            ayudaSharpenToolStripMenuItem.Click += ayudaSharpenToolStripMenuItem_Click;
            // 
            // ayudaGzipToolStripMenuItem
            // 
            ayudaGzipToolStripMenuItem.Name = "ayudaGzipToolStripMenuItem";
            ayudaGzipToolStripMenuItem.Size = new Size(174, 22);
            ayudaGzipToolStripMenuItem.Text = "GZip (if applicable)";
            ayudaGzipToolStripMenuItem.Click += ayudaGzipToolStripMenuItem_Click;
            // 
            // acercaToolStripMenuItem
            // 
            acercaToolStripMenuItem.Name = "acercaToolStripMenuItem";
            acercaToolStripMenuItem.Size = new Size(174, 22);
            acercaToolStripMenuItem.Text = "Acerca de...";
            acercaToolStripMenuItem.Click += acercaToolStripMenuItem_Click;
            // 
            // grpRescale
            // 
            grpRescale.Controls.Add(cmbRescalePreset);
            grpRescale.Controls.Add(nudRescaleW);
            grpRescale.Controls.Add(nudRescaleH);
            grpRescale.Controls.Add(chkKeepRatio);
            grpRescale.Location = new Point(12, 387);
            grpRescale.Name = "grpRescale";
            grpRescale.Size = new Size(558, 55);
            grpRescale.TabIndex = 20;
            grpRescale.TabStop = false;
            grpRescale.Text = "Rescale";
            // 
            // cmbRescalePreset
            // 
            cmbRescalePreset.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRescalePreset.Items.AddRange(new object[] { "Original", "50%", "25%", "160x120", "320x240", "Personalizado" });
            cmbRescalePreset.Location = new Point(18, 20);
            cmbRescalePreset.Name = "cmbRescalePreset";
            cmbRescalePreset.Size = new Size(120, 23);
            cmbRescalePreset.TabIndex = 0;
            cmbRescalePreset.SelectedIndexChanged += cmbRescalePreset_SelectedIndexChanged;
            // 
            // nudRescaleW
            // 
            nudRescaleW.Location = new Point(158, 21);
            nudRescaleW.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            nudRescaleW.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            nudRescaleW.Name = "nudRescaleW";
            nudRescaleW.Size = new Size(70, 23);
            nudRescaleW.TabIndex = 1;
            nudRescaleW.Value = new decimal(new int[] { 320, 0, 0, 0 });
            nudRescaleW.ValueChanged += nudRescaleW_ValueChanged;
            // 
            // nudRescaleH
            // 
            nudRescaleH.Location = new Point(248, 21);
            nudRescaleH.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            nudRescaleH.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            nudRescaleH.Name = "nudRescaleH";
            nudRescaleH.Size = new Size(70, 23);
            nudRescaleH.TabIndex = 2;
            nudRescaleH.Value = new decimal(new int[] { 240, 0, 0, 0 });
            nudRescaleH.ValueChanged += nudRescaleH_ValueChanged;
            // 
            // chkKeepRatio
            // 
            chkKeepRatio.Checked = true;
            chkKeepRatio.CheckState = CheckState.Checked;
            chkKeepRatio.Location = new Point(328, 21);
            chkKeepRatio.Name = "chkKeepRatio";
            chkKeepRatio.Size = new Size(160, 23);
            chkKeepRatio.TabIndex = 3;
            chkKeepRatio.Text = "Mantener proporción";
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(12, 31);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(150, 30);
            btnSelectFolder.TabIndex = 19;
            btnSelectFolder.Text = "Seleccionar GIF/Carpeta";
            btnSelectFolder.UseVisualStyleBackColor = true;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(170, 36);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(400, 23);
            txtFolder.TabIndex = 18;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(12, 89);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(150, 30);
            btnGenerate.TabIndex = 17;
            btnGenerate.Text = "Generar .h";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(170, 89);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(400, 30);
            progressBar.TabIndex = 16;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 131);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(558, 186);
            txtLog.TabIndex = 15;
            // 
            // lstFrames
            // 
            lstFrames.ItemHeight = 15;
            lstFrames.Location = new Point(12, 453);
            lstFrames.Name = "lstFrames";
            lstFrames.Size = new Size(250, 199);
            lstFrames.TabIndex = 14;
            lstFrames.SelectedIndexChanged += lstFrames_SelectedIndexChanged;
            // 
            // picPreview
            // 
            picPreview.Location = new Point(270, 453);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(300, 200);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 13;
            picPreview.TabStop = false;
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(12, 659);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(120, 30);
            btnPlay.TabIndex = 11;
            btnPlay.Text = "▶ Play";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(150, 659);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(120, 30);
            btnStop.TabIndex = 12;
            btnStop.Text = "■ Parar";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // animTimer
            // 
            animTimer.Interval = 50;
            animTimer.Tick += animTimer_Tick;
            // 
            // speedSlider
            // 
            speedSlider.Location = new Point(276, 659);
            speedSlider.Maximum = 200;
            speedSlider.Minimum = 10;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(224, 45);
            speedSlider.TabIndex = 10;
            speedSlider.TickFrequency = 10;
            speedSlider.Value = 50;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // lblSpeed
            // 
            lblSpeed.Location = new Point(506, 667);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(60, 30);
            lblSpeed.TabIndex = 9;
            lblSpeed.Text = "50 ms";
            // 
            // btnNext
            // 
            btnNext.Location = new Point(150, 699);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(120, 30);
            btnNext.TabIndex = 7;
            btnNext.Text = "Siguiente ⟩";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(12, 699);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(120, 30);
            btnPrev.TabIndex = 8;
            btnPrev.Text = "⟨ Anterior";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += btnPrev_Click;
            // 
            // chkLoop
            // 
            chkLoop.Checked = true;
            chkLoop.CheckState = CheckState.Checked;
            chkLoop.Location = new Point(289, 699);
            chkLoop.Name = "chkLoop";
            chkLoop.Size = new Size(163, 30);
            chkLoop.TabIndex = 6;
            chkLoop.Text = "Repetición activada";
            chkLoop.CheckedChanged += chkLoop_CheckedChanged;
            // 
            // chkDither
            // 
            chkDither.Checked = true;
            chkDither.CheckState = CheckState.Checked;
            chkDither.Location = new Point(12, 323);
            chkDither.Name = "chkDither";
            chkDither.Size = new Size(217, 30);
            chkDither.TabIndex = 5;
            chkDither.Text = "Dithering activado";
            chkDither.CheckedChanged += chkDither_CheckedChanged;
            // 
            // chkNoise
            // 
            chkNoise.Location = new Point(12, 351);
            chkNoise.Name = "chkNoise";
            chkNoise.Size = new Size(217, 30);
            chkNoise.TabIndex = 4;
            chkNoise.Text = "Reducción de ruido desactivada";
            chkNoise.CheckedChanged += chkNoise_CheckedChanged;
            // 
            // chkSharpen
            // 
            chkSharpen.Location = new Point(260, 323);
            chkSharpen.Name = "chkSharpen";
            chkSharpen.Size = new Size(146, 30);
            chkSharpen.TabIndex = 3;
            chkSharpen.Text = " Enfoque desactivado";
            chkSharpen.CheckedChanged += chkSharpen_CheckedChanged;
            // 
            // chkGzip
            // 
            chkGzip.Location = new Point(260, 355);
            chkGzip.Name = "chkGzip";
            chkGzip.Size = new Size(146, 23);
            chkGzip.TabIndex = 2;
            chkGzip.Text = "GZip (if applicable)";
            // 
            // btnSimulate
            // 
            btnSimulate.BackColor = Color.Blue;
            btnSimulate.ForeColor = Color.Yellow;
            btnSimulate.Location = new Point(450, 698);
            btnSimulate.Name = "btnSimulate";
            btnSimulate.Size = new Size(120, 30);
            btnSimulate.TabIndex = 1;
            btnSimulate.Text = "Simular .h";
            btnSimulate.UseVisualStyleBackColor = false;
            btnSimulate.Click += btnSimulate_Click;
            // 
            // recentSeparator
            // 
            recentSeparator.Name = "recentSeparator";
            recentSeparator.Size = new Size(6, 6);
            // 
            // lblOutName
            // 
            lblOutName.Location = new Point(65, 64);
            lblOutName.Name = "lblOutName";
            lblOutName.Size = new Size(97, 23);
            lblOutName.TabIndex = 18;
            lblOutName.Text = "Nombre salida:";
            lblOutName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtOutName
            // 
            txtOutName.Location = new Point(172, 62);
            txtOutName.Name = "txtOutName";
            txtOutName.Size = new Size(308, 23);
            txtOutName.TabIndex = 18;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatusDims, lblStatusFrames, lblStatusSize, lblStatusFormat });
            statusStrip1.Location = new Point(0, 735);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(582, 22);
            statusStrip1.TabIndex = 19;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatusDims
            // 
            lblStatusDims.Name = "lblStatusDims";
            lblStatusDims.Size = new Size(62, 17);
            lblStatusDims.Text = "Sin frames";
            // 
            // lblStatusFrames
            // 
            lblStatusFrames.Name = "lblStatusFrames";
            lblStatusFrames.Size = new Size(52, 17);
            lblStatusFrames.Text = "0 frames";
            // 
            // lblStatusSize
            // 
            lblStatusSize.Name = "lblStatusSize";
            lblStatusSize.Size = new Size(30, 17);
            lblStatusSize.Text = "0 KB";
            // 
            // lblStatusFormat
            // 
            lblStatusFormat.Name = "lblStatusFormat";
            lblStatusFormat.Size = new Size(28, 17);
            lblStatusFormat.Text = "N64";
            // 
            // Form1
            // 
            ClientSize = new Size(582, 757);
            Controls.Add(grpRescale);
            Controls.Add(statusStrip1);
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
            Controls.Add(lblOutName);
            Controls.Add(txtOutName);
            Controls.Add(txtFolder);
            Controls.Add(btnSelectFolder);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "GIF → RGB565 Converter (AOUpdate Dark Theme)";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            grpRescale.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudRescaleW).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRescaleH).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
