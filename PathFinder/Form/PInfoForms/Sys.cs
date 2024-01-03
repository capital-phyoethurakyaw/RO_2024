using CsvHelper;
//using DocumentFormat.OpenXml.Wordprocessing;
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
    public partial class Sys : System.Windows.Forms.Form
    {
        static string DataSourceSystemList = Entity.StaticCache.DataSourceSystemList;

        public Sys()
        {
            InitializeComponent();
        }

        DataTable dtSource;
        DataTable dtTemp;
        private void Sys_Load(object sender, EventArgs e)
        {
            BindData();
            //label5.Visible = false;
            //txtSystemTitle.Visible = false;
            //foreach(Control c in tableLayoutPanel8.Controls)
            //{
            //    c.Visible = false;
            //}
            //foreach (Control c in tableLayoutPanel9.Controls)
            //{
            //    c.Visible = false;
            //}
            //foreach (Control c in tableLayoutPanel10.Controls)
            //{
            //    c.Visible = false;
            //}
            //tableLayoutPanel8.Visible = false;
            //tableLayoutPanel9.Visible = false;
            //tableLayoutPanel10.Visible = false;
            //btnDelete2.Visible = false;
            //btnSave2.Visible = false;
        }

        private void BindData()
        {
            if (!File.Exists(DataSourceSystemList))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceSystemList + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("Title"); dtSource.Columns.Add("NW_W"); dtSource.Columns.Add("NW_D");
            dtSource.Columns.Add("NW_H"); dtSource.Columns.Add("NW_Check"); dtSource.Columns.Add("CPU_W");
            dtSource.Columns.Add("CPU_D"); dtSource.Columns.Add("CPU_H"); dtSource.Columns.Add("CPU_Check");
            dtSource.Columns.Add("RIO_W"); dtSource.Columns.Add("RIO_D"); dtSource.Columns.Add("RIO_H");
            dtSource.Columns.Add("RIO_Check"); dtSource.Columns.Add("PLC_Qty"); dtSource.Columns.Add("PLC_Sl");
            dtSource.Columns.Add("PLC_Sl_Spare"); dtSource.Columns.Add("PLC_DI"); dtSource.Columns.Add("PLC_DO");
            dtSource.Columns.Add("PLC_AI"); dtSource.Columns.Add("PLC_AI_C"); dtSource.Columns.Add("PLC_AO");
            dtSource.Columns.Add("RTD"); dtSource.Columns.Add("IO_Ratio");

            List<SystemInfo> result = null; ;
            try
            {
                using (TextReader fileReader = File.OpenText(DataSourceSystemList))
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Read();
                    result = csv.GetRecords<SystemInfo>().ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("access"))
                {
                    return;
                }
            }
            foreach (SystemInfo S in result)
            {
                dtSource.Rows.Add(new object[] { S.Title.Trim(), S.NW_W.Trim(), S.NW_D.Trim(), S.NW_H.Trim(), S.NW_Check.Trim()
                ,S.CPU_W.Trim(), S.CPU_D.Trim(), S.CPU_H.Trim(), S.CPU_Check.Trim()
                ,S.RIO_W.Trim(), S.RIO_D.Trim(), S.RIO_H.Trim(), S.RIO_Check.Trim()
                ,S.PLC_Qty.Trim(), S.PLC_Sl.Trim(), S.PLC_Sl_Spare.Trim()
                ,S.PLC_DI.Trim(),S.PLC_DO.Trim()
                , S.PLC_AI.Trim(), S.PLC_AI_C.Trim(),S.PLC_AO.Trim()
                ,S.RTD.Trim(), S.IO_Ratio.Trim()});
            }
            dtTemp = new DataTable();
            dtTemp.Columns.Add("Title");
            DataView dv = new DataView(dtSource);
            dtTemp = dv.ToTable(true, "Title");
            dataGridView1.DataSource = dtTemp;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;
                if ((columnName == "colTitle") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())))
                {
                    txtSystemTitle.Text = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    BindBottomPanels(txtSystemTitle.Text);
                }
            }

        }

        private void BindBottomPanels(string SystemTitle)
        {
            var rows = dtSource.Select("Title = " + "'" + SystemTitle + "'") ?? null;
            if (rows.Any())
            {
                txtNW_W.Text = rows[0]["NW_W"].ToString();
                txtNW_D.Text = rows[0]["NW_D"].ToString();
                txtNW_H.Text = rows[0]["NW_H"].ToString();
                if (rows[0]["NW_Check"].ToString() == "TRUE")
                    chNW.Checked = true;
                else chNW.Checked = false;

                txtCPU_W.Text = rows[0]["CPU_W"].ToString();
                txtCPU_D.Text = rows[0]["CPU_D"].ToString();
                txtCPU_H.Text = rows[0]["CPU_H"].ToString();
                if (rows[0]["CPU_Check"].ToString() == "TRUE")
                    chCPU.Checked = true;
                else chCPU.Checked = false;

                txtRIO_W.Text = rows[0]["RIO_W"].ToString();
                txtRIO_D.Text = rows[0]["RIO_D"].ToString();
                txtRIO_H.Text = rows[0]["RIO_H"].ToString();
                if (rows[0]["RIO_Check"].ToString() == "TRUE")
                    chRIO.Checked = true;
                else chRIO.Checked = false;

                txtPLC_Qty.Text = rows[0]["PLC_Qty"].ToString();
                txtPLC_Sl.Text = rows[0]["PLC_Sl"].ToString();
                txtPLC_Sl_Spare.Text = rows[0]["PLC_Sl_Spare"].ToString();

                txtPLC_DI.Text = rows[0]["PLC_DI"].ToString();
                txtPLC_DO.Text = rows[0]["PLC_DO"].ToString();
                txtPLC_AI.Text = rows[0]["PLC_AI"].ToString();
                txtPLC_AI_C.Text = rows[0]["PLC_AI_C"].ToString();
                txtPLC_AO.Text = rows[0]["PLC_AO"].ToString();
                txtRTD.Text = rows[0]["RTD"].ToString();
                txtIO_Ratio.Text = rows[0]["IO_Ratio"].ToString();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                if (!checkDuplicate()) return;
                SaveChanges("Save");
                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                
                SaveChanges("Save");

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
                dataGridView1.Update();
                DialogResult result = MessageBox.Show("Area you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (dataGridView1.SelectionMode == System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect && dataGridView1.SelectedRows.Count >= 1)
                    {

                        DeleteSelectedRow();

                        MessageBox.Show("Deleted successfully.");
                    }
                    else
                        MessageBox.Show("Please select at least one row to be deleted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveChanges(string Mode)
        {
            dataGridView1.Update();

            if (Mode == "Save")
            {
                //edit existing row
                foreach (DataRow dr in dtSource.Rows)
                {
                    dr["Title"] = dtTemp.Rows[dtSource.Rows.IndexOf(dr)]["Title"].ToString();
                }

                //add new row
                var newRowCount = dtTemp.Rows.Count - dtSource.Rows.Count;
                var loop = dtTemp.Rows.Count - newRowCount;
                if (newRowCount > 0)
                    for (int y = dtTemp.Rows.Count; y > loop; y--)
                    {
                        DataRow row = dtSource.NewRow();
                        row["Title"] = dtTemp.Rows[y - 1]["Title"].ToString();
                        dtSource.Rows.Add(row);
                    }
                dtSource.AcceptChanges();
            }
            else
            {
                List<DataRow> rowsToDelete = new List<DataRow>();
                var idsNotInB = dtSource.AsEnumerable().Select(r => r.Field<string>("Title"))
        .Except(dtTemp.AsEnumerable().Select(r => r.Field<string>("Title")));
                var TableC = (from row in dtSource.AsEnumerable()
                              join Title in idsNotInB
                              on row.Field<string>("Title") equals Title
                              select row);
                foreach (DataRow dr1 in dtSource.Rows)
                {
                    bool add = true;

                    foreach (var dr2 in TableC)
                    {
                        // Make sure the itemarray[x] is the proper indetifier
                        if (dr2[0].ToString() != dr1[0])
                        {
                            add = false;
                            // break;
                        }
                        else
                        {
                            add = true;
                            break;
                        }

                    }
                    if (add)
                    {
                        rowsToDelete.Add(dr1);
                    }
                }

                foreach (var r in rowsToDelete)
                    dtSource.Rows.Remove(r);
            }

            File.Delete(DataSourceSystemList);
            using (var writer = new StreamWriter(DataSourceSystemList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<SystemInfo> lst = new List<SystemInfo>();
                SystemInfo ie = new SystemInfo();

                foreach (DataRow dr in dtSource.Rows)
                {
                    ie = new SystemInfo();
                    ie.Title = dr["Title"].ToString().Trim();
                    ie.NW_W = dr["NW_W"].ToString().Trim();
                    ie.NW_D = dr["NW_D"].ToString().Trim();
                    ie.NW_H = dr["NW_H"].ToString().Trim();
                    ie.NW_Check = dr["NW_Check"].ToString().Trim();

                    ie.CPU_W = dr["CPU_W"].ToString().Trim();
                    ie.CPU_D = dr["CPU_D"].ToString().Trim();
                    ie.CPU_H = dr["CPU_H"].ToString().Trim();
                    ie.CPU_Check = dr["CPU_Check"].ToString().Trim();

                    ie.RIO_W = dr["RIO_W"].ToString().Trim();
                    ie.RIO_D = dr["RIO_D"].ToString().Trim();
                    ie.RIO_H = dr["RIO_H"].ToString().Trim();
                    ie.RIO_Check = dr["RIO_Check"].ToString().Trim();

                    ie.PLC_Qty = dr["PLC_Qty"].ToString().Trim();
                    ie.PLC_Sl = dr["PLC_Sl"].ToString().Trim();
                    ie.PLC_Sl_Spare = dr["PLC_Sl_Spare"].ToString().Trim();

                    ie.PLC_DI = dr["PLC_DI"].ToString().Trim();
                    ie.PLC_DO = dr["PLC_DO"].ToString().Trim();
                    ie.PLC_AI = dr["PLC_AI"].ToString().Trim();
                    ie.PLC_AI_C = dr["PLC_AI_C"].ToString().Trim();
                    ie.PLC_AO = dr["PLC_AO"].ToString().Trim();
                    ie.RTD = dr["RTD"].ToString().Trim();
                    ie.IO_Ratio = dr["IO_Ratio"].ToString().Trim();

                    lst.Add(ie);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindData();
            
        }

        private void DeleteSelectedRow()
        {
            var count = dataGridView1.SelectedRows.Count;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
                dataGridView1.Update();
                SaveChanges("Delete");
                SaveChanges2("Delete");
                //txtSystemTitle.Text = string.Empty;
            }

        }

        private void label13_Click(object sender, EventArgs e)
        {

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

        private void btnSave2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Update();
                SaveChanges2("Save");

                MessageBox.Show("Your changes have been successfully saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SaveChanges2(string mode)
        {

            for (int i = dtSource.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtSource.Rows[i];
                if (dr["Title"].ToString() == txtSystemTitle.Text.ToString())
                    dr.Delete();
            }
            dtSource.AcceptChanges();

            if (mode == "Save")
            {
                DataRow newRow = dtSource.NewRow();
                newRow["Title"] = txtSystemTitle.Text.ToString();
                newRow["NW_W"] = txtNW_W.Text.ToString();
                newRow["NW_D"] = txtNW_D.Text.ToString();
                newRow["NW_H"] = txtNW_H.Text.ToString();
                if (chNW.Checked == true)
                    newRow["NW_Check"] = "TRUE";
                else newRow["NW_Check"] = "FALSE";

                newRow["CPU_W"] = txtCPU_W.Text.ToString();
                newRow["CPU_D"] = txtCPU_D.Text.ToString();
                newRow["CPU_H"] = txtCPU_H.Text.ToString();
                if (chCPU.Checked == true)
                    newRow["CPU_Check"] = "TRUE";
                else newRow["CPU_Check"] = "FALSE";

                newRow["RIO_W"] = txtRIO_W.Text.ToString();
                newRow["RIO_D"] = txtRIO_D.Text.ToString();
                newRow["RIO_H"] = txtRIO_H.Text.ToString();
                if (chRIO.Checked == true)
                    newRow["RIO_Check"] = "TRUE";
                else newRow["RIO_Check"] = "FALSE";

                newRow["PLC_Qty"] = txtPLC_Qty.Text.ToString();
                newRow["PLC_Sl"] = txtPLC_Sl.Text.ToString();
                newRow["PLC_Sl_Spare"] = txtPLC_Sl_Spare.Text.ToString();

                newRow["PLC_DI"] = txtPLC_DI.Text.ToString();
                newRow["PLC_DO"] = txtPLC_DO.Text.ToString();
                newRow["PLC_AI"] = txtPLC_AI.Text.ToString();
                newRow["PLC_AI_C"] = txtPLC_AI_C.Text.ToString();
                newRow["PLC_AO"] = txtPLC_AO.Text.ToString();
                newRow["RTD"] = txtRTD.Text.ToString();
                newRow["IO_Ratio"] = txtIO_Ratio.Text.ToString();

                dtSource.Rows.Add(newRow);
                dtSource.AcceptChanges();
            }
            else
            {
                DataRow newRow = dtSource.NewRow();
                txtSystemTitle.Text = string.Empty;
                newRow["Title"] = string.Empty;
                newRow["NW_W"] = string.Empty;
                newRow["NW_D"] = string.Empty;
                newRow["NW_H"] = string.Empty;
                newRow["NW_Check"] = string.Empty;

                newRow["CPU_W"] = string.Empty;
                newRow["CPU_D"] = string.Empty;
                newRow["CPU_H"] = string.Empty;
                newRow["CPU_Check"] = string.Empty;

                newRow["RIO_W"] = string.Empty;
                newRow["RIO_D"] = string.Empty;
                newRow["RIO_H"] = string.Empty;
                newRow["RIO_Check"] = string.Empty;

                newRow["PLC_Qty"] = string.Empty;
                newRow["PLC_Sl"] = string.Empty;
                newRow["PLC_Sl_Spare"] = string.Empty;

                newRow["PLC_DI"] = string.Empty;
                newRow["PLC_DO"] = string.Empty;
                newRow["PLC_AI"] = string.Empty;
                newRow["PLC_AI_C"] = string.Empty;
                newRow["PLC_AO"] = string.Empty;
                newRow["RTD"] = string.Empty;
                newRow["IO_Ratio"] = string.Empty;

                dtSource.Rows.Add(newRow);
                dtSource.AcceptChanges();
            }

            using (var writer = new StreamWriter(DataSourceSystemList))
            using (var csvWriter = new CsvWriter(writer))
            {
                List<SystemInfo> lst = new List<SystemInfo>();
                SystemInfo ie = new SystemInfo();

                foreach (DataRow dr in dtSource.Rows)
                {
                    ie = new SystemInfo();
                    ie.Title = dr["Title"].ToString().Trim();
                    ie.NW_W = dr["NW_W"].ToString().Trim();
                    ie.NW_D = dr["NW_D"].ToString().Trim();
                    ie.NW_H = dr["NW_H"].ToString().Trim();
                    ie.NW_Check = dr["NW_Check"].ToString().Trim();

                    ie.CPU_W = dr["CPU_W"].ToString().Trim();
                    ie.CPU_D = dr["CPU_D"].ToString().Trim();
                    ie.CPU_H = dr["CPU_H"].ToString().Trim();
                    ie.CPU_Check = dr["CPU_Check"].ToString().Trim();

                    ie.RIO_W = dr["RIO_W"].ToString().Trim();
                    ie.RIO_D = dr["RIO_D"].ToString().Trim();
                    ie.RIO_H = dr["RIO_H"].ToString().Trim();
                    ie.RIO_Check = dr["RIO_Check"].ToString().Trim();

                    ie.PLC_Qty = dr["PLC_Qty"].ToString().Trim();
                    ie.PLC_Sl = dr["PLC_Sl"].ToString().Trim();
                    ie.PLC_Sl_Spare = dr["PLC_Sl_Spare"].ToString().Trim();

                    ie.PLC_DI = dr["PLC_DI"].ToString().Trim();
                    ie.PLC_DO = dr["PLC_DO"].ToString().Trim();
                    ie.PLC_AI = dr["PLC_AI"].ToString().Trim();
                    ie.PLC_AI_C = dr["PLC_AI_C"].ToString().Trim();
                    ie.PLC_AO = dr["PLC_AO"].ToString().Trim();
                    ie.RTD = dr["RTD"].ToString().Trim();
                    ie.IO_Ratio = dr["IO_Ratio"].ToString().Trim();

                    lst.Add(ie);

                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            BindBottomPanels(txtSystemTitle.Text);
        }

        //private void DeleteSelectedRow2(string SystemTitle)
        //{
        //    var rows = dtSource.Select("Title = " + "'" + SystemTitle + "'") ?? null;
        //    if (rows.Any())
        //    {
        //        foreach (var r in rows)
        //            dtSource.Rows.Remove(r);
        //    }
        //    SaveChanges2("Delete");
        //}

        private void btnDelete2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSystemTitle.Text))
                {
                    SaveChanges2("Delete");

                    MessageBox.Show("Deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool checkDuplicate()
        {
            List<string> data = new List<string>();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                var val = dr.Cells["colTitle"].FormattedValue.ToString().Trim();
                if (!string.IsNullOrEmpty(val)) data.Add(val);
            }

            var e_tri = data.GroupBy(w => w).Where(g => g.Count() > 1).Any();
            if (e_tri)
            {
                MessageBox.Show("There are some duplicated values. Please fix this.");
                return false;
            }
            return true;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells["colTitle"].FormattedValue.ToString().Trim()))
                e.Cancel = true;
            else
                e.Cancel = false;
        }
    }

    public class SystemInfo
    {
        public string Title { get; set; }
        public string NW_W { get; set; }
        public string NW_D { get; set; }
        public string NW_H { get; set; }
        public string NW_Check { get; set; }
        public string CPU_W { get; set; }
        public string CPU_D { get; set; }
        public string CPU_H { get; set; }
        public string CPU_Check { get; set; }
        public string RIO_W { get; set; }
        public string RIO_D { get; set; }
        public string RIO_H { get; set; }
        public string RIO_Check { get; set; }
        public string PLC_Qty { get; set; }
        public string PLC_Sl { get; set; }
        public string PLC_Sl_Spare { get; set; }
        public string PLC_DI { get; set; }
        public string PLC_DO { get; set; }
        public string PLC_AI { get; set; }
        public string PLC_AI_C { get; set; }
        public string PLC_AO { get; set; }
        public string RTD { get; set; }
        public string IO_Ratio { get; set; }
    }
}

