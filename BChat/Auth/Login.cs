using BChat.Data.DataStore.Users_DB;
using BChat.Global;
using BChat.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Auth
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();

            
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
