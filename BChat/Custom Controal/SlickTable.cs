using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat
{
    // ═══════════════════════════════════════════════════════════
    //  ENUMS & SUPPORTING CLASSES
    // ═══════════════════════════════════════════════════════════

    public enum GridCellType
    {
        Text,
        Badge,
        Avatar,
        Currency,
        Actions     // ← أزرار View / Edit / Delete داخل الصف
    }

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
    //  SLICK TABLE
    // ═══════════════════════════════════════════════════════════

    public class SlickTable : UserControl
    {
        // ── Data ─────────────────────────────────────────────
        private List<GridColumn> _columns = new List<GridColumn>();
        private List<Dictionary<string, object>> _rows = new List<Dictionary<string, object>>();

        // ── Layout ───────────────────────────────────────────
        private int _rowHeight = 52;
        private int _headerHeight = 44;
        private int _scrollOffset = 0;
        private int _selectedRow = -1;
        private int _hoverRow = -1;
        private int[] _computedWidths;

        // ── Colors ───────────────────────────────────────────
        public Color HeaderBackground { get; set; } = Color.FromArgb(22, 45, 90);
        public Color HeaderForeground { get; set; } = Color.White;
        public Color RowEven { get; set; } = Color.White;
        public Color RowOdd { get; set; } = Color.FromArgb(240, 247, 255);
        public Color RowSelected { get; set; } = Color.FromArgb(210, 230, 255);
        public Color RowHover { get; set; } = Color.FromArgb(225, 238, 255);
        public Color BorderColor { get; set; } = Color.FromArgb(220, 228, 240);

        // ── Action Icons (اختياري — لو ما ضبطتها تظهر دوائر ملونة) ──
        public Image IconView { get; set; }
        public Image IconEdit { get; set; }
        public Image IconDelete { get; set; }

        // ── Badge Styles ─────────────────────────────────────
        private Dictionary<string, BadgeStyle> _badgeStyles =
            new Dictionary<string, BadgeStyle>(StringComparer.OrdinalIgnoreCase)
        {
            { "Available",   new BadgeStyle(Color.FromArgb(39,  174, 96),  Color.White) },
            { "متاح",        new BadgeStyle(Color.FromArgb(39,  174, 96),  Color.White) },
            { "OnRent",      new BadgeStyle(Color.FromArgb(41,  128, 185), Color.White) },
            { "Rented",      new BadgeStyle(Color.FromArgb(41,  128, 185), Color.White) },
            { "مؤجرة",       new BadgeStyle(Color.FromArgb(41,  128, 185), Color.White) },
            { "InService",   new BadgeStyle(Color.FromArgb(230, 126, 34),  Color.White) },
            { "Maintenance", new BadgeStyle(Color.FromArgb(230, 126, 34),  Color.White) },
            { "صيانة",       new BadgeStyle(Color.FromArgb(230, 126, 34),  Color.White) },
            { "Confirmed",   new BadgeStyle(Color.FromArgb(39,  174, 96),  Color.White) },
            { "Pending",     new BadgeStyle(Color.FromArgb(243, 156, 18),  Color.White) },
            { "Cancelled",   new BadgeStyle(Color.FromArgb(192, 57,  43),  Color.White) },
            { "Active",      new BadgeStyle(Color.FromArgb(39,  174, 96),  Color.White) },
            { "Inactive",    new BadgeStyle(Color.FromArgb(149, 165, 166), Color.White) },
            { "Unavailable", new BadgeStyle(Color.FromArgb(149, 165, 166), Color.White) },
        };

        private readonly Color[] _avatarColors =
        {
            Color.FromArgb(52,  152, 219),
            Color.FromArgb(155, 89,  182),
            Color.FromArgb(46,  204, 113),
            Color.FromArgb(230, 126, 34),
            Color.FromArgb(231, 76,  60),
            Color.FromArgb(26,  188, 156),
            Color.FromArgb(41,  128, 185),
            Color.FromArgb(39,  174, 96),
        };

        // ── Action hover tracking ─────────────────────────────
        private enum ActionBtn { None, View, Edit, Delete }
        private ActionBtn _hoverBtn = ActionBtn.None;

        // ── Scroll ────────────────────────────────────────────
        private VScrollBar _vScroll;

        // ═══════════════════════════════════════════════════════
        //  EVENTS
        // ═══════════════════════════════════════════════════════
        public event EventHandler<int> RowClicked;
        public event EventHandler<int> ViewClicked;
        public event EventHandler<int> EditClicked;
        public event EventHandler<int> DeleteClicked;
        public bool IsRtl { get; set; } = false;

        // ═══════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════
        public SlickTable()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            _vScroll = new VScrollBar
            {
                Dock = DockStyle.Right,
                SmallChange = _rowHeight,
                LargeChange = _rowHeight * 5,
                Visible = false
            };
            _vScroll.Scroll += (s, e) => { _scrollOffset = _vScroll.Value; Invalidate(); };
            Controls.Add(_vScroll);

            MouseMove += OnMouseMove;
            MouseLeave += (s, e) => { _hoverRow = -1; _hoverBtn = ActionBtn.None; Cursor = Cursors.Default; Invalidate(); };
            MouseClick += OnMouseClick;
            MouseWheel += OnMouseWheel;
            Resize += (s, e) => { ComputeWidths(); UpdateScrollbar(); Invalidate(); };
        }

        // ═══════════════════════════════════════════════════════
        //  PUBLIC API
        // ═══════════════════════════════════════════════════════

        public void SetColumns(List<GridColumn> columns)
        {
            _columns = columns;
            ComputeWidths();
            Invalidate();
        }

        public void SetData(List<Dictionary<string, object>> data)
        {
            _rows = data ?? new List<Dictionary<string, object>>();
            _scrollOffset = 0;
            _selectedRow = -1;
            _hoverRow = -1;
            _hoverBtn = ActionBtn.None;
            UpdateScrollbar();
            Invalidate();
        }

        public void AddBadgeStyle(string key, Color bg, Color fg)
            => _badgeStyles[key] = new BadgeStyle(bg, fg);

        public Dictionary<string, object> GetSelectedRow()
            => (_selectedRow >= 0 && _selectedRow < _rows.Count) ? _rows[_selectedRow] : null;

        public int GetSelectedIndex() => _selectedRow;

        // ═══════════════════════════════════════════════════════
        //  COLUMN WIDTHS — يوزع المسافة الزائدة بالتناسب
        // ═══════════════════════════════════════════════════════

        private void ComputeWidths()
        {
            if (_columns == null || _columns.Count == 0) return;

            int scrollW = _vScroll.Visible ? _vScroll.Width : 0;
            int available = Math.Max(0, Width - scrollW);
            int total = 0;
            foreach (var c in _columns) total += c.Width;

            _computedWidths = new int[_columns.Count];

            if (available <= total || total == 0)
            {
                for (int i = 0; i < _columns.Count; i++)
                    _computedWidths[i] = _columns[i].Width;
                return;
            }

            // وزّع الفرق بالتناسب
            int extra = available - total;
            int distributed = 0;
            for (int i = 0; i < _columns.Count; i++)
            {
                int bonus = (i < _columns.Count - 1)
                    ? (int)((double)_columns[i].Width / total * extra)
                    : extra - distributed;
                _computedWidths[i] = _columns[i].Width + bonus;
                distributed += bonus;
            }
        }

        // ═══════════════════════════════════════════════════════
        //  SCROLLBAR
        // ═══════════════════════════════════════════════════════

        private void UpdateScrollbar()
        {
            int totalH = _rows.Count * _rowHeight;
            int visibleH = Height - _headerHeight;

            if (totalH > visibleH)
            {
                _vScroll.Visible = true;
                _vScroll.Maximum = totalH - visibleH + _vScroll.LargeChange;
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

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = -e.Delta / 3;
            _scrollOffset = Math.Max(0, _scrollOffset + delta);
            int maxScroll = Math.Max(0, _rows.Count * _rowHeight - (Height - _headerHeight));
            _scrollOffset = Math.Min(_scrollOffset, maxScroll);
            if (_vScroll.Visible)
                _vScroll.Value = Math.Min(_scrollOffset, _vScroll.Maximum - _vScroll.LargeChange + 1);
            Invalidate();
        }

        // ═══════════════════════════════════════════════════════
        //  HIT TEST
        // ═══════════════════════════════════════════════════════

        private int HitTestRow(int mouseY)
        {
            int ry = mouseY - _headerHeight + _scrollOffset;
            if (ry < 0) return -1;
            int row = ry / _rowHeight;
            return (row < _rows.Count) ? row : -1;
        }

        private ActionBtn HitTestActionBtn(int mouseX, int mouseY)
        {
            int row = HitTestRow(mouseY);
            if (row < 0 || _computedWidths == null) return ActionBtn.None;

            // نجيب x بداية عمود Actions
            int ax = 0;
            int aw = 0;
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i].CellType == GridCellType.Actions)
                { aw = _computedWidths[i]; break; }
                ax += _computedWidths[i];
            }
            if (aw == 0) return ActionBtn.None;

            int rowY = _headerHeight + row * _rowHeight - _scrollOffset;
            int iconSize = 22;
            int spacing = 10;
            int totalW = 3 * iconSize + 2 * spacing;
            int startX = ax + (aw - totalW) / 2;
            int iconY = rowY + (_rowHeight - iconSize) / 2;

            if (new Rectangle(startX, iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.View;
            if (new Rectangle(startX + iconSize + spacing, iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.Edit;
            if (new Rectangle(startX + 2 * (iconSize + spacing), iconY, iconSize, iconSize).Contains(mouseX, mouseY)) return ActionBtn.Delete;
            return ActionBtn.None;
        }

        // ═══════════════════════════════════════════════════════
        //  MOUSE EVENTS
        // ═══════════════════════════════════════════════════════

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int row = HitTestRow(e.Y);
            ActionBtn btn = HitTestActionBtn(e.X, e.Y);

            bool changed = (row != _hoverRow || btn != _hoverBtn);
            _hoverRow = row;
            _hoverBtn = btn;

            Cursor = (row >= 0) ? Cursors.Hand : Cursors.Default;
            if (changed) Invalidate();
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

        // ═══════════════════════════════════════════════════════
        //  PAINT
        // ═══════════════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_computedWidths == null || _computedWidths.Length != _columns.Count)
                ComputeWidths();

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int scrollW = _vScroll.Visible ? _vScroll.Width : 0;
            int gridWidth = Width - scrollW;

            DrawHeader(g, gridWidth);

            g.SetClip(new Rectangle(0, _headerHeight, gridWidth, Height - _headerHeight));
            for (int i = 0; i < _rows.Count; i++)
            {
                int y = _headerHeight + i * _rowHeight - _scrollOffset;
                if (y + _rowHeight < _headerHeight) continue;
                if (y > Height) break;
                DrawRow(g, i, y, gridWidth);
            }
            g.ResetClip();

            using (var pen = new Pen(BorderColor))
                g.DrawRectangle(pen, 0, 0, gridWidth - 1, Height - 1);
        }

        // ── Header ───────────────────────────────────────────

        private void DrawHeader(Graphics g, int gridWidth)
        {
            using (var brush = new SolidBrush(HeaderBackground))
                g.FillRectangle(brush, 0, 0, gridWidth, _headerHeight);

            using (var font = new Font(Font.FontFamily, 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(HeaderForeground))
            {
                int x = 0;
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

                    // نحرك النص قليلاً لليسار عشان يبعد عن الأفاتار
                    var rect = (IsRtl && isAvatar)
                        ? new Rectangle(x, 0, w - 100, _headerHeight)
                        : new Rectangle(x, 0, w, _headerHeight);
                    g.DrawString(_columns[i].Header, font, brush, rect, sf);
                    x += w;
                }
            }

            using (var pen = new Pen(Color.FromArgb(10, 30, 70), 2))
                g.DrawLine(pen, 0, _headerHeight, gridWidth, _headerHeight);
        }
        private IEnumerable<int> GetColumnIndices()
        {
            var indices = Enumerable.Range(0, _columns.Count);
            return IsRtl ? indices.Reverse() : indices;
        }
        // ── Row ──────────────────────────────────────────────


        private void DrawRow(Graphics g, int rowIndex, int y, int gridWidth)
        {

            Color bg = rowIndex == _selectedRow ? RowSelected
                     : rowIndex == _hoverRow ? RowHover
                     : rowIndex % 2 == 0 ? RowEven
                                                : RowOdd;

            using (var brush = new SolidBrush(bg))
                g.FillRectangle(brush, 0, y, gridWidth, _rowHeight);

            using (var pen = new Pen(BorderColor))
                g.DrawLine(pen, 0, y + _rowHeight - 1, gridWidth, y + _rowHeight - 1);

            var row = _rows[rowIndex];

            int x = 0;
            foreach (int c in GetColumnIndices())
            {
                var col = _columns[c];
                int w = _computedWidths[c];
                var cellRect = new Rectangle(x, y, w, _rowHeight);
                string val = row.ContainsKey(col.Field) ? row[col.Field]?.ToString() ?? "" : "";

                switch (col.CellType)
                {
                    case GridCellType.Avatar:
                        DrawAvatarCell(g, cellRect, val, rowIndex);
                        break;
                    case GridCellType.Badge:
                        DrawBadgeCell(g, cellRect, val);
                        break;
                    case GridCellType.Currency:
                        DrawTextCell(g, cellRect, "$" + val, StringAlignment.Center);
                        break;
                    case GridCellType.Actions:
                        DrawActionsCell(g, cellRect, rowIndex);
                        break;
                    default:
                        DrawTextCell(g, cellRect, val, StringAlignment.Center);
                        break;
                }
                x += w;
            }
        }

        // ── Cell Renderers ───────────────────────────────────

        private void DrawTextCell(Graphics g, Rectangle rect, string text, StringAlignment align)
        {
            var sf = new StringFormat
            {
                Alignment = align,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            var r = new Rectangle(rect.X + 6, rect.Y, rect.Width - 12, rect.Height);
            using (var brush = new SolidBrush(Color.FromArgb(50, 50, 70)))
                g.DrawString(text, Font, brush, r, sf);
        }

        private void DrawBadgeCell(Graphics g, Rectangle rect, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            BadgeStyle style;
            if (!_badgeStyles.TryGetValue(text, out style))
                style = new BadgeStyle(Color.FromArgb(149, 165, 166), Color.White);

            SizeF sz = g.MeasureString(text, Font);
            int bw = (int)sz.Width + 20;
            int bh = 24;
            int bx = rect.X + (rect.Width - bw) / 2;   // وسّط في العمود
            int by = rect.Y + (rect.Height - bh) / 2;
            var bRect = new Rectangle(bx, by, bw, bh);

            using (var path = RoundedRect(bRect, 12))
            using (var brush = new SolidBrush(style.Background))
                g.FillPath(brush, path);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (var font = new Font(Font.FontFamily, 8.5f, FontStyle.Bold))
            using (var brush = new SolidBrush(style.Foreground))
                g.DrawString(text, font, brush, bRect, sf);
        }

        private void DrawAvatarCell(Graphics g, Rectangle rect, string text, int rowIndex)
        {
            int diameter = 32;
            int padding = 10;

            int cx, textX, textW;

            if (IsRtl)
            {
                // الدائرة من اليمين
                cx = rect.Right - padding - diameter - 55;
                textX = rect.X + padding;
                textW = rect.Width - diameter - padding * 2 - 58;
            }
            else
            {
                // الدائرة من اليسار
                cx = rect.X + padding;
                textX = cx + diameter + 8;
                textW = rect.Width - diameter - padding - 16;
            }

            int cy = rect.Y + (rect.Height - diameter) / 2;
            var circle = new Rectangle(cx, cy, diameter, diameter);

            using (var brush = new SolidBrush(_avatarColors[rowIndex % _avatarColors.Length]))
                g.FillEllipse(brush, circle);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (var font = new Font(Font.FontFamily, 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
                g.DrawString(GetInitials(text), font, brush, circle, sf);

            var textRect = new Rectangle(textX, rect.Y, textW, rect.Height);
            var textSf = new StringFormat
            {
                Alignment = IsRtl ? StringAlignment.Far : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            using (var brush = new SolidBrush(Color.FromArgb(40, 40, 60)))
                g.DrawString(text, Font, brush, textRect, textSf);
        }
        private void DrawActionsCell(Graphics g, Rectangle rect, int rowIndex)
        {
            int iconSize = 22;
            int spacing = 10;
            int totalW = 3 * iconSize + 2 * spacing;
            int startX = rect.X + (rect.Width - totalW) / 2;
            int iconY = rect.Y + (rect.Height - iconSize) / 2;

            DrawOneAction(g, IconView,
                new Rectangle(startX, iconY, iconSize, iconSize),
                Color.FromArgb(41, 128, 185),
                rowIndex == _hoverRow && _hoverBtn == ActionBtn.View);

            DrawOneAction(g, IconEdit,
                new Rectangle(startX + iconSize + spacing, iconY, iconSize, iconSize),
                Color.FromArgb(230, 126, 34),
                rowIndex == _hoverRow && _hoverBtn == ActionBtn.Edit);

            DrawOneAction(g, IconDelete,
                new Rectangle(startX + 2 * (iconSize + spacing), iconY, iconSize, iconSize),
                Color.FromArgb(192, 57, 43),
                rowIndex == _hoverRow && _hoverBtn == ActionBtn.Delete);
        }

        private void DrawOneAction(Graphics g, Image icon, Rectangle rect, Color color, bool hover)
        {
            if (hover)
            {
                var hRect = new Rectangle(rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8);
                using (var b = new SolidBrush(Color.FromArgb(35, color)))
                    g.FillEllipse(b, hRect);
            }

            if (icon != null)
                g.DrawImage(icon, rect);
            else
            {
                // Fallback: دائرة صغيرة ملونة
                using (var b = new SolidBrush(color))
                    g.FillEllipse(b, rect);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper()
                : (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}