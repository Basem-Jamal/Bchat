// ============================================================
//  BChat — ChatSidebar Custom Control  (v3 — Designer Properties)
//  Namespace : BChat.Custom_Controal.Chat
//  Target    : .NET 8 / Windows Forms
//
//  التحديثات في v3:
//  ① إصلاح مشكلة الـ Panel (مستطيل خلف الأيقونة) → g.Clear(parentBg)
//  ② خصائص Designer كاملة للزر: ButtonColor, ButtonSize,
//     ButtonIconPadding, ButtonShape, HeaderTitle, HeaderIcon
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
    [Description("WhatsApp/Telegram-style chat list sidebar.")]
    public class ChatSidebar : UserControl
    {
        // ── Design Tokens ────────────────────────────────────
        internal static readonly Color C_BG = Color.FromArgb(255, 255, 255);
        internal static readonly Color C_BORDER = Color.FromArgb(226, 232, 240);
        internal static readonly Color C_SEARCH_BG = Color.FromArgb(248, 247, 255);
        internal static readonly Color C_ACCENT = Color.FromArgb(124, 111, 247);
        internal static readonly Color C_ONLINE = Color.FromArgb(16, 185, 129);
        internal static readonly Color C_ITEM_SEL = Color.FromArgb(248, 247, 255);
        internal static readonly Color C_ITEM_HOV = Color.FromArgb(252, 251, 255);
        internal static readonly Color C_TITLE = Color.FromArgb(15, 23, 42);
        internal static readonly Color C_NAME = Color.FromArgb(15, 23, 42);
        internal static readonly Color C_MSG = Color.FromArgb(100, 116, 139);
        internal static readonly Color C_TIME = Color.FromArgb(148, 163, 184);
        internal static readonly Color C_PILL_OFF = Color.FromArgb(241, 245, 249);
        internal static readonly Color C_PILL_TXT_OFF = Color.FromArgb(100, 116, 139);
        internal static readonly Color C_SEP = Color.FromArgb(241, 245, 249);

        // ── Layout ───────────────────────────────────────────
        private const int H_HEADER = 64;
        private const int H_SEARCH = 56;
        private const int H_FILTERS = 52;
        private const int H_ITEM = 76;
        private const int AVATAR_SZ = 48;
        private const int ONLINE_SZ = 11;
        private const int BTN_SZ_DEFAULT = 38;

        // ── Designer Properties: New Chat Button ─────────────
        private Image? _headerIcon = null;
        private Color _btnColor = Color.FromArgb(124, 111, 247);
        private int _btnSize = BTN_SZ_DEFAULT;
        private int _btnIconPadding = 9;
        private ButtonShapeStyle _btnShape = ButtonShapeStyle.Circle;
        private string _headerTitleText = "الدردشات";

        // ─────────────────────────────────────────────────────
        //  [A] أيقونة مخصصة داخل الزر
        // ─────────────────────────────────────────────────────
        [Category("BChat — New Chat Button")]
        [Description("أيقونة مخصصة داخل زر المحادثة الجديدة. اتركها فارغة لرسم أيقونة القلم الافتراضية.")]
        [DefaultValue(null)]
        public Image? HeaderIcon
        {
            get => _headerIcon;
            set { _headerIcon = value; _btnNewChat?.Invalidate(); }
        }

        // ─────────────────────────────────────────────────────
        //  [B] لون خلفية الزر
        // ─────────────────────────────────────────────────────
        [Category("BChat — New Chat Button")]
        [Description("لون خلفية زر المحادثة الجديدة.")]
        public Color ButtonColor
        {
            get => _btnColor;
            set { _btnColor = value; _btnNewChat?.Invalidate(); }
        }

        // ─────────────────────────────────────────────────────
        //  [C] حجم الزر
        // ─────────────────────────────────────────────────────
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

        // ─────────────────────────────────────────────────────
        //  [D] padding الأيقونة داخل الزر
        // ─────────────────────────────────────────────────────
        [Category("BChat — New Chat Button")]
        [Description("مسافة الأيقونة عن حواف الزر بالبكسل. كلما زادت كلما صغرت الأيقونة.")]
        [DefaultValue(9)]
        public int ButtonIconPadding
        {
            get => _btnIconPadding;
            set { _btnIconPadding = Math.Max(2, value); _btnNewChat?.Invalidate(); }
        }

        // ─────────────────────────────────────────────────────
        //  [E] شكل الزر
        // ─────────────────────────────────────────────────────
        [Category("BChat — New Chat Button")]
        [Description("شكل الزر: دائرة كاملة، أو مستطيل بحواف مدورة، أو مستطيل حاد.")]
        [DefaultValue(ButtonShapeStyle.Circle)]
        public ButtonShapeStyle ButtonShape
        {
            get => _btnShape;
            set { _btnShape = value; _btnNewChat?.Invalidate(); }
        }

        // ─────────────────────────────────────────────────────
        //  [F] عنوان الهيدر
        // ─────────────────────────────────────────────────────
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
        private VScrollOnlyFLP _flpList = null!;
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
            if (_flpList == null) return;
            foreach (Control c in _flpList.Controls)
                if (c is ChatItemControl ci)
                    ci.IsSelected = (ci.Data.ContactId == contactId);
        }

        public void RefreshItem(int contactId)
        {
            if (_flpList == null) return;
            foreach (Control c in _flpList.Controls)
                if (c is ChatItemControl ci && ci.Data.ContactId == contactId)
                { ci.Invalidate(); break; }
        }

        public void MoveItemToTop(int contactId)
        {
            if (_flpList == null) return;
            ChatItemControl? target = null;
            foreach (Control c in _flpList.Controls)
                if (c is ChatItemControl ci && ci.Data.ContactId == contactId)
                { target = ci; break; }
            if (target == null) return;
            _flpList.SuspendLayout();
            _flpList.Controls.SetChildIndex(target, 0);
            _flpList.ResumeLayout(true);
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
            // FIX: BackColor = Transparent حتى لا يظهر مستطيل خلف الدائرة
            _btnNewChat = new Button
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,   // ← مهم: شفاف
                Cursor = Cursors.Hand,
                TabStop = false,
                Size = new Size(_btnSize, _btnSize),
            };
            _btnNewChat.FlatAppearance.BorderSize = 0;
            _btnNewChat.FlatAppearance.MouseOverBackColor = Color.Transparent; // ← يمنع مستطيل hover
            _btnNewChat.FlatAppearance.MouseDownBackColor = Color.Transparent; // ← يمنع مستطيل click
            _btnNewChat.Paint += PaintNewChatButton;
            _btnNewChat.Click += (s, e) => NewChatClicked?.Invoke(this, EventArgs.Empty);

            // hover: نحدّث الـ flag فقط ونعيد الرسم
            _btnNewChat.MouseEnter += (s, e) => { _btnHovered = true; _btnNewChat.Invalidate(); };
            _btnNewChat.MouseLeave += (s, e) => { _btnHovered = false; _btnNewChat.Invalidate(); };

            // Wrap button with margin
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

            // ── Chat List ────────────────────────────────────
            _pnlList = new Panel { Dock = DockStyle.Fill, BackColor = C_BG };

            _flpList = new VScrollOnlyFLP
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = C_BG,
            };
            _flpList.HandleCreated += (s, e) =>
            {
                if (s is FlowLayoutPanel flp)
                    flp.AutoScroll = true;
            };

            _pnlList.Controls.Add(_flpList);

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
        //  PaintNewChatButton — الإصلاح الرئيسي هنا
        // ─────────────────────────────────────────────────────
        private void PaintNewChatButton(object? sender, PaintEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Width < 4 || btn.Height < 4) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // ── FIX: امسح بلون الأب أولاً ──────────────────
            // هذا يزيل المستطيل الذي يظهر خلف الدائرة
            Color parentBg = btn.Parent?.BackColor ?? C_BG;
            g.Clear(parentBg);

            // ── حساب الـ radius حسب الشكل المختار ──────────
            int radius = _btnShape switch
            {
                ButtonShapeStyle.Circle => btn.Width / 2,
                ButtonShapeStyle.RoundedSquare => 10,
                ButtonShapeStyle.Square => 2,
                _ => btn.Width / 2,
            };

            var rc = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);

            // Shadow
            var shadowR = new Rectangle(1, 2, btn.Width - 2, btn.Height - 2);
            using (var sp = new SolidBrush(Color.FromArgb(30, _btnColor)))
            using (var spath = RoundRect(shadowR, radius))
                g.FillPath(sp, spath);

            // Button fill — أفتح قليلاً عند hover
            Color fillColor = _btnHovered
                ? ControlPaint.Light(_btnColor, 0.15f)
                : _btnColor;

            using (var path = RoundRect(rc, radius))
            using (var br = new SolidBrush(fillColor))
                g.FillPath(br, path);

            // Highlight sheen
            var innerRc = new Rectangle(1, 1, btn.Width - 3, btn.Height / 2);
            using (var ip = RoundRect(innerRc, radius))
            using (var hb = new LinearGradientBrush(
                new Point(0, 0), new Point(0, innerRc.Height),
                Color.FromArgb(60, 255, 255, 255), Color.Transparent))
                g.FillPath(hb, ip);

            // Icon: صورة مخصصة أو أيقونة القلم الافتراضية
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
            if (_flpList == null) return;
            _flpList.SuspendLayout();
            foreach (Control c in _flpList.Controls) c.Dispose();
            _flpList.Controls.Clear();

            var visible = FilteredChats();
            for (int i = 0; i < visible.Count; i++)
            {
                var item = new ChatItemControl(visible[i], _fontName, _fontMsg, _fontTime)
                {
                    Width = Math.Max(_flpList.ClientSize.Width, 1),
                    Height = H_ITEM,
                    IsSelected = visible[i].ContactId == _selectedId,
                    IsLast = i == visible.Count - 1,
                };
                item.ItemClicked += OnChatItemClicked;
                _flpList.Controls.Add(item);
            }
            _flpList.ResumeLayout(true);
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

        private void OnChatItemClicked(object? sender, int id)
        {
            _selectedId = id;
            var chat = _allChats.FirstOrDefault(c => c.ContactId == id);
            if (chat != null) chat.UnreadCount = 0;
            foreach (Control c in _flpList.Controls)
                if (c is ChatItemControl ci) ci.IsSelected = ci.Data.ContactId == id;
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_flpList == null) return;
            foreach (Control c in _flpList.Controls)
                c.Width = Math.Max(_flpList.ClientSize.Width, 1);
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
        //  Nested: VScrollOnlyFLP
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
        //  Nested: PillButton
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
        //  Nested: SearchBoxPanel
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

        // ═════════════════════════════════════════════════════
        //  Nested: ChatItemControl
        // ═════════════════════════════════════════════════════
        private sealed class ChatItemControl : Control
        {
            private readonly Font _fontName;
            private readonly Font _fontMsg;
            private readonly Font _fontTime;
            private bool _hovered;
            private bool _selected;
            private bool _isLast;

            public ChatListItemData Data { get; }
            public event EventHandler<int>? ItemClicked;

            public bool IsSelected { get => _selected; set { _selected = value; Invalidate(); } }
            public bool IsLast { get => _isLast; set { _isLast = value; Invalidate(); } }

            public ChatItemControl(ChatListItemData data,
                                   Font fontName, Font fontMsg, Font fontTime)
            {
                Data = data;
                _fontName = fontName;
                _fontMsg = fontMsg;
                _fontTime = fontTime;
                Height = H_ITEM;
                Cursor = Cursors.Hand;

                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 4 || Height < 4) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Color bg = _selected ? C_ITEM_SEL : _hovered ? C_ITEM_HOV : C_BG;
                using (var br = new SolidBrush(bg))
                    g.FillRectangle(br, 0, 0, Width, Height);

                if (_selected)
                {
                    using var sb = new LinearGradientBrush(
                        new Rectangle(Width - 3, 10, 3, Height - 20),
                        C_ACCENT, Color.FromArgb(140, C_ACCENT), 90f);
                    g.FillRectangle(sb, Width - 3, 10, 3, Height - 20);

                    using var wb = new LinearGradientBrush(
                        new Rectangle(0, 0, 60, Height),
                        Color.FromArgb(12, C_ACCENT), Color.Transparent, 0f);
                    g.FillRectangle(wb, 0, 0, 60, Height);
                }

                if (!_isLast && !_selected)
                {
                    using var sp = new Pen(C_SEP, 1f);
                    g.DrawLine(sp, 72, Height - 1, Width - 16, Height - 1);
                }

                int avatarRight = Width - 12;
                int avatarTop = (Height - AVATAR_SZ) / 2;
                var avatarRc = new Rectangle(avatarRight - AVATAR_SZ, avatarTop, AVATAR_SZ, AVATAR_SZ);
                DrawAvatar(g, avatarRc, Data.Avatar, Data.ContactName);

                if (Data.IsOnline)
                {
                    int dx = avatarRc.Right - ONLINE_SZ + 1;
                    int dy = avatarRc.Bottom - ONLINE_SZ + 1;
                    using var wb = new SolidBrush(C_BG);
                    g.FillEllipse(wb, dx - 2, dy - 2, ONLINE_SZ + 4, ONLINE_SZ + 4);
                    using var gb = new SolidBrush(C_ONLINE);
                    g.FillEllipse(gb, dx, dy, ONLINE_SZ, ONLINE_SZ);
                }

                const int LEFT_PAD = 8;
                const int TIME_EXTRA = 12;

                SizeF timeSz = g.MeasureString(
                    string.IsNullOrEmpty(Data.Timestamp) ? "12:30 ص" : Data.Timestamp,
                    _fontTime);
                int timeW = (int)timeSz.Width + TIME_EXTRA;

                int textRight = avatarRc.Left - 10;
                int textLeft = LEFT_PAD + timeW + 6;
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
                        g.DrawString(Data.ContactName, _fontName, nb,
                            new Rectangle(textLeft, avatarTop + 4, textWidth, 22), nameSf);

                    string displayMsg = Data.IsLastMessageSent
                        ? "أنت: " + Data.LastMessage
                        : Data.LastMessage;
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

                var timeRect = new Rectangle(LEFT_PAD, avatarTop + 4, timeW, 20);
                using var timeSf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };
                using (var tb = new SolidBrush(C_TIME))
                    g.DrawString(Data.Timestamp, _fontTime, tb, timeRect, timeSf);

                if (Data.UnreadCount > 0)
                {
                    string txt = Data.UnreadCount > 99 ? "99+" : Data.UnreadCount.ToString();
                    SizeF txtsz = g.MeasureString(txt, _fontTime);
                    int bw = (int)Math.Max(txtsz.Width + 12, 22);
                    int bh = 20;
                    int bx = LEFT_PAD + (timeW - bw) / 2;
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
                    using (var wb = new SolidBrush(Color.White))
                        g.DrawString(txt, _fontTime, wb,
                            new Rectangle(bx, by, bw, bh), numSf);
                }
            }

            private static void DrawAvatar(Graphics g, Rectangle r, Image? avatar, string name)
            {
                if (r.Width < 2 || r.Height < 2) return;
                using var path = RoundRect(r, r.Width / 2);
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
                    var pair = colors[Math.Abs(name.GetHashCode()) % colors.Length];
                    using var gr = new LinearGradientBrush(r, pair[0], pair[1], 135f);
                    g.FillPath(gr, path);

                    string initial = name.Length > 0 ? name[0].ToString() : "?";
                    string face = IsFontInstalled("Cairo") ? "Cairo" : "Segoe UI";
                    using var fi = new Font(face, 16f, FontStyle.Bold, GraphicsUnit.Point);
                    using var ib = new SolidBrush(Color.White);
                    using var isf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                    };
                    g.DrawString(initial, fi, ib, r, isf);
                }

                g.ResetClip();

                using var ring = new Pen(Color.FromArgb(18, 0, 0, 0), 1f);
                g.DrawEllipse(ring, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
            }

            private static bool IsFontInstalled(string name)
            {
                using var fc = new InstalledFontCollection();
                return fc.Families.Any(f =>
                    f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnClick(EventArgs e) { ItemClicked?.Invoke(this, Data.ContactId); base.OnClick(e); }
        }
    }
}