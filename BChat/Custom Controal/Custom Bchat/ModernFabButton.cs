// Controls/ModernFabButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    /// <summary>
    /// زر FAB — يظهر ثابتاً أسفل يسار الشاشة.
    /// نوعان: دائري فقط أيقونة (FabType.Icon) أو ممتد مع نص (FabType.Extended)
    /// </summary>
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernFabButton : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private FabType _fabType = FabType.Icon;
        private Image? _icon;
        private bool _isHovered = false;
        private bool _isPressed = false;

        private Color _primaryColor = Color.FromArgb(124, 111, 247);   // accent #7C6FF7
        private Color _fgColor      = Color.White;

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        [DefaultValue(FabType.Icon)]
        public FabType FabType
        {
            get => _fabType;
            set
            {
                _fabType = value;
                // ضبط الحجم تلقائياً
                Size = value == FabType.Extended
                    ? new Size(180, 54)
                    : new Size(64, 64);
                Invalidate();
            }
        }

        [Category("BChat")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat")]
        public Color FabColor
        {
            get => _primaryColor;
            set { _primaryColor = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernFabButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(64, 64);
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
            _isPressed = false; Invalidate();
            base.OnMouseUp(e);
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent?.BackColor ?? Color.Transparent);

            // Scale effect when pressed
            if (_isPressed)
            {
                g.TranslateTransform(Width * 0.03f, Height * 0.03f);
                g.ScaleTransform(0.94f, 0.94f);
            }

            // ── Shadow ────────────────────────────────────────
            if (!_isPressed)
                DrawShadow(g);

            // ── Background ────────────────────────────────────
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Color bg = _isHovered
                ? ControlPaint.Dark(_primaryColor, 0.06f)
                : _primaryColor;

            using var path = _fabType == FabType.Icon
                ? CirclePath(rect)
                : RoundedRect(rect, Height / 2);

            using var bgBrush = new SolidBrush(bg);
            g.FillPath(bgBrush, path);

            // ── Content ───────────────────────────────────────
            if (_fabType == FabType.Icon)
                DrawIconOnly(g);
            else
                DrawExtended(g);
        }

        private void DrawIconOnly(Graphics g)
        {
            if (_icon == null) return;
            int p = 18;
            var iconRect = new Rectangle(p, p, Width - p * 2, Height - p * 2);
            g.DrawImage(_icon, iconRect);
        }

        private void DrawExtended(Graphics g)
        {
            // Icon على اليمين + نص
            bool hasIcon = _icon != null;
            string txt = Text ?? "";
            float iconW = hasIcon ? 22f : 0f;
            float gap = (hasIcon && !string.IsNullOrEmpty(txt)) ? 8f : 0f;

            var textSize = string.IsNullOrEmpty(txt)
                ? SizeF.Empty
                : g.MeasureString(txt, Font);

            float totalW = iconW + gap + textSize.Width;
            float startX = (Width - totalW) / 2f;
            float centerY = Height / 2f;

            if (hasIcon)
            {
                var iconRect = new RectangleF(startX, centerY - iconW / 2f, iconW, iconW);
                g.DrawImage(_icon!, iconRect);
                startX += iconW + gap;
            }

            if (!string.IsNullOrEmpty(txt))
            {
                using var brush = new SolidBrush(_fgColor);
                g.DrawString(txt, Font, brush,
                    new PointF(startX, centerY - textSize.Height / 2f));
            }
        }

        private void DrawShadow(Graphics g)
        {
            // ظل خفيف بنفسجي
            for (int i = 3; i >= 1; i--)
            {
                int alpha = 12 * i;
                var shadowRect = new Rectangle(i, i + 2, Width - 1, Height - 1);
                using var shadowPath = _fabType == FabType.Icon
                    ? CirclePath(shadowRect)
                    : RoundedRect(shadowRect, Height / 2);
                using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 85, 69, 205));
                g.FillPath(shadowBrush, shadowPath);
            }
        }

        // ─── Helpers ──────────────────────────────────────────
        private static GraphicsPath CirclePath(Rectangle r)
        {
            var path = new GraphicsPath();
            path.AddEllipse(r);
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
    }

    public enum FabType
    {
        /// <summary>دائرة فقط أيقونة — 64×64</summary>
        Icon,
        /// <summary>بيضاوي مع أيقونة + نص — 180×54</summary>
        Extended
    }
}
