using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat
{
    // ═══════════════════════════════════════════════════════════
    //  ENUMS & SUPPORTING CLASSES  (لا تغيير)
    // ═══════════════════════════════════════════════════════════

    public enum GridCellType { Text, Badge, Avatar, Currency, Actions }

    public class GridColumn
    {
        public string Header { get; set; }
        public string Field { get; set; }
        public int Width { get; set; } = 120;
        public GridCellType CellType { get; set; } = GridCellType.Text;
        public bool Sortable { get; set; } = true;   // ✦ جديد — اختياري
    }

    public class BadgeStyle
    {
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public BadgeStyle(Color bg, Color fg) { Background = bg; Foreground = fg; }
    }

    // ═══════════════════════════════════════════════════════════
    //  SLICK TABLE  —  v3  (GDI Cache + Per-Row Anim + Sort)
    // ═══════════════════════════════════════════════════════════

    public class SlickTable : UserControl
    {
        // ── Data ─────────────────────────────────────────────
        private List<GridColumn> _columns = new();
        private List<Dictionary<string, object>> _rows = new();

        // ── Layout ───────────────────────────────────────────
        private int _rowHeight = 52;
        private int _headerHeight = 74;
        private int _scrollOffset = 0;
        private int _selectedRow = -1;
        private int _hoverRow = -1;
        private int[] _computedWidths;

        // ── Column index cache  (لا LINQ في كل paint) ────────
        private int[] _colIndices;                     // ✦ جديد

        // ── Sort ─────────────────────────────────────────────
        private int _sortCol = -1;                    // ✦ جديد
        private bool _sortAsc = true;                  // ✦ جديد

        // ── Smooth Hover Animation ────────────────────────────
        private float _hoverAlpha = 0f;
        private int _animHoverRow = -1;
        private Timer _animTimer;

        // ── Shadow cache ──────────────────────────────────────
        private Bitmap _shadowCache;
        private Size _shadowCacheSize;

        // ── GDI Resource Cache  (✦ المكسب الأكبر في الأداء) ──
        // يُعاد بناؤها فقط عند تغيير خاصية تؤثر عليها
        private Font _cachedHeaderFont;
        private Font _cachedBadgeFont;
        private Font _cachedAvatarFont;
        private SolidBrush _headerBgBrush;
        private SolidBrush _headerFgBrush;
        private SolidBrush _rowTextBrush;
        private Pen _separatorPen;
        private Pen _outerBorderPen;
        private Pen _headerBorderPen;
        private bool _gdiDirty = true;          // ✦ جديد — flag إعادة البناء

        // StringFormats ثابتة — تُبنى مرة واحدة فقط
        private readonly StringFormat _sfCenter = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        private readonly StringFormat _sfNear = new()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        private readonly StringFormat _sfFar = new()
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Shape
        // ════════════════════════════════════════════════════════

        private int _borderRadius = 0;
        [Category("✦ Shape"), DefaultValue(16)]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); RebuildShadow(); Invalidate(); } }

        private int _shadowDepth = 0;
        [Category("✦ Shape"), DefaultValue(12)]
        public int ShadowDepth
        { get => _shadowDepth; set { _shadowDepth = Math.Clamp(value, 0, 40); RebuildShadow(); Invalidate(); } }

        private Color _shadowColor = Color.FromArgb(60, 0, 0, 0);
        [Category("✦ Shape")]
        public Color ShadowColor
        { get => _shadowColor; set { _shadowColor = value; RebuildShadow(); Invalidate(); } }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Header
        // ════════════════════════════════════════════════════════

        private Color _headerBackground = Color.FromArgb(22, 45, 90);
        [Category("✦ Header")]
        public Color HeaderBackground
        { get => _headerBackground; set { _headerBackground = value; MarkGdiDirty(); } }

        private Color _headerForeground = Color.White;
        [Category("✦ Header")]
        public Color HeaderForeground
        { get => _headerForeground; set { _headerForeground = value; MarkGdiDirty(); } }

        private Color _headerBorderColor = Color.FromArgb(10, 30, 70);
        [Category("✦ Header")]
        public Color HeaderBorderColor
        { get => _headerBorderColor; set { _headerBorderColor = value; MarkGdiDirty(); } }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Rows
        // ════════════════════════════════════════════════════════

        private Color _rowEven = Color.White;
        private Color _rowOdd = Color.FromArgb(240, 247, 255);
        private Color _rowSelected = Color.FromArgb(210, 230, 255);
        private Color _rowHover = Color.FromArgb(225, 238, 255);
        private int _rowHeight2 = 52;

        [Category("✦ Rows")] public Color RowEven { get => _rowEven; set { _rowEven = value; Invalidate(); } }
        [Category("✦ Rows")] public Color RowOdd { get => _rowOdd; set { _rowOdd = value; Invalidate(); } }
        [Category("✦ Rows")] public Color RowSelected { get => _rowSelected; set { _rowSelected = value; Invalidate(); } }
        [Category("✦ Rows")] public Color RowHover { get => _rowHover; set { _rowHover = value; Invalidate(); } }

        [Category("✦ Rows"), DefaultValue(52)]
        public int RowHeight
        { get => _rowHeight2; set { _rowHeight2 = Math.Max(24, value); _rowHeight = _rowHeight2; UpdateScrollbar(); Invalidate(); } }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Grid Lines
        // ════════════════════════════════════════════════════════

        private Color _borderColor = Color.FromArgb(220, 228, 240);
        private Color _outerBorderColor = Color.FromArgb(200, 215, 235);
        private bool _showOuterBorder = true;

        [Category("✦ Grid Lines")] public Color BorderColor { get => _borderColor; set { _borderColor = value; MarkGdiDirty(); } }
        [Category("✦ Grid Lines")] public Color OuterBorderColor { get => _outerBorderColor; set { _outerBorderColor = value; MarkGdiDirty(); } }
        [Category("✦ Grid Lines"), DefaultValue(true)]
        public bool ShowOuterBorder { get => _showOuterBorder; set { _showOuterBorder = value; Invalidate(); } }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Hover Animation
        // ════════════════════════════════════════════════════════

        private bool _smoothHover = true;
        private int _hoverSpeed = 16;   // ✦ تقليل الـ interval الافتراضي لـ 60fps

        [Category("✦ Hover Animation"), DefaultValue(true)]
        public bool SmoothHover { get => _smoothHover; set { _smoothHover = value; } }

        [Category("✦ Hover Animation"), DefaultValue(16)]
        public int HoverSpeed
        { get => _hoverSpeed; set { _hoverSpeed = Math.Clamp(value, 5, 60); _animTimer.Interval = _hoverSpeed; } }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Features  (جديد)
        // ════════════════════════════════════════════════════════

        private bool _allowSort = true;
        private string _emptyText = "لا توجد بيانات للعرض";
        private Color _sortArrowColor = Color.FromArgb(190, 215, 255);

        [Category("✦ Features"), DefaultValue(true), Description("السماح بالترتيب بالنقر على الهيدر")]
        public bool AllowSort { get => _allowSort; set { _allowSort = value; } }

        [Category("✦ Features"), Description("النص المعروض عند فراغ الجدول")]
        public string EmptyText { get => _emptyText; set { _emptyText = value; Invalidate(); } }

        [Category("✦ Features"), Description("لون سهم الترتيب")]
        public Color SortArrowColor { get => _sortArrowColor; set { _sortArrowColor = value; Invalidate(); } }

        // ════════════════════════════════════════════════════════
        //  ✦ RTL + Action Icons + Badge Styles  (لا تغيير)
        // ════════════════════════════════════════════════════════

        private bool _isRtl = false;
        [Category("✦ Layout"), DefaultValue(false)]
        public bool IsRtl
        {
            get => _isRtl;
            set { _isRtl = value; RebuildColIndices(); Invalidate(); }
        }

        public Image IconView { get; set; }
        public Image IconEdit { get; set; }
        public Image IconDelete { get; set; }

        private Dictionary<string, BadgeStyle> _badgeStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Available",   new(Color.FromArgb(39,  174, 96),  Color.White) },
            { "متاح",        new(Color.FromArgb(39,  174, 96),  Color.White) },
            { "OnRent",      new(Color.FromArgb(41,  128, 185), Color.White) },
            { "Rented",      new(Color.FromArgb(41,  128, 185), Color.White) },
            { "مؤجرة",       new(Color.FromArgb(41,  128, 185), Color.White) },
            { "InService",   new(Color.FromArgb(230, 126, 34),  Color.White) },
            { "Maintenance", new(Color.FromArgb(230, 126, 34),  Color.White) },
            { "صيانة",       new(Color.FromArgb(230, 126, 34),  Color.White) },
            { "Confirmed",   new(Color.FromArgb(39,  174, 96),  Color.White) },
            { "Pending",     new(Color.FromArgb(243, 156, 18),  Color.White) },
            { "Cancelled",   new(Color.FromArgb(192, 57,  43),  Color.White) },
            { "Active",      new(Color.FromArgb(39,  174, 96),  Color.White) },
            { "Inactive",    new(Color.FromArgb(149, 165, 166), Color.White) },
            { "Unavailable", new(Color.FromArgb(149, 165, 166), Color.White) },
        };

        private readonly Color[] _avatarColors =
        {
            Color.FromArgb(52,  152, 219), Color.FromArgb(155, 89,  182),
            Color.FromArgb(46,  204, 113), Color.FromArgb(230, 126, 34),
            Color.FromArgb(231, 76,  60),  Color.FromArgb(26,  188, 156),
            Color.FromArgb(41,  128, 185), Color.FromArgb(39,  174, 96),
        };

        private enum ActionBtn { None, View, Edit, Delete }
        private ActionBtn _hoverBtn = ActionBtn.None;
        private VScrollBar _vScroll;

        // ════════════════════════════════════════════════════════
        //  EVENTS  (لا تغيير)
        // ════════════════════════════════════════════════════════
        public event EventHandler<int> RowClicked;
        public event EventHandler<int> ViewClicked;
        public event EventHandler<int> EditClicked;
        public event EventHandler<int> DeleteClicked;

        // ════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ════════════════════════════════════════════════════════
        public SlickTable()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Font = new Font("IBM Plex Sans Arabic", 13.5f, FontStyle.Regular);

            _vScroll = new VScrollBar
            {
                Dock = DockStyle.Right,
                SmallChange = _rowHeight,
                LargeChange = _rowHeight * 5,
                Visible = false
            };
            _vScroll.Scroll += (s, e) => { _scrollOffset = _vScroll.Value; Invalidate(); };
            Controls.Add(_vScroll);

            _animTimer = new Timer { Interval = _hoverSpeed, Enabled = false };
            _animTimer.Tick += AnimTick;

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            MouseClick += OnMouseClick;
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;   // ✦ جديد — للسورت

            Resize += (s, e) =>
            {
                ComputeWidths(); UpdateScrollbar();
                RebuildShadow(); Invalidate();
            };
        }

        // ════════════════════════════════════════════════════════
        //  GDI RESOURCE MANAGEMENT  ✦ الجديد الأساسي
        // ════════════════════════════════════════════════════════

        private void MarkGdiDirty() { _gdiDirty = true; Invalidate(); }

        /// <summary>يُعاد استدعاؤها في بداية OnPaint فقط عند الحاجة — صفر allocation في باقي الأوقات</summary>
        private void EnsureGdi()
        {
            if (!_gdiDirty) return;

            // تحرير القديم
            _cachedHeaderFont?.Dispose();
            _cachedBadgeFont?.Dispose();
            _cachedAvatarFont?.Dispose();
            _headerBgBrush?.Dispose();
            _headerFgBrush?.Dispose();
            _rowTextBrush?.Dispose();
            _separatorPen?.Dispose();
            _outerBorderPen?.Dispose();
            _headerBorderPen?.Dispose();

            // بناء الجديد
            _cachedHeaderFont = new Font(Font.FontFamily, 15f, FontStyle.Bold);
            _cachedBadgeFont = new Font(Font.FontFamily, 8.5f, FontStyle.Bold);
            _cachedAvatarFont = new Font(Font.FontFamily, 9f, FontStyle.Bold);
            _headerBgBrush = new SolidBrush(_headerBackground);
            _headerFgBrush = new SolidBrush(_headerForeground);
            _rowTextBrush = new SolidBrush(Color.FromArgb(50, 50, 70));
            _separatorPen = new Pen(_borderColor, 1f);
            _outerBorderPen = new Pen(_outerBorderColor, 1f);
            _headerBorderPen = new Pen(_headerBorderColor, 2f);

            _gdiDirty = false;
        }

        // ════════════════════════════════════════════════════════
        //  PUBLIC API  (لا تغيير في التوقيعات)
        // ════════════════════════════════════════════════════════

        public void SetColumns(List<GridColumn> columns)
        {
            _columns = columns ?? new();
            RebuildColIndices();
            ComputeWidths();
            Invalidate();
        }

        public void SetData(List<Dictionary<string, object>> data)
        {
            _rows = data ?? new();
            _scrollOffset = 0; _selectedRow = -1;
            _hoverRow = -1; _hoverBtn = ActionBtn.None;
            _sortCol = -1; _sortAsc = true;
            UpdateScrollbar();
            Invalidate();
        }

        public void AddBadgeStyle(string key, Color bg, Color fg)
            => _badgeStyles[key] = new BadgeStyle(bg, fg);

        public Dictionary<string, object> GetSelectedRow()
            => (_selectedRow >= 0 && _selectedRow < _rows.Count) ? _rows[_selectedRow] : null;

        public int GetSelectedIndex() => _selectedRow;

        // ════════════════════════════════════════════════════════
        //  COLUMN INDEX CACHE  ✦
        // ════════════════════════════════════════════════════════

        private void RebuildColIndices()
        {
            var range = Enumerable.Range(0, _columns.Count);
            _colIndices = (IsRtl ? range.Reverse() : range).ToArray();
        }

        // ════════════════════════════════════════════════════════
        //  SMOOTH HOVER ANIMATION  ✦ per-row invalidation
        // ════════════════════════════════════════════════════════

        private void AnimTick(object s, EventArgs e)
        {
            if (!_smoothHover) { _animTimer.Stop(); return; }

            float prev = _hoverAlpha;
            bool entering = (_animHoverRow == _hoverRow && _hoverRow >= 0);
            float step = 0.10f;

            _hoverAlpha = entering
                ? Math.Min(1f, _hoverAlpha + step)
                : Math.Max(0f, _hoverAlpha - step);

            // ✦ إعادة رسم الصف المتأثر فقط — لا Invalidate() كاملة
            if (Math.Abs(_hoverAlpha - prev) > 0.001f)
            {
                int dirtyRow = _hoverRow >= 0 ? _hoverRow : _animHoverRow;
                InvalidateRow(dirtyRow);
                if (_animHoverRow != _hoverRow && _animHoverRow >= 0)
                    InvalidateRow(_animHoverRow);
            }

            if (_hoverAlpha <= 0f || _hoverAlpha >= 1f)
                _animTimer.Stop();
        }

        /// <summary>يُعيد رسم صف واحد فقط — توفير هائل في الـ GPU</summary>
        private void InvalidateRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows.Count) { Invalidate(); return; }
            int sd = _shadowDepth;
            int y = sd + _headerHeight + rowIndex * _rowHeight - _scrollOffset;
            Invalidate(new Rectangle(sd, y, Width - sd * 2, _rowHeight + 1));
        }

        // ════════════════════════════════════════════════════════
        //  SHADOW  (لا تغيير جوهري — فقط تحسين الفورمولا)
        // ════════════════════════════════════════════════════════

        private void RebuildShadow()
        {
            _shadowCache?.Dispose();
            _shadowCache = null;
            if (_shadowDepth <= 0 || Width <= 0 || Height <= 0) return;

            int sd = _shadowDepth;
            var bmp = new Bitmap(Width + sd * 2, Height + sd * 2);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = sd; i >= 1; i--)
            {
                float t = 1f - (float)i / sd;
                int alpha = (int)(_shadowColor.A * t * t * 0.55f);
                alpha = Math.Clamp(alpha, 0, 255);
                if (alpha == 0) continue;

                var sr = new Rectangle(sd - i, sd - i + 3, Width - 1 + i * 2, Height - 1 + i * 2);
                using var sp = RoundedPath(sr, _borderRadius + i);
                using var sb = new SolidBrush(Color.FromArgb(alpha, _shadowColor.R, _shadowColor.G, _shadowColor.B));
                g.FillPath(sb, sp);
            }
            _shadowCache = bmp;
            _shadowCacheSize = new Size(Width, Height);
        }

        // ════════════════════════════════════════════════════════
        //  COLUMN WIDTHS
        // ════════════════════════════════════════════════════════

        private void ComputeWidths()
        {
            if (_columns == null || _columns.Count == 0) return;
            int scrollW = _vScroll.Visible ? _vScroll.Width : 0;
            int available = Math.Max(0, Width - scrollW - _shadowDepth * 2);
            int total = _columns.Sum(c => c.Width);

            _computedWidths = new int[_columns.Count];
            if (available <= total || total == 0)
            { for (int i = 0; i < _columns.Count; i++) _computedWidths[i] = _columns[i].Width; return; }

            int extra = available - total, distributed = 0;
            for (int i = 0; i < _columns.Count; i++)
            {
                int bonus = (i < _columns.Count - 1)
                    ? (int)((double)_columns[i].Width / total * extra)
                    : extra - distributed;
                _computedWidths[i] = _columns[i].Width + bonus;
                distributed += bonus;
            }
        }

        // ════════════════════════════════════════════════════════
        //  SCROLLBAR
        // ════════════════════════════════════════════════════════

        private void UpdateScrollbar()
        {
            int totalH = _rows.Count * _rowHeight;
            int visibleH = Height - _headerHeight - _shadowDepth * 2;

            if (totalH > visibleH)
            {
                _vScroll.Visible = true;
                _vScroll.Maximum = totalH - visibleH + _vScroll.LargeChange;
                _vScroll.Value = Math.Min(_scrollOffset, Math.Max(0, _vScroll.Maximum - _vScroll.LargeChange));
                ComputeWidths();
            }
            else
            { _vScroll.Visible = false; _scrollOffset = 0; ComputeWidths(); }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int maxScroll = Math.Max(0, _rows.Count * _rowHeight - (Height - _headerHeight - _shadowDepth * 2));
            _scrollOffset = Math.Clamp(_scrollOffset - e.Delta / 3, 0, maxScroll);
            if (_vScroll.Visible)
                _vScroll.Value = Math.Min(_scrollOffset, _vScroll.Maximum - _vScroll.LargeChange + 1);
            Invalidate();
        }

        // ════════════════════════════════════════════════════════
        //  HIT TEST
        // ════════════════════════════════════════════════════════

        private int HitTestRow(int mouseY)
        {
            int ry = mouseY - _headerHeight - _shadowDepth + _scrollOffset;
            if (ry < 0) return -1;
            int row = ry / _rowHeight;
            return row < _rows.Count ? row : -1;
        }

        private int HitTestColumn(int mouseX)
        {
            if (_computedWidths == null || _colIndices == null) return -1;
            int x = _shadowDepth;
            foreach (int i in _colIndices)
            {
                if (mouseX >= x && mouseX < x + _computedWidths[i]) return i;
                x += _computedWidths[i];
            }
            return -1;
        }

        private ActionBtn HitTestActionBtn(int mouseX, int mouseY)
        {
            int row = HitTestRow(mouseY);
            if (row < 0 || _computedWidths == null) return ActionBtn.None;

            int ax = _shadowDepth, aw = 0;
            foreach (int i in _colIndices)
            {
                if (_columns[i].CellType == GridCellType.Actions) { aw = _computedWidths[i]; break; }
                ax += _computedWidths[i];
            }
            if (aw == 0) return ActionBtn.None;

            int rowY = _headerHeight + _shadowDepth + row * _rowHeight - _scrollOffset;
            int iSz = 22, sp = 10, totalW = 3 * iSz + 2 * sp;
            int startX = ax + (aw - totalW) / 2;
            int iconY = rowY + (_rowHeight - iSz) / 2;

            if (new Rectangle(startX, iconY, iSz, iSz).Contains(mouseX, mouseY)) return ActionBtn.View;
            if (new Rectangle(startX + iSz + sp, iconY, iSz, iSz).Contains(mouseX, mouseY)) return ActionBtn.Edit;
            if (new Rectangle(startX + 2 * (iSz + sp), iconY, iSz, iSz).Contains(mouseX, mouseY)) return ActionBtn.Delete;
            return ActionBtn.None;
        }

        // ════════════════════════════════════════════════════════
        //  MOUSE EVENTS
        // ════════════════════════════════════════════════════════

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int row = HitTestRow(e.Y);
            ActionBtn btn = HitTestActionBtn(e.X, e.Y);
            bool changed = (row != _hoverRow || btn != _hoverBtn);

            if (changed && _smoothHover)
            {
                // ✦ لا نعيد الـ alpha إلى 0 — نبدأ من القيمة الحالية
                _animHoverRow = row;
                _animTimer.Start();
            }

            _hoverRow = row; _hoverBtn = btn;
            Cursor = row >= 0 ? Cursors.Hand : Cursors.Default;
            if (changed && !_smoothHover) Invalidate();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            _hoverRow = -1; _hoverBtn = ActionBtn.None;
            if (_smoothHover) { _animHoverRow = -1; _animTimer.Start(); }
            else Invalidate();
            Cursor = Cursors.Default;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // ✦ Sort على click الهيدر
            if (!_allowSort || e.Y > _headerHeight + _shadowDepth) return;
            int ci = HitTestColumn(e.X);
            if (ci < 0 || !_columns[ci].Sortable || _columns[ci].CellType == GridCellType.Actions) return;

            if (_sortCol == ci) _sortAsc = !_sortAsc;
            else { _sortCol = ci; _sortAsc = true; }

            ApplySort();
            Invalidate();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            int row = HitTestRow(e.Y);
            if (row < 0) return;

            ActionBtn btn = HitTestActionBtn(e.X, e.Y);
            _selectedRow = row;
            Invalidate();

            RowClicked?.Invoke(this, row);
            switch (btn)
            {
                case ActionBtn.View: ViewClicked?.Invoke(this, row); break;
                case ActionBtn.Edit: EditClicked?.Invoke(this, row); break;
                case ActionBtn.Delete: DeleteClicked?.Invoke(this, row); break;
            }
        }

        // ════════════════════════════════════════════════════════
        //  SORT  ✦
        // ════════════════════════════════════════════════════════

        private void ApplySort()
        {
            if (_sortCol < 0 || _sortCol >= _columns.Count) return;
            string field = _columns[_sortCol].Field;

            // نستخدم SortKey struct بدلاً من object خام — يضمن مقارنة متسقة
            _rows = _sortAsc
                ? _rows.OrderBy(r => GetSortKey(r, field)).ToList()
                : _rows.OrderByDescending(r => GetSortKey(r, field)).ToList();

            _selectedRow = -1;
            Invalidate();
        }

        /// <summary>
        /// يُرجع مفتاح مقارنة موحّد الـ type — رقم أو تاريخ أو نص.
        /// Tuple (priority, numericVal, strVal) يضمن عدم خلط الأنواع.
        /// </summary>
        private static (int priority, double num, string text)
            GetSortKey(Dictionary<string, object> row, string field)
        {
            if (!row.TryGetValue(field, out var v) || v == null)
                return (2, 0, "");

            // أرقام صريحة
            if (v is int i) return (0, i, "");
            if (v is long l) return (0, l, "");
            if (v is float f) return (0, f, "");
            if (v is double d) return (0, d, "");
            if (v is decimal dm) return (0, (double)dm, "");

            // تاريخ — نحوّله لـ ticks رقمي
            if (v is DateTime dt) return (1, (double)dt.Ticks, "");

            // نص قابل للتحويل لرقم
            string s = v.ToString();
            if (double.TryParse(s, System.Globalization.NumberStyles.Any,
                                   System.Globalization.CultureInfo.InvariantCulture, out double n))
                return (0, n, "");

            // تاريخ كنص
            if (DateTime.TryParse(s, out DateTime dtp))
                return (1, (double)dtp.Ticks, "");

            // نص عادي
            return (2, 0, s);
        }

        // ════════════════════════════════════════════════════════
        //  PAINT
        // ════════════════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
            // ✦ بناء موارد GDI مرة واحدة فقط عند الحاجة
            EnsureGdi();

            if (_computedWidths == null || _computedWidths.Length != _columns.Count)
                ComputeWidths();

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int sd = _shadowDepth;
            int scrollW = _vScroll.Visible ? _vScroll.Width : 0;
            int gridW = Width - sd * 2 - scrollW;
            int gridH = Height - sd * 2;
            var cardRect = new Rectangle(sd, sd, gridW - 1, gridH - 1);

            // ── 1. Shadow ─────────────────────────────────────
            if (sd > 0)
            {
                if (_shadowCache == null || _shadowCacheSize != Size) RebuildShadow();
                if (_shadowCache != null) g.DrawImage(_shadowCache, -sd, -sd);
            }

            // ── 2. Clip to card ───────────────────────────────
            using var cardPath = RoundedPath(cardRect, _borderRadius);
            g.SetClip(cardPath);

            // ── 3. Header ─────────────────────────────────────
            DrawHeader(g, sd, gridW);

            // ── 4. Rows / Empty ───────────────────────────────
            var rowClip = new Rectangle(sd, sd + _headerHeight, gridW, gridH - _headerHeight);
            g.SetClip(rowClip);

            if (_rows.Count == 0)
                DrawEmpty(g, rowClip);
            else
                for (int i = 0; i < _rows.Count; i++)
                {
                    int y = sd + _headerHeight + i * _rowHeight - _scrollOffset;
                    if (y + _rowHeight < rowClip.Top) continue;
                    if (y > rowClip.Bottom) break;
                    DrawRow(g, i, y, sd, gridW);
                }

            // ── 5. Outer border ───────────────────────────────
            g.ResetClip();
            g.SetClip(cardPath);
            if (_showOuterBorder)
                g.DrawPath(_outerBorderPen, cardPath);
            g.ResetClip();
        }

        // ── Header ───────────────────────────────────────────

        private void DrawHeader(Graphics g, int sd, int gridW)
        {
            var headerRect = new Rectangle(sd, sd, gridW, _headerHeight);
            g.FillRectangle(_headerBgBrush, headerRect);   // ✦ cached brush

            int x = sd;
            foreach (int i in _colIndices)                 // ✦ cached indices
            {
                int w = _computedWidths[i];

                // عنوان العمود
                bool isAvatar = _columns[i].CellType == GridCellType.Avatar;
                var tRect = (IsRtl && isAvatar)
                    ? new Rectangle(x, sd, w - 100, _headerHeight)
                    : new Rectangle(x, sd, w, _headerHeight);
                var sf = (IsRtl && isAvatar) ? _sfFar : _sfCenter;

                g.DrawString(_columns[i].Header, _cachedHeaderFont, _headerFgBrush, tRect, sf);

                // ✦ سهم الترتيب — يسار في RTL، يمين في LTR
                if (_allowSort && _sortCol == i)
                {
                    int arrowX = _isRtl ? x + 6 : x + w - 18;
                    DrawSortArrow(g, arrowX, sd + _headerHeight / 2 - 5, _sortAsc);
                }

                x += w;
            }

            g.DrawLine(_headerBorderPen, sd, sd + _headerHeight, sd + gridW, sd + _headerHeight);
        }

        private void DrawSortArrow(Graphics g, int x, int y, bool asc)
        {
            using var br = new SolidBrush(_sortArrowColor);
            Point[] pts = asc
                ? new[] { new Point(x, y + 8), new Point(x + 8, y + 8), new Point(x + 4, y) }
                : new[] { new Point(x, y), new Point(x + 8, y), new Point(x + 4, y + 8) };
            g.FillPolygon(br, pts);
        }

        // ── Row ──────────────────────────────────────────────

        private void DrawRow(Graphics g, int rowIndex, int y, int sd, int gridW)
        {
            Color baseBg = rowIndex == _selectedRow ? _rowSelected
                         : rowIndex % 2 == 0 ? _rowEven
                         : _rowOdd;

            Color bg = baseBg;
            if (rowIndex == _hoverRow && rowIndex != _selectedRow)
            {
                float a = _smoothHover ? _hoverAlpha : 1f;
                bg = BlendColor(baseBg, _rowHover, a);
            }

            using (var brush = new SolidBrush(bg))
                g.FillRectangle(brush, sd, y, gridW, _rowHeight);

            g.DrawLine(_separatorPen, sd, y + _rowHeight - 1, sd + gridW, y + _rowHeight - 1); // ✦ cached pen

            // ✦ accent bar — يسار LTR، يمين RTL
            if (rowIndex == _selectedRow)
            {
                using var ab = new SolidBrush(_headerBackground);
                int barX = _isRtl ? sd + gridW - 3 : sd;
                g.FillRectangle(ab, barX, y + 6, 3, _rowHeight - 12);
            }

            var row = _rows[rowIndex];
            int x = sd;

            foreach (int c in _colIndices)
            {
                var col = _columns[c];
                int w = _computedWidths[c];
                var cr = new Rectangle(x, y, w, _rowHeight);
                string val = row.TryGetValue(col.Field, out var v) ? v?.ToString() ?? "" : "";

                switch (col.CellType)
                {
                    case GridCellType.Avatar: DrawAvatarCell(g, cr, val, rowIndex); break;
                    case GridCellType.Badge: DrawBadgeCell(g, cr, val); break;
                    case GridCellType.Currency: DrawTextCell(g, cr, "$" + val, StringAlignment.Center); break;
                    case GridCellType.Actions: DrawActionsCell(g, cr, rowIndex); break;
                    default: DrawTextCell(g, cr, val, StringAlignment.Center); break;
                }
                x += w;
            }
        }

        // ── Cell Renderers ───────────────────────────────────

        private void DrawTextCell(Graphics g, Rectangle rect, string text, StringAlignment align)
        {
            // ✦ في RTL: Near ↔ Far يتبادلان — Center يبقى ثابت
            if (_isRtl)
            {
                if (align == StringAlignment.Near) align = StringAlignment.Far;
                else if (align == StringAlignment.Far) align = StringAlignment.Near;
            }
            var sf = align == StringAlignment.Near ? _sfNear : align == StringAlignment.Far ? _sfFar : _sfCenter;
            var r = new Rectangle(rect.X + 6, rect.Y, rect.Width - 12, rect.Height);
            g.DrawString(text, Font, _rowTextBrush, r, sf);
        }

        private void DrawBadgeCell(Graphics g, Rectangle rect, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!_badgeStyles.TryGetValue(text, out var style))
                style = new BadgeStyle(Color.FromArgb(149, 165, 166), Color.White);

            SizeF sz = g.MeasureString(text, _cachedBadgeFont);  // ✦ cached font
            int bw = (int)sz.Width + 20, bh = 24;
            var bRect = new Rectangle(rect.X + (rect.Width - bw) / 2, rect.Y + (rect.Height - bh) / 2, bw, bh);

            using var path = RoundedPath(bRect, 12);
            using var bg = new SolidBrush(style.Background);
            g.FillPath(bg, path);

            using var fg = new SolidBrush(style.Foreground);
            g.DrawString(text, _cachedBadgeFont, fg, bRect, _sfCenter);  // ✦ cached font & format
        }

        private void DrawAvatarCell(Graphics g, Rectangle rect, string text, int rowIndex)
        {
            int d = 32, pad = 10;
            int cx, textX, textW;

            if (IsRtl)
            { cx = rect.Right - pad - d - 55; textX = rect.X + pad; textW = rect.Width - d - pad * 2 - 58; }
            else
            { cx = rect.X + pad; textX = cx + d + 8; textW = rect.Width - d - pad - 16; }

            int cy = rect.Y + (rect.Height - d) / 2;
            var circle = new Rectangle(cx, cy, d, d);

            using (var ab = new SolidBrush(_avatarColors[rowIndex % _avatarColors.Length]))
                g.FillEllipse(ab, circle);

            using var white = new SolidBrush(Color.White);
            g.DrawString(GetInitials(text), _cachedAvatarFont, white, circle, _sfCenter);  // ✦ cached

            var textSf = IsRtl ? _sfFar : _sfNear;
            var textRect = new Rectangle(textX, rect.Y, textW, rect.Height);
            g.DrawString(text, Font, _rowTextBrush, textRect, textSf);   // ✦ cached
        }

        private void DrawActionsCell(Graphics g, Rectangle rect, int rowIndex)
        {
            int iSz = 22, sp = 10, totalW = 3 * iSz + 2 * sp;
            int startX = rect.X + (rect.Width - totalW) / 2;
            int iconY = rect.Y + (rect.Height - iSz) / 2;

            DrawOneAction(g, IconView, new Rectangle(startX, iconY, iSz, iSz), Color.FromArgb(41, 128, 185), rowIndex == _hoverRow && _hoverBtn == ActionBtn.View);
            DrawOneAction(g, IconEdit, new Rectangle(startX + iSz + sp, iconY, iSz, iSz), Color.FromArgb(230, 126, 34), rowIndex == _hoverRow && _hoverBtn == ActionBtn.Edit);
            DrawOneAction(g, IconDelete, new Rectangle(startX + 2 * (iSz + sp), iconY, iSz, iSz), Color.FromArgb(192, 57, 43), rowIndex == _hoverRow && _hoverBtn == ActionBtn.Delete);
        }

        private void DrawOneAction(Graphics g, Image icon, Rectangle rect, Color color, bool hover)
        {
            if (hover)
            {
                var hr = new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8);
                using var hb = new SolidBrush(Color.FromArgb(35, color));
                g.FillEllipse(hb, hr);
            }
            if (icon != null) g.DrawImage(icon, rect);
            else { using var b = new SolidBrush(color); g.FillEllipse(b, rect); }
        }

        // ✦ Empty State ───────────────────────────────────────

        private void DrawEmpty(Graphics g, Rectangle area)
        {
            using var br = new SolidBrush(Color.FromArgb(140, 50, 50, 70));
            using var f = new Font(Font.FontFamily, Font.Size + 1f);
            g.DrawString(_emptyText, f, br, area, _sfCenter);
        }

        // ════════════════════════════════════════════════════════
        //  HELPERS  (لا تغيير جوهري)
        // ════════════════════════════════════════════════════════

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            // ✦ دعم أسماء عربية: نأخذ أول حرف حقيقي من كل كلمة
            var parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                foreach (char c in parts[0]) if (char.IsLetter(c)) return c.ToString().ToUpper();
                return "?";
            }
            char c1 = parts[0].FirstOrDefault(char.IsLetter);
            char c2 = parts[1].FirstOrDefault(char.IsLetter);
            return ((c1 == default ? "" : c1.ToString()) + (c2 == default ? "" : c2.ToString())).ToUpper();
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Max(0, Math.Min(radius, Math.Min(r.Width, r.Height) / 2)) * 2;
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

        // ════════════════════════════════════════════════════════
        //  DISPOSE  — تحرير كامل
        // ════════════════════════════════════════════════════════

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer?.Dispose();
                _shadowCache?.Dispose();
                _cachedHeaderFont?.Dispose();
                _cachedBadgeFont?.Dispose();
                _cachedAvatarFont?.Dispose();
                _headerBgBrush?.Dispose();
                _headerFgBrush?.Dispose();
                _rowTextBrush?.Dispose();
                _separatorPen?.Dispose();
                _outerBorderPen?.Dispose();
                _headerBorderPen?.Dispose();
                _sfCenter?.Dispose();
                _sfNear?.Dispose();
                _sfFar?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}