using BChat.Custom_Controal.Custom_Bchat;
using BChat.Data.DataStore.CustomerProfile_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Events;
using BChat.Forms;
using BChat.Global;
using BChat.Models;
using BChat.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BChat.Menu___Nav.UserControls.CustomerUC.Customer_Info_UC
{
    public partial class ucCustomerInfo : UserControl
    {
        private Customer _customer;
        public ucCustomerInfo()
        {
            InitializeComponent();

            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            AppEvents.OnCustomerUpdated += LoadCustomer;



            this.DoubleBuffered = true;
            //Settings
            SettingLocationControls();
            SettingIcons();
        }

        public void LoadCustomer(Customer customer)
        {


            _customer = customer;
            if (_customer == null) return;

            //Profile Info
            Profile(_customer);

            //Order Info
            OrderCount(_customer);
            CancelledOrders(_customer);
            TotalSpent(_customer);

        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            var parent = this.Parent;

            parent.Controls.Clear();

            CustomersControl customersControl = new CustomersControl();
            customersControl.Dock = DockStyle.Fill;

            parent.Controls.Add(customersControl);
        }


        private void SettingLocationControls()
        {
            lblCurrentCustomerName.AutoSize = false;

            lblCurrentCustomerName.Dock = DockStyle.Fill;

            lblCurrentCustomerName.TextAlign = ContentAlignment.MiddleCenter;


            lblGroupCustomerName.AutoSize = false;
            lblGroupCustomerName.Dock = DockStyle.Fill;
            lblGroupCustomerName.TextAlign = ContentAlignment.MiddleRight;


        }
        private void SettingIcons()
        {
            btnIconTotalSpent.IconChar = FontAwesome.Sharp.IconChar.MoneyBill;
            btnIconCancelledOrders.IconChar = FontAwesome.Sharp.IconChar.Ban;
            btnIconOrderCount.IconChar = FontAwesome.Sharp.IconChar.CartShopping;

            // btn footer
            btnEdit.IconChar = FontAwesome.Sharp.IconChar.UserEdit;
            btnBlock.IconChar = FontAwesome.Sharp.IconChar.Ban;

        }
        private void Profile(Customer customer)
        {
            avatarCustomer.FullName = customer.Name;

            lblCurrentCustomerName.Text = customer.Name;
            btnCurrentPhone.Text = customer.Phone;
            var GroupMembers = AppCache.GroupMembers.FirstOrDefault(gm => gm.CustomerId == customer.Id);


            var groupName = AppCache.Groups.FirstOrDefault(gr => gr.Id == GroupMembers?.GroupId);
            lblGroupCustomerName.Text = groupName?.Name ?? "لايوجد تصنيف";
        }
        private void OrderCount(Customer customer)
        {
            var order = CustomerProfileRepository.GetByCusotmerId(customer.Id);

            btnOrderCount.Text = order?.OrderCount != null ? order.OrderCount.ToString() : "0";
        }

        private void CancelledOrders(Customer customer)
        {
            var order = CustomerProfileRepository.GetByCusotmerId(customer.Id);
            btnCancelledOrders.Text = order?.CancelledOrders != null ? order.CancelledOrders.ToString() : "0";
        }

        private void TotalSpent(Customer customer)
        {
            var order = CustomerProfileRepository.GetByCusotmerId(customer.Id);
            btnTotalSpent.Text = order?.TotalSpent != null ? $"ريـال {order.TotalSpent:N2}" : "ريـال " + "0.00";

        }
        private void modernPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void avatarControl1_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddCustomerForm customerForm = new AddCustomerForm(_customer, CustomerStatus.Update);
            customerForm.ShowDialog();

        }

        private void btnBlock_Click(object sender, EventArgs e)
        {
            _customer.IsBlocked = !_customer.IsBlocked;
            CustomerRepository.Block(_customer);

            string msg = _customer.IsBlocked ? "تم حجب العميل ✅" : "تم رفع الحجب ✅";
            MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
