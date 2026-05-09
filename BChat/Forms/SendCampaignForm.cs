using BChat.Data.DataStore;
using BChat.Data.DataStore.Campaigns_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Global;
using BChat.Models;
using BChat.Models.Campaign_Module;
using BChat.Models.Campaign_Modules;
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

            // ── 4. جيب عملاء المجموعة ─────────────────────────
            int groupId = 5;
            var groupMemberIds = AppCache.GroupMembers
                .Where(m => m.GroupId == groupId)
                .Select(m => m.CustomerId)
                .ToList();

            // ← الإرسال الذكي: استثني من وصلتهم رسالة خلال 7 أيام
            var recentlySentIds = CampaignMessageRepository.GetRecentlySentCustomerIds();

            var customers = AppCache.Customers
                .Where(c => groupMemberIds.Contains(c.Id))
                .Where(c => !recentlySentIds.Contains(c.Id))
                .Take(3000)
                .ToList();

            if (customers.Count == 0)
            {
                MessageBox.Show("لا يوجد عملاء جدد في المجموعة!\nجميعهم وصلتهم رسالة خلال 7 أيام.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── 5. تأكيد الإرسال ───────────────────────────────
            var confirm = MessageBox.Show(
                $"سيتم إرسال القالب [{template.Name}] لـ {customers.Count} عميل\nهل أنت متأكد؟",
                "تأكيد الإرسال",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            // ── 6. أنشئ الحملة في DB ───────────────────────────
            var campaign = new Campaign
            {
                Name = txbCampaignName.Text.Trim(),
                GroupId = groupId,
                TemplateId = template.Id,
                SentAt = DateTime.Now,
                Status = CampaignStatus.Sending,
                TotalCount = customers.Count,
                SuccessCount = 0,
                FailedCount = 0,
            };
            campaign.Id = CampaignRepository.Add(campaign);

            // ── 7. إرسال الرسائل ────────────────────────────────
            int success = 0;
            int failed = 0;

            btnSendCampaign.Enabled = false;
            btnSendCampaign.Text = "جاري الإرسال...";

            foreach (var customer in customers)
            {
                bool sent = await MetaSender.SendTemplateAsync(
                    customer.Phone,
                    template.Name,
                    template.Language ?? "ar",
                    template.HeaderType,
                    template.MediaId ?? "",
                    mediaUrl
                );

                // ── حفظ في CampaignMessages ──────────────────
                var campaignMsg = new CampaignMessage
                {
                    CampaignId = campaign.Id,
                    CustomerId = customer.Id,
                    Status = sent ? CampaignMessageStatus.Completed : CampaignMessageStatus.Failed,
                    SentAt = DateTime.Now
                };
                CampaignMessageRepository.Add(campaignMsg);

                // ── حفظ في ChatMessages ───────────────────────
                if (sent)
                {
                    var chatMsg = new ChatMessage
                    {
                        CustomerId = customer.Id,
                        Text = template.BodyText ?? template.Name,
                        SentAt = DateTime.Now,
                        IsSent = true,
                        IsRead = false,
                        HasAttachment = false,
                        Status = "sent"
                    };
                    chatMsg.Id = ChatMessageRepository.Add(chatMsg);
                    AppCache.ChatMessages.Add(chatMsg);
                    success++;
                }
                else
                {
                    failed++;
                }

                // تأخير بين الرسائل لتجنب Rate Limiting
                await Task.Delay(300);
            }

            // ── 8. حدّث الحملة ────────────────────────────────
            campaign.SuccessCount = success;
            campaign.FailedCount = failed;
            campaign.Status = CampaignStatus.Completed;
            CampaignRepository.Update(campaign);

            // ── 9. النتيجة ─────────────────────────────────────
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