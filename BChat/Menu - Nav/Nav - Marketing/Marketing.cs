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

        private void btnNavCreateCampaign_Click(object sender, EventArgs e)
        {
            ResetButtons();

            btnNavCreateCampaign.IsActive = true;

        }
        private void btnNavTemplates_Click(object sender, EventArgs e)
        {
            ResetButtons();

            btnNavTemplates.IsActive = true;

            if (!pnlSubContent.Controls.ContainsKey("Templates_View"))
            {
                TemplatesControl templatesPage = new TemplatesControl();
                templatesPage.Name = "Templates_View";
                templatesPage.Dock = DockStyle.Fill;
                pnlSubContent.Controls.Add(templatesPage);
            }

            pnlSubContent.Controls["Templates_View"].BringToFront();

        }

        private void ResetButtons()
        {
            foreach (Control ctrl in pnlMenuSidebar.Controls)
            {
                if (ctrl is BChat.Controls.ModernNavButton btn)
                {

                    btn.BaseBackground = Color.FromName("ButtonFace");
                    btn.NormalTextColor = Color.Gray;
                    btn.IsActive = false;
                }
            }
        }

    }
}
