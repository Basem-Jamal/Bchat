using BChat.Salla;
using BChat.UserControls;
using FontAwesome.Sharp;
using Guna.UI2.WinForms;
using ReaLTaiizor.Animate.Parrot;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Guna.UI2.WinForms.Suite.Descriptions;

namespace BChat
{
    public partial class Home : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }
         
        public Home()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
        }

        private async void Home_Load(object sender, EventArgs e)
        {
        }

        private void btnNavHome_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnNavHome.IsActive = true;

            pnlContent.Controls.Clear();


        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnNavCustomers.IsActive = true;


            if (!pnlContent.Controls.ContainsKey("Customers_View"))
            {
                CustomersControl customersPage = new CustomersControl();
                customersPage.Name = "Customers_View";
                customersPage.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(customersPage);
            }

            pnlContent.Controls["Customers_View"].BringToFront();

        }
        private void btnMessages_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnNavMessages.IsActive = true;


            if (!pnlContent.Controls.ContainsKey("Messages_View"))
            {
                CampaignsControl messagesPage = new CampaignsControl();
                messagesPage.Name = "Messages_View";
                messagesPage.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(messagesPage);
            }

            pnlContent.Controls["Messages_View"].BringToFront();

        }


        private void btnOrders_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnOrders.IconColor = Color.WhiteSmoke;

            MessageBox.Show("الصفحة غير متوفرة, ولن تتوفر حتى يتم الربط مع سلة!", "Salla", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //if (!pnlContent.Controls.ContainsKey("Messages_View"))
            //{
            //    MessagesControl messagesPage = new MessagesControl();
            //    messagesPage.Name = "Messages_View";
            //    messagesPage.Dock = DockStyle.Fill;
            //    pnlContent.Controls.Add(messagesPage);
            //}

            //pnlContent.Controls["Messages_View"].BringToFront();

        }


        private void btnScheduledMessages_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnScheduledMessages.IconColor = Color.WhiteSmoke;


            if (!pnlContent.Controls.ContainsKey("ScheduledMessages_View"))
            {

                ScheduledControl scheduledPage = new ScheduledControl();
                scheduledPage.Name = "ScheduledMessages_View";
                scheduledPage.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(scheduledPage);
            }

            pnlContent.Controls["ScheduledMessages_View"].BringToFront();

        }
        private void btnTemplates_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnTemplates.IconColor = Color.WhiteSmoke;


            if (!pnlContent.Controls.ContainsKey("Templates_View"))
            {
                TemplatesControl templatesPage = new TemplatesControl();
                templatesPage.Name = "Templates_View";
                templatesPage.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(templatesPage);
            }

            pnlContent.Controls["Templates_View"].BringToFront();

        }
        private void ResetButtons()
        {
            foreach (Control ctrl in pnlMenuSidebar.Controls)
            {
                if (ctrl is BChat.Controls.ModernNavButton btn)
                {

                    btn.BaseBackground = Color.FromArgb(37, 43, 74);
                    btn.NormalTextColor = Color.Gray;
                    btn.IsActive = false;
                }
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            frm.ShowDialog();
        }

        private void btnNavCustomerGroups_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnNavCustomerGroups.IsActive = true;


            if (!pnlContent.Controls.ContainsKey("CustomerGroups_View"))
            {
                GroupsControl customerGroupsPage = new GroupsControl();
                customerGroupsPage.Name = "CustomerGroups_View";
                customerGroupsPage.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(customerGroupsPage);
            }

            pnlContent.Controls["CustomerGroups_View"].BringToFront();

        }

    }
}
