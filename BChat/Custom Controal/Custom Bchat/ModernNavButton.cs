// Controls/ModernNavButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    /// <summary>
    /// زر القائمة الجانبية.
    /// عند IsActive = true يظهر الشريط البنفسجي على اليمين + نص ملون.
    /// </summary>
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernNavButton : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private Image? _icon;
        private bool _isActive = false;
        private bool _isHovered = false;

        private Color _activeBg      = Color.FromArgb(37, 43, 74);    // sidebar-hover #252B4A
        private Color _activeText    = Color.FromArgb(124, 111, 247); // sidebar-accent #7C6FF7
        private Color _activeBar     = Color.FromArgb(124, 111, 247); // شريط يمين
        private Color _hoverBg       = Color.FromArgb(37, 43, 74);
        private Color _normalText    = Color.FromArgb(148, 163, 184); // slate-400
        private Color _hoverText     = Color.White;
        private int   _activeBarW    = 4;

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernNavButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(220, 46);
            Font = new Font("IBM Plex Sans Arabic", 9.5f,
                            FontStyle.Bold);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            RightToLeft = RightToLeft.Yes;
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true; Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; Invalidate();
            base.OnMouseLeave(e);
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent?.BackColor ?? Color.FromArgb(26, 31, 60));

            bool showBg = _isActive || _isHovered;

            // ── Background ────────────────────────────────────
            if (showBg)
            {
                // شكل مستطيل مع حواف دائرية فقط على اليسار
                var bgRect = new Rectangle(0, 2, Width - (_isActive ? _activeBarW : 0), Height - 4);
                using var bgBrush = new SolidBrush(_isActive ? _activeBg : _hoverBg);
                using var bgPath  = LeftRoundedRect(bgRect, 8);
                g.FillPath(bgBrush, bgPath);
            }

            // ── Active Bar (شريط أيمن بنفسجي) ────────────────
            if (_isActive)
            {
                using var barBrush = new SolidBrush(_activeBar);
                var barRect = new Rectangle(Width - _activeBarW, 2, _activeBarW, Height - 4);
                using var barPath = RoundedRect(barRect, 3);
                g.FillPath(barBrush, barPath);
            }

            // ── Icon ──────────────────────────────────────────
            Color textColor = _isActive ? _activeText
                            : _isHovered ? _hoverText
                            : _normalText;

            if (_icon != null)
            {
                int iconSize = 20;
                int iconX = Width - 46;          // يمين مع padding
                int iconY = (Height - iconSize) / 2;
                DrawTintedImage(g, _icon,
                    new Rectangle(iconX, iconY, iconSize, iconSize), textColor);
            }

            // ── Text ──────────────────────────────────────────
            if (!string.IsNullOrEmpty(Text))
            {
                using var brush = new SolidBrush(textColor);
                var textRect = new RectangleF(0, 0, Width - 58, Height);
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Text, Font, brush, textRect, sf);
            }
        }

        // ─── Helpers ──────────────────────────────────────────
        /// <summary>مستطيل بحواف دائرية على الجانب الأيسر فقط</summary>
        private static GraphicsPath LeftRoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            // top-right: حاد
            path.AddLine(r.Right, r.Y, r.Right, r.Bottom);
            // bottom-left: دائري
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            // top-left: دائري
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawTintedImage(Graphics g, Image img, Rectangle rect, Color tint)
        {
            var cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new[] { tint.R/255f, 0, 0, 0, 0 },
                new[] { 0f, tint.G/255f, 0, 0, 0 },
                new[] { 0f, 0, tint.B/255f, 0, 0 },
                new[] { 0f, 0, 0, 1f, 0 },
                new[] { 0f, 0, 0, 0, 1f }
            });
            var attr = new System.Drawing.Imaging.ImageAttributes();
            attr.SetColorMatrix(cm);
            g.DrawImage(img, rect, 0, 0, img.Width, img.Height,
                GraphicsUnit.Pixel, attr);
        }
    }
}
