// Controls/ModernPictureBox.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BChat.Controls
{
    // ─── Enums ────────────────────────────────────────────────────────────────
    public enum ImageFit { Fill, Contain, Cover, Center, Stretch }
    public enum OverlayStyle { None, DarkGradient, LightGradient, Solid }

    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernPictureBox : Control
    {
        // ─── Fields ───────────────────────────────────────────────────────────
        private Image? _image;
        private Image? _scaledCache;          // cached scaled image to avoid re-scaling every paint
        private Size   _lastScaledSize;       // track when cache is stale

        private int    _borderRadius    = 16;
        private int    _borderThickness = 0;
        private Color  _borderColor     = Color.FromArgb(80, 255, 255, 255);
        private Color  _shadowColor     = Color.FromArgb(60, 0, 0, 0);
        private bool   _showShadow      = false;
        private int    _shadowDepth     = 6;
        private bool   _showOverlay     = false;
        private OverlayStyle _overlayStyle = OverlayStyle.DarkGradient;
        private Color  _overlayColor    = Color.FromArgb(80, 0, 0, 0);
        private string _placeholderText = "";
        private ImageFit _imageFit      = ImageFit.Cover;

        // Hover / Press state
        private bool _isHovered  = false;
        private bool _isPressed  = false;
        private bool _hoverZoom  = false;       // subtle scale on hover
        private float _zoomScale = 1.0f;

        // ─── Properties ───────────────────────────────────────────────────────

        [Category("BChat")]
        [Description("The image to display.")]
        public Image? Image
        {
            get => _image;
            set
            {
                // Dispose old scaled cache when image changes
                DisposeScaledCache();
                _image = value;
                Invalidate();
            }
        }

        [Category("BChat")]
        [DefaultValue(16)]
        [Description("Corner radius in pixels. Use 999 for pill/circle shape.")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(0)]
        [Description("Border stroke thickness. 0 = no border.")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("BChat")]
        [Description("Color of the border stroke.")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        [Description("Draw a soft drop-shadow beneath the control.")]
        public bool ShowShadow
        {
            get => _showShadow;
            set { _showShadow = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(6)]
        [Description("Shadow blur depth in pixels.")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = Math.Max(1, Math.Min(20, value)); Invalidate(); }
        }

        [Category("BChat")]
        [Description("Shadow color.")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        [Description("Draw a gradient/solid overlay on top of the image.")]
        public bool ShowOverlay
        {
            get => _showOverlay;
            set { _showOverlay = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(OverlayStyle.DarkGradient)]
        public OverlayStyle OverlayStyle
        {
            get => _overlayStyle;
            set { _overlayStyle = value; Invalidate(); }
        }

        [Category("BChat")]
        [Description("Base color used for the solid overlay or the opaque end of the gradient.")]
        public Color OverlayColor
        {
            get => _overlayColor;
            set { _overlayColor = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(ImageFit.Cover)]
        [Description("How the image is fitted inside the control.")]
        public ImageFit ImageFit
        {
            get => _imageFit;
            set { _imageFit = value; DisposeScaledCache(); Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue("")]
        [Description("Text shown when no image is set (placeholder).")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set { _placeholderText = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(false)]
        [Description("Slightly zooms the image on mouse-hover for an interactive feel.")]
        public bool HoverZoom
        {
            get => _hoverZoom;
            set { _hoverZoom = value; Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────────────────────
        public ModernPictureBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.UserPaint             |
                ControlStyles.ResizeRedraw          |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            DoubleBuffered = true;
            Size           = new Size(200, 200);
            BackColor      = Color.FromArgb(230, 230, 235);
            Cursor         = Cursors.Hand;
        }

        // ─── Mouse Events ─────────────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            if (_hoverZoom) { _zoomScale = 1.06f; Invalidate(); }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; _isPressed = false;
            if (_hoverZoom) { _zoomScale = 1.0f; Invalidate(); }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            if (_hoverZoom) { _zoomScale = 0.97f; Invalidate(); }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            if (_hoverZoom) { _zoomScale = _isHovered ? 1.06f : 1.0f; Invalidate(); }
            base.OnMouseUp(e);
        }

        // Invalidate cache when resized
        protected override void OnResize(EventArgs e)
        {
            DisposeScaledCache();
            base.OnResize(e);
        }

        // ─── Paint ────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode        = SmoothingMode.AntiAlias;
            g.InterpolationMode    = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode      = PixelOffsetMode.HighQuality;
            g.TextRenderingHint    = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // ── Transparent parent background ─────────────────────────────────
            if (Parent != null)
            {
                g.TranslateTransform(-Left, -Top);
                InvokePaintBackground(Parent, new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)));
                InvokePaint(Parent, new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)));
                g.TranslateTransform(Left, Top);
            }
            else
            {
                g.Clear(BackColor);
            }

            // ── Shadow ────────────────────────────────────────────────────────
            if (_showShadow)
                DrawShadow(g);

            // ── Clip region (rounded rect) ────────────────────────────────────
            int r = Math.Min(_borderRadius, Math.Min(Width, Height) / 2);
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var clipPath = RoundedRect(rect, r);

            g.SetClip(clipPath);

            // ── Background fill ───────────────────────────────────────────────
            using (var bgBrush = new SolidBrush(BackColor))
                g.FillPath(bgBrush, clipPath);

            // ── Image ─────────────────────────────────────────────────────────
            if (_image != null)
            {
                DrawImage(g, rect);
            }
            else
            {
                DrawPlaceholder(g, rect);
            }

            // ── Overlay ───────────────────────────────────────────────────────
            if (_showOverlay)
                DrawOverlay(g, clipPath, rect);

            // ── Release clip ──────────────────────────────────────────────────
            g.ResetClip();

            // ── Border stroke ─────────────────────────────────────────────────
            if (_borderThickness > 0)
            {
                using var pen = new Pen(_borderColor, _borderThickness);
                g.DrawPath(pen, clipPath);
            }
        }

        // ─── Image Drawing ────────────────────────────────────────────────────
        private void DrawImage(Graphics g, Rectangle rect)
        {
            // Apply zoom transform (hover effect)
            if (_zoomScale != 1.0f)
            {
                float cx = Width  / 2f;
                float cy = Height / 2f;
                g.TranslateTransform(cx, cy);
                g.ScaleTransform(_zoomScale, _zoomScale);
                g.TranslateTransform(-cx, -cy);
            }

            var destRect = GetDestinationRect(rect);
            g.DrawImage(GetScaledImage(), destRect);

            if (_zoomScale != 1.0f)
                g.ResetTransform();
        }

        // ─── Scaled Image Cache (prevents memory churn) ───────────────────────
        /// <summary>
        /// Returns a cached scaled bitmap. Re-creates only when the size or source changes.
        /// The caller must NOT dispose this — it is managed by the control.
        /// </summary>
        private Image GetScaledImage()
        {
            if (_image == null) throw new InvalidOperationException();

            // For Contain/Cover/Fill modes we pre-scale once for performance.
            // Center/Stretch draw straight to destRect so no pre-scaling needed.
            if (_imageFit == ImageFit.Center || _imageFit == ImageFit.Stretch)
                return _image;

            var targetSize = new Size(Width, Height);
            if (_scaledCache != null && _lastScaledSize == targetSize)
                return _scaledCache;

            DisposeScaledCache();

            // Build a scaled bitmap at control size for Cover/Contain/Fill
            _scaledCache    = new Bitmap(targetSize.Width, targetSize.Height, PixelFormat.Format32bppArgb);
            _lastScaledSize = targetSize;

            using var bg = Graphics.FromImage(_scaledCache);
            bg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bg.PixelOffsetMode   = PixelOffsetMode.HighQuality;
            bg.SmoothingMode     = SmoothingMode.AntiAlias;

            // For the cache bitmap we draw at the raw dest rect relative to (0,0)
            var rawDest = GetDestinationRectRaw(new Rectangle(0, 0, targetSize.Width - 1, targetSize.Height - 1));
            bg.DrawImage(_image, rawDest);

            return _scaledCache;
        }

        private void DisposeScaledCache()
        {
            _scaledCache?.Dispose();
            _scaledCache = null;
        }

        // ─── Destination Rect Helpers ─────────────────────────────────────────
        // Used during paint (may be offset by zoom transform already applied)
        private Rectangle GetDestinationRect(Rectangle rect)
            => GetDestinationRectRaw(rect);

        private Rectangle GetDestinationRectRaw(Rectangle rect)
        {
            if (_image == null) return rect;

            int imgW = _image.Width;
            int imgH = _image.Height;
            int ctlW = rect.Width;
            int ctlH = rect.Height;

            return _imageFit switch
            {
                ImageFit.Stretch => rect,
                ImageFit.Fill    => rect,

                ImageFit.Center =>
                    new Rectangle(
                        rect.X + (ctlW - imgW) / 2,
                        rect.Y + (ctlH - imgH) / 2,
                        imgW, imgH),

                ImageFit.Contain =>
                    ScaledRect(rect, imgW, imgH, false),

                ImageFit.Cover =>
                    ScaledRect(rect, imgW, imgH, true),

                _ => rect
            };
        }

        /// <summary>
        /// Scales the image rect to fill (cover=true) or fit (cover=false) the control.
        /// </summary>
        private static Rectangle ScaledRect(Rectangle ctrl, int imgW, int imgH, bool cover)
        {
            float scaleX = (float)ctrl.Width  / imgW;
            float scaleY = (float)ctrl.Height / imgH;
            float scale  = cover ? Math.Max(scaleX, scaleY) : Math.Min(scaleX, scaleY);

            int w = (int)(imgW * scale);
            int h = (int)(imgH * scale);
            int x = ctrl.X + (ctrl.Width  - w) / 2;
            int y = ctrl.Y + (ctrl.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        // ─── Placeholder ──────────────────────────────────────────────────────
        private void DrawPlaceholder(Graphics g, Rectangle rect)
        {
            // Simple camera icon (SVG-style, drawn with GDI+)
            int iconSize = Math.Min(rect.Width, rect.Height) / 4;
            iconSize = Math.Max(iconSize, 20);
            int cx = rect.Width  / 2;
            int cy = rect.Height / 2;

            using var iconPen = new Pen(Color.FromArgb(160, 160, 170), 2f);
            // lens circle
            g.DrawEllipse(iconPen,
                cx - iconSize / 2, cy - iconSize / 2,
                iconSize, iconSize);
            // body rect
            int bW = iconSize * 2, bH = (int)(iconSize * 1.3f);
            g.DrawRoundedRect(iconPen,
                cx - bW / 2, cy - bH / 2 + 4,
                bW, bH, 5);
            // viewfinder bump
            g.DrawRectangle(iconPen,
                cx - 6, cy - bH / 2 + 2,
                12, 6);

            // Optional text
            if (!string.IsNullOrEmpty(_placeholderText))
            {
                using var f    = new Font("Segoe UI", 9f);
                using var br   = new SolidBrush(Color.FromArgb(140, 140, 150));
                var sz  = g.MeasureString(_placeholderText, f);
                g.DrawString(_placeholderText, f, br,
                    (rect.Width  - sz.Width)  / 2f,
                    cy + iconSize / 2f + 10f);
            }
        }

        // ─── Overlay ──────────────────────────────────────────────────────────
        private void DrawOverlay(Graphics g, GraphicsPath clipPath, Rectangle rect)
        {
            if (_overlayStyle == OverlayStyle.None) return;

            switch (_overlayStyle)
            {
                case OverlayStyle.Solid:
                    using (var b = new SolidBrush(_overlayColor))
                        g.FillPath(b, clipPath);
                    break;

                case OverlayStyle.DarkGradient:
                    using (var lgb = new LinearGradientBrush(
                        rect,
                        Color.Transparent,
                        Color.FromArgb(_overlayColor.A, 0, 0, 0),
                        LinearGradientMode.Vertical))
                        g.FillPath(lgb, clipPath);
                    break;

                case OverlayStyle.LightGradient:
                    using (var lgb = new LinearGradientBrush(
                        rect,
                        Color.Transparent,
                        Color.FromArgb(_overlayColor.A, 255, 255, 255),
                        LinearGradientMode.Vertical))
                        g.FillPath(lgb, clipPath);
                    break;
            }
        }

        // ─── Shadow ───────────────────────────────────────────────────────────
        private void DrawShadow(Graphics g)
        {
            int d  = _shadowDepth;
            int r  = Math.Min(_borderRadius, Math.Min(Width, Height) / 2);

            for (int i = d; i >= 1; i--)
            {
                int alpha = (int)((_shadowColor.A / (float)d) * (d - i + 1) * 0.7f);
                if (alpha <= 0) continue;

                var shadowColor = Color.FromArgb(alpha,
                    _shadowColor.R, _shadowColor.G, _shadowColor.B);

                var shadowRect = new Rectangle(i, i, Width - 1 - i, Height - 1 - i);
                using var shadowPath = RoundedRect(shadowRect, Math.Max(0, r - 1));
                using var pen        = new Pen(shadowColor, 1.5f);
                g.DrawPath(pen, shadowPath);
            }
        }

        // ─── Rounded Rect Helper ──────────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d    = radius * 2;
            if (d <= 0)
            {
                path.AddRectangle(r);
                return path;
            }
            path.AddArc(r.X,           r.Y,            d, d, 180, 90);
            path.AddArc(r.Right - d,   r.Y,            d, d, 270, 90);
            path.AddArc(r.Right - d,   r.Bottom - d,   d, d,   0, 90);
            path.AddArc(r.X,           r.Bottom - d,   d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        // ─── Dispose ──────────────────────────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeScaledCache();
                // NOTE: we do NOT dispose _image here — the caller owns that image.
                // If you want the control to own the image, set _ownsImage = true
                // and call _image?.Dispose() here.
            }
            base.Dispose(disposing);
        }
    }

    // ─── Extension: DrawRoundedRect on Graphics ───────────────────────────────
    internal static class GraphicsExtensions
    {
        public static void DrawRoundedRect(
            this Graphics g, Pen pen,
            float x, float y, float w, float h, float r)
        {
            float d = r * 2;
            using var path = new GraphicsPath();
            path.AddArc(x,     y,     d, d, 180, 90);
            path.AddArc(x+w-d, y,     d, d, 270, 90);
            path.AddArc(x+w-d, y+h-d, d, d,   0, 90);
            path.AddArc(x,     y+h-d, d, d,  90, 90);
            path.CloseFigure();
            g.DrawPath(pen, path);
        }
    }
}
