using BChat.Controls;
using BChat.Data.DataStore;
using BChat.Events;
using BChat.Forms;
using BChat.Models;

namespace BChat.UserControls
{
    public partial class CustomersControl : UserControl
    {
        private SlickTable _table;

        public CustomersControl()
        {
            InitializeComponent();
            InitTable();
            LoadCustomers();
            _table.IsRtl = true;
            _table.BorderRadius = 10;
            _table.ShadowDepth = 0;
            AppEvents.OnRefreshCustomersTable += LoadCustomers;

        }

        private void InitTable()
        {
            _table = new SlickTable
            {
                Dock = DockStyle.Fill,
                HeaderBackground = Color.FromArgb(22, 45, 90),
                RowOdd = Color.FromArgb(240, 247, 255),

                IconView = Properties.Resources.Show1,
                IconEdit = Properties.Resources.edit,
                IconDelete = Properties.Resources.trash,
            };

            _table.SetColumns(new List<GridColumn>
            {
                new GridColumn { Header = "الاسم",          Field = "Name",      Width = 200, CellType = GridCellType.Avatar },
                new GridColumn { Header = "رقم الهاتف",    Field = "Phone",     Width = 150 },
                new GridColumn { Header = "تاريخ الإضافة", Field = "CreatedAt", Width = 150 },
                new GridColumn { Header = "إجراءات",        Field = "Actions",   Width = 130, CellType = GridCellType.Actions },
            });

            _table.DeleteClicked += Table_DeleteClicked;
            _table.ViewClicked += Table_ViewClicked;
            _table.EditClicked += Table_EditClicked;
            pnlContent.Controls.Add(_table);
        }
        private void LoadCustomers()
        {
            var customers = CustomerRepository.GetAll();

            stcdCoustomers.Value = customers.Count.ToString();

            var rows = new List<Dictionary<string, object>>();

            foreach (var c in customers)
            {
                rows.Add(new Dictionary<string, object>
                {
                    { "Id",        c.Id },
                    { "Name",      c.Name },
                    { "Phone",     c.Phone },
                    { "CreatedAt", c.CreatedAt.ToString("yyyy/MM/dd") }
                });
            }

            _table.SetData(rows);
        }

        private void Table_ViewClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            MessageBox.Show(
                $"الاسم: {row["Name"]}\nالهاتف: {row["Phone"]}",
                "تفاصيل العميل",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Table_EditClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            Customer customer = new Customer()
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Phone = row["Phone"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
            AddCustomerForm updateCustomer = new AddCustomerForm(customer, CustomerStatus.Update);
            updateCustomer.ShowDialog();
        }
        private void Table_DeleteClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            var confirm = MessageBox.Show(
                $"هل تريد حذف العميل: {row["Name"]}؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(row["Id"]);
                bool deleted = CustomerRepository.Delete(id);

                if (deleted)
                {
                    MessageBox.Show("تم الحذف بنجاح ✅", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                }
                else
                {
                    MessageBox.Show("فشل الحذف ❌", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {

            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);


            Customer newCustomer = new Customer();
            AddCustomerForm addCustomerForm = new AddCustomerForm(newCustomer, CustomerStatus.Add);
            addCustomerForm.ShowDialog();

            overlay.Close(mainForm);

        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {

        }
    }
}