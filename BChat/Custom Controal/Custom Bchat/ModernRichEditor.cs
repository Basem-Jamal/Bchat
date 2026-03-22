// Controls/ModernRichEditor.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BChat.Controls
{
    public class ModernRichEditor : Panel
    {

        public string PlainText => _rtb.Text;

        // ─── Colors ───────────────────────────────────────
        private readonly Color _bgColor = Color.FromArgb(237, 235, 255);
        private readonly Color _borderColor = Color.FromArgb(220, 215, 250);
        private readonly Color _focusBorder = Color.FromArgb(124, 111, 247);
        private readonly Color _toolbarBg = Color.FromArgb(245, 244, 255);
        private readonly Color _toolbarBorder = Color.FromArgb(225, 220, 250);
        private readonly Color _labelColor = Color.FromArgb(60, 60, 90);
        private readonly Color _accentColor = Color.FromArgb(124, 111, 247);
        private readonly Color _btnHover = Color.FromArgb(30, 124, 111, 247);
        private readonly Color _btnActive = Color.FromArgb(60, 124, 111, 247);

        private readonly int _borderRadius = 14;
        private readonly int _labelHeight = 24;
        private readonly int _toolbarH = 40;

        // ─── Inner Controls ───────────────────────────────
        private RichTextBox _rtb = null!;
        private ToolbarPanel _toolbar = null!;
        private bool _focused = false;

        // ─── Images Store ─────────────────────────────────
        private readonly Dictionary<string, string> _images = new();
        private int _imgCounter = 0;

        // ─── Properties ───────────────────────────────────
        private string _labelText = "محتوى الرسالة";

        public string LabelText
        {
            get => _labelText;
            set { _labelText = value; Invalidate(); }
        }

        public List<string> Variables { get; set; } = new()
        {
            "{الاسم}", "{رقم_الطلب}", "{المبلغ}", "{اسم_المنتج}", "{التاريخ}"
        };

        // ─── Constructor ──────────────────────────────────
        public ModernRichEditor()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(460, 220);
            BackColor = Color.Transparent;

            BuildToolbar();
            BuildEditor();

            Resize += (s, e) => LayoutChildren();
            LayoutChildren();
        }

        // ─── Build ────────────────────────────────────────
        private void BuildToolbar()
        {
            _toolbar = new ToolbarPanel
            {
                BgColor = _toolbarBg,
                BorderColor = _toolbarBorder,
                AccentColor = _accentColor,
                BtnHover = _btnHover,
                BtnActive = _btnActive,
                BorderRadius = 10,
            };

            _toolbar.BoldClicked += OnBoldClicked;
            _toolbar.ItalicClicked += OnItalicClicked;
            _toolbar.ImageClicked += OnImageClicked;
            _toolbar.LinkClicked += OnLinkClicked;
            _toolbar.EmojiClicked += OnEmojiClicked;
            _toolbar.VariableClicked += OnVariableClicked;

            Controls.Add(_toolbar);
        }

        private void BuildEditor()
        {
            _rtb = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = _bgColor,
                ForeColor = Color.FromArgb(40, 40, 70),
                Font = new Font("Cairo", 10.5f),
                RightToLeft = RightToLeft.Yes,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Multiline = true,
            };

            _rtb.GotFocus += (s, e) => { _focused = true; Invalidate(); };
            _rtb.LostFocus += (s, e) => { _focused = false; Invalidate(); };

            Controls.Add(_rtb);
        }

        private void LayoutChildren()
        {
            int boxTop = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            int innerPad = 8;

            _toolbar.SetBounds(
                innerPad,
                boxTop + innerPad,
                Width - innerPad * 2,
                _toolbarH);

            int rtbTop = boxTop + innerPad + _toolbarH + 4;
            int rtbH = Height - rtbTop - innerPad;
            _rtb.SetBounds(
                innerPad + 6,
                rtbTop,
                Width - (innerPad + 6) * 2,
                rtbH);
        }

        // ─── Toolbar Actions ──────────────────────────────
        private void OnBoldClicked()
        {
            _rtb.Focus();
            if (_rtb.SelectionFont == null) return;
            var s = _rtb.SelectionFont.Style;
            _rtb.SelectionFont = new Font(_rtb.SelectionFont,
                s.HasFlag(FontStyle.Bold) ? s & ~FontStyle.Bold : s | FontStyle.Bold);
        }

        private void OnItalicClicked()
        {
            _rtb.Focus();
            if (_rtb.SelectionFont == null) return;
            var s = _rtb.SelectionFont.Style;
            _rtb.SelectionFont = new Font(_rtb.SelectionFont,
                s.HasFlag(FontStyle.Italic) ? s & ~FontStyle.Italic : s | FontStyle.Italic);
        }

        private void OnImageClicked()
        {
            using var dlg = new OpenFileDialog
            {
                Title = "اختر صورة",
                Filter = "صور|*.jpg;*.jpeg;*.png;*.gif;*.webp"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                byte[] bytes = File.ReadAllBytes(dlg.FileName);
                string base64 = Convert.ToBase64String(bytes);
                string ext = Path.GetExtension(dlg.FileName).TrimStart('.').ToLower();
                string dataUri = $"data:image/{ext};base64,{base64}";

                string key = $"[IMG_{_imgCounter++}]";
                _images[key] = dataUri;

                InsertImagePlaceholder(key, dlg.FileName);
            }
            catch
            {
                MessageBox.Show("تعذّر تحميل الصورة.", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InsertImagePlaceholder(string key, string filePath)
        {
            try
            {
                using var img = Image.FromFile(filePath);
                var thumb = img.GetThumbnailImage(80, 60, null, IntPtr.Zero);
                var old = Clipboard.GetDataObject();

                Clipboard.SetImage(thumb);
                _rtb.Focus();
                _rtb.Paste();

                if (old != null) Clipboard.SetDataObject(old);
                _rtb.AppendText($" {key} ");
            }
            catch
            {
                _rtb.AppendText($" [صورة:{key}] ");
            }
        }

        private void OnLinkClicked()
        {
            using var dlg = new LinkInputForm();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            string url = dlg.Url;
            string text = string.IsNullOrWhiteSpace(dlg.LinkText) ? url : dlg.LinkText;

            int start = _rtb.SelectionStart;
            _rtb.SelectedText = text;
            _rtb.Select(start, text.Length);
            _rtb.SelectionColor = _accentColor;
            _rtb.SelectionFont = new Font(_rtb.Font, FontStyle.Underline);
            _rtb.SelectionStart = _rtb.SelectionStart + _rtb.SelectionLength;
            _rtb.SelectionColor = Color.FromArgb(40, 40, 70);
            _rtb.SelectionFont = _rtb.Font;
            _rtb.Focus();
        }

        private void OnEmojiClicked()
        {
            var picker = new EmojiPickerPopup();
            picker.EmojiSelected += emoji =>
            {
                _rtb.Focus();
                _rtb.SelectedText = emoji;
            };
            var pt = _toolbar.PointToScreen(new Point(_toolbar.Width - 220, _toolbarH));
            picker.Show(pt);
        }

        private void OnVariableClicked()
        {
            var popup = new VariablePickerPopup(Variables);
            popup.VariableSelected += variable =>
            {
                _rtb.Focus();
                int pos = _rtb.SelectionStart;
                _rtb.SelectedText = variable;
                _rtb.Select(pos, variable.Length);
                _rtb.SelectionColor = _accentColor;
                _rtb.SelectionFont = new Font(_rtb.Font, FontStyle.Bold);
                _rtb.SelectionStart = pos + variable.Length;
                _rtb.SelectionColor = Color.FromArgb(40, 40, 70);
                _rtb.SelectionFont = _rtb.Font;
            };
            var pt = _toolbar.PointToScreen(new Point(_toolbar.Width - 100, _toolbarH));
            popup.Show(pt);
        }

        // ─── Public API ───────────────────────────────────
        public string GetHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<div dir='rtl' style='font-family:Cairo;font-size:10.5pt;'>");
            string html = _rtb.Text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\n", "<br/>");

            foreach (var kv in _images)
                if (kv.Key.StartsWith("[IMG_"))
                    html = html.Replace(kv.Key,
                        $"<img src='{kv.Value}' style='max-width:200px;border-radius:8px;'/>");

            sb.Append(html);
            sb.Append("</div>");
            return sb.ToString();
        }

        public void SetContent(string text)
        {
            _rtb.Clear();
            _rtb.AppendText(text);
        }

        public void Clear()
        {
            _rtb.Clear();
            _images.Clear();
            _imgCounter = 0;
        }

        // ─── Paint ────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Color.WhiteSmoke);

            if (!string.IsNullOrEmpty(_labelText))
            {
                using var lFont = new Font("Cairo", 9.5f);
                using var lBrush = new SolidBrush(_labelColor);
                var lFmt = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_labelText, lFont, lBrush,
                    new RectangleF(0, 0, Width, _labelHeight), lFmt);
            }

            int boxTop = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            var boxRect = new Rectangle(0, boxTop, Width - 1, Height - boxTop - 1);

            using var path = RoundedRect(boxRect, _borderRadius);
            using var bgBrush = new SolidBrush(_bgColor);
            g.FillPath(bgBrush, path);

            float bWidth = _focused ? 1.8f : 1f;
            using var pen = new Pen(_focused ? _focusBorder : _borderColor, bWidth);
            g.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ══════════════════════════════════════════════════════
    //  ToolbarPanel
    // ══════════════════════════════════════════════════════
    internal class ToolbarPanel : Panel
    {
        public Color BgColor { get; set; } = Color.FromArgb(245, 244, 255);
        public Color BorderColor { get; set; } = Color.FromArgb(225, 220, 250);
        public Color AccentColor { get; set; } = Color.FromArgb(124, 111, 247);
        public Color BtnHover { get; set; }
        public Color BtnActive { get; set; }
        public int BorderRadius { get; set; } = 10;

        public event Action? BoldClicked;
        public event Action? ItalicClicked;
        public event Action? ImageClicked;
        public event Action? LinkClicked;
        public event Action? EmojiClicked;
        public event Action? VariableClicked;

        private readonly List<ToolbarButton> _buttons = new();

        public ToolbarPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            Height = 40;
            BackColor = Color.Transparent;
        }

        // يُستدعى بعد ضبط الـ Properties
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            BuildButtons();
        }

        private void BuildButtons()
        {
            Controls.Clear();
            _buttons.Clear();

            _buttons.Add(new ToolbarButton("B", "Bold", () => BoldClicked?.Invoke(), BtnHover, BtnActive, isBold: true));
            _buttons.Add(new ToolbarButton("I", "Italic", () => ItalicClicked?.Invoke(), BtnHover, BtnActive, isItalic: true));
            _buttons.Add(new ToolbarButton("رابط", "إدراج رابط", () => LinkClicked?.Invoke(), BtnHover, BtnActive));
            _buttons.Add(new ToolbarButton("صورة", "إدراج صورة", () => ImageClicked?.Invoke(), BtnHover, BtnActive));
            _buttons.Add(new ToolbarButton("ايموجي", "إدراج Emoji", () => EmojiClicked?.Invoke(), BtnHover, BtnActive));
            _buttons.Add(new ToolbarButton("+ متغير", "إضافة متغير", () => VariableClicked?.Invoke(), BtnHover, BtnActive, isWide: true));

            foreach (var btn in _buttons)
                Controls.Add(btn);

            LayoutButtons();
        }

        private void LayoutButtons()
        {
            int pad = 6;
            int btnH = Height - pad * 2;
            int x = pad;

            foreach (var btn in _buttons)
            {
                if (btn.IsWide)
                {
                    btn.SetBounds(Width - 90 - pad, pad, 90, btnH);
                }
                else
                {
                    int w = btn.Text.Length <= 2 ? btnH : btnH + 24;
                    btn.SetBounds(x, pad, w, btnH);
                    x += w + 4;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutButtons();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedRect(rect, BorderRadius);
            using var bgBrush = new SolidBrush(BgColor);
            g.FillPath(bgBrush, path);

            using var pen = new Pen(BorderColor, 1f);
            g.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ══════════════════════════════════════════════════════
    //  ToolbarButton — Control مش Panel
    // ══════════════════════════════════════════════════════
    internal class ToolbarButton : Control
    {
        private bool _hovered = false;
        private bool _pressed = false;
        private Action _onClick;
        private bool _isBold;
        private bool _isItalic;

        public Color HoverColor { get; set; }
        public Color ActiveColor { get; set; }
        public bool IsWide { get; }

        public ToolbarButton(string text, string tooltip, Action onClick,
                             Color hoverColor, Color activeColor,
                             bool isBold = false,
                             bool isItalic = false,
                             bool isWide = false)
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            Text = text;
            _onClick = onClick;
            _isBold = isBold;
            _isItalic = isItalic;
            IsWide = isWide;
            HoverColor = hoverColor;
            ActiveColor = activeColor;
            Cursor = Cursors.Hand;
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(80, 70, 120);

            new ToolTip().SetToolTip(this, tooltip);

            MouseEnter += (s, e) => { _hovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _hovered = false; Invalidate(); };
            MouseDown += (s, e) => { _pressed = true; Invalidate(); };
            MouseUp += (s, e) =>
            {
                _pressed = false;
                if (_hovered) _onClick?.Invoke();
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // ارسم خلفية الـ Parent يدوياً
            if (Parent != null)
            {
                using var parentBrush = new SolidBrush(Parent.BackColor);
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            var rect = new Rectangle(1, 1, Width - 2, Height - 2);

            if (_pressed || _hovered)
            {
                using var path = RoundedRect(rect, 8);
                using var fill = new SolidBrush(_pressed ? ActiveColor : HoverColor);
                g.FillPath(fill, path);
            }

            var style = FontStyle.Regular;
            if (_isBold) style |= FontStyle.Bold;
            if (_isItalic) style |= FontStyle.Italic;

            using var font = new Font("Cairo", IsWide ? 8.5f : 9.5f, style);
            using var brush = new SolidBrush(ForeColor);
            var fmt = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(Text, font, brush,
                new RectangleF(0, 0, Width, Height), fmt);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ══════════════════════════════════════════════════════
    //  EmojiPickerPopup
    // ══════════════════════════════════════════════════════
    internal class EmojiPickerPopup : Form
    {
        public event Action<string>? EmojiSelected;

        private static readonly string[] Emojis =
        {
            "😊","😍","🎉","👍","✅","❤️","🔥","💯",
            "🙏","😂","🤩","💪","⭐","🛍️","📦","💬",
            "📱","🎁","🌟","✨","👋","🙌","💥","🚀"
        };

        public EmojiPickerPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            BackColor = Color.White;
            Size = new Size(220, 120);

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(4) };

            foreach (var emoji in Emojis)
            {
                var e = emoji;
                var btn = new Button
                {
                    Text = e,
                    Size = new Size(36, 36),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Emoji", 14f),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, _) => { EmojiSelected?.Invoke(e); Close(); };
                panel.Controls.Add(btn);
            }

            Controls.Add(panel);
            Deactivate += (s, e) => Close();
        }

        public void Show(Point location)
        {
            Location = location;
            Show();
        }
    }

    // ══════════════════════════════════════════════════════
    //  VariablePickerPopup
    // ══════════════════════════════════════════════════════
    internal class VariablePickerPopup : Form
    {
        public event Action<string>? VariableSelected;

        public VariablePickerPopup(List<string> variables)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            BackColor = Color.White;
            Size = new Size(180, variables.Count * 36 + 10);

            var accent = Color.FromArgb(124, 111, 247);
            int y = 5;

            foreach (var variable in variables)
            {
                var v = variable;
                var btn = new Button
                {
                    Text = v,
                    Bounds = new Rectangle(5, y, 170, 30),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Cairo", 9.5f),
                    ForeColor = accent,
                    TextAlign = ContentAlignment.MiddleRight,
                    RightToLeft = RightToLeft.Yes,
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 215, 250);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(237, 235, 255);
                btn.Click += (s, e) => { VariableSelected?.Invoke(v); Close(); };
                Controls.Add(btn);
                y += 34;
            }

            Deactivate += (s, e) => Close();
        }

        public void Show(Point location)
        {
            Location = location;
            Show();
        }
    }

    // ══════════════════════════════════════════════════════
    //  LinkInputForm
    // ══════════════════════════════════════════════════════
    internal class LinkInputForm : Form
    {
        public string Url { get; private set; } = "";
        public string LinkText { get; private set; } = "";

        public LinkInputForm()
        {
            Text = "إدراج رابط";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(340, 180);
            MaximizeBox = false;
            MinimizeBox = false;
            RightToLeft = RightToLeft.Yes;
            Font = new Font("Cairo", 9.5f);

            var lblUrl = new Label { Text = "الرابط:", Bounds = new Rectangle(10, 15, 60, 22) };
            var txtUrl = new TextBox { Bounds = new Rectangle(80, 12, 230, 24), RightToLeft = RightToLeft.No };
            var lblText = new Label { Text = "النص:", Bounds = new Rectangle(10, 50, 60, 22) };
            var txtText = new TextBox { Bounds = new Rectangle(80, 47, 230, 24) };

            var btnOk = new Button
            {
                Text = "إدراج",
                DialogResult = DialogResult.OK,
                Bounds = new Rectangle(200, 110, 110, 32),
            };
            btnOk.Click += (s, e) => { Url = txtUrl.Text.Trim(); LinkText = txtText.Text.Trim(); };

            var btnCancel = new Button
            {
                Text = "إلغاء",
                DialogResult = DialogResult.Cancel,
                Bounds = new Rectangle(80, 110, 110, 32),
            };

            Controls.AddRange(new Control[] { lblUrl, txtUrl, lblText, txtText, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}