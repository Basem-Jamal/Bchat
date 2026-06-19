using BChat.Data.DataStore.Users_DB;
using BChat.Global;
using BChat.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Auth
{
    public partial class Login : Form


    {   // ─────────────────────────────────────────────
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


        public Login()
        {
            InitializeComponent();

            
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("يرجى إدخال البريد وكلمة المرور", "BChat",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            User? user = UsersRepository.Login(email, password);

            if (user == null)
            {
                MessageBox.Show("البريد أو كلمة المرور غير صحيحة", "BChat",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppCache.CurrentUser = user;

            this.DialogResult = DialogResult.OK;
            this.Close();


        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
