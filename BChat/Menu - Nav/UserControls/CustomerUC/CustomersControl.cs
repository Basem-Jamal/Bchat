using BChat.Controls;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Events;
using BChat.Forms;
using BChat.Global;
using BChat.Menu___Nav.UserControls.CustomerUC.Customer_Info_UC;
using BChat.Models;
using BChat.Services;

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
            //AppCache.Customers = CustomerRepository.GetAll();
            var customers = AppCache.Customers;

            stcdCoustomers.Value = customers.Count.ToString("N0")+"K";

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
            var parent = this.Parent;
            var row = _table.GetSelectedRow();
            if (row == null) return;

            int Id = Convert.ToInt32(row["Id"]);


            pnlContent.Controls.Clear();

            if (!parent.Controls.ContainsKey("CustomerInfo_View"))
            {
                ucCustomerInfo ucCustomerInfo = new ucCustomerInfo();
                ucCustomerInfo.Name = "CustomerInfo_View";
                ucCustomerInfo.Dock = DockStyle.Fill;
                parent.Controls.Add(ucCustomerInfo);
            }

            parent.Controls["CustomerInfo_View"].Visible = true;
            parent.Controls["CustomerInfo_View"].BringToFront();

        }

        private void Table_EditClicked(object sender, int rowIndex)
        {
            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            var row = _table.GetSelectedRow();
            if (row == null) return;

            //Customer customer = new Customer()
            //{
            //    Id = Convert.ToInt32(row["Id"]),
            //    Name = row["Name"].ToString(),
            //    Phone = row["Phone"].ToString(),
            //    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            //};

            int id = Convert.ToInt32(row["Id"]);
            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return;


            AddCustomerForm updateCustomer = new AddCustomerForm(customer, CustomerStatus.Update);
            updateCustomer.ShowDialog();

            overlay.Close(mainForm);

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
                    var oldGroupIds = AppCache.GetGroupIdsByCustomer(id);

                    AppCache.GroupMembers.RemoveAll(m => m.CustomerId == id);
                    AppCache.Customers.RemoveAll(c => c.Id == id);
                    AppCache.ChatMessages.RemoveAll(c => c.Id == id);
                    AppEvents.NotifyCustomerDeleted(id);

                    foreach (var groupId in oldGroupIds)
                    {
                        var group = AppCache.Groups.FirstOrDefault(g => g.Id == groupId);

                        if (group != null)
                        {
                            int count = AppCache.GroupMembers.Count(m => m.GroupId == groupId);
                            group.StatOneValue = count.ToString();

                            AppEvents.AppGroups.ChangeGroupUpdated(group);

                        }
                    }

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

        private void btnRefreshData_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private async void btnImportExcel_Click(object sender, EventArgs e)
        {
            
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "اختر ملف Excel";
                dialog.Filter = "Excel Files|*.xlsx;*.xls";

                if (dialog.ShowDialog() != DialogResult.OK) return;


                progressBar1.Visible = true;
                btnImportExcel.Enabled = false;

                var progress = new Progress<(int current, int total)>(p =>
                {
                    progressBar1.Maximum = p.total;
                    progressBar1.Value = p.current;
                });


                var (added, skipped) = await Task.Run(() =>
                    ExcelImportService.ImportCustomersAsync(dialog.FileName, progress));

                progressBar1.Visible = false;
                btnImportExcel.Enabled = true;


                MessageBox.Show(
                             $"✅ تم إضافة {added} عميل\n⚠️ تم تخطّي {skipped} صف",
                             "نتيجة الاستيراد",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information);

                LoadCustomers();

            }
        }
    }
}