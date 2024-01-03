using RouteOptimizer.Entity;
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
    public partial class MCCSetForm : System.Windows.Forms.Form
    {
        BL.SettingBL SettingBL = new BL.SettingBL();
        public RouteOptimizer.Object.RouteInfo RouteInfo = null;
        public DestinationSettingEntity des;
        public MCCSetForm(DestinationSettingEntity dse = null)
        {
            InitializeComponent();

            this.des = dse;
            RouteInfo = dse.RouteInfo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
      
        }

        private void MCCSetForm_Load(object sender, EventArgs e)
        {
            if (des != null && des.TBBOXDestination != null)
            {
                txtmccName.Text = des.TBBOXDestination.Name.Replace(String.IsNullOrEmpty(RouteInfo.TBABBRE) ? "____" : RouteInfo.TBABBRE, ""); 
                txtmccName.Enabled = false;
            }
            else
                txtmccName.Enabled = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView2.AutoGenerateColumns = false;
            BindComboGrid();
            BindGrid_1();
            BindGrid_2_Initial();
           
       
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void BindGrid_1()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("colNo_UG");
            dataTable.Columns.Add("colEquip_UG");

            dataTable.Columns.Add("colTitle_UG");
            //dataTable.Columns.Add("colSystem_UG");
            //dataTable.Columns.Add("colCable_UG");
            //dataTable.Columns.Add("colStatus_UG");
            //dataTable.Columns.Add("colTagNo_UG");
            dataTable.Columns.Add("colCheck_UG"); 
            //var mcclist = SettingBL.GetMCCPkgList();
            //if (mcclist.Count == 0)
            //{
            //    MessageBox.Show("There is no cable in MCC file.");
            //    return;
            //}
            int j = 0;

            if (!txtmccName.Enabled && des.TBBOXDestination.LstmCCEntitiesHeader != null)
            {
                foreach (var ml in des.TBBOXDestination.LstmCCEntitiesHeader)
                {
                    j++;
                    dataTable.Rows.Add(
                        new object[]
                        {
                    j.ToString(),
                     ml.EquipmentName,
                    ml.Title,
                    //ml.Signal,
                    //ml.CableSpecifications,
                    //ml.Status,
                    //ml.TagNo,
                    false
                    });
                }
            }


            dataGridView1.DataSource = dataTable;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            bool ExistCheck = false;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (string.IsNullOrEmpty(dr.Cells["colNo_UG"].EditedFormattedValue.ToString()))  continue;
                if ( string.IsNullOrEmpty( dr.Cells["colEquip_UG"].EditedFormattedValue.ToString() )  && !string.IsNullOrEmpty(dr.Cells["colTitle_UG"].EditedFormattedValue.ToString()))
                {
                    ExistCheck = true;
                    break;
                }
                if (!string.IsNullOrEmpty(dr.Cells["colEquip_UG"].EditedFormattedValue.ToString()) && string.IsNullOrEmpty(dr.Cells["colTitle_UG"].EditedFormattedValue.ToString()))
                {
                    ExistCheck = true;
                    break;
                }
            }
            if (ExistCheck)
            {
                MessageBox.Show("There are invalid or empty inputs. Please check that.");
                return;
            }
            var msg = MessageBox.Show("These all items will be overwrite previous items. Are you sure to import these items?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            if (msg == DialogResult.Yes)
            {
                BindGrid_2();
            } 

        }
        private void BindGrid_2_Initial()
        {
            if (des != null && des.TBBOXDestination != null)
            {
                var cableList = des.TBBOXDestination.LstmCCEntities;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("colNo_DG");
                dataTable.Columns.Add("colEquip_DG");
                dataTable.Columns.Add("colTitle_DG");
                dataTable.Columns.Add("colSystem_DG");
                dataTable.Columns.Add("colCable_DG");
                dataTable.Columns.Add("colStatus_DG");
                dataTable.Columns.Add("colTagNo_DG");
                dataTable.Columns.Add("colTo_DG");
                int k = 0;
                foreach (var l in cableList)
                {
                    k++;
                    dataTable.Rows.Add(new object[] {
                    k.ToString(),
                    l.EquipmentName,
                    l.Title,
                    l.Signal,
                    l.CableSpecifications,
                    l.Status,
                    l.TagNo,
                    l.To
                    });
                }
                dataGridView2.DataSource = dataTable; 
                checkBox2.Checked = des.TBBOXDestination.AutoCheck == "1";
                checkBox3.Checked = des.TBBOXDestination.EachCheck == "1";
            }
        }
        private void BindGrid_2()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("colNo_DG");
            dataTable.Columns.Add("colEquip_DG");
            dataTable.Columns.Add("colTitle_DG");
            dataTable.Columns.Add("colSystem_DG");
            dataTable.Columns.Add("colCable_DG");
            dataTable.Columns.Add("colStatus_DG");
            dataTable.Columns.Add("colTagNo_DG");
            dataTable.Columns.Add("colTo_DG");

            int j = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (!string.IsNullOrEmpty(dr.Cells["colTitle_UG"].EditedFormattedValue.ToString().Trim()))
                {
                    var mcclist = SettingBL.GetMCCPkgList(); 
                    var lst = mcclist.Where(c => c.Title == dr.Cells["colTitle_UG"].EditedFormattedValue.ToString().Trim());
                    if (!lst.Any()) continue;
                    foreach (var mc in lst)
                    {
                        j++;
                        dataTable.Rows.Add(
                            new object[]
                            {
                            j.ToString(),
                             dr.Cells["colEquip_UG"].EditedFormattedValue.ToString().Trim(),
                            dr.Cells["colTitle_UG"].EditedFormattedValue.ToString().Trim(),
                            mc.Signal,
                            mc.CableSpecifications,
                            mc.Status,
                           mc.TagNo,
                            -1
                            }
                            );
                    }
                }
            }
            dataGridView2.DataSource = dataTable;
        }
        private void BindComboGrid()
        {
            DataTable MCC = new DataTable();
            MCC.Columns.Add("colTitle_UG");
       
            var mcclist = SettingBL.GetMCCPkgList();
            if (mcclist.Count == 0)
            {
                MessageBox.Show("There is no cable in MCC file.");
                return;
            }
            List<string> tmp = new List<string>();
            foreach (var t in mcclist)
            {
                if (tmp.Contains(t.Title)) continue;
                //var e_tri = MCC.GroupBy(w => w).Where(g => g.Count() > 1).Any();
                //if (!MCC.Contains(t.Title))
                tmp.Add(t.Title);
                MCC.Rows.Add(t.Title); 
            }
            DataGridViewComboBoxColumn colMCC = (DataGridViewComboBoxColumn)dataGridView1.Columns["colTitle_UG"];
            colMCC.DisplayMember = "colTitle_UG";
            colMCC.ValueMember = colMCC.DataPropertyName = "colTitle_UG";
            colMCC.DataSource = MCC;


            DataTable type = new DataTable();
            type.Columns.Add("GUID");
            type.Columns.Add("colTo_DG");
            type.Rows.Add(null, -1);
            foreach (var t in RouteInfo.LstTBBOXes)
            {
                if (t.IsIO != Object.RouteInfo.eDestinationType.MCC && !string.IsNullOrEmpty(t.Name))
                {
                    type.Rows.Add(t.Name, t.guid);
                }
            }
            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dataGridView2.Columns["colTo_DG"];
            col.DisplayMember = "GUID";
            col.ValueMember = col.DataPropertyName = "colTo_DG";
            col.DataSource = type;



        }
        private void button2_Click(object sender, EventArgs e)
        { 
            this.DialogResult = DialogResult.None;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!txtmccName.Enabled)
            {
                var msg = MessageBox.Show("Are you sure to save the current changes?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (msg != DialogResult.Yes)
                {
                    return;
                }
            }
            if (SaveData())
            {
                MessageBox.Show("Saved successfully!");
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Failed to save!");
                return; 
            } 
        }
        private bool SaveData()
        {
            if (txtmccName.Enabled && string.IsNullOrEmpty(txtmccName.Text))
            {
                return false;
            }
            try
            {
                var tb = RouteInfo.LstTBBOXes.Where(x => x.Name == (txtmccName.Text));
                if (tb.Any() && txtmccName.Enabled)
                {
                    MessageBox.Show("This name is already specified in the system. Please set an another name.");
                    return false;
                }
                if (des.TBBOXDestination == null)
                    des.TBBOXDestination = new TBBOXDestination();

                var lstCableHeader = new List<MCCEntity>();
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    if (dr.Cells["colNo_UG"].Value == null) continue;
                    MCCEntity mCCEntity = new MCCEntity();
                    mCCEntity.Title = dr.Cells["colTitle_UG"].Value.ToString();
                    mCCEntity.EquipmentName = dr.Cells["colEquip_UG"].Value.ToString();
                    //mCCEntity.CableSpecifications = dr.Cells["colCable_DG"].Value.ToString();
                    //mCCEntity.Status = dr.Cells["colStatus_DG"].Value.ToString();
                    //mCCEntity.TagNo = dr.Cells["colTagNo_DG"].Value.ToString();
                    //mCCEntity.To = dr.Cells["colTo_DG"].Value == null ? "-1" : dr.Cells["colTo_DG"].Value.ToString();
                    lstCableHeader.Add(mCCEntity);
                }
                des.TBBOXDestination.LstmCCEntitiesHeader = lstCableHeader;


                var lstCable = new List<MCCEntity>();
                foreach (DataGridViewRow dr in dataGridView2.Rows)
                {
                    MCCEntity mCCEntity = new MCCEntity();
                    mCCEntity.EquipmentName = dr.Cells["colEquip_DG"].Value.ToString(); 
                    mCCEntity.Title = dr.Cells["colTitle_DG"].Value.ToString();

                    mCCEntity.Signal = dr.Cells["colSystem_DG"].Value.ToString();
                    mCCEntity.CableSpecifications = dr.Cells["colCable_DG"].Value.ToString();
                    mCCEntity.Status = dr.Cells["colStatus_DG"].Value.ToString();
                    mCCEntity.TagNo = dr.Cells["colTagNo_DG"].Value.ToString();
                    mCCEntity.To = dr.Cells["colTo_DG"].Value == null ? "-1" : dr.Cells["colTo_DG"].Value.ToString();
                    lstCable.Add(mCCEntity);
                }

                des.TBBOXDestination.AutoCheck = checkBox2.Checked ? "1" : "0";
                des.TBBOXDestination.EachCheck = checkBox3.Checked ? "1" : "0";
                des.TBBOXDestination.LstmCCEntities = lstCable;
                des.TBBOXDestination.Name = txtmccName.Text;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);
                return false;
            }
            return true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        { 
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
               // dr.Cells["colCheck_UG"].Value = checkBox1.Checked;
            } 
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow dr in dataGridView2.Rows)
            //{
            //    dr.Cells["colCheck_DG"].Value = checkBox4.Checked;
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool ExistCheck = false;
            List<DataGridViewRow> lstDr = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in dataGridView2.Rows)
            {
               if ((bool) dr.Cells["colCheck_DG"].FormattedValue)
                {
                    lstDr.Add(dr);
                    ExistCheck = true;
                   // break;
                }
            }
            if (!ExistCheck)
            {
                MessageBox.Show("There is no selected item to delete. Please check the items.");
                return;
            }
            var msg = MessageBox.Show("Are you sure to delete the selected items?","Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (msg == DialogResult.Yes)
            { 
                foreach (DataGridViewRow dr in lstDr)
                {
                    dataGridView2.Rows.Remove(dr);
                }
                
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

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int o = 0;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (string.IsNullOrEmpty(dr.Cells["colEquip_UG"].EditedFormattedValue.ToString()) && string.IsNullOrEmpty(dr.Cells["colTitle_UG"].EditedFormattedValue.ToString())) continue;
                o++;
                dr.Cells["colNo_UG"].Value = o.ToString();
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!checkDuplicate(dataGridView1, "colEquip_UG"))
            {
                e.Cancel = true;
            }
        }
        private bool checkDuplicate(DataGridView dgv, string Col1)
        {
            List<string> data = new List<string>();
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var val = dr.Cells[Col1].EditedFormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val))
                    data.Add(val); 
            } 
            var e_tri = data.GroupBy(w => w).Where(g => g.Count() > 1).Any();
            if (e_tri)
            {
                MessageBox.Show("There are some duplicated values. Please fix this.");
                return false;
            }
            return true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
