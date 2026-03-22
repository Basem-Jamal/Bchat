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
    //  ENUMS & SUPPORTING CLASSES
    // ═══════════════════════════════════════════════════════════

    public enum GridCellType { Text, Badge, Avatar, Currency, Actions }

    public class GridColumn
    {
        public string Header { get; set; }
        public string Field { get; set; }
        public int Width { get; set; } = 120;
        public GridCellType CellType { get; set; } = GridCellType.Text;
    }

    public class BadgeStyle
    {

        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public BadgeStyle(Color bg, Color fg) { Background = bg; Foreground = fg; }
    }

    // ═══════════════════════════════════════════════════════════
    //  SLICK TABLE  —  v2  (Border-Radius + Shadow + Smooth Hover)
    // ═══════════════════════════════════════════════════════════

    public class SlickTable : UserControl
    {

        // ── Data ─────────────────────────────────────────────
        private List<GridColumn> _columns = new();
        private List<Dictionary<string, object>> _rows = new();

        // ── Layout ───────────────────────────────────────────
        private int _rowHeight = 52;
        private int _headerHeight = 44;
        private int _scrollOffset = 0;
        private int _selectedRow = -1;
        private int _hoverRow = -1;
        private int[] _computedWidths;

        // ── Smooth Hover Animation ────────────────────────────
        private float _hoverAlpha = 0f;     // 0..1
        private int _animHoverRow = -1;
        private Timer _animTimer;

        // ── Shadow painting buffer ────────────────────────────
        private Bitmap _shadowCache;
        private Size _shadowCacheSize;

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Shape
        // ════════════════════════════════════════════════════════

        private int _borderRadius = 0;
        [Category("✦ Shape")]
        [DefaultValue(16)]
        [Description("نصف قطر زوايا الجدول كامل (0 = حواف حادة)")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); RebuildShadow(); Invalidate(); }
        }

        private int _shadowDepth = 0;
        [Category("✦ Shape")]
        [DefaultValue(12)]
        [Description("عمق الظل الخارجي (0 = بدون ظل)")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = Math.Max(0, Math.Min(40, value)); RebuildShadow(); Invalidate(); }
        }

        private Color _shadowColor = Color.FromArgb(60, 0, 0, 0);
        [Category("✦ Shape")]
        [Description("لون الظل — Alpha يتحكم في الكثافة")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; RebuildShadow(); Invalidate(); }
        }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Header
        // ════════════════════════════════════════════════════════

        private Color _headerBackground = Color.FromArgb(22, 45, 90);
        [Category("✦ Header")]
        [Description("لون خلفية الهيدر")]
        public Color HeaderBackground
        {
            get => _headerBackground;
            set { _headerBackground = value; Invalidate(); }
        }

        private Color _headerForeground = Color.White;
        [Category("✦ Header")]
        [Description("لون نص الهيدر")]
        public Color HeaderForeground
        {
            get => _headerForeground;
            set { _headerForeground = value; Invalidate(); }
        }

        private Color _headerBorderColor = Color.FromArgb(10, 30, 70);
        [Category("✦ Header")]
        [Description("لون الخط الفاصل تحت الهيدر")]
        public Color HeaderBorderColor
        {
            get => _headerBorderColor;
            set { _headerBorderColor = value; Invalidate(); }
        }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Rows
        // ════════════════════════════════════════════════════════

        private Color _rowEven = Color.White;
        [Category("✦ Rows")]
        [Description("لون صفوف الأرقام الزوجية")]
        public Color RowEven
        {
            get => _rowEven;
            set { _rowEven = value; Invalidate(); }
        }

        private Color _rowOdd = Color.FromArgb(240, 247, 255);
        [Category("✦ Rows")]
        [Description("لون صفوف الأرقام الفردية")]
        public Color RowOdd
        {
            get => _rowOdd;
            set { _rowOdd = value; Invalidate(); }
        }

        private Color _rowSelected = Color.FromArgb(210, 230, 255);
        [Category("✦ Rows")]
        [Description("لون الصف المحدد")]
        public Color RowSelected
        {
            get => _rowSelected;
            set { _rowSelected = value; Invalidate(); }
        }

        private Color _rowHover = Color.FromArgb(225, 238, 255);
        [Category("✦ Rows")]
        [Description("لون الصف عند تمرير الماوس")]
        public Color RowHover
        {
            get => _rowHover;
            set { _rowHover = value; Invalidate(); }
        }

        private int _rowHeight2 = 52;
        [Category("✦ Rows")]
        [DefaultValue(52)]
        [Description("ارتفاع كل صف بالبكسل")]
        public int RowHeight
        {
            get => _rowHeight2;
            set { _rowHeight2 = Math.Max(24, value); _rowHeight = _rowHeight2; UpdateScrollbar(); Invalidate(); }
        }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Grid Lines
        // ════════════════════════════════════════════════════════

        private Color _borderColor = Color.FromArgb(220, 228, 240);
        [Category("✦ Grid Lines")]
        [Description("لون الخطوط الفاصلة بين الصفوف")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        private Color _outerBorderColor = Color.FromArgb(200, 215, 235);
        [Category("✦ Grid Lines")]
        [Description("لون حد الجدول الخارجي")]
        public Color OuterBorderColor
        {
            get => _outerBorderColor;
            set { _outerBorderColor = value; Invalidate(); }
        }

        private bool _showOuterBorder = true;
        [Category("✦ Grid Lines")]
        [DefaultValue(true)]
        [Description("إظهار الحد الخارجي للجدول")]
        public bool ShowOuterBorder
        {
            get => _showOuterBorder;
            set { _showOuterBorder = value; Invalidate(); }
        }

        // ════════════════════════════════════════════════════════
        //  ✦ DESIGNER PROPERTIES — ✦ Hover Animation
        // ════════════════════════════════════════════════════════

        private bool _smoothHover = true;
        [Category("✦ Hover Animation")]
        [DefaultValue(true)]
        [Description("تفعيل أنيمشن ناعم عند تمرير الماوس على الصفوف")]
        public bool SmoothHover
        {
            get => _smoothHover;
            set { _smoothHover = value; }
        }

        private int _hoverSpeed = 25;
        [Category("✦ Hover Animation")]
        [DefaultValue(25)]
        [Description("سرعة الأنيمشن — أقل = أسرع (5..60)")]
        public int HoverSpeed
        {
            get => _hoverSpeed;
            set { _hoverSpeed = Math.Max(5, Math.Min(60, value)); _animTimer.Interval = _hoverSpeed; }
        }

        // ════════════════════════════════════════════════════════
        //  ✦ RTL
        // ════════════════════════════════════════════════════════
        [Category("✦ Layout")]
        [DefaultValue(false)]
        [Description("عكس اتجاه الجدول — عربي RTL")]
        public bool IsRtl { get; set; } = false;

        // ── Action Icons ──────────────────────────────────────
        public Image IconView { get; set; }
        public Image IconEdit { get; set; }
        public Image IconDelete { get; set; }

        // ── Badge Styles ─────────────────────────────────────
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
        //  EVENTS
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
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Font = new Font("IBM Plex Sans Arabic", 13.5f, FontStyle.Regular);

            // ── Scroll ────────────────────────────────────────
            _vScroll = new VScrollBar
            {
                Dock = DockStyle.Right,
                SmallChange = _rowHeight,
                LargeChange = _rowHeight * 5,
                Visible = false
            };
            _vScroll.Scroll += (s, e) => { _scrollOffset = _vScroll.Value; Invalidate(); };
            Controls.Add(_vScroll);

            // ── Smooth Hover Timer ────────────────────────────
            _animTimer = new Timer { Interval = _hoverSpeed, Enabled = false };
            _animTimer.Tick += AnimTick;

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            MouseClick += OnMouseClick;
            MouseWheel += OnMouseWheel;
            Resize += (s, e) => { ComputeWidths(); UpdateScrollbar(); RebuildShadow(); Invalidate(); };
        }

        // ════════════════════════════════════════════════════════
        //  PUBLIC API
        // ════════════════════════════════════════════════════════

        public void SetColumns(List<GridColumn> columns)
        { _columns = columns; ComputeWidths(); Invalidate(); }

        public void SetData(List<Dictionary<string, object>> data)
        {
            _rows = data ?? new();
            _scrollOffset = 0; _selectedRow = -1; _hoverRow = -1; _hoverBtn = ActionBtn.None;
            UpdateScrollbar(); Invalidate();
        }

        public void AddBadgeStyle(string key, Color bg, Color fg)
            => _badgeStyles[key] = new BadgeStyle(bg, fg);

        public Dictionary<string, object> GetSelectedRow()
            => (_selectedRow >= 0 && _selectedRow < _rows.Count) ? _rows[_selectedRow] : null;

        public int GetSelectedIndex() => _selectedRow;

        // ════════════════════════════════════════════════════════
        //  SMOOTH HOVER ANIMATION
        // ════════════════════════════════════════════════════════

        private void AnimTick(object s, EventArgs e)
        {
            if (!_smoothHover) { _animTimer.Stop(); return; }

            bool entering = (_animHoverRow == _hoverRow && _hoverRow >= 0);
            float step = 0.12f;

            if (entering)
                _hoverAlpha = Math.Min(1f, _hoverAlpha + step);
            else
                _hoverAlpha = Math.Max(0f, _hoverAlpha - step);

            Invalidate();

            if (_hoverAlpha <= 0f || _hoverAlpha >= 1f)
                _animTimer.Stop();
        }

        // ════════════════════════════════════════════════════════
        //  SHADOW — Gaussian-style omni shadow
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

            var cardRect = new Rectangle(sd, sd, Width - 1, Height - 1);
            using var path = RoundedPath(cardRect, _borderRadius);

            // رسم طبقات ظل متراكبة
            for (int i = sd; i >= 1; i--)
            {
                float t = 1f - (float)i / sd;
                int alpha = (int)(_shadowColor.A * t * t * 0.6f);
                alpha = Math.Max(0, Math.Min(255, alpha));
                if (alpha == 0) continue;

                var sr = new Rectangle(sd - i, sd - i, Width - 1 + i * 2, Height - 1 + i * 2);
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
            {
                _vScroll.Visible = false; _scrollOffset = 0; ComputeWidths();
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = -e.Delta / 3;
            _scrollOffset = Math.Max(0, _scrollOffset + delta);
            int maxScroll = Math.Max(0, _rows.Count * _rowHeight - (Height - _headerHeight - _shadowDepth * 2));
            _scrollOffset = Math.Min(_scrollOffset, maxScroll);
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
            return (row < _rows.Count) ? row : -1;
        }

        private ActionBtn HitTestActionBtn(int mouseX, int mouseY)
        {
            int row = HitTestRow(mouseY);
            if (row < 0 || _computedWidths == null) return ActionBtn.None;

            int ax = _shadowDepth, aw = 0;
            foreach (int i in GetColumnIndices())
            {
                if (_columns[i].CellType == GridCellType.Actions) { aw = _computedWidths[i]; break; }
                ax += _computedWidths[i];
            }
            if (aw == 0) return ActionBtn.None;

            int rowY = _headerHeight + _shadowDepth + row * _rowHeight - _scrollOffset;
            int iconSize = 22, spacing = 10;
            int totalW = 3 * iconSize + 2 * spacing;
            int startX = ax + (aw - totalW) / 2;
            int iconY = rowY + (_rowHeight - iconSize) / 2;

            if (new Rectangle(startX, iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.View;
            if (new Rectangle(startX + iconSize + spacing, iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.Edit;
            if (new Rectangle(startX + 2 * (iconSize + spacing), iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.Delete;
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
                _animHoverRow = row;
                if (row >= 0) { _hoverAlpha = 0f; _animTimer.Start(); }
                else { _animTimer.Start(); }
            }

            _hoverRow = row; _hoverBtn = btn;
            Cursor = (row >= 0) ? Cursors.Hand : Cursors.Default;
            if (changed) Invalidate();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            _hoverRow = -1; _hoverBtn = ActionBtn.None;
            if (_smoothHover) { _animHoverRow = -1; _animTimer.Start(); }
            Cursor = Cursors.Default;
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
        //  PAINT
        // ════════════════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
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
                if (_shadowCache != null)
                    g.DrawImage(_shadowCache, -sd, -sd);
            }

            // ── 2. Clip to rounded card ───────────────────────
            using var cardPath = RoundedPath(cardRect, _borderRadius);
            g.SetClip(cardPath);

            // ── 3. Header ─────────────────────────────────────
            DrawHeader(g, sd, gridW);

            // ── 4. Rows ───────────────────────────────────────
            var rowClip = new Rectangle(sd, sd + _headerHeight, gridW, gridH - _headerHeight);
            using (var rowClipRegion = RectangleToRegion(rowClip))
            {
                g.SetClip(rowClipRegion, CombineMode.Replace);
                for (int i = 0; i < _rows.Count; i++)
                {
                    int y = sd + _headerHeight + i * _rowHeight - _scrollOffset;
                    if (y + _rowHeight < sd + _headerHeight) continue;
                    if (y > Height) break;
                    DrawRow(g, i, y, sd, gridW);
                }
                g.ResetClip();
            }

            g.ResetClip();
            g.SetClip(cardPath);

            // ── 5. Outer border ───────────────────────────────
            if (_showOuterBorder)
            {
                using var pen = new Pen(_outerBorderColor, 1f);
                g.DrawPath(pen, cardPath);
            }

            g.ResetClip();
        }

        private static Region RectangleToRegion(Rectangle r)
        {
            using var path = new GraphicsPath();
            path.AddRectangle(r);
            return new Region(path);
        }

        // ── Header ───────────────────────────────────────────

        private void DrawHeader(Graphics g, int sd, int gridW)
        {
            var headerRect = new Rectangle(sd, sd, gridW, _headerHeight);
            using var br = new SolidBrush(_headerBackground);
            g.FillRectangle(br, headerRect);

            using var font = new Font(Font.FontFamily, 15f, FontStyle.Bold);
            using var fg = new SolidBrush(_headerForeground);

            int x = sd;
            foreach (int i in GetColumnIndices())
            {
                int w = _computedWidths[i];
                bool isAvatar = _columns[i].CellType == GridCellType.Avatar;
                var sf = new StringFormat
                {
                    Alignment = (IsRtl && isAvatar) ? StringAlignment.Far : StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                var rect = (IsRtl && isAvatar)
                    ? new Rectangle(x, sd, w - 100, _headerHeight)
                    : new Rectangle(x, sd, w, _headerHeight);
                g.DrawString(_columns[i].Header, font, fg, rect, sf);
                x += w;
            }

            using var pen = new Pen(_headerBorderColor, 2f);
            g.DrawLine(pen, sd, sd + _headerHeight, sd + gridW, sd + _headerHeight);
        }

        // ── Row ─────────────────────────────────────────────-

        private void DrawRow(Graphics g, int rowIndex, int y, int sd, int gridW)
        {
            Color baseBg = rowIndex == _selectedRow ? _rowSelected
                         : rowIndex % 2 == 0 ? _rowEven
                         : _rowOdd;

            // Smooth hover blend
            Color bg = baseBg;
            if (rowIndex == _hoverRow && rowIndex != _selectedRow)
            {
                float a = _smoothHover ? _hoverAlpha : 1f;
                bg = BlendColor(baseBg, _rowHover, a);
            }

            using (var brush = new SolidBrush(bg))
                g.FillRectangle(brush, sd, y, gridW, _rowHeight);

            using (var pen = new Pen(_borderColor))
                g.DrawLine(pen, sd, y + _rowHeight - 1, sd + gridW, y + _rowHeight - 1);

            var row = _rows[rowIndex];
            int x = sd;

            foreach (int c in GetColumnIndices())
            {
                var col = _columns[c];
                int w = _computedWidths[c];
                var cellRect = new Rectangle(x, y, w, _rowHeight);
                string val = row.ContainsKey(col.Field) ? row[col.Field]?.ToString() ?? "" : "";

                switch (col.CellType)
                {
                    case GridCellType.Avatar: DrawAvatarCell(g, cellRect, val, rowIndex); break;
                    case GridCellType.Badge: DrawBadgeCell(g, cellRect, val); break;
                    case GridCellType.Currency: DrawTextCell(g, cellRect, "$" + val, StringAlignment.Center); break;
                    case GridCellType.Actions: DrawActionsCell(g, cellRect, rowIndex); break;
                    default: DrawTextCell(g, cellRect, val, StringAlignment.Center); break;
                }
                x += w;
            }
        }

        // ── Cell Renderers ───────────────────────────────────

        private void DrawTextCell(Graphics g, Rectangle rect, string text, StringAlignment align)
        {
            var sf = new StringFormat { Alignment = align, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            var r = new Rectangle(rect.X + 6, rect.Y, rect.Width - 12, rect.Height);
            using var brush = new SolidBrush(Color.FromArgb(50, 50, 70));
            g.DrawString(text, Font, brush, r, sf);
        }

        private void DrawBadgeCell(Graphics g, Rectangle rect, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!_badgeStyles.TryGetValue(text, out var style))
                style = new BadgeStyle(Color.FromArgb(149, 165, 166), Color.White);

            SizeF sz = g.MeasureString(text, Font);
            int bw = (int)sz.Width + 20, bh = 24;
            int bx = rect.X + (rect.Width - bw) / 2;
            int by = rect.Y + (rect.Height - bh) / 2;
            var bRect = new Rectangle(bx, by, bw, bh);

            using var path = RoundedPath(bRect, 12);
            using var brush = new SolidBrush(style.Background);
            g.FillPath(brush, path);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var font2 = new Font(Font.FontFamily, 8.5f, FontStyle.Bold);
            using var fg = new SolidBrush(style.Foreground);
            g.DrawString(text, font2, fg, bRect, sf);
        }

        private void DrawAvatarCell(Graphics g, Rectangle rect, string text, int rowIndex)
        {
            int diameter = 32, padding = 10;
            int cx, textX, textW;

            if (IsRtl)
            { cx = rect.Right - padding - diameter - 55; textX = rect.X + padding; textW = rect.Width - diameter - padding * 2 - 58; }
            else
            { cx = rect.X + padding; textX = cx + diameter + 8; textW = rect.Width - diameter - padding - 16; }

            int cy = rect.Y + (rect.Height - diameter) / 2;
            var circle = new Rectangle(cx, cy, diameter, diameter);

            using (var brush = new SolidBrush(_avatarColors[rowIndex % _avatarColors.Length]))
                g.FillEllipse(brush, circle);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var font2 = new Font(Font.FontFamily, 9f, FontStyle.Bold);
            using var white = new SolidBrush(Color.White);
            g.DrawString(GetInitials(text), font2, white, circle, sf);

            var textRect = new Rectangle(textX, rect.Y, textW, rect.Height);
            var textSf = new StringFormat
            {
                Alignment = IsRtl ? StringAlignment.Far : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            using var dark = new SolidBrush(Color.FromArgb(40, 40, 60));
            g.DrawString(text, Font, dark, textRect, textSf);
        }

        private void DrawActionsCell(Graphics g, Rectangle rect, int rowIndex)
        {
            int iconSize = 22, spacing = 10;
            int totalW = 3 * iconSize + 2 * spacing;
            int startX = rect.X + (rect.Width - totalW) / 2;
            int iconY = rect.Y + (rect.Height - iconSize) / 2;

            DrawOneAction(g, IconView, new Rectangle(startX, iconY, iconSize, iconSize), Color.FromArgb(41, 128, 185), rowIndex == _hoverRow && _hoverBtn == ActionBtn.View);
            DrawOneAction(g, IconEdit, new Rectangle(startX + iconSize + spacing, iconY, iconSize, iconSize), Color.FromArgb(230, 126, 34), rowIndex == _hoverRow && _hoverBtn == ActionBtn.Edit);
            DrawOneAction(g, IconDelete, new Rectangle(startX + 2 * (iconSize + spacing), iconY, iconSize, iconSize), Color.FromArgb(192, 57, 43), rowIndex == _hoverRow && _hoverBtn == ActionBtn.Delete);
        }

        private void DrawOneAction(Graphics g, Image icon, Rectangle rect, Color color, bool hover)
        {
            if (hover)
            {
                var hRect = new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8);
                using var b = new SolidBrush(Color.FromArgb(35, color));
                g.FillEllipse(b, hRect);
            }
            if (icon != null) g.DrawImage(icon, rect);
            else { using var b = new SolidBrush(color); g.FillEllipse(b, rect); }
        }

        // ════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════

        private IEnumerable<int> GetColumnIndices()
        {
            var idx = Enumerable.Range(0, _columns.Count);
            return IsRtl ? idx.Reverse() : idx;
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper()
                : (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer?.Dispose();
                _shadowCache?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}