// Controls/ModernButton.cs
// ✅ FontAwesome.Sharp 6.x — IconChar + IconPosition + IconTextGap
// ✅ دعم كامل للشفافية (Alpha) عبر PaintParentBackground
// ✅ بوردر مخصص مع Hover
// ✅ 6 Variants جاهزة + Custom Colors

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace BChat.Controls
{
    // ══════════════════════════════════════════════════════════
    //  Enums
    // ══════════════════════════════════════════════════════════
    public enum ButtonVariant
    {
        Primary, Secondary, Ghost, Danger, OnPrimary, CustomBasem
    }

    public enum ModernBorderStyle
    {
        None, Solid, Dashed, Dotted, DashDot, DashDotDot
    }

    public enum IconPosition
    {
        Left,   // أيقونة يسار النص
        Right   // أيقونة يمين النص (افتراضي عربي)
    }

    // ══════════════════════════════════════════════════════════
    //  ModernButton
    // ══════════════════════════════════════════════════════════
    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernButton : Control
    {
        // ─── Core Fields ──────────────────────────────────────
        private ButtonVariant _variant = ButtonVariant.Primary;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private int _borderRadius = 999;
        private bool _useCustomColors = false;

        // Custom fill colors
        private Color _customBg = Color.FromArgb(85, 69, 205);
        private Color _customBgHover = Color.FromArgb(63, 43, 184);
        private Color _customFg = Color.White;

        // ─── Icon (Font Awesome) Fields ───────────────────────
        private IconChar _iconChar = IconChar.None;
        private int _iconSize = 18;
        private IconPosition _iconPosition = IconPosition.Right;
        private int _iconTextGap = 6;

        // ─── Image Fallback Field ─────────────────────────────
        private Image? _image;

        // ─── Border Fields ────────────────────────────────────
        private ModernBorderStyle _borderStyle = ModernBorderStyle.None;
        private float _borderWidth = 1.5f;
        private Color _borderColor = Color.FromArgb(200, 196, 214);
        private Color _borderHoverColor = Color.FromArgb(85, 69, 205);
        private bool _borderUseHoverColor = false;

        // ─── Variant Palette ──────────────────────────────────
        private static readonly Color PrimaryBg = Color.FromArgb(85, 69, 205);
        private static readonly Color PrimaryHover = Color.FromArgb(63, 43, 184);
        private static readonly Color PrimaryFg = Color.White;

        private static readonly Color SecondaryBg = Color.FromArgb(240, 240, 255);
        private static readonly Color SecondaryHover = Color.FromArgb(224, 222, 255);
        private static readonly Color SecondaryFg = Color.FromArgb(87, 92, 126);

        private static readonly Color GhostBg = Color.White;
        private static readonly Color GhostHover = Color.FromArgb(228, 223, 255);
        private static readonly Color GhostFg = Color.FromArgb(85, 69, 205);
        private static readonly Color GhostBorder = Color.FromArgb(200, 196, 214);

        private static readonly Color DangerBg = Color.FromArgb(255, 218, 214);
        private static readonly Color DangerHover = Color.FromArgb(240, 185, 180);
        private static readonly Color DangerFg = Color.FromArgb(186, 26, 26);

        private static readonly Color OnPrimaryBg = Color.White;
        private static readonly Color OnPrimaryFg = Color.FromArgb(85, 69, 205);

        private static readonly Color CustomBasemBg = Color.FromArgb(87, 92, 126);
        private static readonly Color CustomBasemFg = Color.White;

        // ════════════════════════════════════════════════════
        //  PROPERTIES
        // ════════════════════════════════════════════════════

        // ─── BChat ────────────────────────────────────────────
        [Category("BChat")]
        [DefaultValue(ButtonVariant.Primary)]
        [Description("شكل الزر — يُستخدم فقط إذا UseCustomColors = false")]
        public ButtonVariant Variant
        {
            get => _variant;
            set { _variant = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        [Description("ON = استخدم ألوانك الخاصة  |  OFF = استخدم ألوان الـ Variant")]
        public bool UseCustomColors
        {
            get => _useCustomColors;
            set { _useCustomColors = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(999)]
        [Description("نصف قطر الحواف — 999 = pill كامل  |  0 = مستطيل")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        // ─── BChat - Icon (Font Awesome) ──────────────────────
        [Category("BChat - Icon")]
        [DefaultValue(IconChar.None)]
        [Description("أيقونة Font Awesome 6 Free — اختر None لإخفائها")]
        [Editor(typeof(IconCharSortedEditor), typeof(UITypeEditor))]
        public IconChar IconChar
        {
            get => _iconChar;
            set { _iconChar = value; Invalidate(); }
        }

        [Category("BChat - Icon")]
        [DefaultValue(18)]
        [Description("حجم الأيقونة بالبكسل")]
        public int IconSize
        {
            get => _iconSize;
            set { _iconSize = Math.Max(8, value); Invalidate(); }
        }

        [Category("BChat - Icon")]
        [DefaultValue(IconPosition.Right)]
        [Description("موضع الأيقونة: Right = أمام النص (للعربية) | Left = بعده")]
        public IconPosition IconPosition
        {
            get => _iconPosition;
            set { _iconPosition = value; Invalidate(); }
        }

        [Category("BChat - Icon")]
        [DefaultValue(6)]
        [Description("المسافة بين الأيقونة والنص بالبكسل")]
        public int IconTextGap
        {
            get => _iconTextGap;
            set { _iconTextGap = Math.Max(0, value); Invalidate(); }
        }

        [Category("BChat - Icon")]
        [Description("صورة بديلة (Image) — تُستخدم فقط إذا كان IconChar = None")]
        public Image? Image
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        /// <summary>
        /// توافق مع الكود القديم — نفس Image تماماً
        /// </summary>
        [Category("BChat - Icon")]
        [Description("نفس Image — للتوافق مع الكود القديم الذي يستخدم .Icon")]
        [Browsable(false)]   // مخفي في Designer لتجنب التكرار
        public Image? Icon
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        // ─── BChat - Custom Colors ────────────────────────────
        [Category("BChat - Custom Colors")]
        [Description("لون الخلفية — يشغّل UseCustomColors تلقائياً — يدعم الشفافية (Alpha)")]
        public Color CustomBackground
        {
            get => _customBg;
            set { _customBg = value; _useCustomColors = true; Invalidate(); }
        }

        [Category("BChat - Custom Colors")]
        [Description("لون الخلفية عند Hover — يشغّل UseCustomColors تلقائياً")]
        public Color CustomBackgroundHover
        {
            get => _customBgHover;
            set { _customBgHover = value; _useCustomColors = true; Invalidate(); }
        }

        [Category("BChat - Custom Colors")]
        [Description("لون النص والأيقونة — يشغّل UseCustomColors تلقائياً")]
        public Color CustomForeground
        {
            get => _customFg;
            set { _customFg = value; _useCustomColors = true; Invalidate(); }
        }

        // ─── BChat - Border ───────────────────────────────────
        [Category("BChat - Border")]
        [DefaultValue(ModernBorderStyle.None)]
        [Description("شكل البوردر: None / Solid / Dashed / Dotted / DashDot / DashDotDot")]
        public ModernBorderStyle BorderStyle
        {
            get => _borderStyle;
            set { _borderStyle = value; Invalidate(); }
        }

        [Category("BChat - Border")]
        [DefaultValue(1.5f)]
        [Description("سُمك البوردر بالبكسل")]
        public float BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = Math.Max(0.5f, value); Invalidate(); }
        }

        [Category("BChat - Border")]
        [Description("لون البوردر")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("BChat - Border")]
        [DefaultValue(false)]
        [Description("ON = لون مختلف للبوردر عند Hover")]
        public bool BorderUseHoverColor
        {
            get => _borderUseHoverColor;
            set { _borderUseHoverColor = value; Invalidate(); }
        }

        [Category("BChat - Border")]
        [Description("لون البوردر عند Hover — يشتغل فقط إذا BorderUseHoverColor = true")]
        public Color BorderHoverColor
        {
            get => _borderHoverColor;
            set { _borderHoverColor = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(160, 44);
            Font = new Font("IBM Plex Sans Arabic", 10f, FontStyle.Bold);
            RightToLeft = RightToLeft.Yes;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Min(_borderRadius, Height / 2);

            using var path = RoundedRect(rect, r);

            // 1. خلفية الـ Parent الحقيقية (يُمكّن الشفافية الكاملة)
            PaintParentBackground(g);

            // 2. خلفية الزر
            Color bg = GetBgColor();
            if (_isPressed) bg = BlendWithAlpha(bg, -20);
            using (var bgBrush = new SolidBrush(bg))
                g.FillPath(bgBrush, path);

            // 3. البوردر
            DrawBorder(g, path);

            // 4. تأثير الضغط (scale)
            if (_isPressed)
            {
                g.ScaleTransform(0.97f, 0.97f);
                g.TranslateTransform(Width * 0.015f, Height * 0.015f);
            }

            // 5. المحتوى (أيقونة + نص)
            DrawContent(g, GetFgColor());
        }

        // ─── Paint Parent Background ──────────────────────────
        private void PaintParentBackground(Graphics g)
        {
            if (Parent == null) return;
            var state = g.Save();
            try
            {
                g.TranslateTransform(-Left, -Top);
                using var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height));
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
            }
            finally { g.Restore(state); }
        }

        // ─── Draw Border ──────────────────────────────────────
        private void DrawBorder(Graphics g, GraphicsPath path)
        {
            bool isGhost = !_useCustomColors && _variant == ButtonVariant.Ghost;

            // Ghost Variant: بوردر افتراضي رفيع دائماً
            if (isGhost && _borderStyle == ModernBorderStyle.None)
            {
                using var defaultPen = new Pen(GhostBorder, 1f);
                g.DrawPath(defaultPen, path);
                return;
            }

            if (_borderStyle == ModernBorderStyle.None) return;

            Color bColor = (_borderUseHoverColor && _isHovered) ? _borderHoverColor : _borderColor;

            using var pen = new Pen(bColor, _borderWidth)
            {
                DashStyle = _borderStyle switch
                {
                    ModernBorderStyle.Solid => DashStyle.Solid,
                    ModernBorderStyle.Dashed => DashStyle.Dash,
                    ModernBorderStyle.Dotted => DashStyle.Dot,
                    ModernBorderStyle.DashDot => DashStyle.DashDot,
                    ModernBorderStyle.DashDotDot => DashStyle.DashDotDot,
                    _ => DashStyle.Solid
                }
            };

            float inset = _borderWidth / 2f;
            var insetRect = new RectangleF(inset, inset, Width - 1 - inset * 2, Height - 1 - inset * 2);
            int rInset = Math.Max(0, Math.Min(_borderRadius, (int)(Height / 2f - inset)));

            using var borderPath = RoundedRectF(insetRect, rInset);
            g.DrawPath(pen, borderPath);
        }

        // ─── Draw Content ─────────────────────────────────────
        private void DrawContent(Graphics g, Color fg)
        {
            // الأولوية: IconChar (FA)  ←  Image مخصصة
            bool hasFaIcon = _iconChar != IconChar.None;
            bool hasImage = !hasFaIcon && _image != null;
            bool hasIcon = hasFaIcon || hasImage;

            string txt = Text ?? "";
            bool hasText = !string.IsNullOrEmpty(txt);

            float iconW = hasIcon ? _iconSize : 0f;
            float gap = (hasIcon && hasText) ? _iconTextGap : 0f;
            var tsz = hasText ? g.MeasureString(txt, Font) : SizeF.Empty;
            float totalW = iconW + gap + tsz.Width;
            float startX = (Width - totalW) / 2f;
            float cy = Height / 2f;

            // Right = أيقونة أولاً (RTL عربي) | Left = نص أولاً
            if (_iconPosition == IconPosition.Right || !hasText)
            {
                if (hasIcon) { DrawIcon(g, fg, hasFaIcon, startX, cy); startX += iconW + gap; }
                if (hasText) DrawText(g, fg, txt, startX, cy, tsz);
            }
            else
            {
                if (hasText) { DrawText(g, fg, txt, startX, cy, tsz); startX += tsz.Width + gap; }
                if (hasIcon) DrawIcon(g, fg, hasFaIcon, startX, cy);
            }
        }

        private void DrawIcon(Graphics g, Color fg, bool useFa, float x, float cy)
        {
            float y = cy - _iconSize / 2f;

            if (useFa)
            {
                try
                {
                    using var bmp = _iconChar.ToBitmap(fg, _iconSize);
                    g.DrawImage(bmp, x, y, _iconSize, _iconSize);
                }
                catch { /* أيقونة غير متاحة — تجاهل */ }
            }
            else if (_image != null)
            {
                g.DrawImage(_image, new RectangleF(x, y, _iconSize, _iconSize));
            }
        }

        private void DrawText(Graphics g, Color fg, string txt, float x, float cy, SizeF tsz)
        {
            using var brush = new SolidBrush(fg);
            g.DrawString(txt, Font, brush, new PointF(x, cy - tsz.Height / 2f));
        }

        // ─── Color Resolution ─────────────────────────────────
        private Color GetBgColor()
        {
            if (_useCustomColors)
                return _isHovered ? _customBgHover : _customBg;

            return _variant switch
            {
                ButtonVariant.Primary => _isHovered ? PrimaryHover : PrimaryBg,
                ButtonVariant.Secondary => _isHovered ? SecondaryHover : SecondaryBg,
                ButtonVariant.Ghost => _isHovered ? GhostHover : GhostBg,
                ButtonVariant.Danger => _isHovered ? DangerHover : DangerBg,
                ButtonVariant.OnPrimary => OnPrimaryBg,
                ButtonVariant.CustomBasem => CustomBasemBg,
                _ => PrimaryBg
            };
        }

        private Color GetFgColor()
        {
            if (_useCustomColors) return _customFg;

            return _variant switch
            {
                ButtonVariant.Primary => PrimaryFg,
                ButtonVariant.Secondary => SecondaryFg,
                ButtonVariant.Ghost => GhostFg,
                ButtonVariant.Danger => DangerFg,
                ButtonVariant.OnPrimary => OnPrimaryFg,
                ButtonVariant.CustomBasem => CustomBasemFg,
                _ => PrimaryFg
            };
        }

        // ─── Alpha-safe Darken ────────────────────────────────
        private static Color BlendWithAlpha(Color c, int offset) =>
            Color.FromArgb(
                c.A,
                Math.Clamp(c.R + offset, 0, 255),
                Math.Clamp(c.G + offset, 0, 255),
                Math.Clamp(c.B + offset, 0, 255));

        // ─── Rounded Rectangle Helpers ────────────────────────
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

        private static GraphicsPath RoundedRectF(RectangleF r, int radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2f;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}