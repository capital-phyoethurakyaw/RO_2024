using CsvHelper;
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
using RouteOptimizer.PInfoForms;
using System.Collections;

namespace RouteOptimizer.PInfoForms
{
    public partial class Inst : System.Windows.Forms.Form
    {
        static string DataSourceInstrumentList = Entity.StaticCache.DataSourceInstrumentList;
        static string DataSourceCableList = Entity.StaticCache.DataSourceCableList;
        static string DataSourceSignal = Entity.StaticCache.DataSourceSignal;
        static string DataSourceInstCable = Entity.StaticCache.DataSourceInstCable;

        public Inst()
        {
            Loaded = 1;
            InitializeComponent();
            this.Load += IS_Load;
        }

        DataTable dtSource; //InstrumentList
        DataTable dtSource1;
        DataTable dtSignal; //Signal
        DataTable dtSystem; //system
        DataTable dtSource2; //CableList
        DataTable dtCableSpec; //Gridview2

        private void IS_Load(object sender, EventArgs e)
        {
           //SyncChildItems();
            BindGrid();
            dataGridView2.CellValueChanged += new DataGridViewCellEventHandler(dataGridView2_CellValueChanged);
            dataGridView2.CurrentCellDirtyStateChanged += new EventHandler(dataGridView2_CurrentCellDirtyStateChanged);
            //foreach (DataGridViewRow dr in dataGridView1.Rows)
            //{
            //    if (!string.IsNullOrWhiteSpace(dr.Cells["Col3"].FormattedValue.ToString()) || dr.Cells["Col3"].FormattedValue != null)
            //    {
            //        dr.ReadOnly = true;
            //        dr.Cells["Col3"].ReadOnly = true;

            //    }
            //    dataGridView1.RefreshEdit();
            //}
           
        }

        private void BindGrid()
        {
            if (!File.Exists(DataSourceInstrumentList) || !File.Exists(DataSourceSignal) || !File.Exists(DataSourceCableList) || !File.Exists(DataSourceInstCable))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceInstrumentList + " & " + DataSourceCableList + " & " + DataSourceSignal + " & " + DataSourceInstCable + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("Col1");
            dtSource.Columns.Add("Col2");
            dtSource.Columns.Add("Col3");
            dtSource.Columns.Add("Col4");

            List<InstrumentList> result;
            using (TextReader fileReader = File.OpenText(DataSourceInstrumentList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<InstrumentList>().ToList(); 
            }

            int i = 0;
            foreach (InstrumentList Il in result)
            {
                dtSource.Rows.Add(new object[] { Il.Classification_1.Trim(), Il.Classification_2.Trim(), Il.Classification_3.Trim(), Il.Classification_3.Trim() });
            }
            dataGridView1.DataSource = dtSource;

            

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                if (!checkDuplicate()) return;
                if (!CheckGrid(dataGridView1, "Col1", "Col2", "Col3")) return; 
                SaveChanges();
            
                //ClearDgv2();
                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool IsEditing = false;

        private void SaveChanges()
        {
            File.Delete(DataSourceInstrumentList);
            using (var writer = new StreamWriter(DataSourceInstrumentList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<InstrumentList> lst = new List<InstrumentList>();
                InstrumentList ie = new InstrumentList();

                foreach (DataRow dr in dtSource.Rows)
                {
                    ie = new InstrumentList();
                    ie.Classification_1 = dr["Col1"].ToString().Trim();
                    ie.Classification_2 = dr["Col2"].ToString().Trim();
                    ie.Classification_3 = dr["Col3"].ToString().Trim();
                    lst.Add(ie);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            SyncChildItems();
            BindGrid();
        }
        private void SyncChildItems()
        {
            List<InstrumentList1> lst = new List<InstrumentList1>();
            foreach (DataRow dr in dtSource.Rows)
            {
                InstrumentList1 ie = new InstrumentList1();
                ie.Classification_1 = dr["Col1"].ToString().Trim();
                ie.Classification_2 = dr["Col2"].ToString().Trim();
                ie.Classification_3 = dr["Col3"].ToString().Trim();
                ie.Classification_4 = dr["Col4"].ToString().Trim();
                lst.Add(ie);
            }
            List<Inst_Cable> chi;
            using (TextReader fileReader = File.OpenText(DataSourceInstCable))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                chi = csv.GetRecords<Inst_Cable>().ToList();

            }
            List<Inst_Cable> lstremove = new List<Inst_Cable>();
            foreach (var f in chi)
            {
                if (!lst.Where(x => x.Classification_4 == f.InstType).Any())
                {
                    lstremove.Add(f);
                }
            }
            foreach (var f in lstremove)
            {
                chi.Remove(f);
            }
            List<Inst_Cable> res = new List<Inst_Cable>();
            foreach (var f in chi)
            {
                var r = lst.Where(x => x.Classification_4 == f.InstType);
                if (r.Any())
                {
                    Inst_Cable re = new Inst_Cable();
                    re.InstType = r.FirstOrDefault().Classification_3;
                    re.Type = f.Type;
                    re.System = f.Cable;
                    re.Cable = f.Cable;
                    res.Add(re);
                }
            }
            if (File.Exists(DataSourceInstCable))
                File.Delete(DataSourceInstCable);
            using (var writer = new StreamWriter(DataSourceInstCable))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.WriteRecords(res);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            /////hereee
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                dataGridView2.Update();

                if (dataGridView1.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView1.SelectedRows.Count >= 1)
                {
                    DialogResult result = MessageBox.Show("Area you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        DeleteSelectedRow();
                        txtInstType.Text = string.Empty;
                        MessageBox.Show("Deleted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DeleteSelectedRow()
        {
            if (!String.IsNullOrWhiteSpace(txtInstType.Text))
            { 
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                { 
                    for (int i = dataGridView2.RowCount - 2; i >= 0; i--)
                    {
                        var deleterows = dataGridView2.Rows[i];
                        if (!row.IsNewRow)
                        {
                            if (row.Cells[2].EditedFormattedValue.ToString().Equals(deleterows.Cells[0].EditedFormattedValue.ToString()))
                            {
                                dataGridView2.Rows.Remove(deleterows);
                                dataGridView2.Update();
                            }

                        }
                    }
                    dataGridView1.Rows.Remove(row);
                    dataGridView1.Update();
                    dataGridView2.Update();
                    SaveChanges();
                    SaveChanges_G2("delete");
                }
               
            }
            else
            {
                MessageBox.Show("Please select item to be delete.");
                return;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;
            //if ((columnName == "Col3") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())))
            //{
            //    txtInstType.Text = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            //    BindGridView2(txtInstType.Text);
            //}
        }
        private void ClearDgv2( )
        {  
            List<DataGridViewRow> g = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in dataGridView2.Rows)
            {
                if (dr.Cells[0].Value != null && !string.IsNullOrWhiteSpace(dr.Cells[0].Value.ToString()))
                    g.Add(dr);
            }
            foreach(var r in g)
            {
                dataGridView2.Rows.Remove(r);
            }

            txtInstType.Text = "";
            //IsEditing = false;
            //btnGv2Delete.Enabled = btnGv2Save.Enabled = false;
        }
        private void BindGridView2(string InstType )
        {
            try
            { 
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("InstType");
                dtTemp.Columns.Add("Type");
                dtTemp.Columns.Add("System");
                dtTemp.Columns.Add("Cable");
                 
                if (!String.IsNullOrWhiteSpace(InstType))
                {
                    //Signal
                    dtSignal = new DataTable();
                    dtSignal.Columns.Add("Type");

                    List<Signal> result1;
                    using (TextReader fileReader = File.OpenText(DataSourceSignal))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = false;
                        csv.Read();
                        result1 = csv.GetRecords<Signal>().ToList();

                    }
                    foreach (Signal S in result1)
                    {
                        dtSignal.Rows.Add(new object[] { S.Type.Trim() });
                    }
                    dtSignal = RemoveDuplicateRows(dtSignal, "Type");

                    //CableList
                    dtSource2 = new DataTable();
                    dtSource2.Columns.Add("Cable");
                    List<Cable_List> result2;
                    using (TextReader fileReader = File.OpenText(DataSourceCableList))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = false;
                        csv.Read();
                        result2 = csv.GetRecords<Cable_List>().ToList();

                    }
                    foreach (Cable_List CL in result2)
                    {
                        dtSource2.Rows.Add(new object[] { CL.Cable.Trim() });
                    }
                   
                    //Bind datat to ComboBoxColumn at GridView2
                    DataGridViewComboBoxColumn colSignal = (DataGridViewComboBoxColumn)dataGridView2.Columns["cboSignal"];
                    colSignal.DataPropertyName = "Type";
                    colSignal.FlatStyle = FlatStyle.Standard;
                    colSignal.DisplayMember = "Type";
                    ((DataGridViewComboBoxColumn)dataGridView2.Columns["cboSignal"]).DataSource = dtSignal;

                    DataGridViewComboBoxColumn colCable = (DataGridViewComboBoxColumn)dataGridView2.Columns["cboCable"];
                    colCable.DataPropertyName = "Cable";
                    colCable.FlatStyle = FlatStyle.Standard;
                    colCable.DisplayMember = "Cable";
                    ((DataGridViewComboBoxColumn)dataGridView2.Columns["cboCable"]).DataSource = dtSource2;

                     dtCableSpec = new DataTable();
                     dtCableSpec.Columns.Add("InstType");
                     dtCableSpec.Columns.Add("Type");
                     dtCableSpec.Columns.Add("System");
                     dtCableSpec.Columns.Add("Cable");
                     List<Inst_Cable> result3;
                     using (TextReader fileReader = File.OpenText(DataSourceInstCable))
                     {
                         var csv = new CsvReader(fileReader);
                         csv.Configuration.HasHeaderRecord = false;
                         csv.Read();
                         result3 = csv.GetRecords<Inst_Cable>().ToList();

                     }

                     foreach (Inst_Cable IC in result3)
                     {
                         dtCableSpec.Rows.Add(new object[] { IC.InstType.Trim(), IC.Type.Trim(), IC.System.Trim(), IC.Cable.Trim() });
                     }

                    var rows = dtCableSpec.Select("InstType = " + "'" + InstType + "'") ?? null;

                    if (rows.Any())
                     {
                        dtTemp = rows.CopyToDataTable();
                        //dtTemp.Columns.Remove("InstType");
                        dataGridView2.DataSource = dtTemp;

                        foreach(DataGridViewRow dgvr in dataGridView2.Rows)
                        {
                            if (!dgvr.IsNewRow)
                            {
                                var Type = dgvr.Cells[1].EditedFormattedValue.ToString();
                                var System = dgvr.Cells[2].EditedFormattedValue.ToString();
                                var systemlist = result1.Where(x => x.Type.Equals(Type)).Select(x => x.System).ToList();
                                DataGridViewComboBoxCell cbCell = (DataGridViewComboBoxCell)dataGridView2.Rows[dgvr.Index].Cells["cboSystem"];

                                cbCell.DisplayMember = "System";
                                cbCell.DataSource = systemlist;
                            }
                            
                        }
                    }
                     else
                     {
                        dataGridView2.DataSource = dtTemp;
                     }
                }
                else dataGridView2.DataSource = dtTemp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView2.NotifyCurrentCellDirty(true);
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView2.Rows[e.RowIndex].Cells["cboSignal"];
            if (cb.EditedFormattedValue != null)
            {
                dtSystem = new DataTable();
                dtSystem.Columns.Add("System");

                List<Signal> result1;
                using (TextReader fileReader = File.OpenText(DataSourceSignal))
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Read();
                    result1 = csv.GetRecords<Signal>().ToList();

                }
                var systemlist = result1.Where(x => x.Type.Equals(cb.EditedFormattedValue.ToString())).Select(x => x.System).ToList();
                foreach (var S in systemlist)
                {
                    //dtSignal.Rows.Add(new object[] { S.Type.Trim() });
                    dtSystem.Rows.Add(new object[] { S.ToString() });
                }

                DataGridViewComboBoxCell cbCell = (DataGridViewComboBoxCell)dataGridView2.Rows[e.RowIndex].Cells["cboSystem"];
                cbCell.DataSource = systemlist;
                //ClearDgv2();
                //IsEditing = true;
            }
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
                if (!((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void SaveChanges_G2(string flg)
        {
            //if(flg == "delete")
            //{

            //   //DataTable deleteRows = dtCableSpec.AsEnumerable().Where(x => x.Field<string>("InstType").Equals(txtInstType.Text)).CopyToDataTable();
            //   //foreach (DataRow r in deleteRows.Rows)
            //   //{
            //   //    dtCableSpec.Rows.Remove(r);
            //   //}
            //    foreach (DataRow rowr in dtCableSpec.Rows)
            //    {
            //        if (rowr["InstType"].ToString() == txtInstType.Text)
            //            rowr.Delete();
            //    }
            //    dtCableSpec.AcceptChanges();
            //}
            //else
            //{
            if(dtCableSpec != null)
            {
                for (int i = dtCableSpec.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtCableSpec.Rows[i];
                    if (dr["InstType"].ToString() == txtInstType.Text.ToString())
                        dr.Delete();
                }
                //}


                dtCableSpec.AcceptChanges();
                dataGridView2.Update();
                //Saving data
                using (var writer = new StreamWriter(DataSourceInstCable))
                using (var csvWriter = new CsvWriter(writer))
                {
                    List<Inst_Cable> lst = new List<Inst_Cable>();
                    Inst_Cable ie = new Inst_Cable();
                    DataRow row;
                    foreach (DataGridViewRow dr in dataGridView2.Rows)
                    {
                        if (dr.Index != this.dataGridView2.NewRowIndex)
                        {

                            row = dtCableSpec.NewRow();
                            row[0] = txtInstType.Text.ToString();
                            for (int i = 1; i < dr.Cells.Count; i++)
                            {

                                row[i] = dr.Cells[i].EditedFormattedValue;
                            }

                            dtCableSpec.Rows.Add(row);

                        }
                    }
                    foreach (DataRow dr in dtCableSpec.Rows)
                    {
                        ie = new Inst_Cable();
                        ie.InstType = dr["InstType"].ToString().Trim();
                        ie.Type = dr["Type"].ToString().Trim();
                        ie.System = dr["System"].ToString().Trim();
                        ie.Cable = dr["Cable"].ToString().Trim();
                        lst.Add(ie);
                    }

                    csvWriter.WriteRecords(lst);
                    csvWriter.Flush();
                    writer.Flush();
                    writer.Close();
                }
                //BindGrid();
                BindGridView2(txtInstType.Text);
            }
            
        }

        private void DeleteSelectedRow_G2()
        {
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                dataGridView2.Rows.Remove(row);
                dataGridView2.Update();
            }

            SaveChanges_G2("delete");
            //MessageBox.Show("Deleted successfully.");

        }

        private void btnGv2Save_Click(object sender, EventArgs e)
        {
            try
            {
                   if (string.IsNullOrEmpty(txtInstType.Text)) return;
                dataGridView2.Update();
                if (!CheckGrid(dataGridView2, "cboSignal", "cboSystem", "cboCable")) return; 
                SaveChanges_G2("delete");
                if(sender != null && e != null)
                    MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnGv2Delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInstType.Text)) return;
            try
            {
                dataGridView2.Update();

                if (dataGridView2.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView2.SelectedRows.Count >=1)
                {

                    DeleteSelectedRow_G2();

                    MessageBox.Show("Deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtInstType_TextChanged(object sender, EventArgs e)
        {

        }

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;

                //if (dataGridView1.CurrentRow.Cells["Col4"].Value.ToString() == dataGridView1.CurrentCell.EditedFormattedValue.ToString())
                //{
                //    ClearDgv2();
                //    return;
                //}
                if (checkBox1.Checked) return; 
                if ((columnName == "Col3") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString())))
                {
                    txtInstType.Text = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                    BindGridView2(txtInstType.Text);
                }
            }
            //ClearDgv2
        }

        private bool CheckGrid(DataGridView dgv, string col1, string col2, string col3)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var c1 = String.IsNullOrWhiteSpace(dr.Cells[col1].FormattedValue.ToString().Trim());
                var c2 = String.IsNullOrWhiteSpace(dr.Cells[col2].FormattedValue.ToString().Trim());
                var c3 = String.IsNullOrWhiteSpace(dr.Cells[col3].FormattedValue.ToString().Trim());
                if (c1 && c2 && c3) continue;
                if (c1 || c2 || c3)
                {
                    MessageBox.Show("Data required at some rows. Could you please check?");
                    return false;
                }
            }
            return true;
        }

        private bool checkDuplicate()
        {
            List<string> data = new List<string>();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                var val = dr.Cells["Col3"].FormattedValue.ToString().Trim();
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

        private void dataGridView2_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
                if (!((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView2.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dataGridView2_Leave(object sender, EventArgs e)
        {
            btnGv2Save_Click(null, null);
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {   btnGv2Save_Click(null, null);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            if (!string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells["Col2"].FormattedValue.ToString().Trim()) && !string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells["Col1"].FormattedValue.ToString().Trim())
                )
            {
                if (dataGridView1.CurrentCell.OwningColumn.Name != "Col3")
                {
                    e.Cancel = true;
                }
                if (dataGridView1.CurrentCell.OwningColumn.Name == "Col3" && checkBox1.Checked)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;

                }
            }
            else
            {
                e.Cancel = false;
            } 
        }
        int Loaded = 0;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {  
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                TextBox tb = (TextBox)e.Control;
                tb.TextChanged += new EventHandler(tb_TextChanged);
            }
        }
        void tb_TextChanged(object sender, EventArgs e)
        {
            var enteredText = (sender as TextBox).Text;
            ClearDgv2();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {

                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    r.Cells["Col3"].Value = r.Cells["Col4"].Value;
                }


                //return;
            }
            ClearDgv2();
            btnGv2Save.Enabled = btnGv2Delete.Enabled = !checkBox1.Checked;
        }
    }
    public class InstrumentList
    {
        public string Classification_1 { get; set; }
        public string Classification_2 { get; set; }
        public string Classification_3 { get; set; }
    }
    public class InstrumentList1
    {
        public string Classification_1 { get; set; }
        public string Classification_2 { get; set; }
        public string Classification_3 { get; set; }
        public string Classification_4 { get; set; }
    }

    public class Signal
    {
        public string Type { get; set; }
        public string System { get; set; }
        public string AssignedCableDuct { get; set; }
    }

    public class Cable_List
    {
        public string TypeCL { get; set; }
        public string Cable { get; set; }
        public string Diameter { get; set; }
    }

    public class Inst_Cable
    {
        public string InstType { get; set; }
        public string Type { get; set; }
        public string System { get; set; }
        public string Cable { get; set; }
    }
}
