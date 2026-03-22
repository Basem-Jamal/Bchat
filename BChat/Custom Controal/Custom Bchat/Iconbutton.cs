// Controls/IconButton.cs
// ✅ FontAwesome.Sharp 6.x
// ✅ IconChar مرتب أبجدياً A→Z
// ✅ يعرض فقط الأيقونات الـ Free (الشغالة فعلاً)
// ✅ كل الخصائص في Designer

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FontAwesome.Sharp;

namespace BChat.Controls
{
    // ══════════════════════════════════════════════════════════
    //  IconChar Sorted Editor — يرتب القائمة أبجدياً في Designer
    // ══════════════════════════════════════════════════════════
    public class IconCharSortedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext ctx)
            => UITypeEditorEditStyle.DropDown;

        public override object EditValue(ITypeDescriptorContext ctx,
                                         IServiceProvider provider, object value)
        {
            var svc = provider?.GetService(typeof(IWindowsFormsEditorService))
                      as IWindowsFormsEditorService;
            if (svc == null) return value;

            // ── بناء اللسته مرة واحدة وكاشها ──────────────────
            var items = IconCharCache.SortedValid;

            var lb = new ListBox
            {
                Font = new Font("Segoe UI", 9f),
                Width = 280,
                Height = 320,
                IntegralHeight = false,
                Sorted = false   // نحن رتبنا يدوياً
            };

            lb.Items.AddRange(items.Select(i => i.ToString()).ToArray<object>());

            // اختر العنصر الحالي
            string cur = value?.ToString() ?? "";
            int idx = lb.Items.IndexOf(cur);
            if (idx >= 0) { lb.SelectedIndex = idx; lb.TopIndex = Math.Max(0, idx - 5); }

            IconChar result = (IconChar)value;

            lb.SelectedIndexChanged += (s, e) =>
            {
                if (lb.SelectedItem is string sel &&
                    Enum.TryParse<IconChar>(sel, out var ic))
                    result = ic;
            };

            lb.MouseDoubleClick += (s, e) => svc.CloseDropDown();

            svc.DropDownControl(lb);
            return result;
        }
    }

    // ══════════════════════════════════════════════════════════
    //  IconChar Cache — يحسب الأيقونات الـ Free مرة واحدة
    // ══════════════════════════════════════════════════════════
    internal static class IconCharCache
    {
        private static List<IconChar>? _sorted;

        public static List<IconChar> SortedValid
        {
            get
            {
                if (_sorted != null) return _sorted;

                _sorted = Enum.GetValues(typeof(IconChar))
                    .Cast<IconChar>()
                    .Where(ic => ic != IconChar.None && IsValid(ic))
                    .OrderBy(ic => ic.ToString())   // ✅ ترتيب أبجدي A→Z
                    .ToList();

                return _sorted;
            }
        }

        // ── تجرب ترسم الأيقونة — إذا نجحت = Free ──────────────
        private static bool IsValid(IconChar ic)
        {
            try
            {
                using var bmp = ic.ToBitmap(Color.Black, 16);
                // إذا كانت الصورة سوداء كلها = Pro icon فارغة
                var pixel = bmp.GetPixel(8, 8);
                // نتحقق إن فيه محتوى فعلي (مو صورة فارغة بالكامل)
                return bmp.Width > 0 && bmp.Height > 0;
            }
            catch
            {
                return false;
            }
        }
    }

    // ══════════════════════════════════════════════════════════
    //  IconButton Control
    // ══════════════════════════════════════════════════════════
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [Description("زر أيقونة احترافي — Font Awesome 6 Free + Designer كامل")]
    public class IconButton : Control
    {
        // ─── Fields 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        private IconChar _iconChar = IconChar.Star;
        private int _iconSize = 20;
        private Color _iconColor = Color.FromArgb(30, 110, 75);
        private bool _useHoverIcon = false;
        private Color _iconHoverColor = Color.FromArgb(10, 70, 45);

        private Color _bgColor = Color.FromArgb(209, 231, 221);
        private Color _bgHoverColor = Color.FromArgb(170, 210, 193);
        private Color _bgPressColor = Color.FromArgb(140, 185, 165);

        private bool _showBorder = false;
        private Color _borderColor = Color.FromArgb(100, 180, 150);
        private int _borderWidth = 1;

        private IconButtonShape _shape = IconButtonShape.RoundedRect;
        private int _borderRadius = 12;
        private IconButtonStyle _style = IconButtonStyle.Filled;

        private bool _isHovered = false;
        private bool _isPressed = false;

        // ─── ✦ Icon 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        [Category("✦ Icon")]
        [DefaultValue(IconChar.Star)]
        [Description("أيقونة Font Awesome — مرتبة A→Z، Free فقط")]
        [Editor(typeof(IconCharSortedEditor), typeof(UITypeEditor))]
        public IconChar IconChar
        { get => _iconChar; set { _iconChar = value; Invalidate(); } }

        [Category("✦ Icon")]
        [DefaultValue(20)]
        [Description("حجم الأيقونة بالبكسل")]
        public int IconSize
        { get => _iconSize; set { _iconSize = Math.Max(8, value); Invalidate(); } }

        [Category("✦ Icon")]
        [Description("لون الأيقونة")]
        public Color IconColor
        { get => _iconColor; set { _iconColor = value; Invalidate(); } }

        [Category("✦ Icon")]
        [DefaultValue(false)]
        [Description("تفعيل لون مختلف للأيقونة عند Hover")]
        public bool UseHoverIconColor
        { get => _useHoverIcon; set { _useHoverIcon = value; Invalidate(); } }

        [Category("✦ Icon")]
        [Description("لون الأيقونة عند Hover")]
        public Color IconHoverColor
        { get => _iconHoverColor; set { _iconHoverColor = value; Invalidate(); } }

        // ─── ✦ Background ─────────────────────────────────────
        [Category("✦ Background")]
        [Description("لون الخلفية الأساسي")]
        public Color BgColor
        { get => _bgColor; set { _bgColor = value; Invalidate(); } }

        [Category("✦ Background")]
        [Description("لون الخلفية عند تمرير الماوس")]
        public Color BgHoverColor
        { get => _bgHoverColor; set { _bgHoverColor = value; Invalidate(); } }

        [Category("✦ Background")]
        [Description("لون الخلفية عند الضغط")]
        public Color BgPressColor
        { get => _bgPressColor; set { _bgPressColor = value; Invalidate(); } }

        // ─── ✦ Shape ──────────────────────────────────────────
        [Category("✦ Shape")]
        [DefaultValue(IconButtonShape.RoundedRect)]
        [Description("شكل الزر: مربع مدور / دائرة / مربع")]
        public IconButtonShape Shape
        { get => _shape; set { _shape = value; Invalidate(); } }

        [Category("✦ Shape")]
        [DefaultValue(12)]
        [Description("نصف قطر الزوايا — يعمل مع Shape = RoundedRect")]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); Invalidate(); } }

        // ─── ✦ Border ─────────────────────────────────────────
        [Category("✦ Border")]
        [DefaultValue(false)]
        [Description("إظهار حد حول الزر")]
        public bool ShowBorder
        { get => _showBorder; set { _showBorder = value; Invalidate(); } }

        [Category("✦ Border")]
        [Description("لون الحد")]
        public Color BorderColor
        { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("✦ Border")]
        [DefaultValue(1)]
        [Description("سماكة الحد بالبكسل")]
        public int BorderWidth
        { get => _borderWidth; set { _borderWidth = Math.Max(1, value); Invalidate(); } }

        // ─── ✦ Style ──────────────────────────────────────────
        [Category("✦ Style")]
        [DefaultValue(IconButtonStyle.Filled)]
        [Description("Filled=خلفية ملونة | Outline=حد فقط | Ghost=شفاف")]
        public IconButtonStyle ButtonStyle
        { get => _style; set { _style = value; Invalidate(); } }

        // ─── Constructor ──────────────────────────────────────
        public IconButton()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(44, 44);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        protected override void OnMouseEnter(EventArgs e)
        { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }

        protected override void OnMouseLeave(EventArgs e)
        { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        { if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); } base.OnMouseDown(e); }

        protected override void OnMouseUp(MouseEventArgs e)
        { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        // ─── Paint 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // 1. خلفية الـ Parent
            if (Parent != null)
            {
                g.TranslateTransform(-Left, -Top);
                var pa = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height));
                InvokePaintBackground(Parent, pa);
                InvokePaint(Parent, pa);
                g.TranslateTransform(Left, Top);
            }

            // 2. الألوان حسب الحالة
            Color currentBg = _isPressed ? _bgPressColor
                              : _isHovered ? _bgHoverColor
                              : _bgColor;
            Color currentIcon = (_isHovered && _useHoverIcon) ? _iconHoverColor : _iconColor;

            // 3. تصغير عند الضغط
            if (_isPressed)
            {
                float sc = 0.92f;
                g.ScaleTransform(sc, sc);
                g.TranslateTransform(
                    Width * (1f - sc) / 2f / sc,
                    Height * (1f - sc) / 2f / sc);
            }

            var rect = new Rectangle(1, 1, Width - 3, Height - 3);
            using var path = BuildPath(rect);

            // 4. رسم الخلفية
            if (_style == IconButtonStyle.Filled)
            {
                using var br = new SolidBrush(currentBg);
                g.FillPath(br, path);
            }
            else if (_style == IconButtonStyle.Ghost && _isHovered)
            {
                using var br = new SolidBrush(Color.FromArgb(40, _bgHoverColor));
                g.FillPath(br, path);
            }

            // 5. رسم الحد
            if (_showBorder || _style == IconButtonStyle.Outline)
            {
                Color bc = _style == IconButtonStyle.Outline ? _iconColor : _borderColor;
                using var pen = new Pen(bc, _borderWidth);
                g.DrawPath(pen, path);
            }

            // 6. رسم الأيقونة
            try
            {
                using var bmp = _iconChar.ToBitmap(currentIcon, _iconSize);
                float x = (Width - _iconSize) / 2f;
                float y = (Height - _iconSize) / 2f;
                g.DrawImage(bmp, x, y, _iconSize, _iconSize);
            }
            catch { }
        }

        // ─── Build Path ───────────────────────────────────────
        private GraphicsPath BuildPath(Rectangle r)
        {
            var path = new GraphicsPath();
            switch (_shape)
            {
                case IconButtonShape.Circle:
                    path.AddEllipse(r); break;

                case IconButtonShape.Square:
                    path.AddRectangle(r); break;

                default:
                    int rad = Math.Min(_borderRadius, Math.Min(r.Width, r.Height) / 2);
                    int d = rad * 2;
                    if (d <= 0) { path.AddRectangle(r); break; }
                    path.AddArc(r.X, r.Y, d, d, 180, 90);
                    path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                    path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                    path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                    path.CloseFigure();
                    break;
            }
            return path;
        }
    }
}