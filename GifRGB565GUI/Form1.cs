using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace GifRGB565GUI
{
    public partial class Form1 : Form
    {
        private string framesFolder = "";
        private string[] frameFiles = Array.Empty<string>();
        private Bitmap[] gifFrames = Array.Empty<Bitmap>();
        private int currentFrameIndex = 0;
        private bool usingGif = false;

        public Form1()
        {
            InitializeComponent();
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
            // APLICAR OPCIONES DE FILTROS
            ImageConverter.EnableDithering = chkDither.Checked;
            ImageConverter.EnableNoiseReduction = chkNoise.Checked;
            ImageConverter.EnableSharpen = chkSharpen.Checked;

            Log($"Dithering: {chkDither.Checked}");
            Log($"Noise Reduction: {chkNoise.Checked}");
            Log($"Sharpen: {chkSharpen.Checked}");

            int totalFrames = usingGif ? gifFrames.Length : frameFiles.Length;

            if (totalFrames == 0)
            {
                MessageBox.Show("No hay frames cargados.");
                return;
            }

            Directory.CreateDirectory("output");
            string outputFile = Path.Combine("output", "n64.h");

            int width = usingGif ? gifFrames[0].Width : Image.FromFile(frameFiles[0]).Width;
            int height = usingGif ? gifFrames[0].Height : Image.FromFile(frameFiles[0]).Height;

            progressBar.Value = 0;
            progressBar.Maximum = totalFrames;

            using (StreamWriter writer = new StreamWriter(outputFile))
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

                    progressBar.Value++;
                    Log($"Convertido frame {i}");
                }

                writer.WriteLine("};");
            }

            Log("✔ Archivo generado: output/n64.h");
            MessageBox.Show("Conversión completada.");
        }

        private void Log(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }
    }
}
