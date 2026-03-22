// Controls/ModernIconButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    /// <summary>
    /// زر أيقونة دائري — يُستخدم في TopBar (notifications, apps, search)
    /// وفي صفوف الجدول (edit, delete, replay)
    /// </summary>
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernIconButton : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private Image? _icon;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private IconButtonStyle _style = IconButtonStyle.RoundGhost;

        private Color _hoverColor   = Color.FromArgb(236, 236, 255);   // surface-container
        private Color _normalColor  = Color.Transparent;
        private Color _accentColor  = Color.FromArgb(85, 69, 205);     // primary
        private Color _dangerColor  = Color.FromArgb(186, 26, 26);     // error
        private Color _iconColor    = Color.FromArgb(100, 100, 130);   // on-surface-variant

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(IconButtonStyle.RoundGhost)]
        public IconButtonStyle ButtonStyle
        {
            get => _style;
            set { _style = value; Invalidate(); }
        }

        /// <summary>لون الأيقونة الطبيعي</summary>
        [Category("BChat")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernIconButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(38, 38);
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
            g.Clear(Parent?.BackColor ?? Color.WhiteSmoke);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            bool isSquare = _style == IconButtonStyle.SquareGhost ||
                            _style == IconButtonStyle.SquareEdit  ||
                            _style == IconButtonStyle.SquareDelete;

            // ── Background ────────────────────────────────────
            if (_isHovered || _isPressed)
            {
                Color hoverBg = _style == IconButtonStyle.SquareDelete
                    ? Color.FromArgb(30, 186, 26, 26)    // خفيف أحمر للحذف
                    : _hoverColor;

                using var path = isSquare
                    ? RoundedRect(rect, 8)
                    : CirclePath(rect);
                using var brush = new SolidBrush(hoverBg);
                g.FillPath(brush, path);
            }

            // ── Icon ──────────────────────────────────────────
            if (_icon != null)
            {
                int padding = _isPressed ? 10 : 8;
                var iconRect = new Rectangle(padding, padding,
                    Width - padding * 2, Height - padding * 2);

                // تلوين الأيقونة عند Hover
                if (_isHovered)
                {
                    Color tintColor = _style == IconButtonStyle.SquareDelete ? _dangerColor
                                    : _style == IconButtonStyle.SquareEdit   ? _accentColor
                                    : _accentColor;
                    DrawTintedImage(g, _icon, iconRect, tintColor);
                }
                else
                {
                    DrawTintedImage(g, _icon, iconRect, _iconColor);
                }
            }
        }

        /// <summary>يرسم الصورة مع تلوينها بلون معين</summary>
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

    public enum IconButtonStyle
    {
        /// <summary>دائرة — TopBar (notifications, apps...)</summary>
        RoundGhost,
        /// <summary>مربع — header/toolbar actions</summary>
        SquareGhost,
        /// <summary>مربع — Edit action في الجدول (hover = primary)</summary>
        SquareEdit,
        /// <summary>مربع — Delete action في الجدول (hover = red)</summary>
        SquareDelete
    }
}
