using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RouteOptimizer
{
    public partial class DuctSizeOptimization : System.Windows.Forms.Form
    {
        public DuctSizeOptimization()
        {
            InitializeComponent();
        }

        private void DuctSizeOptimization_Load(object sender, EventArgs e)
        {
            if (MainP1.dtOptimalResult != null)
            {
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = MainP1.dtOptimalResult;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
