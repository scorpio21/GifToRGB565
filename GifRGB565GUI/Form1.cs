using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Collections.Generic;

namespace GifRGB565GUI
{
    public partial class Form1 : Form
    {
        private string framesFolder = "";
        private string[] frameFiles = Array.Empty<string>();
        private Bitmap[] gifFrames = Array.Empty<Bitmap>();
        private int currentFrameIndex = 0;
        private bool usingGif = false;

        private enum ExportFormat { N64, BIN, BINGZ }
        private ExportFormat currentExportFormat = ExportFormat.N64;

        public Form1()
        {
            InitializeComponent();

            // Initialize menu state from default
            UpdateGenerateButtonText();
        }

        private void UpdateGenerateButtonText()
        {
            switch (currentExportFormat)
            {
                case ExportFormat.N64:
                    btnGenerate.Text = "Generar .h";
                    break;
                case ExportFormat.BIN:
                    btnGenerate.Text = "Generar .bin";
                    break;
                case ExportFormat.BINGZ:
                    btnGenerate.Text = "Generar .bin.gz";
                    break;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void compN64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentExportFormat = ExportFormat.N64;
            compN64ToolStripMenuItem.Checked = true;
            compBinToolStripMenuItem.Checked = false;
            compBinGzToolStripMenuItem.Checked = false;
            UpdateGenerateButtonText();
        }

        private void compBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentExportFormat = ExportFormat.BIN;
            compN64ToolStripMenuItem.Checked = false;
            compBinToolStripMenuItem.Checked = true;
            compBinGzToolStripMenuItem.Checked = false;
            UpdateGenerateButtonText();
        }

        private void compBinGzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentExportFormat = ExportFormat.BINGZ;
            compN64ToolStripMenuItem.Checked = false;
            compBinToolStripMenuItem.Checked = false;
            compBinGzToolStripMenuItem.Checked = true;
            UpdateGenerateButtonText();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "GIF Animado|*.gif|Carpeta de frames|*.*";
                dialog.Title = "Selecciona un GIF o una carpeta";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;

                    if (path.EndsWith(".gif"))
                    {
                        LoadGif(path);
                        usingGif = true;
                        txtFolder.Text = path;
                        Log($"GIF cargado: {path}");
                    }
                    else
                    {
                        usingGif = false;
                        framesFolder = Path.GetDirectoryName(path);
                        txtFolder.Text = framesFolder;
                        Log($"Carpeta seleccionada: {framesFolder}");
                        LoadFrames();
                    }
                }
            }
        }

        private void LoadGif(string gifPath)
        {
            Image gif = Image.FromFile(gifPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd);

            gifFrames = new Bitmap[count];
            lstFrames.Items.Clear();

            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                gifFrames[i] = new Bitmap(gif);
                lstFrames.Items.Add($"Frame {i}");
            }

            Log($"Frames del GIF: {count}");

            if (count > 0)
            {
                currentFrameIndex = 0;
                lstFrames.SelectedIndex = 0;
                picPreview.Image = gifFrames[0];
            }
        }

        private void LoadFrames()
        {
            frameFiles = Directory.GetFiles(framesFolder)
                                  .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg"))
                                  .ToArray();

            lstFrames.Items.Clear();
            foreach (var f in frameFiles)
                lstFrames.Items.Add(Path.GetFileName(f));

            Log($"Frames cargados: {frameFiles.Length}");

            if (frameFiles.Length > 0)
            {
                currentFrameIndex = 0;
                lstFrames.SelectedIndex = 0;
                picPreview.Image = Image.FromFile(frameFiles[0]);
            }
        }

        private void lstFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndex < 0) return;

            currentFrameIndex = lstFrames.SelectedIndex;

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            animTimer.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            animTimer.Stop();
        }

        private void animTimer_Tick(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : frameFiles.Length;

            if (total == 0)
            {
                animTimer.Stop();
                return;
            }

            if (currentFrameIndex >= total)
            {
                if (chkLoop.Checked)
                    currentFrameIndex = 0;
                else
                {
                    animTimer.Stop();
                    return;
                }
            }

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);

            lstFrames.SelectedIndex = currentFrameIndex;
            currentFrameIndex++;
        }

        private void speedSlider_Scroll(object sender, EventArgs e)
        {
            animTimer.Interval = speedSlider.Value;
            lblSpeed.Text = $"{speedSlider.Value} ms";
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : frameFiles.Length;

            currentFrameIndex++;
            if (currentFrameIndex >= total)
                currentFrameIndex = chkLoop.Checked ? 0 : total - 1;

            lstFrames.SelectedIndex = currentFrameIndex;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : frameFiles.Length;

            currentFrameIndex--;
            if (currentFrameIndex < 0)
                currentFrameIndex = chkLoop.Checked ? total - 1 : 0;

            lstFrames.SelectedIndex = currentFrameIndex;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Update converter options
            ImageConverter.EnableDithering = chkDither.Checked;
            ImageConverter.EnableNoiseReduction = chkNoise.Checked;
            ImageConverter.EnableSharpen = chkSharpen.Checked;

            int totalFrames = usingGif ? gifFrames.Length : frameFiles.Length;
            if (totalFrames == 0)
            {
                MessageBox.Show("No hay frames cargados.");
                return;
            }

            int width = usingGif ? gifFrames[0].Width : Image.FromFile(frameFiles[0]).Width;
            int height = usingGif ? gifFrames[0].Height : Image.FromFile(frameFiles[0]).Height;

            // Prepare progress bar
            try
            {
                progressBar.Value = 0;
                progressBar.Maximum = Math.Max(1, totalFrames);
            }
            catch { }

            if (currentExportFormat == ExportFormat.N64)
            {
                // Generate header at default path
                Directory.CreateDirectory("output");
                string path = Path.Combine("output", "n64.h");
                GenerateHeaderAtPath(path, width, height, totalFrames);
                Log($"✔ Archivo header generado: {path}");
                MessageBox.Show($"Header generado: {path}");
                return;
            }

            // For bin exports, ask user for path
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "ESP32 binary|*.bin;*.bin.gz|All files|*.*";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = currentExportFormat == ExportFormat.BINGZ ? "animation.bin.gz" : "animation.bin";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                string outPath = dialog.FileName;
                bool gzip = currentExportFormat == ExportFormat.BINGZ || chkGzip.Checked;

                // Validate frames sizes
                if (!ValidateFrameSizes(width, height, totalFrames))
                {
                    MessageBox.Show("Todos los frames deben tener el mismo tamaño.");
                    return;
                }

                List<ushort[]> framesData = new List<ushort[]>(totalFrames);

                // convert frames and update progress
                for (int i = 0; i < totalFrames; i++)
                {
                    Bitmap bmp = usingGif ? gifFrames[i] : new Bitmap(frameFiles[i]);
                    framesData.Add(ImageConverter.ToRGB565(bmp).ToArray());

                    // update progress
                    try { progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value + 1); } catch { }
                    Log($"Convertido frame {i}");
                    Application.DoEvents();
                }

                try
                {
                    ExportBin(outPath, width, height, framesData, gzip);
                    Log($"✔ Archivo exportado: {outPath}");
                    MessageBox.Show($"Exportación completada: {outPath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    Log($"Error export: {ex.Message}");
                }
            }
        }

        private bool ValidateFrameSizes(int width, int height, int totalFrames)
        {
            for (int i = 0; i < totalFrames; i++)
            {
                Bitmap bmp = usingGif ? gifFrames[i] : new Bitmap(frameFiles[i]);
                if (bmp.Width != width || bmp.Height != height) return false;
            }

            return true;
        }

        private void GenerateHeaderAtPath(string outPath, int width, int height, int totalFrames)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? "output");

            using (StreamWriter writer = new StreamWriter(outPath))
            {
                writer.WriteLine($"int frames = {totalFrames};");
                writer.WriteLine($"int animation_width = {width};");
                writer.WriteLine($"int animation_height = {height};");
                writer.WriteLine();
                writer.WriteLine($"const unsigned short PROGMEM n64[{totalFrames}][{width * height}] = {{");

                for (int i = 0; i < totalFrames; i++)
                {
                    Bitmap bmp = usingGif ? gifFrames[i] : new Bitmap(frameFiles[i]);
                    var rgb565 = ImageConverter.ToRGB565(bmp);

                    writer.Write("{");
                    writer.Write(string.Join(",", rgb565.Select(v => "0x" + v.ToString("X"))));
                    writer.WriteLine("},");

                    // update progress and log
                    try { progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value + 1); } catch { }
                    Log($"Convertido frame {i}");
                    Application.DoEvents();
                }

                writer.WriteLine("};");
            }
        }

        private void ExportBin(string outPath, int width, int height, List<ushort[]> framesData, bool gzip)
        {
            using (var ms = new MemoryStream())
            {
                // Header: width (int32 LE), height (int32 LE), frames (int32 LE)
                ms.Write(BitConverter.GetBytes(width), 0, 4);
                ms.Write(BitConverter.GetBytes(height), 0, 4);
                ms.Write(BitConverter.GetBytes(framesData.Count), 0, 4);

                // Datos frames: cada píxel uint16 LE
                foreach (var frame in framesData)
                {
                    foreach (var px in frame)
                    {
                        ms.Write(BitConverter.GetBytes(px), 0, 2);
                    }
                }

                // Ensure position reset before writing
                ms.Position = 0;

                if (gzip)
                {
                    using (var fs = File.Create(outPath))
                    using (var gz = new GZipStream(fs, CompressionMode.Compress))
                    {
                        ms.CopyTo(gz);
                    }
                }
                else
                {
                    using (var fs = File.Create(outPath))
                    {
                        ms.CopyTo(fs);
                    }
                }
            }
        }

        // -------------------------
        // Simulación RGB565 desde frames en memoria
        // -------------------------
        private void btnSimulate_Click(object sender, EventArgs e)
        {
            int totalFrames = usingGif ? gifFrames.Length : frameFiles.Length;
            if (totalFrames == 0) { MessageBox.Show("No hay frames cargados."); return; }

            int width = usingGif ? gifFrames[0].Width : Image.FromFile(frameFiles[0]).Width;
            int height = usingGif ? gifFrames[0].Height : Image.FromFile(frameFiles[0]).Height;

            List<ushort[]> framesData = new List<ushort[]>(totalFrames);
            for (int i = 0; i < totalFrames; i++)
            {
                Bitmap bmp = usingGif ? gifFrames[i] : new Bitmap(frameFiles[i]);
                framesData.Add(ImageConverter.ToRGB565(bmp).ToArray());
            }

            // Mostrar una ventana modal simple para simular la reproducción de los frames RGB565
            using (var simForm = new Form())
            {
                simForm.Text = "Simulación RGB565";
                simForm.ClientSize = new Size(width, height + 30);
                simForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                simForm.StartPosition = FormStartPosition.CenterParent;

                var picture = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
                simForm.Controls.Add(picture);

                var t = new System.Windows.Forms.Timer { Interval = speedSlider.Value };
                int idx = 0;

                t.Tick += (s, ev) =>
                {
                    picture.Image = ConvertRgb565ToBitmap(framesData[idx], width, height);
                    idx++;
                    if (idx >= framesData.Count)
                    {
                        if (chkLoop.Checked) idx = 0; else t.Stop();
                    }
                };

                t.Start();
                simForm.ShowDialog();
                t.Stop();
            }
        }

        private Bitmap ConvertRgb565ToBitmap(ushort[] data, int width, int height)
        {
            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int p = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ushort v = data[p++];
                    int r5 = (v >> 11) & 0x1F;
                    int g6 = (v >> 5) & 0x3F;
                    int b5 = v & 0x1F;

                    int r = (r5 << 3) | (r5 >> 2);
                    int g = (g6 << 2) | (g6 >> 4);
                    int b = (b5 << 3) | (b5 >> 2);

                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return bmp;
        }

        private void btnGenerate_Click_1(object sender, EventArgs e)
        {
            btnGenerate_Click(sender, e);
        }

        private void Log(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }
    }
}
