using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using RouteOptimizer;
using System.Collections;
using BrightIdeasSoftware;

namespace RouteOptimizer.PInfoForms
{
    public partial class PI : System.Windows.Forms.Form
    {
        static string DataSourcePSCombo = Entity.StaticCache.DataSourcePSCombo;
        static string DataSourcePSCombo1 = Entity.StaticCache.DataSourcePSCombo1;
        static string DataSourcePSCombo2 = Entity.StaticCache.DataSourcePSCombo2;
        static string DataSourcePSCombo3 = Entity.StaticCache.DataSourcePSCombo3;
        static string DataSourceProjectInfo = Entity.StaticCache.DataSourceProjectInfo;

        public PI()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Entity.ProjectInfoEntity pet = new Entity.ProjectInfoEntity()
            {
                Nation = cboNation.Text,
                Site = cboSite.Text,
                ProjectTitle = txtProTitle.Text,
                ProjectSubTitle = txtProSubTitle.Text,
                Line = cboLine.Text,
                Phase = cboPhrase.Text,
                Scope = cboScope.Text,
                ConstrutionPhase = cboConPhrase.Text,
                RevNo = txtRevNo.Text,
                Deliverable1 = cboDeliverable1.Text,
                Deliverable2 = cboDeliverable2.Text,
                Assign1 = txtAssign1.Text,
                Assign2 = txtAssign2.Text,
                //Password = txtPwd.Text
            };

            try
            {
                File.Delete(DataSourceProjectInfo);
                using (var writer = new StreamWriter(DataSourceProjectInfo, true, Encoding.UTF8))
                using (var csvWriter = new CsvWriter(writer))
                {
                    List<Entity.ProjectInfoEntity> lst = new List<Entity.ProjectInfoEntity>();
                    lst.Add(pet);
                    csvWriter.WriteRecords(lst);
                    csvWriter.Flush();
                    MessageBox.Show("Project information was saved successfully!");
                    ResetValues();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetValues()
        {
            foreach (Control ctrl in tableLayoutPanel1.Controls)
            {
                if (ctrl is TextBox)
                    (ctrl as TextBox).Text = "";
            }

        }

        private void PI_Load(object sender, EventArgs e)
        {
            SetSetting();
        }

        DataTable dtEnumerationsList1 = new DataTable();

        DataTable dtNation = new DataTable();
        DataTable dtSite = new DataTable();
        DataTable dtCopySite = new DataTable();
        DataTable dtProjectTitle = new DataTable();
        DataTable dtLine = new DataTable();
        DataTable dtCopyLine = new DataTable();
        DataTable dtPhase = new DataTable();
        DataTable dtScope = new DataTable();
        DataTable dtConPhase = new DataTable();
        DataTable dtDeliverable1 = new DataTable();
        DataTable dtDeliverable2 = new DataTable();

        private void SetSetting()
        {
            if (!File.Exists(DataSourcePSCombo1) || !File.Exists(DataSourcePSCombo2) || !File.Exists(DataSourcePSCombo3) || !File.Exists(DataSourceProjectInfo))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourcePSCombo + " & " + DataSourceProjectInfo + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtEnumerationsList1.Columns.Add("Nation"); dtEnumerationsList1.Columns.Add("Site"); dtEnumerationsList1.Columns.Add("Line");

            dtNation.Columns.Add("ID"); dtNation.Columns.Add("Value");
            dtSite.Columns.Add("ID"); dtSite.Columns.Add("Value");

            dtLine.Columns.Add("ID"); dtLine.Columns.Add("Value");
            dtPhase.Columns.Add("ID"); dtPhase.Columns.Add("Value");
            dtScope.Columns.Add("ID"); dtScope.Columns.Add("Value");
            dtConPhase.Columns.Add("ID"); dtConPhase.Columns.Add("Value");
            dtDeliverable1.Columns.Add("ID"); dtDeliverable1.Columns.Add("Value");
            dtDeliverable2.Columns.Add("ID"); dtDeliverable2.Columns.Add("Value");

            MakeSettingCombo1();
            MakeSettingCombo2();
            MakeSettingCombo3();

            RemoveDuplicateRows(dtNation, "Value");
            RemoveDuplicateRows(dtSite, "Value");
            RemoveDuplicateRows(dtLine, "Value");
            RemoveDuplicateRows(dtPhase, "Value");
            RemoveDuplicateRows(dtScope, "Value");
            RemoveDuplicateRows(dtConPhase, "Value");
            RemoveDuplicateRows(dtDeliverable1, "Value");
            RemoveDuplicateRows(dtDeliverable2, "Value");

            List<Entity.ProjectInfoEntity> lst = new List<Entity.ProjectInfoEntity>();
            Array[] result;
            using (TextReader fileReader = File.OpenText(DataSourceProjectInfo))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                lst = csv.GetRecords<Entity.ProjectInfoEntity>().ToList();
                if (lst.Count() > 0)
                {
                    cboNation.ValueMember = "ID";
                    cboNation.DisplayMember = "Value";
                    cboNation.DataSource = dtNation;
                    cboNation.SelectedIndex = cboNation.FindString(lst[0].Nation.ToString());

                    cboSite.ValueMember = "ID";
                    cboSite.DisplayMember = "Value";
                    cboSite.DataSource = dtSite;
                    cboSite.SelectedIndex = cboSite.FindString(lst[0].Site.ToString());

                    cboLine.ValueMember = "ID";
                    cboLine.DisplayMember = "Value";
                    cboLine.DataSource = dtLine;
                    cboLine.SelectedIndex = cboLine.FindString(lst[0].Line.ToString());

                    cboPhrase.ValueMember = "ID";
                    cboPhrase.DisplayMember = "Value";
                    cboPhrase.DataSource = dtPhase;
                    cboPhrase.SelectedIndex = cboPhrase.FindString(lst[0].Phase.ToString());

                    cboScope.ValueMember = "ID";
                    cboScope.DisplayMember = "Value";
                    cboScope.DataSource = dtScope;
                    cboScope.SelectedIndex = cboScope.FindString(lst[0].Scope.ToString());

                    cboConPhrase.ValueMember = "ID";
                    cboConPhrase.DisplayMember = "Value";
                    cboConPhrase.DataSource = dtConPhase;
                    cboConPhrase.SelectedIndex = cboConPhrase.FindString(lst[0].ConstrutionPhase.ToString());

                    cboDeliverable1.ValueMember = "ID";
                    cboDeliverable1.DisplayMember = "Value";
                    cboDeliverable1.DataSource = dtDeliverable1;
                    cboDeliverable1.SelectedIndex = cboDeliverable1.FindString(lst[0].Deliverable1.ToString());

                    cboDeliverable2.ValueMember = "ID";
                    cboDeliverable2.DisplayMember = "Value";
                    cboDeliverable2.DataSource = dtDeliverable2;
                    cboDeliverable2.SelectedIndex = cboDeliverable2.FindString(lst[0].Deliverable2.ToString());

                    txtProTitle.Text = lst[0].ProjectTitle.ToString();
                    txtProSubTitle.Text = lst[0].ProjectSubTitle.ToString();
                    txtRevNo.Text = lst[0].RevNo.ToString();
                    txtAssign1.Text = lst[0].Assign1.ToString();
                    txtAssign2.Text = lst[0].Assign2.ToString();
                }
                else
                {
                    cboNation.ValueMember = "ID";
                    cboNation.DisplayMember = "Value";
                    cboNation.DataSource = dtNation;

                    cboSite.ValueMember = "ID";
                    cboSite.DisplayMember = "Value";
                    cboSite.DataSource = dtSite;

                    cboLine.ValueMember = "ID";
                    cboLine.DisplayMember = "Value";
                    cboLine.DataSource = dtLine;

                    cboPhrase.ValueMember = "ID";
                    cboPhrase.DisplayMember = "Value";
                    cboPhrase.DataSource = dtPhase;

                    cboScope.ValueMember = "ID";
                    cboScope.DisplayMember = "Value";
                    cboScope.DataSource = dtScope;

                    cboConPhrase.ValueMember = "ID";
                    cboConPhrase.DisplayMember = "Value";
                    cboConPhrase.DataSource = dtConPhase;

                    cboDeliverable1.ValueMember = "ID";
                    cboDeliverable1.DisplayMember = "Value";
                    cboDeliverable1.DataSource = dtDeliverable1;

                    cboDeliverable2.ValueMember = "ID";
                    cboDeliverable2.DisplayMember = "Value";
                    cboDeliverable2.DataSource = dtDeliverable2;
                }
            }
        }



        /*
        private void MakeSettingCombo()
        {
            List<ComboSetting> result;
            using (TextReader fileReader = File.OpenText(DataSourcePSCombo))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<ComboSetting>().ToList();
            }
            int i = 0;
            foreach (ComboSetting cbe in result)
            {
                i++;
                if (!String.IsNullOrEmpty(cbe.Nation.Trim()))
                {
                    dtNation.Rows.Add(new object[] { i.ToString(), cbe.Nation.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Nation.Trim()))
                {

                    dtSite.Rows.Add(new object[] { i.ToString(), cbe.Site.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Line.Trim()))
                {

                    dtLine.Rows.Add(new object[] { i.ToString(), cbe.Line.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Phase.Trim()))
                {

                    dtPhase.Rows.Add(new object[] { i.ToString(), cbe.Phase.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Scope.Trim()))
                {

                    dtScope.Rows.Add(new object[] { i.ToString(), cbe.Scope.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.ConstuctionPhase.Trim()))
                {

                    dtConPhase.Rows.Add(new object[] { i.ToString(), cbe.ConstuctionPhase.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Deliverable1.Trim()))
                {

                    dtDeliverable1.Rows.Add(new object[] { i.ToString(), cbe.Deliverable1.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Deliverable2.Trim()))
                {

                    dtDeliverable2.Rows.Add(new object[] { i.ToString(), cbe.Deliverable2.Trim() });
                }
            }
        }
        */

        private void getdtEnumerationsList1()
        {
            List<ComboSetting1> result;
            using (TextReader fileReader = File.OpenText(DataSourcePSCombo1))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<ComboSetting1>().ToList();
            }
            int i = 0;
        }

        private void MakeSettingCombo1()
        {
            List<ComboSetting1> result;
            using (TextReader fileReader = File.OpenText(DataSourcePSCombo1))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<ComboSetting1>().ToList();
            }
            int i = 0;
            foreach (ComboSetting1 cbe in result)
            {
                i++;
                if (!String.IsNullOrEmpty(cbe.Nation.Trim()))
                {
                    dtNation.Rows.Add(new object[] { i.ToString(), cbe.Nation.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Site.Trim()))
                {
                    dtSite.Rows.Add(new object[] { i.ToString(), cbe.Site.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Line.Trim()))
                {

                    dtLine.Rows.Add(new object[] { i.ToString(), cbe.Line.Trim() });
                }

                dtEnumerationsList1.Rows.Add(new object[] { cbe.Nation.Trim(), cbe.Site.Trim(), cbe.Line.Trim() });
                dtCopySite = dtSite.Copy();
                dtCopyLine = dtLine.Copy();
            }
        }

        private void MakeSettingCombo2()
        {
            List<ComboSetting2> result;
            using (TextReader fileReader = File.OpenText(DataSourcePSCombo2))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<ComboSetting2>().ToList();
            }
            int i = 0;
            foreach (ComboSetting2 cbe in result)
            {
                i++;

                if (!String.IsNullOrEmpty(cbe.Phase.Trim()))
                {
                    dtPhase.Rows.Add(new object[] { i.ToString(), cbe.Phase.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Scope.Trim()))
                {

                    dtScope.Rows.Add(new object[] { i.ToString(), cbe.Scope.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.ConstuctionPhase.Trim()))
                {

                    dtConPhase.Rows.Add(new object[] { i.ToString(), cbe.ConstuctionPhase.Trim() });
                }
            }
        }

        private void MakeSettingCombo3()
        {
            List<ComboSetting3> result;
            using (TextReader fileReader = File.OpenText(DataSourcePSCombo3))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Read();
                result = csv.GetRecords<ComboSetting3>().ToList();
            }
            int i = 0;
            foreach (ComboSetting3 cbe in result)
            {
                i++;

                if (!String.IsNullOrEmpty(cbe.Deliverable1.Trim()))
                {

                    dtDeliverable1.Rows.Add(new object[] { i.ToString(), cbe.Deliverable1.Trim() });
                }
                if (!String.IsNullOrEmpty(cbe.Deliverable2.Trim()))
                {

                    dtDeliverable2.Rows.Add(new object[] { i.ToString(), cbe.Deliverable2.Trim() });
                }
            }
        }

        public void CboDuplicateRemoval(ComboBox cboItem)
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

        private bool ErrorCheck()
        {
            var ctrl = GetAllControls(this);
            foreach (var ctrlTxt in ctrl)
            {
                if (ctrlTxt is TextBox)
                {
                    if (string.IsNullOrEmpty((ctrlTxt as TextBox).Text.TrimEnd()))
                    {
                        ctrlTxt.Focus();
                        return true;
                    }
                }
                if (ctrlTxt is ComboBox)
                {
                    if (string.IsNullOrEmpty((ctrlTxt as ComboBox).Text.TrimEnd()))
                    {
                        ctrlTxt.Focus();
                        return true;
                    }
                }
            }
            return false;
        }
        public IEnumerable<Control> GetAllControls(Control root)
        {
            foreach (Control control in root.Controls)
            {
                foreach (Control child in GetAllControls(control))
                {
                    yield return child;
                }
            }
            yield return root;
        }

        private void cboNation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            DataRowView selectedItem = (DataRowView)cmb.Items[cmb.SelectedIndex];
            string nations = selectedItem[1].ToString();
            List<DataRow> rowsToDelete = new List<DataRow>();

            if (!string.IsNullOrEmpty(nations))
            {
                var selectedSite = dtEnumerationsList1.AsEnumerable()
                         .Where(row => row.Field<string>("Nation") == nations)
                         .Select(row => row.Field<string>("Site")).Distinct();
                dtSite = dtCopySite.Copy();
                foreach (DataRow dr1 in dtSite.Rows)
                {
                    bool add = true;

                    foreach (var dr2 in selectedSite)
                    {
                        // Make sure the itemarray[x] is the proper indetifier
                        if (dr2.ToString() == dr1["Value"])
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        rowsToDelete.Add(dr1);
                    }
                }

                foreach (var r in rowsToDelete)
                    dtSite.Rows.Remove(r);

                cboSite.ValueMember = "ID";
                cboSite.DisplayMember = "Value";
                cboSite.DataSource = dtSite;
            }
        }

        private void cboSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            DataRowView selectedItem = (DataRowView)cmb.Items[cmb.SelectedIndex];
            string Site = selectedItem[1].ToString();

            List<DataRow> rowsToDelete = new List<DataRow>();
            if (!string.IsNullOrEmpty(Site))
            {
                var selectedLine = dtEnumerationsList1.AsEnumerable()
                         .Where(row => row.Field<string>("Site") == Site)
                         .Select(row => row.Field<string>("Line")).Distinct();
                dtLine = dtCopyLine.Copy();
                foreach (DataRow dr1 in dtLine.Rows)
                {
                    bool add = true;

                    foreach (var dr2 in selectedLine)
                    {
                        // Make sure the itemarray[x] is the proper indetifier
                        if (dr2.ToString() == dr1["Value"])
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        rowsToDelete.Add(dr1);
                    }
                }

                foreach (var r in rowsToDelete)
                    dtLine.Rows.Remove(r);

                cboLine.ValueMember = "ID";
                cboLine.DisplayMember = "Value";
                cboLine.DataSource = dtLine;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


    }
    public class ComboSetting1
    {
        public string Nation { get; set; }
        public string Site { get; set; }

        public string Line { get; set; }
    }

    public class ComboSetting2
    {
        public string Phase { get; set; }
        public string Scope { get; set; }
        public string ConstuctionPhase { get; set; }
    }

    public class ComboSetting3
    {
        public string Deliverable1 { get; set; }
        public string Deliverable2 { get; set; }
    }
}
