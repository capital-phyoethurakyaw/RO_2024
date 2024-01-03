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
    public partial class DuplicateItem : System.Windows.Forms.Form
    {
        public RouteInfo RouteInfo { get; set; }
        public vdControls.vdFramedControl vdFramedControl { get; set; }
        public DataTable dtInstrumnet { get; set; }
        public List<int> OutofRange { get; set; }

        public DuplicateItem(RouteInfo routeInfo, vdControls.vdFramedControl vdFramedControl, bool OnlyDetectedItem = false, List<int> OutofRange= null)
        {
            InitializeComponent();
            this.RouteInfo = routeInfo;
            this.vdFramedControl = vdFramedControl;
            checkBox2.Checked = OnlyDetectedItem; 
            if (OutofRange != null && OutofRange.Count >0)
            {
                this.OutofRange = OutofRange;
                this.Text = "분석 범위 외 계측기기 확인하기";
                label2.Text = "분석 범위 외 계측기기 확인하기";
                label1.Text = "아래의 계측기기들은 분석 범위에 포함되지 못하였습니다. 확인 후 재 분석 진행해주세요.";
            } 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
         
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BindGridView(bool OnlyDetectedItem= false)
        {
            var lstIns = RouteInfo.GetAllInstrument(vdFramedControl, RouteInfo);

            var dtduplicate = new DataTable();
            dtduplicate.Columns.Add("T1_T2");
            foreach (var r in lstIns)
            {
                dtduplicate.Rows.Add(new object[] { r.t1 + "_" + r.t2 });
            }
            var query = (from row in dtduplicate.AsEnumerable()
                         group row by new { t11 = row.Field<string>("T1_T2") } into dupli
                         where dupli.Count() > 1
                         select new { t11 = dupli.Key.t11 });
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("colGUID");
            dataTable.Columns.Add("colNo");
            dataTable.Columns.Add("colLayer");
            dataTable.Columns.Add("colT1");
            dataTable.Columns.Add("colT2");
            dataTable.Columns.Add("colFlg");
            int n = 0;
            foreach (var i in query)
            {
                var ins = lstIns.Where(z => z.t1 + "_" + z.t2 == i.t11);
                if (ins.Any())
                {
                    if (OnlyDetectedItem)
                    {
                        var inst = ins.FirstOrDefault();
                        n++;
                        dataTable.Rows.Add(
                            new object[] {
                            inst.OwnerInsert.Id,
                            n,
                            inst.OwnerInsert.Layer.Name,
                            inst.t1,
                            inst.t2,
                            false
                            }
                            )
                                ;
                    }
                    else
                    {
                        foreach (var inst in ins.ToList())
                        {
                            n++;
                            dataTable.Rows.Add(
                                new object[] {
                            inst.OwnerInsert.Id,
                            n,
                            inst.OwnerInsert.Layer.Name,
                            inst.t1,
                            inst.t2,
                            false
                                }
                                )
                                    ;
                        }
                    }
                }
            }
            dgv_Dupli.DataSource = dataTable;

        }
        private void Process()
        {
            var lstIns = RouteInfo.GetAllInstrument(vdFramedControl, RouteInfo);
            selectedItem = 0;
            Dictionary<int, string> tupleIDs = new Dictionary<int, string>();
            //List<int> InsertIDs = new List<int>();
            foreach (DataGridViewRow dr in dgv_Dupli.Rows)
            {
                try
                {
                    tupleIDs.Add(Convert.ToInt32(dr.Cells["colGUID"].Value), dr.Cells["colT1"].FormattedValue.ToString() + "_" + dr.Cells["colT2"].FormattedValue.ToString());
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);

                    MessageBox.Show(ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
                }
            }
            // RouteInfo.SyncronyzeRelatedUI(InsertIDs, vdFramedControl);

            var e = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems();
            foreach (vdFigure val in e)
            {
                if (val is vdInsert vs && vs.Block.Name == "TAG No.")
                {
                    var value = tupleIDs.Where(x => x.Key == val.Id);
                    if (!value.Any()) continue;

                    var v1 = value.FirstOrDefault().Value.Split('_').First();
                    var v2 = value.FirstOrDefault().Value.Split('_').Last();
                    if (vs.Attributes.Count > 0)
                    {
                        foreach (vdAttrib vda in vs.Attributes)
                        {
                            if (vda.TagString == "T1")
                            {
                                vda.ValueString = v1;
                            }
                            if (vda.TagString == "T2")
                            {
                                vda.ValueString = v2;
                            }
                        }
                    }
                }
            }
            selectedItem = tupleIDs.Count; 
            RefreshCADSpace();
        }
        private void RefreshCADSpace()
        {
            vdFramedControl.BaseControl.Redraw();
            vdFramedControl.BaseControl.Update();
            vdFramedControl.Update();
            vdFramedControl.BaseControl.ActiveDocument.Redraw(true);
            vdFramedControl.BaseControl.ActiveDocument.ActiveLayer.Update();
        }
        private void button1_Click(object sender, EventArgs e)
        {
         //   MessageBox.Show("The selected " + selectedItem + " items have been updated.");
            if (dgv_Dupli.Rows.Count == 0) return;
            if (sender != null)
            {
                var dlg = MessageBox.Show("The modified attributes will be updated into CAD window. Would you like to proceed?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlg != DialogResult.OK) return;
            }
            
            Process();
            BindGridView();
           
            if (selectedItem == 0)
            {
                //MessageBox.Show("There is no selected item. Please make check at least one.");
            }  
            else
            {
                MessageBox.Show("The selected " + selectedItem + " items have been updated.");
            }
            SendKeys.Send("{ESC}");
            SendKeys.Send("{ESC}");
            RefreshCADSpace();
            //SendKeys.Send("{ESC}");
            //SendKeys.Send("{ESC}");
            //RefreshCADSpace();
            //SendKeys.Send("{ESC}");
            //SendKeys.Send("{ESC}");
            SaveFlg = true;
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

            t.Left = rightmost.WorkingArea.Right - t.Width-50;
            t.Top = rightmost.WorkingArea.Bottom - t.Height - 300;
        }
        private void DuplicateItem_Load(object sender, EventArgs e)
        {
            if (OutofRange != null && OutofRange.Count >0)
            {
              
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("colGUID");
                dataTable.Columns.Add("colNo");
                dataTable.Columns.Add("colLayer");
                dataTable.Columns.Add("colT1");
                dataTable.Columns.Add("colT2");
                dataTable.Columns.Add("colFlg");
                int n = 0;
                var en = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                foreach (var i in OutofRange)
                {
                    var ins = RouteInfo.LstallInstruments.Where(x=>x.OwnerInsert.Id == i).FirstOrDefault();
                    if (ins != null)
                    { 
                            n++;
                            dataTable.Rows.Add(
                                new object[] {
                            ins.OwnerInsert.Id,
                            n,
                            ins.OwnerInsert.Layer.Name,
                            ins.t1,
                            ins.t2,
                            false
                                }
                                )
                                    ; 
                    }
                }
                dgv_Dupli.DataSource = dataTable;
                button1.Visible = button2.Visible = checkBox1.Visible = checkBox2.Visible = dgv_Dupli.Columns["colFlg"].Visible = false;
                button3.Text = "닫기";
            }
            else
            {
                BindGridView(checkBox2.Checked);
            }
            PlaceLowerRight(this);
        }

        private void dgv_Dupli_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var en = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems();
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            if (validClick)
            {
                var guid = Convert.ToInt32(dgv_Dupli[0, e.RowIndex].Value);
                
                foreach (vdFigure f in en)
                {
                    if (f.Id == guid)
                    {
                        f.HighLight = true;
                       // break;
                    }
                    else
                    {
                        f.HighLight = false;
                    }
                }
                var fg = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(guid).BoundingBox;
                var lc = fg.UpperLeft;
                lc.x -= fg.Width;
                lc.y += fg.Height;
                var rb = fg.LowerRight;
                rb.x += fg.Width;
                rb.y -= fg.Height;
                vdFramedControl.BaseControl.ActiveDocument.CommandAction.Zoom("w",  lc, rb);
            }
            RefreshCADSpace();
        }

        private void dgv_Dupli_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            if (dgv_Dupli.CurrentCell is DataGridViewTextBoxCell c && validClick)
            {
                if (!c.ReadOnly)
                    dgv_Dupli.BeginEdit(true);
            }
        }

        bool SaveFlg = false;
        private void button3_Click_2(object sender, EventArgs e)
        {
            try
            {
                if (dgv_Dupli.Rows.Count > 0)
                {
                    if (OutofRange != null && OutofRange.Count > 0)
                    {
                        this.Close();
                    }
                    else
                    {
                        if (!SaveFlg)
                        {
                            var dlg = MessageBox.Show("Do you want to save changes?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                            if (dlg != DialogResult.Yes)
                            {
                                this.DialogResult = DialogResult.OK;

                                this.Close();
                                //return;
                            }
                            button1_Click(null, null); // Go to saved function

                        }
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace +  Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
                this.Close();
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
        int selectedItem = 0;
        private void RemoveItems()
        {
            selectedItem = 0;
            var lstIns = RouteInfo.GetAllInstrument(vdFramedControl, RouteInfo);

            Dictionary<int, string> tupleIDs = new Dictionary<int, string>();
            List<int> InsertIDs = new List<int>();
            foreach (DataGridViewRow dr in dgv_Dupli.Rows)
            {
                if ((bool)dr.Cells["colFlg"].FormattedValue)
                {
                    InsertIDs.Add(Convert.ToInt32(dr.Cells["colGUID"].Value));
                }
            }
            RouteInfo.SyncronyzeRelatedUI(InsertIDs, vdFramedControl);
            selectedItem = InsertIDs.Count;
            RefreshCADSpace();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dgv_Dupli.Rows.Count == 0) return;

            var dlg = MessageBox.Show("Are you sure to remove the checked instrument?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (dlg == DialogResult.Yes)
            {
                RemoveItems();
                BindGridView();
               
                if (selectedItem == 0)
                {
                    MessageBox.Show("There is no selected item. Please make check at least one.");
                }
                else
                {
                    MessageBox.Show("The selected " + selectedItem + " items have been removed.");
                }
                SendKeys.Send("{ESC}");
                SendKeys.Send("{ESC}");
                RefreshCADSpace();
            } 
            SaveFlg = true;
        }

        private void dgv_Dupli_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveFlg = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            BindGridView(checkBox2.Checked);
            checkBox1.Enabled = button1.Enabled = button2.Enabled = !checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dgv_Dupli.Rows)
            {
                dr.Cells["colFlg"].Value = (sender as CheckBox).Checked;
            }
        }
    }
}
