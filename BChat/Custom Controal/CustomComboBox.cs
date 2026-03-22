using BChat.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [ToolboxItem(true)]
    [DefaultEvent("SelectedIndexChanged")]
    public class AdvancedComboBox : Control
    {
        // ── ألوان ────────────────────────────────────────────────
        private Color _accent = Color.FromArgb(94, 148, 255);
        private Color _borderColor = Color.FromArgb(210, 214, 220);
        private Color _backColorEx = Color.White;
        private Color _textColor = Color.FromArgb(35, 35, 35);
        private int _radius = 10;
        private bool _hover, _focused;

        // ── بيانات ──────────────────────────────────────────────
        private int _selectedIndex = -1;
        private Rectangle _arrowRect;
        private PopupList _popup;

        public event EventHandler SelectedIndexChanged;

        // ═══════════════════════════════════════════════════════
        public AdvancedComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            Font = new Font("Segoe UI", 10f);
            Size = new Size(210, 40);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
        }

        // ═══════════════════════════════════════════════════════
        //   خصائص Appearance
        // ═══════════════════════════════════════════════════════
        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accent;
            set { _accent = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BackColorEx
        {
            get => _backColorEx;
            set { _backColorEx = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        [Category("Layout")]
        public int BorderRadius
        {
            get => _radius;
            set { _radius = Math.Max(4, value); Invalidate(); }
        }

        // ═══════════════════════════════════════════════════════
        //   📋 Items — يقبل أي عدد من العناصر
        // ═══════════════════════════════════════════════════════
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
                typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> Items { get; private set; } = new List<string>();

        // ═══════════════════════════════════════════════════════
        //   SelectedIndex / SelectedItem
        // ═══════════════════════════════════════════════════════
        [Category("Behavior")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex) return;
                if (value < -1 || value >= Items.Count) return;
                _selectedIndex = value;
                Invalidate();
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public string SelectedItem
        {
            get => _selectedIndex >= 0 && _selectedIndex < Items.Count
                   ? Items[_selectedIndex] : "";
            set
            {
                int idx = Items.IndexOf(value);
                if (idx >= 0) SelectedIndex = idx;
            }
        }

        public Array DataSource { get; internal set; }

        // ═══════════════════════════════════════════════════════
        //   🖌 رسم الـ Control
        // ═══════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Color.White);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Color border = _focused ? _accent : _borderColor;

            using (GraphicsPath path = Rounded(rect, _radius))
            using (SolidBrush sb = new SolidBrush(_backColorEx))
            using (Pen pen = new Pen(border, 1.6f))
            {
                g.FillPath(sb, path);
                g.DrawPath(pen, path);
            }

            // ── السهم ▼ ──────────────────────────────────────────
            int arrowW = 24;
            _arrowRect = new Rectangle(Width - arrowW - 4, 0, arrowW + 4, Height);
            DrawArrow(g, _arrowRect, _focused ? _accent : Color.Gray);

            // ── النص ─────────────────────────────────────────────
            string text = SelectedItem;
            var textRect = new Rectangle(12, 0, Width - arrowW - 20, Height);
            using (SolidBrush tb = new SolidBrush(_textColor))
            {
                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(string.IsNullOrEmpty(text) ? "Select..." : text, Font, tb, textRect, sf);
            }

            // ── Hover / Focus ────────────────────────────────────
            if (_hover || _focused)
            {
                using (SolidBrush hb = new SolidBrush(Color.FromArgb(_focused ? 12 : 8, _accent)))
                using (GraphicsPath hp = Rounded(rect, _radius))
                    g.FillPath(hb, hp);
            }
        }

        private void DrawArrow(Graphics g, Rectangle rect, Color color)
        {
            int midX = rect.Left + rect.Width / 2;
            int midY = rect.Top + rect.Height / 2;
            using (Pen p = new Pen(color, 2f))
                g.DrawLines(p, new[]
                {
                    new Point(midX - 4, midY - 2),
                    new Point(midX,     midY + 3),
                    new Point(midX + 4, midY - 2)
                });
        }

        // ═══════════════════════════════════════════════════════
        //   نقر → فتح القائمة
        // ═══════════════════════════════════════════════════════
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ShowPopup();
        }

        private void ShowPopup()
        {
            if (_popup != null && !_popup.IsDisposed && _popup.Visible)
            {
                _popup.Close();
                return;
            }

            if (Items.Count == 0) return;

            _popup = new PopupList(
                this, Items, _accent, _backColorEx, _textColor, _selectedIndex);

            Point screen = PointToScreen(new Point(0, Height + 2));
            _popup.StartPosition = FormStartPosition.Manual;
            _popup.Location = screen;
            _popup.ItemSelected += (s, idx) => SelectedIndex = idx;
            _popup.Show(this);
        }

        // ═══════════════════════════════════════════════════════
        //   أحداث
        // ═══════════════════════════════════════════════════════
        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool h = ClientRectangle.Contains(e.Location);
            if (h != _hover) { _hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnGotFocus(EventArgs e) { _focused = true; Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { _focused = false; Invalidate(); base.OnLostFocus(e); }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ═══════════════════════════════════════════════════════
        //   🗂 PopupList — قائمة منسدلة بـ Virtual Scrolling
        //   تعمل مع أي عدد من العناصر (آلاف العناصر بدون مشكلة)
        // ═══════════════════════════════════════════════════════
        private class PopupList : Form
        {
            // ── ثوابت التصميم ────────────────────────────────────
            private const int ITEM_H = 34;   // ارتفاع كل عنصر
            private const int SCROLL_W = 10;   // عرض شريط التمرير
            private const int VISIBLE_MAX = 8;    // أقصى عدد عناصر مرئية في آن واحد
            private const int PADDING_X = 12;   // مسافة النص من اليسار

            // ── مراجع ────────────────────────────────────────────
            private readonly AdvancedComboBox _owner;
            private readonly List<string> _items;
            private readonly Color _accent, _back, _text;
            private readonly Font _font;

            // ── حالة التمرير ─────────────────────────────────────
            private int _scrollOffset = 0;   // أول عنصر مرئي
            private int _hoverIndex = -1;
            private int _selectedIndex;
            private bool _draggingScroll = false;
            private int _dragStartY;
            private int _dragStartOffset;

            // ── مناطق الرسم المُحسَبة ────────────────────────────
            private Rectangle _listRect;   // منطقة القائمة
            private Rectangle _scrollRect; // منطقة شريط التمرير الكامل
            private Rectangle _thumbRect;  // مقبض التمرير

            public event EventHandler<int> ItemSelected;

            // ── البناء ───────────────────────────────────────────
            public PopupList(AdvancedComboBox owner, List<string> items,
                             Color accent, Color back, Color text, int selected)
            {
                _owner = owner;
                _items = items;
                _accent = accent;
                _back = back;
                _text = text;
                _selectedIndex = selected;
                _font = owner.Font;

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                DoubleBuffered = true;
                AutoSize = false;
                BackColor = _back;

                // ── الحجم: أقصاه VISIBLE_MAX عنصر ─────────────────
                int visibleCount = Math.Min(_items.Count, VISIBLE_MAX);
                Width = owner.Width;
                Height = visibleCount * ITEM_H + 4;

                // ── حساب مناطق الرسم ──────────────────────────────
                bool needsScroll = _items.Count > VISIBLE_MAX;
                int listW = needsScroll ? Width - SCROLL_W - 1 : Width;
                _listRect = new Rectangle(0, 0, listW, Height);
                _scrollRect = new Rectangle(listW + 1, 0, SCROLL_W - 1, Height);

                // ── إذا كان العنصر المحدد خارج النطاق المرئي ────────
                if (_selectedIndex >= 0)
                    EnsureVisible(_selectedIndex);

                // ── إغلاق عند فقدان الفوكس ──────────────────────────
                Deactivate += (s, e) => Close();
            }

            // ═══════════════════════════════════════════════════
            //   🖌 الرسم الرئيسي (Virtual Rendering)
            //   فقط العناصر المرئية تُرسم — مهما كان الإجمالي
            // ═══════════════════════════════════════════════════
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(_back);

                // ── رسم العناصر المرئية فقط ───────────────────────
                int firstVisible = _scrollOffset;
                int lastVisible = Math.Min(_scrollOffset + VISIBLE_MAX, _items.Count) - 1;

                for (int i = firstVisible; i <= lastVisible; i++)
                {
                    int drawY = (i - _scrollOffset) * ITEM_H + 2;
                    var rect = new Rectangle(0, drawY, _listRect.Width, ITEM_H);
                    bool isSel = (i == _selectedIndex);
                    bool isHov = (i == _hoverIndex);

                    // خلفية العنصر
                    if (isSel)
                    {
                        using (var b = new SolidBrush(_accent))
                            g.FillRectangle(b, rect);
                    }
                    else if (isHov)
                    {
                        using (var b = new SolidBrush(Color.FromArgb(20, _accent)))
                            g.FillRectangle(b, rect);
                    }

                    // ── علامة ✓ للعنصر المحدد ──────────────────────
                    if (isSel)
                    {
                        using (var p = new Pen(Color.White, 1.5f))
                        {
                            int cx = rect.Right - 18, cy = rect.Top + ITEM_H / 2;
                            g.DrawLines(p, new[]
                            {
                                new Point(cx - 4, cy),
                                new Point(cx - 1, cy + 4),
                                new Point(cx + 5, cy - 4)
                            });
                        }
                    }

                    // النص
                    Color fc = isSel ? Color.White : _text;
                    var tr = new Rectangle(PADDING_X, drawY, _listRect.Width - PADDING_X - 24, ITEM_H);
                    TextRenderer.DrawText(g, _items[i], _font, tr, fc,
                        TextFormatFlags.VerticalCenter |
                        TextFormatFlags.Left |
                        TextFormatFlags.EndEllipsis);

                    // فاصل خفيف
                    if (i < lastVisible)
                    {
                        using (var p = new Pen(Color.FromArgb(15, 0, 0, 0), 1))
                            g.DrawLine(p, PADDING_X, drawY + ITEM_H - 1,
                                       _listRect.Width - PADDING_X, drawY + ITEM_H - 1);
                    }
                }

                // ── شريط التمرير (فقط إذا لزم) ─────────────────────
                if (_items.Count > VISIBLE_MAX)
                    DrawScrollBar(g);

                // ── الحد الخارجي ──────────────────────────────────
                using (var p = new Pen(_accent, 1.4f))
                    g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
            }

            // ── رسم شريط التمرير ──────────────────────────────────
            private void DrawScrollBar(Graphics g)
            {
                // خلفية الشريط
                using (var b = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                    g.FillRectangle(b, _scrollRect);

                // حساب مقبض التمرير
                _thumbRect = GetThumbRect();

                // رسم المقبض
                using (var b = new SolidBrush(Color.FromArgb(160, _accent)))
                using (var path = RoundedRect(_thumbRect, 4))
                    g.FillPath(b, path);
            }

            // ── حساب موضع وحجم مقبض التمرير ──────────────────────
            private Rectangle GetThumbRect()
            {
                int totalItems = _items.Count;
                int visibleItems = VISIBLE_MAX;

                // نسبة حجم المقبض
                float thumbRatio = (float)visibleItems / totalItems;
                int thumbH = Math.Max(20, (int)((_scrollRect.Height - 4) * thumbRatio));

                // نسبة موقع المقبض
                float posRatio = (float)_scrollOffset / Math.Max(1, totalItems - visibleItems);
                int thumbY = _scrollRect.Top + 2 +
                                 (int)(posRatio * (_scrollRect.Height - 4 - thumbH));

                return new Rectangle(_scrollRect.Left + 1, thumbY, _scrollRect.Width - 2, thumbH);
            }

            // ═══════════════════════════════════════════════════
            //   🖱 أحداث الفأرة
            // ═══════════════════════════════════════════════════
            protected override void OnMouseMove(MouseEventArgs e)
            {
                // ── سحب مقبض التمرير ──────────────────────────────
                if (_draggingScroll)
                {
                    int dy = e.Y - _dragStartY;
                    int trackH = _scrollRect.Height - 4 - _thumbRect.Height;
                    float ratio = trackH > 0 ? (float)dy / trackH : 0;
                    int maxOffset = _items.Count - VISIBLE_MAX;
                    int newOffset = _dragStartOffset + (int)(ratio * maxOffset);
                    SetScrollOffset(newOffset);
                    return;
                }

                // ── تحديد العنصر المُحوَّم عليه ──────────────────────
                if (_listRect.Contains(e.Location))
                {
                    int idx = _scrollOffset + (e.Y - 2) / ITEM_H;
                    int newHover = (idx >= 0 && idx < _items.Count) ? idx : -1;
                    if (newHover != _hoverIndex) { _hoverIndex = newHover; Invalidate(); }
                }
                else
                {
                    if (_hoverIndex != -1) { _hoverIndex = -1; Invalidate(); }
                }

                base.OnMouseMove(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                // بدء سحب مقبض التمرير
                if (_items.Count > VISIBLE_MAX && _thumbRect.Contains(e.Location))
                {
                    _draggingScroll = true;
                    _dragStartY = e.Y;
                    _dragStartOffset = _scrollOffset;
                    Capture = true;
                }
                base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (_draggingScroll)
                {
                    _draggingScroll = false;
                    Capture = false;
                }
                base.OnMouseUp(e);
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                if (_draggingScroll) return;

                // نقر على منطقة القائمة
                if (_listRect.Contains(e.Location))
                {
                    int idx = _scrollOffset + (e.Y - 2) / ITEM_H;
                    if (idx >= 0 && idx < _items.Count)
                    {
                        ItemSelected?.Invoke(this, idx);
                        Close();
                    }
                }
            }

            // ── عجلة الفأرة للتمرير السلس ─────────────────────────
            protected override void OnMouseWheel(MouseEventArgs e)
            {
                int delta = e.Delta > 0 ? -1 : 1;
                SetScrollOffset(_scrollOffset + delta * 3);
                base.OnMouseWheel(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _hoverIndex = -1;
                Invalidate();
                base.OnMouseLeave(e);
            }

            // ── تمرير بالكيبورد ───────────────────────────────────
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                if (keyData == Keys.Down)
                {
                    if (_hoverIndex < _items.Count - 1)
                    {
                        _hoverIndex++;
                        EnsureVisible(_hoverIndex);
                        Invalidate();
                    }
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    if (_hoverIndex > 0)
                    {
                        _hoverIndex--;
                        EnsureVisible(_hoverIndex);
                        Invalidate();
                    }
                    return true;
                }
                if (keyData == Keys.Enter && _hoverIndex >= 0)
                {
                    ItemSelected?.Invoke(this, _hoverIndex);
                    Close();
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    Close();
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // ═══════════════════════════════════════════════════
            //   🔧 مساعدات
            // ═══════════════════════════════════════════════════

            // ضبط offset مع التحقق من الحدود
            private void SetScrollOffset(int value)
            {
                int max = Math.Max(0, _items.Count - VISIBLE_MAX);
                int newOffset = Math.Max(0, Math.Min(value, max));
                if (newOffset != _scrollOffset)
                {
                    _scrollOffset = newOffset;
                    Invalidate();
                }
            }

            // التأكد أن العنصر ظاهر في النافذة
            private void EnsureVisible(int index)
            {
                if (index < _scrollOffset)
                    SetScrollOffset(index);
                else if (index >= _scrollOffset + VISIBLE_MAX)
                    SetScrollOffset(index - VISIBLE_MAX + 1);
            }

            private static GraphicsPath RoundedRect(Rectangle r, int rad)
            {
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

        public static implicit operator AdvancedComboBox(ModernComboBox v)
        {
            throw new NotImplementedException();
        }
    }
}