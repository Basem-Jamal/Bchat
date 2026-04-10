using BChat.Controls;
using BChat.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.UserControls
{
    public partial class CustomerGroupsControl : UserControl
    {
        public CustomerGroupsControl()
        {
            InitializeComponent();
        }

        private void crdAddNewGroup_Click(object sender, EventArgs e)
        {

            var mainForm = this.FindForm();

            var overlay = OverlayPanel.Show(mainForm);

            fmAddGroup fmAddGroup = new fmAddGroup();
            fmAddGroup.ShowDialog();

            overlay.Close(mainForm);
        }
    }
}
