// Controls/OverlayPanel.cs
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
        private const int FADE_STEPS = 12;      // عدد الخطوات
        private const int FADE_INTERVAL = 15;      // ms بين كل خطوة
        private const int MAX_ALPHA = 80;      // الشفافية النهائية للتلوين

        // ─── State ───────────────────────────────────
        private Image? _blurredBackground;
        private int _currentAlpha = 0;
        private Timer _fadeTimer = new Timer();
        private bool _isFadingIn = true;
        private Action? _onFadeOutComplete;           // callback بعد الاختفاء

        public int BlurRadius { get; set; } = 6;

        // ─── Constructor ─────────────────────────────
        public OverlayPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Dock = DockStyle.Fill;
            Cursor = Cursors.Default;

            // إعداد الـ Timer
            _fadeTimer.Interval = FADE_INTERVAL;
            _fadeTimer.Tick += OnFadeTick;
        }

        // ─── الرسم ───────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_blurredBackground != null)
                e.Graphics.DrawImage(_blurredBackground, ClientRectangle);

            // التلوين الداكن يتغير مع الـ alpha
            using var tintBrush = new SolidBrush(Color.FromArgb(_currentAlpha, 15, 15, 40));
            e.Graphics.FillRectangle(tintBrush, ClientRectangle);
        }

        // ─── Fade Tick ───────────────────────────────
        private void OnFadeTick(object? sender, EventArgs e)
        {
            int step = MAX_ALPHA / FADE_STEPS;

            if (_isFadingIn)
            {
                _currentAlpha = Math.Min(_currentAlpha + step, MAX_ALPHA);
                Invalidate();

                if (_currentAlpha >= MAX_ALPHA)
                    _fadeTimer.Stop();
            }
            else
            {
                _currentAlpha = Math.Max(_currentAlpha - step, 0);
                Invalidate();

                if (_currentAlpha <= 0)
                {
                    _fadeTimer.Stop();
                    _onFadeOutComplete?.Invoke();   // أخبر الـ caller إن الإخفاء انتهى
                }
            }
        }

        // ─── Show ─────────────────────────────────────
        public static OverlayPanel Show(Form targetForm)
        {
            var overlay = new OverlayPanel();
            overlay._blurredBackground = CaptureAndBlur(targetForm, overlay.BlurRadius);
            overlay._currentAlpha = 0;
            overlay._isFadingIn = true;

            targetForm.Controls.Add(overlay);
            overlay.BringToFront();

            overlay._fadeTimer.Start();   // ابدأ الـ Fade In
            return overlay;
        }

        // ─── Close مع Fade Out ────────────────────────
        public void Close(Form targetForm)
        {
            _isFadingIn = false;
            _onFadeOutComplete = () =>
            {
                // بعد انتهاء الاختفاء — أزل نفسك
                if (targetForm.IsHandleCreated)
                    targetForm.Controls.Remove(this);

                _blurredBackground?.Dispose();
                Dispose();
            };

            _fadeTimer.Start();   // ابدأ الـ Fade Out
        }

        // ─── Capture + Blur ──────────────────────────
        private static Bitmap CaptureAndBlur(Form form, int radius)
        {
            var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
            form.DrawToBitmap(bmp, new Rectangle(Point.Empty, form.ClientSize));
            return ApplyGaussianBlur(bmp, radius);
        }

        private static Bitmap ApplyGaussianBlur(Bitmap source, int radius)
        {
            Bitmap result = source;
            for (int pass = 0; pass < 3; pass++)
            {
                result = BoxBlurVertical(BoxBlurHorizontal(result, radius), radius);
            }
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
                        dp[0] = (byte)(b / count); dp[1] = (byte)(g / count);
                        dp[2] = (byte)(r / count); dp[3] = 255;
                    }
            }
            src.UnlockBits(srcData); dst.UnlockBits(dstData);
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
                        dp[0] = (byte)(b / count); dp[1] = (byte)(g / count);
                        dp[2] = (byte)(r / count); dp[3] = 255;
                    }
            }
            src.UnlockBits(srcData); dst.UnlockBits(dstData);
            return dst;
        }
    }
}