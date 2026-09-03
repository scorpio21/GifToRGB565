using System.Drawing;
using System.Windows.Forms;

namespace GifRGB565GUI
{
    public static class ThemeManager
    {
        public static bool IsDark { get; private set; } = false;

        private static readonly Color DarkBg = Color.FromArgb(30, 30, 30);
        private static readonly Color DarkFg = Color.FromArgb(212, 212, 212);
        private static readonly Color DarkControl = Color.FromArgb(45, 45, 48);
        private static readonly Color DarkMenuBg = Color.FromArgb(37, 37, 38);
        private static readonly Color DarkPanel = Color.FromArgb(35, 35, 45);

        private static readonly Color LightBg = SystemColors.Control;
        private static readonly Color LightFg = SystemColors.ControlText;
        private static readonly Color LightControl = SystemColors.Window;
        private static readonly Color LightMenuBg = SystemColors.MenuBar;

        public static void ApplyTheme(Form form, bool dark)
        {
            IsDark = dark;
            Color bg = dark ? DarkBg : LightBg;
            Color fg = dark ? DarkFg : LightFg;
            Color ctrl = dark ? DarkControl : LightControl;
            Color menuBg = dark ? DarkMenuBg : LightMenuBg;

            form.BackColor = bg;
            form.ForeColor = fg;

            foreach (Control c in form.Controls)
                ApplyToControl(c, dark, bg, fg, ctrl, menuBg);
        }

        private static void ApplyToControl(Control c, bool dark, Color bg, Color fg, Color ctrl, Color menuBg)
        {
            if (c is MenuStrip ms)
            {
                ms.BackColor = menuBg;
                ms.ForeColor = fg;
                foreach (ToolStripMenuItem item in ms.Items)
                    ApplyToMenuItem(item, dark, fg, menuBg);
                return;
            }

            if (c is TextBox || c is ListBox)
            {
                c.BackColor = ctrl;
                c.ForeColor = fg;
            }
            else if (c is PictureBox || c is ProgressBar)
            {
                c.BackColor = dark ? Color.FromArgb(51, 51, 51) : SystemColors.Control;
            }
            else if (c is Button btn)
            {
                if (dark)
                {
                    btn.BackColor = Color.FromArgb(62, 62, 66);
                    btn.ForeColor = fg;
                }
                else
                {
                    btn.BackColor = SystemColors.Control;
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            else if (c is CheckBox)
            {
                if (dark)
                {
                    c.BackColor = Color.FromArgb(62, 62, 66);
                    c.ForeColor = fg;
                }
                else
                {
                    c.BackColor = SystemColors.Control;
                    c.ForeColor = SystemColors.ControlText;
                }
            }
            else if (c is ComboBox cmb)
            {
                cmb.BackColor = ctrl;
                cmb.ForeColor = fg;
            }
            else if (c is Panel pnl)
            {
                if (dark)
                {
                    if (pnl.Dock == DockStyle.Top || pnl.Dock == DockStyle.Bottom || pnl.Dock == DockStyle.Left || pnl.Dock == DockStyle.Right)
                        pnl.BackColor = Color.FromArgb(50, 50, 60);
                    else
                        pnl.BackColor = DarkPanel;
                }
                else
                {
                    pnl.BackColor = SystemColors.Control;
                }
            }
            else if (c is Label || c is TrackBar)
            {
                c.BackColor = bg;
                c.ForeColor = fg;
            }

            if (c.HasChildren)
            {
                foreach (Control child in c.Controls)
                    ApplyToControl(child, dark, bg, fg, ctrl, menuBg);
            }
        }

        private static void ApplyToMenuItem(ToolStripMenuItem item, bool dark, Color fg, Color menuBg)
        {
            item.ForeColor = fg;
            item.BackColor = menuBg;
            foreach (ToolStripItem sub in item.DropDownItems)
            {
                if (sub is ToolStripMenuItem mi)
                    ApplyToMenuItem(mi, dark, fg, menuBg);
            }
        }
    }
}
