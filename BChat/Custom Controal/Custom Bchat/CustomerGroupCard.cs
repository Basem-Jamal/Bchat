using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal
{
    [ToolboxItem(true)]
    [Description("بطاقة مجموعة العملاء")]
    public class CustomerGroupCard : Control
    {
        // ══════════════════════════════════════════════════════
        //  Enums & State
        // ══════════════════════════════════════════════════════
        private enum HoverZone { None, Card, DeleteBtn, EditBtn, ViewBtn }
        private HoverZone _hoverZone = HoverZone.None;

        private Rectangle _deleteBtnRect;
        private Rectangle _editBtnRect;
        private Rectangle _viewBtnRect;

        // ══════════════════════════════════════════════════════
        //  Fields
        // ══════════════════════════════════════════════════════
        private Color _cardBackColor        = Color.White;
        private Color _cardHoverBackColor   = Color.FromArgb(250, 249, 255);
        private Color _cardBorderColor      = Color.FromArgb(226, 232, 240);
        private Color _cardHoverBorderColor = Color.FromArgb(124, 111, 247);
        private int   _cardBorderRadius     = 16;
        private int   _cardBorderWidth      = 1;
        private int   _cardPadding          = 16;

        private bool  _showShadow  = true;
        private Color _shadowColor = Color.FromArgb(20, 100, 100, 150);
        private int   _shadowDepth = 4;
        private int   _shadowBlur  = 8;

        private bool   _isActive               = true;
        private string _activeText             = "نشط";
        private string _inactiveText           = "غير نشط";
        private Color  _activeBadgeBackColor   = Color.FromArgb(220, 252, 231);
        private Color  _activeBadgeTextColor   = Color.FromArgb(22, 163, 74);
        private Color  _inactiveBadgeBackColor = Color.FromArgb(243, 244, 246);
        private Color  _inactiveBadgeTextColor = Color.FromArgb(107, 114, 128);
        private int    _badgeBorderRadius      = 10;

        private Image _iconImage           = null;
        private Color _iconBoxBackColor    = Color.FromArgb(255, 237, 213);
        private int   _iconBoxSize         = 44;
        private int   _iconBoxBorderRadius = 12;

        private string _title      = "اسم المجموعة";
        private Font   _titleFont  = new Font("IBM Plex Sans Arabic", 14f, FontStyle.Bold);
        private Color  _titleColor = Color.FromArgb(15, 23, 42);

        private string _subtitle      = "وصف مختصر للمجموعة";
        private Font   _subtitleFont  = new Font("IBM Plex Sans Arabic", 8.5f, FontStyle.Regular);
        private Color  _subtitleColor = Color.FromArgb(100, 116, 139);

        private string _statOneValue           = "0";
        private string _statOneLabel           = "عضو";
        private string _statTwoValue           = "-";
        private string _statTwoLabel           = "آخر نشاط";
        private Color  _statPillBackColor      = Color.FromArgb(241, 245, 249);
        private Color  _statPillHoverBackColor = Color.FromArgb(237, 233, 254);
        private int    _statPillBorderRadius   = 10;
        private Font   _statValueFont          = new Font("IBM Plex Sans Arabic", 11f, FontStyle.Bold);
        private Font   _statLabelFont          = new Font("IBM Plex Sans Arabic", 7.5f, FontStyle.Regular);
        private Color  _statValueColor         = Color.FromArgb(15, 23, 42);
        private Color  _statLabelColor         = Color.FromArgb(100, 116, 139);

        private Color _deleteBtnBackColor      = Color.FromArgb(254, 226, 226);
        private Color _deleteBtnHoverBackColor = Color.FromArgb(254, 202, 202);
        private Color _deleteBtnIconColor      = Color.FromArgb(220, 38, 38);
        private int   _deleteBtnBorderRadius   = 10;
        private int   _deleteBtnSize           = 36;

        private Color _editBtnBackColor      = Color.FromArgb(241, 245, 249);
        private Color _editBtnHoverBackColor = Color.FromArgb(226, 232, 240);
        private Color _editBtnIconColor      = Color.FromArgb(71, 85, 105);
        private int   _editBtnBorderRadius   = 10;
        private int   _editBtnSize           = 36;

        private string _viewBtnText           = "عرض المجموعة";
        private Color  _viewBtnBackColor      = Color.FromArgb(124, 111, 247);
        private Color  _viewBtnHoverBackColor = Color.FromArgb(99, 86, 224);
        private Color  _viewBtnTextColor      = Color.White;
        private int    _viewBtnBorderRadius   = 10;
        private int    _viewBtnHeight         = 36;
        private Font   _viewBtnFont           = new Font("IBM Plex Sans Arabic", 9f, FontStyle.Bold);

        private int _groupId = 0;

        // ══════════════════════════════════════════════════════
        //  Events
        // ══════════════════════════════════════════════════════
        [Category("BChat - Events")]
        public event EventHandler<int> DeleteClicked;
        [Category("BChat - Events")]
        public event EventHandler<int> EditClicked;
        [Category("BChat - Events")]
        public event EventHandler<int> ViewClicked;

        // ══════════════════════════════════════════════════════
        //  Constructor
        // ══════════════════════════════════════════════════════
        public CustomerGroupCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.UserPaint             |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw,
                true);
            UpdateStyles();
            Size        = new Size(280, 215);
            BackColor   = Color.Transparent;
            Cursor      = Cursors.Hand;
            RightToLeft = RightToLeft.Yes;
        }

        // ══════════════════════════════════════════════════════
        //  ✅ FIX — فلكيرينج الـ Designer
        //
        //  WS_EX_TRANSPARENT يجعل Windows يعيد رسم الكنترول
        //  كلما رُسم أي عنصر جانبي → فلكيرينج مستمر في Designer.
        //
        //  الحل: نفعّله فقط في Runtime (التشغيل الفعلي) وليس في Designer.
        //  LicenseManager.UsageMode هو الطريقة الموثوقة للتمييز.
        // ══════════════════════════════════════════════════════
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        //            cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT — runtime فقط
        //        return cp;
        //    }
        //}

        // ══════════════════════════════════════════════════════
        //  ✅ OnPaintBackground
        //
        //  Runtime:   WS_EX_TRANSPARENT يضمن أن الـ Parent (GradientPanel)
        //             رسم خلفيته قبلنا → ما نحتاج نرسم شيئاً هنا.
        //
        //  Designer:  نرسم لون الـ Parent كبديل بسيط (لا gradient
        //             لكن كافٍ لعرض الكارد بشكل صحيح).
        // ══════════════════════════════════════════════════════
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;

            if (Parent != null)
            {
                var state = g.Save();

                // نحرك الرسم لمكان الكنترول
                g.TranslateTransform(-Left, -Top);

                // نرسم الأب (حتى لو فيه Gradient)
                var pe = new PaintEventArgs(g, Parent.ClientRectangle);
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);

                g.Restore(state);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        // ══════════════════════════════════════════════════════
        //  Properties — Card
        // ══════════════════════════════════════════════════════
        [Category("BChat - Card")] [Description("معرّف المجموعة")] [DefaultValue(0)]
        public int GroupId { get => _groupId; set => _groupId = value; }

        [Category("BChat - Card")] [Description("لون خلفية الكارد")]
        public Color CardBackColor { get => _cardBackColor; set { _cardBackColor = value; Invalidate(); } }

        [Category("BChat - Card")] [Description("لون خلفية الكارد عند الهوفر")]
        public Color CardHoverBackColor { get => _cardHoverBackColor; set { _cardHoverBackColor = value; Invalidate(); } }

        [Category("BChat - Card")] [Description("لون البوردر")]
        public Color CardBorderColor { get => _cardBorderColor; set { _cardBorderColor = value; Invalidate(); } }

        [Category("BChat - Card")] [Description("لون البوردر عند الهوفر")]
        public Color CardHoverBorderColor { get => _cardHoverBorderColor; set { _cardHoverBorderColor = value; Invalidate(); } }

        [Category("BChat - Card")] [Description("نصف قطر الزوايا")] [DefaultValue(16)]
        public int CardBorderRadius { get => _cardBorderRadius; set { _cardBorderRadius = value; Invalidate(); } }

        [Category("BChat - Card")] [Description("سماكة البوردر")] [DefaultValue(1)]
        public int CardBorderWidth { get => _cardBorderWidth; set { _cardBorderWidth = value; Invalidate(); } }

        [Category("BChat - Shadow")] [Description("إظهار الظل")] [DefaultValue(true)]
        public bool ShowShadow { get => _showShadow; set { _showShadow = value; Invalidate(); } }

        [Category("BChat - Shadow")] [Description("لون الظل")]
        public Color ShadowColor { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }

        [Category("BChat - Shadow")] [Description("عمق الظل")] [DefaultValue(4)]
        public int ShadowDepth { get => _shadowDepth; set { _shadowDepth = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("حالة المجموعة")] [DefaultValue(true)]
        public bool IsActive { get => _isActive; set { _isActive = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("نص حالة نشط")] [DefaultValue("نشط")]
        public string ActiveText { get => _activeText; set { _activeText = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("نص حالة غير نشط")] [DefaultValue("غير نشط")]
        public string InactiveText { get => _inactiveText; set { _inactiveText = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("لون خلفية بادج نشط")]
        public Color ActiveBadgeBackColor { get => _activeBadgeBackColor; set { _activeBadgeBackColor = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("لون نص بادج نشط")]
        public Color ActiveBadgeTextColor { get => _activeBadgeTextColor; set { _activeBadgeTextColor = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("لون خلفية بادج غير نشط")]
        public Color InactiveBadgeBackColor { get => _inactiveBadgeBackColor; set { _inactiveBadgeBackColor = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("لون نص بادج غير نشط")]
        public Color InactiveBadgeTextColor { get => _inactiveBadgeTextColor; set { _inactiveBadgeTextColor = value; Invalidate(); } }

        [Category("BChat - Badge")] [Description("نصف قطر زوايا البادج")] [DefaultValue(20)]
        public int BadgeBorderRadius { get => _badgeBorderRadius; set { _badgeBorderRadius = value; Invalidate(); } }

        [Category("BChat - Icon")] [Description("صورة الأيقونة")]
        public Image IconImage { get => _iconImage; set { _iconImage = value; Invalidate(); } }

        [Category("BChat - Icon")] [Description("لون خلفية صندوق الأيقونة")]
        public Color IconBoxBackColor { get => _iconBoxBackColor; set { _iconBoxBackColor = value; Invalidate(); } }

        [Category("BChat - Icon")] [Description("حجم صندوق الأيقونة")] [DefaultValue(44)]
        public int IconBoxSize { get => _iconBoxSize; set { _iconBoxSize = value; Invalidate(); } }

        [Category("BChat - Icon")] [Description("نصف قطر زوايا صندوق الأيقونة")] [DefaultValue(12)]
        public int IconBoxBorderRadius { get => _iconBoxBorderRadius; set { _iconBoxBorderRadius = value; Invalidate(); } }

        [Category("BChat - Title")] [Description("عنوان المجموعة")]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        [Category("BChat - Title")] [Description("خط العنوان")]
        public Font TitleFont { get => _titleFont; set { _titleFont = value; Invalidate(); } }

        [Category("BChat - Title")] [Description("لون العنوان")]
        public Color TitleColor { get => _titleColor; set { _titleColor = value; Invalidate(); } }

        [Category("BChat - Subtitle")] [Description("الوصف")]
        public string Subtitle { get => _subtitle; set { _subtitle = value; Invalidate(); } }

        [Category("BChat - Subtitle")] [Description("خط الوصف")]
        public Font SubtitleFont { get => _subtitleFont; set { _subtitleFont = value; Invalidate(); } }

        [Category("BChat - Subtitle")] [Description("لون الوصف")]
        public Color SubtitleColor { get => _subtitleColor; set { _subtitleColor = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("قيمة الإحصائية الأولى")]
        public string StatOneValue { get => _statOneValue; set { _statOneValue = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("تسمية الإحصائية الأولى")]
        public string StatOneLabel { get => _statOneLabel; set { _statOneLabel = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("قيمة الإحصائية الثانية")]
        public string StatTwoValue { get => _statTwoValue; set { _statTwoValue = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("تسمية الإحصائية الثانية")]
        public string StatTwoLabel { get => _statTwoLabel; set { _statTwoLabel = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("لون خلفية الـ Pill")]
        public Color StatPillBackColor { get => _statPillBackColor; set { _statPillBackColor = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("لون خلفية الـ Pill عند الهوفر")]
        public Color StatPillHoverBackColor { get => _statPillHoverBackColor; set { _statPillHoverBackColor = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("نصف قطر زوايا الـ Pill")] [DefaultValue(10)]
        public int StatPillBorderRadius { get => _statPillBorderRadius; set { _statPillBorderRadius = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("خط قيمة الإحصائية")]
        public Font StatValueFont { get => _statValueFont; set { _statValueFont = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("خط تسمية الإحصائية")]
        public Font StatLabelFont { get => _statLabelFont; set { _statLabelFont = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("لون قيمة الإحصائية")]
        public Color StatValueColor { get => _statValueColor; set { _statValueColor = value; Invalidate(); } }

        [Category("BChat - Stats")] [Description("لون تسمية الإحصائية")]
        public Color StatLabelColor { get => _statLabelColor; set { _statLabelColor = value; Invalidate(); } }

        [Category("BChat - Delete Btn")] [Description("لون خلفية زر الحذف")]
        public Color DeleteBtnBackColor { get => _deleteBtnBackColor; set { _deleteBtnBackColor = value; Invalidate(); } }

        [Category("BChat - Delete Btn")] [Description("لون خلفية زر الحذف عند الهوفر")]
        public Color DeleteBtnHoverBackColor { get => _deleteBtnHoverBackColor; set { _deleteBtnHoverBackColor = value; Invalidate(); } }

        [Category("BChat - Delete Btn")] [Description("لون أيقونة الحذف")]
        public Color DeleteBtnIconColor { get => _deleteBtnIconColor; set { _deleteBtnIconColor = value; Invalidate(); } }

        [Category("BChat - Delete Btn")] [Description("نصف قطر زوايا زر الحذف")] [DefaultValue(10)]
        public int DeleteBtnBorderRadius { get => _deleteBtnBorderRadius; set { _deleteBtnBorderRadius = value; Invalidate(); } }

        [Category("BChat - Delete Btn")] [Description("حجم زر الحذف")] [DefaultValue(36)]
        public int DeleteBtnSize { get => _deleteBtnSize; set { _deleteBtnSize = value; Invalidate(); } }

        [Category("BChat - Edit Btn")] [Description("لون خلفية زر التعديل")]
        public Color EditBtnBackColor { get => _editBtnBackColor; set { _editBtnBackColor = value; Invalidate(); } }

        [Category("BChat - Edit Btn")] [Description("لون خلفية زر التعديل عند الهوفر")]
        public Color EditBtnHoverBackColor { get => _editBtnHoverBackColor; set { _editBtnHoverBackColor = value; Invalidate(); } }

        [Category("BChat - Edit Btn")] [Description("لون أيقونة التعديل")]
        public Color EditBtnIconColor { get => _editBtnIconColor; set { _editBtnIconColor = value; Invalidate(); } }

        [Category("BChat - Edit Btn")] [Description("نصف قطر زوايا زر التعديل")] [DefaultValue(10)]
        public int EditBtnBorderRadius { get => _editBtnBorderRadius; set { _editBtnBorderRadius = value; Invalidate(); } }

        [Category("BChat - Edit Btn")] [Description("حجم زر التعديل")] [DefaultValue(36)]
        public int EditBtnSize { get => _editBtnSize; set { _editBtnSize = value; Invalidate(); } }

        [Category("BChat - View Btn")] [Description("نص زر عرض المجموعة")] [DefaultValue("عرض المجموعة")]
        public string ViewBtnText { get => _viewBtnText; set { _viewBtnText = value; Invalidate(); } }

        [Category("BChat - Message Btn")] [Description("لون خلفية زر عرض المجموعة")]
        public Color ViewBtnBackColor { get => _viewBtnBackColor; set { _viewBtnBackColor = value; Invalidate(); } }

        [Category("BChat - View Btn")] [Description("لون خلفية زر عرض المجموعة عند الهوفر")]
        public Color ViewBtnHoverBackColor { get => _viewBtnHoverBackColor; set { _viewBtnHoverBackColor = value; Invalidate(); } }

        [Category("BChat - View Btn")] [Description("لون نص زر عرض المجموعة")]
        public Color ViewBtnTextColor { get => _viewBtnTextColor; set { _viewBtnTextColor = value; Invalidate(); } }
        [Category("BChat - View Btn")] [Description("نصف قطر زوايا زر عرض المجموعة")] [DefaultValue(10)]
        public int ViewBtnBorderRadius { get => _viewBtnBorderRadius; set { _viewBtnBorderRadius = value; Invalidate(); } }

        [Category("BChat - View Btn")] [Description("ارتفاع زر عرض المجموعة")] [DefaultValue(36)]
        public int ViewBtnHeight { get => _viewBtnHeight; set { _viewBtnHeight = value; Invalidate(); } }

        [Category("BChat - View Btn")] [Description("خط زر عرض المجموعة")]
        public Font ViewBtnFont { get => _viewBtnFont; set { _viewBtnFont = value; Invalidate(); } }
        // ══════════════════════════════════════════════════════
        //  OnPaint
        // ══════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int  p          = _cardPadding;
            bool isHovered  = _hoverZone != HoverZone.None;
            bool inDesigner = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            // في Designer: غطِّ الخلفية كاملاً (لأن WS_EX_TRANSPARENT غير مفعّل)
            if (inDesigner)
            {
                using (var b = new SolidBrush(GetSolidAncestorColor()))
                    g.FillRectangle(b, ClientRectangle);
            }

            // Shadow — فقط في Runtime
            if (_showShadow && !inDesigner) DrawShadow(g);

            // Card area
            float shadowOffset = (_showShadow && !inDesigner) ? _shadowDepth : 0f;
            var cardRect = new RectangleF(0, 0, Width - shadowOffset - 1f, Height - shadowOffset - 1f);

            Color bg     = isHovered ? _cardHoverBackColor   : _cardBackColor;
            Color border = isHovered ? _cardHoverBorderColor : _cardBorderColor;

            using (var path = GetRoundedRect(cardRect, _cardBorderRadius))
            {
                using (var brush = new SolidBrush(bg))     g.FillPath(brush, path);
                using (var pen   = new Pen(border, _cardBorderWidth)) g.DrawPath(pen, path);
            }

            // ── ROW 1 ─────────────────────────────────────────
            int row1Y = p;

            // Icon Box
            var iconBoxRect = new RectangleF(p, row1Y, _iconBoxSize, _iconBoxSize);
            using (var path  = GetRoundedRect(iconBoxRect, _iconBoxBorderRadius))
            using (var brush = new SolidBrush(_iconBoxBackColor))
                g.FillPath(brush, path);

            if (_iconImage != null)
            {
                int ip = 8;
                g.DrawImage(_iconImage, new Rectangle((int)iconBoxRect.X + ip, (int)iconBoxRect.Y + ip,
                    _iconBoxSize - ip * 2, _iconBoxSize - ip * 2));
            }
            else DrawDefaultIcon(g, iconBoxRect);

            // Badge
            DrawStatusBadge(g, row1Y, p, (int)shadowOffset);

            // ── ROW 2: Title ──────────────────────────────────
            float row2Y = row1Y + _iconBoxSize + 12f;
            using (var brush = new SolidBrush(_titleColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center,
                                            Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(_title, _titleFont, brush, new RectangleF(p, row2Y, cardRect.Width - p * 2, 28f), sf);
            }

            // ── ROW 3: Subtitle ───────────────────────────────
            float row3Y = row2Y + 30f;
            using (var brush = new SolidBrush(_subtitleColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near,
                                            Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(_subtitle, _subtitleFont, brush, new RectangleF(p, row3Y, cardRect.Width - p * 2, 34f), sf);
            }

            // ── ROW 4: Stat Pills ──────────────────────────────
            float row4Y  = row3Y + 40f;
            int   pillGap = 8;
            int   pillW  = (int)((cardRect.Width - p * 2 - pillGap) / 2);
            int   pillH  = 48;
            Color pillBg = isHovered ? _statPillHoverBackColor : _statPillBackColor;

            DrawStatPill(g, new RectangleF(cardRect.Width - p - pillW, row4Y, pillW, pillH), _statOneValue, _statOneLabel, pillBg);
            DrawStatPill(g, new RectangleF(p, row4Y, pillW, pillH), _statTwoValue, _statTwoLabel, pillBg);

            // ── ROW 5: Buttons ────────────────────────────────
            int row5Y  = (int)(row4Y + pillH + 12);
            int btnGap = 8;

            _deleteBtnRect = new Rectangle(p, row5Y, _deleteBtnSize, _deleteBtnSize);
            DrawIconButton(g, _deleteBtnRect,
                _hoverZone == HoverZone.DeleteBtn ? _deleteBtnHoverBackColor : _deleteBtnBackColor,
                _deleteBtnBorderRadius, IconType.Trash, _deleteBtnIconColor);

            _editBtnRect = new Rectangle(p + _deleteBtnSize + btnGap, row5Y, _editBtnSize, _editBtnSize);
            DrawIconButton(g, _editBtnRect,
                _hoverZone == HoverZone.EditBtn ? _editBtnHoverBackColor : _editBtnBackColor,
                _editBtnBorderRadius, IconType.Pencil, _editBtnIconColor);

            int msgX = p + _deleteBtnSize + btnGap + _editBtnSize + btnGap;
            _viewBtnRect = new Rectangle(msgX, row5Y, (int)(cardRect.Width - p - msgX), _viewBtnHeight);
            DrawMessageButton(g, _viewBtnRect,
                _hoverZone == HoverZone.ViewBtn ? _viewBtnHoverBackColor : _viewBtnBackColor,
                _viewBtnBorderRadius);
        }

        // ══════════════════════════════════════════════════════
        //  Draw Helpers
        // ══════════════════════════════════════════════════════
        private void DrawShadow(Graphics g)
        {
            var shadowRect = new RectangleF(
                _shadowDepth,
                _shadowDepth,
                Width - _shadowDepth,
                Height - _shadowDepth);

            using var path = GetRoundedRect(shadowRect, _cardBorderRadius);
            using var brush = new PathGradientBrush(path)
            {
                CenterColor = Color.FromArgb(40, _shadowColor),
                SurroundColors = new[] { Color.Transparent }
            };

            g.FillPath(brush, path);
        }

        private void DrawStatusBadge(Graphics g, int row1Y, int p, int shadowOffset)
        {
            string text   = _isActive ? _activeText : _inactiveText;
            Color  bgClr  = _isActive ? _activeBadgeBackColor   : _inactiveBadgeBackColor;
            Color  fgClr  = _isActive ? _activeBadgeTextColor   : _inactiveBadgeTextColor;

            using (var badgeFont = new Font("IBM Plex Sans Arabic", 7.5f, FontStyle.Regular))
            {
                float dotW   = 7f;
                float badgeW = g.MeasureString(text, badgeFont).Width + dotW + 20f;
                float badgeH = 20f;
                float badgeX = Width - p - shadowOffset - badgeW;
                float badgeY = row1Y + (_iconBoxSize - badgeH) / 2f;
                var   rect   = new RectangleF(badgeX, badgeY, badgeW, badgeH);

                using (var path  = GetRoundedRect(rect, _badgeBorderRadius))
                using (var brush = new SolidBrush(bgClr)) g.FillPath(brush, path);

                using (var brush = new SolidBrush(fgClr))
                    g.FillEllipse(brush, badgeX + 8f, badgeY + (badgeH - dotW) / 2f, dotW, dotW);

                using (var brush = new SolidBrush(fgClr))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, badgeFont, brush, new RectangleF(badgeX + dotW + 12f, badgeY, badgeW - dotW - 14f, badgeH), sf);
                }
            }
        }

        private void DrawStatPill(Graphics g, RectangleF rect, string value, string label, Color backColor)
        {
            using (var path  = GetRoundedRect(rect, _statPillBorderRadius))
            using (var brush = new SolidBrush(backColor)) g.FillPath(brush, path);

            float valH = rect.Height * 0.54f;
            using (var brush = new SolidBrush(_statValueColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(value, _statValueFont, brush, new RectangleF(rect.X, rect.Y + 5f, rect.Width, valH), sf);
            }
            using (var brush = new SolidBrush(_statLabelColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(label, _statLabelFont, brush, new RectangleF(rect.X, rect.Y + valH, rect.Width, rect.Height - valH - 4f), sf);
            }
        }

        private enum IconType { Trash, Pencil }

        private void DrawIconButton(Graphics g, Rectangle rect, Color backColor, int radius, IconType iconType, Color iconColor)
        {
            using (var path  = GetRoundedRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), radius))
            using (var brush = new SolidBrush(backColor)) g.FillPath(brush, path);

            int cx = rect.X + rect.Width / 2, cy = rect.Y + rect.Height / 2;
            using (var pen = new Pen(iconColor, 1.6f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (iconType == IconType.Trash)
                {
                    g.DrawLine(pen, cx - 6, cy - 4, cx + 6, cy - 4);
                    g.DrawLine(pen, cx - 2, cy - 4, cx - 2, cy - 7);
                    g.DrawLine(pen, cx + 2, cy - 4, cx + 2, cy - 7);
                    g.DrawLine(pen, cx - 2, cy - 7, cx + 2, cy - 7);
                    g.DrawRectangle(pen, cx - 5, cy - 3, 10, 10);
                    g.DrawLine(pen, cx - 2, cy - 1, cx - 2, cy + 5);
                    g.DrawLine(pen, cx + 2, cy - 1, cx + 2, cy + 5);
                }
                else
                {
                    g.DrawLine(pen, new PointF(cx - 4, cy + 5), new PointF(cx + 5, cy - 4));
                    g.DrawLine(pen, new PointF(cx + 5, cy - 4), new PointF(cx + 7, cy - 2));
                    g.DrawLine(pen, new PointF(cx + 7, cy - 2), new PointF(cx - 2, cy + 7));
                    g.DrawLine(pen, new PointF(cx - 2, cy + 7), new PointF(cx - 4, cy + 5));
                    g.DrawLine(pen, new PointF(cx + 4, cy - 5), new PointF(cx + 6, cy - 3));
                }
            }
        }

        private void DrawMessageButton(Graphics g, Rectangle rect, Color backColor, int radius)
        {
            using (var path  = GetRoundedRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), radius))
            using (var brush = new SolidBrush(backColor)) g.FillPath(brush, path);

            int ax = rect.X + 14, ay = rect.Y + rect.Height / 2;
            using (var pen = new Pen(_viewBtnTextColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawLine(pen, ax + 7, ay, ax,     ay);
                g.DrawLine(pen, ax,     ay, ax + 4, ay - 4);
                g.DrawLine(pen, ax,     ay, ax + 4, ay + 4);
            }
            using (var brush = new SolidBrush(_viewBtnTextColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(_viewBtnText, _viewBtnFont, brush, new RectangleF(rect.X + 22, rect.Y, rect.Width - 22, rect.Height), sf);
            }
        }

        private void DrawDefaultIcon(Graphics g, RectangleF rect)
        {
            float cx = rect.X + rect.Width / 2f, cy = rect.Y + rect.Height / 2f, r = Math.Min(rect.Width, rect.Height) * 0.25f;
            using (var pen = new Pen(Color.FromArgb(80, 120, 120, 120), 1.5f))
            {
                g.DrawEllipse(pen, cx - r, cy - r, r * 2, r * 2);
                g.DrawLine(pen, cx, cy - r - 4, cx, cy + r + 4);
                g.DrawLine(pen, cx - r - 4, cy, cx + r + 4, cy);
            }
        }

        // ══════════════════════════════════════════════════════
        //  Mouse
        // ══════════════════════════════════════════════════════
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var zone = HoverZone.Card;

            if (_deleteBtnRect.Contains(e.Location)) zone = HoverZone.DeleteBtn;
            else if (_editBtnRect.Contains(e.Location)) zone = HoverZone.EditBtn;
            else if (_viewBtnRect.Contains(e.Location)) zone = HoverZone.ViewBtn;

            if (zone == _hoverZone) return; // 🔥 أهم سطر

            _hoverZone = zone;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)   { base.OnMouseLeave(e); if (_hoverZone != HoverZone.None) { _hoverZone = HoverZone.None; Invalidate(); } }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) Invalidate(); }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left) return;
            if      (_deleteBtnRect.Contains(e.Location))  DeleteClicked?.Invoke(this, _groupId);
            else if (_editBtnRect.Contains(e.Location))    EditClicked?.Invoke(this, _groupId);
            else if (_viewBtnRect.Contains(e.Location)) ViewClicked?.Invoke(this, _groupId);
        }

        // ══════════════════════════════════════════════════════
        //  Helpers
        // ══════════════════════════════════════════════════════
        private Color GetSolidAncestorColor()
        {
            var p = Parent;
            while (p != null)
            {
                if (p.BackColor.A > 0 && p.BackColor != Color.Transparent) return p.BackColor;
                p = p.Parent;
            }
            return SystemColors.Control;
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, float radius)
        {
            float r = Math.Max(0.1f, radius * 2f);
            var path = new GraphicsPath();
            path.AddArc(rect.X,         rect.Y,          r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y,          r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r,   0, 90);
            path.AddArc(rect.X,         rect.Bottom - r, r, r,  90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose(); _subtitleFont?.Dispose();
                _statValueFont?.Dispose(); _statLabelFont?.Dispose();
                _viewBtnFont?.Dispose(); _iconImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
