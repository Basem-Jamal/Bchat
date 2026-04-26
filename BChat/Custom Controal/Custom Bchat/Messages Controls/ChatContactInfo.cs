using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Message_Controls
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ChatContactInfo  –  BChat side-panel contact detail control  (v2)
    //  .NET 8 / Windows Forms  |  Cairo font  |  RTL
    //  ✦ Custom vertical-only styled scrollbar
    //  ✦ Full Designer property exposure
    //  ✦ Complete event surface
    // ══════════════════════════════════════════════════════════════════════════
    [ToolboxItem(true)]
    [Category("BChat - Chat")]
    [Description("Shows complete contact information for the active chat " +
                 "(avatar, tags, media, block button). " +
                 "Supports full Designer customisation and vertical-only scroll.")]
    [DefaultEvent("BlockClicked")]
    public class ChatContactInfo : UserControl
    {
        // ── .NET 8 design-mode guard ─────────────────────────────────────────
        private static readonly bool _isAnyDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            System.Diagnostics.Process.GetCurrentProcess().ProcessName
                  .IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;

        // ════════════════════════════════════════════════════════════════════
        //  Scrollbar width constant (used throughout layout)
        // ════════════════════════════════════════════════════════════════════
        private const int SCROLL_W = 6;  // slim track

        // ════════════════════════════════════════════════════════════════════
        //  Designer-exposed colour / metric backing fields
        // ════════════════════════════════════════════════════════════════════

        // ── Background / Structure ───────────────────────────────────────────
        private Color _colorBackground = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private Color _colorDivider = Color.FromArgb(0xF1, 0xF5, 0xF9);

        // ── Typography ───────────────────────────────────────────────────────
        private Color _colorTitle = Color.FromArgb(0x0F, 0x17, 0x2A);
        private Color _colorSubtitle = Color.FromArgb(0x64, 0x74, 0x8B);
        private Color _colorLabel = Color.FromArgb(0x94, 0xA3, 0xB8);
        private Color _colorInfoText = Color.FromArgb(0x0F, 0x17, 0x2A);

        // ── Icons ────────────────────────────────────────────────────────────
        private Color _colorIcon = Color.FromArgb(0x94, 0xA3, 0xB8);
        private Color _colorIconBg = Color.FromArgb(0xF1, 0xF5, 0xF9);

        // ── Avatar ───────────────────────────────────────────────────────────
        private Color _colorAvatarRing = Color.FromArgb(0xE0, 0xDD, 0xFD);
        private Color _colorAvatarGradTop = Color.FromArgb(0xC4, 0xC0, 0xFB);
        private Color _colorAvatarGradBot = Color.FromArgb(0x7C, 0x6F, 0xF7);
        private Color _colorAccent = Color.FromArgb(0x7C, 0x6F, 0xF7);

        // ── Media ────────────────────────────────────────────────────────────
        private Color _colorMediaBg = Color.FromArgb(0xF1, 0xF5, 0xF9);
        private Color _colorMediaOverlay = Color.FromArgb(0x1E, 0x29, 0x3B);

        // ── Block Button ─────────────────────────────────────────────────────
        private Color _colorBlockBg = Color.FromArgb(0xFE, 0xE2, 0xE2);
        private Color _colorBlockBgHover = Color.FromArgb(0xFE, 0xCA, 0xCA);
        private Color _colorBlockBgPress = Color.FromArgb(0xFC, 0xB0, 0xB0);
        private Color _colorBlockText = Color.FromArgb(0xDC, 0x26, 0x26);
        private string _blockButtonLabel = "حظر المستخدم";

        // ── Online Status ────────────────────────────────────────────────────
        private Color _colorOnline = Color.FromArgb(0x22, 0xC5, 0x5E);
        private Color _colorOffline = Color.FromArgb(0xCB, 0xD5, 0xE1);
        private Color _colorAway = Color.FromArgb(0xF5, 0x9E, 0x0B);

        // ── Scrollbar ────────────────────────────────────────────────────────
        private Color _colorScrollTrack = Color.FromArgb(0xF1, 0xF5, 0xF9);
        private Color _colorScrollThumb = Color.FromArgb(0xC4, 0xC0, 0xFB);
        private Color _colorScrollThumbHv = Color.FromArgb(0x7C, 0x6F, 0xF7);

        // ── Metrics ──────────────────────────────────────────────────────────
        private int _avatarSize = 96;
        private int _sectionPadH = 20;
        private int _mediaTileSize = 48;
        private int _mediaRadius = 8;
        private int _blockHeight = 44;
        private int _blockRadius = 10;
        private int _tagRadius = 16;
        private int _dividerThickness = 1;

        // ── Font family ──────────────────────────────────────────────────────
        private string _fontFamily = "";   // resolved once in BuildFonts()

        // ════════════════════════════════════════════════════════════════════
        //  Data backing fields
        // ════════════════════════════════════════════════════════════════════
        private string _contactName = "اسم المستخدم";
        private string _contactRole = "عميل";
        private Image? _contactAvatar = null;
        private string _contactPhone = "+966 50 000 0000";
        private string _contactEmail = "user@example.com";
        private string _contactLocation = "";
        private string _contactNotes = "";
        private OnlineStatus _onlineStatus = OnlineStatus.Offline;
        private bool _isBlocked = false;
        private List<string> _tags = new() { "VIP", "مميز", "نشط" };
        private List<Image> _mediaThumbs = new();
        private int _totalMedia = 0;

        // Tag palette (BG, FG) cycled by index
        private static readonly (Color bg, Color fg)[] TAG_PALETTE =
        {
            (Color.FromArgb(0xFE, 0xF3, 0xC7), Color.FromArgb(0x92, 0x40, 0x0E)),
            (Color.FromArgb(0xDB, 0xEA, 0xFE), Color.FromArgb(0x1E, 0x40, 0xAF)),
            (Color.FromArgb(0xD1, 0xFA, 0xE5), Color.FromArgb(0x06, 0x5F, 0x46)),
            (Color.FromArgb(0xFC, 0xE7, 0xF3), Color.FromArgb(0x9D, 0x17, 0x4D)),
            (Color.FromArgb(0xED, 0xE9, 0xFE), Color.FromArgb(0x5B, 0x21, 0xB6)),
        };

        // ════════════════════════════════════════════════════════════════════
        //  Fonts
        // ════════════════════════════════════════════════════════════════════
        private Font _fntName = null!;
        private Font _fntRole = null!;
        private Font _fntLabel = null!;
        private Font _fntInfo = null!;
        private Font _fntTag = null!;
        private Font _fntBlock = null!;
        private Font _fntCounter = null!;
        private Font _fntNotes = null!;

        // ════════════════════════════════════════════════════════════════════
        //  Child controls
        // ════════════════════════════════════════════════════════════════════
        private CustomScrollPanel _scrollPanel = null!;
        private StyledScrollBar _vScrollBar = null!;

        private Panel _pnlAvatar = null!;
        private Panel _pnlNameRole = null!;
        private Panel _pnlContactInfo = null!;
        private Panel _pnlTags = null!;
        private Panel _pnlMedia = null!;
        private Panel _pnlNotes = null!;
        private Panel _pnlBlock = null!;

        // Button state
        private bool _blockHover = false;
        private bool _blockPress = false;

        // ════════════════════════════════════════════════════════════════════
        //  Events
        // ════════════════════════════════════════════════════════════════════
        /// <summary>Fired when the block/unblock button is clicked.</summary>
        [Category("BChat"), Description("Fired when the block/unblock button is clicked.")]
        public event EventHandler? BlockClicked;

        /// <summary>Fired when a tag chip is clicked. Arg = tag text.</summary>
        [Category("BChat"), Description("Fired when a tag chip is clicked. Argument = tag text.")]
        public event EventHandler<string>? TagClicked;

        /// <summary>Fired when a media tile is clicked. Arg = zero-based tile index.</summary>
        [Category("BChat"), Description("Fired when a media tile is clicked. Argument = tile index.")]
        public event EventHandler<int>? MediaClicked;

        /// <summary>Fired when the phone row is clicked. Arg = phone string.</summary>
        [Category("BChat"), Description("Fired when the phone row is clicked. Argument = phone text.")]
        public event EventHandler<string>? PhoneClicked;

        /// <summary>Fired when the email row is clicked. Arg = email string.</summary>
        [Category("BChat"), Description("Fired when the email row is clicked. Argument = email text.")]
        public event EventHandler<string>? EmailClicked;

        /// <summary>Fired when the location row is clicked. Arg = location string.</summary>
        [Category("BChat"), Description("Fired when the location row is clicked. Argument = location text.")]
        public event EventHandler<string>? LocationClicked;

        /// <summary>Fired when the avatar image is clicked.</summary>
        [Category("BChat"), Description("Fired when the avatar image is clicked.")]
        public event EventHandler? AvatarClicked;

        /// <summary>Fired when the online status indicator is clicked.</summary>
        [Category("BChat"), Description("Fired when the online status dot is clicked.")]
        public event EventHandler<OnlineStatus>? StatusClicked;

        // ════════════════════════════════════════════════════════════════════
        //  Constructor
        // ════════════════════════════════════════════════════════════════════
        public ChatContactInfo()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = Color.White;
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

        // ════════════════════════════════════════════════════════════════════
        //  ── DESIGNER PROPERTIES ─────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════

        // ── Data ─────────────────────────────────────────────────────────────
        [Category("BChat – Data")]
        [Description("Display name of the contact.")]
        public string ContactName
        {
            get => _contactName;
            set { _contactName = value ?? ""; RefreshNameRole(); }
        }

        [Category("BChat – Data")]
        [Description("Role or status label shown below the name.")]
        public string ContactRole
        {
            get => _contactRole;
            set { _contactRole = value ?? ""; RefreshNameRole(); }
        }

        [Category("BChat – Data")]
        [Description("Avatar image (clipped to a circle).")]
        public Image? ContactAvatar
        {
            get => _contactAvatar;
            set { _contactAvatar = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Data")]
        [Description("Phone number string.")]
        public string ContactPhone
        {
            get => _contactPhone;
            set { _contactPhone = value ?? ""; RefreshContactInfo(); }
        }

        [Category("BChat – Data")]
        [Description("Email address string.")]
        public string ContactEmail
        {
            get => _contactEmail;
            set { _contactEmail = value ?? ""; RefreshContactInfo(); }
        }

        [Category("BChat – Data")]
        [Description("Location string (leave empty to hide the row).")]
        public string ContactLocation
        {
            get => _contactLocation;
            set { _contactLocation = value ?? ""; RefreshContactInfo(); }
        }

        [Category("BChat – Data")]
        [Description("Internal notes / memo text (leave empty to hide the section).")]
        public string ContactNotes
        {
            get => _contactNotes;
            set { _contactNotes = value ?? ""; RefreshNotes(); }
        }

        [Category("BChat – Data")]
        [Description("Online presence status of the contact.")]
        [DefaultValue(OnlineStatus.Offline)]
        public OnlineStatus ContactStatus
        {
            get => _onlineStatus;
            set { _onlineStatus = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Data")]
        [Description("Whether the contact is currently blocked (changes button label/style).")]
        [DefaultValue(false)]
        public bool IsBlocked
        {
            get => _isBlocked;
            set { _isBlocked = value; _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Data")]
        [Description("List of tag strings shown as chip pills.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> Tags
        {
            get => _tags;
            set { _tags = value ?? new(); RebuildTags(); }
        }

        [Category("BChat – Data")]
        [Description("List of media thumbnail images (max 4 rendered; remainder counted).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Image> MediaThumbnails
        {
            get => _mediaThumbs;
            set { _mediaThumbs = value ?? new(); RefreshMedia(); }
        }

        [Category("BChat – Data")]
        [Description("Total number of media items (used to calculate the +X overflow counter tile).")]
        [DefaultValue(0)]
        public int TotalMediaCount
        {
            get => _totalMedia;
            set { _totalMedia = Math.Max(0, value); RefreshMedia(); }
        }

        // ── Colours: Background / Structure ──────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorBackground
        {
            get => _colorBackground;
            set { _colorBackground = value; ApplyBackground(); }
        }

        [Category("BChat – Colors")]
        public Color ColorDivider
        {
            get => _colorDivider;
            set { _colorDivider = value; RebuildDividers(); }
        }

        // ── Colours: Typography ───────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorTitle
        {
            get => _colorTitle;
            set { _colorTitle = value; _pnlNameRole?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorSubtitle
        {
            get => _colorSubtitle;
            set { _colorSubtitle = value; _pnlNameRole?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorLabel
        {
            get => _colorLabel;
            set { _colorLabel = value; InvalidateLabels(); }
        }

        [Category("BChat – Colors")]
        public Color ColorInfoText
        {
            get => _colorInfoText;
            set { _colorInfoText = value; RefreshContactInfo(); }
        }

        // ── Colours: Icons ────────────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorIcon
        {
            get => _colorIcon;
            set { _colorIcon = value; RefreshContactInfo(); }
        }

        [Category("BChat – Colors")]
        public Color ColorIconBackground
        {
            get => _colorIconBg;
            set { _colorIconBg = value; RefreshContactInfo(); }
        }

        // ── Colours: Avatar ───────────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorAvatarRing
        {
            get => _colorAvatarRing;
            set { _colorAvatarRing = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorAvatarGradientTop
        {
            get => _colorAvatarGradTop;
            set { _colorAvatarGradTop = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorAvatarGradientBottom
        {
            get => _colorAvatarGradBot;
            set { _colorAvatarGradBot = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorAccent
        {
            get => _colorAccent;
            set { _colorAccent = value; Invalidate(true); }
        }

        // ── Colours: Online Status ────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorOnline
        {
            get => _colorOnline;
            set { _colorOnline = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorOffline
        {
            get => _colorOffline;
            set { _colorOffline = value; _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorAway
        {
            get => _colorAway;
            set { _colorAway = value; _pnlAvatar?.Invalidate(); }
        }

        // ── Colours: Media ────────────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorMediaBackground
        {
            get => _colorMediaBg;
            set { _colorMediaBg = value; _pnlMedia?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorMediaOverlay
        {
            get => _colorMediaOverlay;
            set { _colorMediaOverlay = value; _pnlMedia?.Invalidate(); }
        }

        // ── Colours: Block Button ─────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorBlockBackground
        {
            get => _colorBlockBg;
            set { _colorBlockBg = value; _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorBlockBackgroundHover
        {
            get => _colorBlockBgHover;
            set { _colorBlockBgHover = value; _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorBlockBackgroundPressed
        {
            get => _colorBlockBgPress;
            set { _colorBlockBgPress = value; _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorBlockText
        {
            get => _colorBlockText;
            set { _colorBlockText = value; _pnlBlock?.Invalidate(); }
        }

        // ── Colours: Scrollbar ────────────────────────────────────────────────
        [Category("BChat – Colors")]
        public Color ColorScrollTrack
        {
            get => _colorScrollTrack;
            set { _colorScrollTrack = value; _vScrollBar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorScrollThumb
        {
            get => _colorScrollThumb;
            set { _colorScrollThumb = value; _vScrollBar?.Invalidate(); }
        }

        [Category("BChat – Colors")]
        public Color ColorScrollThumbHover
        {
            get => _colorScrollThumbHv;
            set { _colorScrollThumbHv = value; _vScrollBar?.Invalidate(); }
        }

        // ── Metrics ───────────────────────────────────────────────────────────
        [Category("BChat – Metrics")]
        [Description("Diameter of the avatar circle in pixels.")]
        [DefaultValue(96)]
        public int AvatarSize
        {
            get => _avatarSize;
            set { _avatarSize = Math.Max(32, value); _pnlAvatar?.Invalidate(); }
        }

        [Category("BChat – Metrics")]
        [Description("Left/right padding inside each section, in pixels.")]
        [DefaultValue(20)]
        public int SectionPaddingH
        {
            get => _sectionPadH;
            set { _sectionPadH = Math.Max(4, value); PerformFullLayout(); }
        }

        [Category("BChat – Metrics")]
        [Description("Media thumbnail tile size in pixels.")]
        [DefaultValue(48)]
        public int MediaTileSize
        {
            get => _mediaTileSize;
            set { _mediaTileSize = Math.Max(24, value); RefreshMedia(); }
        }

        [Category("BChat – Metrics")]
        [Description("Corner radius of media tiles.")]
        [DefaultValue(8)]
        public int MediaTileRadius
        {
            get => _mediaRadius;
            set { _mediaRadius = Math.Max(0, value); RefreshMedia(); }
        }

        [Category("BChat – Metrics")]
        [Description("Height of the block/unblock button.")]
        [DefaultValue(44)]
        public int BlockButtonHeight
        {
            get => _blockHeight;
            set { _blockHeight = Math.Max(28, value); _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Metrics")]
        [Description("Corner radius of the block/unblock button.")]
        [DefaultValue(10)]
        public int BlockButtonRadius
        {
            get => _blockRadius;
            set { _blockRadius = Math.Max(0, value); _pnlBlock?.Invalidate(); }
        }

        [Category("BChat – Metrics")]
        [Description("Corner radius of tag chip pills.")]
        [DefaultValue(16)]
        public int TagChipRadius
        {
            get => _tagRadius;
            set { _tagRadius = Math.Max(0, value); RebuildTags(); }
        }

        [Category("BChat – Metrics")]
        [Description("Thickness of divider lines in pixels.")]
        [DefaultValue(1)]
        public int DividerThickness
        {
            get => _dividerThickness;
            set { _dividerThickness = Math.Max(1, value); RebuildDividers(); }
        }

        // ── Text / Labels ─────────────────────────────────────────────────────
        [Category("BChat – Text")]
        [Description("Label of the block / unblock button.")]
        public string BlockButtonLabel
        {
            get => _blockButtonLabel;
            set { _blockButtonLabel = value ?? ""; _pnlBlock?.Invalidate(); }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Public API
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Atomically sets all contact data and redraws once.
        /// </summary>
        public void SetContact(
            string name,
            string role,
            Image? avatar,
            string phone,
            string email,
            string location = "",
            string notes = "",
            OnlineStatus status = OnlineStatus.Offline,
            bool isBlocked = false,
            List<string>? tags = null,
            List<Image>? mediaThumbs = null,
            int totalMediaCount = 0)
        {
            _contactName = name ?? "";
            _contactRole = role ?? "";
            _contactAvatar = avatar;
            _contactPhone = phone ?? "";
            _contactEmail = email ?? "";
            _contactLocation = location ?? "";
            _contactNotes = notes ?? "";
            _onlineStatus = status;
            _isBlocked = isBlocked;
            _tags = tags ?? new();
            _mediaThumbs = mediaThumbs ?? new();
            _totalMedia = Math.Max(0, totalMediaCount);

            RefreshAll();
        }

        /// <summary>Scrolls the panel to the top.</summary>
        public void ScrollToTop()
        {
            _scrollPanel?.ScrollToTop();
            _vScrollBar?.UpdateThumb();
        }

        // ════════════════════════════════════════════════════════════════════
        //  Font building
        // ════════════════════════════════════════════════════════════════════
        private void BuildFonts()
        {
            _fontFamily = IsFontAvailable("Cairo") ? "Cairo" : "Segoe UI";

            _fntName = new Font(_fontFamily, 13f, FontStyle.Bold, GraphicsUnit.Point);
            _fntRole = new Font(_fontFamily, 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fntLabel = new Font(_fontFamily, 8f, FontStyle.Bold, GraphicsUnit.Point);
            _fntInfo = new Font(_fontFamily, 10f, FontStyle.Regular, GraphicsUnit.Point);
            _fntTag = new Font(_fontFamily, 8f, FontStyle.Regular, GraphicsUnit.Point);
            _fntBlock = new Font(_fontFamily, 10f, FontStyle.Bold, GraphicsUnit.Point);
            _fntCounter = new Font(_fontFamily, 9f, FontStyle.Bold, GraphicsUnit.Point);
            _fntNotes = new Font(_fontFamily, 9f, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static bool IsFontAvailable(string name)
        {
            using var fc = new InstalledFontCollection();
            foreach (var ff in fc.Families)
                if (string.Equals(ff.Name, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Layout building  (called once from ctor)
        // ════════════════════════════════════════════════════════════════════
        private void BuildLayout()
        {
            // ── Custom scrollbar ─────────────────────────────────────────────
            _vScrollBar = new StyledScrollBar
            {
                Dock = DockStyle.Right,
                Width = SCROLL_W + 4,   // 4 = margin breathing room
                TrackColor = _colorScrollTrack,
                ThumbColor = _colorScrollThumb,
                ThumbHover = _colorScrollThumbHv,
                Visible = false,
            };

            // ── Scrollable content panel ─────────────────────────────────────
            _scrollPanel = new CustomScrollPanel
            {
                Dock = DockStyle.Fill,
                BackColor = _colorBackground,
                RightToLeft = RightToLeft.Yes,
            };
            _scrollPanel.ScrollChanged += (s, e) => _vScrollBar?.UpdateThumb();
            _scrollPanel.ContentHeightChanged += OnContentHeightChanged;

            // Wire scrollbar → panel
            _vScrollBar.Scroll += (delta) => _scrollPanel.ScrollBy(delta);

            // Section panels
            _pnlAvatar = BuildAvatarPanel();
            _pnlNameRole = BuildNameRolePanel();
            _pnlContactInfo = BuildContactInfoPanel();
            _pnlTags = BuildTagsPanel();
            _pnlMedia = BuildMediaPanel();
            _pnlNotes = BuildNotesPanel();
            _pnlBlock = BuildBlockPanel();

            // Add sections into the scroll panel in order
            _scrollPanel.AddSection(_pnlAvatar);
            _scrollPanel.AddSection(_pnlNameRole);
            _scrollPanel.AddSection(MakeDivider());
            _scrollPanel.AddSection(WrapSection("معلومات الاتصال", _pnlContactInfo));
            _scrollPanel.AddSection(MakeDivider());
            _scrollPanel.AddSection(WrapSection("الوسوم", _pnlTags));
            _scrollPanel.AddSection(MakeDivider());
            _scrollPanel.AddSection(WrapSection("الصور والملفات", _pnlMedia));
            _scrollPanel.AddSection(MakeDivider());
            _scrollPanel.AddSection(WrapSection("ملاحظات", _pnlNotes));
            _scrollPanel.AddSection(MakeSpacerPanel(16));
            _scrollPanel.AddSection(_pnlBlock);
            _scrollPanel.AddSection(MakeSpacerPanel(16));

            // Attach scrollbar to the scroll panel
            _scrollPanel.LinkedScrollBar = _vScrollBar;

            Controls.Add(_scrollPanel);
            Controls.Add(_vScrollBar);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Avatar
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildAvatarPanel()
        {
            var pnl = new Panel
            {
                Height = _avatarSize + 40,
                BackColor = _colorBackground,
                Margin = new Padding(0, 16, 0, 0),
                Cursor = Cursors.Hand,
            };
            pnl.Paint += PaintAvatar;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            pnl.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                var p = s as Panel;
                if (p == null) return;

                int cx = p.Width / 2;
                int cy = p.Height / 2;
                int r = _avatarSize / 2;

                // Status dot hit-test
                int dotR = 10;
                int dotX = cx + r - dotR;
                int dotY = cy + r - dotR;
                var dotRect = new Rectangle(dotX - 4, dotY - 4, dotR * 2 + 8, dotR * 2 + 8);

                if (dotRect.Contains(e.Location))
                    StatusClicked?.Invoke(this, _onlineStatus);
                else
                    AvatarClicked?.Invoke(this, EventArgs.Empty);
            };
            return pnl;
        }

        private void PaintAvatar(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int r = _avatarSize / 2;
            int cx = pnl.Width / 2;
            int cy = pnl.Height / 2;

            // Subtle ring
            using (var ringPen = new Pen(_colorAvatarRing, 3f))
                g.DrawEllipse(ringPen, cx - r - 3, cy - r - 3, (r + 3) * 2, (r + 3) * 2);

            var clipRect = new Rectangle(cx - r, cy - r, _avatarSize, _avatarSize);

            using (var gp = new GraphicsPath())
            {
                gp.AddEllipse(clipRect);
                var prevClip = g.Clip;
                g.SetClip(gp);

                if (_contactAvatar != null)
                {
                    g.DrawImage(_contactAvatar, clipRect);
                }
                else
                {
                    using var brush = new LinearGradientBrush(
                        clipRect,
                        _colorAvatarGradTop,
                        _colorAvatarGradBot,
                        LinearGradientMode.ForwardDiagonal);
                    g.FillEllipse(brush, clipRect);

                    if (!string.IsNullOrWhiteSpace(_contactName))
                    {
                        string initial = _contactName[0].ToString();
                        using var fnt = new Font(_fntName.FontFamily, _avatarSize * 0.28f, FontStyle.Bold, GraphicsUnit.Point);
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

            // Online status dot
            int dotR = 10;
            int dotX = cx + r - dotR;
            int dotY = cy + r - dotR;
            Color dotColor = _onlineStatus switch
            {
                OnlineStatus.Online => _colorOnline,
                OnlineStatus.Away => _colorAway,
                _ => _colorOffline,
            };
            using (var b = new SolidBrush(dotColor))
                g.FillEllipse(b, dotX, dotY, dotR * 2, dotR * 2);
            using (var p = new Pen(_colorBackground, 2.5f))
                g.DrawEllipse(p, dotX, dotY, dotR * 2, dotR * 2);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Name + Role
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildNameRolePanel()
        {
            var pnl = new Panel
            {
                Height = 56,
                BackColor = _colorBackground,
                Margin = new Padding(0, 4, 0, 0),
            };
            pnl.Paint += PaintNameRole;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private void PaintNameRole(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntName == null || _fntRole == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            };

            using (var b = new SolidBrush(_colorTitle))
            {
                var nameRect = new RectangleF(_sectionPadH, 4, pnl.Width - _sectionPadH * 2, 26);
                g.DrawString(_contactName, _fntName, b, nameRect, sf);
            }
            using (var b = new SolidBrush(_colorSubtitle))
            {
                var roleRect = new RectangleF(_sectionPadH, 30, pnl.Width - _sectionPadH * 2, 22);
                g.DrawString(_contactRole, _fntRole, b, roleRect, sf);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Contact Info  (phone / email / location rows)
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildContactInfoPanel()
        {
            var pnl = new Panel
            {
                Height = 108,
                BackColor = _colorBackground,
            };
            pnl.Paint += PaintContactInfo;
            pnl.MouseDown += ContactInfo_MouseDown;
            pnl.MouseMove += ContactInfo_MouseMove;
            pnl.Cursor = Cursors.Default;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private int ContactRowHeight => 36;
        private Rectangle GetPhoneRowRect(Panel pnl) => new(0, 0, pnl.Width, ContactRowHeight);
        private Rectangle GetEmailRowRect(Panel pnl) => new(0, ContactRowHeight, pnl.Width, ContactRowHeight);
        private Rectangle GetLocationRowRect(Panel pnl) => new(0, ContactRowHeight * 2, pnl.Width, ContactRowHeight);

        private void PaintContactInfo(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntInfo == null) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawInfoRow(g, pnl.Width, 0, IconPhone(), _contactPhone);
            DrawInfoRow(g, pnl.Width, ContactRowHeight, IconEnvelope(), _contactEmail);

            // Location row — only if set
            if (!string.IsNullOrWhiteSpace(_contactLocation))
                DrawInfoRow(g, pnl.Width, ContactRowHeight * 2, IconLocation(), _contactLocation);

            // Adjust height
            int rows = string.IsNullOrWhiteSpace(_contactLocation) ? 2 : 3;
            if (pnl.Height != rows * ContactRowHeight)
                pnl.Height = rows * ContactRowHeight;
        }

        private void DrawInfoRow(Graphics g, int panelWidth, int y, GraphicsPath icon, string text)
        {
            const int iconSize = 16;
            int iconPad = _sectionPadH;
            int rowH = ContactRowHeight;

            int iconX = panelWidth - iconPad - iconSize;
            int iconY = y + (rowH - iconSize) / 2;
            var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);

            // Icon background circle
            using (var b = new SolidBrush(_colorIconBg))
                g.FillEllipse(b, iconRect.Left - 3, iconRect.Top - 3, iconSize + 6, iconSize + 6);

            // Icon path
            var state = g.Save();
            g.TranslateTransform(iconRect.Left, iconRect.Top);
            g.ScaleTransform(iconSize / 24f, iconSize / 24f);
            using (var pen = new Pen(_colorIcon, 1.8f)
            { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
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
            using (var b = new SolidBrush(_colorInfoText))
                g.DrawString(text, _fntInfo, b, textRect, sf);
        }

        private void ContactInfo_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl) return;
            bool over = GetPhoneRowRect(pnl).Contains(e.Location)
                     || GetEmailRowRect(pnl).Contains(e.Location)
                     || (!string.IsNullOrWhiteSpace(_contactLocation)
                         && GetLocationRowRect(pnl).Contains(e.Location));
            pnl.Cursor = over ? Cursors.Hand : Cursors.Default;
        }

        private void ContactInfo_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl || e.Button != MouseButtons.Left) return;
            if (GetPhoneRowRect(pnl).Contains(e.Location))
                PhoneClicked?.Invoke(this, _contactPhone);
            else if (GetEmailRowRect(pnl).Contains(e.Location))
                EmailClicked?.Invoke(this, _contactEmail);
            else if (!string.IsNullOrWhiteSpace(_contactLocation)
                     && GetLocationRowRect(pnl).Contains(e.Location))
                LocationClicked?.Invoke(this, _contactLocation);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Tags
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildTagsPanel()
        {
            var pnl = new Panel { BackColor = _colorBackground, Margin = new Padding(0) };
            PopulateTagsPanel(pnl);
            return pnl;
        }

        private void PopulateTagsPanel(Panel pnl)
        {
            pnl.Controls.Clear();

            const int chipH = 26;
            const int gapX = 6;
            const int gapY = 6;
            int padH = _sectionPadH;
            int x = padH;
            int y = 0;

            for (int i = 0; i < _tags.Count; i++)
            {
                string tag = _tags[i];
                var colors = TAG_PALETTE[i % TAG_PALETTE.Length];

                using var tmp = Graphics.FromHwnd(IntPtr.Zero);
                int textW = (int)tmp.MeasureString(tag, _fntTag).Width + 1;
                int chipW = textW + 20;

                int availW = (pnl.Width > 0 ? pnl.Width : 260) - padH * 2;
                if (x + chipW > padH + availW && x > padH)
                {
                    x = padH;
                    y += chipH + gapY;
                }

                string ft = tag;
                var chip = new TagChip
                {
                    Text = tag,
                    ChipBg = colors.bg,
                    ChipFg = colors.fg,
                    ChipFont = _fntTag,
                    ChipRadius = _tagRadius,
                    Location = new Point(x, y),
                    Size = new Size(chipW, chipH),
                    Cursor = Cursors.Hand,
                };
                chip.Click += (s, _) => TagClicked?.Invoke(this, ft);
                pnl.Controls.Add(chip);

                x += chipW + gapX;
            }

            // Empty-state label
            if (_tags.Count == 0)
            {
                var lbl = new SectionLabel
                {
                    Text = "لا توجد وسوم",
                    LabelFont = _fntRole,
                    ForeColor = _colorLabel,
                    Height = 28,
                    BackColor = _colorBackground,
                    Location = new Point(0, 0),
                    Width = pnl.Width,
                };
                pnl.Controls.Add(lbl);
                pnl.Height = 28;
                return;
            }

            pnl.Height = Math.Max(y + chipH + 4, chipH + 4);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Media
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildMediaPanel()
        {
            var pnl = new Panel
            {
                Height = _mediaTileSize + 8,
                BackColor = _colorBackground,
                Margin = new Padding(0),
            };
            pnl.Paint += PaintMedia;
            pnl.MouseDown += Media_MouseDown;
            pnl.MouseMove += Media_MouseMove;
            pnl.Cursor = Cursors.Default;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private void PaintMedia(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntRole == null || _fntCounter == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int visibleCount = Math.Min(_mediaThumbs.Count, 4);
            bool hasOverflow = _totalMedia > 4;
            int tileCount = visibleCount + (hasOverflow ? 1 : 0);

            if (tileCount == 0)
            {
                using var b = new SolidBrush(_colorLabel);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                };
                g.DrawString("لا توجد وسائط", _fntRole, b,
                    new RectangleF(0, 0, pnl.Width, pnl.Height), sf);
                return;
            }

            int totalW = tileCount * _mediaTileSize + Math.Max(0, tileCount - 1) * 6;
            int startX = (pnl.Width - totalW) / 2;
            int startY = 4;

            for (int i = 0; i < visibleCount; i++)
            {
                int x = startX + i * (_mediaTileSize + 6);
                var rect = new Rectangle(x, startY, _mediaTileSize, _mediaTileSize);
                DrawRoundedTile(g, rect, _mediaThumbs[i], false, 0);
            }

            if (hasOverflow)
            {
                int x = startX + visibleCount * (_mediaTileSize + 6);
                var rect = new Rectangle(x, startY, _mediaTileSize, _mediaTileSize);
                DrawRoundedTile(g, rect, null, true, _totalMedia - 4);
            }
        }

        private void DrawRoundedTile(Graphics g, Rectangle rect, Image? img, bool isCounter, int count)
        {
            using var path = RoundedRect(rect, _mediaRadius);

            if (isCounter)
            {
                using (var b = new SolidBrush(_colorMediaOverlay))
                    g.FillPath(b, path);

                string txt = "+" + count;
                using var b2 = new SolidBrush(Color.White);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                };
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
                using (var b = new SolidBrush(_colorMediaBg))
                    g.FillPath(b, path);
            }
        }

        private int GetMediaTileIndex(Panel pnl, Point pt)
        {
            int visibleCount = Math.Min(_mediaThumbs.Count, 4);
            bool hasOverflow = _totalMedia > 4;
            int tileCount = visibleCount + (hasOverflow ? 1 : 0);
            if (tileCount == 0) return -1;

            int totalW = tileCount * _mediaTileSize + Math.Max(0, tileCount - 1) * 6;
            int startX = (pnl.Width - totalW) / 2;
            int startY = 4;

            for (int i = 0; i < tileCount; i++)
            {
                int x = startX + i * (_mediaTileSize + 6);
                var rect = new Rectangle(x, startY, _mediaTileSize, _mediaTileSize);
                if (rect.Contains(pt)) return i;
            }
            return -1;
        }

        private void Media_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl) return;
            pnl.Cursor = GetMediaTileIndex(pnl, e.Location) >= 0
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void Media_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Panel pnl || e.Button != MouseButtons.Left) return;
            int idx = GetMediaTileIndex(pnl, e.Location);
            if (idx >= 0) MediaClicked?.Invoke(this, idx);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section: Notes
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildNotesPanel()
        {
            var pnl = new Panel
            {
                Height = 40,
                BackColor = _colorBackground,
                Margin = new Padding(0),
            };
            pnl.Paint += PaintNotes;
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            RefreshNotesHeight(pnl);
            return pnl;
        }

        private void PaintNotes(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntNotes == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            string text = string.IsNullOrWhiteSpace(_contactNotes)
                ? "لا توجد ملاحظات"
                : _contactNotes;

            Color c = string.IsNullOrWhiteSpace(_contactNotes) ? _colorLabel : _colorInfoText;

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.Word,
            };
            var rect = new RectangleF(_sectionPadH, 4, pnl.Width - _sectionPadH * 2, pnl.Height - 8);
            using var b = new SolidBrush(c);
            g.DrawString(text, _fntNotes, b, rect, sf);
        }

        private void RefreshNotesHeight(Panel pnl)
        {
            if (pnl.Width <= 0) return;
            string text = string.IsNullOrWhiteSpace(_contactNotes) ? "لا توجد ملاحظات" : _contactNotes;
            using var tmp = Graphics.FromHwnd(IntPtr.Zero);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                Trimming = StringTrimming.Word,
            };
            float w = Math.Max(1, pnl.Width - _sectionPadH * 2);
            var size = tmp.MeasureString(text, _fntNotes, (int)w, sf);
            pnl.Height = (int)size.Height + 16;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Block Button
        // ════════════════════════════════════════════════════════════════════
        private Panel BuildBlockPanel()
        {
            var pnl = new Panel
            {
                Height = _blockHeight + 24,
                BackColor = _colorBackground,
                Margin = new Padding(0),
                Cursor = Cursors.Hand,
            };
            pnl.Paint += PaintBlockButton;
            pnl.MouseEnter += (s, _) => { _blockHover = true; _blockPress = false; (s as Control)?.Invalidate(); };
            pnl.MouseLeave += (s, _) => { _blockHover = false; _blockPress = false; (s as Control)?.Invalidate(); };
            pnl.MouseDown += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left
                    && GetBlockButtonRect(s as Panel).Contains(me.Location))
                {
                    _blockPress = true;
                    (s as Control)?.Invalidate();
                }
            };
            pnl.MouseUp += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
                {
                    bool wasPress = _blockPress;
                    _blockPress = false;
                    (s as Control)?.Invalidate();
                    if (wasPress && GetBlockButtonRect(s as Panel).Contains(me.Location))
                        BlockClicked?.Invoke(this, EventArgs.Empty);
                }
            };
            pnl.Resize += (s, _) => (s as Control)?.Invalidate();
            return pnl;
        }

        private Rectangle GetBlockButtonRect(Panel? pnl)
        {
            if (pnl == null) return Rectangle.Empty;
            int y = (pnl.Height - _blockHeight) / 2;
            return new Rectangle(_sectionPadH, y, pnl.Width - _sectionPadH * 2, _blockHeight);
        }

        private void PaintBlockButton(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel pnl) return;
            if (_fntBlock == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = GetBlockButtonRect(pnl);
            Color bgColor = _blockPress ? _colorBlockBgPress
                          : _blockHover ? _colorBlockBgHover
                          : _colorBlockBg;

            using (var path = RoundedRect(rect, _blockRadius))
            using (var b = new SolidBrush(bgColor))
                g.FillPath(b, path);

            // Ban icon
            int iconSize = 18;
            int iconX = rect.Right - _sectionPadH - iconSize;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;
            DrawBanIcon(g, new Rectangle(iconX, iconY, iconSize, iconSize));

            // Button label
            string label = _isBlocked ? "إلغاء الحظر" : _blockButtonLabel;
            var textRect = new RectangleF(rect.Left, rect.Top, rect.Width - 32, rect.Height);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap,
            };
            using (var b = new SolidBrush(_colorBlockText))
                g.DrawString(label, _fntBlock, b, textRect, sf);
        }

        private void DrawBanIcon(Graphics g, Rectangle r)
        {
            using var pen = new Pen(_colorBlockText, 1.8f) { LineJoin = LineJoin.Round };
            g.DrawEllipse(pen, r);
            float margin = r.Width * 0.21f;
            g.DrawLine(pen,
                r.Left + margin, r.Bottom - margin,
                r.Right - margin, r.Top + margin);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Section wrapper helpers
        // ════════════════════════════════════════════════════════════════════
        private Panel WrapSection(string label, Panel contentPanel)
        {
            var outer = new Panel { BackColor = _colorBackground, Margin = new Padding(0) };

            var lbl = new SectionLabel
            {
                Text = label,
                LabelFont = _fntLabel,
                ForeColor = _colorLabel,
                Height = 20,
                BackColor = _colorBackground,
            };

            outer.Controls.Add(lbl);
            outer.Controls.Add(contentPanel);

            outer.HandleCreated += (s, _) => LayoutSection(s as Panel, contentPanel);
            outer.Resize += (s, _) => LayoutSection(s as Panel, contentPanel);

            return outer;
        }

        private static void LayoutSection(Panel? outer, Panel content)
        {
            if (outer == null) return;
            int w = outer.Width;
            if (outer.Controls.Count >= 1 && outer.Controls[0] is SectionLabel lbl)
                lbl.SetBounds(0, 10, w, 20);

            content.SetBounds(0, 32, w, content.Height);
            outer.Height = 32 + content.Height + 12;
        }

        private Panel MakeDivider()
        {
            return new Panel
            {
                Height = _dividerThickness,
                BackColor = _colorDivider,
                Margin = new Padding(_sectionPadH, 4, _sectionPadH, 4),
            };
        }

        private Panel MakeSpacerPanel(int height) =>
            new Panel { Height = height, BackColor = _colorBackground };

        // ════════════════════════════════════════════════════════════════════
        //  Refresh / invalidation helpers
        // ════════════════════════════════════════════════════════════════════
        private void RefreshAll()
        {
            if (_isAnyDesignMode || _scrollPanel == null) return;
            _pnlAvatar?.Invalidate();
            RefreshNameRole();
            RefreshContactInfo();
            RebuildTags();
            RefreshMedia();
            RefreshNotes();
            _pnlBlock?.Invalidate();
        }

        private void RefreshNameRole() => _pnlNameRole?.Invalidate();
        private void RefreshContactInfo() => _pnlContactInfo?.Invalidate();
        private void RefreshMedia() => _pnlMedia?.Invalidate();

        private void RefreshNotes()
        {
            if (_pnlNotes == null) return;
            RefreshNotesHeight(_pnlNotes);
            _pnlNotes.Invalidate();
            // Re-layout the wrapper that contains notes
            RewrapSection(_pnlNotes);
            _scrollPanel?.RecalculateLayout();
        }

        private void RebuildTags()
        {
            if (_pnlTags == null || _scrollPanel == null) return;
            PopulateTagsPanel(_pnlTags);
            RewrapSection(_pnlTags);
            _scrollPanel.RecalculateLayout();
        }

        private void RewrapSection(Panel contentPanel)
        {
            foreach (Control c in _scrollPanel.InnerControls)
            {
                if (c is Panel outer
                    && outer.Controls.Count == 2
                    && outer.Controls[0] is SectionLabel
                    && outer.Controls[1] == contentPanel)
                {
                    LayoutSection(outer, contentPanel);
                    break;
                }
            }
        }

        private void ApplyBackground()
        {
            if (_scrollPanel != null) _scrollPanel.BackColor = _colorBackground;
            Invalidate(true);
        }

        private void RebuildDividers()
        {
            // Dividers are plain panels — just invalidate root
            Invalidate(true);
        }

        private void InvalidateLabels() => Invalidate(true);

        private void PerformFullLayout()
        {
            PerformLayout();
            Invalidate(true);
        }

        // ════════════════════════════════════════════════════════════════════
        //  OnLayout — sync widths & show/hide scrollbar
        // ════════════════════════════════════════════════════════════════════
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (_scrollPanel == null) return;

            _scrollPanel.RecalculateLayout();
            OnContentHeightChanged(this, EventArgs.Empty);
        }

        private void OnContentHeightChanged(object? sender, EventArgs e)
        {
            if (_scrollPanel == null || _vScrollBar == null) return;

            bool needsScroll = _scrollPanel.ContentHeight > _scrollPanel.Height;
            _vScrollBar.Visible = needsScroll;

            if (needsScroll)
            {
                _vScrollBar.SetRange(_scrollPanel.ContentHeight, _scrollPanel.Height);
                _vScrollBar.TrackColor = _colorScrollTrack;
                _vScrollBar.ThumbColor = _colorScrollThumb;
                _vScrollBar.ThumbHover = _colorScrollThumbHv;
                _vScrollBar.UpdateThumb();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _scrollPanel?.ScrollBy(-e.Delta / 3);
            _vScrollBar?.UpdateThumb();
        }

        // ════════════════════════════════════════════════════════════════════
        //  GDI+ utilities
        // ════════════════════════════════════════════════════════════════════
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

        // ── Icon paths (24×24 coordinate space) ─────────────────────────────
        private static GraphicsPath IconPhone()
        {
            var p = new GraphicsPath();
            p.StartFigure();
            // Stylised phone arc path
            p.AddBezier(6f, 2f, 6f, 2f, 4f, 2f, 4f, 4f);
            p.AddBezier(4f, 4f, 4f, 8f, 6f, 10f, 8f, 10f);
            p.AddBezier(8f, 10f, 10f, 10f, 10f, 10f, 10f, 12f);
            p.AddBezier(10f, 12f, 10f, 14f, 8f, 14f, 8f, 16f);
            p.AddBezier(8f, 16f, 8f, 18f, 10f, 20f, 12f, 20f);
            p.AddBezier(12f, 20f, 14f, 20f, 14f, 20f, 14f, 22f);
            p.AddBezier(14f, 22f, 14f, 22f, 20f, 22f, 20f, 20f);
            p.AddBezier(20f, 20f, 20f, 18f, 18f, 16f, 16f, 16f);
            p.AddBezier(16f, 16f, 14f, 16f, 14f, 14f, 14f, 14f);
            p.AddBezier(14f, 14f, 14f, 12f, 16f, 12f, 18f, 10f);
            p.AddBezier(18f, 10f, 20f, 8f, 20f, 4f, 20f, 4f);
            p.AddBezier(20f, 4f, 20f, 2f, 18f, 2f, 16f, 2f);
            p.CloseFigure();
            return p;
        }

        private static GraphicsPath IconEnvelope()
        {
            var p = new GraphicsPath();
            p.AddRectangle(new RectangleF(2, 5, 20, 14));
            p.StartFigure();
            p.AddLine(2, 5, 12, 13);
            p.AddLine(12, 13, 22, 5);
            return p;
        }

        private static GraphicsPath IconLocation()
        {
            var p = new GraphicsPath();
            // Pin outline
            p.AddEllipse(8, 2, 8, 8);
            p.StartFigure();
            p.AddBezier(8, 8, 4, 14, 12, 22, 12, 22);
            p.AddBezier(12, 22, 12, 22, 20, 14, 16, 8);
            p.CloseFigure();
            return p;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Dispose
        // ════════════════════════════════════════════════════════════════════
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
                _fntNotes?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ════════════════════════════════════════════════════════════════════
        //  ── INNER HELPER CONTROLS ──────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════

        // ──────────────────────────────────────────────────────────────────────
        //  CustomScrollPanel  –  vertical-only, no native scroll UI
        // ──────────────────────────────────────────────────────────────────────
        private sealed class CustomScrollPanel : Panel
        {
            private int _scrollOffset = 0;
            private int _contentHeight = 0;
            private readonly List<Control> _sections = new();

            public int ContentHeight => _contentHeight;
            public IEnumerable<Control> InnerControls => _sections;

            public StyledScrollBar? LinkedScrollBar { get; set; }

            public event EventHandler? ScrollChanged;
            public event EventHandler? ContentHeightChanged;

            public CustomScrollPanel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);
                // Suppress native scrollbars
                AutoScroll = false;
            }

            public void AddSection(Control c)
            {
                _sections.Add(c);
                Controls.Add(c);
            }

            public void ScrollBy(int delta)
            {
                int maxScroll = Math.Max(0, _contentHeight - Height);
                _scrollOffset = Math.Max(0, Math.Min(_scrollOffset + delta, maxScroll));
                RecalculateLayout();
                ScrollChanged?.Invoke(this, EventArgs.Empty);
            }

            public void ScrollToTop()
            {
                _scrollOffset = 0;
                RecalculateLayout();
                ScrollChanged?.Invoke(this, EventArgs.Empty);
            }

            /// <summary>Stack sections vertically, shift by scroll offset.</summary>
            public void RecalculateLayout()
            {
                int scrollbarW = (LinkedScrollBar?.Visible == true)
                    ? LinkedScrollBar.Width
                    : 0;
                int w = Math.Max(1, Width - scrollbarW);

                int y = -_scrollOffset;
                foreach (var c in _sections)
                {
                    // Sync width — never force horizontal scroll
                    if (c.Width != w)
                    {
                        c.Width = w;

                        // Propagate width to section-wrapper children
                        if (c is Panel outer)
                        {
                            foreach (Control inner in outer.Controls)
                                inner.Width = w;
                        }
                    }

                    c.SetBounds(0, y, w, c.Height);
                    y += c.Height;
                }

                int newH = y + _scrollOffset;   // total content height
                if (newH != _contentHeight)
                {
                    _contentHeight = newH;
                    ContentHeightChanged?.Invoke(this, EventArgs.Empty);
                }

                Invalidate();
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                // Clamp scroll after resize
                int maxScroll = Math.Max(0, _contentHeight - Height);
                _scrollOffset = Math.Min(_scrollOffset, maxScroll);
                RecalculateLayout();
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                ScrollBy(-e.Delta / 3);
                LinkedScrollBar?.UpdateThumb();
            }

            // Kill the horizontal scrollbar at WM level
            protected override CreateParams CreateParams
            {
                get
                {
                    var cp = base.CreateParams;
                    cp.Style &= ~0x00100000; // WS_HSCROLL
                    cp.Style &= ~0x00200000; // WS_VSCROLL (we draw our own)
                    return cp;
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  StyledScrollBar  –  slim custom vertical scrollbar
        // ──────────────────────────────────────────────────────────────────────
        private sealed class StyledScrollBar : Control
        {
            public Color TrackColor { get; set; } = Color.FromArgb(0xF1, 0xF5, 0xF9);
            public Color ThumbColor { get; set; } = Color.FromArgb(0xC4, 0xC0, 0xFB);
            public Color ThumbHover { get; set; } = Color.FromArgb(0x7C, 0x6F, 0xF7);

            private int _totalH = 100;
            private int _viewH = 100;
            private int _offset = 0;

            private bool _thumbHover = false;
            private bool _dragging = false;
            private int _dragStartY = 0;
            private int _dragStartOffset = 0;

            public event Action<int>? Scroll;

            public StyledScrollBar()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            public void SetRange(int totalHeight, int viewHeight)
            {
                _totalH = Math.Max(1, totalHeight);
                _viewH = Math.Max(1, viewHeight);
                Invalidate();
            }

            public void UpdateThumb()
            {
                // Thumb position is driven by scroll panel's offset
                Invalidate();
            }

            // Map scroll panel offset → thumb top
            public void SyncOffset(int scrollOffset)
            {
                _offset = scrollOffset;
                Invalidate();
            }

            private (int thumbTop, int thumbH) CalcThumb()
            {
                if (_totalH <= _viewH) return (0, Height);
                int trackH = Height;
                int thumbH = Math.Max(24, (int)((float)_viewH / _totalH * trackH));
                int thumbTop = (int)((float)_offset / (_totalH - _viewH) * (trackH - thumbH));
                return (Math.Clamp(thumbTop, 0, trackH - thumbH), thumbH);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Track
                using (var b = new SolidBrush(TrackColor))
                    g.FillRectangle(b, 0, 0, Width, Height);

                var (thumbTop, thumbH) = CalcThumb();
                var thumbRect = new Rectangle(2, thumbTop, Width - 4, thumbH);
                int r = (Width - 4) / 2;

                // Thumb pill
                Color tc = _thumbHover || _dragging ? ThumbHover : ThumbColor;
                using (var path = RoundedRect(thumbRect, r))
                using (var b = new SolidBrush(tc))
                    g.FillPath(b, path);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                _thumbHover = true; Invalidate();
            }
            protected override void OnMouseLeave(EventArgs e)
            {
                _thumbHover = false; Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                var (thumbTop, thumbH) = CalcThumb();
                if (e.Y >= thumbTop && e.Y <= thumbTop + thumbH)
                {
                    _dragging = true;
                    _dragStartY = e.Y;
                    _dragStartOffset = _offset;
                    Capture = true;
                }
                else
                {
                    // Click on track → jump
                    JumpTo(e.Y);
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (!_dragging) return;
                int delta = e.Y - _dragStartY;
                int trackH = Height;
                var (_, thumbH) = CalcThumb();
                int newOffset = _dragStartOffset + (int)((float)delta / (trackH - thumbH) * (_totalH - _viewH));
                _offset = Math.Clamp(newOffset, 0, _totalH - _viewH);
                Scroll?.Invoke(newOffset - _dragStartOffset);
                Invalidate();
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                _dragging = false;
                Capture = false;
                Invalidate();
            }

            private void JumpTo(int y)
            {
                int trackH = Height;
                var (_, thumbH) = CalcThumb();
                int newOffset = (int)((float)(y - thumbH / 2) / (trackH - thumbH) * (_totalH - _viewH));
                newOffset = Math.Clamp(newOffset, 0, _totalH - _viewH);
                int delta = newOffset - _offset;
                _offset = newOffset;
                Scroll?.Invoke(delta);
                Invalidate();
            }

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
        }

        // ──────────────────────────────────────────────────────────────────────
        //  TagChip  –  single custom-drawn tag pill
        // ──────────────────────────────────────────────────────────────────────
        private sealed class TagChip : Control
        {
            public Color ChipBg { get; set; } = Color.FromArgb(0xED, 0xE9, 0xFE);
            public Color ChipFg { get; set; } = Color.FromArgb(0x5B, 0x21, 0xB6);
            public Font? ChipFont { get; set; }
            public int ChipRadius { get; set; } = 16;

            public TagChip()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (var path = RoundedRect(rect, ChipRadius))
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
        }

        // ──────────────────────────────────────────────────────────────────────
        //  SectionLabel  –  custom-painted section header
        // ──────────────────────────────────────────────────────────────────────
        private sealed class SectionLabel : Control
        {
            public Font? LabelFont { get; set; }

            public SectionLabel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                };

                using var fnt = LabelFont != null
                    ? new Font(LabelFont, LabelFont.Style)
                    : new Font("Segoe UI", 8f, FontStyle.Bold);
                using var b = new SolidBrush(ForeColor);
                g.DrawString(Text, fnt, b,
                    new RectangleF(20, 0, Width - 40, Height), sf);
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  OnlineStatus enum  (public so callers & Designer can reference it)
    // ══════════════════════════════════════════════════════════════════════════
    public enum OnlineStatus
    {
        [Description("متصل")] Online = 0,
        [Description("غائب")] Away = 1,
        [Description("غير متصل")] Offline = 2,
    }
}