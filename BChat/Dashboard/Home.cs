using BChat.UserControls;
using FontAwesome.Sharp;
using Guna.UI2.WinForms;
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

        public Home()
        {
            InitializeComponent();

            // ✅ يمنع الفلكشن بشكل كامل
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
        }

        private void Home_Load(object sender, EventArgs e)
        {
        }

        private void customPanel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            ResetButtons();
            btnCustomers.IconColor = Color.WhiteSmoke;


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
            btnMessages.IconColor = Color.WhiteSmoke;


            if (!pnlContent.Controls.ContainsKey("Messages_View"))
            {
                MessagesControl messagesPage = new MessagesControl();
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
                if (ctrl is FontAwesome.Sharp.IconButton btn)
                {

                    btn.BackColor = Color.Transparent;
                    btn.IconColor = Color.FromArgb(150, 255, 255, 255);

                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
                    btn.FlatAppearance.BorderSize = 0;

                }
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
