using BChat.Controls;
using BChat.Data.DataStore;
using BChat.Events;
using BChat.Forms;
using BChat.Models;

namespace BChat.UserControls
{
    public partial class MessagesControl : UserControl
    {
        private SlickTable _table;

        public MessagesControl()
        {
            InitializeComponent();
            InitTable();
            LoadMessages();
            _table.IsRtl = true;
            _table.BorderRadius = 10;
            _table.ShadowDepth = 0;

            AppEvents.OnRefreshMessagesTable += LoadMessages;
        }

        private void InitTable()
        {
            _table = new SlickTable
            {
                Dock = DockStyle.Fill,
                HeaderBackground = Color.FromArgb(22, 45, 90),
                RowOdd = Color.FromArgb(240, 247, 255),

                IconView = Properties.Resources.Show1,
                IconDelete = Properties.Resources.trash,
            };

            _table.SetColumns(new List<GridColumn>
            {
                new GridColumn { Header = "العميل",       Field = "CustomerName", Width = 200, CellType = GridCellType.Avatar },
                new GridColumn { Header = "القالب",       Field = "TemplateName", Width = 150 },
                new GridColumn { Header = "نوع الإرسال",   Field = "TriggerType",  Width = 130, CellType = GridCellType.Badge },
                new GridColumn { Header = "الحالة",       Field = "Status",       Width = 100, CellType = GridCellType.Badge },
                new GridColumn { Header = "التاريخ",      Field = "SentAt",       Width = 130 },
                new GridColumn { Header = "إجراءات",      Field = "Actions",      Width = 100, CellType = GridCellType.Actions },
            });

            _table.ViewClicked += Table_ViewClicked;
            _table.DeleteClicked += Table_DeleteClicked;

            pnlContent.Controls.Add(_table);
        }

        private void LoadMessages()
        {
            var messages = MessageRepository.GetAllWithDetails();

            stcdCountCampaign.Value = messages.Count.ToString();

            var rows = new List<Dictionary<string, object>>();

            foreach (var m in messages)
            {
                rows.Add(new Dictionary<string, object>
                {
                    { "Id",           m.Id },
                    { "CustomerName", m.CustomerName },
                    { "CustomerPhone",m.CustomerPhone },
                    { "TemplateName", m.TemplateName },
                    { "TriggerType",  m.TriggerType },
                    { "Status",       m.Status },
                    { "SentAt",       m.SentAt.ToString("yyyy/MM/dd HH:mm") }
                });
            }

            _table.SetData(rows);
        }

        private void Table_ViewClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            MessageBox.Show(
                $"العميل: {row["CustomerName"]}\n" +
                $"الهاتف: {row["CustomerPhone"]}\n" +
                $"القالب: {row["TemplateName"]}\n" +
                $"الحالة: {row["Status"]}\n" +
                $"التاريخ: {row["SentAt"]}",
                "تفاصيل الرسالة",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Table_DeleteClicked(object sender, int rowIndex)
        {
            var row = _table.GetSelectedRow();
            if (row == null) return;

            var confirm = MessageBox.Show(
                $"هل تريد حذف سجل هذه الرسالة؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(row["Id"]);
                bool deleted = MessageRepository.Delete(id);

                if (deleted)
                    LoadMessages();
            }
        }

        private void btnCreateACampaign_Click(object sender, EventArgs e)
        {    
            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            SendCampaignForm send = new SendCampaignForm();
            send.ShowDialog();

            overlay.Close(mainForm);

        }
    }
}