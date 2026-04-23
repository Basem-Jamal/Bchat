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

namespace BChat.Menu___Nav.UserControls.Show_Group_Members_Control
{
    public partial class GroupMembersControl : UserControl
    {
        private SlickTable _table;
        Customer _customer;
        public GroupMembersControl(int groupId)
        {
            InitializeComponent();
            InitTable();
            LoadCustomers(groupId);

            _table.IsRtl = true;
            _table.BorderRadius = 10;
            _table.ShadowDepth = 0;

        }

        private void InitTable()
        {
            _table = new SlickTable
            {
                Dock = DockStyle.Fill,
                HeaderBackground = Color.FromArgb(22, 45, 90),
                RowOdd = Color.FromArgb(240, 247, 255),

                //IconView = Properties.Resources.Show1,
                //IconEdit = Properties.Resources.edit,
                //IconDelete = Properties.Resources.trash,
            };

            _table.SetColumns(new List<GridColumn>
            {
                new GridColumn { Header = "الاسم",          Field = "Name",      Width = 200, CellType = GridCellType.Avatar },
                new GridColumn { Header = "رقم الهاتف",    Field = "Phone",     Width = 150 },
                new GridColumn { Header = "تاريخ الإضافة", Field = "CreatedAt", Width = 150 },
                //new GridColumn { Header = "إجراءات",        Field = "Actions",   Width = 130, CellType = GridCellType.Actions },
            });

            pnlContent.Controls.Add(_table);
        }
        private void LoadCustomers(int groupId)
        {
            List<Customer> listCustomer;
            var group = AppCache.Groups.FirstOrDefault(g=> g.Id == groupId);
            if (group != null)
            {
                btnGroupName.Text = group.Name;
            }


            var customerIds = AppCache.GroupMembers
                .Where(m => m.GroupId == groupId)
                .Select(m => m.CustomerId)
                .ToList();

            var customers = AppCache.Customers
                .Where(c=> customerIds.Contains(c.Id))
                .ToList();
            
            //var groupMembers = AppCache.GroupMembers.Where(g => g.GroupId == groupId).ToList();

            //foreach (var member in groupMembers)
            //{
            //    var c = AppCache.Customers.Where(c => c.Id == member.CustomerId);
            //}

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

        private void btnBack_Click(object sender, EventArgs e)
        {
            var parent = this.Parent;

            parent.Controls.Clear();

            var groupsControl = new GroupsControl();
            groupsControl.Dock = DockStyle.Fill;
            parent.Controls.Add(groupsControl);
        }
    }
}
