using BChat.Data.DataStore;
using BChat.Forms;
using BChat.Models;

namespace BChat.UserControls
{
    public partial class CustomersControl : UserControl
    {
        private CustomerRepository _customerRepo = new CustomerRepository();
        private SlickTable _table;

        public CustomersControl()
        {
            InitializeComponent();
            InitTable();
            LoadCustomers();
            _table.IsRtl = true;

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

            pnlContent.Controls.Add(_table);
        }
        private void LoadCustomers()
        {
            var customers = _customerRepo.GetAll();
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
                bool deleted = _customerRepo.Delete(id);

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
            AddCustomerForm addCustomerForm = new AddCustomerForm();
            addCustomerForm.ShowDialog();
        }
    }
}