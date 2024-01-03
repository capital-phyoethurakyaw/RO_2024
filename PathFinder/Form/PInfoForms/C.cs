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

namespace RouteOptimizer.PInfoForms
{
    public partial class C : System.Windows.Forms.Form
    {
        static string DataSourceCableList = Entity.StaticCache.DataSourceCableList;
        public C()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        DataTable dtSource;

        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();
        }

        public void PutDecimal(DataGridView dgv, List<string> lst)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    if (lst.Contains(dc.OwningColumn.Name))
                    {
                        dc.Value = String.Format("{0:0.0}", Convert.ToDouble(dc.Value));
                    }
                }
            }
        }

        private void BindGrid()
        {
            if (!File.Exists(DataSourceCableList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceCableList + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("Col1");
            dtSource.Columns.Add("Col2");
            dtSource.Columns.Add("Col3");

            List<CableList> result;
            using (TextReader fileReader = File.OpenText(DataSourceCableList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<CableList>().ToList();

            }

            int i = 0;
            foreach (CableList cl in result)
            {
                dtSource.Rows.Add(new object[] { cl.Title.Trim(), cl.Type.Trim(), cl.Diameter.Trim() });
            }
            dataGridView1.DataSource = dtSource;

            PutDecimal(dataGridView1, new List<string> { "Col3" });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                if (!checkDuplicate()) return;
                if (!CheckGrid(dataGridView1, "Col1", "Col2", "Col3")) return;
                //for (int i = dataGridView1.RowCount - 2; i >= 0; i--)
                //{
                //    foreach(DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dataGridView1.ClearSelection();
                //            //cell.Selected = true;
                //            return;
                //        }
                //    }
                //}

                //var duplicates = dtSource.AsEnumerable()
                //    .GroupBy(r => new { Col1 = r.Field<string>("Col1"), Col2 = r.Field<string>("Col2"), Col3 = r.Field<string>("Col3") }).Select(g => g.Count());
                //if(duplicates.Contains())
                //.Select(grp => new
                //{
                //    Count = grp.Count()
                //});
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
            File.Delete(DataSourceCableList);
            using (var writer = new StreamWriter(DataSourceCableList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<CableList> lst = new List<CableList>();
                CableList ie = new CableList();

                foreach (DataRow dr in dtSource.Rows)
                {
                    ie = new CableList();
                    ie.Title = dr["Col1"].ToString().Trim();
                    ie.Type = dr["Col2"].ToString().Trim();
                    ie.Diameter = dr["Col3"].ToString().Trim();
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

                if (dataGridView1.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView1.SelectedCells.Count >= 3)
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
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
                dataGridView1.Update();
            }

            SaveChanges();
        }

        private bool CheckGrid(DataGridView dgv, string col1, string col2, string col3)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                var type = String.IsNullOrWhiteSpace(dr.Cells[col1].FormattedValue.ToString().Trim());
                var title = String.IsNullOrWhiteSpace(dr.Cells[col2].FormattedValue.ToString().Trim());
                var diameter = String.IsNullOrWhiteSpace(dr.Cells[col3].FormattedValue.ToString().Trim());
                if (type && title && diameter) continue;
                if (type || title || diameter)
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
                var val = dr.Cells["Col2"].FormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val))  data.Add(val); 
            }

            var e_tri = data.GroupBy(w => w).Where(g => g.Count() > 1).Any();
            if (e_tri)
            {
                MessageBox.Show("There are some duplicated values. Please fix this.");
                return false;
            }
            return true;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

    }
    public class CableList
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Diameter { get; set; }
    }
}


