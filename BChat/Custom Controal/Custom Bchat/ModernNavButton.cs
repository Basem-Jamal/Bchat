// Controls/ModernNavButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernNavButton : Control
    {
        private int _cardPadding;
        private bool _activeBarFullHeight = true;
        private int _activeBarPadding = 0;
        // ─── Fields ───────────────────────────────
        private Image? _icon;
        private bool _isActive;
        private bool _isHovered;

        // ─── Colors ───────────────────────────────
        private Color _baseBackground = Color.Transparent;
        private Color _hoverBackground = Color.Transparent;
        private Color _activeBackground = Color.Transparent;

        private Color _normalText = Color.White;
        private Color _hoverText = Color.White;
        private Color _activeText = Color.White;

        private Color _activeBarColor = Color.Transparent;

        // ─── Behavior ─────────────────────────────
        private bool _useHoverEffect = true;
        private bool _useActiveEffect = true;

        // ─── Layout ───────────────────────────────
        private int _borderRadius = 8;
        private int _activeBarWidth = 4;
        private int _iconSize = 20;
        private int _contentPadding = 12;

        // ─── Properties ───────────────────────────

        [Category("BChat - Layout")]
        public int CardPadding
        {
            get => _cardPadding;
            set { _cardPadding = value; Invalidate(); }
        }

        [Category("BChat - Layout")]
        public bool ActiveBarFullHeight
        {
            get => _activeBarFullHeight;
            set { _activeBarFullHeight = value; Invalidate(); }
        }

        [Category("BChat - Layout")]
        public int ActiveBarPadding
        {
            get => _activeBarPadding;
            set { _activeBarPadding = value; Invalidate(); }
        }
        [Category("BChat - Data")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat - State")]
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        // ─── Backgrounds ─────────────────────────

        [Category("BChat - Colors")]
        public Color BaseBackground
        {
            get => _baseBackground;
            set { _baseBackground = value; Invalidate(); }
        }

        [Category("BChat - Colors")]
        public Color HoverBackground
        {
            get => _hoverBackground;
            set { _hoverBackground = value; Invalidate(); }
        }

        [Category("BChat - Colors")]
        public Color ActiveBackground
        {
            get => _activeBackground;
            set { _activeBackground = value; Invalidate(); }
        }

        // ─── Text Colors ─────────────────────────

        [Category("BChat - Colors")]
        public Color NormalTextColor
        {
            get => _normalText;
            set { _normalText = value; Invalidate(); }
        }

        [Category("BChat - Colors")]
        public Color HoverTextColor
        {
            get => _hoverText;
            set { _hoverText = value; Invalidate(); }
        }

        [Category("BChat - Colors")]
        public Color ActiveTextColor
        {
            get => _activeText;
            set { _activeText = value; Invalidate(); }
        }

        // ─── Active Bar ─────────────────────────

        [Category("BChat - Colors")]
        public Color ActiveBarColor
        {
            get => _activeBarColor;
            set { _activeBarColor = value; Invalidate(); }
        }

        // ─── Behavior ───────────────────────────

        [Category("BChat - Behavior")]
        public bool UseHoverEffect
        {
            get => _useHoverEffect;
            set { _useHoverEffect = value; Invalidate(); }
        }

        [Category("BChat - Behavior")]
        public bool UseActiveEffect
        {
            get => _useActiveEffect;
            set { _useActiveEffect = value; Invalidate(); }
        }

        // ─── Layout ─────────────────────────────

        [Category("BChat - Layout")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("BChat - Layout")]
        public int ActiveBarWidth
        {
            get => _activeBarWidth;
            set { _activeBarWidth = value; Invalidate(); }
        }

        [Category("BChat - Layout")]
        public int IconSize
        {
            get => _iconSize;
            set { _iconSize = value; Invalidate(); }
        }

        [Category("BChat - Layout")]
        public int ContentPadding
        {
            get => _contentPadding;
            set { _contentPadding = value; Invalidate(); }
        }

        // ─── Constructor ─────────────────────────
        public ModernNavButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(220, 46);
            Font = new Font("IBM Plex Sans Arabic", 9.5f, FontStyle.Bold);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            RightToLeft = RightToLeft.Yes;
        }

        // ─── Mouse ───────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        // ─── Paint ───────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(
                0, 2,
                Width - (_isActive ? _activeBarWidth : 0),
                Height - 4);

            // Base
            DrawBackground(g, rect, _baseBackground);

            // Hover
            if (_isHovered && _useHoverEffect)
                DrawBackground(g, rect, _hoverBackground);

            // Active
            if (_isActive && _useActiveEffect)
                DrawBackground(g, rect, _activeBackground);

            // ✅ Active Bar (FULL HEIGHT)
            if (_isActive && _activeBarWidth > 0)
            {
                int y = _activeBarFullHeight ? 0 : _activeBarPadding;
                int h = _activeBarFullHeight ? Height : Height - (_activeBarPadding * 2);

                var barRect = new Rectangle(
                    Width - _activeBarWidth,
                    y,
                    _activeBarWidth,
                    h);
                using var brush = new SolidBrush(_activeBarColor);

                // رسم مستقيم بدون حواف (احترافي أكثر)
                g.FillRectangle(brush, barRect);
            }

            // Text Color
            Color textColor = _normalText;

            if (_isActive && _useActiveEffect)
                textColor = _activeText;
            else if (_isHovered && _useHoverEffect)
                textColor = _hoverText;

            // Icon
            if (_icon != null)
            {
                int x = Width - _contentPadding - _iconSize;
                int y = (Height - _iconSize) / 2;

                DrawTintedImage(g, _icon,
                    new Rectangle(x, y, _iconSize, _iconSize),
                    textColor);
            }

            // Text
            using var textBrush = new SolidBrush(textColor);

            var textRect = new RectangleF(
                0,
                0,
                Width - (_icon != null ? _iconSize + _contentPadding * 2 : _contentPadding),
                Height);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(Text, Font, textBrush, textRect, sf);
        }

        // ─── Helpers ─────────────────────────────

        private void DrawBackground(Graphics g, Rectangle rect, Color color)
        {
            if (color.A == 0) return;

            using var path = LeftRoundedRect(rect, _borderRadius);
            using var brush = new SolidBrush(color);

            g.FillPath(brush, path);
        }

        private static GraphicsPath LeftRoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;

            path.AddLine(r.Right, r.Y, r.Right, r.Bottom);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.AddArc(r.X, r.Y, d, d, 180, 90);

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