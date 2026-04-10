using BChat.Data.DataStore;
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

        private string selectedPathIcon = null;
        private string selectedBGColor  = null;
        public fmAddGroup()
        {
            InitializeComponent();
        }

        private void fmAddGroup_Load(object sender, EventArgs e)
        {
            lblFileTextIcon.Visible = false;
            lblSelectedBG.Visible = false;



            adColorPicker.ColorChanged += handelColor;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            lblSelectedBG.Text = adColorPicker.SelectedColor.ToString();
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

        private void btnAddCustomerGroups_Click(object sender, EventArgs e)
        {
            CheckTheFields();
            CustomerGroups newGroup = new CustomerGroups()
            {
                Name = txbGroupName.Text,
                Description = txbGroupDescription.Text,
                Icon        = selectedPathIcon,
                IconBoxColor= selectedBGColor,
            };
            AddGroup(newGroup);

        }

        private void AddGroup(CustomerGroups newGroup)
        {
            bool result = false;

            if (newGroup == null)
            {
                MessageBox.Show("يبدو انه لاتوجد قيمة في الحقول!", "Erorr System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            result = CustomerGroupRepository.Add(newGroup);

            if (result)
                MessageBox.Show("تم اضافة المجموعة بنجاح", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("لم تتم اضافة المجموعة", "Erorr!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void CheckTheFields()
        {
            if (string.IsNullOrEmpty(txbGroupName.Text))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(اسم المجموعة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            else if (string.IsNullOrEmpty(selectedPathIcon))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(لا توجد ايقونة محددة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            else if (string.IsNullOrEmpty(selectedBGColor))
            {
                MessageBox.Show($"يبدو انه لاتوجد قيمة في الحقل! - {"(لم تضف لون لخلفية الايقونة المختارة!)"}", "Oups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }     
            
        }
    }
}
