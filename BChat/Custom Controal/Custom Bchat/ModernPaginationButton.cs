// Controls/ModernPaginationButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    /// <summary>
    /// زر الصفحات — مربع صغير 32×32.
    /// IsActive = true → خلفية بنفسجية + نص أبيض.
    /// </summary>
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernPaginationButton : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private bool _isActive  = false;
        private bool _isHovered = false;
        private bool _isPressed = false;

        private Color _activeBg   = Color.FromArgb(85, 69, 205);      // primary
        private Color _activeFg   = Color.White;
        private Color _normalFg   = Color.FromArgb(71, 69, 84);       // on-surface-variant
        private Color _hoverBg    = Color.FromArgb(236, 236, 255);    // surface-container
        private Color _borderColor= Color.FromArgb(50, 200, 196, 214);

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        [DefaultValue(false)]
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernPaginationButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(32, 32);
            Font = new Font("IBM Plex Sans Arabic", 9f, FontStyle.Bold);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true; Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; _isPressed = false; Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true; Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false; Invalidate();
            base.OnMouseUp(e);
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent?.BackColor ?? Color.White);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedRect(rect, 8);

            if (_isActive)
            {
                using var bgBrush = new SolidBrush(_activeBg);
                g.FillPath(bgBrush, path);
            }
            else if (_isHovered)
            {
                using var bgBrush = new SolidBrush(_hoverBg);
                g.FillPath(bgBrush, path);
            }
            else
            {
                using var pen = new Pen(_borderColor, 1f);
                g.DrawPath(pen, path);
            }

            // ── Text / Icon ───────────────────────────────────
            if (!string.IsNullOrEmpty(Text))
            {
                Color fg = _isActive ? _activeFg : _normalFg;
                using var brush = new SolidBrush(fg);
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
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
    }


    // ════════════════════════════════════════════════════════════
    //  ModernToolbarButton — أزرار تنسيق النص (Bold, Italic...)
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// زر صغير داخل شريط الـ Toolbar فوق حقل النص.
    /// يدعم حالة IsToggled (مثلاً: Bold مفعّل).
    /// </summary>
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernToolbarButton : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private Image? _icon;
        private bool _isToggled = false;
        private bool _isHovered = false;
        private bool _isPressed = false;

        private Color _toggledBg   = Color.White;
        private Color _hoverBg     = Color.White;
        private Color _normalColor = Color.FromArgb(87, 92, 126);     // secondary
        private Color _activeColor = Color.FromArgb(85, 69, 205);     // primary

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        public bool IsToggled
        {
            get => _isToggled;
            set { _isToggled = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernToolbarButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(30, 30);
            Font = new Font("IBM Plex Sans Arabic", 10f, FontStyle.Bold);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true; Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; _isPressed = false; Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true; Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            // Toggle عند الضغط
            _isToggled = !_isToggled;
            Invalidate();
            base.OnMouseUp(e);
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Color.FromArgb(236, 236, 255));

            var rect = new Rectangle(1, 1, Width - 3, Height - 3);
            using var path = RoundedRect(rect, 5);

            // ── Background ────────────────────────────────────
            if (_isToggled || _isHovered || _isPressed)
            {
                using var bgBrush = new SolidBrush(_toggledBg);
                g.FillPath(bgBrush, path);
            }

            // ── Icon or Text ──────────────────────────────────
            Color iconColor = _isToggled ? _activeColor : _normalColor;

            if (_icon != null)
            {
                int p = 6;
                var iconRect = new Rectangle(p, p, Width - p * 2, Height - p * 2);
                DrawTintedImage(g, _icon, iconRect, iconColor);
            }
            else if (!string.IsNullOrEmpty(Text))
            {
                using var brush = new SolidBrush(iconColor);
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
        }

        // ─── Helpers ──────────────────────────────────────────
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
    }
}
