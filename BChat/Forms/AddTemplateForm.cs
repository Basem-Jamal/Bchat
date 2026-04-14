using BChat.Controls;
using BChat.Data.DataStore;
using BChat.Models;
using BChat.Services;
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
    public partial class AddTemplateForm : Form
    {
        private Template _template;
        private TemplateStatus _status;
        public AddTemplateForm(Template template, TemplateStatus status)
        {
            InitializeComponent();

            _template = template;
            _status = status;

            // ── 1. عبّي الـ ComboBoxes بقيم Meta الصحيحة ─────
            cmbCategory.ClearItems();
            cmbCategory.AddItems(new[] { "MARKETING", "UTILITY", "AUTHENTICATION" });
            cmbCategory.SelectedIndex = 0;

            cmbLanguage.ClearItems();
            cmbLanguage.AddItems(new[] { "ar", "en" });
            cmbLanguage.SelectedIndex = 0;

            cmbHeaderType.ClearItems();
            cmbHeaderType.AddItems(new[] { "بدون هيدر", "نص", "صورة" });
            cmbHeaderType.SelectedIndex = 0;

            // ── 2. إخفاء حقول الهيدر افتراضياً ───────────────
            txbHeaderText.Visible = false;
            txbImageUrl.Visible = false;

            // ── 3. ربط سلوك تغيير نوع الهيدر ─────────────────
            cmbHeaderType.SelectedIndexChanged += CmbHeaderType_SelectedIndexChanged;

            // ── 4. Placeholders واضحة ────────────────────────
            txbTemplateName.PlaceholderText = "eid_offer (إنجليزي فقط)";
            //txbHeaderText.PlaceholderText = "النص اللي يظهر في أعلى الرسالة";
            txbImageUrl.PlaceholderText = "رابط الصورة (اختياري عند الإنشاء)";
            txbQuickReply.PlaceholderText = "مثال: نعم، أريد العرض";
            txbUrlText.PlaceholderText = "مثال: تسوق الآن";
            txbUrlLink.PlaceholderText = "https://oudorchid.com";
            txbPhoneText.PlaceholderText = "مثال: اتصل بنا";
            txbPhoneNumber.PlaceholderText = "+966501234567";

            // ── 5. وضع التحديث ───────────────────────────────
            if (status == TemplateStatus.Update && template != null)
            {
                txbTemplateName.Text = template.Name;
                rtbxContent.SetContent(template.Content);

                if (!string.IsNullOrEmpty(template.Category))
                    cmbCategory.SelectedItem = template.Category;

                if (!string.IsNullOrEmpty(template.Language))
                    cmbLanguage.SelectedItem = template.Language;

                // اعرض الهيدر إذا موجود
                if (template.HeaderType == "TEXT")
                {
                    cmbHeaderType.SelectedItem = "نص";
                    pnlContent.Text = template.HeaderText ?? "";
                }
                else if (template.HeaderType == "IMAGE")
                {
                    cmbHeaderType.SelectedItem = "صورة";
                }
            }
        }

        private void CmbHeaderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbHeaderType.SelectedItem?.ToString();

            pnlContent.Visible = selected == "نص";
            txbImageUrl.Visible = selected == "صورة";
        }
        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txbCustomerPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام وزر الحذف (Backspace)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // يمنع الحرف من الظهور
            }
        }

        private async void btnAddTemplate_Click(object sender, EventArgs e)
        {
            // ── 1. تحقق من الحقول الأساسية ─────────────────────
            if (string.IsNullOrWhiteSpace(txbTemplateName.Text) ||
                string.IsNullOrWhiteSpace(rtbxContent.PlainText))
            {
                MessageBox.Show("يجب إدخال اسم القالب والمحتوى!", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── 2. بناء الأزرار ─────────────────────────────────
            var buttons = new List<object>();

            // Quick Reply
            if (!string.IsNullOrWhiteSpace(txbQuickReply.Text))
                buttons.Add(new { type = "QUICK_REPLY", text = txbQuickReply.Text.Trim() });

            // URL
            if (!string.IsNullOrWhiteSpace(txbUrlText.Text) &&
                !string.IsNullOrWhiteSpace(txbUrlLink.Text))
                buttons.Add(new { type = "URL", text = txbUrlText.Text.Trim(), url = txbUrlLink.Text.Trim() });

            // Phone
            if (!string.IsNullOrWhiteSpace(txbPhoneText.Text) &&
                !string.IsNullOrWhiteSpace(txbPhoneNumber.Text))
                buttons.Add(new { type = "PHONE_NUMBER", text = txbPhoneText.Text.Trim(), phone_number = txbPhoneNumber.Text.Trim() });

            // ── 3. بناء Components ──────────────────────────────
            var components = new List<object>();

            // Header
            if (cmbHeaderType.SelectedItem?.ToString() == "نص" &&
                !string.IsNullOrWhiteSpace(pnlContent.Text))
            {
                components.Add(new { type = "HEADER", format = "TEXT", text = pnlContent.Text.Trim() });
            }
            else if (cmbHeaderType.SelectedItem?.ToString() == "صورة")
            {
                components.Add(new { type = "HEADER", format = "IMAGE" });
            }

            // Body
            components.Add(new { type = "BODY", text = rtbxContent.PlainText.Trim() });

            // Buttons
            if (buttons.Count > 0)
                components.Add(new { type = "BUTTONS", buttons });

            // ── 4. بناء الـ Payload ─────────────────────────────
            var payload = new
            {
                name = txbTemplateName.Text.Trim().ToLower().Replace(" ", "_"),
                language = cmbLanguage.SelectedItem?.ToString() ?? "ar",
                category = cmbCategory.SelectedItem?.ToString() ?? "MARKETING",
                components
            };

            // ── 5. إرسال لـ Meta ────────────────────────────────
            var service = new WhatsAppService();
            bool success = await service.CreateTemplateAsync(payload);

            if (success)
            {
                MessageBox.Show("✅ تم إرسال القالب لـ Meta وينتظر الموافقة!",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ فشل إرسال القالب — تحقق من البيانات!",
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
