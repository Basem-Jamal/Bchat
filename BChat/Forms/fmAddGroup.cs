using BChat.Data.DataStore;
using BChat.Events;
using BChat.Global;
using BChat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Forms
{
    public partial class fmAddGroup : Form
    {
        private CustomerGroupStatus _customerGroupStatus;
        private Groups _customerGroup = null;

        private byte[] imageBytes = null;
        private string selectedPathIcon = null;
        private string selectedBGColor = null;
        public fmAddGroup(CustomerGroupStatus customerGroupStatus, Groups customerGroup = null)
        {
            InitializeComponent();

            lblFileTextIcon.Visible = false;
            lblSelectedBG.Visible = false;

            _customerGroupStatus = customerGroupStatus;
            _customerGroup = customerGroup;
            if (customerGroupStatus == CustomerGroupStatus.Edit)
            {
                LoadGroupData(customerGroup);
                btnAddOrEditCustomerGroups.Text = "تعديل المجموعة";
            }

            txbGroupName.MaxLength = 100;
            txbGroupDescription.MaxLength = 200;


        }

        private void fmAddGroup_Load(object sender, EventArgs e)
        {
            adColorPicker.ColorChanged += handelColor;
        }

        private void btnAddOrEditCustomerGroups_Click(object sender, EventArgs e)
        {
            if (!CheckTheFields())
                return;

            imageBytes = GeneralFunctions.ResizeImage(selectedPathIcon, 125, 125);

            if (CustomerGroupStatus.Add == _customerGroupStatus)
            {
                Groups newGroup = new Groups()
                {
                    Name = txbGroupName.Text,
                    Description = txbGroupDescription.Text,
                    Icon = Convert.ToBase64String(imageBytes),
                    IconBoxColor = selectedBGColor,
                };
                AddGroup(newGroup);

            }
            else
            {
                _customerGroup.Name = txbGroupName.Text;
                _customerGroup.Description = txbGroupDescription.Text;
                _customerGroup.Icon = Convert.ToBase64String(imageBytes);
                _customerGroup.IconBoxColor = selectedBGColor;


                UpdateGroup(_customerGroup);
            }

            this.Close();

        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadGroupData(Groups customerGroup)
        {
            if (customerGroup == null) return;

            adColorPicker.Hex = customerGroup.IconBoxColor;
            txbGroupName.Text = customerGroup.Name;
            txbGroupDescription.Text = customerGroup.Description;


            lblSelectedBG.Visible = true;

            lblSelectedBG.Text = customerGroup.IconBoxColor;
            lblSelectedBG.ForeColor = Color.Black;
            lblSelectedBG.BackColor = ColorTranslator.FromHtml(customerGroup.IconBoxColor);

            lblSelectedBG.ForeColor = Color.Red;

            selectedBGColor = customerGroup.IconBoxColor;

        }

        private void btnUplaodFileIcon_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر صورة";
                ofd.Filter = "Iamge Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedPathIcon = ofd.FileName;
                    string filePath = ofd.FileName;
                    IconImagePath(filePath);

                }
            }

        }
        private void handelColor(object sender, EventArgs e)
        {
            lblSelectedBG.Visible = true;
            lblSelectedBG.Text = GeneralFunctions.ColorToHex(adColorPicker.SelectedColor);
            lblSelectedBG.ForeColor = Color.Red;
            lblSelectedBG.BackColor = adColorPicker.SelectedColor;

            selectedBGColor = GeneralFunctions.ColorToHex(adColorPicker.SelectedColor);
        }
        private void IconImagePath(string path)
        {

            lblFileTextIcon.AutoEllipsis = true;
            lblFileTextIcon.AutoSize = false;
            lblFileTextIcon.Width = 260;


            lblFileTextIcon.Text = path;

            lblFileTextIcon.Visible = true;
            lblFileTextIcon.ForeColor = Color.Red;


        }

        private void AddGroup(Groups newGroup)
        {
            bool result = false;

            if (newGroup == null)
            {
                MessageBox.Show("يبدو انه لاتوجد قيمة في الحقول!", "Erorr System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            result = GroupRepository.Add(newGroup);

            if (result)
            {
                MessageBox.Show("تم اضافة المجموعة بنجاح", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //AppCache.Groups.Add(newGroup);              // ← أضف هذا
                AppEvents.Groups.ChangeGroupAdded(newGroup);
            }
            else
                MessageBox.Show("لم تتم اضافة المجموعة", "Erorr!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void UpdateGroup(Groups EditGroup)
        {
            bool result = false;

            if (EditGroup == null)
            {
                MessageBox.Show("يبدو انه لاتوجد قيمة ففي الحقول!", "Erorr System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            result = GroupRepository.Update(EditGroup);

            if (result)
            {
                MessageBox.Show("تم تعديل المجموعة بنجاح", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);


                //var existing = AppCache.Groups.FirstOrDefault(g => g.Id == EditGroup.Id);
                //if (existing != null)
                //{
                //    existing.Name = EditGroup.Name;
                //    existing.Description = EditGroup.Description;
                //    existing.Icon = EditGroup.Icon;
                //    existing.IconBoxColor = EditGroup.IconBoxColor;
                //}

                //AppEvents.Groups.ChangeGroupUpdated(existing);


            }

            else
                MessageBox.Show("لم يتم تعديل المجموعة", "Erorr!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }


        private bool CheckTheFields()
        {
            if (string.IsNullOrEmpty(txbGroupName.Text))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(اسم المجموعة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbGroupName.Focus();
                return false;

            }
            if (txbGroupName.Text.Length > 100)
            {
                MessageBox.Show("اسم المجموعة طويل جدًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbGroupName.Focus();
                return false;
            }

            if (txbGroupDescription.Text.Length > 200)
            {
                MessageBox.Show("الوصف طويل جدًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbGroupDescription.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(selectedPathIcon))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(لا توجد ايقونة محددة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUplaodFileIcon.Focus();
                return false;

            }
            if (string.IsNullOrEmpty(selectedBGColor))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(لم تضف لون لخلفية الايقونة المختارة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                adColorPicker.Focus();
                return false;
            }


            return true;
        }

    }
}
