using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RouteOptimizer;

namespace RouteOptimizer.PInfoForms
{
    public partial class Cd : System.Windows.Forms.Form
    {
        static string DataSourceCableList = Entity.StaticCache.DataSourceCableList;
        static string DataSourceCableDuctList = Entity.StaticCache.DataSourceCableDuctList;
        static string DataSourceCableDuctCableList = Entity.StaticCache.DataSourceCableDuctCableList;
        static string DataSourceCableDuctTypeList = Entity.StaticCache.DataSourceCableDuctTypeList;  //Upper table data 07/11/2023 by saw
        public Cd()
        {
            InitializeComponent();
            this.Load += IS_Load;

        }
        DataTable dtSourceType; //Upper table data 07/11/2023 by saw
        DataTable dtSource; //CableDuctList

        DataTable dtCale; //Cable
        DataTable dtCableSpec; //Gridview2

        private void IS_Load(object sender, EventArgs e)
        {
            //this.dataGridView1.ReadOnly = true;
            BindGrid();
            this.dataGridView1.CellClick += DataGridView1_CellClick;
            
            // this.dataGridView1.KeyDown += DataGridView1_KeyDown;
        }

        public void PutDecimal(DataGridView dgv, List<string> lst)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    if (lst.Contains(dc.OwningColumn.Name))
                    {
                        dc.Value = String.Format("{0:0,000.0}", Convert.ToDouble(dc.Value));
                    }
                }
            }
        }



        private void BindGrid()
        {
            if (!File.Exists(DataSourceCableDuctTypeList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceCableDuctTypeList + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }

            dtSource = new DataTable();
            dtSource.Columns.Add("Type");

            List<CableDuctTypeList> result;
            using (TextReader fileReader = File.OpenText(DataSourceCableDuctTypeList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<CableDuctTypeList>().ToList();
            }
            foreach (CableDuctTypeList type in result)
            {
                dtSource.Rows.Add(new object[] { type.CableDuctType.Trim() });
            }
            //colType.ReadOnly = true;
            dataGridView1.DataSource = dtSource;

            //dataGridView1.Rows[2].ReadOnly = true;
            //dataGridView1.ReadOnly = true;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (dr.Index > -1)
                {
                    //if (dr.Cells[0].Value != null || !string.IsNullOrEmpty(dr.Cells[0].EditedFormattedValue.ToString()))
                    //    dr.Cells[0].ReadOnly = true;
                    //else
                    //{
                    //    dr.Cells[0].ReadOnly = false;
                    //    dr.Cells[0] = false;
                    //}
                }
            }
            dataGridView1.RefreshEdit();
            //dataGridView1.Refresh();

        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != -1)
                {
                    string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;
                    if ((columnName == "colType") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString())))
                    {
                        txtCdType.Text = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                        BindGridView2(txtCdType.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BindGridView2(string CableType)
        {
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("Title");
            dtTemp.Columns.Add("Type");
            dtTemp.Columns.Add("Width");
            dtTemp.Columns.Add("Height");
            if (!File.Exists(DataSourceCableDuctList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceCableDuctList + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }

            if (!String.IsNullOrWhiteSpace(CableType))
            {
                try
                {
                    dtCableSpec = new DataTable();
                    dtCableSpec.Columns.Add("Title");
                    dtCableSpec.Columns.Add("Type");
                    dtCableSpec.Columns.Add("Width");
                    dtCableSpec.Columns.Add("Height");

                    List<CableDuctList> result2;
                    using (TextReader fileReader = File.OpenText(DataSourceCableDuctList))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = false;
                        csv.Read();
                        result2 = csv.GetRecords<CableDuctList>().ToList();

                    }
                    foreach (CableDuctList Cd in result2)
                    {
                        dtCableSpec.Rows.Add(new object[] { Cd.Title.Trim(), Cd.Type.Trim(), Cd.Width.Trim(), Cd.Height.Trim() });
                    }
                    if (dtCableSpec.Rows.Count > 0)
                    {
                        var rows = dtCableSpec.Select("Type = " + "'" + CableType + "'") ?? null;
                        if (rows.Any())
                        {
                            dtTemp = rows.CopyToDataTable();
                            dGVCdCable.DataSource = dtTemp;
                        }
                        else
                        {
                            dGVCdCable.DataSource = dtTemp;
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            PutDecimal(dGVCdCable, new List<string> { "Width", "Height"});
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                if (!checkDuplicate(dataGridView1, "colType")) return;
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
            File.Delete(DataSourceCableDuctTypeList);
            using (var writer = new StreamWriter(DataSourceCableDuctTypeList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<CableDuctTypeList> lst = new List<CableDuctTypeList>();
                CableDuctTypeList ie = new CableDuctTypeList();

                foreach (DataRow dr in dtSource.Rows)
                {
                    ie = new CableDuctTypeList();
                    ie.CableDuctType = dr["Type"].ToString().Trim();
                    lst.Add(ie);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                dGVCdCable.Update();
                if (dataGridView1.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView1.SelectedRows.Count >=1)
                {

                    DialogResult result = MessageBox.Show("Area you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo);
                    if(result == DialogResult.Yes)
                    {
                        DeleteSelectedRow();
                        txtCdType.Text = string.Empty;
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
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
               
                for (int i = dGVCdCable.RowCount - 2; i >= 0; i--)
                {
                    var deleterows = dGVCdCable.Rows[i];
                    if (!row.IsNewRow) 
                    {
                        if (row.Cells[0].EditedFormattedValue.ToString().Equals(deleterows.Cells[1].EditedFormattedValue.ToString()))
                        {
                            dGVCdCable.Rows.Remove(deleterows);
                            dGVCdCable.Update();
                        }
                        
                    }
                }
                dataGridView1.Rows.Remove(row);
                dataGridView1.Update();
                
            }

            SaveChanges();
            SaveChanges_G2();
        }


        private void btnGv2Delete_Click(object sender, EventArgs e)
        {
            try
            {
                dGVCdCable.Update();

                if (dGVCdCable.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dGVCdCable.SelectedRows.Count >= 1)
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

        private void btnGv2Save_Click(object sender, EventArgs e)
        {
            try
            {
                dGVCdCable.Update();
                if (!checkDuplicate(dGVCdCable, "colTitle")) return;
                if (!CheckGrid(dGVCdCable, "colTitle", "colWidth", "colHeight")) return;
                //for (int i = dGVCdCable.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dGVCdCable.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dGVCdCable.ClearSelection();
                //            //cell.Selected = true;
                //            return;
                //        }
                //    }
                //}
                SaveChanges_G2();
                if (sender != null)
                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveChanges_G2()
        {
            if(dtCableSpec != null)
            {
                for (int i = dtCableSpec.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtCableSpec.Rows[i];
                    if (dr["Type"].ToString() == txtCdType.Text.ToString())
                        dr.Delete();
                }
                dtCableSpec.AcceptChanges();

                using (var writer = new StreamWriter(DataSourceCableDuctList))
                using (var csvWriter = new CsvWriter(writer))
                {
                    List<CableDuctList> lst = new List<CableDuctList>();
                    CableDuctList ie = new CableDuctList();

                    DataRow row;
                    foreach (DataGridViewRow dr in dGVCdCable.Rows)
                    {
                        if (dr.Index != this.dGVCdCable.NewRowIndex)
                        {

                            row = dtCableSpec.NewRow();

                            for (int i = 0; i < dr.Cells.Count; i++)
                            {

                                row[i] = dr.Cells[i].EditedFormattedValue;
                            }
                            row[1] = txtCdType.Text.ToString();
                            dtCableSpec.Rows.Add(row);

                        }
                    }
                    foreach (DataRow dr in dtCableSpec.Rows)
                    {
                        ie = new CableDuctList();
                        ie.Title = dr["Title"].ToString().Trim();
                        ie.Type = dr["Type"].ToString().Trim();
                        ie.Width = dr["Width"].ToString().Trim();
                        ie.Height = dr["Height"].ToString().Trim();
                        lst.Add(ie);
                    }
                    csvWriter.WriteRecords(lst);
                    csvWriter.Flush();
                    writer.Flush();
                    writer.Close();
                }
                BindGridView2(txtCdType.Text);
            }
            
        }

        private void DeleteSelectedRow_G2()
        {
            foreach (DataGridViewRow row in dGVCdCable.SelectedRows)
            {
                dGVCdCable.Rows.Remove(row);
                dGVCdCable.Update();
            }

            SaveChanges_G2();
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

        private bool CheckGrid(DataGridView dgv, string col1, string col2, string col3)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var title = String.IsNullOrWhiteSpace(dr.Cells[col1].EditedFormattedValue.ToString().Trim());
                var width = String.IsNullOrWhiteSpace(dr.Cells[col2].EditedFormattedValue.ToString().Trim());
                var height = String.IsNullOrWhiteSpace(dr.Cells[col3].EditedFormattedValue.ToString().Trim());
                if (title && width && height) continue;
                if (title || width || height)
                {
                    MessageBox.Show("Data required at some rows. Could you please check?");
                    return false;
                }
            }
            return true;
        }
        private bool checkDuplicate(DataGridView dgv,string Col1)
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

        private void dGVCdCable_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public class CableDuctList
        {
            public string Title { get; set; }
            public string Type { get; set; }
            public string Width { get; set; }
            public string Height { get; set; }

        }

        public class CableDuctCableList
        {
            public string CableDuctType { get; set; }
            public string Cable { get; set; }
        }

        public class CableDuctTypeList
        {
            public string CableDuctType { get; set; }
        }

        private void dGVCdCable_Leave(object sender, EventArgs e)
        {
            btnGv2Save_Click(null,null);
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnGv2Save_Click(null, null);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (e.RowIndex == dataGridView1.Rows.Count - 2)
            { if (!string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells["colType"].FormattedValue.ToString().Trim()))
                    e.Cancel = true;
                else
                    e.Cancel = false;
            }
        }
    }
}
