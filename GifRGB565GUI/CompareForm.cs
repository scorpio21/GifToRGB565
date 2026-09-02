using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

            Controls.Add(picRight);
            Controls.Add(picLeft);
            Controls.Add(picWipe);
            Controls.Add(sliderWipe);
            Controls.Add(lblWipe);
            Controls.Add(btnOverlay);
            Controls.Add(btnWipe);
            Controls.Add(btnSideBySide);

            UpdateLayout();
        }

        public void SetImages(Bitmap original, Bitmap rgb565)
        {
            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            originalBmp = (Bitmap)original.Clone();
            rgb565Bmp = (Bitmap)rgb565.Clone();

            picLeft.Image = originalBmp;
            picRight.Image = rgb565Bmp;
            picWipe.Image = originalBmp;

            UpdateWipe();
        }

        private void UpdateLayout()
        {
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
                picLeft.SetBounds(0, btnSideBySide.Height, ClientSize.Width, ClientSize.Height - btnSideBySide.Height - sliderWipe.Height - lblWipe.Height);
                picRight.SetBounds(0, btnSideBySide.Height, ClientSize.Width, ClientSize.Height - btnSideBySide.Height - sliderWipe.Height - lblWipe.Height);
                picWipe.SetBounds(0, btnSideBySide.Height, ClientSize.Width, ClientSize.Height - btnSideBySide.Height - sliderWipe.Height - lblWipe.Height);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            originalBmp?.Dispose();
            rgb565Bmp?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
