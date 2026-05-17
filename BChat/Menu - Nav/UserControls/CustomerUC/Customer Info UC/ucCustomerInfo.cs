using BChat.Custom_Controal.Custom_Bchat;
using BChat.Global;
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


            lblCurrentCustomerName.Text = customer.Name;
            btnCurrentPhone.Text = customer.Phone;
            //avatarControl1.AvatarImage = Properties.Resources.users;
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


        }

        private void modernPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
