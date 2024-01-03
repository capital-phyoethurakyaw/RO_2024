using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RouteOptimizer;
using System.Collections;

namespace RouteOptimizer.PInfoForms
{
    public partial class Mccpkg : System.Windows.Forms.Form
    {
        static string DataSourceMccPkgList = Entity.StaticCache.DataSourceMccPkgList;
        static string DataSourceMccPkgCombo = Entity.StaticCache.DataSourceMccPkgCombo;
        static string DataSourceCableList = Entity.StaticCache.DataSourceCableList;
        static string DataSourceSignal = Entity.StaticCache.DataSourceSignal;
        static string DataSourceMCCManager = Entity.StaticCache.DataSourceMCCManager;

        static string SelectedRow = string.Empty;
        public Mccpkg()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }
        DataTable dtSource, dtcs, dtMCCManager, dtTemp, dtSignal, dtTemp1;

        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();
            BindCbo();
            dtTemp = new DataTable();
            dtTemp.Columns.Add("Title");
            dtTemp.Columns.Add("Description");
            dtTemp.Columns.Add("CableSpecifications");
            dtTemp.Columns.Add("Status");
            dtTemp.Columns.Add("TagNo");
            dtTemp.Columns.Add("Signal");
            dataGridView2.DataSource = dtTemp;
        }

        private void BindGrid()
        {
            if (!File.Exists(DataSourceMccPkgList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceMccPkgList + "&" + DataSourceMccPkgCombo + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("Col1");
            dtSource.Columns.Add("Col2");

            txtCableSpec.Text = string.Empty;
            List<MccPkgList> result;
            using (TextReader fileReader = File.OpenText(DataSourceMccPkgList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<MccPkgList>().ToList();

            }
            int i = 0;

            //Get MCCManager.csv data to datatable
            dtMCCManager = new DataTable();
            //dtMCCManager.Columns.Add("GUID");
            dtMCCManager.Columns.Add("Title");
            dtMCCManager.Columns.Add("Description");
            dtMCCManager.Columns.Add("CableSpecifications");
            dtMCCManager.Columns.Add("Status");
            dtMCCManager.Columns.Add("TagNo");
            dtMCCManager.Columns.Add("Signal");

            foreach (MccPkgList mc in result)
            {
                dtSource.Rows.Add(new object[] { mc.Title.Trim(), mc.Description.Trim() });
                dtMCCManager.Rows.Add(new object[] { mc.Title.Trim(), mc.Description.Trim(), mc.CableSpecifications.Trim(), mc.Status.Trim(), mc.TagNo.Trim(), mc.Signal.Trim() });
            }
            RemoveDuplicateRows(dtSource, "Col1");
            dtTemp1 = new DataTable();
            dtTemp1 = dtSource.Copy();
            dataGridView1_1.DataSource = dtSource;
        }
        private bool checkDuplicate()
        {
            List<string> data = new List<string>();
            foreach(DataGridViewRow dr in dataGridView1_1.Rows)
            {
                var val = dr.Cells["Col1"].FormattedValue.ToString().Trim();
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1_1.Update();
                if (!checkDuplicate()) return;
                if (!CheckGrid(dataGridView1_1, "Col1", "Col2", string.Empty, string.Empty,"1")) return;
                //for (int i = dataGridView1_1.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dataGridView1_1.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dataGridView1_1.ClearSelection();
                //            //cell.Selected = true;
                //            return;
                //        }
                //    }
                //}
                SaveChanges();

                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SaveChanges()
        {
            var newRowCount = dtSource.Rows.Count - dtTemp1.Rows.Count;
            var loop = dtSource.Rows.Count - newRowCount;
            if (newRowCount > 0)
                for (int y = dtTemp1.Rows.Count; y >= loop; y--)
                {
                    DataRow row = dtMCCManager.NewRow();
                    row["Title"] = dtSource.Rows[y]["Col1"].ToString();
                    row["Description"] = dtSource.Rows[y]["Col2"].ToString();
                    dtMCCManager.Rows.Add(row);
                }
            dtMCCManager.AcceptChanges();

            File.Delete(DataSourceMccPkgList);
            using (var writer = new StreamWriter(DataSourceMccPkgList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<MccPkgList> lst = new List<MccPkgList>();
                MccPkgList mt = new MccPkgList();

                foreach (DataRow dr in dtMCCManager.Rows)
                {
                    mt = new MccPkgList();
                    //mt.GUID = dr["GUID"].ToString().Trim();
                    mt.Title = dr["Title"].ToString().Trim();
                    mt.Description = dr["Description"].ToString().Trim();
                    mt.CableSpecifications = dr["CableSpecifications"].ToString().Trim();
                    mt.Status = dr["Status"].ToString().Trim();
                    mt.TagNo = dr["TagNo"].ToString().Trim();
                    mt.Signal = dr["Signal"].ToString().Trim();
                    lst.Add(mt);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindGrid();
            BindMCCType(txtCableSpec.Text);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1_1.SelectedRows.Count == 0 || dataGridView1_1.SelectedRows[0].Cells["Col1"].Value == null)
                {
                    return;
                }
                dataGridView1_1.Update();

                if (dataGridView1_1.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView1_1.SelectedCells.Count == 2)
                {
                    DeleteSelectedRow();
                    MessageBox.Show("Deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DeleteSelectedRow()
        {
            if (dataGridView1_1.CurrentRow.Cells["Col1"].Value == null)
            {
                return;
            }
            string selectedValue = string.Empty;
            foreach (DataGridViewRow row in dataGridView1_1.SelectedRows)
            {
                selectedValue = row.Cells["Col1"].Value.ToString();
            }


            for (int i = dtMCCManager.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtMCCManager.Rows[i];
                if (dr["Title"].ToString() == selectedValue)
                    dr.Delete();
            }
            dtMCCManager.AcceptChanges();

            using (var writer = new StreamWriter(DataSourceMccPkgList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<MccPkgList> lst = new List<MccPkgList>();
                MccPkgList mt = new MccPkgList();

                foreach (DataRow dr in dtMCCManager.Rows)
                {
                    mt = new MccPkgList();
                    //mt.GUID = dr["GUID"].ToString().Trim();
                    mt.Title = dr["Title"].ToString().Trim();
                    mt.Description = dr["Description"].ToString().Trim();
                    mt.CableSpecifications = dr["CableSpecifications"].ToString().Trim();
                    mt.Status = dr["Status"].ToString().Trim();
                    mt.TagNo = dr["TagNo"].ToString().Trim();
                    mt.Signal = dr["Signal"].ToString().Trim();
                    lst.Add(mt);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindGrid();
            BindMCCType(txtCableSpec.Text);
        }

        private void DeleteSelectedRow2()
        {
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                dataGridView2.Rows.Remove(row);
                dataGridView2.Update();
            }

            MCCTypeSaveChanges("Delete");
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (dataGridView1_1.CurrentRow != null && dataGridView1_1.CurrentRow.Index >= 0 && dtMCCManager.Rows.Count > 0)
            {
                DataTable dtresult = new DataTable();
                try
                {
                    txtCableSpec.Text = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                    BindMCCType(dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString());


                }
                catch (Exception ex)
                {
                    dtTemp.Rows.Clear();
                    dataGridView2.DataSource = dtTemp;
                }
            }
        }

        private void BindMCCType(string CableSpec)
        {
            var rows = dtMCCManager.Select("Title = " + "'" + CableSpec + "'") ?? null;
            if (rows.Any())
            {
                dtTemp = rows.CopyToDataTable();
                dtTemp.AcceptChanges();
                dataGridView2.DataSource = dtTemp;
            }
            else
            {
                dtTemp.Rows.Clear();
                dataGridView2.DataSource = dtTemp;
            }
        }

        //For Gridview2 (MCC Type)
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1_1.CurrentRow != null && dataGridView1_1.CurrentRow.Index >= 0 && dtMCCManager.Rows.Count > 0)
            {
                DataTable dtresult = new DataTable();
                try
                {
                    txtCableSpec.Text = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                    BindMCCType(dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString()); 
                }
                catch (Exception ex)
                {
                    dtTemp.Rows.Clear();
                    dataGridView2.DataSource = dtTemp;
                }
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView1_1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1_1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1_1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
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
        }

        private void btnMCCType_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateDG2();
                if (dataGridView2.SelectedRows.Count == 0 || dataGridView2.SelectedRows[0].Cells[1].Value == null)
                {
                    return;
                }
                dataGridView2.Update();
               
                if (dataGridView2.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView2.SelectedCells.Count == 7)
                {

                    DeleteSelectedRow2();

                    MessageBox.Show("Deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BindCbo()
        {
            try
            {
                if (!File.Exists(DataSourceCableList) || !File.Exists(DataSourceSignal))
                {
                    MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceCableList + " & " + DataSourceSignal + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                    return;
                }

                //CableSpecification
                dtcs = new DataTable();
                dtcs.Columns.Add("Type");
                dtcs.Columns.Add("colCableSpecifications");

                List<Cable_List> result;
                using (TextReader fileReader = File.OpenText(DataSourceCableList))
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Read();
                    result = csv.GetRecords<Cable_List>().ToList();

                }
                int i = 0;
                foreach (Cable_List mc in result)
                {
                    dtcs.Rows.Add(new object[] { mc.TypeCL.Trim(), mc.Cable.Trim() });
                }

                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dataGridView2.Columns["colCableSpecifications"];
                col.DataPropertyName = "CableSpecifications";
                col.FlatStyle = FlatStyle.Standard;
                col.DisplayMember = "colCableSpecifications";
                ((DataGridViewComboBoxColumn)dataGridView2.Columns["colCableSpecifications"]).DataSource = dtcs;

                //Signal
                dtSignal = new DataTable();
                dtSignal.Columns.Add("Type");
                dtSignal.Columns.Add("Signal");

                List<Signal_List> result1;
                using (TextReader fileReader = File.OpenText(DataSourceSignal))
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Read();
                    result1 = csv.GetRecords<Signal_List>().ToList();

                }
                foreach (Signal_List S in result1)
                {
                    dtSignal.Rows.Add(new object[] { S.Type.Trim(), S.Title.Trim() });
                }

                DataGridViewComboBoxColumn ColSignal = (DataGridViewComboBoxColumn)dataGridView2.Columns["ColSignal"];
                ColSignal.DataPropertyName = "Signal";
                ColSignal.FlatStyle = FlatStyle.Standard;
                ColSignal.DisplayMember = "Signal";
                ((DataGridViewComboBoxColumn)dataGridView2.Columns["ColSignal"]).DataSource = dtSignal;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private bool CheckGrid(DataGridView dgv, string col1, string col2)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var title = String.IsNullOrWhiteSpace(dr.Cells[col1].FormattedValue.ToString().Trim());
                var AssignDuct = String.IsNullOrWhiteSpace(dr.Cells[col2].FormattedValue.ToString().Trim());
                if (title && AssignDuct) continue;
                if (title || AssignDuct)
                {
                    MessageBox.Show("Data required at some rows. Could you please check?");
                    return false;
                }
            }
            return true;
        }

        private void dataGridView1_1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(dataGridView1_1.CurrentRow.Cells["Col1"].FormattedValue.ToString().Trim())
                && !string.IsNullOrEmpty(dataGridView1_1.CurrentRow.Cells["Col2"].FormattedValue.ToString().Trim()))
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UpdateDG2()
        {
            //foreach (DataGridViewRow dr in dataGridView2.Rows)
            //{
            //    dr.Cells["colTitle"].Value = dataGridView1_1.CurrentCell.OwningRow.Cells["col1"].Value.ToString();
            //    dr.Cells["colDescription"].Value = dataGridView1_1.CurrentCell.OwningRow.Cells["col2"].Value.ToString();
            //}
            dataGridView2.Update();
        }
        private void btnMCCType_Save_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateDG2();
                if (!CheckGrid(dataGridView2, "colCableSpecifications", "ColStatus", "ColTagNo", "ColSignal", "2")) return;
                //for (int i = dataGridView2.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dataGridView2.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dataGridView2.ClearSelection();
                //            //cell.Selected = true;
                //            return;
                //        }
                //    }
                //}
                MCCTypeSaveChanges("Save");

                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MCCTypeSaveChanges(string mode)
        {
            if (mode == "Save")
            {
                string forDelete = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                //File.Delete(DataSourceMccPkgList);
                for (int i = dtMCCManager.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtMCCManager.Rows[i];
                    if (dr["Title"].ToString() == forDelete)
                        dr.Delete();
                }
                dtMCCManager.AcceptChanges();

                dtTemp = dataGridView2.DataSource as DataTable;
                foreach (DataRow row in dtTemp.Rows)
                {
                    //row["GUID"] = Guid.NewGuid();
                    row["Title"] = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                    row["Description"] = dataGridView1_1.CurrentRow.Cells["Col2"].Value.ToString();
                }
                dtTemp.AcceptChanges();

                foreach (DataRow dr in dtTemp.Rows)
                {
                    dtMCCManager.Rows.Add(dr.ItemArray);
                }
                dtMCCManager.AcceptChanges();
            }
            else
            {
                string forDelete = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                //File.Delete(DataSourceMccPkgList);
                for (int i = dtMCCManager.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtMCCManager.Rows[i];
                    if (dr["Title"].ToString() == forDelete)
                        dr.Delete();
                }
                dtMCCManager.AcceptChanges();

                dtTemp = dataGridView2.DataSource as DataTable;
                //And u can also add a condition,while fetching the data,
                foreach (DataRow row in dtTemp.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        row["Title"] = dataGridView1_1.CurrentRow.Cells["Col1"].Value.ToString();
                        row["Description"] = dataGridView1_1.CurrentRow.Cells["Col2"].Value.ToString();
                    }
                }
                dtTemp.AcceptChanges();

                foreach (DataRow dr in dtTemp.Rows)
                {
                    dtMCCManager.Rows.Add(dr.ItemArray);
                }
                dtMCCManager.AcceptChanges();
            }


            using (var writer = new StreamWriter(DataSourceMccPkgList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<MccPkgList> lst = new List<MccPkgList>();
                MccPkgList mt = new MccPkgList();

                foreach (DataRow dr in dtMCCManager.Rows)
                {
                    mt = new MccPkgList();
                    //mt.GUID = dr["GUID"].ToString().Trim();
                    mt.Title = dr["Title"].ToString().Trim();
                    mt.Description = dr["Description"].ToString().Trim();
                    mt.CableSpecifications = dr["CableSpecifications"].ToString().Trim();
                    mt.Status = dr["Status"].ToString().Trim();
                    mt.TagNo = dr["TagNo"].ToString().Trim();
                    mt.Signal = dr["Signal"].ToString().Trim();
                    lst.Add(mt);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindGrid();
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

        private bool CheckGrid(DataGridView dgv, string col1, string col2, string col3, string col4, string flg)
        {
            if(flg == "1")
            {
                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    var c1 = String.IsNullOrWhiteSpace(dr.Cells[col1].FormattedValue.ToString().Trim());
                    var c2 = String.IsNullOrWhiteSpace(dr.Cells[col2].FormattedValue.ToString().Trim());
                    if (c1 && c2) continue;
                    if (c1 || c2)
                    {
                        MessageBox.Show("Data required at some rows. Could you please check?");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    var c1 = String.IsNullOrWhiteSpace(dr.Cells[col1].FormattedValue.ToString().Trim());
                    var c2 = String.IsNullOrWhiteSpace(dr.Cells[col2].FormattedValue.ToString().Trim());
                    var c3 = String.IsNullOrWhiteSpace(dr.Cells[col3].FormattedValue.ToString().Trim());
                    var c4 = String.IsNullOrWhiteSpace(dr.Cells[col3].FormattedValue.ToString().Trim());
                    if (c1 && c2 && c3 && c4) continue;
                    if (c1 || c2 || c3 || c4)
                    {
                        MessageBox.Show("Data required at some rows. Could you please check?");
                        return false;
                    }
                }
                return true;
            }
            
        }
    }



    public class MccPkgList
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CableSpecifications { get; set; }
        public string Status { get; set; }
        public string TagNo { get; set; }
        public string Signal { get; set; }
    }

    public class MCCType
    {
        // public string GUID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CableSpecifications { get; set; }
        public string Status { get; set; }
        public string TagNo { get; set; }
        public string Signal { get; set; }
    }
}
