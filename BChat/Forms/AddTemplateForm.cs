using BChat.Controls;
using BChat.Data.DataStore;
using BChat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Forms
{
    public partial class AddTemplateForm : Form
    {
        private Template _template;
        private TemplateStatus _status;
        public AddTemplateForm(Template template, TemplateStatus status)
        {
            InitializeComponent();

            cmbCategory.AddItems(new[] { "المبيعات", "الدعم", "العروض" });
            cmbCategory.SelectedIndex = 0;
            cmbCategory.LabelText = "اختر نوع القالب";
            _template = template;
            _status = status;

            if (status == TemplateStatus.Update)
            {
                txbTemplateName.Text = template.Name;
                rtbxContent.SetContent( template.Content);
                cmbCategory.SelectedValue = template.Category;
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txbCustomerPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام وزر الحذف (Backspace)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // يمنع الحرف من الظهور
            }
        }

        private void btnAddTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTemplateName.Text) || string.IsNullOrWhiteSpace(rtbxContent.PlainText))
            {
                MessageBox.Show("يجب اضافة اسم القالب ومحتوى القالب مع التأكد من عدم وجود مساحات فاضية في الحقول!", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

       
            if (_template != null && _status == TemplateStatus.Add)
            {
                Template template = new Template()
                {
                    Name = txbTemplateName.Text,
                    Content = rtbxContent.PlainText,
                    Category = cmbCategory.SelectedItem.ToString(),
                    CreatedAt = DateTime.Now,
                };
                TemplateRepository.Add(template);


                MessageBox.Show("تم اضافة القالب بنجاح!", "النظام", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }


            else if (_template != null && _status == TemplateStatus.Update)
            {
                _template.Name     = txbTemplateName.Text;
                _template.Content  = rtbxContent.Text;
                _template.Category = cmbCategory.SelectedItem.ToString();

                TemplateRepository.Update(_template);

                MessageBox.Show("تم تحديث القالب بنجاح!", "النظام", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            else
            {
                MessageBox.Show("قيمة القالب تكون Null!", "النظام", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            this.Close();


        }

    }
}
