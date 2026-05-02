using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using BChat.Models.Report_Home;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    // ══════════════════════════════════════════════════════════════════════
    //  CustomerInsightsControl
    //
    //  Usage from outside:
    //
    //      var data = CustomerInsightsRepository.GetInsights();   // your class
    //      customerInsightsControl1.LoadData(data);
    //
    //  The Refresh button raises the RefreshRequested event so the FORM
    //  (or presenter) decides how to reload:
    //
    //      customerInsightsControl1.RefreshRequested += (_, _) =>
    //      {
    //          var data = CustomerInsightsRepository.GetInsights();
    //          customerInsightsControl1.LoadData(data);
    //      };
    // ══════════════════════════════════════════════════════════════════════
    public sealed class CustomerInsightsControl: UserControl
    {
        // ── Design tokens (defined ONCE, used everywhere) ─────────────────
        private static readonly Color BgColor = ColorTranslator.FromHtml("#F8F7FF");
        private static readonly Color AccentColor = ColorTranslator.FromHtml("#7C6FF7");
        private static readonly Color CardBg = Color.White;
        private static readonly Color TxtDark = Color.FromArgb(30, 30, 50);
        private static readonly Color TxtMuted = Color.FromArgb(120, 120, 150);

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Raised when the user clicks the Refresh button.
        /// Subscribe from outside to reload data and call LoadData() again.
        /// </summary>
        public event EventHandler? RefreshRequested;

        /// <summary>
        /// Feed the control with pre-fetched data.
        /// Call this from your form / presenter after loading from the repository.
        /// </summary>
        public void LoadData(CustomerInsightsData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            _data = data;
            BuildCards();
        }

        // ── Private state ─────────────────────────────────────────────────
        private CustomerInsightsData? _data;
        private Panel _header = null!;
        private Label _titleLabel = null!;
        private Button _refreshBtn = null!;
        private FlowLayoutPanel _flow = null!;
        private Label _placeholder = null!;

        // ── Constructor ───────────────────────────────────────────────────
        public CustomerInsightsControl()
        {
            BuildLayout();
        }

        // ── Chrome ────────────────────────────────────────────────────────
        private void BuildLayout()
        {
            this.RightToLeft = RightToLeft.Yes;
            this.BackColor = BgColor;
            this.Font = SafeFont("Cairo", 10f);
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;

            // ── Header
            _header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White,
                Padding = new Padding(20, 0, 20, 0)
            };
            _header.Paint += (_, e) =>
            {
                using var sh = new SolidBrush(Color.FromArgb(12, 0, 0, 0));
                e.Graphics.FillRectangle(sh,
                    0, _header.Height - 4, _header.Width, 4);
            };

            _titleLabel = new Label
            {
                Text = "لوحة إحصاءات العملاء",
                Font = SafeFont("Cairo", 14f, FontStyle.Bold),
                ForeColor = TxtDark,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(0, 18)
            };

            _refreshBtn = new Button
            {
                Text = "↻  تحديث",
                Font = SafeFont("Cairo", 9f),
                ForeColor = Color.White,
                BackColor = AccentColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Location = new Point(20, 14),
                Cursor = Cursors.Hand
            };
            _refreshBtn.FlatAppearance.BorderSize = 0;
            _refreshBtn.Region = MakeRoundRegion(100, 36, 10);
            _refreshBtn.Click += (_, e) => RefreshRequested?.Invoke(this, e);

            _header.Controls.Add(_titleLabel);
            _header.Controls.Add(_refreshBtn);
            _header.Resize += (_, _) =>
                _titleLabel.Location = new Point(
                    _header.Width - _titleLabel.PreferredWidth - 20, 18);

            // ── Flow panel
            _flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(16),
                BackColor = BgColor
            };

            // ── Placeholder shown until LoadData() is called
            _placeholder = new Label
            {
                Text = "لم يتم تحميل البيانات بعد...",
                Font = SafeFont("Cairo", 11f),
                ForeColor = TxtMuted,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _flow.Controls.Add(_placeholder);

            this.Controls.Add(_flow);
            this.Controls.Add(_header);
        }

        // ── Card builder ──────────────────────────────────────────────────
        private void BuildCards()
        {
            if (_data is null) return;

            _flow.SuspendLayout();
            _flow.Controls.Clear();

            // Row 1 : KPI cards
            AddSection("المؤشرات الرئيسية");
            _flow.Controls.Add(MakeKpi("إجمالي العملاء",
                _data.TotalCustomers.ToString("N0"), "👥", AccentColor));
            _flow.Controls.Add(MakeKpi("إجمالي الطلبات",
                _data.TotalOrders.ToString("N0"), "🛒",
                Color.FromArgb(16, 185, 129)));
            _flow.Controls.Add(MakeKpi("الإيرادات",
                $"{_data.TotalRevenue:N2} ر.س", "💰",
                Color.FromArgb(245, 158, 11)));
            _flow.Controls.Add(MakeKpi("متوسط الطلب",
                $"{_data.AvgOrderValue:N2} ر.س", "📊",
                Color.FromArgb(139, 92, 246)));
            _flow.Controls.Add(MakeKpi("رصيد المحفظة",
                $"{_data.TotalWalletBalance:N2} ر.س", "👜",
                Color.FromArgb(236, 72, 153)));
            _flow.Controls.Add(MakeKpi("سلات متروكة",
                _data.AbandonedCarts.ToString("N0"), "🗑️",
                Color.FromArgb(239, 68, 68)));
            AddBreak();

            // Row 2 : Gender pie
            AddSection("توزيع الجنس");
            _flow.Controls.Add(new GenderPieCard(_data.GenderCounts));
            AddBreak();

            // Row 3 : Top Cities
            AddSection("أفضل 5 مدن");
            _flow.Controls.Add(new HBarCard(_data.TopCities, 380, 220, AccentColor));
            AddBreak();

            // Row 4 : Top Countries
            AddSection("أفضل 5 دول");
            _flow.Controls.Add(new HBarCard(_data.TopCountries, 380, 220,
                Color.FromArgb(16, 185, 129)));
            AddBreak();

            // Row 5 : Loyalty
            AddSection("توزيع نقاط الولاء");
            var loyaltyList = new List<(string, int)>();
            foreach (var key in new[] { "0", "1-100", "101-500", "501-1000", "1000+" })
                loyaltyList.Add((key, _data.LoyaltyBuckets.GetValueOrDefault(key, 0)));
            _flow.Controls.Add(new VBarCard(loyaltyList, 500, 200,
                Color.FromArgb(139, 92, 246)));
            AddBreak();

            // Row 6 : Recency
            AddSection("حداثة الشراء");
            var recencyList = new List<(string, int)>();
            foreach (var key in new[] { "Last 30 days", "31-90 days", "91-180 days", "180+ days", "Never" })
                recencyList.Add((MapRecency(key), _data.RecencyBuckets.GetValueOrDefault(key, 0)));
            _flow.Controls.Add(new HBarCard(recencyList, 500, 200, AccentColor));

            _flow.ResumeLayout();
        }

        // ── Section helpers ───────────────────────────────────────────────
        private void AddSection(string title)
        {
            var lbl = new Label
            {
                Text = title,
                Font = SafeFont("Cairo", 11f, FontStyle.Bold),
                ForeColor = TxtDark,
                AutoSize = false,
                Size = new Size(_flow.Width - 40, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(0, 8, 0, 4)
            };
            _flow.Controls.Add(lbl);
            _flow.SetFlowBreak(lbl, true);
        }

        private void AddBreak()
        {
            var sep = new Panel
            {
                Size = new Size(_flow.Width - 40, 1),
                BackColor = Color.FromArgb(220, 220, 240),
                Margin = new Padding(0, 4, 0, 4)
            };
            _flow.Controls.Add(sep);
            _flow.SetFlowBreak(sep, true);
        }

        private static KpiCard MakeKpi(string label, string value,
                                       string icon, Color accent)
            => new(label, value, icon, accent);

        private static string MapRecency(string en) => en switch
        {
            "Last 30 days" => "آخر 30 يوم",
            "31-90 days" => "31-90 يوم",
            "91-180 days" => "91-180 يوم",
            "180+ days" => "180+ يوم",
            "Never" => "لم يشتر",
            _ => en
        };

        // ── Shared utilities ──────────────────────────────────────────────
        internal static Font SafeFont(string family,
            float size, FontStyle style = FontStyle.Regular)
        {
            try { return new Font(family, size, style); }
            catch { return new Font("Segoe UI", size, style); }
        }

        internal static Region MakeRoundRegion(int w, int h, int r)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, r * 2, r * 2, 180, 90);
            path.AddArc(w - r * 2, 0, r * 2, r * 2, 270, 90);
            path.AddArc(w - r * 2, h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(0, h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }

        // ══════════════════════════════════════════════════════════════════
        //  BASE CARD
        // ══════════════════════════════════════════════════════════════════
        private abstract class InsightCard : Control
        {
            protected const int Radius = 16;

            protected InsightCard(int w, int h)
            {
                Size = new Size(w, h);
                Margin = new Padding(8);
                DoubleBuffered = true;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.ResizeRedraw, true);
            }

            protected sealed override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                PaintShadow(g);

                var card = new Rectangle(4, 4, Width - 8, Height - 8);
                using var path = RoundPath(card, Radius);
                using var bg = new SolidBrush(CardBg);
                g.FillPath(bg, path);

                PaintContent(g, card);
            }

            protected abstract void PaintContent(Graphics g, Rectangle card);

            private void PaintShadow(Graphics g)
            {
                for (int i = 7; i >= 0; i--)
                {
                    using var b = new SolidBrush(Color.FromArgb(4 * (8 - i), 100, 80, 200));
                    var r = new Rectangle(i, i + 2, Width - i * 2, Height - i * 2);
                    using var p = RoundPath(r, Radius + 2);
                    g.FillPath(b, p);
                }
            }

            protected static GraphicsPath RoundPath(Rectangle r, int rad)
            {
                var p = new GraphicsPath();
                p.AddArc(r.X, r.Y, rad * 2, rad * 2, 180, 90);
                p.AddArc(r.Right - rad * 2, r.Y, rad * 2, rad * 2, 270, 90);
                p.AddArc(r.Right - rad * 2, r.Bottom - rad * 2, rad * 2, rad * 2, 0, 90);
                p.AddArc(r.X, r.Bottom - rad * 2, rad * 2, rad * 2, 90, 90);
                p.CloseFigure();
                return p;
            }

            protected static void DrawRight(Graphics g, string text,
                Font font, Color color, RectangleF rect)
            {
                using var b = new SolidBrush(color);
                using var f = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(text, font, b, rect, f);
            }

            protected static void DrawCenter(Graphics g, string text,
                Font font, Color color, RectangleF rect)
            {
                using var b = new SolidBrush(color);
                using var f = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, b, rect, f);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  KPI CARD
        // ══════════════════════════════════════════════════════════════════
        private sealed class KpiCard : InsightCard
        {
            private readonly string _label, _value, _icon;
            private readonly Color _accent;

            public KpiCard(string label, string value,
                           string icon, Color accent) : base(200, 120)
            {
                _label = label;
                _value = value;
                _icon = icon;
                _accent = accent;
            }

            protected override void PaintContent(Graphics g, Rectangle c)
            {
                using var strip = new SolidBrush(Color.FromArgb(30, _accent));
                g.FillRectangle(strip, c.Right - 6, c.Y + Radius, 2, c.Height - Radius * 2);

                var circle = new RectangleF(c.Right - 54, c.Y + 14, 36, 36);
                using var circleBg = new SolidBrush(Color.FromArgb(20, _accent));
                g.FillEllipse(circleBg, circle);
                DrawCenter(g, _icon, SafeFont("Segoe UI Emoji", 14f), _accent, circle);

                DrawRight(g, _value,
                    SafeFont("Cairo", 15f, FontStyle.Bold), TxtDark,
                    new RectangleF(c.X + 8, c.Y + 16, c.Width - 68, 38));

                DrawRight(g, _label,
                    SafeFont("Cairo", 9f), TxtMuted,
                    new RectangleF(c.X + 8, c.Y + 58, c.Width - 16, 26));

                using var line = new SolidBrush(Color.FromArgb(180, _accent));
                g.FillRectangle(line,
                    c.X + Radius, c.Bottom - 5, c.Width - Radius * 2, 3);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  GENDER PIE CARD
        // ══════════════════════════════════════════════════════════════════
        private sealed class GenderPieCard : InsightCard
        {
            private readonly Dictionary<string, int> _counts;

            private static readonly (string Key, Color Color, string ArabicLabel)[] _meta =
            [
                ("M", ColorTranslator.FromHtml("#3B82F6"), "ذكر"),
                ("F", ColorTranslator.FromHtml("#EC4899"), "أنثى"),
                ("N", ColorTranslator.FromHtml("#94A3B8"), "غير محدد")
            ];

            public GenderPieCard(Dictionary<string, int> counts) : base(300, 200)
                => _counts = counts;

            protected override void PaintContent(Graphics g, Rectangle c)
            {
                DrawRight(g, "توزيع الجنس",
                    SafeFont("Cairo", 11f, FontStyle.Bold), TxtDark,
                    new RectangleF(c.X, c.Y + 8, c.Width - 16, 26));

                int total = 0;
                foreach (var v in _counts.Values) total += v;
                if (total == 0) return;

                var pie = new RectangleF(c.X + 12, c.Y + 40, 110, 110);
                float ang = -90f;

                foreach (var (key, color, _) in _meta)
                {
                    if (!_counts.TryGetValue(key, out int cnt) || cnt == 0) continue;
                    float sweep = 360f * cnt / total;
                    using var b = new SolidBrush(color);
                    g.FillPie(b, pie.X, pie.Y, pie.Width, pie.Height, ang, sweep);
                    using var pen = new Pen(Color.White, 2f);
                    g.DrawPie(pen, pie.X, pie.Y, pie.Width, pie.Height, ang, sweep);
                    ang += sweep;
                }

                float ly = c.Y + 42, lx = c.Right - 140;
                foreach (var (key, color, lbl) in _meta)
                {
                    int cnt = _counts.GetValueOrDefault(key, 0);
                    float pct = total > 0 ? 100f * cnt / total : 0f;

                    using var dot = new SolidBrush(color);
                    g.FillEllipse(dot, lx, ly + 5, 10, 10);

                    using var fb = new SolidBrush(TxtDark);
                    using var fmt = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Near
                    };
                    g.DrawString($"{lbl}  {pct:N1}%  ({cnt:N0})",
                        SafeFont("Cairo", 8.5f), fb,
                        new RectangleF(lx - 100, ly, 120, 20), fmt);
                    ly += 28;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  HORIZONTAL BAR CARD  (Cities / Countries / Recency)
        // ══════════════════════════════════════════════════════════════════
        private sealed class HBarCard : InsightCard
        {
            private readonly List<(string Name, int Count)> _items;
            private readonly Color _barColor;

            public HBarCard(List<(string Name, int Count)> items,
                            int w, int h, Color barColor) : base(w, h)
            {
                _items = items;
                _barColor = barColor;
            }

            protected override void PaintContent(Graphics g, Rectangle c)
            {
                if (_items.Count == 0) return;

                int max = 1;
                foreach (var it in _items) if (it.Count > max) max = it.Count;

                int rowH = (c.Height - 16) / Math.Max(_items.Count, 1);
                int barArea = c.Width - 130;

                for (int i = 0; i < _items.Count; i++)
                {
                    var (name, cnt) = _items[i];
                    float y = c.Y + 8 + i * rowH;
                    float bx = c.X + 46;
                    float bw = barArea - 8;
                    float bh = Math.Max(12f, rowH - 14f);
                    float by = y + (rowH - bh) / 2f;

                    // Name
                    using var lb = new SolidBrush(TxtDark);
                    using var lfmt = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap
                    };
                    g.DrawString(name, SafeFont("Cairo", 8.5f), lb,
                        new RectangleF(c.Right - 100, y + 2, 90, rowH - 8), lfmt);

                    // Track
                    using var track = new SolidBrush(Color.FromArgb(20, _barColor));
                    g.FillRectangle(track, bx, by, bw, bh);

                    // Fill
                    float fw = max > 0 ? bw * cnt / max : 0;
                    if (fw > 0)
                    {
                        using var fill = new LinearGradientBrush(
                            new RectangleF(bx, by, Math.Max(fw, 1f), bh),
                            Color.FromArgb(160, _barColor), _barColor,
                            LinearGradientMode.Horizontal);
                        g.FillRectangle(fill, bx, by, fw, bh);
                    }

                    // Count text
                    using var cb = new SolidBrush(TxtDark);
                    using var cfmt = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(cnt.ToString("N0"),
                        SafeFont("Cairo", 8f), cb,
                        new RectangleF(c.X + 8, by, 38, bh), cfmt);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  VERTICAL BAR CARD  (Loyalty buckets)
        // ══════════════════════════════════════════════════════════════════
        private sealed class VBarCard : InsightCard
        {
            private readonly List<(string Label, int Count)> _items;
            private readonly Color _barColor;

            public VBarCard(List<(string Label, int Count)> items,
                            int w, int h, Color barColor) : base(w, h)
            {
                _items = items;
                _barColor = barColor;
            }

            protected override void PaintContent(Graphics g, Rectangle c)
            {
                if (_items.Count == 0) return;

                int max = 1;
                foreach (var it in _items) if (it.Count > max) max = it.Count;

                int n = _items.Count;
                float colW = (c.Width - 24f) / n;
                float barMaxH = c.Height - 70;
                float baseline = c.Bottom - 30;

                for (int i = 0; i < n; i++)
                {
                    var (label, cnt) = _items[i];
                    float x = c.X + 12 + i * colW;
                    float bw = colW * 0.55f;
                    float bx = x + (colW - bw) / 2f;
                    float bh = max > 0 ? barMaxH * cnt / max : 0;
                    float by = baseline - bh;

                    using var track = new SolidBrush(Color.FromArgb(18, _barColor));
                    g.FillRectangle(track, bx, c.Y + 14, bw, barMaxH);

                    if (bh > 1)
                    {
                        using var fill = new LinearGradientBrush(
                            new RectangleF(bx, by, bw, bh),
                            Color.FromArgb(160, _barColor), _barColor,
                            LinearGradientMode.Vertical);
                        using var barPath = new GraphicsPath();
                        int r = 6;
                        barPath.AddArc(bx, by, r * 2, r * 2, 180, 90);
                        barPath.AddArc(bx + bw - r * 2, by, r * 2, r * 2, 270, 90);
                        barPath.AddLine(bx + bw, baseline, bx, baseline);
                        barPath.CloseFigure();
                        g.FillPath(fill, barPath);
                    }

                    DrawCenter(g, cnt.ToString("N0"),
                        SafeFont("Cairo", 8f, FontStyle.Bold), _barColor,
                        new RectangleF(bx, by - 18, bw, 16));

                    DrawCenter(g, label,
                        SafeFont("Cairo", 7.5f), TxtMuted,
                        new RectangleF(bx - 4, baseline + 4, bw + 8, 22));
                }
            }
        }
    }
}