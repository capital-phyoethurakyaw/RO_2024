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
    public partial class frmInstrument : System.Windows.Forms.Form
    {
        static DataTable DataSourceInstrumentType = Entity.StaticCache.DataSourceInstrumentType();
        public frmInstrument()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmInstrument_Load(object sender, EventArgs e)
        {
            if (MainP1.dtInstrument != null)
            {
                dataGridView1.AutoGenerateColumns = false;

                InstrumentType.Name = "Name";
                InstrumentType.DataSource = DataSourceInstrumentType;
                InstrumentType.DisplayMember = "Name";
                InstrumentType.ValueMember = "Key";

                dataGridView1.DataSource = MainP1.dtInstrument;

            }
        }
    }
}
