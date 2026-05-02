using BChat.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.Menu___Nav.UserControls.Today_s_Summary_Report_UC
{


    public partial class ucMonthlySummary : UserControl
    {
        private OverlayPanel? _overlay;

        public ucMonthlySummary()
        {
            InitializeComponent();
            ShowOverlay();
        }

        // ── لما تريد تفتح الـ Overlay ──
        private void ShowOverlay()
        {
            _overlay = OverlayPanel.Show(this);
        }

        // ── لما تريد تغلقه ──
        private void HideOverlay()
        {
            _overlay?.Close();
            _overlay = null;
        }

    }
}
