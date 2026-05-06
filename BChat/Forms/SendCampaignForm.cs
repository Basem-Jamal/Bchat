using BChat.Data.DataStore;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Global;
using BChat.Models;
using BChat.WhatsApp;
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

            var templates = AppCache.WhatsAppTemplates;
            cmbTemplate.ClearItems();

            foreach (var template in templates)
            {
                cmbTemplate.AddItem(template.Name);
            }
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
            // ── 1. تحقق من اختيار قالب ─────────────────────────
            if (cmbTemplate.SelectedIndex < 0)
            {
                MessageBox.Show("يجب اختيار قالب!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── 2. جيب القالب ─────────────────────────
            var template = AppCache.WhatsAppTemplates[cmbTemplate.SelectedIndex];

            // ── 3. طلب رابط الوسائط فقط إذا ما في Media ID ───
            string mediaUrl = "";
            if ((template.HeaderType == "VIDEO" || template.HeaderType == "IMAGE")
                && string.IsNullOrEmpty(template.MediaId))
            {
                string mediaLabel = template.HeaderType == "VIDEO" ? "الفيديو" : "الصورة";
                mediaUrl = Microsoft.VisualBasic.Interaction.InputBox(
                    $"هذا القالب يحتوي على {mediaLabel}\nأدخل رابط {mediaLabel} (رابط مباشر):",
                    $"رابط {mediaLabel}"
                );

                if (string.IsNullOrEmpty(mediaUrl))
                {
                    MessageBox.Show("تم الإلغاء — لم يتم إدخال رابط.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // ── 4. تأكيد الإرسال ───────────────────────────────
            var confirm = MessageBox.Show(
                $"سيتم إرسال القالب [{template.Name}]\nهل أنت متأكد؟",
                "تأكيد الإرسال",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            // ── 5. إرسال الرسائل ────────────────────────────────
            int success = 0;
            int failed = 0;

            btnSendCampaign.Enabled = false;
            btnSendCampaign.Text = "جاري الإرسال...";

            bool sent = await MetaSender.SendTemplateAsync(
                "+966534926949",
                template.Name,
                template.Language ?? "ar",
                template.HeaderType,
                template.MediaId ?? "",
                mediaUrl
            );

            if (sent) success++;
            else failed++;

            // ── 6. النتيجة ─────────────────────────────────────
            MessageBox.Show(
                $"✅ تم الإرسال: {success}\n❌ فشل: {failed}",
                "نتيجة الحملة",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            btnSendCampaign.Enabled = true;
            btnSendCampaign.Text = "إرسال الحملة";
        }
    }
}