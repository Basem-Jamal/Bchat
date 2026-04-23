using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Message_Controls
{
    // ──────────────────────────────────────────────────────────────────────────
    //  ChatContactInfo  –  BChat side-panel contact detail control
    //  .NET 8 / Windows Forms  |  Cairo font  |  RTL  |  GDI+ anti-aliased
    // ──────────────────────────────────────────────────────────────────────────
    [ToolboxItem(true)]
    [Category("BChat - Chat")]
    [Description("Shows complete contact information for the active chat (avatar, tags, media, block button).")]
    [DefaultEvent("BlockClicked")]
    public class ChatContactInfo : UserControl
    {
        // ── .NET 8 design-mode guard ────────────────────────────────────────
        private static readonly bool _isAnyDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            System.Diagnostics.Process.GetCurrentProcess().ProcessName
                .IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;

        // ────────────────────────────────────────────────────────────────────
        //  Colour & metric constants
        // ────────────────────────────────────────────────────────────────────
        private static readonly Color C_BG = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private static readonly Color C_DIVIDER = Color.FromArgb(0xF1, 0xF5, 0xF9);
        private static readonly Color C_TITLE = Color.FromArgb(0x0F, 0x17, 0x2A);
        private static readonly Color C_SUBTITLE = Color.FromArgb(0x64, 0x74, 0x8B);
        private static readonly Color C_LABEL = Color.FromArgb(0x94, 0xA3, 0xB8);
        private static readonly Color C_ICON = Color.FromArgb(0x94, 0xA3, 0xB8);
        private static readonly Color C_INFO_TEXT = Color.FromArgb(0x0F, 0x17, 0x2A);
        private static readonly Color C_MEDIA_BG = Color.FromArgb(0xF1, 0xF5, 0xF9);
        private static readonly Color C_BLOCK_BG = Color.FromArgb(0xFE, 0xE2, 0xE2);
        private static readonly Color C_BLOCK_BG_HOV = Color.FromArgb(0xFE, 0xCA, 0xCA);
        private static readonly Color C_BLOCK_TEXT = Color.FromArgb(0xDC, 0x26, 0x26);
        private static readonly Color C_ACCENT = Color.FromArgb(0x7C, 0x6F, 0xF7);
        private static readonly Color C_AVATAR_RING = Color.FromArgb(0xE0, 0xDD, 0xFD);

        // Tag palette (BG, FG) cycled by index
        private static readonly (Color bg, Color fg)[] TAG_PALETTE =
        {
            (Color.FromArgb(0xFE, 0xF3, 0xC7), Color.FromArgb(0x92, 0x40, 0x0E)),
            (Color.FromArgb(0xDB, 0xEA, 0xFE), Color.FromArgb(0x1E, 0x40, 0xAF)),
            (Color.FromArgb(0xD1, 0xFA, 0xE5), Color.FromArgb(0x06, 0x5F, 0x46)),
            (Color.FromArgb(0xFC, 0xE7, 0xF3), Color.FromArgb(0x9D, 0x17, 0x4D)),
            (Color.FromArgb(0xED, 0xE9, 0xFE), Color.FromArgb(0x5B, 0x21, 0xB6)),
        };

        private const int AVATAR_SIZE = 96;
        private const int SECTION_PAD_H = 20;   // horizontal padding inside sections
        private const int MEDIA_TILE_SIZE = 48;
        private const int MEDIA_RADIUS = 8;
        private const int BLOCK_HEIGHT = 44;
        private const int BLOCK_RADIUS = 10;
        private const int TAG_RADIUS = 16;

        // ────────────────────────────────────────────────────────────────────
        //  Fonts  (Cairo; GDI fallback if not installed)
        // ────────────────────────────────────────────────────────────────────
        private Font _fntName = null!;   // Cairo 13pt Bold
        private Font _fntRole = null!;   // Cairo  9pt Regular
        private Font _fntLabel = null!;   // Cairo  8pt Bold
        private Font _fntInfo = null!;   // Cairo 10pt Regular
        private Font _fntTag = null!;   // Cairo  8pt Regular
        private Font _fntBlock = null!;   // Cairo 10pt Bold
        private Font _fntCounter = null!;   // Cairo  9pt Bold

        // ────────────────────────────────────────────────────────────────────
        //  Backing fields
        // ────────────────────────────────────────────────────────────────────
        private string _contactName = "اسم المستخدم";
        private string _contactRole = "عميل";
        private Image? _contactAvatar = null;
        private string _contactPhone = "+966 50 000 0000";
        private string _contactEmail = "user@example.com";
        private List<string> _tags = new() { "VIP", "مميز", "نشط" };
        private List<Image> _mediaThumbs = new();
        private int _totalMedia = 0;

        // ────────────────────────────────────────────────────────────────────
        //  Layout panels
        // ────────────────────────────────────────────────────────────────────
        private FlowLayoutPanel _flpSections = null!;
        private Panel _pnlAvatar = null!;
        private Panel _pnlNameRole = null!;
        private Panel _pnlContactInfo = null!;
        private Panel _pnlTags = null!;
        private Panel _pnlMedia = null!;
        private Panel _pnlBlock = null!;

        // Block button hover state
        private bool _blockHover = false;

        // ────────────────────────────────────────────────────────────────────
        //  Events
        // ────────────────────────────────────────────────────────────────────
        public event EventHandler? BlockClicked;
        public event EventHandler<string>? TagClicked;
        public event EventHandler<int>? MediaClicked;
        public event EventHandler<string>? PhoneClicked;
        public event EventHandler<string>? EmailClicked;

        // ────────────────────────────────────────────────────────────────────
        //  Constructor
        // ────────────────────────────────────────────────────────────────────
        public ChatContactInfo()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = C_BG;
            RightToLeft = RightToLeft.Yes;
            MinimumSize = new Size(260, 400);

            if (_isAnyDesignMode)
            {
                Text = "ChatContactInfo";
                return;
            }

            BuildFonts();
            BuildLayout();
        }

        // ────────────────────────────────────────────────────────────────────
        //  Properties
        // ────────────────────────────────────────────────────────────────────
        [Category("BChat")]
        [Description("Display name of the contact.")]
        public string ContactName
        {
            get => _contactName;
            set { _contactName = value ?? ""; RefreshNameRole(); }
        }

        [Category("BChat")]
        [Description("Role or status label shown below the name.")]
        public string ContactRole
        {
            get => _contactRole;
            set { _contactRole = value ?? ""; RefreshNameRole(); }
        }

        [Category("BChat")]
        [Description("Avatar image (will be clipped to a circle).")]
        public Image? ContactAvatar
        {
            get => _contactAvatar;
            set { _contactAvatar = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat")]
        [Description("Phone number string.")]
        public string ContactPhone
        {
            get => _contactPhone;
            set { _contactPhone = value ?? ""; RefreshContactInfo(); }
        }

        [Category("BChat")]
        [Description("Email address string.")]
        public string ContactEmail
        {
            get => _contactEmail;
            set { _contactEmail = value ?? ""; RefreshContactInfo(); }
        }

        [Category("BChat")]
        [Description("List of tag strings to show as chip pills.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> Tags
        {
            get => _tags;
            set { _tags = value ?? new(); RebuildTags(); }
        }

        [Category("BChat")]
        [Description("List of media thumbnail images (max 4 rendered; rest counted).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Image> MediaThumbnails
        {
            get => _mediaThumbs;
            set { _mediaThumbs = value ?? new(); RefreshMedia(); }
        }

        [Category("BChat")]
        [Description("Total number of media items (for the +X counter tile).")]
        public int TotalMediaCount
        {
            get => _totalMedia;
            set { _totalMedia = Math.Max(0, value); RefreshMedia(); }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Public API
        // ────────────────────────────────────────────────────────────────────
        public void SetContact(
            string name,
            string role,
            Image? avatar,
            string phone,
            string email,
            List<string> tags,
            List<Image> mediaThumbnails,
            int totalMediaCount)
        {
            _contactName = name ?? "";
            _contactRole = role ?? "";
            _contactAvatar = avatar;
            _contactPhone = phone ?? "";
            _contactEmail = email ?? "";
            _tags = tags ?? new();
            _mediaThumbs = mediaThumbnails ?? new();
            _totalMedia = Math.Max(0, totalMediaCount);

            RefreshAll();
        }

        // ────────────────────────────────────────────────────────────────────
        //  Font building
        // ────────────────────────────────────────────────────────────────────
        private void BuildFonts()
        {
            string family = IsFontAvailable("Cairo") ? "Cairo" : "Segoe UI";

            _fntName = new Font(family, 13f, FontStyle.Bold, GraphicsUnit.Point);
            _fntRole = new Font(family, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fntLabel = new Font(family, 8f, FontStyle.Bold, GraphicsUnit.Point);
            _fntInfo = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
            _fntTag = new Font(family, 8f, FontStyle.Regular, GraphicsUnit.Point);
            _fntBlock = new Font(family, 10f, FontStyle.Bold, GraphicsUnit.Point);
            _fntCounter = new Font(family, 9f, FontStyle.Bold, GraphicsUnit.Point);
        }

        private static bool IsFontAvailable(string name)
        {
            using var fc = new System.Drawing.Text.InstalledFontCollection();
            foreach (var ff in fc.Families)
                if (string.Equals(ff.Name, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Layout building  (called once from ctor)
        // ────────────────────────────────────────────────────────────────────
        private void BuildLayout()
        {
            // Main FlowLayoutPanel  — BUG #2: defer AutoScroll
            _flpSections = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0),
                BackColor = C_BG,
                RightToLeft = RightToLeft.Yes,
            };
            _flpSections.HandleCreated += (s, _) =>
            {
                if (s is FlowLayoutPanel flp) flp.AutoScroll = true;
            };

            // Build individual section panels
            _pnlAvatar = BuildAvatarPanel();
            _pnlNameRole = BuildNameRolePanel();
            _pnlContactInfo = BuildContactInfoPanel();
            _pnlTags = BuildTagsPanel();
            _pnlMedia = BuildMediaPanel();
            _pnlBlock = BuildBlockPanel();

            _flpSections.Controls.AddRange(new Control[]
            {
                _pnlAvatar,
                _pnlNameRole,
                MakeDivider(),
                WrapSection("معلومات الاتصال", _pnlContactInfo),
                MakeDivider(),
                WrapSection("الوسوم", _pnlTags),
                MakeDivider(),
                WrapSection("الصور والملفات", _pnlMedia),
                MakeSpringPanel(),
                _pnlBlock,
            });

            Controls.Add(_flpSections);
        }

        // ── Section: Avatar ─────────────────────────────────────────────────
        private Panel BuildAvatarPanel()
        {
            var pnl = new Panel
            {
                Height = 130,
                BackColor = C_BG,
                Margin = new Padding(0, 16, 0, 0),
            };
            pnl.Paint += PaintAvatar;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();

            // Width will be set in OnLayout
            return pnl;
        }

        private void PaintAvatar(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int cx = pnl.Width / 2;
            int cy = pnl.Height / 2;
            int r = AVATAR_SIZE / 2;

            // Subtle ring
            using (var ringPen = new Pen(C_AVATAR_RING, 3f))
            {
                g.DrawEllipse(ringPen,
                    cx - r - 3, cy - r - 3,
                    (r + 3) * 2, (r + 3) * 2);
            }

            // Clip-circle for avatar image
            var clipRect = new Rectangle(cx - r, cy - r, AVATAR_SIZE, AVATAR_SIZE);
            using var gp = new GraphicsPath();
            gp.AddEllipse(clipRect);

            var prevClip = g.Clip;
            g.SetClip(gp);

            if (_contactAvatar != null)
            {
                g.DrawImage(_contactAvatar, clipRect);
            }
            else
            {
                // Placeholder gradient fill
                using var brush = new LinearGradientBrush(
                    clipRect,
                    Color.FromArgb(0xC4, 0xC0, 0xFB),
                    Color.FromArgb(0x7C, 0x6F, 0xF7),
                    LinearGradientMode.ForwardDiagonal);
                g.FillEllipse(brush, clipRect);

                // Initial letter
                if (!string.IsNullOrWhiteSpace(_contactName))
                {
                    string initial = _contactName[0].ToString();
                    using var fnt = new Font(_fntName.FontFamily, 28f, FontStyle.Bold, GraphicsUnit.Point);
                    using var brs = new SolidBrush(Color.White);
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                    };
                    g.DrawString(initial, fnt, brs, clipRect, sf);
                }
            }

            g.Clip = prevClip;
        }

        // ── Section: Name + Role ────────────────────────────────────────────
        private Panel BuildNameRolePanel()
        {
            var pnl = new Panel
            {
                Height = 56,
                BackColor = C_BG,
                Margin = new Padding(0, 4, 0, 0),
            };
            pnl.Paint += PaintNameRole;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private void PaintNameRole(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntName == null || _fntRole == null) return; // BUG #3 guard
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            };

            // Name
            using (var b = new SolidBrush(C_TITLE))
            {
                var nameRect = new RectangleF(SECTION_PAD_H, 4, pnl.Width - SECTION_PAD_H * 2, 26);
                g.DrawString(_contactName, _fntName, b, nameRect, sf);
            }

            // Role
            using (var b = new SolidBrush(C_SUBTITLE))
            {
                var roleRect = new RectangleF(SECTION_PAD_H, 30, pnl.Width - SECTION_PAD_H * 2, 22);
                g.DrawString(_contactRole, _fntRole, b, roleRect, sf);
            }
        }

        // ── Section: Contact Info ───────────────────────────────────────────
        private Panel BuildContactInfoPanel()
        {
            var pnl = new Panel
            {
                Height = 72,
                BackColor = C_BG,
            };
            pnl.Paint += PaintContactInfo;
            pnl.MouseDown += ContactInfo_MouseDown;
            pnl.MouseMove += ContactInfo_MouseMove;
            pnl.Cursor = Cursors.Default;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private Rectangle GetPhoneRowRect(Panel pnl) => new Rectangle(0, 0, pnl.Width, 36);
        private Rectangle GetEmailRowRect(Panel pnl) => new Rectangle(0, 36, pnl.Width, 36);

        private void PaintContactInfo(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntInfo == null) return; // BUG #3 guard
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawInfoRow(g, pnl.Width, 0, IconPhone(), _contactPhone, _fntInfo);
            DrawInfoRow(g, pnl.Width, 36, IconEnvelope(), _contactEmail, _fntInfo);
        }

        private void DrawInfoRow(Graphics g, int panelWidth, int y, GraphicsPath icon, string text, Font fnt)
        {
            const int iconSize = 16;
            const int iconPad = SECTION_PAD_H;
            const int rowH = 36;

            int iconX = panelWidth - iconPad - iconSize;
            int iconY = y + (rowH - iconSize) / 2;

            // Icon background circle  (subtle)
            var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
            using (var b = new SolidBrush(C_DIVIDER))
                g.FillEllipse(b, iconRect.Left - 3, iconRect.Top - 3, iconSize + 6, iconSize + 6);

            // Draw icon path (scaled to iconRect)
            var state = g.Save();
            g.TranslateTransform(iconRect.Left, iconRect.Top);
            g.ScaleTransform(iconSize / 24f, iconSize / 24f);
            using (var pen = new Pen(C_ICON, 1.8f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawPath(pen, icon);
            g.Restore(state);
            icon.Dispose();

            // Text
            int textX = iconPad;
            int textW = iconX - iconPad * 2 - 4;
            var textRect = new RectangleF(textX, y + (rowH - 18) / 2f, textW, 20);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            };
            using (var b = new SolidBrush(C_INFO_TEXT))
                g.DrawString(text, fnt, b, textRect, sf);
        }

        private void ContactInfo_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl) return;
            bool overPhone = GetPhoneRowRect(pnl).Contains(e.Location);
            bool overEmail = GetEmailRowRect(pnl).Contains(e.Location);
            pnl.Cursor = (overPhone || overEmail) ? Cursors.Hand : Cursors.Default;
        }

        private void ContactInfo_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl || e.Button != MouseButtons.Left) return;
            if (GetPhoneRowRect(pnl).Contains(e.Location))
                PhoneClicked?.Invoke(this, _contactPhone);
            else if (GetEmailRowRect(pnl).Contains(e.Location))
                EmailClicked?.Invoke(this, _contactEmail);
        }

        // ── Section: Tags ───────────────────────────────────────────────────
        private Panel BuildTagsPanel()
        {
            var pnl = new Panel
            {
                BackColor = C_BG,
                Margin = new Padding(0),
            };
            PopulateTagsPanel(pnl);
            return pnl;
        }

        private void PopulateTagsPanel(Panel pnl)
        {
            pnl.Controls.Clear();

            const int padH = SECTION_PAD_H;
            const int chipH = 26;
            const int gapX = 6;
            const int gapY = 6;

            int x = padH;
            int y = 0;
            int maxX = 0;

            // We'll do a simulated measure pass to compute the panel height
            // then a draw pass.  Use custom-painted labels.
            for (int i = 0; i < _tags.Count; i++)
            {
                string tag = _tags[i];
                int idx = i % TAG_PALETTE.Length;
                var colors = TAG_PALETTE[idx];

                // Measure text
                using var tmp = Graphics.FromHwnd(IntPtr.Zero);
                int textW = (int)tmp.MeasureString(tag, _fntTag).Width + 1;
                int chipW = textW + 20; // 10px padding each side

                // Wrap to next line
                int availW = (pnl.Width > 0 ? pnl.Width : 260) - padH * 2;
                if (x + chipW > padH + availW && x > padH)
                {
                    x = padH;
                    y += chipH + gapY;
                }

                int fi = i; // capture
                string ft = tag;
                (Color fbg, Color ffg) = colors;

                var chip = new TagChip
                {
                    Text = tag,
                    ChipBg = colors.bg,
                    ChipFg = colors.fg,
                    ChipFont = _fntTag,
                    Location = new Point(x, y),
                    Size = new Size(chipW, chipH),
                    Cursor = Cursors.Hand,
                };
                chip.Click += (s, _) => TagClicked?.Invoke(this, ft);

                pnl.Controls.Add(chip);
                x += chipW + gapX;
                maxX = Math.Max(maxX, x);
            }

            int totalH = y + chipH + 4;
            pnl.Height = Math.Max(totalH, chipH + 4);
        }

        // ── Section: Media ──────────────────────────────────────────────────
        private Panel BuildMediaPanel()
        {
            var pnl = new Panel
            {
                Height = MEDIA_TILE_SIZE + 8,
                BackColor = C_BG,
                Margin = new Padding(0),
            };
            pnl.Paint += PaintMedia;
            pnl.MouseDown += Media_MouseDown;
            pnl.Cursor = Cursors.Default;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private void PaintMedia(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntRole == null || _fntCounter == null) return; // BUG #3 guard
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int visibleCount = Math.Min(_mediaThumbs.Count, 4);
            bool hasOverflow = _totalMedia > 4;
            int tileCount = visibleCount + (hasOverflow ? 1 : 0);

            int totalW = tileCount * MEDIA_TILE_SIZE + Math.Max(0, tileCount - 1) * 6;
            int startX = (pnl.Width - totalW) / 2;
            int startY = 4;

            for (int i = 0; i < visibleCount; i++)
            {
                int x = startX + i * (MEDIA_TILE_SIZE + 6);
                var rect = new Rectangle(x, startY, MEDIA_TILE_SIZE, MEDIA_TILE_SIZE);
                DrawRoundedTile(g, rect, _mediaThumbs[i], false, 0);
            }

            if (hasOverflow)
            {
                int x = startX + visibleCount * (MEDIA_TILE_SIZE + 6);
                var rect = new Rectangle(x, startY, MEDIA_TILE_SIZE, MEDIA_TILE_SIZE);
                int extra = _totalMedia - 4;
                DrawRoundedTile(g, rect, null, true, extra);
            }

            // No media placeholder
            if (tileCount == 0)
            {
                using var b = new SolidBrush(C_LABEL);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("لا توجد وسائط", _fntRole, b,
                    new RectangleF(0, 0, pnl.Width, pnl.Height), sf);
            }
        }

        private void DrawRoundedTile(Graphics g, Rectangle rect, Image? img, bool isCounter, int count)
        {
            using var path = RoundedRect(rect, MEDIA_RADIUS);

            if (isCounter)
            {
                // Dark overlay tile
                using (var b = new SolidBrush(Color.FromArgb(0x1E, 0x29, 0x3B)))
                    g.FillPath(b, path);

                string txt = "+" + count;
                using var b2 = new SolidBrush(Color.White);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(txt, _fntCounter, b2, (RectangleF)rect, sf);
            }
            else if (img != null)
            {
                var state = g.Save();
                g.SetClip(path);
                g.DrawImage(img, rect);
                g.Restore(state);
            }
            else
            {
                using (var b = new SolidBrush(C_MEDIA_BG))
                    g.FillPath(b, path);
            }
        }

        private int GetMediaTileIndex(Panel pnl, Point pt)
        {
            int visibleCount = Math.Min(_mediaThumbs.Count, 4);
            bool hasOverflow = _totalMedia > 4;
            int tileCount = visibleCount + (hasOverflow ? 1 : 0);
            if (tileCount == 0) return -1;

            int totalW = tileCount * MEDIA_TILE_SIZE + Math.Max(0, tileCount - 1) * 6;
            int startX = (pnl.Width - totalW) / 2;
            int startY = 4;

            for (int i = 0; i < tileCount; i++)
            {
                int x = startX + i * (MEDIA_TILE_SIZE + 6);
                var rect = new Rectangle(x, startY, MEDIA_TILE_SIZE, MEDIA_TILE_SIZE);
                if (rect.Contains(pt)) return i;
            }
            return -1;
        }

        private void Media_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl || e.Button != MouseButtons.Left) return;
            int idx = GetMediaTileIndex(pnl, e.Location);
            if (idx >= 0) MediaClicked?.Invoke(this, idx);
        }

        // ── Block Button ────────────────────────────────────────────────────
        private Panel BuildBlockPanel()
        {
            var pnl = new Panel
            {
                Height = BLOCK_HEIGHT + 24,
                BackColor = C_BG,
                Margin = new Padding(0),
            };
            pnl.Paint += PaintBlockButton;
            pnl.MouseEnter += (s, _) => { _blockHover = true; (s as Control)?.Invalidate(); };
            pnl.MouseLeave += (s, _) => { _blockHover = false; (s as Control)?.Invalidate(); };
            pnl.MouseDown += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left
                    && GetBlockButtonRect(s as Panel).Contains(me.Location))
                    BlockClicked?.Invoke(this, EventArgs.Empty);
            };
            pnl.Cursor = Cursors.Hand;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private Rectangle GetBlockButtonRect(Panel? pnl)
        {
            if (pnl == null) return Rectangle.Empty;
            int padH = SECTION_PAD_H;
            int y = (pnl.Height - BLOCK_HEIGHT) / 2;
            return new Rectangle(padH, y, pnl.Width - padH * 2, BLOCK_HEIGHT);
        }

        private void PaintBlockButton(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntBlock == null) return; // BUG #3 guard
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = GetBlockButtonRect(pnl);
            Color bgColor = _blockHover ? C_BLOCK_BG_HOV : C_BLOCK_BG;

            using (var path = RoundedRect(rect, BLOCK_RADIUS))
            using (var b = new SolidBrush(bgColor))
                g.FillPath(b, path);

            // Icon (shield / ban)
            int iconSize = 18;
            int iconX = rect.Right - 20 - iconSize;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;
            DrawBanIcon(g, new Rectangle(iconX, iconY, iconSize, iconSize));

            // Text
            var textRect = new RectangleF(rect.Left, rect.Top, rect.Width - 28, rect.Height);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap,
            };
            using (var b = new SolidBrush(C_BLOCK_TEXT))
                g.DrawString("حظر المستخدم", _fntBlock, b, textRect, sf);
        }

        private void DrawBanIcon(Graphics g, Rectangle r)
        {
            // Circle with diagonal line (ban/block symbol)
            using var pen = new Pen(C_BLOCK_TEXT, 1.8f) { LineJoin = LineJoin.Round };
            g.DrawEllipse(pen, r);
            float margin = r.Width * 0.21f;
            g.DrawLine(pen,
                r.Left + margin, r.Bottom - margin,
                r.Right - margin, r.Top + margin);
        }

        // ────────────────────────────────────────────────────────────────────
        //  Section wrapper + helpers
        // ────────────────────────────────────────────────────────────────────
        private Panel WrapSection(string label, Panel contentPanel)
        {
            var outer = new Panel { BackColor = C_BG, Margin = new Padding(0) };

            // Section label
            var lbl = new SectionLabel
            {
                Text = label,
                LabelFont = _fntLabel,
                ForeColor = C_LABEL,
                Height = 20,
                BackColor = C_BG,
            };

            outer.Controls.Add(lbl);
            outer.Controls.Add(contentPanel);

            // Layout on handle creation (first real size available)
            outer.HandleCreated += (s, _) => LayoutSection(s as Panel, label, contentPanel);
            outer.Resize += (s, _) => LayoutSection(s as Panel, label, contentPanel);

            return outer;
        }

        private static void LayoutSection(Panel? outer, string label, Panel content)
        {
            if (outer == null) return;
            int w = outer.Width;

            // label
            if (outer.Controls.Count >= 1 && outer.Controls[0] is SectionLabel lbl)
            {
                lbl.SetBounds(0, 10, w, 20);
            }

            // content
            content.SetBounds(0, 32, w, content.Height);
            outer.Height = 32 + content.Height + 12;
        }

        private Panel MakeDivider()
        {
            return new Panel
            {
                Height = 1,
                BackColor = C_DIVIDER,
                Margin = new Padding(SECTION_PAD_H, 4, SECTION_PAD_H, 4),
            };
        }

        private Panel MakeSpringPanel()
        {
            return new Panel { Height = 16, BackColor = C_BG };
        }

        // ────────────────────────────────────────────────────────────────────
        //  Refresh helpers
        // ────────────────────────────────────────────────────────────────────
        private void RefreshAll()
        {
            if (_isAnyDesignMode || _flpSections == null) return;
            RefreshNameRole();
            RefreshContactInfo();
            RebuildTags();
            RefreshMedia();
            _pnlAvatar?.Invalidate();
        }

        private void RefreshNameRole()
        {
            _pnlNameRole?.Invalidate();
        }

        private void RefreshContactInfo()
        {
            _pnlContactInfo?.Invalidate();
        }

        private void RebuildTags()
        {
            if (_pnlTags == null || _flpSections == null) return;
            PopulateTagsPanel(_pnlTags);

            // Find the wrapper panel that contains _pnlTags and re-layout it
            foreach (Control c in _flpSections.Controls)
            {
                if (c is Panel outer
                    && outer.Controls.Count == 2
                    && outer.Controls[0] is SectionLabel sLbl
                    && outer.Controls[1] == _pnlTags)
                {
                    LayoutSection(outer, sLbl.Text, _pnlTags);
                    break;
                }
            }
            _flpSections.PerformLayout();
        }

        private void RefreshMedia()
        {
            _pnlMedia?.Invalidate();
        }

        // ────────────────────────────────────────────────────────────────────
        //  OnLayout — resize child panels to match our width
        // ────────────────────────────────────────────────────────────────────
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (_flpSections == null) return; // BUG #3 guard

            int w = Width;
            if (w <= 0) return;

            foreach (Control c in _flpSections.Controls)
            {
                if (c is not Panel p) continue;
                p.Width = w;

                // Section wrapper panels (label + content) re-layout themselves
                if (p.Controls.Count == 2
                    && p.Controls[0] is SectionLabel sLbl
                    && p.Controls[1] is Panel innerContent)
                {
                    // Tags panel may need re-wrapping on width change
                    if (innerContent == _pnlTags)
                        PopulateTagsPanel(innerContent);

                    LayoutSection(p, sLbl.Text, innerContent);
                }
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  GDI+ utilities
        // ────────────────────────────────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var gp = new GraphicsPath();
            gp.AddArc(r.Left, r.Top, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        // ── SVG-style icon paths (24×24 coordinate space) ──────────────────
        private static GraphicsPath IconPhone()
        {
            // Simplified phone handset path in 24x24
            var p = new GraphicsPath();
            // Outer rounded rect approximation of a phone
            p.AddArc(3, 1, 18, 22, 180, 90);
            p.AddArc(3, 1, 18, 22, 270, 90);
            p.AddArc(3, 1, 18, 22, 0, 90);
            p.AddArc(3, 1, 18, 22, 90, 90);
            p.CloseFigure();
            // Speaker slot
            p.AddLine(9, 5, 15, 5);
            return p;
        }

        private static GraphicsPath IconEnvelope()
        {
            var p = new GraphicsPath();
            // Envelope outline
            p.AddRectangle(new RectangleF(2, 5, 20, 14));
            // Flap V
            p.StartFigure();
            p.AddLine(2, 5, 12, 13);
            p.AddLine(12, 13, 22, 5);
            return p;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Dispose
        // ────────────────────────────────────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fntName?.Dispose();
                _fntRole?.Dispose();
                _fntLabel?.Dispose();
                _fntInfo?.Dispose();
                _fntTag?.Dispose();
                _fntBlock?.Dispose();
                _fntCounter?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Inner helper controls
        // ════════════════════════════════════════════════════════════════════

        // ── TagChip — a single custom-drawn tag pill ─────────────────────
        private sealed class TagChip : Control
        {
            public Color ChipBg { get; set; } = Color.FromArgb(0xED, 0xE9, 0xFE);
            public Color ChipFg { get; set; } = Color.FromArgb(0x5B, 0x21, 0xB6);
            public Font? ChipFont { get; set; }

            public TagChip()
            {
                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (var path = RoundedRect(rect, TAG_RADIUS))
                using (var b = new SolidBrush(ChipBg))
                    g.FillPath(b, path);

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };
                using var fnt = ChipFont != null
                    ? new Font(ChipFont, ChipFont.Style)
                    : new Font("Segoe UI", 8f);
                using var fb = new SolidBrush(ChipFg);
                g.DrawString(Text, fnt, fb, (RectangleF)rect, sf);
            }
        }

        // ── SectionLabel — custom-painted section header label ───────────
        private sealed class SectionLabel : Control
        {
            public Font? LabelFont { get; set; }

            public SectionLabel()
            {
                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
                BackColor = C_BG;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Far,   // RTL: text aligns right
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };

                using var fnt = LabelFont != null
                    ? new Font(LabelFont, LabelFont.Style)
                    : new Font("Segoe UI", 8f, FontStyle.Bold);
                using var b = new SolidBrush(C_LABEL);
                g.DrawString(Text, fnt, b,
                    new RectangleF(SECTION_PAD_H, 0, Width - SECTION_PAD_H * 2, Height), sf);
            }
        }
    }
}