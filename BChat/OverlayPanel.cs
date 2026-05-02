// Controls/OverlayPanel.cs — v2
// ✅ يدعم Form و UserControl و أي Control آخر
// ✅ Capture يعمل على أي Control وليس فقط Form
// ✅ باقي الميزات محفوظة (Blur, Fade In/Out, Tint)

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Controls
{
    public class OverlayPanel : Panel
    {
        // ─── إعدادات الـ Animation ────────────────────
        private const int FADE_STEPS = 12;
        private const int FADE_INTERVAL = 15;   // ms
        private const int MAX_ALPHA = 80;

        // ─── State ───────────────────────────────────
        private Image? _blurredBackground;
        private int _currentAlpha = 0;
        private Timer _fadeTimer = new Timer();
        private bool _isFadingIn = true;
        private Action? _onFadeOutComplete;

        // ─── صاحب الـ Overlay (Form أو Control) ──────
        private Control? _targetControl;

        public int BlurRadius { get; set; } = 6;

        // ─── Constructor ─────────────────────────────
        public OverlayPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            Dock = DockStyle.Fill;
            Cursor = Cursors.Default;

            _fadeTimer.Interval = FADE_INTERVAL;
            _fadeTimer.Tick += OnFadeTick;
        }

        // ─── الرسم ───────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_blurredBackground != null)
                e.Graphics.DrawImage(_blurredBackground, ClientRectangle);

            using var tintBrush = new SolidBrush(Color.FromArgb(_currentAlpha, 15, 15, 40));
            e.Graphics.FillRectangle(tintBrush, ClientRectangle);
        }

        // ─── Fade Tick ───────────────────────────────
        private void OnFadeTick(object? sender, EventArgs e)
        {
            int step = Math.Max(1, MAX_ALPHA / FADE_STEPS);

            if (_isFadingIn)
            {
                _currentAlpha = Math.Min(_currentAlpha + step, MAX_ALPHA);
                Invalidate();
                if (_currentAlpha >= MAX_ALPHA) _fadeTimer.Stop();
            }
            else
            {
                _currentAlpha = Math.Max(_currentAlpha - step, 0);
                Invalidate();
                if (_currentAlpha <= 0)
                {
                    _fadeTimer.Stop();
                    _onFadeOutComplete?.Invoke();
                }
            }
        }

        // ─────────────────────────────────────────────
        //  Show — يقبل Form أو أي Control
        // ─────────────────────────────────────────────

        /// <summary>يعرض الـ Overlay فوق Form كامل.</summary>
        public static OverlayPanel Show(Form targetForm)
            => ShowOnControl(targetForm);

        /// <summary>يعرض الـ Overlay فوق UserControl أو أي Control آخر.</summary>
        public static OverlayPanel Show(Control targetControl)
            => ShowOnControl(targetControl);

        // ─── الـ Core المشترك ─────────────────────────
        private static OverlayPanel ShowOnControl(Control target)
        {
            var overlay = new OverlayPanel();
            overlay._targetControl = target;
            overlay._blurredBackground = CaptureControl(target, overlay.BlurRadius);
            overlay._currentAlpha = 0;
            overlay._isFadingIn = true;

            // أضفه لنفس الـ Controls collection وأحضره للأمام
            target.Controls.Add(overlay);
            overlay.BringToFront();

            overlay._fadeTimer.Start();
            return overlay;
        }

        // ─── Close مع Fade Out ────────────────────────

        /// <summary>يخفي الـ Overlay بـ Fade Out ويُزيله من الـ Form.</summary>
        public void Close(Form targetForm)
            => CloseFromControl(targetForm);

        /// <summary>يخفي الـ Overlay بـ Fade Out ويُزيله من الـ Control.</summary>
        public void Close(Control targetControl)
            => CloseFromControl(targetControl);

        /// <summary>يخفي الـ Overlay تلقائياً باستخدام الـ target المحفوظ عند Show.</summary>
        public void Close()
        {
            if (_targetControl != null)
                CloseFromControl(_targetControl);
        }

        private void CloseFromControl(Control target)
        {
            _isFadingIn = false;
            _onFadeOutComplete = () =>
            {
                if (target.IsHandleCreated)
                    target.Controls.Remove(this);

                _blurredBackground?.Dispose();
                _blurredBackground = null;
                Dispose();
            };

            _fadeTimer.Start();
        }

        // ─── Capture ─────────────────────────────────

        /// <summary>
        /// يلتقط صورة الـ Control كما تبدو على الشاشة.
        /// يعمل مع Form و UserControl و Panel وأي Control آخر.
        /// </summary>
        private static Bitmap CaptureControl(Control ctrl, int blurRadius)
        {
            // نستخدم ClientSize لأنه صحيح سواء كان Form أو UserControl
            int w = ctrl.ClientSize.Width;
            int h = ctrl.ClientSize.Height;

            if (w <= 0 || h <= 0)
                return new Bitmap(1, 1);

            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            // DrawToBitmap يعمل على أي Control وليس فقط Form
            ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, w, h));

            return ApplyGaussianBlur(bmp, blurRadius);
        }

        // ─── Gaussian Blur ───────────────────────────
        private static Bitmap ApplyGaussianBlur(Bitmap source, int radius)
        {
            if (radius <= 0) return source;

            Bitmap result = source;
            for (int pass = 0; pass < 3; pass++)
                result = BoxBlurVertical(BoxBlurHorizontal(result, radius), radius);

            return result;
        }

        private static Bitmap BoxBlurHorizontal(Bitmap src, int radius)
        {
            var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height),
                                       ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int width = src.Width, height = src.Height, stride = srcData.Stride;
            unsafe
            {
                byte* s = (byte*)srcData.Scan0;
                byte* d = (byte*)dstData.Scan0;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        int r = 0, g = 0, b = 0, count = 0;
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int nx = Math.Clamp(x + kx, 0, width - 1);
                            byte* p = s + y * stride + nx * 4;
                            b += p[0]; g += p[1]; r += p[2]; count++;
                        }
                        byte* dp = d + y * stride + x * 4;
                        dp[0] = (byte)(b / count);
                        dp[1] = (byte)(g / count);
                        dp[2] = (byte)(r / count);
                        dp[3] = 255;
                    }
            }
            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
            return dst;
        }

        private static Bitmap BoxBlurVertical(Bitmap src, int radius)
        {
            var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height),
                                       ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int width = src.Width, height = src.Height, stride = srcData.Stride;
            unsafe
            {
                byte* s = (byte*)srcData.Scan0;
                byte* d = (byte*)dstData.Scan0;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        int r = 0, g = 0, b = 0, count = 0;
                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int ny = Math.Clamp(y + ky, 0, height - 1);
                            byte* p = s + ny * stride + x * 4;
                            b += p[0]; g += p[1]; r += p[2]; count++;
                        }
                        byte* dp = d + y * stride + x * 4;
                        dp[0] = (byte)(b / count);
                        dp[1] = (byte)(g / count);
                        dp[2] = (byte)(r / count);
                        dp[3] = 255;
                    }
            }
            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
            return dst;
        }
    }
}