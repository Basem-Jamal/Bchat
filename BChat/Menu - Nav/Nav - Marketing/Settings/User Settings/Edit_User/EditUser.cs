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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings.InfoUser
{
    public partial class EditUser : UserControl
    {
        private Dictionary<string, int> userMap = new();
        public EditUser()
        {
            InitializeComponent();

            // حاليا وقفت عند تعديل الصلاحيات
            cmbUser.SelectedIndexChanged += cmbUser_SelectedIndexChanged;
            LoadUsers();
        }
        private void LoadUsers()
        {
            cmbUser.Items.Clear();

            foreach (var item in AppCache.Users)
            {
                string Role = item.Role;
                string Result = item.Name + " \"" + item.Role + "\"";
                cmbUser.AddItem(Result);

                userMap[Result] = item.Id;
            }
        }

        private void LoadDataUser(int Id)
        {
            LoadUserName(Id);
            LoadUserEmail(Id);
            LoadUserBranch(Id);
            LoadUserPermissions(Id);
            LoadUserDate(Id);
        }

        private void LoadUserName(int Id)
        {
            var userName = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userName == null)
            {
                MessageBox.Show("قد لا يكون هناك مستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txbName.Text = userName.Name;
        }
        private void LoadUserEmail(int Id)
        {
            var userEmail = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userEmail == null)
            {
                MessageBox.Show("قد لا يكون هناك مستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txbEmail.Text = userEmail.Email;

        }
        private void LoadUserBranch (int Id)
        {
            cmbBranch.Items.Clear();

            var userBranch = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userBranch == null)
            {
                MessageBox.Show("قد لا يكون هناك مستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cmbBranch.AddItem(userBranch.BranchId.ToString());
        }
        private void LoadUserPermissions(int Id)
        {
            cmbPermissions.Items.Clear();
            
            var userPermissins = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userPermissins == null)
            {
                MessageBox.Show("قد لا يكون هناك مستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var permission in AppCache.Users)
            {
                cmbPermissions.AddItem(permission.Role);

            }

        }
        private void LoadUserDate(int Id)
        {
            var userDate = AppCache.Users.FirstOrDefault(u =>u.Id == Id);

            if (userDate == null)
            {
                MessageBox.Show("قد لا يكون هناك مستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lblUserDate.Text = userDate.CreatedAt.ToString("yyyy/mm/dd");
        }
        private void cmbUser_SelectedIndexChanged(object sender, EventArgs e)
        {

            var selected = cmbUser.SelectedItem;

            if (selected != null)
            {
                string text = selected.ToString();

                if (userMap.TryGetValue(text, out int id))
                {
                    LoadDataUser(id);
                }

            }
        }
    }
}
