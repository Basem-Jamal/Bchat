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
            TestTabelData();

        }

        private void TestTabelData()
        {
            var table = new ProTable
            {
                Dock = DockStyle.Fill,
                TableStyle = ProTableStyle.Card,
                HeaderStyle = ProHeaderStyle.Gradient,
                SelectionMode = ProSelectionMode.Multi,
                IsRtl = true,
                BorderRadius = 14,
                ShadowDepth = 10,
                ShowSearch = true,
                ShowRowNumber = true,
                PaginationStyle = ProPaginationStyle.Bottom,
                PageSize = 20,
                AnimationSpeed = ProAnimationSpeed.Normal,
                AccentColor = Color.FromArgb(41, 128, 185),
            };

            table.SetColumns(new List<ProColumn>
            {
                new() { Header = "العميل",   Field = "Name",     Width = 200, CellType = ProCellType.AvatarText },
                new() { Header = "الحالة",   Field = "Status",   Width = 120, CellType = ProCellType.Badge },
                new() { Header = "التقييم",  Field = "Rating",   Width = 130, CellType = ProCellType.Rating },
                new() { Header = "الإنجاز",  Field = "Progress", Width = 140, CellType = ProCellType.Progress },
                new() { Header = "المبلغ",   Field = "Amount",   Width = 130, CellType = ProCellType.Currency,
                        Prefix = "ر.س ", Format = "N2" },
                new() { Header = "نشط",      Field = "IsActive", Width = 80,  CellType = ProCellType.Boolean },
                new() { Header = "إجراءات",  Field = "Actions",  Width = 140, CellType = ProCellType.Actions, Sortable = false },
            });


                
            table.SetActions(new List<ProAction>
            {
                new() { Key = "view",   Icon = Properties.Resources.Show1,  Color = Color.FromArgb(41,128,185), Tooltip = "عرض" },
                new() { Key = "edit",   Icon = Properties.Resources.edit,   Color = Color.FromArgb(230,126,34),  Tooltip = "تعديل" },
                new() { Key = "delete", Icon = Properties.Resources.trash,  Color = Color.FromArgb(192,57,43),   Tooltip = "حذف" },
            });

            table.ActionClicked += (s, e) =>
            {
                if (e.ActionKey == "delete") { /* حذف */ }
                else if (e.ActionKey == "edit") { /* تعديل */ }
            };

            // أضف هذا السطر بعد SetActions وقبل ActionClicked
            table.SetData(GenerateFakeData());

            customPanel3.Controls.Add(table);     
        }
        private List<Dictionary<string, object>> GenerateFakeData()
        {
            var names = new[]
            {
        "محمد العمري", "سارة الأحمدي", "خالد المطيري", "نورة السبيعي",
        "عبدالله الحربي", "ريم الزهراني", "فهد الدوسري", "هند القحطاني",
        "يوسف العتيبي", "لطيفة الشهري", "عمر الغامدي", "منى البقمي",
        "أحمد الرشيدي", "دلال الجهني", "سعود العنزي", "أمل الحازمي",
        "تركي الصاعدي", "وفاء الشمري", "بندر الرويلي", "غادة المالكي",
        "ناصر الحربي", "رهف الأسمري", "ماجد الثبيتي", "شيماء البلوي",
        "وليد القرني", "هيا العمير", "زياد الدوسري", "نجود الشريف",
        "حمد الرشيدي", "سمر الغامدي"
    };

            var statuses = new[] { "نشط", "معلق", "مؤجرة", "صيانة", "ملغي", "متاح", "مؤكد" };
            var rnd = new Random(42);

            var data = new List<Dictionary<string, object>>();
            for (int i = 0; i < 30; i++)
            {
                data.Add(new Dictionary<string, object>
                {
                    ["Name"] = names[i % names.Length],
                    ["Status"] = statuses[rnd.Next(statuses.Length)],
                    ["Rating"] = Math.Round(rnd.NextDouble() * 4 + 1, 1),   // 1.0 – 5.0
                    ["Progress"] = rnd.Next(5, 101),                           // 5% – 100%
                    ["Amount"] = Math.Round(rnd.NextDouble() * 49000 + 1000, 2), // 1000 – 50000
                    ["IsActive"] = rnd.Next(2) == 1 ? "true" : "false",
                });
            }
            return data;
        }
        public void LoadCustomer(int Id)
        {
            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == Id);
            if (customer == null) return;

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
    }
}
