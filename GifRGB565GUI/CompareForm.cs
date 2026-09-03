using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    public class CompareForm : Form
    {
        private Bitmap? originalBmp;
        private Bitmap? rgb565Bmp;
        private PictureBox picLeft;
        private PictureBox picRight;
        private PictureBox picWipe;
        private TrackBar sliderWipe;
        private Label lblWipe;
        private Button btnSideBySide;
        private Button btnWipe;
        private Button btnOverlay;
        private enum CompareMode { SideBySide, Wipe, Overlay }
        private CompareMode mode = CompareMode.SideBySide;

        private Bitmap[]? originalFrames;
        private Bitmap[]? rgb565Frames;
        private int[]? frameDelays;
        private int currentFrameIndex = 0;
        private bool isAnimating = false;
        private System.Windows.Forms.Timer animTimer;
        private Button btnPlay;
        private Button btnStop;
        private TrackBar speedSlider;
        private Label lblSpeed;
        private Label lblFrame;

        public CompareForm()
        {
            Text = "Comparación Original vs RGB565";
            Size = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 400);

            picLeft = new PictureBox { Dock = DockStyle.Left, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Black };
            picRight = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Black };
            picWipe = new PictureBox { Dock = DockStyle.None, SizeMode = PictureBoxSizeMode.Normal, BackColor = Color.Black, Visible = false };

            sliderWipe = new TrackBar { Minimum = 0, Maximum = 100, Value = 50, Dock = DockStyle.Bottom, TickFrequency = 10 };
            sliderWipe.Scroll += (s, e) => UpdateWipe();

            lblWipe = new Label { Text = "50%", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Height = 20 };

            btnSideBySide = new Button { Text = "Lado a lado", Dock = DockStyle.Top, Height = 30 };
            btnSideBySide.Click += (s, e) => { mode = CompareMode.SideBySide; UpdateLayout(); };

            btnWipe = new Button { Text = "Wipe (división)", Dock = DockStyle.Top, Height = 30 };
            btnWipe.Click += (s, e) => { mode = CompareMode.Wipe; UpdateLayout(); };

            btnOverlay = new Button { Text = "Superpuesta", Dock = DockStyle.Top, Height = 30 };
            btnOverlay.Click += (s, e) => { mode = CompareMode.Overlay; UpdateLayout(); };

            btnPlay = new Button { Text = "▶ Play", Dock = DockStyle.Top, Height = 30, Enabled = false };
            btnPlay.Click += BtnPlay_Click;

            btnStop = new Button { Text = "■ Stop", Dock = DockStyle.Top, Height = 30, Enabled = false };
            btnStop.Click += BtnStop_Click;

            speedSlider = new TrackBar { Minimum = 10, Maximum = 500, Value = 50, Dock = DockStyle.Bottom, TickFrequency = 50 };
            speedSlider.Scroll += (s, e) =>
            {
                if (animTimer.Enabled)
                    animTimer.Interval = speedSlider.Value;
                lblSpeed.Text = $"{speedSlider.Value} ms";
            };

            lblSpeed = new Label { Text = "50 ms", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Height = 20 };
            lblFrame = new Label { Text = "", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Height = 20, Visible = false };

            animTimer = new System.Windows.Forms.Timer { Interval = 50 };
            animTimer.Tick += AnimTimer_Tick;

            Controls.Add(picRight);
            Controls.Add(picLeft);
            Controls.Add(picWipe);
            Controls.Add(sliderWipe);
            Controls.Add(lblWipe);
            Controls.Add(lblFrame);
            Controls.Add(lblSpeed);
            Controls.Add(speedSlider);
            Controls.Add(btnStop);
            Controls.Add(btnPlay);
            Controls.Add(btnOverlay);
            Controls.Add(btnWipe);
            Controls.Add(btnSideBySide);

            UpdateLayout();
        }

        public void SetImages(Bitmap original, Bitmap rgb565)
        {
            StopAnimation();
            DisposeFrameArrays();
            currentFrameIndex = 0;

            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            originalBmp = (Bitmap)original.Clone();
            rgb565Bmp = (Bitmap)rgb565.Clone();

            picLeft.Image = originalBmp;
            picRight.Image = rgb565Bmp;

            btnPlay.Enabled = false;
            btnStop.Enabled = false;
            lblFrame.Visible = false;

            UpdateWipe();
        }

        public void SetAnimatedImages(Bitmap[] origFrames, Bitmap[] rgb565FramesArr, int[] delays)
        {
            StopAnimation();
            DisposeFrameArrays();
            currentFrameIndex = 0;

            originalFrames = origFrames;
            rgb565Frames = rgb565FramesArr;
            frameDelays = delays;

            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            originalBmp = new Bitmap(originalFrames[0]);
            rgb565Bmp = new Bitmap(rgb565Frames[0]);

            picLeft.Image = originalBmp;
            picRight.Image = rgb565Bmp;

            int defaultDelay = delays.Length > 0 ? delays[0] : 50;
            animTimer.Interval = defaultDelay;
            speedSlider.Value = Math.Max(speedSlider.Minimum, Math.Min(speedSlider.Maximum, defaultDelay));
            lblSpeed.Text = $"{defaultDelay} ms";

            bool hasMultiple = originalFrames.Length > 1;
            btnPlay.Enabled = hasMultiple;
            btnStop.Enabled = false;
            lblFrame.Visible = hasMultiple;
            UpdateFrameLabel();

            UpdateWipe();
        }

        private void BtnPlay_Click(object? sender, EventArgs e)
        {
            if (originalFrames == null || originalFrames.Length <= 1) return;

            animTimer.Interval = Math.Max(1, speedSlider.Value);
            animTimer.Start();
            isAnimating = true;
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            StopAnimation();
        }

        private void StopAnimation()
        {
            animTimer.Stop();
            isAnimating = false;
            bool hasMultiple = originalFrames != null && originalFrames.Length > 1;
            btnPlay.Enabled = hasMultiple;
            btnStop.Enabled = false;
        }

        private void AnimTimer_Tick(object? sender, EventArgs e)
        {
            if (originalFrames == null || rgb565Frames == null) return;
            int total = originalFrames.Length;
            if (total <= 1) return;

            currentFrameIndex++;
            if (currentFrameIndex >= total)
                currentFrameIndex = 0;

            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            originalBmp = new Bitmap(originalFrames[currentFrameIndex]);
            rgb565Bmp = new Bitmap(rgb565Frames[currentFrameIndex]);

            picLeft.Image = originalBmp;
            picRight.Image = rgb565Bmp;

            if (frameDelays != null && currentFrameIndex < frameDelays.Length)
            {
                animTimer.Interval = frameDelays[currentFrameIndex];
                speedSlider.Value = Math.Max(speedSlider.Minimum, Math.Min(speedSlider.Maximum, frameDelays[currentFrameIndex]));
                lblSpeed.Text = $"{frameDelays[currentFrameIndex]} ms";
            }

            UpdateFrameLabel();

            if (mode == CompareMode.Wipe)
                UpdateWipe();
            else if (mode == CompareMode.Overlay)
                picLeft.Image = rgb565Bmp;
        }

        private void UpdateFrameLabel()
        {
            if (originalFrames != null && originalFrames.Length > 1)
                lblFrame.Text = $"Frame {currentFrameIndex + 1}/{originalFrames.Length}";
            else
                lblFrame.Text = "";
        }

        private void UpdateLayout()
        {
            bool hasAnim = originalFrames != null && originalFrames.Length > 1;
            btnPlay.Visible = hasAnim;
            btnStop.Visible = hasAnim;
            speedSlider.Visible = hasAnim;
            lblSpeed.Visible = hasAnim;

            sliderWipe.Visible = mode == CompareMode.Wipe;
            lblWipe.Visible = mode == CompareMode.Wipe;
            picWipe.Visible = mode == CompareMode.Wipe;

            if (mode == CompareMode.SideBySide)
            {
                picLeft.Visible = true;
                picRight.Visible = true;
                picLeft.Dock = DockStyle.Left;
                picLeft.Width = ClientSize.Width / 2;
                picRight.Dock = DockStyle.Fill;
                picWipe.Visible = false;
            }
            else if (mode == CompareMode.Wipe)
            {
                picLeft.Visible = true;
                picRight.Visible = true;
                picLeft.Dock = DockStyle.None;
                picRight.Dock = DockStyle.None;
                int topOffset = btnSideBySide.Height;
                int bottomOffset = sliderWipe.Height + lblWipe.Height;
                if (hasAnim) bottomOffset += speedSlider.Height + lblSpeed.Height;
                picLeft.SetBounds(0, topOffset, ClientSize.Width, ClientSize.Height - topOffset - bottomOffset);
                picRight.SetBounds(0, topOffset, ClientSize.Width, ClientSize.Height - topOffset - bottomOffset);
                picWipe.SetBounds(0, topOffset, ClientSize.Width, ClientSize.Height - topOffset - bottomOffset);
                picWipe.Visible = true;
                UpdateWipe();
            }
            else // Overlay
            {
                picLeft.Visible = true;
                picRight.Visible = false;
                picLeft.Dock = DockStyle.Fill;
                picLeft.Image = rgb565Bmp;
                picWipe.Visible = false;
            }
        }

        private void UpdateWipe()
        {
            if (originalBmp == null || rgb565Bmp == null) return;
            if (picWipe.Width <= 0 || picWipe.Height <= 0) return;
            int splitX = picWipe.Width * sliderWipe.Value / 100;
            lblWipe.Text = $"{sliderWipe.Value}%";

            var bmp = new Bitmap(picWipe.Width, picWipe.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(originalBmp, 0, 0, picWipe.Width, picWipe.Height);
                g.SetClip(new Rectangle(splitX, 0, picWipe.Width - splitX, picWipe.Height));
                g.DrawImage(rgb565Bmp, 0, 0, picWipe.Width, picWipe.Height);
            }
            picWipe.Image?.Dispose();
            picWipe.Image = bmp;
        }

        private void DisposeFrameArrays()
        {
            if (originalFrames != null)
            {
                for (int i = 0; i < originalFrames.Length; i++)
                    originalFrames[i].Dispose();
                originalFrames = null;
            }
            if (rgb565Frames != null)
            {
                for (int i = 0; i < rgb565Frames.Length; i++)
                    rgb565Frames[i].Dispose();
                rgb565Frames = null;
            }
            frameDelays = null;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopAnimation();
            animTimer.Dispose();
            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            DisposeFrameArrays();
            base.OnFormClosing(e);
        }
    }
}
