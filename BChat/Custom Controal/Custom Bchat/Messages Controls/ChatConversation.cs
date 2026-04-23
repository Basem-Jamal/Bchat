using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Message_Controls
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  Data model
    // ═══════════════════════════════════════════════════════════════════════════
    public class ChatMessageData
    {
        public int MessageId;
        public string Text;
        public string Timestamp;       // e.g. "10:30 ص"
        public DateTime SentAt;          // for date-group separators
        public bool IsSent;
        public Image SenderAvatar;    // received messages only
        public bool HasAttachment;
        public string AttachmentName;
        public string AttachmentSize;
        public string AttachmentType;  // "pdf","image","video","audio",…
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  ChatConversation — main UserControl
    // ═══════════════════════════════════════════════════════════════════════════
    [ToolboxItem(true)]
    [Category("BChat - Chat")]
    [Description("A complete WhatsApp-style Arabic RTL chat conversation view.")]
    [DefaultProperty("ContactName")]
    public class ChatConversation : UserControl
    {
        // ── .NET 8 designer guard ──────────────────────────────────────────────
        private static readonly bool _isAnyDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            System.Diagnostics.Process.GetCurrentProcess().ProcessName
                .IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;

        // ══════════════════════════════════════════════════════════════════════
        //  Backing fields for designer properties
        // ══════════════════════════════════════════════════════════════════════
        private Color _pageBgColor = ColorTranslator.FromHtml("#F8F7FF");
        private Color _headerBgColor = Color.White;
        private Color _composerBgColor = Color.White;
        private Color _borderColor = ColorTranslator.FromHtml("#E2E8F0");
        private Color _sentBubbleColor = ColorTranslator.FromHtml("#7C6FF7");
        private Color _recvBubbleColor = Color.White;
        private Color _sentTextColor = Color.White;
        private Color _recvTextColor = ColorTranslator.FromHtml("#0F172A");
        private Color _mutedColor = ColorTranslator.FromHtml("#94A3B8");
        private Color _onlineColor = ColorTranslator.FromHtml("#10B981");
        private Color _accentColor = ColorTranslator.FromHtml("#7C6FF7");
        private Color _datePillBgColor = ColorTranslator.FromHtml("#E2E8F0");
        private Color _datePillTextColor = ColorTranslator.FromHtml("#64748B");
        private string _contactName = "اسم المحادثة";
        private string _contactStatus = "متصل الآن";
        private bool _contactOnline = true;
        private string _fontFace = "Cairo";
        private string _placeholder = "اكتب رسالتك هنا...";
        private int _headerHeight = 64;
        private int _composerHeight = 72;

        // ══════════════════════════════════════════════════════════════════════
        //  Designer-exposed properties
        // ══════════════════════════════════════════════════════════════════════

        [Category("BChat - Appearance"), Description("Messages area background colour.")]
        public Color PageBackColor
        {
            get => _pageBgColor;
            set { _pageBgColor = value; ApplyPageBg(); }
        }

        [Category("BChat - Appearance"), Description("Header background colour.")]
        public Color HeaderBackColor
        {
            get => _headerBgColor;
            set { _headerBgColor = value; _header?.Invalidate(); }
        }

        [Category("BChat - Appearance"), Description("Composer background colour.")]
        public Color ComposerBackColor
        {
            get => _composerBgColor;
            set { _composerBgColor = value; _composer?.Invalidate(); }
        }

        [Category("BChat - Appearance"), Description("Border / separator line colour.")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Sent bubble fill colour.")]
        public Color SentBubbleColor
        {
            get => _sentBubbleColor;
            set { _sentBubbleColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Received bubble fill colour.")]
        public Color ReceivedBubbleColor
        {
            get => _recvBubbleColor;
            set { _recvBubbleColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Text colour inside sent bubbles.")]
        public Color SentTextColor
        {
            get => _sentTextColor;
            set { _sentTextColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Text colour inside received bubbles.")]
        public Color ReceivedTextColor
        {
            get => _recvTextColor;
            set { _recvTextColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Muted / secondary text colour.")]
        public Color MutedColor
        {
            get => _mutedColor;
            set { _mutedColor = value; Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Online status dot colour.")]
        public Color OnlineColor
        {
            get => _onlineColor;
            set { _onlineColor = value; _header?.Invalidate(); }
        }

        [Category("BChat - Appearance"), Description("Accent / send-button colour.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Date separator pill background.")]
        public Color DatePillBackColor
        {
            get => _datePillBgColor;
            set { _datePillBgColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Appearance"), Description("Date separator pill text colour.")]
        public Color DatePillForeColor
        {
            get => _datePillTextColor;
            set { _datePillTextColor = value; _flpMessages?.Invalidate(true); }
        }

        [Category("BChat - Contact"), Description("Contact name shown in the header.")]
        public string ContactName
        {
            get => _contactName;
            set { _contactName = value ?? ""; _header?.Invalidate(); }
        }

        [Category("BChat - Contact"), Description("Status text shown below the name.")]
        public string ContactStatus
        {
            get => _contactStatus;
            set { _contactStatus = value ?? ""; _header?.Invalidate(); }
        }

        [Category("BChat - Contact"), Description("When true the green online dot is shown.")]
        public bool ContactIsOnline
        {
            get => _contactOnline;
            set { _contactOnline = value; _header?.Invalidate(); }
        }

        [Category("BChat - Layout"), Description("Font family (default: Cairo).")]
        public string FontFamily
        {
            get => _fontFace;
            set { _fontFace = string.IsNullOrWhiteSpace(value) ? "Cairo" : value; RebuildFonts(); Invalidate(true); }
        }

        [Category("BChat - Layout"), Description("Placeholder text in the composer.")]
        public string ComposerPlaceholder
        {
            get => _placeholder;
            set { _placeholder = value ?? ""; _composer?.UpdatePlaceholder(_placeholder); }
        }

        [Category("BChat - Layout"), Description("Header panel height (px)."), DefaultValue(64)]
        public int HeaderHeight
        {
            get => _headerHeight;
            set { _headerHeight = Math.Max(48, value); if (_header != null) _header.Height = _headerHeight; }
        }

        [Category("BChat - Layout"), Description("Composer panel height (px)."), DefaultValue(72)]
        public int ComposerHeight
        {
            get => _composerHeight;
            set { _composerHeight = Math.Max(52, value); if (_composer != null) _composer.Height = _composerHeight; }
        }

        // Hide irrelevant inherited properties
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Color BackColor { get => base.BackColor; set => base.BackColor = value; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Font Font { get => base.Font; set => base.Font = value; }

        // ══════════════════════════════════════════════════════════════════════
        //  Child controls & state
        // ══════════════════════════════════════════════════════════════════════
        private HeaderPanel _header;
        private Panel _msgScroll;
        private FlowLayoutPanel _flpMessages;
        private ComposerPanel _composer;
        private readonly List<ChatMessageData> _messages = new();

        // Shared fonts (internal so inner classes can read them)
        internal Font F_Name, F_Status, F_Bubble, F_Timestamp, F_DatePill, F_AttachName, F_AttachSize;

        // ══════════════════════════════════════════════════════════════════════
        //  Events
        // ══════════════════════════════════════════════════════════════════════
        public event EventHandler<string> MessageSent;
        public event EventHandler AttachmentClicked;
        public event EventHandler EmojiClicked;
        public event EventHandler MenuClicked;
        public event EventHandler CallClicked;
        public event EventHandler SearchClicked;
        public event EventHandler<int> MessageAttachmentClicked;

        // ══════════════════════════════════════════════════════════════════════
        //  Constructor
        // ══════════════════════════════════════════════════════════════════════
        public ChatConversation()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            RightToLeft = RightToLeft.Yes;

            if (_isAnyDesignMode)
            {
                base.BackColor = _pageBgColor;
                Size = new Size(440, 600);
                return;
            }
            BuildFonts();
            BuildLayout();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Fonts
        // ══════════════════════════════════════════════════════════════════════
        private void BuildFonts()
        {
            DispFonts();
            string f = _fontFace;
            F_Name = MkFont(f, 11f, FontStyle.Bold);
            F_Status = MkFont(f, 9f, FontStyle.Regular);
            F_Bubble = MkFont(f, 10f, FontStyle.Regular);
            F_Timestamp = MkFont(f, 8f, FontStyle.Regular);
            F_DatePill = MkFont(f, 9f, FontStyle.Regular);
            F_AttachName = MkFont(f, 9.5f, FontStyle.Bold);
            F_AttachSize = MkFont(f, 8.5f, FontStyle.Regular);
        }
        private void RebuildFonts() { BuildFonts(); }
        private void DispFonts()
        {
            F_Name?.Dispose(); F_Status?.Dispose(); F_Bubble?.Dispose();
            F_Timestamp?.Dispose(); F_DatePill?.Dispose();
            F_AttachName?.Dispose(); F_AttachSize?.Dispose();
        }
        internal static Font MkFont(string fam, float sz, FontStyle style)
        {
            try { return new Font(fam, sz, style, GraphicsUnit.Point); }
            catch { return new Font("Segoe UI", sz, style, GraphicsUnit.Point); }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Layout
        // ══════════════════════════════════════════════════════════════════════
        private void BuildLayout()
        {
            base.BackColor = _pageBgColor;
            MinimumSize = new Size(300, 400);

            _header = new HeaderPanel(this) { Dock = DockStyle.Top, Height = _headerHeight };
            _header.MenuClicked += (s, e) => MenuClicked?.Invoke(this, e);
            _header.CallClicked += (s, e) => CallClicked?.Invoke(this, e);
            _header.SearchClicked += (s, e) => SearchClicked?.Invoke(this, e);

            _composer = new ComposerPanel(this) { Dock = DockStyle.Bottom, Height = _composerHeight };
            _composer.SendMessage += (s, t) => MessageSent?.Invoke(this, t);
            _composer.AttachmentClicked += (s, e) => AttachmentClicked?.Invoke(this, e);
            _composer.EmojiClicked += (s, e) => EmojiClicked?.Invoke(this, e);

            _msgScroll = new Panel { Dock = DockStyle.Fill, BackColor = _pageBgColor };

            // BUG #2: do NOT set AutoScroll in ctor
            _flpMessages = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = _pageBgColor,
                Padding = new Padding(0, 8, 0, 8),
                RightToLeft = RightToLeft.Yes,
            };
            _flpMessages.HandleCreated += (s, e) =>
            {
                if (s is FlowLayoutPanel f) f.AutoScroll = true;
            };

            _msgScroll.Controls.Add(_flpMessages);
            Controls.Add(_msgScroll);
            Controls.Add(_composer);
            Controls.Add(_header);
        }

        private void ApplyPageBg()
        {
            base.BackColor = _pageBgColor;
            if (_msgScroll != null) _msgScroll.BackColor = _pageBgColor;
            if (_flpMessages != null) _flpMessages.BackColor = _pageBgColor;
            Invalidate(true);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Public API
        // ══════════════════════════════════════════════════════════════════════
        public void SetContact(string name, string status, bool isOnline, Image avatar)
        {
            if (_isAnyDesignMode || _header == null) return;
            _contactName = name ?? "";
            _contactStatus = status ?? "";
            _contactOnline = isOnline;
            _header.SetContact(name, status, isOnline, avatar);
        }

        public void LoadMessages(List<ChatMessageData> messages)
        {
            if (_isAnyDesignMode || _flpMessages == null) return;
            _messages.Clear();
            _flpMessages.SuspendLayout();
            _flpMessages.Controls.Clear();
            foreach (var m in messages) _messages.Add(m);
            RebuildAllBubbles();
            _flpMessages.ResumeLayout(true);
            DeferredScroll();
        }

        public void AppendMessage(ChatMessageData message)
        {
            if (_isAnyDesignMode || _flpMessages == null) return;
            _messages.Add(message);
            _flpMessages.SuspendLayout();
            AddBubble(message, _messages.Count - 1);
            _flpMessages.ResumeLayout(true);
            DeferredScroll();
        }

        public void ClearMessages()
        {
            if (_isAnyDesignMode || _flpMessages == null) return;
            _messages.Clear();
            _flpMessages.Controls.Clear();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Message rendering
        // ══════════════════════════════════════════════════════════════════════
        private void RebuildAllBubbles()
        {
            _flpMessages.Controls.Clear();
            for (int i = 0; i < _messages.Count; i++)
                AddBubble(_messages[i], i);
        }

        private void AddBubble(ChatMessageData msg, int idx)
        {
            // Date separator
            bool needSep = idx == 0 || _messages[idx - 1].SentAt.Date != msg.SentAt.Date;
            if (needSep)
            {
                var sep = new DateSepCtrl(this, FmtDate(msg.SentAt))
                {
                    Width = Math.Max(10, _flpMessages.ClientSize.Width > 0
                                         ? _flpMessages.ClientSize.Width : Width)
                };
                _flpMessages.Controls.Add(sep);
            }

            bool sameAsPrev = idx > 0 && _messages[idx - 1].IsSent == msg.IsSent
                                        && _messages[idx - 1].SentAt.Date == msg.SentAt.Date;
            bool lastInGroup = idx == _messages.Count - 1
                             || _messages[idx + 1].IsSent != msg.IsSent
                             || _messages[idx + 1].SentAt.Date != msg.SentAt.Date;

            int sbw = SystemInformation.VerticalScrollBarWidth;
            int avail = Math.Max(80, (_msgScroll.Width > 0 ? _msgScroll.ClientSize.Width : Width));

            var bc = new BubbleCtrl(this, msg, lastInGroup, sameAsPrev ? 6 : 12)
            {
                Width = avail,
                MaxBubbleWidth = (int)((avail - sbw) * 65 / 100.0),
            };
            bc.AttachmentClicked += (s, e) => MessageAttachmentClicked?.Invoke(this, msg.MessageId);
            _flpMessages.Controls.Add(bc);
        }

        private static string FmtDate(DateTime dt)
        {
            var t = DateTime.Today;
            if (dt.Date == t) return "اليوم";
            if (dt.Date == t.AddDays(-1)) return "أمس";
            return dt.ToString("d MMMM", new System.Globalization.CultureInfo("ar-SA"));
        }

        private void DeferredScroll()
        {
            if (!IsHandleCreated) return;
            BeginInvoke((Action)(() =>
            {
                if (_flpMessages == null || !_flpMessages.IsHandleCreated) return;
                int sbw = SystemInformation.VerticalScrollBarWidth;
                int avail = _msgScroll.ClientSize.Width;
                _flpMessages.Width = avail;
                foreach (Control c in _flpMessages.Controls)
                {
                    c.Width = avail;
                    if (c is BubbleCtrl bc)
                        bc.MaxBubbleWidth = (int)((avail - sbw) * 65 / 100.0);
                }
                if (_flpMessages.Controls.Count > 0)
                    _flpMessages.ScrollControlIntoView(
                        _flpMessages.Controls[_flpMessages.Controls.Count - 1]);
            }));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_flpMessages == null || _msgScroll == null) return;
            _flpMessages.Width = _msgScroll.ClientSize.Width;
            DeferredScroll();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) DispFonts();
            base.Dispose(disposing);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INNER ── HeaderPanel
        // ══════════════════════════════════════════════════════════════════════
        private sealed class HeaderPanel : Panel
        {
            private readonly ChatConversation _o;
            private string _name = "اسم المحادثة";
            private string _status = "متصل الآن";
            private bool _online = true;
            private Image _avatar;
            private int _hoverIcon = -1;

            public event EventHandler MenuClicked;
            public event EventHandler CallClicked;
            public event EventHandler SearchClicked;

            public HeaderPanel(ChatConversation owner)
            {
                _o = owner;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
                BackColor = Color.White;
                RightToLeft = RightToLeft.No; // We paint everything manually, no RTL flip needed
            }

            public void SetContact(string name, string status, bool online, Image avatar)
            {
                _name = name ?? ""; _status = status ?? ""; _online = online; _avatar = avatar;
                Invalidate();
            }

            // Icons on the physical LEFT (x=16). Avatar on the physical RIGHT.
            private Rectangle[] IconRects()
            {
                int cy = Height / 2, x = 16, step = 40;
                return new[]
                {
                    new Rectangle(x,          cy - 12, 24, 24),
                    new Rectangle(x + step,   cy - 12, 24, 24),
                    new Rectangle(x + step*2, cy - 12, 24, 24),
                };
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                var rects = IconRects();
                int prev = _hoverIcon; _hoverIcon = -1;
                for (int i = 0; i < rects.Length; i++)
                {
                    var h = rects[i]; h.Inflate(8, 8);
                    if (h.Contains(e.Location)) { _hoverIcon = i; break; }
                }
                if (_hoverIcon != prev) Invalidate();
                Cursor = _hoverIcon >= 0 ? Cursors.Hand : Cursors.Default;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (_hoverIcon >= 0) { _hoverIcon = -1; Invalidate(); }
                Cursor = Cursors.Default;
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                var rects = IconRects();
                for (int i = 0; i < rects.Length; i++)
                {
                    var h = rects[i]; h.Inflate(8, 8);
                    if (!h.Contains(e.Location)) continue;
                    if (i == 0) MenuClicked?.Invoke(this, EventArgs.Empty);
                    else if (i == 1) CallClicked?.Invoke(this, EventArgs.Empty);
                    else SearchClicked?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(_o._headerBgColor);

                // Bottom border
                using (var bp = new Pen(_o._borderColor, 1f))
                    g.DrawLine(bp, 0, Height - 1, Width, Height - 1);

                int cy = Height / 2;

                // ── LEFT: 3 action icons ───────────────────────────────────────
                var iconRects = IconRects();
                for (int i = 0; i < iconRects.Length; i++)
                {
                    if (_hoverIcon == i)
                    {
                        var hr = iconRects[i]; hr.Inflate(8, 8);
                        using var hb = new SolidBrush(Color.FromArgb(18, _o._accentColor));
                        g.FillEllipse(hb, hr);
                    }
                    PaintIcon(g, i, iconRects[i]);
                }

                // ── RIGHT: circular avatar ─────────────────────────────────────
                const int AV = 40;
                int avX = Width - 16 - AV;
                int avY = cy - AV / 2;
                var avRect = new Rectangle(avX, avY, AV, AV);

                if (_avatar != null)
                {
                    using var cp = new GraphicsPath(); cp.AddEllipse(avRect);
                    var oc = g.Clip;
                    g.SetClip(cp, CombineMode.Intersect);
                    g.DrawImage(_avatar, avRect);
                    g.Clip = oc;
                }
                else
                {
                    using var lg = new LinearGradientBrush(avRect,
                        _o._accentColor, Color.FromArgb(180, _o._accentColor),
                        LinearGradientMode.ForwardDiagonal);
                    g.FillEllipse(lg, avRect);
                    string init = _name.Length > 0 ? _name[0].ToString() : "؟";
                    using var iFont = MkFont(_o._fontFace, 14f, FontStyle.Bold);
                    var isf = new StringFormat
                    { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(init, iFont, Brushes.White, avRect, isf);
                }
                using (var rp = new Pen(Color.White, 2f)) g.DrawEllipse(rp, avRect);

                // Online dot on avatar (bottom-left of avatar circle)
                if (_online)
                {
                    var dr = new Rectangle(avRect.Left + 1, avRect.Bottom - 11, 10, 10);
                    using var db = new SolidBrush(_o._onlineColor);
                    using var drp = new Pen(Color.White, 2f);
                    g.FillEllipse(db, dr); g.DrawEllipse(drp, dr);
                }

                // ── TEXT BLOCK: between last icon and avatar ────────────────────
                // Left edge  = right edge of 3rd icon + 8px gap
                // Right edge = left edge of avatar   - 8px gap
                int textLeft = iconRects[2].Right + 8;
                int textRight = avX - 8;
                int textW = textRight - textLeft;
                if (textW < 10) return;

                const int nameH = 18, statH = 16, gap = 2;
                int blockH = nameH + gap + statH;
                int nameY = cy - blockH / 2;
                int statY = nameY + nameH + gap;

                // Name — right-aligned (Arabic reads right-to-left, so Far = correct)
                var nameRect = new RectangleF(textLeft, nameY, textW, nameH);
                using var nameBr = new SolidBrush(_o._recvTextColor);
                var sf = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                };
                if (_o.F_Name != null)
                    g.DrawString(_name, _o.F_Name, nameBr, nameRect, sf);

                // Status — also right-aligned, but leave 14px on the right for the online dot
                // The dot will be drawn just to the right of the text
                var statRect = new RectangleF(textLeft, statY, textW - 14, statH);
                using var statBr = new SolidBrush(_o._mutedColor);
                if (_o.F_Status != null)
                    g.DrawString(_status, _o.F_Status, statBr, statRect, sf);

                // Online dot next to status (to the right of text = near the avatar)
                if (_online && _o.F_Status != null)
                {
                    float dotCy = statY + statH / 2f;
                    // Measure text so the dot sits just after the status string ends (right side)
                    float dotX = textRight - 10;
                    using var ob = new SolidBrush(_o._onlineColor);
                    g.FillEllipse(ob, dotX, dotCy - 4f, 8, 8);
                }
            }

            private void PaintIcon(Graphics g, int idx, Rectangle r)
            {
                int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
                using var pen = new Pen(_o._mutedColor, 1.8f)
                { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };

                switch (idx)
                {
                    case 0: // Three vertical dots
                        using (var br = new SolidBrush(_o._mutedColor))
                        {
                            g.FillEllipse(br, cx - 2, cy - 9, 4, 4);
                            g.FillEllipse(br, cx - 2, cy - 2, 4, 4);
                            g.FillEllipse(br, cx - 2, cy + 5, 4, 4);
                        }
                        break;
                    case 1: // Phone
                        g.DrawPolygon(pen, new PointF[]
                        {
                            new(cx-6,cy-8), new(cx-3,cy-4), new(cx-5,cy-1),
                            new(cx+1,cy+5), new(cx+4,cy+3), new(cx+7,cy+6),
                            new(cx+4,cy+9), new(cx-3,cy+4), new(cx-8,cy-2), new(cx-8,cy-5),
                        });
                        break;
                    default: // Magnifier
                        g.DrawEllipse(pen, cx - 7, cy - 8, 12, 12);
                        g.DrawLine(pen, cx + 3, cy + 2, cx + 8, cy + 8);
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INNER ── DateSepCtrl
        // ══════════════════════════════════════════════════════════════════════
        private sealed class DateSepCtrl : Control
        {
            private readonly ChatConversation _o;
            private readonly string _label;

            public DateSepCtrl(ChatConversation owner, string label)
            {
                _o = owner; _label = label;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
                BackColor = Color.Transparent;
                Height = 36;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (_o.F_DatePill == null) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                SizeF tsz = g.MeasureString(_label, _o.F_DatePill);
                int pw = (int)tsz.Width + 28, ph = 22;
                int px = (Width - pw) / 2, py = (Height - ph) / 2;
                int ly = Height / 2;

                using (var lp = new Pen(_o._borderColor, 1f))
                {
                    g.DrawLine(lp, 16, ly, px - 6, ly);
                    g.DrawLine(lp, px + pw + 6, ly, Width - 16, ly);
                }
                var pr = new Rectangle(px, py, pw, ph);
                using (var pb = new SolidBrush(_o._datePillBgColor))
                    g.FillRoundedRect(pb, pr, ph / 2);
                using var tb = new SolidBrush(_o._datePillTextColor);
                var sf = new StringFormat
                { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(_label, _o.F_DatePill, tb, pr, sf);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INNER ── BubbleCtrl
        // ══════════════════════════════════════════════════════════════════════
        private sealed class BubbleCtrl : Control
        {
            private readonly ChatConversation _o;
            private readonly ChatMessageData _d;
            private readonly bool _lastInGroup;
            private readonly int _topMargin;
            private Rectangle _bubble;
            private bool _attachHover;

            private int _maxW = 320;
            public int MaxBubbleWidth
            {
                get => _maxW;
                set { _maxW = Math.Max(80, value); Recalc(); Invalidate(); }
            }

            public event EventHandler AttachmentClicked;

            public BubbleCtrl(ChatConversation owner, ChatMessageData data,
                               bool lastInGroup, int topMargin)
            {
                _o = owner; _d = data; _lastInGroup = lastInGroup; _topMargin = topMargin;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.ResizeRedraw, true);
                BackColor = Color.Transparent;
                RightToLeft = RightToLeft.No; // We paint manually
            }

            private Rectangle AttachTile() =>
                new Rectangle(_bubble.X + 6, _bubble.Y + 8, _bubble.Width - 12, 50);

            internal void Recalc()
            {
                if (_o.F_Bubble == null) return;
                const int padH = 12, padV = 10, tsH = 16;
                const int avSlot = 36; // space reserved for received avatar

                using var g = Graphics.FromHwnd(IntPtr.Zero);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var textSz = g.MeasureString(_d.Text ?? " ", _o.F_Bubble,
                                             new SizeF(_maxW - padH * 2, 4000));
                var tsSz = g.MeasureString(_d.Timestamp ?? "", _o.F_Timestamp ?? _o.F_Bubble);

                int bw = (int)Math.Min(
                    Math.Max(textSz.Width, tsSz.Width + 24) + padH * 2 + 4,
                    _maxW);
                int bh = padV * 2 + (int)Math.Ceiling(textSz.Height) + tsH;
                if (_d.HasAttachment) bh += 56;

                Height = _topMargin + bh;
                int rowW = Math.Max(10, Width);

                if (_d.IsSent)
                {
                    // right-aligned, 16px from right edge
                    _bubble = new Rectangle(rowW - bw - 16, _topMargin, bw, bh);
                }
                else
                {
                    // left-aligned with avatar slot
                    _bubble = new Rectangle(16 + avSlot, _topMargin, bw, bh);
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e); Recalc(); Invalidate();
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (!_d.HasAttachment) return;
                bool was = _attachHover;
                _attachHover = AttachTile().Contains(e.Location);
                if (was != _attachHover) Invalidate();
                Cursor = _attachHover ? Cursors.Hand : Cursors.Default;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (_attachHover) { _attachHover = false; Invalidate(); }
                Cursor = Cursors.Default;
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                if (_d.HasAttachment && AttachTile().Contains(e.Location))
                    AttachmentClicked?.Invoke(this, EventArgs.Empty);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (_o.F_Bubble == null) return;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                if (_bubble.Width < 10) Recalc();

                bool sent = _d.IsSent;
                var br = _bubble;

                // Shadow
                for (int i = 2; i >= 1; i--)
                {
                    var sr = new Rectangle(br.X - i, br.Y + i, br.Width + i * 2, br.Height + i);
                    using var sp = new SolidBrush(Color.FromArgb(9 / i, 0, 0, 0));
                    using var sh = BubblePath(sr, 16 + i, 4, sent);
                    g.FillPath(sp, sh);
                }

                // Bubble fill + border
                using var path = BubblePath(br, 16, 4, sent);
                using (var bgb = new SolidBrush(sent ? _o._sentBubbleColor : _o._recvBubbleColor))
                    g.FillPath(bgb, path);
                if (!sent)
                    using (var bp = new Pen(_o._borderColor, 1f))
                        g.DrawPath(bp, path);

                // Avatar for received (last in group)
                if (!sent && _lastInGroup)
                {
                    const int avSz = 26;
                    var avR = new Rectangle(br.X - avSz - 6, br.Bottom - avSz, avSz, avSz);
                    if (_d.SenderAvatar != null)
                    {
                        using var cp = new GraphicsPath(); cp.AddEllipse(avR);
                        var oc = g.Clip; g.SetClip(cp, CombineMode.Intersect);
                        g.DrawImage(_d.SenderAvatar, avR); g.Clip = oc;
                    }
                    else
                    {
                        using var ab = new SolidBrush(Color.FromArgb(180, _o._accentColor));
                        g.FillEllipse(ab, avR);
                    }
                }

                const int padH = 12, padV = 10, tsH = 16;
                int curY = br.Y + padV;

                // Attachment tile
                if (_d.HasAttachment)
                {
                    PaintAttach(g, AttachTile(), sent);
                    curY = AttachTile().Bottom + 6;
                }

                // Message text (right-aligned, RTL)
                using var tb = new SolidBrush(sent ? _o._sentTextColor : _o._recvTextColor);
                var txtR = new RectangleF(br.X + padH, curY,
                                          br.Width - padH * 2, br.Bottom - curY - tsH - 2);
                var tsf = new StringFormat(StringFormatFlags.NoClip)
                {
                    Alignment = StringAlignment.Far,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft,
                };
                g.DrawString(_d.Text ?? "", _o.F_Bubble, tb, txtR, tsf);

                // Timestamp
                var tsRect = new RectangleF(br.X + padH, br.Bottom - tsH - 2,
                                             br.Width - padH * 2, tsH);
                using var tsb = new SolidBrush(sent
                    ? Color.FromArgb(160, Color.White) : _o._mutedColor);
                var tssf = new StringFormat
                { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                if (_o.F_Timestamp != null)
                    g.DrawString(_d.Timestamp ?? "", _o.F_Timestamp, tsb, tsRect, tssf);

                // Double checkmarks (sent)
                if (sent)
                {
                    float cx = br.Right - padH - 22;
                    float cy = tsRect.Y + tsRect.Height / 2f;
                    using var cp = new Pen(Color.FromArgb(180, Color.White), 1.5f)
                    { StartCap = LineCap.Round, EndCap = LineCap.Round };
                    g.DrawLine(cp, cx, cy, cx + 4, cy + 4); g.DrawLine(cp, cx + 4, cy + 4, cx + 9, cy - 2);
                    g.DrawLine(cp, cx + 3, cy, cx + 7, cy + 4); g.DrawLine(cp, cx + 7, cy + 4, cx + 12, cy - 2);
                }
            }

            private void PaintAttach(Graphics g, Rectangle r, bool sent)
            {
                Color bg = sent ? Color.FromArgb(35, Color.White)
                                : ColorTranslator.FromHtml("#F1F5F9");
                using (var b = new SolidBrush(bg)) g.FillRoundedRect(b, r, 10);
                if (_attachHover)
                    using (var b = new SolidBrush(Color.FromArgb(20, Color.White))) g.FillRoundedRect(b, r, 10);

                // File icon
                const int isz = 30;
                var ir = new Rectangle(r.X + 10, r.Y + (r.Height - isz) / 2, isz, isz);
                PaintFileIcon(g, ir, _d.AttachmentType ?? "file");

                int tx = ir.Right + 8, tw = r.Right - ir.Right - 40;
                Color nc = sent ? Color.White : _o._recvTextColor;
                Color sc = sent ? Color.FromArgb(180, Color.White) : _o._mutedColor;
                var esf = new StringFormat(StringFormatFlags.NoWrap) { Trimming = StringTrimming.EllipsisCharacter };
                using (var nb = new SolidBrush(nc))
                    if (_o.F_AttachName != null)
                        g.DrawString(_d.AttachmentName ?? "ملف", _o.F_AttachName, nb,
                                     new RectangleF(tx, r.Y + 8, tw, 17), esf);
                using (var sb = new SolidBrush(sc))
                    if (_o.F_AttachSize != null)
                        g.DrawString(_d.AttachmentSize ?? "", _o.F_AttachSize, sb,
                                     new RectangleF(tx, r.Y + 26, tw, 14), esf);

                // Download arrow
                float ax = r.Right - 18, ay = r.Y + r.Height / 2f;
                Color ac = sent ? Color.FromArgb(200, Color.White) : _o._accentColor;
                using var ap = new Pen(ac, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(ap, ax, ay - 7, ax, ay + 3);
                g.DrawLine(ap, ax - 4, ay, ax, ay + 4); g.DrawLine(ap, ax + 4, ay, ax, ay + 4);
                g.DrawLine(ap, ax - 5, ay + 6, ax + 5, ay + 6);
            }

            private static void PaintFileIcon(Graphics g, Rectangle r, string type)
            {
                Color c = type.ToLowerInvariant() switch
                {
                    "pdf" => Color.FromArgb(239, 68, 68),
                    "image" => Color.FromArgb(16, 185, 129),
                    "video" => Color.FromArgb(234, 179, 8),
                    "audio" => Color.FromArgb(168, 85, 247),
                    _ => Color.FromArgb(100, 116, 139),
                };
                int fold = 8;
                using var pp = new GraphicsPath();
                pp.AddLines(new PointF[]
                {
                    new(r.X, r.Y+fold), new(r.X+fold, r.Y), new(r.Right, r.Y),
                    new(r.Right, r.Bottom), new(r.X, r.Bottom),
                });
                pp.CloseFigure();
                using (var pb = new SolidBrush(Color.FromArgb(210, c))) g.FillPath(pb, pp);
                using (var fb = new SolidBrush(Color.FromArgb(120, Color.White)))
                    g.FillPolygon(fb, new PointF[]
                        { new(r.X, r.Y+fold), new(r.X+fold, r.Y), new(r.X+fold, r.Y+fold) });
                string lbl = (type.Length > 3 ? type[..3] : type).ToUpper();
                using var lf = MkFont("Cairo", 6f, FontStyle.Bold);
                var lsf = new StringFormat
                { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(lbl, lf, Brushes.White, r, lsf);
            }

            private static GraphicsPath BubblePath(Rectangle r, int radius, int tailR, bool sent)
            {
                radius = Math.Max(1, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
                tailR = Math.Max(1, Math.Min(tailR, Math.Min(r.Width, r.Height) / 2));
                int d = radius * 2, td = tailR * 2;
                var p = new GraphicsPath();
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                if (sent)
                {
                    p.AddArc(r.Right - td, r.Bottom - td, td, td, 0, 90); // BR tail
                    p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                }
                else
                {
                    p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                    p.AddArc(r.X, r.Bottom - td, td, td, 90, 90); // BL tail
                }
                p.CloseFigure();
                return p;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INNER ── ComposerPanel
        // ══════════════════════════════════════════════════════════════════════
        private sealed class ComposerPanel : Panel
        {
            private readonly ChatConversation _o;
            private TextBox _tb;
            private bool _sendHov, _emojiHov, _attachHov;

            public event EventHandler<string> SendMessage;
            public event EventHandler AttachmentClicked;
            public event EventHandler EmojiClicked;

            public ComposerPanel(ChatConversation owner)
            {
                _o = owner;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
                BackColor = Color.White;
                RightToLeft = RightToLeft.No; // manual painting
            }

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                _tb = new TextBox
                {
                    BorderStyle = BorderStyle.None,
                    BackColor = ColorTranslator.FromHtml("#F8FAFC"),
                    ForeColor = _o._recvTextColor,
                    Font = _o.F_Bubble ?? new Font("Segoe UI", 10f),
                    RightToLeft = RightToLeft.Yes,
                    PlaceholderText = _o._placeholder,
                };
                _tb.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) { ev.SuppressKeyPress = true; TrySend(); } };
                _tb.TextChanged += (s, ev) => Invalidate();
                Controls.Add(_tb);
            }

            internal void UpdatePlaceholder(string text)
            {
                if (_tb != null) _tb.PlaceholderText = text ?? "";
            }

            private void TrySend()
            {
                if (_tb == null) return;
                string t = _tb.Text.Trim();
                if (string.IsNullOrEmpty(t)) return;
                _tb.Text = "";
                SendMessage?.Invoke(this, t);
            }

            // ── Geometry ───────────────────────────────────────────────────────
            // Layout (physical left → right):
            //   [12px] [SendBtn 48×48] [8px] [InputBox fills rest] [12px]
            // Inside InputBox (physical left → right):
            //   [12px] [TextBox (fills)] [AttachIcon 28px] [4px] [EmojiIcon 28px] [8px]

            private Rectangle SendR()
            {
                int s = 48, y = (Height - s) / 2;
                return new Rectangle(12, y, s, s);
            }

            private Rectangle InputR()
            {
                var sr = SendR();
                return new Rectangle(sr.Right + 8, sr.Y, Width - sr.Right - 8 - 12, sr.Height);
            }

            // Emoji icon: rightmost icon in the input box
            private Rectangle EmojiR()
            {
                var ir = InputR();
                const int isz = 26;
                int iy = ir.Y + (ir.Height - isz) / 2;
                return new Rectangle(ir.Right - isz - 8, iy, isz, isz);
            }

            // Attach icon: just to the left of emoji
            private Rectangle AttachR()
            {
                var er = EmojiR();
                return new Rectangle(er.X - er.Width - 4, er.Y, er.Width, er.Height);
            }

            // BUG #3: guard _tb
            protected override void OnLayout(LayoutEventArgs e)
            {
                base.OnLayout(e);
                if (_tb == null) return;

                var ir = InputR();
                var attR = AttachR();

                // TextBox occupies left part of InputBox, up to (but not including) the icons
                int tbX = ir.X + 12;
                int tbW = Math.Max(10, attR.X - tbX - 4);
                int tbH = _tb.PreferredHeight;
                int tbY = ir.Y + Math.Max(0, (ir.Height - tbH) / 2);
                _tb.SetBounds(tbX, tbY, tbW, tbH);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                bool sh = SendR().Contains(e.Location);
                bool eh = EmojiR().Contains(e.Location);
                bool ah = AttachR().Contains(e.Location);
                if (sh != _sendHov || eh != _emojiHov || ah != _attachHov)
                {
                    _sendHov = sh; _emojiHov = eh; _attachHov = ah;
                    Invalidate();
                }
                Cursor = (sh || eh || ah) ? Cursors.Hand : Cursors.Default;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _sendHov = _emojiHov = _attachHov = false;
                Invalidate(); Cursor = Cursors.Default;
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                if (SendR().Contains(e.Location)) TrySend();
                else if (EmojiR().Contains(e.Location)) EmojiClicked?.Invoke(this, EventArgs.Empty);
                else if (AttachR().Contains(e.Location)) AttachmentClicked?.Invoke(this, EventArgs.Empty);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(_o._composerBgColor);

                using (var bp = new Pen(_o._borderColor, 1f)) g.DrawLine(bp, 0, 0, Width, 0);

                bool hasText = _tb != null && !string.IsNullOrEmpty(_tb.Text?.Trim());

                // Send button
                var sr = SendR();
                Color sbg = hasText
                    ? (_sendHov ? Color.FromArgb(220, _o._accentColor) : _o._accentColor)
                    : ColorTranslator.FromHtml("#CBD5E1");
                using (var sb = new SolidBrush(sbg)) g.FillEllipse(sb, sr);
                DrawArrow(g, sr);

                // Input box
                var ir = InputR();
                using (var ib = new SolidBrush(ColorTranslator.FromHtml("#F8FAFC")))
                    g.FillRoundedRect(ib, ir, 24);
                using (var ip = new Pen(_o._borderColor, 1f))
                    g.DrawRoundedRect(ip, ir, 24);

                // Icons
                var er = EmojiR();
                var ar = AttachR();
                if (_emojiHov)
                    using (var hb = new SolidBrush(Color.FromArgb(22, _o._accentColor)))
                        g.FillEllipse(hb, er);
                if (_attachHov)
                    using (var hb = new SolidBrush(Color.FromArgb(22, _o._accentColor)))
                        g.FillEllipse(hb, ar);
                DrawEmoji(g, er);
                DrawClip(g, ar);
            }

            private void DrawArrow(Graphics g, Rectangle r)
            {
                int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
                using var p = new Pen(Color.White, 2.2f)
                { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
                g.DrawLine(p, cx - 7, cy, cx + 6, cy);
                g.DrawLine(p, cx + 1, cy - 5, cx + 7, cy);
                g.DrawLine(p, cx + 1, cy + 5, cx + 7, cy);
            }

            private void DrawEmoji(Graphics g, Rectangle r)
            {
                int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2, sr = 9;
                using var p = new Pen(_o._mutedColor, 1.6f);
                g.DrawEllipse(p, cx - sr, cy - sr, sr * 2, sr * 2);
                using var eb = new SolidBrush(_o._mutedColor);
                g.FillEllipse(eb, cx - 4, cy - 3, 3, 3);
                g.FillEllipse(eb, cx + 1, cy - 3, 3, 3);
                g.DrawArc(p, cx - 5, cy + 1, 10, 6, 0, 180);
            }

            private void DrawClip(Graphics g, Rectangle r)
            {
                int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
                using var p = new Pen(_o._mutedColor, 1.8f)
                { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawCurve(p, new PointF[]
                {
                    new(cx+4, cy-7), new(cx+7, cy-4), new(cx+1, cy+5),
                    new(cx-2, cy+7), new(cx-6, cy+3), new(cx,    cy-4), new(cx+3, cy-1),
                }, 0.3f);
            }
        }

    } // end ChatConversation

    // ═══════════════════════════════════════════════════════════════════════════
    //  GDI+ extension helpers
    // ═══════════════════════════════════════════════════════════════════════════
    internal static class GfxExt
    {
        public static void FillRoundedRect(this Graphics g, Brush br, Rectangle r, int rad)
        {
            using var p = RPath(r, rad); g.FillPath(br, p);
        }
        public static void DrawRoundedRect(this Graphics g, Pen pen, Rectangle r, int rad)
        {
            using var p = RPath(r, rad); g.DrawPath(pen, p);
        }
        private static GraphicsPath RPath(Rectangle r, int rad)
        {
            rad = Math.Max(1, Math.Min(rad, Math.Min(r.Width, r.Height) / 2));
            int d = rad * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

} // end namespace