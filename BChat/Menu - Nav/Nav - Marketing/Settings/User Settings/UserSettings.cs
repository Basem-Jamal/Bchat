using BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings.InfoUser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings
{
    public partial class UserSettings : Form
    {
        public UserSettings()
        {
            InitializeComponent();


        }

        private void UserSettings_Load(object sender, EventArgs e)
        {
            LoadPage();
        }
        private void LoadPage()
        {

            if (!pnlContent.Controls.ContainsKey("EditUser_View"))
            {
                EditUser editUser = new EditUser();
                editUser.Name = "EditUser_View";
                editUser.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(editUser);
            }

            pnlContent.Controls["EditUser_View"].BringToFront();

        }
        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            
        }
    }
}
