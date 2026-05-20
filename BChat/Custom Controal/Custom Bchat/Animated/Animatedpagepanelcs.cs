using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Drawing;
using System.Windows.Forms;

public static class ControlExtensions
{
    public static void OpacityFake(
        this Control control,
        float opacity
    )
    {
        opacity = Math.Max(0f, Math.Min(1f, opacity));

        control.ForeColor = Color.FromArgb(
            (int)(255 * opacity),
            control.ForeColor
        );
    }
}
namespace BChat.Custom_Controal.Custom_Bchat.Animated
{
    // ═══════════════════════════════════════════════
    // ENUMS
    // ═══════════════════════════════════════════════

    public enum PageTransitionType
    {
        Fade,
        SlideFromRight,
        SlideFromLeft,
        SlideFromBottom,
        SlideFromTop,
        ZoomIn,
        ZoomOut,
        SlideFade,
        None
    }

    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        Bounce,
        Elastic,
        CubicEaseInOut
    }

    // ═══════════════════════════════════════════════
    // EVENTS
    // ═══════════════════════════════════════════════

    public class PageTransitionEventArgs : EventArgs
    {
        public Control OldPage { get; }
        public Control NewPage { get; }
        public PageTransitionType TransitionType { get; }

        public PageTransitionEventArgs(Control oldPage, Control newPage, PageTransitionType type)
        {
            OldPage = oldPage;
            NewPage = newPage;
            TransitionType = type;
        }
    }

    // ═══════════════════════════════════════════════
    // MAIN CONTROL
    // ═══════════════════════════════════════════════

    [ToolboxItem(true)]
    public class AnimatedPagePanel : Panel
    {
      
        // ─────────────────────────────────────────────
        // Fields
        // ─────────────────────────────────────────────

        private readonly Timer _timer;

        private Bitmap _bmpOld;
        private Bitmap _bmpNew;

        private float _progress;
        private bool _isAnimating;

        private Control _currentPage;
        private Control _pendingPage;

        private DateTime _animStartTime;

        private readonly Queue<Control> _navQueue = new();

        // ─────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────

        private PageTransitionType _transitionType = PageTransitionType.SlideFade;
        private EasingType _easingType = EasingType.EaseInOut;
        private int _duration = 320;
        private bool _animationsEnabled = true;
        private int _fps = 60;
        private bool _queueTransitions = false;

        [Category("Transition")]
        public PageTransitionType TransitionType
        {
            get => _transitionType;
            set => _transitionType = value;
        }

        [Category("Transition")]
        public EasingType EasingFunction
        {
            get => _easingType;
            set => _easingType = value;
        }

        [Category("Transition")]
        public int TransitionDuration
        {
            get => _duration;
            set => _duration = Math.Max(50, Math.Min(3000, value));
        }

        [Category("Transition")]
        public bool AnimationsEnabled
        {
            get => _animationsEnabled;
            set => _animationsEnabled = value;
        }

        [Category("Transition")]
        public int FramesPerSecond
        {
            get => _fps;
            set
            {
                _fps = Math.Max(10, Math.Min(144, value));

                if (_timer != null)
                    _timer.Interval = 1000 / _fps;
            }
        }

        [Category("Transition")]
        public bool QueueTransitions
        {
            get => _queueTransitions;
            set => _queueTransitions = value;
        }

        [Browsable(false)]
        public bool IsAnimating => _isAnimating;

        [Browsable(false)]
        public Control CurrentPage => _currentPage;

        // ─────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────

        public event EventHandler<PageTransitionEventArgs> TransitionStarted;
        public event EventHandler<PageTransitionEventArgs> TransitionCompleted;

        // ─────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────

        public AnimatedPagePanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true
            );

            DoubleBuffered = true;
            UpdateStyles();

            _timer = new Timer
            {
                Interval = 1000 / _fps
            };

            _timer.Tick += OnTimerTick;
        }

        // ─────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────

        public void RegisterPage(Control page, string name = null)
        {
            if (page == null)
                return;

            if (!Controls.Contains(page))
            {
                if (!string.IsNullOrWhiteSpace(name))
                    page.Name = name;

                page.Dock = DockStyle.Fill;
                page.Visible = false;

                EnableDoubleBuffering(page);

                Controls.Add(page);
            }
        }

        public Control GetPage(string name)
        {
            return Controls[name];
        }

        public void NavigateTo(Control page)
        {
            if (page == null)
                return;

            if (_isAnimating)
            {
                if (_queueTransitions)
                {
                    _navQueue.Enqueue(page);
                    return;
                }

                ForceCompleteAnimation();
            }

            if (page == _currentPage)
                return;

            if (!Controls.Contains(page))
            {
                page.Dock = DockStyle.Fill;
                page.Visible = false;

                EnableDoubleBuffering(page);

                Controls.Add(page);
            }

            // أول صفحة
            if (_currentPage == null)
            {
                page.Visible = true;
                page.BringToFront();

                _currentPage = page;

                return;
            }

            Control oldPage = _currentPage;

            // جهز الصفحة الجديدة
            page.Visible = true;
            page.Bounds = ClientRectangle;
            page.BringToFront();

            page.Refresh();
            Application.DoEvents();

            // تصوير الصفحتين
            DisposeBitmaps();

            _bmpOld = CaptureControl(oldPage);
            _bmpNew = CaptureControl(page);

            // أخفِ الكنترول الحقيقي أثناء الرسم
            oldPage.Visible = false;
            page.Visible = false;

            _pendingPage = page;

            _progress = 0f;
            _isAnimating = true;

            _animStartTime = DateTime.Now;

            TransitionStarted?.Invoke(
                this,
                new PageTransitionEventArgs(oldPage, page, _transitionType)
            );

            _timer.Start();
        }
        public void ForceCompleteAnimation()
        {
            if (!_isAnimating)
                return;

            _timer.Stop();

            CompleteTransition();
        }

        // ─────────────────────────────────────────────
        // Animation
        // ─────────────────────────────────────────────

        private void OnTimerTick(object sender, EventArgs e)
        {
            float elapsed =
                (float)(DateTime.Now - _animStartTime).TotalMilliseconds;

            _progress = Math.Min(elapsed / _duration, 1f);

            Invalidate();

            if (_progress >= 1f)
            {
                _timer.Stop();
                CompleteTransition();
            }
        }

        private void CompleteTransition()
        {
            _isAnimating = false;

            Control old = _currentPage;

            _currentPage = _pendingPage;
            _pendingPage = null;

            if (old != null)
                old.Visible = false;

            if (_currentPage != null)
            {
                _currentPage.Visible = true;
                _currentPage.BringToFront();
            }

            DisposeBitmaps();

            Invalidate();

            TransitionCompleted?.Invoke(
                this,
                new PageTransitionEventArgs(old, _currentPage, _transitionType)
            );

            if (_queueTransitions && _navQueue.Count > 0)
            {
                NavigateTo(_navQueue.Dequeue());
            }
        }

        // ─────────────────────────────────────────────
        // Paint
        // ─────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!_isAnimating)
                return;

            if (_bmpOld == null || _bmpNew == null)
                return;

            float t = Ease(_progress);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            DrawTransition(e.Graphics, t);
        }
        private void DrawTransition(Graphics g, float t)
        {
            Rectangle rect = ClientRectangle;

            switch (_transitionType)
            {
                case PageTransitionType.Fade:
                    DrawFade(g, t, rect);
                    break;

                case PageTransitionType.SlideFromRight:
                    DrawSlide(g, t, 1, 0);
                    break;

                case PageTransitionType.SlideFromLeft:
                    DrawSlide(g, t, -1, 0);
                    break;

                case PageTransitionType.SlideFromBottom:
                    DrawSlide(g, t, 0, 1);
                    break;

                case PageTransitionType.SlideFromTop:
                    DrawSlide(g, t, 0, -1);
                    break;

                case PageTransitionType.ZoomIn:
                    DrawZoom(g, t, true);
                    break;

                case PageTransitionType.ZoomOut:
                    DrawZoom(g, t, false);
                    break;

                case PageTransitionType.SlideFade:
                    DrawSlideFade(g, t);
                    break;
            }
        }

        // ─────────────────────────────────────────────
        // Effects
        // ─────────────────────────────────────────────

        private void DrawFade(Graphics g, float t, Rectangle rect)
        {
            DrawWithOpacity(g, _bmpOld, rect, 1f - t);
            DrawWithOpacity(g, _bmpNew, rect, t);
        }

        private void DrawSlide(Graphics g, float t, int dirX, int dirY)
        {
            int oldX = -(int)(dirX * Width * t);
            int oldY = -(int)(dirY * Height * t);

            int newX = (int)(dirX * Width * (1f - t));
            int newY = (int)(dirY * Height * (1f - t));

            g.DrawImage(_bmpOld, new Rectangle(oldX, oldY, Width, Height));
            g.DrawImage(_bmpNew, new Rectangle(newX, newY, Width, Height));
        }

        private void DrawSlideFade(Graphics g, float t)
        {
            int oldX = -(int)(Width * 0.20f * t);

            DrawWithOpacity(
                g,
                _bmpOld,
                new Rectangle(oldX, 0, Width, Height),
                1f - t
            );

            int newX = (int)(Width * 0.20f * (1f - t));

            DrawWithOpacity(
                g,
                _bmpNew,
                new Rectangle(newX, 0, Width, Height),
                t
            );
        }

        private void DrawZoom(Graphics g, float t, bool zoomIn)
        {
            DrawWithOpacity(
                g,
                _bmpOld,
                ClientRectangle,
                1f - t
            );

            float scale = zoomIn
                ? 0.85f + (0.15f * t)
                : 1.15f - (0.15f * t);

            int w = (int)(Width * scale);
            int h = (int)(Height * scale);

            int x = (Width - w) / 2;
            int y = (Height - h) / 2;

            DrawWithOpacity(
                g,
                _bmpNew,
                new Rectangle(x, y, w, h),
                t
            );
        }

        // ─────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────

        private Bitmap CaptureControl(Control ctrl)
        {
            Bitmap bmp = new Bitmap(
                ctrl.Width,
                ctrl.Height
            );

            ctrl.Invoke(new Action(() =>
            {
                ctrl.DrawToBitmap(
                    bmp,
                    new Rectangle(0, 0, ctrl.Width, ctrl.Height)
                );
            }));

            return bmp;
        }
        private static void DrawWithOpacity(
            Graphics g,
            Bitmap bmp,
            Rectangle dest,
            float opacity
        )
        {
            if (bmp == null || opacity <= 0f)
                return;

            opacity = Math.Min(1f, opacity);

            using ImageAttributes ia = new();

            ColorMatrix cm = new();
            cm.Matrix33 = opacity;

            ia.SetColorMatrix(
                cm,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap
            );

            g.DrawImage(
                bmp,
                dest,
                0,
                0,
                bmp.Width,
                bmp.Height,
                GraphicsUnit.Pixel,
                ia
            );
        }

        // ─────────────────────────────────────────────
        // Easing
        // ─────────────────────────────────────────────

        private float Ease(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));

            switch (_easingType)
            {
                case EasingType.Linear:
                    return t;

                case EasingType.EaseIn:
                    return t * t * t;

                case EasingType.EaseOut:
                    float inv = 1f - t;
                    return 1f - inv * inv * inv;

                case EasingType.EaseInOut:
                    return t < 0.5f
                        ? 4f * t * t * t
                        : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;

                case EasingType.CubicEaseInOut:
                    return t < 0.5f
                        ? 4f * t * t * t
                        : (float)(1 - Math.Pow(-2 * t + 2, 3) / 2);

                case EasingType.Bounce:
                    return EaseBounce(t);

                case EasingType.Elastic:
                    return EaseElastic(t);

                default:
                    return t;
            }
        }

        private float EaseBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
                return n1 * t * t;

            if (t < 2f / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            t -= 2.625f / d1;

            return n1 * t * t + 0.984375f;
        }

        private float EaseElastic(float t)
        {
            if (t <= 0f)
                return 0f;

            if (t >= 1f)
                return 1f;

            const float c4 = (float)(2 * Math.PI / 3);

            return (float)(
                Math.Pow(2, -10 * t) *
                Math.Sin((t * 10 - 0.75) * c4) + 1
            );
        }

        // ─────────────────────────────────────────────
        // Performance
        // ─────────────────────────────────────────────

        private void EnableDoubleBuffering(Control control)
        {
            typeof(Control)
                .GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance
                )
                ?.SetValue(control, true, null);

            foreach (Control child in control.Controls)
                EnableDoubleBuffering(child);
        }

        private void DisposeBitmaps()
        {
            _bmpOld?.Dispose();
            _bmpOld = null;

            _bmpNew?.Dispose();
            _bmpNew = null;
        }

        // ─────────────────────────────────────────────
        // Dispose
        // ─────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Stop();
                _timer?.Dispose();

                DisposeBitmaps();
            }

            base.Dispose(disposing);
        }
    }
}