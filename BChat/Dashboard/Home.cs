using BChat.Auth;
using BChat.Controls;
using BChat.Custom_Controal.Custom_Bchat.Animated;
using BChat.Events;
using BChat.Global;
using BChat.Menu___Nav.Nav___Marketing;
using BChat.Menu___Nav.UserControls.Today_s_Summary_Report_UC;
using BChat.Salla;
using BChat.UserControls;
using System.Runtime.InteropServices;

namespace BChat
{
    public partial class Home : Form
    {
        // ─────────────────────────────────────────────
        // Window Drag
        // ─────────────────────────────────────────────

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam
        );

        // ─────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────

        public Home()
        {
            InitializeComponent();

            // تحسين الرسم
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );

            DoubleBuffered = true;

            // تفعيل DoubleBuffer لـ pnlContent
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                pnlContent,
                new object[] { true }
            );

            // Events
            AppEvents.AppUsers.OnRefershUsers += RefreshUserUI;

            // User Name
            lblUserCurrentName.Text = AppCache.CurrentUser.Name;

            // ─────────────────────────────────────────
            // Animation Settings
            // ─────────────────────────────────────────
            //pnlContent.TransitionType = PageTransitionType.ZoomIn;

            //pnlContent.EasingFunction = EasingType.EaseIn;

            //pnlContent.TransitionDuration = 70;

            //pnlContent.FramesPerSecond = 20;

            //pnlContent.AnimationsEnabled = true;

            //pnlContent.QueueTransitions = true;
        }

        // ─────────────────────────────────────────────
        // Form Load
        // ─────────────────────────────────────────────

        private void Home_Load(object sender, EventArgs e)
        {
            OpenPage<ucMonthlySummary>(
                "MonthlyReports_View"
            );

            btnNavHome.IsActive = true;
        }

        // ─────────────────────────────────────────────
        // Window Drag
        // ─────────────────────────────────────────────

        private void pnlHeader_MouseDown(
            object sender,
            MouseEventArgs e
        )
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();

                SendMessage(
                    Handle,
                    WM_NCLBUTTONDOWN,
                    HT_CAPTION,
                    0
                );
            }
        }

        // ─────────────────────────────────────────────
        // User Refresh
        // ─────────────────────────────────────────────

        public void RefreshUserUI()
        {
            lblUserCurrentName.Text =
                AppCache.CurrentUser.Name;

            PermissionsInHome();
        }

        private void PermissionsInHome()
        {
            MessageBox.Show("تم تحديث الصلاحيات");
        }

        // ─────────────────────────────────────────────
        // Navigation
        // ─────────────────────────────────────────────

        private void btnNavHome_Click(
            object sender,
            EventArgs e
        )
        {
            ResetButtons();

            btnNavHome.IsActive = true;

            OpenPage<ucMonthlySummary>(
                "MonthlyReports_View"
            );
        }

        private void btnCustomers_Click(
            object sender,
            EventArgs e
        )
        {
            ResetButtons();

            btnNavCustomers.IsActive = true;

            OpenPage<CustomersControl>(
                "Customers_View"
            );
        }

        private void btnMessages_Click(
            object sender,
            EventArgs e
        )
        {
            ResetButtons();

            btnNavMessages.IsActive = true;

            OpenPage<ucMessageControl>(
                "Messages_View"
            );
        }

        private void btnNavCustomerGroups_Click(
            object sender,
            EventArgs e
        )
        {
            ResetButtons();

            btnNavCustomerGroups.IsActive = true;

            OpenPage<ucGroupsControl>(
                "CustomerGroups_View"
            );
        }

        // ─────────────────────────────────────────────
        // Marketing
        // ─────────────────────────────────────────────

        private void btnNavMarketingAPI_Click(
            object sender,
            EventArgs e
        )
        {
            ResetButtons();

            btnNavMarketingAPI.IsActive = true;

            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            Marketing marketing = new Marketing();

            marketing.ShowDialog();

            overlay.Close(marketing);
        }

        // ─────────────────────────────────────────────
        // Open Pages
        // ─────────────────────────────────────────────

        private void OpenPage<T>(string pageName)
            where T : Control, new()
        {
            if (pnlContent.GetPage(pageName) == null)
            {
                var page = new T
                {
                    Name = pageName,
                    Dock = DockStyle.Fill
                };

                pnlContent.RegisterPage(page);
            }

            pnlContent.NavigateTo(
                pnlContent.GetPage(pageName)
            );
        }

        // ─────────────────────────────────────────────
        // Reset Buttons
        // ─────────────────────────────────────────────

        private void ResetButtons()
        {
            foreach (Control ctrl in pnlMenuSidebar.Controls)
            {
                if (ctrl is ModernNavButton btn)
                {
                    btn.NormalTextColor = Color.White;

                    btn.IsActive = false;
                }
            }
        }

        // ─────────────────────────────────────────────
        // Close
        // ─────────────────────────────────────────────

        private void picClose_Click(
            object sender,
            EventArgs e
        )
        {
            FormClosed += (s, args) =>
            {
                AppEvents.AppUsers.OnRefershUsers
                    -= RefreshUserUI;
            };

            Close();
        }

        // ─────────────────────────────────────────────
        // Settings
        // ─────────────────────────────────────────────

        private void btnSettings_Click(
            object sender,
            EventArgs e
        )
        {
            MainForm frm = new MainForm();

            frm.ShowDialog();
        }

        // ─────────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────────

        private void btnLogout_Click(
            object sender,
            EventArgs e
        )
        {
            AppCache.CurrentUser = null;

            Close();
        }

        // ─────────────────────────────────────────────
        // Minimize
        // ─────────────────────────────────────────────

        private void btnFormMinimized_Click(
            object sender,
            EventArgs e
        )
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}