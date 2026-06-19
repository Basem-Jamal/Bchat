using BChat.Controls;
using BChat.Custom_Controal.Custom_Bchat;
using BChat.Data.DataStore.CustomerProfile_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Events;
using BChat.Forms;
using BChat.Global;
using BChat.Models;
using BChat.Models.Customer_Module.CustomerProfiles_Module;
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
        private CustomerProfile _profile;
        public ucCustomerInfo(Customer customer = null)
        {

            InitializeComponent();

            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);



            _customer = customer;
            AppEvents.OnCustomerUpdated -= LoadCustomer;
            AppEvents.OnCustomerUpdated += LoadCustomer;



            this.DoubleBuffered = true;
            //Settings
            SettingLocationControls();
            SettingIcons();
        }

        private async void ucCustomerInfo_Load(object sender, EventArgs e)
        {
            if (_customer == null) return;

            pnlLoading.BringToFront();
            pnlLoading.Visible = true;


            await Task.Run(async () =>
            {
                await Task.Delay(1000);

                _profile = CustomerProfileRepository.GetByCusotmerId(_customer.Id);

            });

            LoadCustomer(_customer);

            pnlLoading.Visible = false;


        }
        public void LoadCustomer(Customer customer)
        {
            if (customer == null) return;

            _customer = customer; // ✅ حدّث الـ state

            if (_profile == null || _profile.CustomerId != customer.Id)
                _profile = CustomerProfileRepository.GetByCusotmerId(customer.Id);


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
            btnCurrentEmail.Text = _profile.Email;

            var GroupMembers = AppCache.GroupMembers.FirstOrDefault(gm => gm.CustomerId == customer.Id);


            var groupName = AppCache.Groups.FirstOrDefault(gr => gr.Id == GroupMembers?.GroupId);
            lblGroupCustomerName.Text = groupName?.Name ?? "لايوجد تصنيف";
        }
        private void OrderCount(Customer customer)
        {

            btnOrderCount.Text = _profile?.OrderCount != null ? _profile.OrderCount.ToString() : "0";
        }

        private void CancelledOrders(Customer customer)
        {
            btnCancelledOrders.Text = _profile?.CancelledOrders != null ? _profile.CancelledOrders.ToString() : "0";
        }

        private void TotalSpent(Customer customer)
        {
            btnTotalSpent.Text = _profile?.TotalSpent != null ? $"ريـال {_profile.TotalSpent:N2}" : "ريـال " + "0.00";

        }
        private void modernPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void avatarControl1_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            var mainForm = this.FindForm();
            var overlaye = OverlayPanel.Show(mainForm);


            AddCustomerForm customerForm = new AddCustomerForm(_customer, CustomerStatus.Update , _profile);
            customerForm.ShowDialog();

            overlaye.Close(customerForm);
        }

        private void btnBlock_Click(object sender, EventArgs e)
        {
            _customer.IsBlocked = !_customer.IsBlocked;
            CustomerRepository.Block(_customer);

            string msg = _customer.IsBlocked ? "تم حجب العميل ✅" : "تم رفع الحجب ✅";
            MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnBlock.Text = _customer.IsBlocked ? "رفع الحظر" : "حظر العميل";

        }

    
    }
}
