using BChat.Controls;
using BChat.Events;
using BChat.Menu___Nav.Nav___Marketing.Settings.ApiSettings;
using BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings;
using BChat.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace BChat.Menu___Nav.Nav___Marketing
{
    public partial class Marketing : Form
    {

        public Marketing()
        {
            InitializeComponent();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      


        private void btnFormMinimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnNavCampaignsTabel_Click(object sender, EventArgs e)
        {
            ResetButtons();

            btnNavCampaignsTabel.IsActive = true;

            foreach (Control c in pnlSubContent.Controls)
                c.Visible = false;

            CampaignsView();
        }
        private void Marketing_Load(object sender, EventArgs e)
        {

        }

        private void btnNavTemplates_Click(object sender, EventArgs e)
        {
            ResetButtons();

            btnNavTemplates.IsActive = true;

            foreach (Control c in pnlSubContent.Controls)
                c.Visible = false;


            TemplatesView();
        }


        private void CampaignsView()
        {
            if (!pnlSubContent.Controls.ContainsKey("CampaignsTabel_View"))
            {
                ucCampaignsControl ucCampaignsControl = new ucCampaignsControl();
                ucCampaignsControl.Name = "CampaignsTabel_View";
                ucCampaignsControl.Dock = DockStyle.Fill;
                pnlSubContent.Controls.Add(ucCampaignsControl);
            }

            pnlSubContent.Controls["CampaignsTabel_View"].Visible = true;
            pnlSubContent.Controls["CampaignsTabel_View"].BringToFront();

        }

        private void TemplatesView()
        {
            if (!pnlSubContent.Controls.ContainsKey("Templates_View"))
            {
                ucTemplatesControl templatesPage = new ucTemplatesControl();
                templatesPage.Name = "Templates_View";
                templatesPage.Dock = DockStyle.Fill;
                pnlSubContent.Controls.Add(templatesPage);

            }

            pnlSubContent.Controls["Templates_View"].Visible = true;
            pnlSubContent.Controls["Templates_View"].BringToFront();

        }

        private void ResetButtons()
        {
            foreach (Control ctrl in pnlMenuSidebar.Controls)
            {
                if (ctrl is BChat.Controls.ModernNavButton btn)
                {

                    btn.BaseBackground = Color.Transparent;
                    btn.NormalTextColor = Color.Gray;
                    btn.IsActive = false;
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            var overlay = OverlayPanel.Show(mainForm);

            UserSettings userSettings = new UserSettings();
            userSettings.ShowDialog();

            overlay.Close(userSettings);

        }

        private void btnNavAPIs_Click(object sender, EventArgs e)
        {
            ResetButtons();

            btnNavAPIs.IsActive = true;

            var mainForm = this.FindForm();
            var overlay = OverlayPanel.Show(mainForm);

            ApiSettings apiSettings = new ApiSettings();
            apiSettings.ShowDialog();

            overlay.Close(apiSettings);

        }

        private void btnNavHome_Click(object sender, EventArgs e)
        {

        }


    }
}
