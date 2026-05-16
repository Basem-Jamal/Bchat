using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Custom_Controal.Custom_Bchat
{
    // ═══════════════════════════════════════════════
    //  ENUMS
    // ═══════════════════════════════════════════════

    public enum ProCellType
    {
        Text, Badge, Avatar, AvatarText, Currency,
        Actions, Boolean, Progress, Rating, Tag, Image, Number, Date, Custom
    }

    public enum ProSortDirection { None, Ascending, Descending }
    public enum ProTableStyle { Flat, Card, Glass, Bordered, Minimal, Striped }
    public enum ProHeaderStyle { Solid, Gradient, Underline }
    public enum ProSelectionMode { None, Single, Multi }
    public enum ProAnimationSpeed { Off, Fast, Normal, Slow }
    public enum ProPaginationStyle { None, Bottom, Top, Both }

    // ═══════════════════════════════════════════════
    //  COLUMN DEFINITION
    // ═══════════════════════════════════════════════

    public class ProColumn
    {
        public string Header { get; set; } = "";
        public string Field { get; set; } = "";
        public int Width { get; set; } = 120;
        public int MinWidth { get; set; } = 60;
        public ProCellType CellType { get; set; } = ProCellType.Text;
        public bool Sortable { get; set; } = true;
        public bool Visible { get; set; } = true;
        public bool Resizable { get; set; } = true;
        public StringAlignment TextAlign { get; set; } = StringAlignment.Center;
        public string Format { get; set; } = "";
        public string Prefix { get; set; } = "";
        public string Suffix { get; set; } = "";
        public Color? CustomColor { get; set; }
        public Image CustomIcon { get; set; }
        public Func<object, string> ValueFormatter { get; set; }
    }

    // ═══════════════════════════════════════════════
    //  BADGE / ACTION / EVENT ARGS
    // ═══════════════════════════════════════════════

    public class ProBadge
    {
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public Color? BorderColor { get; set; }
        public ProBadge(Color bg, Color fg, Color? border = null)
        { Background = bg; Foreground = fg; BorderColor = border; }
    }

    public class ProAction
    {
        public string Key { get; set; }
        public Image Icon { get; set; }
        public Color Color { get; set; } = Color.FromArgb(108, 117, 145);
        public string Tooltip { get; set; } = "";
        public bool Visible { get; set; } = true;
    }

    public class ProRowEventArgs : EventArgs
    {
        public int RowIndex { get; }
        public Dictionary<string, object> Row { get; }
        public ProRowEventArgs(int idx, Dictionary<string, object> row) { RowIndex = idx; Row = row; }
    }

    public class ProActionEventArgs : EventArgs
    {
        public string ActionKey { get; }
        public int RowIndex { get; }
        public Dictionary<string, object> Row { get; }
        public ProActionEventArgs(string key, int idx, Dictionary<string, object> row)
        { ActionKey = key; RowIndex = idx; Row = row; }
    }

    public class ProSortEventArgs : EventArgs
    {
        public string Field { get; }
        public ProSortDirection Direction { get; }
        public ProSortEventArgs(string field, ProSortDirection dir) { Field = field; Direction = dir; }
    }

    // ═══════════════════════════════════════════════
    //  PRO TABLE CONTROL
    // ═══════════════════════════════════════════════

    [ToolboxItem(true)]
    [Description("جدول بيانات احترافي متكامل")]
    public class ProTable : UserControl
    {
        // ── Data ──────────────────────────────────────────────────────────────
        private List<ProColumn> _columns = new();
        private List<Dictionary<string, object>> _allRows = new();
        private List<Dictionary<string, object>> _filteredRows = new();
        private List<Dictionary<string, object>> _pageRows = new();
        private List<ProAction> _actions = new();
        private HashSet<int> _selected = new();

        // ── Column cache (rebuild only on SetColumns / visibility change) ─────
        private List<ProColumn> _visibleCols = new();
        private int[] _computedWidths;
        private int[] _colX;           // pre-computed screen X for each visible col

        // ── State ─────────────────────────────────────────────────────────────
        private string _sortField = null;
        private ProSortDirection _sortDir = ProSortDirection.None;
        private int _currentPage = 0;
        private int _pageSize = 20;
        private int _rowHeight = 56;
        private int _headerHeight = 52;
        private int _scrollOffset = 0;
        private int _hoverRow = -1;
        private string _hoverActionKey = null;
        private int _resizingCol = -1;
        private int _resizeStartX, _resizeStartW;
        private bool _isResizing = false;
        private string _searchText = "";

        // ── GDI Resource cache ────────────────────────────────────────────────
        // Rebuilt only when font/color properties change → zero alloc on normal paint
        private Font _cachedHFont;
        private Font _cachedRFont;
        private Font _cachedBadgeFont;
        private SolidBrush _hFgBrush;
        private SolidBrush _rTextBrush;
        private Pen _sepPen;
        private Pen _borderPen;
        private bool _resourcesDirty = true;

        // Instance StringFormat cache (not static → safe to Dispose)
        private readonly StringFormat _sfC = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
        private readonly StringFormat _sfN = new() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
        private readonly StringFormat _sfF = new() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

        // ── Animation ─────────────────────────────────────────────────────────
        private float _hoverAlpha = 0f;
        private int _animHoverRow = -1;
        private Timer _animTimer;

        // ── Shadow & background cache ─────────────────────────────────────────
        private Bitmap _shadowBmp;
        private Size _shadowBmpSize;
        private Bitmap _parentBgBmp;       // cached parent background — avoids per-frame InvokePaint
        private bool _parentBgDirty = true;

        // ── Controls ─────────────────────────────────────────────────────────
        private Panel _paginationPanel;
        private Button _btnFirst, _btnPrev, _btnNext, _btnLast;
        private Label _lblPageInfo;
        private ComboBox _cmbPageSize;
        private VScrollBar _vScroll;
        private TextBox _searchBox;
        private ToolTip _tt = new();
        private string _lastTip = "";

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Style
        // ══════════════════════════════════════════════════════════════════════

        private ProTableStyle _tableStyle = ProTableStyle.Flat;
        [Category("✦ Style")]
        [DefaultValue(ProTableStyle.Flat)]
        public ProTableStyle TableStyle
        { get => _tableStyle; set { _tableStyle = value; ApplyStyle(); Invalidate(); } }

        private ProHeaderStyle _headerStyle = ProHeaderStyle.Gradient;
        [Category("✦ Style")]
        [DefaultValue(ProHeaderStyle.Gradient)]
        public ProHeaderStyle HeaderStyle
        { get => _headerStyle; set { _headerStyle = value; Invalidate(); } }

        private ProSelectionMode _selectionMode = ProSelectionMode.Single;
        [Category("✦ Style")]
        [DefaultValue(ProSelectionMode.Single)]
        public ProSelectionMode SelectionMode
        { get => _selectionMode; set { _selectionMode = value; _selected.Clear(); Invalidate(); } }

        private ProAnimationSpeed _animSpeed = ProAnimationSpeed.Normal;
        [Category("✦ Style")]
        [DefaultValue(ProAnimationSpeed.Normal)]
        public ProAnimationSpeed AnimationSpeed
        {
            get => _animSpeed;
            set
            {
                _animSpeed = value;
                _animTimer.Interval = value switch
                {
                    ProAnimationSpeed.Fast => 10,
                    ProAnimationSpeed.Normal => 16,
                    ProAnimationSpeed.Slow => 32,
                    _ => 9999
                };
            }
        }

        private ProPaginationStyle _paginStyle = ProPaginationStyle.Bottom;
        [Category("✦ Style")]
        [DefaultValue(ProPaginationStyle.Bottom)]
        public ProPaginationStyle PaginationStyle
        { get => _paginStyle; set { _paginStyle = value; BuildLayout(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Colors (modern defaults)
        // ══════════════════════════════════════════════════════════════════════

        private Color _headerBg = Color.FromArgb(17, 24, 48);
        [Category("✦ Colors")]
        public Color HeaderBackground
        { get => _headerBg; set { _headerBg = value; MarkResourcesDirty(); } }

        private Color _headerBg2 = Color.FromArgb(36, 58, 115);
        [Category("✦ Colors")]
        [Description("اللون الثاني للـ Gradient")]
        public Color HeaderBackground2
        { get => _headerBg2; set { _headerBg2 = value; Invalidate(); } }

        private Color _headerFg = Color.FromArgb(235, 241, 255);
        [Category("✦ Colors")]
        public Color HeaderForeground
        { get => _headerFg; set { _headerFg = value; MarkResourcesDirty(); } }

        private Color _rowEven = Color.White;
        [Category("✦ Colors")]
        public Color RowEven
        { get => _rowEven; set { _rowEven = value; Invalidate(); } }

        private Color _rowOdd = Color.FromArgb(249, 251, 255);
        [Category("✦ Colors")]
        public Color RowOdd
        { get => _rowOdd; set { _rowOdd = value; Invalidate(); } }

        private Color _rowSel = Color.FromArgb(215, 233, 255);
        [Category("✦ Colors")]
        public Color RowSelected
        { get => _rowSel; set { _rowSel = value; Invalidate(); } }

        private Color _rowHover = Color.FromArgb(232, 242, 255);
        [Category("✦ Colors")]
        public Color RowHover
        { get => _rowHover; set { _rowHover = value; Invalidate(); } }

        private Color _rowText = Color.FromArgb(28, 35, 58);
        [Category("✦ Colors")]
        public Color RowTextColor
        { get => _rowText; set { _rowText = value; MarkResourcesDirty(); } }

        private Color _sepColor = Color.FromArgb(233, 238, 250);
        [Category("✦ Colors")]
        public Color SeparatorColor
        { get => _sepColor; set { _sepColor = value; MarkResourcesDirty(); } }

        private Color _outerBorderColor = Color.FromArgb(210, 220, 245);
        [Category("✦ Colors")]
        public Color OuterBorderColor
        { get => _outerBorderColor; set { _outerBorderColor = value; MarkResourcesDirty(); } }

        private Color _accent = Color.FromArgb(79, 130, 247);
        [Category("✦ Colors")]
        [Description("لون التمييز — Sort / Pagination / Accent bar")]
        public Color AccentColor
        { get => _accent; set { _accent = value; ApplyPaginationColors(); Invalidate(); } }

        private Color _sortArrow = Color.FromArgb(190, 215, 255);
        [Category("✦ Colors")]
        public Color SortArrowColor
        { get => _sortArrow; set { _sortArrow = value; Invalidate(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Shape
        // ══════════════════════════════════════════════════════════════════════

        private int _borderRadius = 16;
        [Category("✦ Shape")]
        [DefaultValue(16)]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); RebuildShadow(); Invalidate(); } }

        private int _shadowDepth = 10;
        [Category("✦ Shape")]
        [DefaultValue(10)]
        public int ShadowDepth
        { get => _shadowDepth; set { _shadowDepth = Math.Clamp(value, 0, 40); RebuildShadow(); Invalidate(); } }

        private Color _shadowColor = Color.FromArgb(40, 0, 20, 70);
        [Category("✦ Shape")]
        public Color ShadowColor
        { get => _shadowColor; set { _shadowColor = value; RebuildShadow(); Invalidate(); } }

        private bool _showOuterBorder = true;
        [Category("✦ Shape")]
        [DefaultValue(true)]
        public bool ShowOuterBorder
        { get => _showOuterBorder; set { _showOuterBorder = value; Invalidate(); } }

        private int _rowBorderRadius = 0;
        [Category("✦ Shape")]
        [DefaultValue(0)]
        public int RowBorderRadius
        { get => _rowBorderRadius; set { _rowBorderRadius = Math.Max(0, value); Invalidate(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Rows
        // ══════════════════════════════════════════════════════════════════════

        private int _rowHeightProp = 56;
        [Category("✦ Rows")]
        [DefaultValue(56)]
        public int RowHeight
        { get => _rowHeightProp; set { _rowHeightProp = Math.Max(28, value); _rowHeight = _rowHeightProp; UpdateScrollbar(); Invalidate(); } }

        private bool _showRowNumber = false;
        [Category("✦ Rows")]
        [DefaultValue(false)]
        public bool ShowRowNumber
        { get => _showRowNumber; set { _showRowNumber = value; RebuildColCache(); Invalidate(); } }

        private bool _showRowSep = true;
        [Category("✦ Rows")]
        [DefaultValue(true)]
        public bool ShowRowSeparator
        { get => _showRowSep; set { _showRowSep = value; Invalidate(); } }

        private bool _showHover = true;
        [Category("✦ Rows")]
        [DefaultValue(true)]
        public bool ShowHoverEffect
        { get => _showHover; set { _showHover = value; Invalidate(); } }

        private bool _altRows = true;
        [Category("✦ Rows")]
        [DefaultValue(true)]
        public bool AlternateRowColors
        { get => _altRows; set { _altRows = value; Invalidate(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Features
        // ══════════════════════════════════════════════════════════════════════

        private bool _showSearch = false;
        [Category("✦ Features")]
        [DefaultValue(false)]
        public bool ShowSearch
        { get => _showSearch; set { _showSearch = value; BuildLayout(); } }

        private string _searchPlaceholder = "بحث...";
        [Category("✦ Features")]
        public string SearchPlaceholder
        { get => _searchPlaceholder; set { _searchPlaceholder = value; if (_searchBox != null) _searchBox.PlaceholderText = value; } }

        private bool _allowSort = true;
        [Category("✦ Features")]
        [DefaultValue(true)]
        public bool AllowSort
        { get => _allowSort; set { _allowSort = value; Invalidate(); } }

        private bool _allowColResize = true;
        [Category("✦ Features")]
        [DefaultValue(true)]
        public bool AllowColumnResize
        { get => _allowColResize; set { _allowColResize = value; } }

        private bool _showSortIndicator = true;
        [Category("✦ Features")]
        [DefaultValue(true)]
        public bool ShowColumnSortIndicator
        { get => _showSortIndicator; set { _showSortIndicator = value; Invalidate(); } }

        [Category("✦ Features")]
        [DefaultValue(20)]
        public int PageSize
        { get => _pageSize; set { _pageSize = Math.Max(1, value); _currentPage = 0; ApplyPage(); Invalidate(); } }

        private string _emptyText = "لا توجد بيانات للعرض";
        [Category("✦ Features")]
        public string EmptyText
        { get => _emptyText; set { _emptyText = value; Invalidate(); } }

        private Image _emptyIcon = null;
        [Category("✦ Features")]
        public Image EmptyIcon
        { get => _emptyIcon; set { _emptyIcon = value; Invalidate(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  PROPERTIES — Layout
        // ══════════════════════════════════════════════════════════════════════

        private bool _isRtl = false;
        [Category("✦ Layout")]
        [DefaultValue(false)]
        public bool IsRtl
        { get => _isRtl; set { _isRtl = value; RebuildColCache(); Invalidate(); } }

        private Font _headerFont;
        [Category("✦ Layout")]
        public Font HeaderFont
        { get => _headerFont ?? new Font(Font.FontFamily, Font.Size + 1f, FontStyle.Bold); set { _headerFont = value; MarkResourcesDirty(); } }

        private Font _rowFont;
        [Category("✦ Layout")]
        public Font RowFont
        { get => _rowFont ?? Font; set { _rowFont = value; MarkResourcesDirty(); } }

        /// <summary>
        /// عندما يكون false، يُستخدم BackColor مباشرة بدلاً من إعادة رسم الـ Parent.
        /// اضبطه على false عندما لا تحتاج لخلفية شفافة لتحسين الأداء بشكل ملحوظ.
        /// </summary>
        private bool _transparentBg = true;
        [Category("✦ Layout")]
        [DefaultValue(true)]
        public bool UseTransparentBackground
        { get => _transparentBg; set { _transparentBg = value; _parentBgDirty = true; Invalidate(); } }

        // ══════════════════════════════════════════════════════════════════════
        //  EVENTS
        // ══════════════════════════════════════════════════════════════════════

        public event EventHandler<ProRowEventArgs> RowClicked;
        public event EventHandler<ProRowEventArgs> RowDoubleClicked;
        public event EventHandler<ProActionEventArgs> ActionClicked;
        public event EventHandler<ProSortEventArgs> SortChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler PageChanged;

        // ══════════════════════════════════════════════════════════════════════
        //  BADGE REGISTRY  (Tailwind 500-level palette)
        // ══════════════════════════════════════════════════════════════════════

        private Dictionary<string, ProBadge> _badges = new(StringComparer.OrdinalIgnoreCase)
        {
            { "متاح",        new(Color.FromArgb(16,  185, 129), Color.White) },
            { "Available",   new(Color.FromArgb(16,  185, 129), Color.White) },
            { "Active",      new(Color.FromArgb(16,  185, 129), Color.White) },
            { "نشط",         new(Color.FromArgb(16,  185, 129), Color.White) },
            { "مؤجرة",       new(Color.FromArgb(59,  130, 246), Color.White) },
            { "Rented",      new(Color.FromArgb(59,  130, 246), Color.White) },
            { "Pending",     new(Color.FromArgb(245, 158,  11), Color.White) },
            { "معلق",        new(Color.FromArgb(245, 158,  11), Color.White) },
            { "صيانة",       new(Color.FromArgb(249, 115,  22), Color.White) },
            { "Maintenance", new(Color.FromArgb(249, 115,  22), Color.White) },
            { "Cancelled",   new(Color.FromArgb(239,  68,  68), Color.White) },
            { "ملغي",        new(Color.FromArgb(239,  68,  68), Color.White) },
            { "Inactive",    new(Color.FromArgb(148, 163, 184), Color.White) },
            { "غير نشط",    new(Color.FromArgb(148, 163, 184), Color.White) },
            { "Confirmed",   new(Color.FromArgb(20,  184, 166), Color.White) },
            { "مؤكد",        new(Color.FromArgb(20,  184, 166), Color.White) },
        };

        private readonly Color[] _palette =
        {
            Color.FromArgb(99,  102, 241), Color.FromArgb(236,  72, 153),
            Color.FromArgb(16,  185, 129), Color.FromArgb(245, 158,  11),
            Color.FromArgb(239,  68,  68), Color.FromArgb(20,  184, 166),
            Color.FromArgb(59,  130, 246), Color.FromArgb(139,  92, 246),
            Color.FromArgb(234, 179,   8), Color.FromArgb(249, 115,  22),
        };

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ══════════════════════════════════════════════════════════════════════

        public ProTable()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10f);

            _animTimer = new Timer { Interval = 16 };
            _animTimer.Tick += AnimTick;

            BuildLayout();

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            MouseClick += OnMouseClick;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;

            LocationChanged += (s, e) => { _parentBgDirty = true; Invalidate(); };
            ParentChanged += (s, e) => { _parentBgDirty = true; };
            Resize += (s, e) =>
            {
                RebuildColCache();
                UpdateScrollbar();
                RebuildShadow();
                _parentBgDirty = true;
                Invalidate();
            };
        }

        // ══════════════════════════════════════════════════════════════════════
        //  GDI RESOURCE MANAGEMENT
        // ══════════════════════════════════════════════════════════════════════

        private void MarkResourcesDirty()
        {
            _resourcesDirty = true;
            Invalidate();
        }

        private void EnsureResources()
        {
            if (!_resourcesDirty) return;

            DisposeGdiResources();

            _cachedHFont = new Font(_headerFont?.FontFamily ?? Font.FontFamily,
                                       _headerFont?.Size ?? Font.Size + 1f, FontStyle.Bold);
            _cachedRFont = _rowFont != null ? (Font)_rowFont.Clone() : (Font)Font.Clone();
            _cachedBadgeFont = new Font(_cachedRFont.FontFamily, Math.Max(7f, _cachedRFont.Size - 1f), FontStyle.Bold);
            _hFgBrush = new SolidBrush(_headerFg);
            _rTextBrush = new SolidBrush(_rowText);
            _sepPen = new Pen(_sepColor, 1f);
            _borderPen = new Pen(_outerBorderColor, 1f);
            _resourcesDirty = false;
        }

        private void DisposeGdiResources()
        {
            _cachedHFont?.Dispose(); _cachedHFont = null;
            _cachedRFont?.Dispose(); _cachedRFont = null;
            _cachedBadgeFont?.Dispose(); _cachedBadgeFont = null;
            _hFgBrush?.Dispose(); _hFgBrush = null;
            _rTextBrush?.Dispose(); _rTextBrush = null;
            _sepPen?.Dispose(); _sepPen = null;
            _borderPen?.Dispose(); _borderPen = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  PUBLIC API
        // ══════════════════════════════════════════════════════════════════════

        public void SetColumns(List<ProColumn> columns)
        {
            _columns = columns ?? new();
            RebuildColCache();
            Invalidate();
        }

        public void SetData(List<Dictionary<string, object>> data)
        {
            _allRows = data ?? new();
            _currentPage = 0;
            _selected.Clear();
            _hoverRow = -1;
            _sortField = null;
            _sortDir = ProSortDirection.None;
            ApplyFilterAndSort();
        }

        public void SetActions(List<ProAction> actions) { _actions = actions ?? new(); Invalidate(); }
        public void RegisterBadge(string key, Color bg, Color fg, Color? border = null)
            => _badges[key] = new ProBadge(bg, fg, border);

        public void Search(string text)
        { _searchText = text?.ToLower() ?? ""; _currentPage = 0; ApplyFilterAndSort(); }

        public void SortBy(string field, ProSortDirection dir = ProSortDirection.Ascending)
        { _sortField = field; _sortDir = dir; ApplyFilterAndSort(); }

        public void GoToPage(int page)
        {
            _currentPage = Math.Clamp(page, 0, Math.Max(0, GetTotalPages() - 1));
            ApplyPage(); UpdatePaginationControls(); Invalidate();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<Dictionary<string, object>> GetSelectedRows()
            => _selected.Where(i => i >= 0 && i < _filteredRows.Count).Select(i => _filteredRows[i]).ToList();

        public Dictionary<string, object> GetFirstSelectedRow()
        {
            int idx = _selected.FirstOrDefault(-1);
            return idx >= 0 && idx < _filteredRows.Count ? _filteredRows[idx] : null;
        }

        public void ClearSelection() { _selected.Clear(); Invalidate(); SelectionChanged?.Invoke(this, EventArgs.Empty); }
        public void RefreshData() => ApplyFilterAndSort();

        // ══════════════════════════════════════════════════════════════════════
        //  LAYOUT BUILD
        // ══════════════════════════════════════════════════════════════════════

        private void BuildLayout()
        {
            Controls.Clear();

            _vScroll = new VScrollBar
            {
                Dock = DockStyle.Right,
                SmallChange = _rowHeight,
                LargeChange = _rowHeight * 5,
                Visible = false
            };
            _vScroll.Scroll += (s, e) => { _scrollOffset = _vScroll.Value; Invalidate(); };
            Controls.Add(_vScroll);

            if (_showSearch)
            {
                _searchBox = new TextBox
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    Font = new Font("Segoe UI", 10f),
                    BorderStyle = BorderStyle.FixedSingle,
                    PlaceholderText = _searchPlaceholder,
                    RightToLeft = _isRtl ? RightToLeft.Yes : RightToLeft.No
                };
                _searchBox.TextChanged += (s, e) => Search(_searchBox.Text);
                Controls.Add(_searchBox);
            }

            if (_paginStyle != ProPaginationStyle.None)
            {
                _paginationPanel = new Panel
                {
                    Height = 48,
                    Dock = DockStyle.Bottom,
                    BackColor = Color.Transparent
                };
                BuildPaginationControls();
                Controls.Add(_paginationPanel);
            }

            Invalidate();
        }

        private void BuildPaginationControls()
        {
            _paginationPanel.Controls.Clear();

            void Style(Button b)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = Color.FromArgb(210, 220, 248);
                b.ForeColor = _accent;
                b.BackColor = Color.White;
                b.Size = new Size(34, 30);
                b.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                b.Cursor = Cursors.Hand;
                b.TabStop = false;
            }

            _btnFirst = new Button { Text = "«" }; Style(_btnFirst);
            _btnPrev = new Button { Text = "‹" }; Style(_btnPrev);
            _btnNext = new Button { Text = "›" }; Style(_btnNext);
            _btnLast = new Button { Text = "»" }; Style(_btnLast);

            _lblPageInfo = new Label
            {
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 110, 140),
                Padding = new Padding(4, 5, 4, 0)
            };

            _cmbPageSize = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 72,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat
            };
            _cmbPageSize.Items.AddRange(new object[] { 10, 20, 50, 100 });
            _cmbPageSize.SelectedItem = _pageSize;
            _cmbPageSize.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbPageSize.SelectedItem != null)
                { _pageSize = (int)_cmbPageSize.SelectedItem; _currentPage = 0; ApplyPage(); UpdatePaginationControls(); Invalidate(); }
            };

            _btnFirst.Click += (s, e) => GoToPage(0);
            _btnPrev.Click += (s, e) => GoToPage(_currentPage - 1);
            _btnNext.Click += (s, e) => GoToPage(_currentPage + 1);
            _btnLast.Click += (s, e) => GoToPage(GetTotalPages() - 1);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(8, 8, 8, 0),
                BackColor = Color.Transparent
            };

            var rowsLbl = new Label
            {
                Text = "الصفوف:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(120, 130, 160),
                Padding = new Padding(0, 6, 4, 0)
            };

            flow.Controls.AddRange(new Control[]
            { _btnFirst, _btnPrev, _lblPageInfo, _btnNext, _btnLast, rowsLbl, _cmbPageSize });
            _paginationPanel.Controls.Add(flow);
        }

        private void ApplyPaginationColors()
        {
            if (_btnFirst == null) return;
            foreach (var b in new[] { _btnFirst, _btnPrev, _btnNext, _btnLast })
                b.ForeColor = _accent;
        }

        private void ApplyStyle()
        {
            switch (_tableStyle)
            {
                case ProTableStyle.Minimal:
                    _showOuterBorder = false; _shadowDepth = 0;
                    _sepColor = Color.FromArgb(230, 235, 248);
                    break;
                case ProTableStyle.Bordered:
                    _showOuterBorder = true; _shadowDepth = 0;
                    break;
                case ProTableStyle.Glass:
                    _rowEven = Color.FromArgb(18, 255, 255, 255);
                    _rowOdd = Color.FromArgb(8, 255, 255, 255);
                    break;
            }
            MarkResourcesDirty();
            RebuildShadow();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  COLUMN CACHE  ← كل العمليات الثقيلة هنا فقط، مش في OnPaint
        // ══════════════════════════════════════════════════════════════════════

        private void RebuildColCache()
        {
            _visibleCols = _columns.Where(c => c.Visible).ToList();
            ComputeWidths();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DATA PIPELINE
        // ══════════════════════════════════════════════════════════════════════

        private void ApplyFilterAndSort()
        {
            _filteredRows = string.IsNullOrEmpty(_searchText)
                ? new List<Dictionary<string, object>>(_allRows)
                : _allRows.Where(r => r.Values.Any(v => v?.ToString().ToLower().Contains(_searchText) == true)).ToList();

            if (!string.IsNullOrEmpty(_sortField) && _sortDir != ProSortDirection.None)
            {
                _filteredRows = _sortDir == ProSortDirection.Ascending
                    ? _filteredRows.OrderBy(r => GetSortKey(r, _sortField)).ToList()
                    : _filteredRows.OrderByDescending(r => GetSortKey(r, _sortField)).ToList();
            }

            ApplyPage();
            UpdatePaginationControls();
            UpdateScrollbar();
            Invalidate();
        }

        private object GetSortKey(Dictionary<string, object> row, string field)
        {
            if (!row.TryGetValue(field, out var v)) return "";
            if (v is int i) return i;
            if (v is double d) return d;
            if (v is DateTime dt) return dt;
            if (double.TryParse(v?.ToString(), out double n)) return n;
            return v?.ToString() ?? "";
        }

        private void ApplyPage()
        {
            if (_paginStyle == ProPaginationStyle.None)
            { _pageRows = _filteredRows; return; }
            int start = _currentPage * _pageSize;
            _pageRows = _filteredRows.Skip(start).Take(_pageSize).ToList();
        }

        private int GetTotalPages()
            => _pageSize <= 0 ? 1 : (int)Math.Ceiling(_filteredRows.Count / (double)_pageSize);

        private void UpdatePaginationControls()
        {
            if (_lblPageInfo == null) return;
            int total = GetTotalPages();
            int start = _currentPage * _pageSize + 1;
            int end = Math.Min(start + _pageSize - 1, _filteredRows.Count);
            _lblPageInfo.Text = $"  {start}–{end} من {_filteredRows.Count}  (صفحة {_currentPage + 1}/{total})  ";
            if (_btnFirst != null)
            {
                _btnFirst.Enabled = _btnPrev.Enabled = _currentPage > 0;
                _btnNext.Enabled = _btnLast.Enabled = _currentPage < total - 1;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SHADOW  (Gaussian-like falloff, cached bitmap)
        // ══════════════════════════════════════════════════════════════════════

        private void RebuildShadow()
        {
            _shadowBmp?.Dispose(); _shadowBmp = null;
            if (_shadowDepth <= 0 || Width <= 0 || Height <= 0) return;

            int sd = _shadowDepth;
            var bmp = new Bitmap(Width + sd * 2, Height + sd * 2);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = sd; i >= 1; i--)
            {
                float t = 1f - (float)i / sd;
                int alpha = (int)(_shadowColor.A * t * t * 0.5f);
                alpha = Math.Clamp(alpha, 0, 255);
                if (alpha == 0) continue;

                var sr = new Rectangle(sd - i, sd - i + 3, Width - 1 + i * 2, Height - 1 + i * 2);
                using var sp = RoundedRect(sr, _borderRadius + i);
                using var sb = new SolidBrush(Color.FromArgb(alpha, _shadowColor));
                g.FillPath(sb, sp);
            }

            _shadowBmp = bmp;
            _shadowBmpSize = Size;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  COMPUTE COLUMN WIDTHS + X POSITIONS  ← single call, used by paint
        // ══════════════════════════════════════════════════════════════════════

        private void ComputeWidths()
        {
            int count = _visibleCols.Count;
            if (count == 0)
            { _computedWidths = Array.Empty<int>(); _colX = Array.Empty<int>(); return; }

            int scrollW = _vScroll?.Visible == true ? _vScroll.Width : 0;
            int rowNumW = _showRowNumber ? 44 : 0;
            int available = Math.Max(0, Width - scrollW - _shadowDepth * 2 - rowNumW);
            int total = _visibleCols.Sum(c => c.Width);

            _computedWidths = new int[count];
            if (available <= total || total == 0)
                for (int i = 0; i < count; i++) _computedWidths[i] = _visibleCols[i].Width;
            else
            {
                int extra = available - total, dist = 0;
                for (int i = 0; i < count; i++)
                {
                    int bonus = i < count - 1
                        ? (int)(_visibleCols[i].Width / (double)total * extra)
                        : extra - dist;
                    _computedWidths[i] = _visibleCols[i].Width + bonus;
                    dist += bonus;
                }
            }

            // Pre-compute screen X for each column (respects RTL order)
            _colX = new int[count];
            int x = _shadowDepth + rowNumW;
            foreach (int i in Indices(count))
            { _colX[i] = x; x += _computedWidths[i]; }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SCROLLBAR
        // ══════════════════════════════════════════════════════════════════════

        private void UpdateScrollbar()
        {
            int totalH = _pageRows.Count * _rowHeight;
            int visH = PaintAreaHeight();

            if (totalH > visH && _paginStyle == ProPaginationStyle.None)
            {
                _vScroll.Visible = true;
                _vScroll.Maximum = totalH - visH + _vScroll.LargeChange;
                _vScroll.Value = Math.Min(_scrollOffset, Math.Max(0, _vScroll.Maximum - _vScroll.LargeChange));
                ComputeWidths();
            }
            else
            {
                _vScroll.Visible = false;
                _scrollOffset = 0;
                ComputeWidths();
            }
        }

        private int PaintAreaHeight()
        {
            int ph = _paginationPanel?.Visible == true ? _paginationPanel.Height : 0;
            int sh = _searchBox?.Visible == true ? _searchBox.Height : 0;
            return Height - _headerHeight - _shadowDepth * 2 - ph - sh;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HIT TESTING
        // ══════════════════════════════════════════════════════════════════════

        private int BodyTop => _shadowDepth + _headerHeight
                             + (_searchBox?.Visible == true ? _searchBox.Height : 0);

        private int HitTestRow(int my)
        {
            int ry = my - BodyTop + _scrollOffset;
            if (ry < 0) return -1;
            int row = ry / _rowHeight;
            return row < _pageRows.Count ? row : -1;
        }

        private int HitTestColumn(int mx)
        {
            if (_colX == null) return -1;
            foreach (int i in Indices(_visibleCols.Count))
                if (mx >= _colX[i] && mx < _colX[i] + _computedWidths[i]) return i;
            return -1;
        }

        private int HitTestResizeHandle(int mx)
        {
            if (!_allowColResize || _colX == null) return -1;
            var idxList = Indices(_visibleCols.Count).ToList();
            for (int j = 0; j < idxList.Count - 1; j++)
            {
                int i = idxList[j];
                if (Math.Abs(mx - (_colX[i] + _computedWidths[i])) <= 4) return i;
            }
            return -1;
        }

        private (int row, string key) HitTestAction(int mx, int my)
        {
            int row = HitTestRow(my);
            if (row < 0 || _actions.Count == 0 || _colX == null) return (-1, null);

            for (int ci = 0; ci < _visibleCols.Count; ci++)
            {
                if (_visibleCols[ci].CellType != ProCellType.Actions) continue;
                var va = _actions.Where(a => a.Visible).ToList();
                if (va.Count == 0) break;

                int iSz = 28, iSp = 6;
                int tw = va.Count * iSz + (va.Count - 1) * iSp;
                int sx = _colX[ci] + (_computedWidths[ci] - tw) / 2;
                int ry = BodyTop + row * _rowHeight - _scrollOffset;
                int iy = ry + (_rowHeight - iSz) / 2;

                for (int i = 0; i < va.Count; i++)
                {
                    var r = new Rectangle(sx + i * (iSz + iSp), iy, iSz, iSz);
                    if (r.Contains(mx, my)) return (row, va[i].Key);
                }
                return (row, null);
            }
            return (-1, null);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ANIMATION  ← per-row Invalidate (no full repaint)
        // ══════════════════════════════════════════════════════════════════════

        private void AnimTick(object s, EventArgs e)
        {
            if (_animSpeed == ProAnimationSpeed.Off) { _animTimer.Stop(); return; }

            float prev = _hoverAlpha;
            float step = 0.11f;
            bool enter = _animHoverRow == _hoverRow && _hoverRow >= 0;
            _hoverAlpha = enter ? Math.Min(1f, _hoverAlpha + step) : Math.Max(0f, _hoverAlpha - step);

            if (Math.Abs(_hoverAlpha - prev) < 0.005f) { _animTimer.Stop(); return; }

            int dirtyRow = _hoverRow >= 0 ? _hoverRow : _animHoverRow;
            InvalidateRow(dirtyRow);
            if (_animHoverRow != _hoverRow && _animHoverRow >= 0) InvalidateRow(_animHoverRow);
        }

        /// <summary>يعيد رسم صف واحد فقط بدل الـ Control كاملة</summary>
        private void InvalidateRow(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= _pageRows.Count) { Invalidate(); return; }
            int y = BodyTop + rowIdx * _rowHeight - _scrollOffset;
            Invalidate(new Rectangle(_shadowDepth, y, Width - _shadowDepth * 2, _rowHeight + 1));
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MOUSE EVENTS
        // ══════════════════════════════════════════════════════════════════════

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isResizing)
            {
                int delta = e.X - _resizeStartX;
                _visibleCols[_resizingCol].Width =
                    Math.Max(_visibleCols[_resizingCol].MinWidth, _resizeStartW + delta);
                ComputeWidths(); Invalidate(); return;
            }

            if (HitTestResizeHandle(e.X) >= 0 && e.Y < BodyTop) { Cursor = Cursors.SizeWE; return; }

            int row = HitTestRow(e.Y);
            var (_, key) = HitTestAction(e.X, e.Y);
            bool changed = row != _hoverRow || key != _hoverActionKey;

            if (changed && _animSpeed != ProAnimationSpeed.Off)
            { _animHoverRow = row; _animTimer.Start(); }

            _hoverRow = row;
            _hoverActionKey = key;

            string tip = key != null ? _actions.FirstOrDefault(a => a.Key == key)?.Tooltip ?? "" : "";
            if (tip != _lastTip) { _tt.SetToolTip(this, tip); _lastTip = tip; }

            Cursor = row >= 0 || key != null ? Cursors.Hand : Cursors.Default;
            if (changed && _animSpeed == ProAnimationSpeed.Off) Invalidate();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            _hoverRow = -1; _hoverActionKey = null;
            if (_animSpeed != ProAnimationSpeed.Off) { _animHoverRow = -1; _animTimer.Start(); }
            else Invalidate();
            Cursor = Cursors.Default;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y < BodyTop)
            {
                if (_allowSort && e.Y > _shadowDepth)
                {
                    int ci = HitTestColumn(e.X);
                    if (ci >= 0 && ci < _visibleCols.Count && _visibleCols[ci].Sortable)
                    {
                        if (_sortField == _visibleCols[ci].Field)
                            _sortDir = _sortDir == ProSortDirection.Ascending
                                ? ProSortDirection.Descending : ProSortDirection.Ascending;
                        else { _sortField = _visibleCols[ci].Field; _sortDir = ProSortDirection.Ascending; }
                        ApplyFilterAndSort();
                        SortChanged?.Invoke(this, new ProSortEventArgs(_sortField, _sortDir));
                    }
                }
                int rc = HitTestResizeHandle(e.X);
                if (rc >= 0)
                { _isResizing = true; _resizingCol = rc; _resizeStartX = e.X; _resizeStartW = _visibleCols[rc].Width; }
                return;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e) { _isResizing = false; _resizingCol = -1; }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (_isResizing) return;
            var (aRow, aKey) = HitTestAction(e.X, e.Y);
            if (aRow >= 0 && aKey != null)
            { ActionClicked?.Invoke(this, new ProActionEventArgs(aKey, aRow, _pageRows[aRow])); return; }

            int row = HitTestRow(e.Y);
            if (row < 0) return;

            if (_selectionMode == ProSelectionMode.Single)
            { _selected.Clear(); _selected.Add(row); }
            else if (_selectionMode == ProSelectionMode.Multi)
            { if (!_selected.Remove(row)) _selected.Add(row); }

            Invalidate();
            RowClicked?.Invoke(this, new ProRowEventArgs(row, _pageRows[row]));
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            int row = HitTestRow(e.Y);
            if (row >= 0) RowDoubleClicked?.Invoke(this, new ProRowEventArgs(row, _pageRows[row]));
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int max = Math.Max(0, _pageRows.Count * _rowHeight - PaintAreaHeight());
            _scrollOffset = Math.Clamp(_scrollOffset - e.Delta / 3, 0, max);
            if (_vScroll.Visible)
                _vScroll.Value = Math.Min(_scrollOffset, _vScroll.Maximum - _vScroll.LargeChange + 1);
            Invalidate();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MAIN PAINT
        // ══════════════════════════════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
            EnsureResources();
            if (_computedWidths == null || _computedWidths.Length == 0) ComputeWidths();

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // ── 0. Background ──────────────────────────────────────────────
            PaintBackground(g);

            int sd = _shadowDepth;
            int scrollW = _vScroll?.Visible == true ? _vScroll.Width : 0;
            int searchH = _searchBox?.Visible == true ? _searchBox.Height : 0;
            int paginH = _paginationPanel?.Visible == true ? _paginationPanel.Height : 0;
            int gridW = Width - sd * 2 - scrollW;
            int gridH = Height - sd * 2 - paginH;
            var cardRect = new Rectangle(sd, sd + searchH, gridW - 1, gridH - 1 - searchH);

            // ── 1. Shadow ──────────────────────────────────────────────────
            if (sd > 0)
            {
                if (_shadowBmp == null || _shadowBmpSize != Size) RebuildShadow();
                if (_shadowBmp != null) g.DrawImage(_shadowBmp, -sd, -sd);
            }

            // ── 2. Card background ─────────────────────────────────────────
            using var cardPath = RoundedRect(cardRect, _borderRadius);
            g.SetClip(cardPath);
            using (var bgBrush = new SolidBrush(_rowEven))
                g.FillPath(bgBrush, cardPath);

            // ── 3. Header ──────────────────────────────────────────────────
            DrawHeader(g, cardRect, gridW, sd, searchH);

            // ── 4. Rows / Empty ─────────────────────────────────────────────
            var bodyRect = new Rectangle(sd, sd + searchH + _headerHeight, gridW, gridH - _headerHeight - searchH);
            g.SetClip(new Region(bodyRect), CombineMode.Intersect);

            if (_pageRows.Count == 0)
                DrawEmpty(g, bodyRect);
            else
                for (int i = 0; i < _pageRows.Count; i++)
                {
                    int y = sd + searchH + _headerHeight + i * _rowHeight - _scrollOffset;
                    if (y + _rowHeight < bodyRect.Top) continue;
                    if (y > bodyRect.Bottom) break;
                    DrawRow(g, i, y, sd, gridW);
                }

            // ── 5. Outer border ────────────────────────────────────────────
            g.ResetClip();
            g.SetClip(cardPath);
            if (_showOuterBorder) g.DrawPath(_borderPen, cardPath);
            g.ResetClip();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  BACKGROUND  (cached bitmap — مرة واحدة فقط)
        // ══════════════════════════════════════════════════════════════════════

        private void PaintBackground(Graphics g)
        {
            if (!_transparentBg)
            {
                // أسرع طريقة: fill بـ BackColor مباشرة
                g.Clear(BackColor == Color.Transparent ? Color.White : BackColor);
                return;
            }

            if (Parent == null) return;

            // أعد بناء cache فقط عند الحاجة
            if (_parentBgDirty || _parentBgBmp == null || _parentBgBmp.Size != Size)
            {
                _parentBgBmp?.Dispose();
                _parentBgBmp = null;
                if (Width > 0 && Height > 0)
                {
                    _parentBgBmp = new Bitmap(Width, Height);
                    using var bg = Graphics.FromImage(_parentBgBmp);
                    bg.TranslateTransform(-Left, -Top);
                    var pe = new PaintEventArgs(bg, new Rectangle(Left, Top, Width, Height));
                    InvokePaintBackground(Parent, pe);
                    InvokePaint(Parent, pe);
                    _parentBgDirty = false;
                }
            }
            if (_parentBgBmp != null) g.DrawImage(_parentBgBmp, 0, 0);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DRAW HEADER
        // ══════════════════════════════════════════════════════════════════════

        private void DrawHeader(Graphics g, Rectangle cardRect, int gridW, int sd, int searchH)
        {
            var hRect = new Rectangle(sd, sd + searchH, gridW, _headerHeight);

            // Rounded top only (matches card corners)
            using var hPath = new GraphicsPath();
            int d = Math.Min(_borderRadius * 2, Math.Min(hRect.Width, hRect.Height));
            if (d > 0)
            {
                hPath.AddArc(hRect.X, hRect.Y, d, d, 180, 90);
                hPath.AddArc(hRect.Right - d, hRect.Y, d, d, 270, 90);
            }
            else hPath.AddLine(hRect.X, hRect.Y, hRect.Right, hRect.Y);
            hPath.AddLine(hRect.Right, hRect.Bottom, hRect.X, hRect.Bottom);
            hPath.CloseFigure();

            switch (_headerStyle)
            {
                case ProHeaderStyle.Gradient:
                    using (var br = new LinearGradientBrush(hRect, _headerBg, _headerBg2, LinearGradientMode.Horizontal))
                        g.FillPath(br, hPath);
                    break;
                case ProHeaderStyle.Underline:
                    using (var br = new SolidBrush(_rowEven)) g.FillPath(br, hPath);
                    using (var up = new Pen(_headerBg, 3f))
                        g.DrawLine(up, sd, hRect.Bottom - 1, sd + gridW, hRect.Bottom - 1);
                    break;
                default:
                    using (var br = new SolidBrush(_headerBg)) g.FillPath(br, hPath);
                    break;
            }

            bool underline = _headerStyle == ProHeaderStyle.Underline;
            Color hfg = underline ? _headerBg : _headerFg;
            if (_visibleCols.Count == 0 || _colX == null) return;

            foreach (int i in Indices(_visibleCols.Count))
            {
                int w = _computedWidths[i];
                var col = _visibleCols[i];
                bool sorted = _allowSort && col.Sortable && _sortField == col.Field;
                int txtW = sorted ? w - 22 : w;
                var tRect = new Rectangle(_colX[i], sd + searchH, txtW, _headerHeight);

                using var hfgBrush = new SolidBrush(hfg);
                g.DrawString(col.Header, _cachedHFont, hfgBrush, tRect, _sfC);

                if (sorted && _showSortIndicator)
                    DrawSortArrow(g, _colX[i] + w - 18, sd + searchH + _headerHeight / 2 - 5,
                        _sortDir == ProSortDirection.Ascending,
                        underline ? _headerBg : _sortArrow);

                if (i < _visibleCols.Count - 1)
                {
                    using var sep = new Pen(Color.FromArgb(22, hfg), 1f);
                    g.DrawLine(sep, _colX[i] + w, sd + searchH + 10, _colX[i] + w, sd + searchH + _headerHeight - 10);
                }
            }

            using var hLine = new Pen(underline ? _headerBg : Color.FromArgb(28, _headerFg), 1f);
            g.DrawLine(hLine, sd, sd + searchH + _headerHeight, sd + gridW, sd + searchH + _headerHeight);
        }

        private void DrawSortArrow(Graphics g, int x, int y, bool asc, Color color)
        {
            using var br = new SolidBrush(color);
            Point[] pts = asc
                ? new[] { new Point(x, y + 8), new Point(x + 8, y + 8), new Point(x + 4, y) }
                : new[] { new Point(x, y), new Point(x + 8, y), new Point(x + 4, y + 8) };
            g.FillPolygon(br, pts);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DRAW ROW
        // ══════════════════════════════════════════════════════════════════════

        private void DrawRow(Graphics g, int ri, int y, int sd, int gridW)
        {
            bool sel = _selected.Contains(ri);
            bool hov = ri == _hoverRow && _showHover;

            Color baseBg = sel ? _rowSel
                : _altRows ? (ri % 2 == 0 ? _rowEven : _rowOdd) : _rowEven;

            Color bg = baseBg;
            if (hov && !sel)
            {
                float a = _animSpeed == ProAnimationSpeed.Off ? 1f : _hoverAlpha;
                bg = BlendColor(baseBg, _rowHover, a);
            }

            var rowRect = new Rectangle(sd, y, gridW, _rowHeight);

            if (_rowBorderRadius > 0 && _tableStyle == ProTableStyle.Card)
            {
                int p = 4;
                using var rPath = RoundedRect(new Rectangle(sd + p, y + 2, gridW - p * 2, _rowHeight - 4), _rowBorderRadius);
                using var rb = new SolidBrush(bg);
                g.FillPath(rb, rPath);
            }
            else
            {
                using var rb = new SolidBrush(bg);
                g.FillRectangle(rb, rowRect);
            }

            // Row number
            if (_showRowNumber)
            {
                int numX = _isRtl ? sd + gridW - 44 : sd;
                using var nb = new SolidBrush(Color.FromArgb(130, _rowText));
                g.DrawString((ri + 1).ToString(), _cachedRFont, nb,
                    new Rectangle(numX, y, 44, _rowHeight), _sfC);
            }

            // Cells
            if (_colX == null) return;
            foreach (int i in Indices(_visibleCols.Count))
            {
                int w = _computedWidths[i];
                var col = _visibleCols[i];
                var cr = new Rectangle(_colX[i], y, w, _rowHeight);
                string raw = _pageRows[ri].TryGetValue(col.Field, out var v) ? v?.ToString() ?? "" : "";
                string disp = col.ValueFormatter != null ? col.ValueFormatter(v) : FormatValue(raw, col);

                switch (col.CellType)
                {
                    case ProCellType.Avatar: DrawAvatarCell(g, cr, disp, ri, false); break;
                    case ProCellType.AvatarText: DrawAvatarCell(g, cr, disp, ri, true); break;
                    case ProCellType.Badge: DrawBadgeCell(g, cr, disp); break;
                    case ProCellType.Actions: DrawActionsCell(g, cr, ri); break;
                    case ProCellType.Boolean: DrawBooleanCell(g, cr, raw); break;
                    case ProCellType.Progress: DrawProgressCell(g, cr, raw); break;
                    case ProCellType.Rating: DrawRatingCell(g, cr, raw); break;
                    case ProCellType.Tag: DrawTagCell(g, cr, disp, col); break;
                    case ProCellType.Image: DrawImageCell(g, cr, col.CustomIcon); break;
                    default: DrawTextCell(g, cr, disp, col.TextAlign, col.CustomColor); break;
                }
            }

            if (_showRowSep)
                g.DrawLine(_sepPen, sd, y + _rowHeight - 1, sd + gridW, y + _rowHeight - 1);

            // Accent selection bar
            if (sel)
            {
                using var ab = new SolidBrush(_accent);
                g.FillRectangle(ab, new Rectangle(sd, y + 4, 3, _rowHeight - 8));
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CELL RENDERERS
        // ══════════════════════════════════════════════════════════════════════

        private void DrawTextCell(Graphics g, Rectangle r, string text, StringAlignment align, Color? clr = null)
        {
            var sf = align == StringAlignment.Near ? _sfN : align == StringAlignment.Far ? _sfF : _sfC;
            var tr = new Rectangle(r.X + 8, r.Y, r.Width - 16, r.Height);
            using var br = new SolidBrush(clr ?? _rowText);
            g.DrawString(text, _cachedRFont, br, tr, sf);
        }

        private void DrawBadgeCell(Graphics g, Rectangle r, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!_badges.TryGetValue(text, out var badge))
                badge = new ProBadge(Color.FromArgb(148, 163, 184), Color.White);

            SizeF sz = g.MeasureString(text, _cachedBadgeFont);
            int bw = (int)sz.Width + 22, bh = 26;
            var bRect = new Rectangle(r.X + (r.Width - bw) / 2, r.Y + (r.Height - bh) / 2, bw, bh);

            using var path = RoundedRect(bRect, 13);
            using var bg = new SolidBrush(badge.Background);
            g.FillPath(bg, path);

            if (badge.BorderColor.HasValue)
            {
                using var bp = new Pen(badge.BorderColor.Value, 1f);
                g.DrawPath(bp, path);
            }
            using var fg = new SolidBrush(badge.Foreground);
            g.DrawString(text, _cachedBadgeFont, fg, bRect, _sfC);
        }

        private void DrawAvatarCell(Graphics g, Rectangle r, string text, int ri, bool showText)
        {
            int d = 36, pad = 10;
            Color ac = _palette[Math.Abs(text.GetHashCode()) % _palette.Length];
            int cx = _isRtl ? r.Right - pad - d : r.X + pad;
            int cy = r.Y + (r.Height - d) / 2;
            var circ = new Rectangle(cx, cy, d, d);

            using (var br = new LinearGradientBrush(circ, Lighten(ac, 22), ac, LinearGradientMode.ForwardDiagonal))
            using (var p = RoundedRect(circ, 50))
                g.FillPath(br, p);

            using var iFont = new Font(_cachedRFont.FontFamily, _cachedRFont.Size - 0.5f, FontStyle.Bold);
            using var wh = new SolidBrush(Color.White);
            g.DrawString(GetInitial(text), iFont, wh, circ, _sfC);

            if (showText)
            {
                int tx = _isRtl ? r.X + pad : cx + d + 8;
                var tf = new Rectangle(tx, r.Y, r.Width - d - pad * 2 - 8, r.Height);
                g.DrawString(text, _cachedRFont, _rTextBrush, tf, _isRtl ? _sfF : _sfN);
            }
        }

        private void DrawActionsCell(Graphics g, Rectangle r, int ri)
        {
            var va = _actions.Where(a => a.Visible).ToList();
            if (va.Count == 0) return;

            int iSz = 28, iSp = 6;
            int tw = va.Count * iSz + (va.Count - 1) * iSp;
            int sx = r.X + (r.Width - tw) / 2;
            int iy = r.Y + (r.Height - iSz) / 2;

            for (int i = 0; i < va.Count; i++)
            {
                var a = va[i];
                var ir = new Rectangle(sx + i * (iSz + iSp), iy, iSz, iSz);
                bool hv = ri == _hoverRow && _hoverActionKey == a.Key;

                if (hv)
                {
                    var hr = new Rectangle(ir.X - 4, ir.Y - 4, ir.Width + 8, ir.Height + 8);
                    using var hb = new SolidBrush(Color.FromArgb(22, a.Color));
                    using var hp = RoundedRect(hr, 8);
                    g.FillPath(hb, hp);
                }

                if (a.Icon != null) g.DrawImage(a.Icon, ir);
                else
                {
                    using var ab = new SolidBrush(hv ? a.Color : Color.FromArgb(155, a.Color));
                    g.FillEllipse(ab, ir);
                }
            }
        }

        private void DrawBooleanCell(Graphics g, Rectangle r, string val)
        {
            bool b = val == "1" || val.ToLower() is "true" or "نعم" or "yes";
            int sz = 22;
            var rc = new Rectangle(r.X + (r.Width - sz) / 2, r.Y + (r.Height - sz) / 2, sz, sz);
            Color fc = b ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);
            using var path = RoundedRect(rc, 6);
            using var br = new SolidBrush(fc);
            g.FillPath(br, path);
            using var wf = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var wb = new SolidBrush(Color.White);
            g.DrawString(b ? "✓" : "✕", wf, wb, rc, _sfC);
        }

        private void DrawProgressCell(Graphics g, Rectangle r, string val)
        {
            if (!double.TryParse(val, out double pct)) return;
            pct = Math.Clamp(pct, 0, 100);
            int ph = 8, pw = r.Width - 28, px = r.X + 14;
            int py = r.Y + (r.Height - ph) / 2;
            var tr = new Rectangle(px, py, pw, ph);

            using (var tp = RoundedRect(tr, 4))
            using (var tb = new SolidBrush(Color.FromArgb(222, 228, 245)))
                g.FillPath(tb, tp);

            int fw = (int)(pw * pct / 100.0);
            if (fw > 0)
            {
                var fr = new Rectangle(px, py, fw, ph);
                Color fc = pct >= 75 ? Color.FromArgb(16, 185, 129) : pct >= 40 ? Color.FromArgb(245, 158, 11) : Color.FromArgb(239, 68, 68);
                using var fp = RoundedRect(fr, 4);
                using var fb = new LinearGradientBrush(fr, Lighten(fc, 18), fc, LinearGradientMode.Horizontal);
                g.FillPath(fb, fp);
            }
            using var pf = new Font(_cachedRFont.FontFamily, 7.5f, FontStyle.Bold);
            g.DrawString($"{pct:0}%", pf, _rTextBrush, new Rectangle(px, r.Y, pw, r.Height), _sfC);
        }

        private void DrawRatingCell(Graphics g, Rectangle r, string val)
        {
            if (!double.TryParse(val, out double rating)) return;
            rating = Math.Clamp(rating, 0, 5);
            int ss = 14, sc = 5, tw = sc * ss + (sc - 1) * 2;
            int sx = r.X + (r.Width - tw) / 2, sy = r.Y + (r.Height - ss) / 2;
            using var filled = new SolidBrush(Color.FromArgb(245, 158, 11));
            using var empty = new SolidBrush(Color.FromArgb(215, 222, 235));
            for (int i = 0; i < sc; i++)
                DrawStar(g, new Rectangle(sx + i * (ss + 2), sy, ss, ss),
                    i < (int)Math.Round(rating) ? filled : empty);
        }

        private void DrawStar(Graphics g, Rectangle r, Brush br)
        {
            float cx = r.X + r.Width / 2f, cy = r.Y + r.Height / 2f;
            float ou = r.Width / 2f, inn = ou * 0.4f;
            var pts = new PointF[10];
            for (int i = 0; i < 10; i++)
            {
                float ang = (float)(Math.PI / 5 * i - Math.PI / 2);
                float rad = i % 2 == 0 ? ou : inn;
                pts[i] = new PointF(cx + rad * MathF.Cos(ang), cy + rad * MathF.Sin(ang));
            }
            g.FillPolygon(br, pts);
        }

        private void DrawTagCell(Graphics g, Rectangle r, string text, ProColumn col)
        {
            if (string.IsNullOrEmpty(text)) return;
            Color tc = col.CustomColor ?? _accent;
            SizeF sz = g.MeasureString(text, _cachedBadgeFont);
            int tw = (int)sz.Width + 16, th = 22;
            var tr = new Rectangle(r.X + (r.Width - tw) / 2, r.Y + (r.Height - th) / 2, tw, th);
            using var tp = RoundedRect(tr, 5);
            using var tb = new SolidBrush(Color.FromArgb(22, tc));
            g.FillPath(tb, tp);
            using var tbp = new Pen(Color.FromArgb(105, tc), 1f);
            g.DrawPath(tbp, tp);
            using var tf = new SolidBrush(tc);
            g.DrawString(text, _cachedBadgeFont, tf, tr, _sfC);
        }

        private void DrawImageCell(Graphics g, Rectangle r, Image icon)
        {
            if (icon == null) return;
            int sz = Math.Min(r.Height - 10, r.Width - 10);
            g.DrawImage(icon, new Rectangle(r.X + (r.Width - sz) / 2, r.Y + (r.Height - sz) / 2, sz, sz));
        }

        private void DrawEmpty(Graphics g, Rectangle bodyRect)
        {
            if (_emptyIcon != null)
            {
                int ix = bodyRect.X + (bodyRect.Width - 48) / 2;
                int iy = bodyRect.Y + bodyRect.Height / 2 - 52;
                g.DrawImage(_emptyIcon, ix, iy, 48, 48);
            }
            using var br = new SolidBrush(Color.FromArgb(130, _rowText));
            using var f = new Font(_cachedRFont.FontFamily, _cachedRFont.Size + 1f);
            g.DrawString(_emptyText, f, br, bodyRect, _sfC);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════════════════

        private string FormatValue(string raw, ProColumn col)
        {
            string result = raw;
            if (!string.IsNullOrEmpty(col.Format))
            {
                if (double.TryParse(raw, out double num)) result = num.ToString(col.Format);
                else if (DateTime.TryParse(raw, out DateTime dt)) result = dt.ToString(col.Format);
            }
            return col.Prefix + result + col.Suffix;
        }

        /// <summary>ترتيب الأعمدة: LTR = 0..N-1 ، RTL = N-1..0</summary>
        private IEnumerable<int> Indices(int count)
        {
            var r = Enumerable.Range(0, count);
            return _isRtl ? r.Reverse() : r;
        }

        private string GetInitial(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            foreach (char c in name) if (char.IsLetter(c)) return c.ToString().ToUpper();
            return "?";
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Max(0, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color BlendColor(Color a, Color b, float t) =>
            Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));

        private static Color Lighten(Color c, int amt) =>
            Color.FromArgb(c.A, Math.Min(255, c.R + amt), Math.Min(255, c.G + amt), Math.Min(255, c.B + amt));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeGdiResources();
                _animTimer?.Dispose();
                _shadowBmp?.Dispose();
                _parentBgBmp?.Dispose();
                _tt?.Dispose();
                _sfC?.Dispose();
                _sfN?.Dispose();
                _sfF?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}