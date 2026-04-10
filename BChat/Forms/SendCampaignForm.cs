using BChat.Data.DataStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Forms
{
    public partial class SendCampaignForm : Form
    {
        public SendCampaignForm()
        {
            InitializeComponent();
        }

        private void SendCampaignForm_Load(object sender, EventArgs e)
        {
            var customers = CustomerRepository.GetAll();
            segmented.UpdateSubtitle(0, $"{customers.Count} عميل");

            var templates = TemplateRepository.GetAll();
            cmbTemplate.ClearItems();
            cmbTemplate.AddItems(templates.Select(t => t.Name));
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void modernButton3_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private async void btnSendCampaign_Click(object sender, EventArgs e)
        {
            //// ── 1. تحقق من اختيار قالب ─────────────────────────
            //if (cmbTemplate.SelectedIndex < 0)
            //{
            //    MessageBox.Show("يجب اختيار قالب!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// ── 2. جيب القالب والعملاء ─────────────────────────
            //var templates = TemplateRepository.GetAll();
            //var template = templates[cmbTemplate.SelectedIndex];
            //var customers = CustomerRepository.GetAll();

            //if (customers.Count == 0)
            //{
            //    MessageBox.Show("لا يوجد عملاء!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// ── 3. تأكيد الإرسال ───────────────────────────────
            //var confirm = MessageBox.Show(
            //    $"سيتم إرسال الرسالة لـ {customers.Count} عميل\nهل أنت متأكد؟",
            //    "تأكيد الإرسال",
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question
            //);

            //if (confirm != DialogResult.Yes) return;

            //// ── 4. إرسال الرسائل ────────────────────────────────
            //var service = new BChat.Services.WhatsAppService();
            //int success = 0;
            //int failed = 0;

            //btnSendCampaign.Enabled = false;
            //btnSendCampaign.Text = "جاري الإرسال...";

            //foreach (var customer in customers)
            //{
            //    bool sent = await service.SendTextMessage(customer.Phone, template.Content);

            //    // سجّل الرسالة في الداتابيز
            //    MessageRepository.Add(new BChat.Models.ChatMessage
            //    {
            //        CustomerId = customer.Id,
            //        TemplateId = template.Id,
            //        Status = sent ? "sent" : "failed",
            //        TriggerType = "manual"
            //    });

            //    if (sent) success++;
            //    else failed++;
            //}

            //// ── 5. النتيجة ─────────────────────────────────────
            //MessageBox.Show(
            //    $"✅ تم الإرسال: {success}\n❌ فشل: {failed}",
            //    "نتيجة الحملة",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Information
            //);

            //btnSendCampaign.Enabled = true;
            //btnSendCampaign.Text = "إرسال الحملة";

            this.Close();

        }
    }
}
