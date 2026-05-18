using BChat.Custom_Controal.Custom_Bchat;
using BChat.Data.DataStore.CustomerProfile_Repository;
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
        public ucCustomerInfo()
        {
            InitializeComponent();

            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            this.DoubleBuffered = true;

            //Settings
            SettingLocationControls();
        }

        public void LoadCustomer(int Id)
        {
            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == Id);
            if (customer == null) return;

            //Profile Info
            Profile(customer);

            //Order Info
            OrderCount(customer);
            CancelledOrders(customer);
            TotalSpent(customer);

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
            var order = AppCache.GetCustomerProfilesFromCache(customer.Id);

            btnOrderCount.Text = order?.OrderCount.ToString() ?? "0";
        }

        private void CancelledOrders(Customer customer)
        {
            var order = CustomerProfileRepository.GetByCusotmerId(customer.Id);
            btnCancelledOrders.Text = order?.CancelledOrders.ToString() ?? "0";
        }

        private void TotalSpent(Customer customer)
        {
            var order = AppCache.GetCustomerProfilesFromCache(customer.Id);
            btnTotalSpent.Text = order?.TotalSpent.ToString() ?? "0";

        }
        private void modernPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void avatarControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
