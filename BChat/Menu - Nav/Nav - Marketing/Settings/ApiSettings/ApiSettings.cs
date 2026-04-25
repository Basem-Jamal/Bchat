using BChat.Data.DataStore.Apis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Menu___Nav.Nav___Marketing.Settings.ApiSettings
{
    public partial class ApiSettings : Form
    {
        public ApiSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            txtPhoneNumberId.Text = ApiSettingsRepository.GetValue("WhatsApp", "PhoneNumberId");
            txtBusinessAccountId.Text = ApiSettingsRepository.GetValue("WhatsApp", "BusinessAccountId");
            txtAccessToken.Text = ApiSettingsRepository.GetValue("WhatsApp", "AccessToken");
            txtWebhookVerifyToken.Text = ApiSettingsRepository.GetValue("WhatsApp", "WebhookVerifyToken");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ApiSettingsRepository.UpdateValue("WhatsApp", "PhoneNumberId", txtPhoneNumberId.Text.Trim());
            ApiSettingsRepository.UpdateValue("WhatsApp", "BusinessAccountId", txtBusinessAccountId.Text.Trim());
            ApiSettingsRepository.UpdateValue("WhatsApp", "AccessToken", txtAccessToken.Text.Trim());
            ApiSettingsRepository.UpdateValue("WhatsApp", "WebhookVerifyToken", txtWebhookVerifyToken.Text.Trim());

            MessageBox.Show("✅ تم الحفظ بنجاح", "BChat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void picClose_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
