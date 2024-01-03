using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RouteOptimizer;
using RouteOptimizer.Entity;

namespace RouteOptimizer.PInfoForms
{
    public partial class Ior : System.Windows.Forms.Form
    {
       public DestinationSettingEntity dse;
        public Ior(DestinationSettingEntity dse = null)
        {
            InitializeComponent();
            this.dse = dse;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnDcc_Click(object sender, EventArgs e)
        {
            IorInfo iorInfo = new IorInfo();
            iorInfo.Show();
        }

        private void btnOac_Click(object sender, EventArgs e)
        {
            IorInfo iorInfo = new IorInfo();
            iorInfo.Show();
        }

        private void btnExh_Click(object sender, EventArgs e)
        {
            IorInfo iorInfo = new IorInfo();
            iorInfo.Show();
        }

        private void Ior_Load(object sender, EventArgs e)
        {
            if (dse != null )
            {
                txtTitle.Text = dse.TBBOXDestination.Name;
                if (!string.IsNullOrEmpty(txtTitle.Text))
                    txtTitle.Enabled = false;
            }
        }
        private bool Save()
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                MessageBox.Show("Fill the IORoom Name.");
                return false;
            }
            var tb = dse.RouteInfo.LstTBBOXes.Where(x => x.Name == (txtTitle.Text));
            if (tb.Any() && txtTitle.Enabled)
            {
                MessageBox.Show("This name is already specified in the system. Please set an another name.");
                return false;
            }
            dse.TBBOXDestination.Name = txtTitle.Text;
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                MessageBox.Show("Saved successfully!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save!");
                //this.DialogResult = DialogResult.OK;
                
            }
            
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtAccessF_H_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSize_D_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
