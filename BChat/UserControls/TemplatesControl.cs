using BChat.Controls;
using BChat.Data.DataStore;
using BChat.Events;
using BChat.Forms;
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

namespace BChat.UserControls
{
    public partial class TemplatesControl : UserControl
    {
        private SlickTable _table;

        public TemplatesControl()
        {
            InitializeComponent();
            InitTable();
            LoadTemplates();

            _table.IsRtl = true;
            _table.BorderRadius = 10;
            _table.ShadowDepth = 0;

            AppEvents.OnRefreshTemplatesTable += LoadTemplates;


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
                new GridColumn { Header = "اسم القالب",          Field = "Name",      Width = 200, CellType = GridCellType.Avatar },
                new GridColumn { Header = "المحتوى",    Field = "Content",     Width = 150 },
                new GridColumn { Header = "التصنيف",       Field = "Category", Width = 150 },
                new GridColumn { Header = "إجراءات",        Field = "Actions",   Width = 130, CellType = GridCellType.Actions },
            });

            _table.DeleteClicked += Table_DeleteClicked;
            _table.ViewClicked += Table_ViewClicked;
            _table.EditClicked += Table_EditClicked;
            pnlContent.Controls.Add(_table);
        }

        private void LoadTemplates()
        {
            var templates = TemplateRepository.GetAll();

            stcdTemplates.Value = templates.Count.ToString();

            var rows = new List<Dictionary<string, object>>();

            foreach (var t in templates)
            {
                rows.Add(new Dictionary<string, object>
                {
                    { "Id",        t.Id },
                    { "Name",      t.Name },
                    { "Content",    t.Content },
                    { "Category",     t.Category },
                    { "CreatedAt", t.CreatedAt.ToString("yyyy/MM/dd") }
                });
            }

            _table.SetData(rows);
        }


        private void Table_ViewClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            MessageBox.Show(
                $"اسم القالب: {row["Name"]}\nالمحتوى: {row["Content"]}",
                "تفاصيل العميل",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Table_EditClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            Template template = new Template()
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Content = row["Content"].ToString(),
                Category = row["Category"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
            AddTemplateForm updateTemplate = new AddTemplateForm(template, TemplateStatus.Update);
            updateTemplate.ShowDialog();
        }
        private void Table_DeleteClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            var confirm = MessageBox.Show(
                $"هل تريد حذف قالب: {row["Name"]}؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(row["Id"]);
                bool deleted = TemplateRepository.Delete(id);

                if (deleted)
                {
                    MessageBox.Show("تم الحذف بنجاح ✅", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTemplates();
                }
                else
                {
                    MessageBox.Show("فشل الحذف ❌", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddTemplate_Click(object sender, EventArgs e)
        {


            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            Template newTemplate = new Template();
            AddTemplateForm addTemplateForm = new AddTemplateForm(newTemplate, TemplateStatus.Add);
            addTemplateForm.ShowDialog();

            overlay.Close(mainForm);

        }
    }
}