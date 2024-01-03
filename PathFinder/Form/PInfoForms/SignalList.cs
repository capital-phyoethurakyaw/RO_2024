using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RouteOptimizer.PInfoForms
{

    public partial class SignalList : System.Windows.Forms.Form
    {
        static string DataSourceSignal = Entity.StaticCache.DataSourceSignal;
        static string DataSourceCableDuctList = Entity.StaticCache.DataSourceCableDuctList;
        static string DataSourceCableDuctTypeList = Entity.StaticCache.DataSourceCableDuctTypeList;

        public SignalList()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        DataTable dtSource;
        DataTable dtSignal;
        DataTable dtPower;
        DataTable dtComm;
        DataTable dtEtc;
        DataTable dtCbo;

        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();
            this.dGVPower.DataError += dGVPower_DataError;
            this.dGVSignal.DataError += dGVSignal_DataError;
            this.dGVComm.DataError += dGVComm_DataError;
            this.dGVEtc.DataError += dGVEtc_DataError;
        }
        private void BindGrid()
        {
            if (!File.Exists(DataSourceSignal) || !File.Exists(DataSourceCableDuctList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceSignal + " & " + DataSourceCableDuctList + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("Type");
            dtSource.Columns.Add("Title");
            dtSource.Columns.Add("AssignedCableDuct");

            List<Signal_List> result;
            using (TextReader fileReader = File.OpenText(DataSourceSignal))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<Signal_List>().ToList();

            }
            foreach (Signal_List S in result)
            {
                dtSource.Rows.Add(new object[] { S.Type.Trim(), S.Title.Trim(), S.AssignedCableDuct.Trim() });
            }

            dtSignal = new DataTable(); dtSignal.Columns.Add("Type"); dtSignal.Columns.Add("Title"); dtSignal.Columns.Add("AssignedCableDuct");
            dtPower = new DataTable(); dtPower.Columns.Add("Type"); dtPower.Columns.Add("Title"); dtPower.Columns.Add("AssignedCableDuct");
            dtComm = new DataTable(); dtComm.Columns.Add("Type"); dtComm.Columns.Add("Title"); dtComm.Columns.Add("AssignedCableDuct");
            dtEtc = new DataTable(); dtEtc.Columns.Add("Type"); dtEtc.Columns.Add("Title"); dtEtc.Columns.Add("AssignedCableDuct");

            var SignalRow = dtSource.Select("Type = " + "'" + "Signal" + "'") ?? null;
            if (SignalRow.Any())
            {
                dtSignal = dtSource.Select("Type = " + "'" + "Signal" + "'").CopyToDataTable();
                dGVSignal.DataSource = dtSignal;
            }

            var PowerRow = dtSource.Select("Type = " + "'" + "Power" + "'") ?? null;
            if (PowerRow.Any())
            {
                dtPower = dtSource.Select("Type = " + "'" + "Power" + "'").CopyToDataTable();
                dGVPower.DataSource = dtPower;
            }

            var CommRow = dtSource.Select("Type = " + "'" + "Comm." + "'") ?? null;
            if (CommRow.Any())
            {
                dtComm = dtSource.Select("Type = " + "'" + "Comm." + "'").CopyToDataTable();
                dGVComm.DataSource = dtComm;
            }

            var EtcRow = dtSource.Select("Type = " + "'" + "Etc." + "'") ?? null;
            if (EtcRow.Any())
            {
                dtEtc = dtSource.Select("Type = " + "'" + "Etc." + "'").CopyToDataTable();
                dGVEtc.DataSource = dtEtc;
            }

            dtCbo = new DataTable();
            dtCbo.Columns.Add("AssignedCableDuct");
            List<CableDuctTypeList> result1;
            using (TextReader fileReader = File.OpenText(DataSourceCableDuctTypeList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result1 = csv.GetRecords<CableDuctTypeList>().ToList();

            }
            foreach (CableDuctTypeList CD in result1)
            {
                dtCbo.Rows.Add(new object[] { CD.CableDuctType.Trim()});
            }

            DataGridViewComboBoxColumn cboAssignedCableDuct = (DataGridViewComboBoxColumn)dGVSignal.Columns["cboAssignedCableDuct"];
            cboAssignedCableDuct.DataPropertyName = "AssignedCableDuct";
            cboAssignedCableDuct.FlatStyle = FlatStyle.Standard;
            cboAssignedCableDuct.DisplayMember = "AssignedCableDuct";
            ((DataGridViewComboBoxColumn)dGVSignal.Columns["cboAssignedCableDuct"]).DataSource = dtCbo;

            DataGridViewComboBoxColumn cboAssignedCableDuct1 = (DataGridViewComboBoxColumn)dGVPower.Columns["cboAssignedCableDuct1"];
            cboAssignedCableDuct1.DataPropertyName = "AssignedCableDuct";
            cboAssignedCableDuct1.FlatStyle = FlatStyle.Standard;
            cboAssignedCableDuct1.DisplayMember = "AssignedCableDuct";
            ((DataGridViewComboBoxColumn)dGVPower.Columns["cboAssignedCableDuct1"]).DataSource = dtCbo;

            DataGridViewComboBoxColumn cboAssignedCableDuct2 = (DataGridViewComboBoxColumn)dGVComm.Columns["cboAssignedCableDuct2"];
            cboAssignedCableDuct2.DataPropertyName = "AssignedCableDuct";
            cboAssignedCableDuct2.FlatStyle = FlatStyle.Standard;
            cboAssignedCableDuct2.DisplayMember = "AssignedCableDuct";
            ((DataGridViewComboBoxColumn)dGVComm.Columns["cboAssignedCableDuct2"]).DataSource = dtCbo;

            DataGridViewComboBoxColumn cboAssignedCableDuct3 = (DataGridViewComboBoxColumn)dGVEtc.Columns["cboAssignedCableDuct3"];
            cboAssignedCableDuct3.DataPropertyName = "AssignedCableDuct";
            cboAssignedCableDuct3.FlatStyle = FlatStyle.Standard;
            cboAssignedCableDuct3.DisplayMember = "AssignedCableDuct";
            ((DataGridViewComboBoxColumn)dGVEtc.Columns["cboAssignedCableDuct3"]).DataSource = dtCbo;

        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            try
            {
                dGVSignal.Update();
                dGVPower.Update();
                dGVComm.Update();
                dGVEtc.Update();
                SaveChanges();

                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dGVSignal.Update();
                dGVPower.Update();
                dGVComm.Update();
                dGVEtc.Update();
                if (!checkDuplicate()) return;
                if (!CheckGrid(dGVSignal, "colTitle", "cboAssignedCableDuct")) return;
                if (!CheckGrid(dGVPower, "colTitle1", "cboAssignedCableDuct1")) return;
                if (!CheckGrid(dGVComm, "colTitle2", "cboAssignedCableDuct2")) return;
                if (!CheckGrid(dGVEtc, "colTitle3", "cboAssignedCableDuct3")) return;

                
                //for (int i = dGVSignal.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dGVSignal.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dGVSignal.ClearSelection();
                //            //cell.Selected = true;
                //            return;
                //        }
                //    }
                //}
                //for (int i = dGVPower.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dGVPower.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dGVPower.ClearSelection();
                //           // cell.Selected = true;
                //            return;
                //        }
                //    }
                //}

                //foreach (DataGridViewRow dr in dGVComm.Rows)
                //{
                //    var title = String.IsNullOrWhiteSpace(dr.Cells["colTitle2"].FormattedValue.ToString().Trim());
                //    var AssignDuct = String.IsNullOrWhiteSpace(dr.Cells["cboAssignedCableDuct2"].FormattedValue.ToString().Trim());
                //    if (title && AssignDuct) continue;
                //    if (title || AssignDuct)
                //    {
                //        MessageBox.Show("Data required at some rows. Could you please check?");
                //        return;
                //    } 
                //}
                //for (int i = dGVComm.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dGVComm.Rows[i].Cells)
                //    {

                         
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?"); 
                //            return;
                //        }
                //    }
                //}
                //for (int i = dGVEtc.RowCount - 2; i >= 0; i--)
                //{
                //    foreach (DataGridViewCell cell in dGVEtc.Rows[i].Cells)
                //    {
                //        if (cell.Value == null || cell.Value == DBNull.Value || String.IsNullOrWhiteSpace(cell.Value.ToString()))
                //        {
                //            MessageBox.Show("Data required at some rows. Could you please check?");
                //            //dGVEtc.ClearSelection();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                dGVSignal.Update();
                dGVPower.Update();
                dGVComm.Update();
                dGVEtc.Update();

                if ((dGVSignal.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dGVSignal.SelectedCells.Count == 3) ||
                    (dGVPower.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dGVPower.SelectedCells.Count == 3) ||
                    (dGVComm.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dGVComm.SelectedCells.Count == 3) ||
                    (dGVEtc.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dGVEtc.SelectedCells.Count == 3))
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
            //Signal
            foreach (DataGridViewRow row in dGVSignal.SelectedRows)
            {
                dGVSignal.Rows.Remove(row);
                dGVSignal.Update();
                dtSignal.AcceptChanges();
            }
            //Power
            foreach (DataGridViewRow row in dGVPower.SelectedRows)
            {
                dGVPower.Rows.Remove(row);
                dGVPower.Update();
                dtPower.AcceptChanges();
            }
            //Comm.
            foreach (DataGridViewRow row in dGVComm.SelectedRows)
            {
                dGVComm.Rows.Remove(row);
                dGVComm.Update();
                dtComm.AcceptChanges();
            }
            //Etc.
            foreach (DataGridViewRow row in dGVEtc.SelectedRows)
            {
                dGVEtc.Rows.Remove(row);
                dGVEtc.Update();
                dtEtc.AcceptChanges();
            }

            SaveChanges();
        }

        private void SaveChanges()
        {
            File.Delete(DataSourceSignal);
            using (var writer = new StreamWriter(DataSourceSignal))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<Signal_List> lst = new List<Signal_List>();
                Signal_List sl = new Signal_List();
                //Signal
                foreach (DataRow dr in dtSignal.Rows)
                {
                    sl = new Signal_List();
                    sl.Type = "Signal";
                    sl.Title = dr["Title"].ToString().Trim();
                    sl.AssignedCableDuct = dr["AssignedCableDuct"].ToString().Trim();
                    lst.Add(sl);

                }
                //Power
                foreach (DataRow dr in dtPower.Rows)
                {
                    sl = new Signal_List();
                    sl.Type = "Power";
                    sl.Title = dr["Title"].ToString().Trim();
                    sl.AssignedCableDuct = dr["AssignedCableDuct"].ToString().Trim();
                    lst.Add(sl);

                }
                //Comm.
                foreach (DataRow dr in dtComm.Rows)
                {
                    sl = new Signal_List();
                    sl.Type = "Comm.";
                    sl.Title = dr["Title"].ToString().Trim();
                    sl.AssignedCableDuct = dr["AssignedCableDuct"].ToString().Trim();
                    lst.Add(sl);

                }
                //Etc.
                foreach (DataRow dr in dtEtc.Rows)
                {
                    sl = new Signal_List();
                    sl.Type = "Etc.";
                    sl.Title = dr["Title"].ToString().Trim();
                    sl.AssignedCableDuct = dr["AssignedCableDuct"].ToString().Trim();
                    lst.Add(sl);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindGrid();
        }

        private void dGVSignal_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dGVSignal.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dGVSignal.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dGVSignal.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dGVPower_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dGVPower.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dGVPower.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dGVPower.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dGVComm_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dGVComm.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dGVComm.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dGVComm.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dGVEtc_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dGVEtc.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dGVEtc.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dGVEtc.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private bool checkDuplicate()
        {
            List<string> data = new List<string>();
            foreach (DataGridViewRow dr in dGVPower.Rows)
            {
                var val = dr.Cells["colTitle1"].FormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val))
                    data.Add(val);

            }
            foreach (DataGridViewRow dr in dGVSignal.Rows)
            {
                var val = dr.Cells["colTitle"].FormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val))
                    data.Add(val);

            }
            foreach (DataGridViewRow dr in dGVComm.Rows)
            {
                var val = dr.Cells["colTitle2"].FormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val))
                    data.Add(val);

            }
            foreach (DataGridViewRow dr in dGVEtc.Rows)
            {
                var val = dr.Cells["colTitle3"].FormattedValue.ToString().Trim();
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
    }

    public class Signal_List
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string AssignedCableDuct { get; set; }
    }

    public class CableDuct_List
    {
        public string Type { get; set; }
        public string AssignedCableDuct { get; set; }
    }

    public class CableDuctTypeList
    {
        public string CableDuctType { get; set; }
    }
}
