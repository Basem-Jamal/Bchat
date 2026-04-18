using BChat.Controls;
using BChat.Custom_Controal;
using BChat.Data.DataStore;
using BChat.Events;
using BChat.Forms;
using BChat.Global;
using BChat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.UserControls
{
    public partial class GroupsControl : UserControl
    {
        public GroupsControl()
        {
            InitializeComponent();
            groupsWrapPanel.RightToLeft = RightToLeft.Yes;

            AppEvents.Groups.OnGroupAdded += group => groupsWrapPanel.AddGroup(group, GeneralFunctions.Base64ToImage);
            AppEvents.Groups.OnGroupUpdated += group => groupsWrapPanel.UpdateGroup(group);

   
        }
        private void CustomerGroupsControl_Load(object sender, EventArgs e)
        {
            LoadCustomerGroups();
        }
        private void LoadCustomerGroups()
        {

            

            groupsWrapPanel.LoadGroups(AppCache.Groups, GeneralFunctions.Base64ToImage);


        }

        private void btnRefreshData_Click(object sender, EventArgs e)
        {
            LoadCustomerGroups();
        }



        private void groupsWrapPanel_AddCardClicked(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            fmAddGroup fmAddGroup = new fmAddGroup(CustomerGroupStatus.Add);
            fmAddGroup.ShowDialog();

            overlay.Close(mainForm);

        }

        private void groupsWrapPanel_CardDeleteClicked(object sender, int e)
        {
            if (MessageBox.Show(this, "هل أنت متأكد أنك تريد حذف هذه المجموعة؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                GroupRepository.Delete(e);
                groupsWrapPanel.RemoveGroup(e);
                AppCache.Groups.RemoveAll(g => g.Id == e);

            }
            else
            {
                MessageBox.Show(this, "تم الغاء الحذف.", "الغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void groupsWrapPanel_CardEditClicked(object sender, int e)
        {
            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            var group = groupsWrapPanel.GetGroupFromCard(e);
            
            fmAddGroup fmAddGroup = new fmAddGroup(CustomerGroupStatus.Edit, group);
            fmAddGroup.ShowDialog();

            overlay.Close(mainForm);


        }
    }
}
