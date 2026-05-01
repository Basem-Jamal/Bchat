// ============================================================
//  BChat — ChatSidebar Custom Control  (v4 — Virtual Scrolling)
//  Namespace : BChat.Custom_Controal.Custom_Bchat.Message_Controls
//  Target    : .NET 8 / Windows Forms
//
//  التحديثات في v4:
//  ① استبدال FlowLayoutPanel بـ VirtualChatList (Custom Control واحد)
//  ② رسم العناصر المرئية فقط + buffer (10 فوق و 10 تحت)
//  ③ يتعامل مع 100,000+ محادثة بدون أي مشاكل في الـ window handles
//  ④ Mouse wheel يشتغل بدون سرقة الـ focus من البحث (IMessageFilter)
//
//  التحديثات السابقة في v3 (محتفظ بها):
//  ① إصلاح مشكلة الـ Panel (مستطيل خلف الأيقونة) → g.Clear(parentBg)
//  ② خصائص Designer كاملة للزر
//  ③ Enum ButtonShapeStyle: Circle / RoundedSquare / Square
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Message_Controls
{
    // ─────────────────────────────────────────────────────────
    //  Data Model
    // ─────────────────────────────────────────────────────────
    public class ChatListItemData
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public Image? Avatar { get; set; }
        public bool IsOnline { get; set; }
        public int UnreadCount { get; set; }
        public bool IsGroup { get; set; }
        public bool IsLastMessageSent { get; set; }
        public DateTime LastMessageAt { get; set; } = DateTime.MinValue;
    }

    // ─────────────────────────────────────────────────────────
    //  Enum: شكل الزر
    // ─────────────────────────────────────────────────────────
    public enum ButtonShapeStyle
    {
        Circle,         // دائرة كاملة
        RoundedSquare,  // مستطيل بحواف مدورة (radius = 10)
        Square,         // مستطيل بحواف حادة
    }

    // ─────────────────────────────────────────────────────────
    //  Main Control
    // ─────────────────────────────────────────────────────────
    [ToolboxItem(true)]
    [Category("BChat - Chat")]
    [Description("WhatsApp/Telegram-style chat list sidebar with virtual scrolling.")]
    public class ChatSidebar : UserControl
    {
        // ── Design Tokens ────────────────────────────────────
        internal static readonly Color C_BG = Color.FromArgb(255, 255, 255); // #FFFFFF
        internal static readonly Color C_BORDER = Color.FromArgb(226, 232, 240);
        internal static readonly Color C_SEARCH_BG = Color.FromArgb(248, 247, 255);
        internal static readonly Color C_ACCENT = Color.FromArgb(124, 111, 247); // #7C6FF7
        internal static readonly Color C_ONLINE = Color.FromArgb(16, 185, 129); // #10B981
        internal static readonly Color C_ITEM_SEL = Color.FromArgb(248, 247, 255); // #F8F7FF
        internal static readonly Color C_ITEM_HOV = Color.FromArgb(250, 250, 255); // #FAFAFF
        internal static readonly Color C_TITLE = Color.FromArgb(15, 23, 42);
        internal static readonly Color C_NAME = Color.FromArgb(15, 23, 42); // #0F172A
        internal static readonly Color C_MSG = Color.FromArgb(100, 116, 139); // #64748B
        internal static readonly Color C_TIME = Color.FromArgb(148, 163, 184); // #94A3B8
        internal static readonly Color C_PILL_OFF = Color.FromArgb(241, 245, 249);
        internal static readonly Color C_PILL_TXT_OFF = Color.FromArgb(100, 116, 139);
        internal static readonly Color C_SEP = Color.FromArgb(241, 245, 249); // #F1F5F9

        // ── Layout ───────────────────────────────────────────
        private const int H_HEADER = 64;
        private const int H_SEARCH = 56;
        private const int H_FILTERS = 52;
        private const int BTN_SZ_DEFAULT = 38;

        // ── Designer Properties: New Chat Button ─────────────
        private Image? _headerIcon = null;
        private Color _btnColor = Color.FromArgb(124, 111, 247);
        private int _btnSize = BTN_SZ_DEFAULT;
        private int _btnIconPadding = 9;
        private ButtonShapeStyle _btnShape = ButtonShapeStyle.Circle;
        private string _headerTitleText = "الدردشات";

        [Category("BChat — New Chat Button")]
        [Description("أيقونة مخصصة داخل زر المحادثة الجديدة. اتركها فارغة لرسم أيقونة القلم الافتراضية.")]
        [DefaultValue(null)]
        public Image? HeaderIcon
        {
            get => _headerIcon;
            set { _headerIcon = value; _btnNewChat?.Invalidate(); }
        }

        [Category("BChat — New Chat Button")]
        [Description("لون خلفية زر المحادثة الجديدة.")]
        public Color ButtonColor
        {
            get => _btnColor;
            set { _btnColor = value; _btnNewChat?.Invalidate(); }
        }

        [Category("BChat — New Chat Button")]
        [Description("حجم زر المحادثة الجديدة بالبكسل (مربع، من 24 إلى 64).")]
        [DefaultValue(BTN_SZ_DEFAULT)]
        public int ButtonSize
        {
            get => _btnSize;
            set
            {
                _btnSize = Math.Max(24, Math.Min(value, 64));
                if (_btnNewChat != null)
                {
                    _btnNewChat.Size = new Size(_btnSize, _btnSize);
                    _btnNewChat.Parent?.Invalidate();
                }
            }
        }

        [Category("BChat — New Chat Button")]
        [Description("مسافة الأيقونة عن حواف الزر بالبكسل. كلما زادت كلما صغرت الأيقونة.")]
        [DefaultValue(9)]
        public int ButtonIconPadding
        {
            get => _btnIconPadding;
            set { _btnIconPadding = Math.Max(2, value); _btnNewChat?.Invalidate(); }
        }

        [Category("BChat — New Chat Button")]
        [Description("شكل الزر: دائرة كاملة، أو مستطيل بحواف مدورة، أو مستطيل حاد.")]
        [DefaultValue(ButtonShapeStyle.Circle)]
        public ButtonShapeStyle ButtonShape
        {
            get => _btnShape;
            set { _btnShape = value; _btnNewChat?.Invalidate(); }
        }

        [Category("BChat — New Chat Button")]
        [Description("عنوان القسم في الهيدر.")]
        [DefaultValue("الدردشات")]
        public string HeaderTitle
        {
            get => _headerTitleText;
            set
            {
                _headerTitleText = value ?? "الدردشات";
                if (_lblTitle != null) _lblTitle.Text = _headerTitleText;
            }
        }

        // ── Child Controls ───────────────────────────────────
        private Panel _pnlHeader = null!;
        private Panel _pnlSearch = null!;
        private Panel _pnlFilters = null!;
        private Panel _pnlList = null!;
        private VScrollOnlyFLP _flpFilters = null!;
        private VirtualChatList _listCtrl = null!;
        private PillButton _pillAll = null!;
        private PillButton _pillUnread = null!;
        private PillButton _pillGroups = null!;
        private Button _btnNewChat = null!;
        private Label _lblTitle = null!;

        // ── State ────────────────────────────────────────────
        private List<ChatListItemData> _allChats = new();
        private int _selectedId = -1;
        private string _filter = "all";
        private string _searchQuery = string.Empty;
        private bool _btnHovered = false;

        // ── Fonts ────────────────────────────────────────────
        private Font _fontTitle = null!;
        private Font _fontName = null!;
        private Font _fontMsg = null!;
        private Font _fontTime = null!;
        private Font _fontPill = null!;
        private Font _fontSearch = null!;

        // ── Design mode guard ────────────────────────────────
        private static readonly bool _isAnyDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            System.Diagnostics.Process.GetCurrentProcess().ProcessName
                .IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;

        // ── Events ───────────────────────────────────────────
        public event EventHandler<int>? ChatSelected;
        public event EventHandler? NewChatClicked;
        public event EventHandler<string>? FilterChanged;
        public event EventHandler<string>? SearchChanged;

        // ─────────────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────────────
        public ChatSidebar()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw,
                true);

            RightToLeft = RightToLeft.Yes;
            BackColor = C_BG;
            MinimumSize = new Size(260, 400);
            Size = new Size(320, 700);

            if (_isAnyDesignMode) { Text = "ChatSidebar"; return; }

            BuildFonts();
            BuildLayout();
        }

        // ─────────────────────────────────────────────────────
        //  Public API
        // ─────────────────────────────────────────────────────
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedChatId => _selectedId;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentFilter
        {
            get => _filter;
            set { _filter = value; SyncPills(); RefreshList(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; RefreshList(); }
        }

        public void LoadChats(List<ChatListItemData> chats)
        {
            _allChats = chats ?? new List<ChatListItemData>();
            RefreshList();
        }

        public void SetSelectedChat(int contactId)
        {
            _selectedId = contactId;
            _listCtrl?.SetSelected(contactId);
        }

        public void RefreshItem(int contactId)
        {
            _listCtrl?.InvalidateItem(contactId);
        }

        public void MoveItemToTop(int contactId)
        {
            // إعادة الترتيب في القائمة الكاملة عشان يبقى الترتيب ثابت
            // حتى بعد إعادة التحديث (refresh)
            int idx = _allChats.FindIndex(c => c.ContactId == contactId);
            if (idx > 0)
            {
                var item = _allChats[idx];
                _allChats.RemoveAt(idx);
                _allChats.Insert(0, item);
            }
            _listCtrl?.MoveToTop(contactId);
        }

        // ─────────────────────────────────────────────────────
        //  Designer preview
        // ─────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!DesignMode) return;
            var g = e.Graphics;
            g.Clear(C_BG);
            using var br = new SolidBrush(C_TITLE);
            using var fnt = new Font("Segoe UI", 10f, FontStyle.Bold);
            g.DrawString("ChatSidebar  [BChat]", fnt, br,
                new PointF(ClientSize.Width / 2f - 65, ClientSize.Height / 2f - 10));
        }

        // ─────────────────────────────────────────────────────
        //  Fonts
        // ─────────────────────────────────────────────────────
        private void BuildFonts()
        {
            string face = IsFontInstalled("Cairo") ? "Cairo" : "Segoe UI";
            _fontTitle = new Font(face, 13f, FontStyle.Bold, GraphicsUnit.Point);
            _fontName = new Font(face, 10.5f, FontStyle.Bold, GraphicsUnit.Point);
            _fontMsg = new Font(face, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontTime = new Font(face, 8f, FontStyle.Regular, GraphicsUnit.Point);
            _fontPill = new Font(face, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontSearch = new Font(face, 10f, FontStyle.Regular, GraphicsUnit.Point);
        }

        internal static bool IsFontInstalled(string name)
        {
            using var fc = new InstalledFontCollection();
            return fc.Families.Any(f =>
                f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // ─────────────────────────────────────────────────────
        //  Layout
        // ─────────────────────────────────────────────────────
        private void BuildLayout()
        {
            SuspendLayout();

            // ── Header ───────────────────────────────────────
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = H_HEADER,
                BackColor = C_BG,
            };
            _pnlHeader.Paint += PaintHeaderBorder;

            _lblTitle = new Label
            {
                Text = _headerTitleText,
                Font = _fontTitle,
                ForeColor = C_TITLE,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 12, 0),
                RightToLeft = RightToLeft.Yes,
            };

            // ── زر المحادثة الجديدة ───────────────────────────
            _btnNewChat = new Button
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                TabStop = false,
                Size = new Size(_btnSize, _btnSize),
            };
            _btnNewChat.FlatAppearance.BorderSize = 0;
            _btnNewChat.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _btnNewChat.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _btnNewChat.Paint += PaintNewChatButton;
            _btnNewChat.Click += (s, e) => NewChatClicked?.Invoke(this, EventArgs.Empty);
            _btnNewChat.MouseEnter += (s, e) => { _btnHovered = true; _btnNewChat.Invalidate(); };
            _btnNewChat.MouseLeave += (s, e) => { _btnHovered = false; _btnNewChat.Invalidate(); };

            var pnlBtnWrap = new Panel
            {
                Width = _btnSize + 20,
                Dock = DockStyle.Left,
                BackColor = C_BG,
                Padding = new Padding(10,
                    (H_HEADER - _btnSize) / 2,
                    10,
                    (H_HEADER - _btnSize) / 2),
            };
            _btnNewChat.Dock = DockStyle.Fill;
            pnlBtnWrap.Controls.Add(_btnNewChat);

            _pnlHeader.Controls.Add(_lblTitle);
            _pnlHeader.Controls.Add(pnlBtnWrap);

            // ── Search ───────────────────────────────────────
            _pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = H_SEARCH,
                BackColor = C_BG,
                Padding = new Padding(12, 8, 12, 8),
            };
            var searchBox = new SearchBoxPanel(_fontSearch) { Dock = DockStyle.Fill };
            searchBox.TextChanged += (s, e) =>
            {
                _searchQuery = searchBox.SearchText;
                SearchChanged?.Invoke(this, _searchQuery);
                RefreshList();
            };
            _pnlSearch.Controls.Add(searchBox);

            // ── Filter Pills ─────────────────────────────────
            _pnlFilters = new Panel
            {
                Dock = DockStyle.Top,
                Height = H_FILTERS,
                BackColor = C_BG,
            };
            _pnlFilters.Paint += PaintFiltersBorder;

            _flpFilters = new VScrollOnlyFLP
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                RightToLeft = RightToLeft.Yes,
                BackColor = C_BG,
                Padding = new Padding(12, 8, 12, 8),
            };

            _pillAll = new PillButton("الكل", "all", _fontPill, true);
            _pillUnread = new PillButton("غير مقروءة", "unread", _fontPill, false);
            _pillGroups = new PillButton("المجموعات", "groups", _fontPill, false);

            foreach (var pill in new[] { _pillAll, _pillUnread, _pillGroups })
            {
                pill.Margin = new Padding(0, 0, 8, 0);
                pill.PillClicked += OnPillClicked;
                _flpFilters.Controls.Add(pill);
            }
            _pnlFilters.Controls.Add(_flpFilters);

            // ── Chat List (Virtual) ──────────────────────────
            _pnlList = new Panel { Dock = DockStyle.Fill, BackColor = C_BG };

            _listCtrl = new VirtualChatList(_fontName, _fontMsg, _fontTime)
            {
                Dock = DockStyle.Fill,
                BackColor = C_BG,
            };
            _listCtrl.ChatSelected += OnVirtualChatSelected;

            _pnlList.Controls.Add(_listCtrl);

            // ── Assemble ─────────────────────────────────────
            Controls.Add(_pnlList);
            Controls.Add(_pnlFilters);
            Controls.Add(_pnlSearch);
            Controls.Add(_pnlHeader);

            ResumeLayout(true);
        }

        // ─────────────────────────────────────────────────────
        //  Paint Helpers
        // ─────────────────────────────────────────────────────
        private static void PaintHeaderBorder(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            using var pen = new Pen(C_BORDER, 1f);
            e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
        }

        private static void PaintFiltersBorder(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            using var pen = new Pen(Color.FromArgb(240, 240, 250), 1f);
            e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
        }

        // ─────────────────────────────────────────────────────
        //  PaintNewChatButton
        // ─────────────────────────────────────────────────────
        private void PaintNewChatButton(object? sender, PaintEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Width < 4 || btn.Height < 4) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color parentBg = btn.Parent?.BackColor ?? C_BG;
            g.Clear(parentBg);

            int radius = _btnShape switch
            {
                ButtonShapeStyle.Circle => btn.Width / 2,
                ButtonShapeStyle.RoundedSquare => 10,
                ButtonShapeStyle.Square => 2,
                _ => btn.Width / 2,
            };

            var rc = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);

            var shadowR = new Rectangle(1, 2, btn.Width - 2, btn.Height - 2);
            using (var sp = new SolidBrush(Color.FromArgb(30, _btnColor)))
            using (var spath = RoundRect(shadowR, radius))
                g.FillPath(sp, spath);

            Color fillColor = _btnHovered
                ? ControlPaint.Light(_btnColor, 0.15f)
                : _btnColor;

            using (var path = RoundRect(rc, radius))
            using (var br = new SolidBrush(fillColor))
                g.FillPath(br, path);

            var innerRc = new Rectangle(1, 1, btn.Width - 3, btn.Height / 2);
            using (var ip = RoundRect(innerRc, radius))
            using (var hb = new LinearGradientBrush(
                new Point(0, 0), new Point(0, innerRc.Height),
                Color.FromArgb(60, 255, 255, 255), Color.Transparent))
                g.FillPath(hb, ip);

            if (_headerIcon != null)
            {
                var iconR = new Rectangle(
                    _btnIconPadding,
                    _btnIconPadding,
                    btn.Width - _btnIconPadding * 2,
                    btn.Height - _btnIconPadding * 2);
                g.DrawImage(_headerIcon, iconR);
            }
            else
            {
                DrawPencilIcon(g, btn.Width, btn.Height);
            }
        }

        private static void DrawPencilIcon(Graphics g, int w, int h)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float cx = w / 2f, cy = h / 2f, s = w * 0.24f;
            double a = -45 * Math.PI / 180;
            float cos = (float)Math.Cos(a), sin = (float)Math.Sin(a);

            using var pen = new Pen(Color.White, 1.8f)
            { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var br = new SolidBrush(Color.White);

            PointF tip = new(cx + cos * s * 1.2f, cy + sin * s * 1.2f);
            PointF tl = new(cx - cos * s * 0.7f - sin * s * 0.35f,
                             cy - sin * s * 0.7f + cos * s * 0.35f);
            PointF tr = new(cx - cos * s * 0.7f + sin * s * 0.35f,
                             cy - sin * s * 0.7f - cos * s * 0.35f);

            g.FillPolygon(br, new[] { tip, tl, tr });
            g.DrawLine(pen, tl,
                new PointF(cx - cos * s - sin * s * 0.35f,
                           cy - sin * s + cos * s * 0.35f));
            g.DrawLine(pen, tr,
                new PointF(cx - cos * s + sin * s * 0.35f,
                           cy - sin * s - cos * s * 0.35f));

            using var pe2 = new Pen(Color.FromArgb(180, 255, 255, 255), 2.2f)
            { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pe2,
                new PointF(cx - cos * s * 0.85f - sin * s * 0.35f,
                           cy - sin * s * 0.85f + cos * s * 0.35f),
                new PointF(cx - cos * s * 0.85f + sin * s * 0.35f,
                           cy - sin * s * 0.85f - cos * s * 0.35f));

            g.DrawLine(pen,
                new PointF(cx + cos * s, cy + sin * s),
                new PointF(cx - cos * s * 0.6f, cy - sin * s * 0.6f));
        }

        // ─────────────────────────────────────────────────────
        //  List
        // ─────────────────────────────────────────────────────
        private void RefreshList()
        {
            if (_listCtrl == null) return;
            var visible = FilteredChats();
            _listCtrl.SetItems(visible);
            _listCtrl.SetSelected(_selectedId);
        }

        private List<ChatListItemData> FilteredChats()
        {
            IEnumerable<ChatListItemData> q = _allChats;
            if (!string.IsNullOrEmpty(_searchQuery))
                q = q.Where(c =>
                    c.ContactName.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.LastMessage.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));
            q = _filter switch
            {
                "unread" => q.Where(c => c.UnreadCount > 0),
                "groups" => q.Where(c => c.IsGroup),
                _ => q,
            };
            return q.ToList();
        }

        private void OnVirtualChatSelected(object? sender, int id)
        {
            _selectedId = id;
            var chat = _allChats.FirstOrDefault(c => c.ContactId == id);
            if (chat != null) chat.UnreadCount = 0;
            ChatSelected?.Invoke(this, id);
        }

        private void OnPillClicked(object? sender, string filter)
        {
            _filter = filter;
            SyncPills();
            FilterChanged?.Invoke(this, filter);
            RefreshList();
        }

        private void SyncPills()
        {
            if (_pillAll == null) return;
            _pillAll.Active = _filter == "all";
            _pillUnread.Active = _filter == "unread";
            _pillGroups.Active = _filter == "groups";
        }

        // ─────────────────────────────────────────────────────
        //  Geometry helper
        // ─────────────────────────────────────────────────────
        internal static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            radius = Math.Max(1, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ─────────────────────────────────────────────────────
        //  Dispose
        // ─────────────────────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fontTitle?.Dispose(); _fontName?.Dispose();
                _fontMsg?.Dispose(); _fontTime?.Dispose();
                _fontPill?.Dispose(); _fontSearch?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ═════════════════════════════════════════════════════
        //  Nested: VirtualChatList — القلب: virtual scrolling
        // ═════════════════════════════════════════════════════
        private sealed class VirtualChatList : Control
        {
            // ── Layout constants ─────────────────────────────
            private const int ITEM_H = 72;
            private const int AVATAR_SZ = 48;
            private const int ONLINE_SZ = 10;
            private const int LEFT_PAD = 8;
            private const int LEFT_COL_W = 52;   // عمود الوقت + شارة الإشعار
            private const int AVATAR_R_PAD = 12;   // مسافة الـ avatar من اليمين
            private const int BUFFER_ITEMS = 10;   // عدد العناصر فوق/تحت viewport للتسليس

            // ── Data ─────────────────────────────────────────
            private readonly List<ChatListItemData> _items = new();
            private readonly Dictionary<int, int> _idToIndex = new();
            private int _selectedId = -1;
            private int _hoveredIndex = -1;
            private int _scrollOffset = 0;

            // ── Fonts (مرجع فقط — لا تعمل لها dispose) ───────
            private readonly Font _fontName;
            private readonly Font _fontMsg;
            private readonly Font _fontTime;

            // ── Scrollbar ────────────────────────────────────
            private readonly VScrollBar _vScroll;

            // ── Mouse-wheel filter (scroll-on-hover) ─────────
            private MouseWheelFilter? _wheelFilter;

            // ── Events ───────────────────────────────────────
            public event EventHandler<int>? ChatSelected;

            public VirtualChatList(Font fontName, Font fontMsg, Font fontTime)
            {
                _fontName = fontName;
                _fontMsg = fontMsg;
                _fontTime = fontTime;

                SetStyle(
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable,
                    true);
                UpdateStyles();

                BackColor = C_BG;
                TabStop = false;
                Cursor = Cursors.Default;

                _vScroll = new VScrollBar
                {
                    Dock = DockStyle.Right,
                    Visible = false,
                    SmallChange = ITEM_H,
                };
                _vScroll.Scroll += OnVScroll;
                _vScroll.ValueChanged += OnVScrollValueChanged;
                Controls.Add(_vScroll);
            }

            // ─────────────────────────────────────────────
            //  Public API (يُستدعى من ChatSidebar)
            // ─────────────────────────────────────────────
            public void SetItems(IList<ChatListItemData> items)
            {
                SuspendLayout();
                _items.Clear();
                _idToIndex.Clear();

                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        _items.Add(items[i]);
                        _idToIndex[items[i].ContactId] = i;
                    }
                }

                _hoveredIndex = -1;
                _scrollOffset = 0;
                UpdateScrollBar();
                if (_vScroll.Visible) _vScroll.Value = 0;
                ResumeLayout(false);
                Invalidate();
            }

            public void SetSelected(int contactId)
            {
                if (_selectedId == contactId) return;
                int prev = _selectedId;
                _selectedId = contactId;
                InvalidateItemById(prev);
                InvalidateItemById(contactId);
            }

            public void InvalidateItem(int contactId)
            {
                InvalidateItemById(contactId);
            }

            public void MoveToTop(int contactId)
            {
                if (!_idToIndex.TryGetValue(contactId, out int idx)) return;
                if (idx == 0) return;

                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(0, item);

                // إعادة بناء الفهرس
                _idToIndex.Clear();
                for (int i = 0; i < _items.Count; i++)
                    _idToIndex[_items[i].ContactId] = i;

                _hoveredIndex = -1;
                Invalidate();
            }

            // ─────────────────────────────────────────────
            //  Internal helpers
            // ─────────────────────────────────────────────
            private int ContentWidth =>
                _vScroll.Visible
                    ? Math.Max(1, Width - _vScroll.Width)
                    : Math.Max(1, Width);

            private void InvalidateItemById(int contactId)
            {
                if (contactId < 0) return;
                if (!_idToIndex.TryGetValue(contactId, out int idx)) return;
                InvalidateItemAt(idx);
            }

            private void InvalidateItemAt(int idx)
            {
                if (idx < 0 || idx >= _items.Count) return;
                int y = idx * ITEM_H - _scrollOffset;
                if (y + ITEM_H < 0 || y > Height) return;   // off-screen
                Invalidate(new Rectangle(0, y, ContentWidth, ITEM_H));
            }

            private int HitTest(Point p)
            {
                if (p.X < 0 || p.X >= ContentWidth) return -1;
                if (p.Y < 0 || p.Y >= Height) return -1;
                int abs = p.Y + _scrollOffset;
                int idx = abs / ITEM_H;
                if (idx < 0 || idx >= _items.Count) return -1;
                return idx;
            }

            private void UpdateScrollBar()
            {
                int totalH = _items.Count * ITEM_H;
                int vh = Math.Max(1, Height);

                if (totalH <= vh)
                {
                    if (_vScroll.Visible) _vScroll.Visible = false;
                    _scrollOffset = 0;
                    return;
                }

                if (!_vScroll.Visible) _vScroll.Visible = true;

                _vScroll.Minimum = 0;
                _vScroll.LargeChange = vh;
                _vScroll.SmallChange = ITEM_H;
                // الـ Maximum لازم يخلي maxOffset = totalH - vh
                // VScrollBar formula: maxReachable = Maximum - LargeChange + 1
                //   ⟹  Maximum = totalH - 1
                _vScroll.Maximum = totalH - 1;

                int maxOffset = Math.Max(0, totalH - vh);
                if (_scrollOffset > maxOffset) _scrollOffset = maxOffset;
                if (_scrollOffset < 0) _scrollOffset = 0;

                int safeVal = Math.Min(_scrollOffset, _vScroll.Maximum - _vScroll.LargeChange + 1);
                if (safeVal < 0) safeVal = 0;
                if (_vScroll.Value != safeVal) _vScroll.Value = safeVal;
            }

            private void OnVScroll(object? sender, ScrollEventArgs e)
            {
                _scrollOffset = e.NewValue;
                UpdateHoverFromCurrentMouse();
                Invalidate();
            }

            private void OnVScrollValueChanged(object? sender, EventArgs e)
            {
                if (_scrollOffset == _vScroll.Value) return;
                _scrollOffset = _vScroll.Value;
                UpdateHoverFromCurrentMouse();
                Invalidate();
            }

            private void UpdateHoverFromCurrentMouse()
            {
                if (!IsHandleCreated) return;
                Point p = PointToClient(MousePosition);
                int idx = ClientRectangle.Contains(p) ? HitTest(p) : -1;
                if (idx != _hoveredIndex)
                {
                    int prev = _hoveredIndex;
                    _hoveredIndex = idx;
                    InvalidateItemAt(prev);
                    InvalidateItemAt(idx);
                }
            }

            // ─────────────────────────────────────────────
            //  Lifecycle: install/remove wheel filter
            // ─────────────────────────────────────────────
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                if (_wheelFilter == null)
                {
                    _wheelFilter = new MouseWheelFilter(this);
                    Application.AddMessageFilter(_wheelFilter);
                }
            }

            protected override void OnHandleDestroyed(EventArgs e)
            {
                if (_wheelFilter != null)
                {
                    Application.RemoveMessageFilter(_wheelFilter);
                    _wheelFilter = null;
                }
                base.OnHandleDestroyed(e);
            }

            // ─────────────────────────────────────────────
            //  Resize
            // ─────────────────────────────────────────────
            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                UpdateScrollBar();
                Invalidate();
            }

            // ─────────────────────────────────────────────
            //  Mouse
            // ─────────────────────────────────────────────
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                int idx = HitTest(e.Location);
                if (idx == _hoveredIndex) return;
                int prev = _hoveredIndex;
                _hoveredIndex = idx;
                InvalidateItemAt(prev);
                InvalidateItemAt(idx);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (_hoveredIndex < 0) return;
                int prev = _hoveredIndex;
                _hoveredIndex = -1;
                InvalidateItemAt(prev);
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                if (e.Button != MouseButtons.Left) return;
                int idx = HitTest(e.Location);
                if (idx < 0) return;

                var item = _items[idx];
                int prev = _selectedId;
                _selectedId = item.ContactId;

                // وضعها كمقروءة محلياً عشان الـ badge يختفي فوراً
                if (item.UnreadCount > 0) item.UnreadCount = 0;

                InvalidateItemById(prev);
                InvalidateItemAt(idx);

                ChatSelected?.Invoke(this, item.ContactId);
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                if (!_vScroll.Visible) return;

                // 3 صفوف لكل notch (= 120 delta)
                int notches = e.Delta / 120;
                int delta = -notches * ITEM_H * 3;

                int maxVal = Math.Max(0, _vScroll.Maximum - _vScroll.LargeChange + 1);
                int newVal = _vScroll.Value + delta;
                if (newVal < _vScroll.Minimum) newVal = _vScroll.Minimum;
                if (newVal > maxVal) newVal = maxVal;

                if (newVal != _vScroll.Value) _vScroll.Value = newVal;
            }

            // ─────────────────────────────────────────────
            //  Keyboard (لو الـ control عنده focus)
            // ─────────────────────────────────────────────
            protected override bool IsInputKey(Keys keyData)
            {
                return keyData switch
                {
                    Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown
                    or Keys.Home or Keys.End => true,
                    _ => base.IsInputKey(keyData),
                };
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (!_vScroll.Visible) return;

                int maxVal = Math.Max(0, _vScroll.Maximum - _vScroll.LargeChange + 1);
                int newVal = _vScroll.Value;

                switch (e.KeyCode)
                {
                    case Keys.Up: newVal -= ITEM_H; break;
                    case Keys.Down: newVal += ITEM_H; break;
                    case Keys.PageUp: newVal -= _vScroll.LargeChange; break;
                    case Keys.PageDown: newVal += _vScroll.LargeChange; break;
                    case Keys.Home: newVal = 0; break;
                    case Keys.End: newVal = maxVal; break;
                    default: return;
                }

                if (newVal < 0) newVal = 0;
                if (newVal > maxVal) newVal = maxVal;
                if (newVal != _vScroll.Value) _vScroll.Value = newVal;
                e.Handled = true;
            }

            // ─────────────────────────────────────────────
            //  OnPaint — قلب الـ virtual scrolling
            // ─────────────────────────────────────────────
            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // خلفية كاملة
                using (var bgBr = new SolidBrush(C_BG))
                    g.FillRectangle(bgBr, 0, 0, Width, Height);

                if (_items.Count == 0) return;

                int contentW = ContentWidth;
                int vh = Height;

                // حساب نطاق العناصر المرئية + buffer
                int firstIdx = Math.Max(0, _scrollOffset / ITEM_H - BUFFER_ITEMS);
                int lastIdx = Math.Min(_items.Count - 1,
                                        (_scrollOffset + vh) / ITEM_H + BUFFER_ITEMS);

                Rectangle clip = e.ClipRectangle;

                for (int i = firstIdx; i <= lastIdx; i++)
                {
                    int y = i * ITEM_H - _scrollOffset;
                    var rc = new Rectangle(0, y, contentW, ITEM_H);

                    // skip اللي خارج clip rect عشان الأداء
                    if (rc.Bottom < clip.Top || rc.Top > clip.Bottom) continue;

                    DrawItem(g, rc, _items[i],
                        isSelected: _items[i].ContactId == _selectedId,
                        isHovered: i == _hoveredIndex,
                        isLast: i == _items.Count - 1);
                }
            }

            // ─────────────────────────────────────────────
            //  DrawItem — رسم عنصر واحد في rectangle محدد
            //  (هذا اللي كان OnPaint بتاع ChatItemControl قديماً)
            // ─────────────────────────────────────────────
            private void DrawItem(Graphics g, Rectangle rc, ChatListItemData data,
                                  bool isSelected, bool isHovered, bool isLast)
            {
                // 1) الخلفية
                Color bg = isSelected ? C_ITEM_SEL
                         : isHovered ? C_ITEM_HOV
                                      : C_BG;
                using (var br = new SolidBrush(bg))
                    g.FillRectangle(br, rc);

                // 2) مؤشر اختيار (شريط على اليمين + glow على اليسار)
                if (isSelected)
                {
                    var stripe = new Rectangle(rc.Right - 3, rc.Y + 10, 3, rc.Height - 20);
                    using (var sb = new LinearGradientBrush(
                        stripe, C_ACCENT, Color.FromArgb(140, C_ACCENT), 90f))
                        g.FillRectangle(sb, stripe);

                    using (var wb = new LinearGradientBrush(
                        new Rectangle(rc.X, rc.Y, 60, rc.Height),
                        Color.FromArgb(12, C_ACCENT), Color.Transparent, 0f))
                        g.FillRectangle(wb, rc.X, rc.Y, 60, rc.Height);
                }

                // 3) خط فاصل
                if (!isLast && !isSelected)
                {
                    using var sp = new Pen(C_SEP, 1f);
                    g.DrawLine(sp, rc.X + 72, rc.Bottom - 1, rc.Right - 16, rc.Bottom - 1);
                }

                // 4) الـ Avatar (يمين)
                int avatarRight = rc.Right - AVATAR_R_PAD;
                int avatarTop = rc.Y + (rc.Height - AVATAR_SZ) / 2;
                var avatarRc = new Rectangle(avatarRight - AVATAR_SZ, avatarTop, AVATAR_SZ, AVATAR_SZ);
                DrawAvatar(g, avatarRc, data.Avatar, data.ContactName);

                // 5) النقطة الخضراء (online)
                if (data.IsOnline)
                {
                    int dx = avatarRc.Right - ONLINE_SZ + 1;
                    int dy = avatarRc.Bottom - ONLINE_SZ + 1;
                    using (var wb = new SolidBrush(C_BG))
                        g.FillEllipse(wb, dx - 2, dy - 2, ONLINE_SZ + 4, ONLINE_SZ + 4);
                    using (var gb = new SolidBrush(C_ONLINE))
                        g.FillEllipse(gb, dx, dy, ONLINE_SZ, ONLINE_SZ);
                }

                // 6) العمود الأيسر — الوقت (في أعلى العمود)
                var timeRect = new Rectangle(rc.X + LEFT_PAD, avatarTop + 4, LEFT_COL_W, 18);
                using (var timeSf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisCharacter,
                })
                using (var tb = new SolidBrush(C_TIME))
                    g.DrawString(data.Timestamp ?? string.Empty, _fontTime, tb, timeRect, timeSf);

                // 7) شارة الإشعار (تحت الوقت في نفس العمود)
                if (data.UnreadCount > 0)
                {
                    string txt = data.UnreadCount > 99 ? "99+" : data.UnreadCount.ToString();
                    SizeF txtsz = g.MeasureString(txt, _fontTime);
                    int bw = (int)Math.Max(txtsz.Width + 12, 22);
                    int bh = 20;
                    int bx = rc.X + LEFT_PAD + (LEFT_COL_W - bw) / 2;
                    int by = avatarTop + AVATAR_SZ - bh - 2;

                    using (var glowPath = RoundRect(new Rectangle(bx - 3, by - 3, bw + 6, bh + 6), 12))
                    using (var glb = new SolidBrush(Color.FromArgb(35, C_ACCENT)))
                        g.FillPath(glb, glowPath);

                    using (var badgePath = RoundRect(new Rectangle(bx, by, bw, bh), 10))
                    using (var bb = new SolidBrush(C_ACCENT))
                        g.FillPath(bb, badgePath);

                    using var numSf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };
                    using var wb = new SolidBrush(Color.White);
                    g.DrawString(txt, _fontTime, wb, new Rectangle(bx, by, bw, bh), numSf);
                }

                // 8) الوسط — الاسم + آخر رسالة
                int textRight = avatarRc.Left - 10;
                int textLeft = rc.X + LEFT_PAD + LEFT_COL_W + 6;
                int textWidth = textRight - textLeft;

                if (textWidth > 10)
                {
                    using var nameSf = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };
                    using (var nb = new SolidBrush(C_NAME))
                        g.DrawString(data.ContactName ?? string.Empty, _fontName, nb,
                            new Rectangle(textLeft, avatarTop + 4, textWidth, 22), nameSf);

                    string displayMsg = data.IsLastMessageSent
                        ? "أنت: " + (data.LastMessage ?? string.Empty)
                        : (data.LastMessage ?? string.Empty);

                    using var msgSf = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };
                    using (var mb = new SolidBrush(C_MSG))
                        g.DrawString(displayMsg, _fontMsg, mb,
                            new Rectangle(textLeft, avatarTop + 30, textWidth, 20), msgSf);
                }
            }

            // ─────────────────────────────────────────────
            //  DrawAvatar (نفس الكود القديم)
            // ─────────────────────────────────────────────
            private static void DrawAvatar(Graphics g, Rectangle r, Image? avatar, string name)
            {
                if (r.Width < 2 || r.Height < 2) return;
                using var path = RoundRect(r, r.Width / 2);
                Region? saved = g.Clip.Clone();
                g.SetClip(path);

                if (avatar != null)
                {
                    g.DrawImage(avatar, r);
                }
                else
                {
                    var colors = new[]
                    {
                        new[] { Color.FromArgb(124, 111, 247), Color.FromArgb(167,  97, 247) },
                        new[] { Color.FromArgb( 16, 185, 129), Color.FromArgb( 45, 212, 191) },
                        new[] { Color.FromArgb(245, 158,  11), Color.FromArgb(251, 191,  36) },
                        new[] { Color.FromArgb(239,  68,  68), Color.FromArgb(252, 165, 165) },
                        new[] { Color.FromArgb( 59, 130, 246), Color.FromArgb(147, 197, 253) },
                        new[] { Color.FromArgb(168,  85, 247), Color.FromArgb(216, 180, 254) },
                    };
                    int hash = string.IsNullOrEmpty(name) ? 0 : Math.Abs(name.GetHashCode());
                    var pair = colors[hash % colors.Length];
                    using var gr = new LinearGradientBrush(r, pair[0], pair[1], 135f);
                    g.FillPath(gr, path);

                    string initial = string.IsNullOrEmpty(name) ? "?" : name[0].ToString();
                    string face = ChatSidebar.IsFontInstalled("Cairo") ? "Cairo" : "Segoe UI";
                    using var fi = new Font(face, 16f, FontStyle.Bold, GraphicsUnit.Point);
                    using var ib = new SolidBrush(Color.White);
                    using var isf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                    };
                    g.DrawString(initial, fi, ib, r, isf);
                }

                g.Clip = saved ?? new Region();
                saved?.Dispose();

                using var ring = new Pen(Color.FromArgb(18, 0, 0, 0), 1f);
                g.DrawEllipse(ring, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
            }

            // ─────────────────────────────────────────────
            //  Dispose
            // ─────────────────────────────────────────────
            protected override void Dispose(bool disposing)
            {
                if (disposing && _wheelFilter != null)
                {
                    Application.RemoveMessageFilter(_wheelFilter);
                    _wheelFilter = null;
                }
                base.Dispose(disposing);
            }

            // ═════════════════════════════════════════════════
            //  Nested: MouseWheelFilter
            //  السبب: WM_MOUSEWHEEL في Windows يروح للـ control
            //  اللي معاه focus، مش اللي تحت الماوس. عشان نسوي
            //  scroll-on-hover (زي WhatsApp Desktop) نلتقط الرسالة
            //  من Application.MessageFilter ونرجعها للقائمة لو
            //  الماوس فوقها.
            // ═════════════════════════════════════════════════
            private sealed class MouseWheelFilter : IMessageFilter
            {
                private const int WM_MOUSEWHEEL = 0x020A;
                private readonly VirtualChatList _target;

                public MouseWheelFilter(VirtualChatList target) { _target = target; }

                public bool PreFilterMessage(ref Message m)
                {
                    if (m.Msg != WM_MOUSEWHEEL) return false;
                    if (_target == null || _target.IsDisposed ||
                        !_target.IsHandleCreated || !_target.Visible)
                        return false;

                    Point screenPt;
                    try { screenPt = Cursor.Position; }
                    catch { return false; }

                    Point localPt;
                    try { localPt = _target.PointToClient(screenPt); }
                    catch { return false; }

                    if (!_target.ClientRectangle.Contains(localPt)) return false;

                    // استخراج الـ delta من wParam (HIWORD)
                    long wp = m.WParam.ToInt64();
                    short delta = (short)((wp >> 16) & 0xFFFF);

                    var args = new MouseEventArgs(
                        MouseButtons.None, 0, localPt.X, localPt.Y, delta);
                    _target.OnMouseWheel(args);
                    return true; // استهلكنا الرسالة
                }
            }
        }

        // ═════════════════════════════════════════════════════
        //  Nested: VScrollOnlyFLP (للـ filter pills فقط)
        // ═════════════════════════════════════════════════════
        private sealed class VScrollOnlyFLP : FlowLayoutPanel
        {
            public VScrollOnlyFLP()
            {
                HorizontalScroll.Enabled = false;
                HorizontalScroll.Visible = false;
                AutoScrollMinSize = new Size(0, 0);
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint, true);
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
                HorizontalScroll.Enabled = false;
                HorizontalScroll.Visible = false;
            }

            protected override void OnScroll(ScrollEventArgs se)
            {
                if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll) return;
                base.OnScroll(se);
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    var cp = base.CreateParams;
                    cp.Style &= ~0x00100000; // WS_HSCROLL
                    return cp;
                }
            }
        }

        // ═════════════════════════════════════════════════════
        //  Nested: PillButton (لم يتغير)
        // ═════════════════════════════════════════════════════
        private sealed class PillButton : Control
        {
            private readonly string _label;
            private readonly string _filterId;
            private readonly Font _font;
            private bool _active;
            private bool _hovered;

            public event EventHandler<string>? PillClicked;
            public bool Active { get => _active; set { _active = value; Invalidate(); } }

            public PillButton(string label, string filterId, Font font, bool active)
            {
                _label = label;
                _filterId = filterId;
                _font = font;
                _active = active;
                Cursor = Cursors.Hand;

                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);

                SizeF sz;
                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                    sz = g.MeasureString(label, font);

                Size = new Size((int)sz.Width + 28, 32);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 2 || Height < 2) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var rc = new Rectangle(0, 0, Width - 1, Height - 1);
                using var path = RoundRect(rc, Height / 2);

                if (_active)
                {
                    var sr = new Rectangle(0, 2, Width - 1, Height);
                    using var sp = new SolidBrush(Color.FromArgb(30, C_ACCENT));
                    using var sh = RoundRect(sr, Height / 2);
                    g.FillPath(sp, sh);
                }

                Color bg = _active ? C_ACCENT
                         : _hovered ? Color.FromArgb(232, 228, 255)
                                     : C_PILL_OFF;

                using (var br = new SolidBrush(bg)) g.FillPath(br, path);

                if (_active)
                {
                    var topHalf = new Rectangle(0, 0, Width - 1, Height / 2);
                    using var hb = new LinearGradientBrush(
                        new Point(0, 0), new Point(0, Height / 2),
                        Color.FromArgb(45, 255, 255, 255), Color.Transparent);
                    using var hp = RoundRect(topHalf, topHalf.Height / 2);
                    g.FillPath(hb, hp);
                }

                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };
                using var tbr = new SolidBrush(_active ? Color.White : C_PILL_TXT_OFF);
                g.DrawString(_label, _font, tbr, (RectangleF)rc, sf);
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnClick(EventArgs e) { PillClicked?.Invoke(this, _filterId); base.OnClick(e); }
        }

        // ═════════════════════════════════════════════════════
        //  Nested: SearchBoxPanel (لم يتغير)
        // ═════════════════════════════════════════════════════
        private sealed class SearchBoxPanel : Panel
        {
            private readonly TextBox _tb;
            private bool _focused;

            public event EventHandler? TextChanged;
            public string SearchText => _tb.Text;

            public SearchBoxPanel(Font font)
            {
                Height = 36;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);

                _tb = new TextBox
                {
                    BorderStyle = BorderStyle.None,
                    BackColor = C_SEARCH_BG,
                    ForeColor = C_TITLE,
                    Font = font,
                    RightToLeft = RightToLeft.Yes,
                    TextAlign = HorizontalAlignment.Right,
                    PlaceholderText = "البحث في المحادثات...",
                };
                _tb.TextChanged += (s, e) => { TextChanged?.Invoke(this, e); Invalidate(); };
                _tb.GotFocus += (s, e) => { _focused = true; Invalidate(); };
                _tb.LostFocus += (s, e) => { _focused = false; Invalidate(); };
                Controls.Add(_tb);
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
                if (_tb == null) return;
                int iconW = 28;
                int padV = Math.Max(0, (Height - _tb.PreferredHeight) / 2);
                _tb.SetBounds(iconW, padV, Width - iconW - 8, _tb.PreferredHeight);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 2 || Height < 2) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var rc = new Rectangle(0, 0, Width - 1, Height - 1);
                using var path = RoundRect(rc, 12);
                using var br = new SolidBrush(C_SEARCH_BG);
                g.FillPath(br, path);

                Color borderC = _focused ? C_ACCENT : C_BORDER;
                float borderW = _focused ? 1.8f : 1f;
                using var pen = new Pen(borderC, borderW);
                g.DrawPath(pen, path);

                float cx = Width - 16f, cy = Height / 2f, r = 5f;
                using var mp = new Pen(C_MSG, 1.5f)
                { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawEllipse(mp, cx - r, cy - r, r * 2, r * 2);
                g.DrawLine(mp, cx - r * 0.7f, cy + r * 0.7f, cx - r * 1.8f, cy + r * 1.8f);
            }
        }
    }
}