using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RouteOptimizer.Object;
using System.Data;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdPrimaries;

namespace RouteOptimizer
{
    public partial class Gapped_Item : System.Windows.Forms.Form
    { 

        public RouteInfo RouteInfo { get; set; }
        public vdControls.vdFramedControl vdFramedControl { get; set; }
        public Gapped_Item(vdControls.vdFramedControl vdFramedControl, RouteInfo routeInfo)
        {
            InitializeComponent();
            this.RouteInfo = routeInfo;
            this.vdFramedControl = vdFramedControl;
        }
        private void BindGrid()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("colNo");
            dataTable.Columns.Add("colLayer");
            dataTable.Columns.Add("colT1");
            dataTable.Columns.Add("colT2");
            dataTable.Columns.Add("colFlg");
            int n = 0;
            var en = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            if (RouteInfo.DGV_LstInstrument != null)// return;
            {
                foreach (var i in RouteInfo.DGV_LstInstrument)
                {
                    var ins = RouteInfo.LstallInstruments.Where(x => x.OwnerInsert.Id == i.Instrument.OwnerInsert.Id).FirstOrDefault();
                    if (ins != null)
                    {
                        n++;
                        dataTable.Rows.Add(
                            new object[] {

                            n,
                            ins.OwnerInsert.Layer.Name,
                            ins.t1,
                            ins.t2,
                            (ins.circle.Center ==  ins.centerPoint) ? "NO" : "YES"
                            }
                            )
                                ;
                    }
                }
            }
            dgv_Dupli.DataSource = dataTable;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Gapped_Item_Load(object sender, EventArgs e)
        {
            BindGrid();
            PlaceLowerRight(this);
        }
        private void PlaceLowerRight(System.Windows.Forms.Form t)
        {
            //Determine "rightmost" screen
            Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }

            t.Left = rightmost.WorkingArea.Right - t.Width - 50;
            t.Top = rightmost.WorkingArea.Bottom - t.Height - 300;
        }
        private void dgv_Dupli_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
