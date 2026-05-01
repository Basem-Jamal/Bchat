using BChat.Data.DataStore.Users_DB;
using BChat.Events;
using BChat.Global;
using BChat.Models.Users;
using BChat.Models.Users.ModulePermission;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BChat.Models.Users.ModulePermission.Permission;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings.InfoUser
{
    public partial class EditUser : UserControl
    {
        private Dictionary<string, int> userMap = new();

        private bool _isLoading = false;

        private User _user = new User();
        public EditUser()
        {
            InitializeComponent();

            // حاليا وقفت عند تعديل الصلاحيات
            cmbUser.SelectedIndexChanged += cmbUser_SelectedIndexChanged;
            LoadUsers();
        }
        private void LoadUsers()
        {

            _isLoading = true;

            cmbUser.Items.Clear();
            userMap.Clear();

            foreach (var item in AppCache.Users)
            {
                string Role = item.Role;
                string Result = item.Name + " \"" + item.Role + "\"";
                cmbUser.AddItem(Result);

                userMap[Result] = item.Id;


            }

            _isLoading = false;


            if (cmbUser.Items.Count > 0)
                cmbUser.SelectedIndex = 0;

        }

        private void LoadDataUser(int Id)
        {
            LoadUserName(Id);
            LoadUserEmail(Id);
            LoadPassword(Id);
            LoadUserBranch(Id);
            LoadCurrentUserPermission(Id);
            LoadNewUserPermissions(Id);
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
                MessageBox.Show("قد لا يكون هناك ايميل", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txbEmail.Text = userEmail.Email;

        }

        private void LoadPassword(int Id)
        {
            var userPassword = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userPassword == null)
            {
                MessageBox.Show("قد لا يكون هناك كلمة مرورو", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txbPassword.Text = userPassword.Password;

        }
        private void LoadUserBranch(int Id)
        {
            cmbBranch.Items.Clear();

            var userBranch = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userBranch == null)
            {
                MessageBox.Show("قد لا يكون هناك فرع", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cmbBranch.AddItem(userBranch.BranchId.ToString());
        }
        private void LoadCurrentUserPermission(int Id)
        {
            var currentUserPermission = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (currentUserPermission == null)
            {
                MessageBox.Show("قد لا يكون صلاحية محدده مسبقا للمستخدم", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblCurrentPermission.Text = currentUserPermission.Role;


        }

        private void LoadNewUserPermissions(int Id)
        {
            cmbSelectNewPermissions.Items.Clear();

            var userPermissins = Enum.GetValues(typeof(PermissionType));

            if (userPermissins == null)
            {
                MessageBox.Show("قد لا يكون هناك صلاحية في الخيارات", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (PermissionType permission in userPermissins)
            {
                cmbSelectNewPermissions.AddItem(permission.ToString());

            }

        }

        private void LoadUserDate(int Id)
        {
            var userDate = AppCache.Users.FirstOrDefault(u => u.Id == Id);

            if (userDate == null)
            {
                MessageBox.Show("قد لا يكون هناك تاريخ انشاء", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lblUserDate.Text = userDate.CreatedAt.ToString("yyyy/MM/dd");
        }
        private void cmbUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;

            var selected = cmbUser.SelectedItem;

            if (selected != null)
            {
                string text = selected.ToString();

                if (userMap.TryGetValue(text, out int Id))
                {
                    _user.Id = Id;
                    LoadDataUser(Id);
                }

            }
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {

            User user = new User()
            {
                Id = _user.Id,
                Name = txbName.Text,
                Email = txbEmail.Text,
                Password = txbPassword.Text,
                BranchId = null,
                IsActive = true,
                Role = cmbSelectNewPermissions.SelectedItem?.ToString()
            };
            bool success = UsersRepository.Update(user);

            if (success)
            {
                var userUpdateInCache = AppCache.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userUpdateInCache != null)
                {
                    userUpdateInCache.Name = user.Name;
                    userUpdateInCache.Email = user.Email;
                    userUpdateInCache.Password = user.Password;
                    userUpdateInCache.Role = user.Role;
                    userUpdateInCache.BranchId = user.BranchId;
                    userUpdateInCache.IsActive = user.IsActive;

                }
                RefershCurrentUser();

                AppEvents.AppUsers.ChangeRefershAllUsers();
                MessageBox.Show($"تم تحديث اليوزر {user.Name}", "Bchat Permission", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            else
            {
                MessageBox.Show($"لم يتم تحديث اليوزر {user.Name}", "Bchat Permission", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void RefershCurrentUser()
        {
            var CurrentUse = AppCache.Users.FirstOrDefault(u => u.Id == AppCache.CurrentUser.Id);
            if (CurrentUse != null)
            {
                AppCache.CurrentUser = CurrentUse;
            }
            
        }
    }
}
