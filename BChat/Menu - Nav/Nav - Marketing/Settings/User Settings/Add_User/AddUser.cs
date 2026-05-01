using BChat.Data.DataStore.Users_DB;
using BChat.Events;
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

namespace BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings.Add_User
{
    public partial class AddUser : UserControl
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            //User user = new User()
            //{
            //    Name = txbName.Text,
            //    Email = txbEmail.Text,
            //    Password = txbPassword.Text,
            //    BranchId = null,
            //    IsActive = true,
            //    Role = cmbSelectNewPermissions.SelectedItem?.ToString()
            //};
            //bool success = UsersRepository.Add(user);

            //if (success)
            //{
            //    var userUpdateInCache = AppCache.Users.FirstOrDefault(u => u.Id == user.Id);
            //    if (userUpdateInCache != null)
            //    {
            //        userUpdateInCache.Name = user.Name;
            //        userUpdateInCache.Email = user.Email;
            //        userUpdateInCache.Password = user.Password;
            //        userUpdateInCache.Role = user.Role;
            //        userUpdateInCache.BranchId = user.BranchId;
            //        userUpdateInCache.IsActive = user.IsActive;

            //    }
            //    RefershCurrentUser();

            //    AppEvents.AppUsers.ChangeRefershAllUsers();
            //    MessageBox.Show($"تم تحديث اليوزر {user.Name}", "Bchat Permission", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

            //else
            //{
            //    MessageBox.Show($"لم يتم تحديث اليوزر {user.Name}", "Bchat Permission", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //}


        }
    }
}
