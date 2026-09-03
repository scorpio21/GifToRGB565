using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GifRGB565GUI
{
    public partial class Form1 : Form
    {
        private string framesFolder = "";
        private string[] frameFiles = Array.Empty<string>();
        private Bitmap[] gifFrames = Array.Empty<Bitmap>();
        private int currentFrameIndex = 0;
        private bool usingGif = false;
        // Header-loaded frames
        private List<ushort[]>? headerFrames = null;
        private int headerWidth = 0;
        private int headerHeight = 0;
        private bool usingHeader = false;

        private enum ExportFormat { N64, BIN, BINGZ }
        private ExportFormat currentExportFormat = ExportFormat.N64;
        private CancellationTokenSource? _generateCts;
        private CancellationTokenSource? _exportCts;

        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GifToRGB565", "last_output.json");

        private static string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in invalid)
                name = name.Replace(c.ToString(), "");
            name = name.Replace(Path.DirectorySeparatorChar.ToString(), "");
            name = name.Replace(Path.AltDirectorySeparatorChar.ToString(), "");
            return name.Trim();
        }

        private void SaveConfig()
        {
            try
            {
                var lastName = txtOutName?.Text?.Trim() ?? "";
                var theme = ThemeManager.IsDark ? "dark" : "light";
                var recent = LoadRecentFiles();
                var recentJson = string.Join(",", recent.Select(r => $"\"{r.Replace("\\", "\\\\")}\""));
                var json = $"{{\"lastName\":\"{lastName}\",\"theme\":\"{theme}\",\"recentFiles\":[{recentJson}]}}";
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var match = Regex.Match(json, "\"lastName\"\\s*:\\s*\"([^\"]*)\"");
                    if (match.Success && txtOutName != null)
                        txtOutName.Text = match.Groups[1].Value;

                    var themeMatch = Regex.Match(json, "\"theme\"\\s*:\\s*\"(dark|light)\"");
                    if (themeMatch.Success)
                    {
                        bool dark = themeMatch.Groups[1].Value == "dark";
                        ThemeManager.ApplyTheme(this, dark);
                        btnSimulate.BackColor = Color.Blue;  // tu color del designer
                        if (dark) oscuroToolStripMenuItem.Checked = true;
                        else claroToolStripMenuItem.Checked = true;
                    }
                }
            }
            catch { }
        }

        private List<string> LoadRecentFiles()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var match = Regex.Match(json, "\"recentFiles\"\\s*:\\s*\\[([^\\]]*)\\]");
                    if (match.Success)
                    {
                        var content = match.Groups[1].Value.Trim();
                        if (string.IsNullOrEmpty(content)) return new List<string>();
                        var files = Regex.Matches(content, "\"([^\"]*)\"")
                            .Cast<Match>()
                            .Select(m => m.Groups[1].Value)
                            .Where(f => File.Exists(f) || Directory.Exists(f))
                            .ToList();
                        return files;
                    }
                }
            }
            catch { }
            return new List<string>();
        }

        private void SaveRecentFiles(List<string> recent)
        {
            try
            {
                var lastName = txtOutName?.Text?.Trim() ?? "";
                var theme = ThemeManager.IsDark ? "dark" : "light";
                var recentJson = string.Join(",", recent.Select(r => $"\"{r.Replace("\\", "\\\\")}\""));
                var json = $"{{\"lastName\":\"{lastName}\",\"theme\":\"{theme}\",\"recentFiles\":[{recentJson}]}}";
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }

        private void AddToRecentFiles(string path)
        {
            var recent = LoadRecentFiles();
            recent.Remove(path);
            recent.Insert(0, path);
            if (recent.Count > 5)
                recent.RemoveAt(5);
            SaveRecentFiles(recent);
            LoadRecentMenu();
        }

        private void LoadRecentMenu()
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            var recent = LoadRecentFiles();

            if (recent.Count == 0)
            {
                recentToolStripMenuItem.Enabled = false;
                return;
            }

            recentToolStripMenuItem.Enabled = true;
            foreach (var path in recent)
            {
                var name = File.Exists(path) ? Path.GetFileName(path) : Path.GetFileName(Path.GetDirectoryName(path) ?? path);
                if (string.IsNullOrEmpty(name)) name = path;
                var item = new ToolStripMenuItem(name);
                item.ToolTipText = path;
                item.Tag = path;
                item.Click += recentFileItem_Click;
                recentToolStripMenuItem.DropDownItems.Add(item);
            }
            recentToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            recentToolStripMenuItem.DropDownItems.Add(clearRecentToolStripMenuItem);
        }

        private void recentFileItem_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem item || item.Tag is not string path) return;

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                MessageBox.Show($"El archivo ya no existe:\n{path}", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var recent = LoadRecentFiles();
                recent.Remove(path);
                SaveRecentFiles(recent);
                LoadRecentMenu();
                return;
            }

            usingHeader = false;
            headerFrames = null;
            picRGB565.Image?.Dispose();
            picRGB565.Image = null;

            if (path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            {
                LoadGif(path);
                usingGif = true;
                txtFolder.Text = path;
                Log($"GIF cargado (reciente): {path}");
            }
            else if (File.Exists(path))
            {
                // Individual file (e.g. PNG) — load its parent folder
                usingGif = false;
                framesFolder = Path.GetDirectoryName(path) ?? "";
                txtFolder.Text = framesFolder;
                Log($"Carpeta seleccionada (reciente): {framesFolder}");
                LoadFrames();
            }
            else
            {
                // Folder
                usingGif = false;
                framesFolder = path;
                txtFolder.Text = framesFolder;
                Log($"Carpeta seleccionada (reciente): {framesFolder}");
                LoadFrames();
            }

            AddToRecentFiles(path);
        }

        private void clearRecentToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            SaveRecentFiles(new List<string>());
            LoadRecentMenu();
        }

        public Form1()
        {
            InitializeComponent();

            // Wire Load handler instead of doing UI work in constructor (designer safe)
            this.Load += Form1_Load;
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
            picPreview.MouseWheel += picPreview_MouseWheel;
            this.KeyDown += Form1_KeyDown;
            lblVersion.Text = ReadVersionFromChangelog();
        }

        private string ReadVersionFromChangelog()
        {
            try
            {
                string changelogPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CHANGELOG.md");
                if (!File.Exists(changelogPath))
                    changelogPath = Path.Combine(AppContext.BaseDirectory, "CHANGELOG.md");
                if (!File.Exists(changelogPath)) return "v?.?";

                foreach (var line in File.ReadLines(changelogPath))
                {
                    if (line.Contains("## ["))
                    {
                        int start = line.IndexOf('[');
                        int end = line.IndexOf(']', start);
                        if (start >= 0 && end > start)
                            return "v" + line.Substring(start + 1, end - start - 1);
                    }
                }
            }
            catch { }
            return "v?.?";
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            UpdateGenerateButtonText();
            LoadConfig();
            LoadRecentMenu();
            // Estado inicial de botones de reproducción
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
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
            UpdateStatusBar();
            cmbGzipLevel.Visible = false;
        }

        private void compBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentExportFormat = ExportFormat.BIN;
            compN64ToolStripMenuItem.Checked = false;
            compBinToolStripMenuItem.Checked = true;
            compBinGzToolStripMenuItem.Checked = false;
            UpdateGenerateButtonText();
            UpdateStatusBar();
            cmbGzipLevel.Visible = false;
        }

        private void compBinGzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentExportFormat = ExportFormat.BINGZ;
            compN64ToolStripMenuItem.Checked = false;
            compBinToolStripMenuItem.Checked = false;
            compBinGzToolStripMenuItem.Checked = true;
            UpdateGenerateButtonText();
            UpdateStatusBar();
            cmbGzipLevel.Visible = true;
            if (cmbGzipLevel.SelectedIndex < 0) cmbGzipLevel.SelectedIndex = 1;
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Imágenes|*.gif;*.png;*.jpg;*.jpeg|GIF Animado|*.gif|Carpeta de frames|*.*";
                dialog.Title = "Selecciona un GIF, imagen o carpeta";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;

                    // Clear any loaded header state
                    usingHeader = false;
                    headerFrames = null;
                    picRGB565.Image?.Dispose();
                    picRGB565.Image = null;

                    if (path.EndsWith(".gif"))
                    {
                        LoadGif(path);
                        usingGif = true;
                        txtFolder.Text = path;
                        Log($"GIF cargado: {path}");
                        AddToRecentFiles(path);
                    }
                    else if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
                    {
                        usingGif = false;
                        framesFolder = Path.GetDirectoryName(path) ?? "";
                        txtFolder.Text = framesFolder;
                        Log($"Carpeta del frame: {framesFolder}");
                        LoadFrames();

                        // Seleccionar el archivo clicado
                        string fileName = Path.GetFileName(path);
                        int idx = lstFrames.Items.IndexOf(fileName);
                        if (idx >= 0)
                        {
                            lstFrames.SelectedIndex = idx;
                            currentFrameIndex = idx;
                            picPreview.Image = Image.FromFile(path);
                        }
                        AddToRecentFiles(path);
                    }
                    else
                    {
                        usingGif = false;
                        framesFolder = Path.GetDirectoryName(path) ?? "";
                        txtFolder.Text = framesFolder;
                        Log($"Carpeta seleccionada: {framesFolder}");
                        LoadFrames();
                        AddToRecentFiles(path);
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
                ShowRgb565Preview(0);

                // Después de cargar y poner currentFrameIndex = 0
                btnPrev.Enabled = false;
                btnNext.Enabled = gifFrames.Length > 1;
            }

            UpdateStatusBar();
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
                ShowRgb565Preview(0);
            }

            UpdateStatusBar();
        }

        private void lstFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndex < 0)
            {
                UpdateFrameButtons();
                return;
            }

            currentFrameIndex = lstFrames.SelectedIndex;

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else if (usingHeader && headerFrames != null)
                picPreview.Image = ConvertRgb565ToBitmap(headerFrames[currentFrameIndex], headerWidth, headerHeight);
            else
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);

            UpdateFrameButtons();
            ShowRgb565Preview(currentFrameIndex);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            if (total == 0)
            {
                MessageBox.Show("No hay frames cargados.");
                return;
            }

            // Ensure timer interval is set from the slider and at least 1 ms
            try
            {
                animTimer.Interval = Math.Max(1, speedSlider.Value);
            }
            catch { animTimer.Interval = 50; }

            animTimer.Enabled = true;
            animTimer.Start();
            // Estado de botones
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            btnNext.Enabled = true;
            btnPrev.Enabled = false; // No se puede retroceder hasta avanzar
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            animTimer.Stop();
            animTimer.Enabled = false;

            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            // Estado de botones
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            btnPrev.Enabled = currentFrameIndex > 0;
            btnNext.Enabled = total > 0 && currentFrameIndex < total - 1;
        }

        private void animTimer_Tick(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            if (total == 0)
            {
                animTimer.Stop();
                return;
            }

            if (currentFrameIndex >= total)
            {
                if (chkLoop.Checked)
                {
                    chkLoop.Text = "Repetición activada";
                    currentFrameIndex = 0;
                }
                else
                {
                    chkLoop.Text = "Repetición desactivada";
                    animTimer.Stop();
                    return;
                }
            }

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else if (usingHeader && headerFrames != null)
                picPreview.Image = ConvertRgb565ToBitmap(headerFrames[currentFrameIndex], headerWidth, headerHeight);
            else
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);

            lstFrames.SelectedIndex = currentFrameIndex;
            ShowRgb565Preview(currentFrameIndex);
            currentFrameIndex++;
        }

        private void speedSlider_Scroll(object sender, EventArgs e)
        {
            animTimer.Interval = speedSlider.Value;
            lblSpeed.Text = $"{speedSlider.Value} ms";
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            if (total == 0) return;

            // Avanza uno
            if (currentFrameIndex < total - 1)
                currentFrameIndex++;

            // Actualiza imagen
            lstFrames.SelectedIndex = currentFrameIndex;

            // Estado de botones
            btnPrev.Enabled = currentFrameIndex > 0;
            btnNext.Enabled = currentFrameIndex < total - 1;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            if (total == 0) return;

            // Retrocede uno
            if (currentFrameIndex > 0)
                currentFrameIndex--;

            // Actualiza imagen
            lstFrames.SelectedIndex = currentFrameIndex;

            // Estado de botones
            btnPrev.Enabled = currentFrameIndex > 0;
            btnNext.Enabled = currentFrameIndex < total - 1;
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            // Update converter options
            ImageConverter.EnableDithering = chkDither.Checked;
            ImageConverter.EnableNoiseReduction = chkNoise.Checked;
            ImageConverter.EnableSharpen = chkSharpen.Checked;

            int totalFrames = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            if (totalFrames == 0)
            {
                MessageBox.Show("No hay frames cargados.");
                return;
            }

            int width = usingGif ? gifFrames[0].Width : (usingHeader ? headerWidth : Image.FromFile(frameFiles[0]).Width);
            int height = usingGif ? gifFrames[0].Height : (usingHeader ? headerHeight : Image.FromFile(frameFiles[0]).Height);

            // Prepare progress bar
            try
            {
                progressBar.Value = 0;
                progressBar.Maximum = Math.Max(1, totalFrames);
            }
            catch { }

            _generateCts = new CancellationTokenSource();
            btnCancelar.Visible = true;
            btnCancelar.Enabled = true;
            btnCancelar.Text = "Cancelar";

            string outName = (txtOutName?.Text ?? "").Trim();
            if (string.IsNullOrEmpty(outName))
            {
                // fallback name
                outName = currentExportFormat == ExportFormat.BINGZ ? "animation.bin.gz" : (currentExportFormat == ExportFormat.BIN ? "animation.bin" : "n64.h");
            }

            if (currentExportFormat == ExportFormat.N64)
            {
                // Require user to specify output name for .h
                if (string.IsNullOrWhiteSpace(txtOutName?.Text))
                {
                    MessageBox.Show("Debes introducir un nombre de fichero de salida en 'Nombre salida:' antes de generar el .h.", "Nombre requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOutName?.Focus();
                    return;
                }

                // Sanitize filename — remove invalid characters instead of blocking
                string candidate = txtOutName.Text.Trim();
                string sanitized = SanitizeFileName(candidate);

                if (string.IsNullOrEmpty(sanitized))
                {
                    MessageBox.Show("El nombre resulta vacío tras eliminar caracteres inválidos.", "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOutName.Focus();
                    return;
                }

                if (sanitized != candidate)
                {
                    txtOutName.Text = sanitized;
                    Log($"Nombre sanitizado: '{candidate}' → '{sanitized}'");
                }

                outName = sanitized;

                // Save config for next session
                SaveConfig();

                // Generate header at default path
                Directory.CreateDirectory("output");
                // Ensure .h extension
                string fileName = outName.EndsWith(".h", StringComparison.OrdinalIgnoreCase) ? outName : outName + ".h";
                string path = Path.Combine("output", fileName);
                await GenerateHeaderAtPathAsync(path, width, height, totalFrames, _generateCts.Token);
                Log($"✔ Archivo header generado: {path}");
                MessageBox.Show($"Header generado: {path}");
                _generateCts?.Dispose();
                _generateCts = null;
                btnCancelar.Visible = false;
                return;
            }

            // For bin exports, ask user for path via SaveFileDialog but prefill name from textbox
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "ESP32 binary|*.bin;*.bin.gz|All files|*.*";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;

                // Set default filename from txtOutName if present
                if (!string.IsNullOrEmpty(outName))
                {
                    dialog.FileName = outName;
                }
                else
                {
                    dialog.FileName = currentExportFormat == ExportFormat.BINGZ ? "animation.bin.gz" : "animation.bin";
                }

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
                var ct = _generateCts.Token;

                try
                {
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < totalFrames; i++)
                        {
                            ct.ThrowIfCancellationRequested();

                            Bitmap bmp;
                            if (usingGif) bmp = gifFrames[i];
                            else if (usingHeader && headerFrames != null) bmp = ConvertRgb565ToBitmap(headerFrames[i], headerWidth, headerHeight);
                            else bmp = new Bitmap(frameFiles[i]);

                            framesData.Add(ImageConverter.ToRGB565(bmp).ToArray());

                            if (!usingGif && !(usingHeader && headerFrames != null)) bmp.Dispose();

                            int progress = i + 1;
                            Invoke(() =>
                            {
                                try { progressBar.Value = Math.Min(progressBar.Maximum, progress); } catch { }
                                Log($"Convertido frame {i}");
                            });
                        }
                    }, ct);

                    ExportBin(outPath, width, height, framesData, gzip);
                    Log($"✔ Archivo exportado: {outPath}");
                    MessageBox.Show($"Exportación completada: {outPath}");
                }
                catch (OperationCanceledException)
                {
                    Log($"Generación cancelada en frame {framesData.Count}. Frames procesados: {framesData.Count}/{totalFrames}");
                    var result = MessageBox.Show($"Generación cancelada en frame {framesData.Count}/{totalFrames}.\n¿Conservar los frames procesados?", "Cancelado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes && framesData.Count > 0)
                    {
                        try
                        {
                            ExportBin(outPath, width, height, framesData, gzip);
                            Log($"✔ Parcial exportado: {outPath} ({framesData.Count} frames)");
                            MessageBox.Show($"Parcial exportado: {outPath} ({framesData.Count} frames)");
                        }
                        catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                    }
                }
            }

            _generateCts?.Dispose();
            _generateCts = null;
            btnCancelar.Visible = false;
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

        private async Task GenerateHeaderAtPathAsync(string outPath, int width, int height, int totalFrames, CancellationToken ct = default)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? "output");

            using (StreamWriter writer = new StreamWriter(outPath))
            {
                writer.WriteLine($"int frames = {totalFrames};");
                writer.WriteLine($"int animation_width = {width};");
                writer.WriteLine($"int animation_height = {height};");
                writer.WriteLine();
                writer.WriteLine($"const unsigned short PROGMEM n64[{totalFrames}][{width * height}] = {{");

                await Task.Run(() =>
                {
                    for (int i = 0; i < totalFrames; i++)
                    {
                        ct.ThrowIfCancellationRequested();

                        Bitmap bmp = usingGif ? gifFrames[i] : new Bitmap(frameFiles[i]);
                        var rgb565 = ImageConverter.ToRGB565(bmp);

                        writer.Write("{");
                        writer.Write(string.Join(",", rgb565.Select(v => "0x" + v.ToString("X"))));
                        writer.WriteLine("},");

                        int progress = i + 1;
                        Invoke(() =>
                        {
                            try { progressBar.Value = Math.Min(progressBar.Maximum, progress); } catch { }
                            Log($"Convertido frame {i}");
                        });
                    }
                }, ct);

                writer.WriteLine("};");
            }
        }

        private CompressionLevel GetGzipLevel()
        {
            return cmbGzipLevel.SelectedIndex switch
            {
                0 => CompressionLevel.Fastest,
                2 => CompressionLevel.SmallestSize,
                _ => CompressionLevel.Optimal
            };
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
                    using (var gz = new GZipStream(fs, GetGzipLevel()))
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

        private void ShowRgb565Preview(int index)
        {
            var oldImage = picRGB565.Image;
            Bitmap? rgb565Bmp = null;

            if (usingGif && gifFrames.Length > index)
            {
                var rgb565 = ImageConverter.ToRGB565(gifFrames[index]);
                rgb565Bmp = ConvertRgb565ToBitmap(rgb565.ToArray(), gifFrames[index].Width, gifFrames[index].Height);
            }
            else if (usingHeader && headerFrames != null && headerFrames.Count > index)
            {
                var rgb565 = ImageConverter.ToRGB565(ConvertRgb565ToBitmap(headerFrames[index], headerWidth, headerHeight));
                rgb565Bmp = ConvertRgb565ToBitmap(rgb565.ToArray(), headerWidth, headerHeight);
            }
            else if (!usingGif && !usingHeader && frameFiles.Length > index)
            {
                using var src = new Bitmap(frameFiles[index]);
                var rgb565 = ImageConverter.ToRGB565(src);
                rgb565Bmp = ConvertRgb565ToBitmap(rgb565.ToArray(), src.Width, src.Height);
            }

            picRGB565.Image = rgb565Bmp;
            oldImage?.Dispose();
        }

        private void btnGenerate_Click_1(object sender, EventArgs e)
        {
            btnGenerate_Click(sender, e);
        }

        private void Log(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }

        private void UpdateStatusBar()
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            if (total == 0)
            {
                lblStatusDims.Text = "Sin frames";
                lblStatusFrames.Text = "0 frames";
                lblStatusSize.Text = "";
                lblStatusFormat.Text = currentExportFormat.ToString();
                return;
            }

            int w = 0, h = 0;
            if (usingGif && gifFrames.Length > 0) { w = gifFrames[0].Width; h = gifFrames[0].Height; }
            else if (usingHeader) { w = headerWidth; h = headerHeight; }
            else if (frameFiles.Length > 0) { var bmp = Image.FromFile(frameFiles[0]); w = bmp.Width; h = bmp.Height; bmp.Dispose(); }

            lblStatusDims.Text = $"{w}x{h}";
            lblStatusFrames.Text = $"{total} frames";

            long bytes = (long)w * h * 2 * total;
            if (currentExportFormat == ExportFormat.N64)
                bytes = w * h * 2 * total;
            lblStatusSize.Text = bytes > 1024 * 1024 ? $"{bytes / (1024.0 * 1024.0):F1} MB" : $"{bytes / 1024.0:F1} KB";

            lblStatusFormat.Text = currentExportFormat switch
            {
                ExportFormat.N64 => ".h (N64)",
                ExportFormat.BIN => ".bin (ESP32)",
                ExportFormat.BINGZ => ".bin.gz (ESP32)",
                _ => currentExportFormat.ToString()
            };
        }

        private void ayudaDitherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dithering: aplica un patrón para mitigar bandas de color reduciendo el efecto de posterización. Puede mejorar apariencia en paletas reducidas, pero puede introducir ruido visual.", "Dithering", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Noise Reduction: filtra el ruido de la imagen antes de la conversión para suavizar áreas con grano. Útil si las imágenes tienen mucho ruido, pero puede perder detalle fino.", "Noise Reduction", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaSharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sharpen: aplica un filtro de realce para enfatizar bordes y detalles antes de la conversión. Útil cuando la conversión y reducción de paleta hacen las imágenes más suaves.", "Sharpen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaGzipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GZip: comprime el archivo binario de salida usando GZip (.gz). Reduce tamaño a costa de tiempo de compresión y uso de CPU en dispositivo que lo descomprima.", "GZip (if applicable)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaDragDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Drag & Drop: arrastra archivos directamente a la ventana de la aplicación.\n\n" +
                "• Archivo .gif → carga como animación GIF.\n" +
                "• Archivo .png o .jpg → carga la carpeta padre como secuencia de frames.\n" +
                "• Carpeta → carga todos los frames PNG/JPG que contenga.\n" +
                "• Archivo .h o .txt → parsea como header RGB565.\n\n" +
                "La ruta arrastrada se registra automáticamente en el menú Abierto Reciente.",
                "Drag & Drop", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaRescaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Rescale: redimensiona los frames antes de convertirlos a RGB565.\n\n" +
                "Controles:\n" +
                "• ComboBox de presets: Original, 50%, 25%, 160x120, 320x240, Personalizado.\n" +
                "• NumericBoxes de ancho y alto (8–2048 píxeles).\n" +
                "• Checkbox 'Mantener proporción': al cambiar el ancho se ajusta el alto automáticamente y viceversa.\n\n" +
                "Efecto:\n" +
                "• Los frames se escalan con interpolación bicúbica de alta calidad (HighQualityBicubic) antes de la conversión.\n" +
                "• Útil para reducir el tamaño del output (ej: 50% o 25%) o adaptar a una resolución específica (ej: 160x120 para pantallas pequeñas).\n" +
                "• La barra de estado muestra las dimensiones post-rescale.",
                "Rescale", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaAtajosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Atajos de teclado disponibles:\n\n" +
                "Ctrl+G → Generar archivo de salida\n" +
                "Ctrl+O → Abrir archivo/carpeta\n" +
                "Ctrl+L → Cargar header (.h)\n" +
                "Space → Play / Stop animación\n" +
                "← → Frame anterior\n" +
                "→ → Frame siguiente\n" +
                "Ctrl++ → Zoom 1:1 (píxel real)\n" +
                "Ctrl+- → Zoom ajustado al panel",
                "Atajos de teclado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaCompararToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Comparar Original vs RGB565:\n\n" +
                "Abre una ventana con 3 modos de comparación:\n\n" +
                "• Lado a lado: Panel izquierdo (original) vs panel derecho (RGB565).\n" +
                "• Wipe (división): Slider que controla la posición de división.\n" +
                "• Superpuesta: Muestra solo el RGB565 a pantalla completa.\n\n" +
                "Se aplican los filtros activos (dithering, noise, sharpen) al generar la comparación.",
                "Comparar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaRedimensionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Redimensionar imágenes:\n\n" +
                "Accede desde Menú → Utilidades → Redimensionar imágenes.\n\n" +
                "• Soporta imágenes estáticas (PNG, JPG, BMP, WebP) y GIFs animados.\n" +
                "• Para GIFs animados usa Magick.NET con Coalesce (como ezgif).\n" +
                "• Métodos: Lanczos (calidad), Bilineal, Vecino cercano (rápido).\n" +
                "• Modos de aspect ratio: centrar y recortar, estirar, forzar proporción, relleno transparente.\n" +
                "• Los campos Ancho, Altura y Porcentaje están sincronizados.\n" +
                "• El preview animado se mantiene al redimensionar.",
                "Redimensionar imágenes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayudaRecortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Recortar imágenes:\n\n" +
                "Se abre desde el botón 'Recortar' en el formulario de redimensionar.\n\n" +
                "• Selección visual: arrastra sobre la imagen para crear el área de recorte.\n" +
                "• Mover selección: haz clic dentro del rectángulo y arrastra.\n" +
                "• Campos Izquierda/Arriba/Ancho/Altura sincronizados con la selección.\n" +
                "• Relación de aspecto bloqueable (1:1, 4:3, 16:9, 3:2).\n" +
                "• Autocorte: recorta píxeles transparentes automáticamente.\n" +
                "• Funciona con imágenes estáticas y GIFs animados.",
                "Recortar imágenes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void acercaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version = ReadVersionFromChangelog();
            string repo = "https://github.com/scorpio21/GifToRGB565";
            string author = "scorpio21";

            MessageBox.Show($"GifToRGB565 {version}\nRepositorio: {repo}\nAutor: {author}", "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cargarHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Header n64.h|*.h;*.txt|All files|*.*";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                string path = dlg.FileName;
                try
                {
                    var result = ParseHeaderFile(path);
                    if (!result.Success)
                    {
                        MessageBox.Show($"Error parseando header: {result.Error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // populate main UI list and preview
                    headerFrames = result.FramesData ?? new List<ushort[]>();
                    headerWidth = result.Width;
                    headerHeight = result.Height;
                    usingHeader = true;
                    usingGif = false;
                    frameFiles = Array.Empty<string>();
                    gifFrames = Array.Empty<Bitmap>();
                    picRGB565.Image?.Dispose();
                    picRGB565.Image = null;

                    lstFrames.Items.Clear();
                    for (int i = 0; i < headerFrames.Count; i++)
                        lstFrames.Items.Add($"Frame {i}");

                    if (headerFrames.Count > 0)
                    {
                        currentFrameIndex = 0;
                        lstFrames.SelectedIndex = 0;
                        picPreview.Image = ConvertRgb565ToBitmap(headerFrames[0], headerWidth, headerHeight);
                        ShowRgb565Preview(0);
                    }

                    Log($"Header cargado: {path} - frames: {result.FramesData?.Count ?? 0}");
                    AddToRecentFiles(path);
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error leyendo archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private (bool Success, string Error, int Width, int Height, List<ushort[]>? FramesData) ParseHeaderFile(string path)
        {
            string text = File.ReadAllText(path);

            try
            {
                // Extraer width, height
                var mW = Regex.Match(text, @"animation_width\s*=\s*(\d+)");
                var mH = Regex.Match(text, @"animation_height\s*=\s*(\d+)");
                var mF = Regex.Match(text, @"int\s+frames\s*=\s*(\d+)");

                if (!mW.Success || !mH.Success || !mF.Success)
                    return (false, "No se encontraron width/height/frames en el header.", 0, 0, null);

                int width = int.Parse(mW.Groups[1].Value);
                int height = int.Parse(mH.Groups[1].Value);
                int frames = int.Parse(mF.Groups[1].Value);

                // Buscar la declaración del array con cualquier nombre, p.e. "const unsigned short PROGMEM name[][4900] = { ... };"
                var declPattern = new Regex(@"const\s+unsigned\s+short[\s\S]*?\b(?<name>\w+)\s*\[.*?\]\s*=\s*\{([\s\S]*?)\};", RegexOptions.Multiline);
                var blockMatch = declPattern.Match(text);
                if (!blockMatch.Success)
                {
                    // Fallback: buscar cualquier "[ ... ] = { ... };" sin la cabecera completa
                    blockMatch = Regex.Match(text, @"\[.*?\]\s*=\s*\{([\s\S]*?)\};", RegexOptions.Multiline);
                    if (!blockMatch.Success)
                        return (false, "No se encontró el bloque de datos del array (esperado 'const unsigned short ... = { ... };').", width, height, null);
                }

                string inside = blockMatch.Groups[1].Value;

                // Extraer todos los valores hex dentro del bloque (flexible) and also try to detect per-frame braces
                var frameMatches = Regex.Matches(inside, @"\{([^}]*)\}");
                var framesList = new List<ushort[]>();

                foreach (Match fm in frameMatches)
                {
                    var content = fm.Groups[1].Value;
                    var hexMatches = Regex.Matches(content, @"\0x([0-9A-Fa-f]+)");
                    var arr = new List<ushort>();
                    foreach (Match hx in hexMatches)
                    {
                        ushort val = Convert.ToUInt16(hx.Groups[1].Value, 16);
                        arr.Add(val);
                    }

                    if (arr.Count > 0)
                        framesList.Add(arr.ToArray());
                }

                // If no per-frame braces found, try to extract all hex values from the whole inside block
                if (framesList.Count == 0)
                {
                    var allHex = Regex.Matches(inside, @"0x([0-9A-Fa-f]+)");
                    var allList = new List<ushort>();
                    foreach (Match hx in allHex)
                    {
                        ushort val = Convert.ToUInt16(hx.Groups[1].Value, 16);
                        allList.Add(val);
                    }

                    if (allList.Count == 0)
                        return (false, "No se encontraron valores hexadecimales dentro del bloque.", width, height, null);

                    int perFrame = width * height;
                    if (allList.Count == frames * perFrame)
                    {
                        // split into frames
                        for (int i = 0; i < frames; i++)
                        {
                            var chunk = allList.Skip(i * perFrame).Take(perFrame).ToArray();
                            framesList.Add(chunk);
                        }
                    }
                    else
                    {
                        return (false, $"Número de valores ({allList.Count}) no coincide con frames*width*height ({frames}*{perFrame}={frames * perFrame}).", width, height, null);
                    }
                }
                else
                {
                    // We have some per-brace frames. Validate or try to reconcile if counts don't match frames
                    int perFrame = width * height;
                    int totalValues = framesList.Sum(a => a.Length);

                    if (framesList.Count == frames)
                    {
                        // verify each length equals perFrame
                        foreach (var f in framesList)
                        {
                            if (f.Length != perFrame)
                                return (false, $"Un frame tiene longitud {f.Length} pero se esperaba {perFrame}.", width, height, null);
                        }
                    }
                    else if (totalValues == frames * perFrame)
                    {
                        // join and reslice into exact frames
                        var joined = framesList.SelectMany(a => a).ToArray();
                        framesList.Clear();
                        for (int i = 0; i < frames; i++)
                        {
                            framesList.Add(joined.Skip(i * perFrame).Take(perFrame).ToArray());
                        }
                    }
                    else
                    {
                        return (false, $"Frames encontrados ({framesList.Count}) y valores totales ({totalValues}) no permiten reconstruir {frames} frames de {perFrame} valores.", width, height, null);
                    }
                }

                if (framesList.Count == 0)
                    return (false, "No se encontraron frames dentro del bloque después del parseo.", width, height, null);

                return (true, "", width, height, framesList);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0, 0, null);
            }
        }

        private void ShowSimFromRgb565(List<ushort[]> framesData, int width, int height)
        {
            using (var simForm = new Form())
            {
                simForm.Text = "Simulación RGB565 (.h cargado)";
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

        private void redimensionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new ResizeForm();
            ThemeManager.ApplyTheme(form, ThemeManager.IsDark);
            form.ShowDialog(this);
        }

        private void compararToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (picPreview.Image == null)
            {
                MessageBox.Show("No hay imagen cargada para comparar.", "Comparar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImageConverter.EnableDithering = chkDither.Checked;
            ImageConverter.EnableNoiseReduction = chkNoise.Checked;
            ImageConverter.EnableSharpen = chkSharpen.Checked;

            var compareForm = new CompareForm();

            int totalFrames = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            if (usingGif && gifFrames.Length > 1)
            {
                var origArr = new Bitmap[gifFrames.Length];
                var rgb565Arr = new Bitmap[gifFrames.Length];

                var delays = new int[gifFrames.Length];

                for (int i = 0; i < gifFrames.Length; i++)
                {
                    origArr[i] = (Bitmap)gifFrames[i].Clone();
                    var rgb565 = ImageConverter.ToRGB565(gifFrames[i]);
                    rgb565Arr[i] = ConvertRgb565ToBitmap(rgb565.ToArray(), gifFrames[i].Width, gifFrames[i].Height);
                    delays[i] = 50;
                }

                try
                {
                    using var gifSource = Image.FromFile(txtFolder.Text);
                    FrameDimension fdSrc = new FrameDimension(gifSource.FrameDimensionsList[0]);
                    int count = gifSource.GetFrameCount(fdSrc);
                    for (int i = 0; i < count && i < delays.Length; i++)
                    {
                        gifSource.SelectActiveFrame(fdSrc, i);
                        int delay = gifSource.GetPropertyItem(0x5100).Value[i * 4] | (gifSource.GetPropertyItem(0x5100).Value[i * 4 + 1] << 8);
                        delays[i] = Math.Max(10, delay * 10);
                    }
                }
                catch { }

                compareForm.SetAnimatedImages(origArr, rgb565Arr, delays);
            }
            else
            {
                using var src = new Bitmap(picPreview.Image);
                var rgb565 = ImageConverter.ToRGB565(src);
                var rgb565Bmp = ConvertRgb565ToBitmap(rgb565.ToArray(), src.Width, src.Height);
                compareForm.SetImages(src, rgb565Bmp);
            }

            compareForm.ShowDialog(this);
        }

        private void exportarFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exportPngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFrames(ImageFormat.Png, ".png", -1);
        }

        private void exportJpgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFrames(ImageFormat.Jpeg, ".jpg", 85);
        }

        private void exportBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFrames(ImageFormat.Bmp, ".bmp", -1);
        }

        private async void ExportFrames(ImageFormat format, string ext, int jpegQuality)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecciona la carpeta destino para exportar los frames";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                string outDir = dlg.SelectedPath;
                _exportCts = new CancellationTokenSource();
                var ct = _exportCts.Token;

                try
                {
                    int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
                    if (total == 0) { MessageBox.Show("No hay frames cargados para exportar."); return; }

                    EncoderParameters? encoderParams = null;
                    if (jpegQuality > 0)
                    {
                        var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
                        encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(qualityEncoder, (long)jpegQuality);
                    }

                    var codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == format.Guid);

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < total; i++)
                        {
                            ct.ThrowIfCancellationRequested();

                            Bitmap bmp;
                            if (usingGif)
                                bmp = gifFrames[i];
                            else if (usingHeader && headerFrames != null)
                                bmp = ConvertRgb565ToBitmap(headerFrames[i], headerWidth, headerHeight);
                            else
                                bmp = new Bitmap(frameFiles[i]);

                            string filename = Path.Combine(outDir, $"frame_{i:000}{ext}");
                            if (encoderParams != null && codec != null)
                                bmp.Save(filename, codec, encoderParams);
                            else
                                bmp.Save(filename, format);

                            if (!usingGif && !(usingHeader && headerFrames != null)) bmp.Dispose();

                            int progress = i + 1;
                            Invoke(() => Log($"Exportado: {filename}"));
                        }
                    }, ct);

                    MessageBox.Show($"Exportacion completada: {outDir}", "Exportar Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Exportación cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exportando frames: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _exportCts?.Dispose();
                    _exportCts = null;
                }
            }
        }

        private void chkLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoop.Checked)
                chkLoop.Text = "Repetición activada";
            else
                chkLoop.Text = "Repetición desactivada";
        }

        private void chkDither_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDither.Checked)
                chkDither.Text = "Dithering activado";
            else
                chkDither.Text = "Dithering desactivado";

        }

        private void chkNoise_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNoise.Checked)
                chkNoise.Text = "Reducción de ruido activada";
            else
                chkNoise.Text = "Reducción de ruido desactivada";

        }

        private void chkSharpen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSharpen.Checked)
                chkSharpen.Text = "Enfoque activado";
            else
                chkSharpen.Text = "Enfoque desactivado";

        }

        private void claroToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ThemeManager.ApplyTheme(this, false);
            ClearCheckmarks(temaToolStripMenuItem);
            claroToolStripMenuItem.Checked = true;
            SaveConfig();
        }

        private void oscuroToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ThemeManager.ApplyTheme(this, true);
            ClearCheckmarks(temaToolStripMenuItem);
            oscuroToolStripMenuItem.Checked = true;
            SaveConfig();
        }

        private void ClearCheckmarks(ToolStripMenuItem parent)
        {
            foreach (ToolStripMenuItem item in parent.DropDownItems)
                item.Checked = false;
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            _generateCts?.Cancel();
            _exportCts?.Cancel();
            btnCancelar.Enabled = false;
            btnCancelar.Text = "Cancelando...";
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.G:
                        btnGenerate.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.O:
                        btnSelectFolder.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.L:
                        cargarHeaderToolStripMenuItem.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        if (picPreview.SizeMode == PictureBoxSizeMode.Zoom)
                            picPreview.SizeMode = PictureBoxSizeMode.CenterImage;
                        e.Handled = true;
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        if (picPreview.SizeMode == PictureBoxSizeMode.CenterImage)
                            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
                        e.Handled = true;
                        break;
                }
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (btnPlay.Enabled) btnPlay.PerformClick();
                    else if (btnStop.Enabled) btnStop.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.Left:
                    if (btnPrev.Enabled) btnPrev.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.Right:
                    if (btnNext.Enabled) btnNext.PerformClick();
                    e.Handled = true;
                    break;
            }
        }

        private void picPreview_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (picPreview.Image == null) return;

            if (e.Delta > 0)
                picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            else
                picPreview.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0) return;

            string path = files[0];

            if (Directory.Exists(path))
            {
                framesFolder = path;
                LoadFrames();
                AddToRecentFiles(path);
            }
            else if (path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            {
                LoadGif(path);
                AddToRecentFiles(path);
            }
            else if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                framesFolder = Path.GetDirectoryName(path) ?? "";
                LoadFrames();
                AddToRecentFiles(path);
            }
            else if (path.EndsWith(".h", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = ParseHeaderFile(path);
                    if (!result.Success)
                    {
                        Log($"Error parseando header: {result.Error}");
                        return;
                    }

                    headerFrames = result.FramesData ?? new List<ushort[]>();
                    headerWidth = result.Width;
                    headerHeight = result.Height;
                    usingHeader = true;
                    usingGif = false;
                    frameFiles = Array.Empty<string>();
                    gifFrames = Array.Empty<Bitmap>();
                    picRGB565.Image?.Dispose();
                    picRGB565.Image = null;

                    lstFrames.Items.Clear();
                    for (int i = 0; i < headerFrames.Count; i++)
                        lstFrames.Items.Add($"Frame {i}");

                    if (headerFrames.Count > 0)
                    {
                        currentFrameIndex = 0;
                        lstFrames.SelectedIndex = 0;
                        picPreview.Image = ConvertRgb565ToBitmap(headerFrames[0], headerWidth, headerHeight);
                        ShowRgb565Preview(0);
                    }

                    Log($"Header cargado: {path} - frames: {result.FramesData?.Count ?? 0}");
                    AddToRecentFiles(path);
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    Log($"Error leyendo header: {ex.Message}");
                }
            }
        }

        private void RefreshFrameList()
        {
            lstFrames.Items.Clear();

            if (usingGif)
            {
                for (int i = 0; i < gifFrames.Length; i++)
                    lstFrames.Items.Add($"Frame {i}");
            }
            else if (usingHeader && headerFrames != null)
            {
                for (int i = 0; i < headerFrames.Count; i++)
                    lstFrames.Items.Add($"Frame {i}");
            }
            else
            {
                foreach (var f in frameFiles)
                    lstFrames.Items.Add(Path.GetFileName(f));
            }

            UpdateFrameButtons();
            UpdateStatusBar();
        }

        private void UpdateFrameButtons()
        {
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            bool hasFrames = total > 0;
            bool hasSelection = lstFrames.SelectedIndex >= 0;

            btnMoveUp.Enabled = hasSelection && lstFrames.SelectedIndex > 0;
            btnMoveDown.Enabled = hasSelection && lstFrames.SelectedIndex < total - 1;
            btnDelete.Enabled = hasSelection;
        }

        private void btnMoveUp_Click(object? sender, EventArgs e)
        {
            int idx = lstFrames.SelectedIndex;
            if (idx <= 0) return;

            if (usingGif)
            {
                var tmp = gifFrames[idx];
                gifFrames[idx] = gifFrames[idx - 1];
                gifFrames[idx - 1] = tmp;
            }
            else if (usingHeader && headerFrames != null)
            {
                var tmp = headerFrames[idx];
                headerFrames[idx] = headerFrames[idx - 1];
                headerFrames[idx - 1] = tmp;
            }
            else if (frameFiles.Length > 0)
            {
                var tmp = frameFiles[idx];
                frameFiles[idx] = frameFiles[idx - 1];
                frameFiles[idx - 1] = tmp;

                try
                {
                    string dir = Path.GetDirectoryName(tmp) ?? "";
                    string ext1 = Path.GetExtension(frameFiles[idx]);
                    string ext2 = Path.GetExtension(frameFiles[idx - 1]);
                    string tempPath = Path.Combine(dir, "__reorder_temp__" + ext1);
                    File.Move(frameFiles[idx], tempPath);
                    File.Move(frameFiles[idx - 1], frameFiles[idx]);
                    File.Move(tempPath, frameFiles[idx - 1]);
                }
                catch (Exception ex)
                {
                    Log($"Error reordenando archivos: {ex.Message}");
                }
            }

            RefreshFrameList();
            lstFrames.SelectedIndex = idx - 1;
            currentFrameIndex = idx - 1;

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else if (usingHeader && headerFrames != null)
                picPreview.Image = ConvertRgb565ToBitmap(headerFrames[currentFrameIndex], headerWidth, headerHeight);
            else if (frameFiles.Length > 0)
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);
            ShowRgb565Preview(currentFrameIndex);
        }

        private void btnMoveDown_Click(object? sender, EventArgs e)
        {
            int idx = lstFrames.SelectedIndex;
            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);
            if (idx < 0 || idx >= total - 1) return;

            if (usingGif)
            {
                var tmp = gifFrames[idx];
                gifFrames[idx] = gifFrames[idx + 1];
                gifFrames[idx + 1] = tmp;
            }
            else if (usingHeader && headerFrames != null)
            {
                var tmp = headerFrames[idx];
                headerFrames[idx] = headerFrames[idx + 1];
                headerFrames[idx + 1] = tmp;
            }
            else if (frameFiles.Length > 0)
            {
                var tmp = frameFiles[idx];
                frameFiles[idx] = frameFiles[idx + 1];
                frameFiles[idx + 1] = tmp;

                try
                {
                    string dir = Path.GetDirectoryName(tmp) ?? "";
                    string ext1 = Path.GetExtension(frameFiles[idx]);
                    string ext2 = Path.GetExtension(frameFiles[idx + 1]);
                    string tempPath = Path.Combine(dir, "__reorder_temp__" + ext1);
                    File.Move(frameFiles[idx], tempPath);
                    File.Move(frameFiles[idx + 1], frameFiles[idx]);
                    File.Move(tempPath, frameFiles[idx + 1]);
                }
                catch (Exception ex)
                {
                    Log($"Error reordenando archivos: {ex.Message}");
                }
            }

            RefreshFrameList();
            lstFrames.SelectedIndex = idx + 1;
            currentFrameIndex = idx + 1;

            if (usingGif)
                picPreview.Image = gifFrames[currentFrameIndex];
            else if (usingHeader && headerFrames != null)
                picPreview.Image = ConvertRgb565ToBitmap(headerFrames[currentFrameIndex], headerWidth, headerHeight);
            else if (frameFiles.Length > 0)
                picPreview.Image = Image.FromFile(frameFiles[currentFrameIndex]);
            ShowRgb565Preview(currentFrameIndex);
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            int idx = lstFrames.SelectedIndex;
            if (idx < 0) return;

            int total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            var result = MessageBox.Show(
                $"¿Eliminar el frame {idx}?",
                "Eliminar frame",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            if (usingGif)
            {
                if (gifFrames.Length == 1)
                {
                    gifFrames = Array.Empty<Bitmap>();
                }
                else
                {
                    var newList = new List<Bitmap>(gifFrames);
                    newList[idx].Dispose();
                    newList.RemoveAt(idx);
                    gifFrames = newList.ToArray();
                }
            }
            else if (usingHeader && headerFrames != null)
            {
                if (headerFrames.Count == 1)
                {
                    headerFrames = null;
                    usingHeader = false;
                }
                else
                {
                    headerFrames.RemoveAt(idx);
                }
            }
            else if (frameFiles.Length > 0)
            {
                try
                {
                    File.Delete(frameFiles[idx]);
                }
                catch (Exception ex)
                {
                    Log($"Error eliminando archivo: {ex.Message}");
                }

                var newList = new List<string>(frameFiles);
                newList.RemoveAt(idx);
                frameFiles = newList.ToArray();
            }

            total = usingGif ? gifFrames.Length : (usingHeader && headerFrames != null ? headerFrames.Count : frameFiles.Length);

            RefreshFrameList();

            if (total == 0)
            {
                currentFrameIndex = 0;
                picPreview.Image = null;
                picRGB565.Image?.Dispose();
                picRGB565.Image = null;
                Log("Sin frames");
            }
            else
            {
                int newIdx = Math.Min(idx, total - 1);
                lstFrames.SelectedIndex = newIdx;
                currentFrameIndex = newIdx;

                if (usingGif)
                    picPreview.Image = gifFrames[newIdx];
                else if (usingHeader && headerFrames != null)
                    picPreview.Image = ConvertRgb565ToBitmap(headerFrames[newIdx], headerWidth, headerHeight);
                else if (frameFiles.Length > 0)
                    picPreview.Image = Image.FromFile(frameFiles[newIdx]);
                ShowRgb565Preview(newIdx);
            }
        }
    }
}
