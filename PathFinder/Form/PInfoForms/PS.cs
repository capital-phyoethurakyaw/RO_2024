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

namespace RouteOptimizer.PInfoForms
{
    public partial class PS : System.Windows.Forms.Form
    {
        public PS()
        {
            InitializeComponent();
        }

        private void btproject_Click(object sender, EventArgs e)
        {
            loadform(new PI());
        }

        public void loadform(object Form)
        {
            if (this.mainpanel1.Controls.Count > 0)
                this.mainpanel1.Controls.RemoveAt(0);
           System.Windows.Forms. Form f = Form as System.Windows.Forms.Form;
            f.TopLevel = false;
            f.Dock= DockStyle.Fill;
            this.mainpanel1.Controls.Add(f);
            this.mainpanel1.Tag= f;
            f.Show();
        }

        private void PS_Load(object sender, EventArgs e)
        {
            loadform(new PI());
        }

        private void btinst_Click(object sender, EventArgs e)
        {
            loadform(new Inst());
        }

        private void btcable_Click(object sender, EventArgs e)
        {
            loadform(new C());
        }

        private void btcableduct_Click(object sender, EventArgs e)
        {
            loadform(new Cd());
        }

        private void btmcc_Click(object sender, EventArgs e)
        {
            loadform(new Mccpkg());
        }

        private void btsys_Click(object sender, EventArgs e)
        {
            loadform(new Sys());
        }

        private void btsignal_Click(object sender, EventArgs e)
        {
            loadform(new SignalList());
        }

        private void btioroom_Click(object sender, EventArgs e)
        {
            loadform(new Ior());
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        { 
        } 
        private void mainpanel1_Paint(object sender, PaintEventArgs e)
        { 
        } 
        private void btn_opencsv_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Entity.StaticCache.path);
            this.Close();
            
            // 1. Save current status >> 2. close PS form >> 3. Open the file explorer of DS folder.
        }
    }
}
