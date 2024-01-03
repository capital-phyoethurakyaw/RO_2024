using RouteOptimizer.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RouteOptimizer;
namespace RouteOptimizer
{
    public partial class DestSetForm : System.Windows.Forms.Form
    {
        //static string DataSourceSignal = Entity.StaticCache.DataSourceSignal;
        public RouteOptimizer.Object.RouteInfo RouteInfo = null;
        public DestinationSettingEntity des;
        public DestSetForm(DestinationSettingEntity dse = null)
        {
            InitializeComponent();
            var topLeftHeaderCell1 = dgv_CLInput.TopLeftHeaderCell; // Make sure TopLeftHeaderCell is created 
            this.RouteInfo = dse.RouteInfo;
            this.des = dse;
            dataGridView1.AutoGenerateColumns = false;
       
        }
        public string TBName = "";
        private void button1_Click(object sender, EventArgs e)
        {
            TBName = txtTBName.Text;
            if (!IsSaved()) { return; }
            if (des.TBBOXDestination == null)
                des.TBBOXDestination = new TBBOXDestination();
            var OwnDes = RouteInfo.LstTBBOXes.Where(x => x.Name == cbo_FinalDestination.Text.ToString());
            if (OwnDes.Any() && !string.IsNullOrEmpty( cbo_FinalDestination.Text))
            {
                var t = new TBBOXDestination();
                t = OwnDes.FirstOrDefault();
                des.TBBOXDestination.OwnDestination =  t;
            }
            else
                des.TBBOXDestination.OwnDestination = null;

            des.TBBOXDestination.Name = RouteInfo.TBABBRE + txtTBName.Text;

            if (!OkCheckCableGrid()) { return; }
            //;
            // des.TBBOXDestination.CableType = cbo_CLOutput.SelectedValue == null ? "" : cbo_CLOutput.SelectedValue.ToString();
            des.TBBOXDestination.AutoCheck = chk_AutoRoute.Checked ? "1" : "0";
            des.TBBOXDestination.EachCheck = chk_EachRoute.Checked ? "1" : "0";
            MessageBox.Show("Saved successfully!");
            this.DialogResult = DialogResult.OK;
            this.Close();

        }
        private bool IsSaved()
        {
            if (String.IsNullOrEmpty(txtTBName.Text))
            {
                MessageBox.Show("Please fill the T/B Box Name.");
                return false;
            } 
            if (txtTBName.Text == cbo_FinalDestination.Text.Split('-').Last())
            {
                MessageBox.Show("Please set  a different TBBox.");
                return false;
            }
            var tb = RouteInfo.LstTBBOXes.Where(x=>x.Name ==  (RouteInfo.TBABBRE + txtTBName.Text ));
            if (tb.Any() && txtTBName.Enabled)
            {
                MessageBox.Show("This name is already specified in the system. Please set an another name.");
                return false;
            } 
            return true;
        }

        private void BindGridCombo()
        {
            BL.SettingBL settingBL = new BL.SettingBL();
            var lstCable = settingBL.GetCableList();

            var dt = new DataTable();
            dt.Columns.Add("colCable_UG");
            foreach (var c in lstCable)
                dt.Rows.Add(c.Title);

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dataGridView1.Columns["colCable_UG"];
            col.DisplayMember = "colCable_UG";
            col.ValueMember = col.DataPropertyName = "colCable_UG";
            col.DataSource = dt;

            var lstSginal = settingBL.GetSignalType();
            var dtsignal = new DataTable();
            dtsignal.Columns.Add("colSignal_UG"); 
            foreach (var c in lstSginal)
                dtsignal.Rows.Add(c.Title.TrimEnd());

            DataGridViewComboBoxColumn colSignal = (DataGridViewComboBoxColumn)dataGridView1.Columns["colSignal_UG"];
            colSignal.DisplayMember = "colSignal_UG";
            colSignal.ValueMember = colSignal.DataPropertyName = "colSignal_UG";
            colSignal.DataSource = dtsignal;

        }

        private void DestinationNamingForm_Load(object sender, EventArgs e)
        {
            BindGridCombo();
            //BL.SettingBL settingBL = new BL.SettingBL();
            //var lstCable = settingBL.GetCableList();

            //var dt = new DataTable();
            //dt.Columns.Add("Title");
            //foreach (var c in lstCable) dt.Rows.Add(c.Title);

            //cbo_CLOutput.DataSource = dt;
            //cbo_CLOutput.DisplayMember = cbo_CLOutput.ValueMember = "Title"; 
            if (des != null && des.TBBOXDestination != null)
            { 
                txtTBName.Text = des.TBBOXDestination.Name.Replace(String.IsNullOrEmpty(RouteInfo.TBABBRE)? "____":RouteInfo.TBABBRE,"");//.Replace(
              //  cbo_CLOutput.SelectedValue = des.TBBOXDestination.CableType==null? "": des.TBBOXDestination.CableType ;
                chk_AutoRoute.Checked = (des.TBBOXDestination.AutoCheck == "1");
                chk_EachRoute.Checked = (des.TBBOXDestination.EachCheck == "1");

                txtTBName.Enabled = false;
            }
            else
                txtTBName.Enabled = true;

            if (des.TBBOXDestination == null)   des.TBBOXDestination = new TBBOXDestination(); 
            txtDestinationName.Text = txtTBName.Text;
            cbo_FinalDestination.Items.Clear();
            if (RouteInfo.LstTBBOXes != null)
                foreach (var tb in RouteInfo.LstTBBOXes)
                {
                    if (!string.IsNullOrEmpty(tb.Name) && tb.Name != RouteInfo.TBABBRE + txtTBName.Text && tb.IsIO != Object.RouteInfo.eDestinationType.MCC)
                    {
                        cbo_FinalDestination.Items.Add(tb.Name);
                    }
                }
            cbo_FinalDestination.SelectedText = ( (des.TBBOXDestination) == null || des.TBBOXDestination.OwnDestination == null )? "" : des.TBBOXDestination.OwnDestination.Name;
            BindGrid();
            BindCableGrid();
        }
        private void BindGrid()
        {
            if (RouteInfo == null || RouteInfo.DGV_LstInstrument == null) return;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("No");
            dataTable.Columns.Add("Instrument");
            dataTable.Columns.Add("Cable");
            int o = 0;
            RouteInfo.SetvaluesToNullCablesFromMultiCheck();
            foreach (var iie in RouteInfo.DGV_LstInstrument)
            {
                foreach (var ise in iie.LstInstCableEntity)
                {
                    try
                    {
                        if (!(int.TryParse(ise.To, out int r)))
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(ise.To) ||
                            (ise.To == "-1") ||
                            (RouteInfo.FindTBBox(Convert.ToInt32(ise.To)) == null) ||
                            RouteInfo.FindTBBox(Convert.ToInt32(ise.To)).Name != (RouteInfo.TBABBRE + txtTBName.Text))
                            continue;
                    }
                    catch (Exception ex)
                    {
                        //var loc = ex.StackTrace + Environment.NewLine + ex.Message;
                        DebugLog.WriteLog(ex);
                    }
                    o++;
                    dataTable.Rows.Add(new object[] {
                    o.ToString(),
                    ise.From,
                    ise.Cable
                    });
                }
            }

            dgv_CLInput.DataSource = dataTable;
          //  cbo_CLOutput.SelectedIndexChanged += Cbo_CLOutput_SelectedIndexChanged; 
        }

        private void Cbo_CLOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            var com = (sender as ComboBox);
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtTBName_TextChanged(object sender, EventArgs e)
        {
            txtDestinationName.Text = txtTBName.Text;
            des.TBBOXDestination.Name= RouteInfo.TBABBRE+ txtTBName.Text; ;
        }

        private void dgv_CLInput_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var val = "";
                if (dgv_CLInput.Rows[e.RowIndex].Cells["Cable"].Value != null)
                    val = dgv_CLInput.Rows[e.RowIndex].Cells["Cable"].Value.ToString();
                txtCable.Text = val;
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!checkDuplicate(dataGridView1, "colCable_UG","colSignal_UG"))
            {
                e.Cancel = true;
            }
        }
        private bool checkDuplicate(DataGridView dgv, string Col1, string Col2)
        {
            List<string> data = new List<string>();
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var val = dr.Cells[Col1].EditedFormattedValue.ToString().Trim();
                var val2 = dr.Cells[Col2].EditedFormattedValue.ToString().Trim();
                if (string.IsNullOrEmpty(val) && string.IsNullOrEmpty(val2)) continue;
                //if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(val2))
                data.Add(val + val2);
            }
            var e_tri = data.GroupBy(w => w).Where(g => g.Count() > 1).Any();
            if (e_tri)
            {
                MessageBox.Show("There are some duplicated values. Please fix this.");
                return false;
            }
            return true;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int o = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                try
                {
                    if (string.IsNullOrEmpty(dr.Cells["colSignal_UG"].EditedFormattedValue.ToString()) && string.IsNullOrEmpty(dr.Cells["colCable_UG"].EditedFormattedValue.ToString()))
                        continue;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);

                    // dr.Cells["colNo_UG"].Value = o.ToString();
                    var sf = ex;
                    //o++;
                    //  continue;
                }

                o++;
                dr.Cells["colNo_UG"].Value = o.ToString();


            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool ExistCheck = false;
            List<DataGridViewRow> lstDr = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if ((bool)dr.Cells["colCheck_UG"].FormattedValue)
                {
                    lstDr.Add(dr);
                    ExistCheck = true;
                    //break;
                }
            }
            if (!ExistCheck)
            {
                MessageBox.Show("There is no selected item to delete. Please check the items.");
                return;
            }
            var msg = MessageBox.Show("Are you sure to delete the selected items?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (msg == DialogResult.Yes)
            {
                foreach (DataGridViewRow dr in lstDr)
                {
                    dataGridView1.Rows.Remove(dr);
                }
            }
        }
        private void BindCableGrid()
        {
           
            DataTable dtCable = new DataTable();
            dtCable.Columns.Add("colNo_UG");
            dtCable.Columns.Add("colSignal_UG");
            dtCable.Columns.Add("colCable_UG");
            dtCable.Columns.Add("colCheck_UG");
            if (txtTBName.Enabled)
            {
                //dtCable.Rows.Add(
                //    new object[]{
                //       "",
                //        "-1",
                //    "-1",
                //        false
                //    }
                //    );
               // dataGridView1.DataSource = null;
               //dataGridView1.DataSource = dtCable;
                return;
            }
            if (des.TBBOXDestination != null && des.TBBOXDestination.LstmCCEntities != null && des.TBBOXDestination.LstmCCEntities.Count > 0)
            {
                int n = 0;
                foreach (var c in des.TBBOXDestination.LstmCCEntities)
                {
                    n++;

                    dtCable.Rows.Add(
                        new object[]{
                        n.ToString(), 
                        c.Signal,
                        c.CableSpecifications.TrimEnd(),
                        false
                        }
                        );
                }
            }

            dataGridView1.DataSource = dtCable;
        }
        private bool SaveSelection()
        { 
            try
            {
                
            }
            catch
            {
                return false;
            }
            return true;

        }
        private void button3_Click(object sender, EventArgs e)
        {
    
        }
        private bool OkCheckCableGrid()
        {
            bool ExistCheck = false;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (string.IsNullOrEmpty(dr.Cells["colNo_UG"].EditedFormattedValue.ToString())) continue;
                if (string.IsNullOrEmpty(dr.Cells["colSignal_UG"].EditedFormattedValue.ToString()) && !string.IsNullOrEmpty(dr.Cells["colCable_UG"].EditedFormattedValue.ToString()))
                {
                    ExistCheck = true;
                    break;
                }
                if (!string.IsNullOrEmpty(dr.Cells["colSignal_UG"].EditedFormattedValue.ToString()) && string.IsNullOrEmpty(dr.Cells["colCable_UG"].EditedFormattedValue.ToString()))
                {
                    ExistCheck = true;
                    break;
                }
            }
            if (ExistCheck)
            {
                MessageBox.Show("There are invalid or empty inputs. Please check that.");
                return false;
            }

            var lstCable = new List<MCCEntity>();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (dr.Cells["colNo_UG"].Value == null ||string.IsNullOrEmpty(dr.Cells["colNo_UG"].Value.ToString().Trim() )  ) continue;
                MCCEntity mCCEntity = new MCCEntity();
                mCCEntity.Signal = dr.Cells["colSignal_UG"].Value.ToString();
                mCCEntity.CableSpecifications = dr.Cells["colCable_UG"].Value.ToString();
                mCCEntity.To = des.TBBOXDestination.OwnDestination==null ? "-1" :  des.TBBOXDestination.OwnDestination.guid.ToString();
                lstCable.Add(mCCEntity);
            }
            des.TBBOXDestination.LstmCCEntities = lstCable;
            return true;
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int o = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                try
                {
                    if (string.IsNullOrEmpty(dr.Cells["colSignal_UG"].EditedFormattedValue.ToString()) && string.IsNullOrEmpty(dr.Cells["colCable_UG"].EditedFormattedValue.ToString()))
                        continue;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);

                    // dr.Cells["colNo_UG"].Value = o.ToString();
                    var sf = ex;
                    //o++;
                    //  continue;
                }

                o++;
                dr.Cells["colNo_UG"].Value = o.ToString();


            }
        }

        private void dgv_CLInput_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
 
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void chk_AutoRoute_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
