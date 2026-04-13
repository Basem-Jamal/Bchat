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
    public partial class AddCustomerForm : Form
    {
        private Customer _customer;
        private CustomerStatus _status;
        public AddCustomerForm(Customer customer , CustomerStatus status)
        {
            InitializeComponent();

            txbCustomerPhone.MaxLength = 15; // تحديد عدد الأرقام

            _customer = customer;
            _status = status;

            if (status == CustomerStatus.Update)
            {
                txbCustomerName.Text = customer.Name;
                txbCustomerPhone.Text = customer.Phone;
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

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbCustomerName.Text) || string.IsNullOrWhiteSpace(txbCustomerPhone.Text))
            {
                MessageBox.Show("يجب اضافة اسم ورقم العميل مع التأكد من عدم وجود مساحات فاضية في الحقول!","Erorr",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!long.TryParse(txbCustomerPhone.Text, out long phone))
            {
                MessageBox.Show("رقم الهاتف يجب أن يحتوي على أرقام فقط!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_customer != null && _status == CustomerStatus.Add)
            {
                Customer customer = new Customer()
                {
                    Name = txbCustomerName.Text,
                    Phone = phone.ToString(),
                };
                bool added = CustomerRepository.Add(customer);

                if (added)
                {
                    MessageBox.Show("تمت الإضافة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("رقم الجوال مسجل مسبقاً", "تكرار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            

            else if (_customer != null && _status == CustomerStatus.Update)
            {
                _customer.Name = txbCustomerName.Text;
                _customer.Phone = txbCustomerPhone.Text;

                CustomerRepository.Update(_customer);

                MessageBox.Show("تم اضافة العميل بنجاح!", "النظام", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }

            else
            {
                MessageBox.Show("قيمة العميل تكون Null!", "النظام", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            this.Close();

        }
    }
}
