// ============================================================
//  BChat — ChatSidebar Custom Control
//  Namespace : BChat.Custom_Controal.Chat
//  Target    : .NET 8 / Windows Forms
//  Fix       : .NET 8 OOP Designer safe (DesignToolsServer process guard
//              + deferred AutoScroll via HandleCreated)
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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

        public bool IsLastMessageSent { get; set; } // ✅ أضف هذا

        public DateTime LastMessageAt { get; set; } = DateTime.MinValue;

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
        internal static readonly Color C_BORDER = Color.FromArgb(241, 245, 249);
        internal static readonly Color C_SEARCH_BG = Color.FromArgb(248, 247, 255);
        internal static readonly Color C_ACCENT = Color.FromArgb(124, 111, 247);
        internal static readonly Color C_ONLINE = Color.FromArgb(16, 185, 129);
        internal static readonly Color C_ITEM_SEL = Color.FromArgb(248, 247, 255);
        internal static readonly Color C_ITEM_HOV = Color.FromArgb(250, 250, 255);
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
        private const int H_FILTERS = 48;
        private const int H_ITEM = 72;
        private const int AVATAR_SZ = 48;
        private const int ONLINE_SZ = 10;
        private const int BTN_SZ = 40;

        // ── Child Controls ───────────────────────────────────
        private Panel _pnlHeader = null!;
        private Panel _pnlSearch = null!;
        private Panel _pnlFilters = null!;
        private Panel _pnlList = null!;
        private FlowLayoutPanel _flpFilters = null!;
        private FlowLayoutPanel _flpList = null!;
        private PillButton _pillAll = null!;
        private PillButton _pillUnread = null!;
        private PillButton _pillGroups = null!;
        private Button _btnNewChat = null!;

        // ── State ────────────────────────────────────────────
        private List<ChatListItemData> _allChats = new();
        private int _selectedId = -1;
        private string _filter = "all";
        private string _searchQuery = string.Empty;

        // ── Fonts ────────────────────────────────────────────
        private Font _fontTitle = null!;
        private Font _fontName = null!;
        private Font _fontMsg = null!;
        private Font _fontTime = null!;
        private Font _fontPill = null!;
        private Font _fontSearch = null!;

        // ── Events ───────────────────────────────────────────
        public event EventHandler<int>? ChatSelected;
        public event EventHandler? NewChatClicked;
        public event EventHandler<string>? FilterChanged;
        public event EventHandler<string>? SearchChanged;

        // ─────────────────────────────────────────────────────
        //  FIX ①: اكتشاف Designer يدعم .NET 8 OOP
        //  المشكلة: في .NET 8 يعمل Designer في process منفصل
        //  اسمه "DesignToolsServer" بوضع Runtime وليس Designtime،
        //  لذا LicenseManager.UsageMode يُرجع Runtime فيفشل الـ guard.
        // ─────────────────────────────────────────────────────
        private static readonly bool _isAnyDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            System.Diagnostics.Process.GetCurrentProcess().ProcessName
                .IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;

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

            // ▶ DESIGNER GUARD — يغطي كلا الحالتين:
            //   • .NET Framework Designer  (LicenseManager)
            //   • .NET 8 OOP Designer      (DesignToolsServer process)
            if (_isAnyDesignMode)
            {
                Text = "ChatSidebar";
                return;
            }

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
                {
                    ci.Invalidate(); // ✅ يعيد رسم العنصر فقط بدون إعادة بناء القائمة
                    break;
                }
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
        //  Designer surface preview
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
            _fontName = new Font(face, 11f, FontStyle.Bold, GraphicsUnit.Point);
            _fontMsg = new Font(face, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontTime = new Font(face, 8f, FontStyle.Regular, GraphicsUnit.Point);
            _fontPill = new Font(face, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontSearch = new Font(face, 10f, FontStyle.Regular, GraphicsUnit.Point);
        }

        internal static bool IsFontInstalled(string name)
        {
            using var fc = new InstalledFontCollection();
            return fc.Families.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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

            var lblTitle = new Label
            {
                Text = "الدردشات",
                Font = _fontTitle,
                ForeColor = C_TITLE,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 16, 0),
            };

            _btnNewChat = new Button
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = C_ACCENT,
                Cursor = Cursors.Hand,
                TabStop = false,
                Size = new Size(BTN_SZ, BTN_SZ),
            };
            _btnNewChat.FlatAppearance.BorderSize = 0;
            _btnNewChat.Paint += PaintNewChatButton;
            _btnNewChat.Click += (s, e) => NewChatClicked?.Invoke(this, EventArgs.Empty);
            _btnNewChat.MouseEnter += (s, e) => { _btnNewChat.BackColor = Color.FromArgb(110, 97, 230); };
            _btnNewChat.MouseLeave += (s, e) => { _btnNewChat.BackColor = C_ACCENT; };

            var pnlBtnWrap = new Panel
            {
                Width = BTN_SZ + 20,
                Dock = DockStyle.Left,
                BackColor = C_BG,
                Padding = new Padding(12, 12, 8, 12),
            };
            _btnNewChat.Dock = DockStyle.Fill;
            pnlBtnWrap.Controls.Add(_btnNewChat);

            _pnlHeader.Controls.Add(lblTitle);
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
                Padding = new Padding(12, 8, 12, 8),
            };

            _flpFilters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoScroll = false,
                // ▲ لا نلمس HorizontalScroll/VerticalScroll هنا مطلقاً
            };

            _pillAll = new PillButton("الكل", "all", _fontPill, true);
            _pillUnread = new PillButton("غير مقروءة", "unread", _fontPill, false);
            _pillGroups = new PillButton("المجموعات", "groups", _fontPill, false);

            foreach (var pill in new[] { _pillAll, _pillUnread, _pillGroups })
            {
                pill.PillClicked += OnPillClicked;
                _flpFilters.Controls.Add(pill);
            }
            _pnlFilters.Controls.Add(_flpFilters);

            // ── Chat List ────────────────────────────────────
            _pnlList = new Panel { Dock = DockStyle.Fill, BackColor = C_BG };

            _flpList = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                // FIX ②: لا تضع AutoScroll = true هنا
                // سبب المشكلة: FlowLayoutPanel.set_AutoScroll يستدعي ScrollWindow
                // داخلياً قبل إنشاء الـ Handle → NullReferenceException في OOP Designer.
                // الحل: نؤجل الضبط لحين HandleCreated.
            };

            // FIX ②: ضبط AutoScroll بعد إنشاء الـ Handle بأمان
            _flpList.HandleCreated += (s, e) =>
            {
                if (s is FlowLayoutPanel flp)
                    flp.AutoScroll = true;
            };

            _pnlList.Controls.Add(_flpList);

            // ── Assemble (reverse order لأن DockStyle.Top يتراص من الأسفل) ──
            Controls.Add(_pnlList);
            Controls.Add(_pnlFilters);
            Controls.Add(_pnlSearch);
            Controls.Add(_pnlHeader);

            ResumeLayout(true);
        }

        // ─────────────────────────────────────────────────────
        //  Paint Helpers — all size-guarded
        // ─────────────────────────────────────────────────────
        private static void PaintHeaderBorder(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            using var pen = new Pen(C_BORDER, 1f);
            e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
        }

        private static void PaintNewChatButton(object? sender, PaintEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Width < 4 || btn.Height < 4) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rc = new Rectangle(0, 0, btn.Width, btn.Height);

            using (var path = RoundRect(rc, btn.Width / 2))
            using (var br = new SolidBrush(btn.BackColor))
                g.FillPath(br, path);

            int innerR = btn.Width / 2 - 2;
            if (innerR > 1)
            {
                var innerRc = new Rectangle(2, 2, btn.Width - 4, btn.Height - 4);
                using var ip = RoundRect(innerRc, innerR);
                using var hb = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, btn.Height / 2),
                    Color.FromArgb(50, 255, 255, 255), Color.Transparent);
                g.FillPath(hb, ip);
            }

            DrawPencilIcon(g, btn.Width, btn.Height);
        }

        private static void DrawPencilIcon(Graphics g, int w, int h)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float cx = w / 2f, cy = h / 2f, s = w * 0.26f;
            double a = -45 * Math.PI / 180;
            float cos = (float)Math.Cos(a), sin = (float)Math.Sin(a);

            using var pen = new Pen(Color.White, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var br = new SolidBrush(Color.White);

            PointF tip = new(cx + cos * s * 1.2f, cy + sin * s * 1.2f);
            PointF tl = new(cx - cos * s * 0.7f - sin * s * 0.35f, cy - sin * s * 0.7f + cos * s * 0.35f);
            PointF tr = new(cx - cos * s * 0.7f + sin * s * 0.35f, cy - sin * s * 0.7f - cos * s * 0.35f);

            g.FillPolygon(br, new[] { tip, tl, tr });
            g.DrawLine(pen, tl, new PointF(cx - cos * s - sin * s * 0.35f, cy - sin * s + cos * s * 0.35f));
            g.DrawLine(pen, tr, new PointF(cx - cos * s + sin * s * 0.35f, cy - sin * s - cos * s * 0.35f));

            using var pe2 = new Pen(Color.FromArgb(190, 255, 255, 255), 2.5f)
            { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pe2,
                new PointF(cx - cos * s * 0.85f - sin * s * 0.35f, cy - sin * s * 0.85f + cos * s * 0.35f),
                new PointF(cx - cos * s * 0.85f + sin * s * 0.35f, cy - sin * s * 0.85f - cos * s * 0.35f));

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

            // ✅ تعليم كمقروءة
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
        //  Geometry
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

                Size = new Size((int)sz.Width + 32, 32);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 2 || Height < 2) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var rc = new Rectangle(0, 0, Width - 1, Height - 1);
                using var path = RoundRect(rc, Height / 2);

                Color bg = _active ? C_ACCENT
                         : _hovered ? Color.FromArgb(228, 225, 255)
                         : C_PILL_OFF;

                using (var br = new SolidBrush(bg)) g.FillPath(br, path);

                if (_active)
                {
                    using var hb = new LinearGradientBrush(
                        new Point(0, 0), new Point(0, Height / 2),
                        Color.FromArgb(40, 255, 255, 255), Color.Transparent);
                    g.FillPath(hb, path);
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
                if (_tb == null) return;   // ← guard: OnLayout قد يُستدعى قبل تهيئة _tb

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

                using var pen = new Pen(_focused ? C_ACCENT : C_BORDER, _focused ? 1.5f : 1f);
                g.DrawPath(pen, path);

                float cx = Width - 18f, cy = Height / 2f, r = 5f;
                using var mp = new Pen(C_MSG, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawEllipse(mp, cx - r, cy - r, r * 2, r * 2);
                g.DrawLine(mp, cx - r * 0.7f, cy + r * 0.7f, cx - r * 1.7f, cy + r * 1.7f);
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

            public ChatItemControl(ChatListItemData data, Font fontName, Font fontMsg, Font fontTime)
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
                        new Rectangle(Width - 3, 8, 3, Height - 16),
                        C_ACCENT, Color.FromArgb(160, C_ACCENT), 90f);
                    g.FillRectangle(sb, Width - 3, 8, 3, Height - 16);
                }

                if (!_isLast && !_selected)
                {
                    using var sp = new Pen(C_SEP, 1f);
                    g.DrawLine(sp, 70, Height - 1, Width - 16, Height - 1);
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

                // ✅ منطقتان منفصلتان
                const int LEFT_W = 52;
                int textRight = avatarRc.Left - 8;
                int textWidth = textRight - LEFT_W;

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
                            new Rectangle(LEFT_W, avatarTop + 4, textWidth, 22), nameSf);

                    using var msgSf = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };
                    using (var mb = new SolidBrush(C_MSG))
                    {
                        string displayMsg = Data.IsLastMessageSent
                            ? "أنت: " + Data.LastMessage
                            : Data.LastMessage;

                        g.DrawString(displayMsg, _fontMsg, mb,
                            new Rectangle(LEFT_W, avatarTop + 28, textWidth, 18), msgSf);
                    }
                }

                using var timeSf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };
                using (var tb = new SolidBrush(C_TIME))
                    g.DrawString(Data.Timestamp, _fontTime, tb,
                        new Rectangle(2, avatarTop + 4, LEFT_W - 4, 16), timeSf);

                if (Data.UnreadCount > 0)
                {
                    string txt = Data.UnreadCount > 99 ? "99+" : Data.UnreadCount.ToString();
                    SizeF txtsz = g.MeasureString(txt, _fontTime);
                    int bw = (int)Math.Max(txtsz.Width + 10, 20);
                    int bh = 20;
                    int bx = (LEFT_W - bw) / 2; // ✅ لا يصبح سالباً
                    int by = avatarTop + AVATAR_SZ - bh;

                    using var glowRc = RoundRect(new Rectangle(bx - 3, by - 3, bw + 6, bh + 6), 12);
                    using (var glb = new SolidBrush(Color.FromArgb(40, C_ACCENT)))
                        g.FillPath(glb, glowRc);

                    using var badgePath = RoundRect(new Rectangle(bx, by, bw, bh), 10);
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
                        new[]{ Color.FromArgb(124, 111, 247), Color.FromArgb(167,  97, 247) },
                        new[]{ Color.FromArgb( 16, 185, 129), Color.FromArgb( 45, 212, 191) },
                        new[]{ Color.FromArgb(245, 158,  11), Color.FromArgb(251, 191,  36) },
                        new[]{ Color.FromArgb(239,  68,  68), Color.FromArgb(252, 165, 165) },
                        new[]{ Color.FromArgb( 59, 130, 246), Color.FromArgb(147, 197, 253) },
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
                using var ring = new Pen(Color.FromArgb(20, 0, 0, 0), 1f);
                g.DrawEllipse(ring, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
            }

            private static bool IsFontInstalled(string name)
            {
                using var fc = new InstalledFontCollection();
                return fc.Families.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnClick(EventArgs e) { ItemClicked?.Invoke(this, Data.ContactId); base.OnClick(e); }
        }
    }
}