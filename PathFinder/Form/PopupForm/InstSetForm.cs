namespace RouteOptimizer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using BL;
    using CF;
    using Entity;
    using RouteOptimizer.Object;
    using static RouteOptimizer.Object.RouteInfo;

    public partial class InstSetForm : System.Windows.Forms.Form
    {
        SettingBL SettingBL = new SettingBL();
        public RouteInfo RouteInfo = new RouteInfo();
        CommonFunction cf = new CommonFunction();
        InstrumentInfoEntity iie = new InstrumentInfoEntity();
        public InstSetForm(RouteInfo routeInfo = null)
        {
            InitializeComponent();
            this.RouteInfo = routeInfo;
            if (routeInfo.SelectedGUID == 0)
                iie = null; // RouteInfo.DGV_LstInstrument = null;
            else
                iie = RouteInfo.FindDGV_Instrument(RouteInfo.SelectedGUID);
        }


        private void button11_Click(object sender, EventArgs e)
        {
            if (SaveInfo())
            {
                var ie = RouteInfo.FindDGV_Instrument(RouteInfo.SelectedGUID);
                RouteInfo.DGV_LstInstrument.Remove(ie);
                RouteInfo.DGV_LstInstrument.Add(iie);
                MessageBox.Show("Instrument is saved");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Saving instrument failed!");
            }
        }
        private void AdjustColumnSize(DataGridView[] dgv)
        {
            foreach (var g in dgv)
            {
                g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                foreach (DataGridViewColumn column in g.Columns)
                    column.MinimumWidth = column.Width;
                g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
        private void InstSetForm_Load(object sender, EventArgs e)
        {
            ////dgv_SignalMap.AutoGenerateColumns = false;
            dgv_Signal.AllowUserToAddRows = false;
            dgv_Signal.AutoGenerateColumns = false;
            //dgv_SignalMap.AllowUserToAddRows = false/*;*/
            BindInitialSetting();
            AdjustColumnSize(new DataGridView[] { dgv_Signal });
        }
        public void SystemBind_Layer()
        {
            //DataTable system = new DataTable();
            //system.Columns.Add("System");
            //system.Columns.Add("dgv_System");
            //BL.SettingBL sbl = new BL.SettingBL();
            //if (sbl.GetSystemList().Count > 0)
            //{
            //    s1 = sbl.GetSystemList()[0].Title;
            //    // system.Rows.Add(null, "");
            //    foreach (var t in sbl.GetSystemList())
            //    {
            //        system.Rows.Add(t.Id, t.Title);
            //        //   system.Rows.Add(t.Title);
            //    }
            //    DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"];
            //    col.ValueMember = col.DataPropertyName = "System";
            //    col.DisplayMember = "dgv_System";

            //    ((DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"]).DataSource = system;
            //}
        }
        private void BindComboGrid()
        {
            var cbo1 = cf.ListToDataTable(SettingBL.GetInstrumetnType());
            cbo_T1.DisplayMember = cbo_T1.ValueMember = "Classification_2";
            cbo_T1.DataSource = cbo1;

            var cbo2 = cf.ListToDataTable(SettingBL.GetInstrumetnType());

            cbo_T2.DisplayMember =
                cbo_T2.ValueMember = "Classification_3";
            cbo_T2.DataSource = cbo2;
             
            //DataTable system = new DataTable();
            //system.Columns.Add("System");
            //system.Columns.Add("dgv_System");
            //BL.SettingBL sbl = new BL.SettingBL();
            //if (sbl.GetSystemList().Count > 0)
            //{
            //    foreach (var t in sbl.GetSystemList())
            //    {
            //        system.Rows.Add(t.Id, t.Title);
            //        //   system.Rows.Add(t.Title);
            //    }
            //}
            var cbo3 = cf.ListToDataTable(SettingBL.GetSystemList());
            cbo_System.DisplayMember = "dgv_System";
            cbo_System.ValueMember = "System";
            cbo_System.DataSource = cbo3;

            DataTable type = new DataTable();
            type.Columns.Add("GUID");
            type.Columns.Add("To");
            type.Rows.Add(null, -1);
            foreach (var t in RouteInfo.LstTBBOXes)
            //  if (t.IsIO != eDestinationType.MCC)
            {
                type.Rows.Add(t.Name, t.guid);
            }
            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dgv_Signal.Columns["colTo"];
            col.DisplayMember = "GUID";
            col.ValueMember = col.DataPropertyName = "To";
            col.DataSource = type;

        }
        private void BindInitialSetting()
        {
            BindComboGrid();
            if (iie != null)
            {
                dgv_Signal.DataSource = null;
                dgv_Signal.DataSource = RouteInfo.ListToDataTable(iie.LstInstCableEntity, null, "No");
                //int u= 0;
                //foreach (DataGridViewRow dr in dgv_Signal.Rows)
                //{
                //    u++;
                //    dr.Cells["No"].Value = u.ToString();
                //}
                //.DataSource = RouteInfo.ListToDataTable(iie.LstInstCableEntity, null, "No");

                txt_TagNo.Text = iie.T1 + "_" + iie.T2;

                cbo_System.SelectedValue = iie.System;

                cbo_T1.SelectedValue = iie.Classification_2;

                cbo_T2.SelectedValue = iie.Classification_3;
                cbo_T2.Focus();

                chk_AutoRoute.Checked = (iie.CHK_AutoRoute == 1);

                chk_EachRoute.Checked = (iie.CHK_EachRoute == 1);
            }
            cbo_T2.SelectedIndexChanged += Cbo_T2_SelectedIndexChanged;
            //cbo_T2.SelectedValue = iie.Classification_3;
            if ((cbo_T2.SelectedValue != null || string.IsNullOrEmpty(cbo_T2.SelectedValue.ToString())))
                SetInsCable(cbo_T2.SelectedValue.ToString());
            BL.SettingBL sbl = new BL.SettingBL();
            var lstSignal = sbl.GetSignalType();
            foreach (DataGridViewRow dr in dgv_Signal.Rows)
            {
                var sys = dr.Cells["colS_System"].Value.ToString();
                if (string.IsNullOrEmpty(sys)) continue;
                dr.Cells["col_Signal"].Value = lstSignal.Where(x => x.Title == sys).FirstOrDefault().AssignedCableDuct;
            }
            dgv_Signal.RefreshEdit();
        }

        private void Cbo_T2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = (sender as ComboBox).SelectedValue.ToString();
            if (!string.IsNullOrEmpty(selectedValue))
                SetInsCable(selectedValue);
        }

        List<InstCableEntity> lstICE = null;
        private bool SaveInfo()
        {
            if (!CheckExistInsCable()) return false;
            try
            {
                iie.System = cbo_System.SelectedValue == null ? "" : cbo_System.SelectedValue.ToString();
                iie.T1 = txt_TagNo.Text.Split('_').First(); iie.T2 = txt_TagNo.Text.Split('_').Last();
                iie.Classification_2 = cbo_T1.SelectedValue.ToString();
                iie.Classification_3 = cbo_T2.SelectedValue.ToString();
                iie.CHK_AutoRoute = (chk_AutoRoute.Checked ? Convert.ToByte(1) : Convert.ToByte(0));
                iie.CHK_EachRoute = (chk_EachRoute.Checked ? Convert.ToByte(1) : Convert.ToByte(0));
                lstICE = new List<InstCableEntity>();
                InstCableEntity instCableEntities = null;
                foreach (DataGridViewRow ur in dgv_Signal.Rows)
                {
                    //foreach (DataGridViewRow dr in dgv_SignalMap.Rows)
                    //{
                    //    if (dr.Cells["Map_No"].Value.ToString() == ur.Cells["colNo"].Value.ToString())
                    //    {

                    instCableEntities = new InstCableEntity()
                    {
                        No = ur.Cells["colNo"].Value.ToString(),
                        System = ur.Cells["colS_System"].Value.ToString(),
                        Abb = ur.Cells["colS_Abb"].Value.ToString(),
                        S_Check = ur.Cells["colS_Check"].Value.ToString(),
                        Cable = ur.Cells["colCable"].Value.ToString(),
                        From = ur.Cells["colFrom"].Value.ToString(),
                        To = ur.Cells["colTo"].Value.ToString(),
                        col_Signal = ur.Cells["col_Signal"].Value.ToString(),
                        Type = ur.Cells["Type"].Value.ToString(),
                    };
                    lstICE.Add(instCableEntities);
                    //break;
                    ////    }
                    ////}
                }
                iie.LstInstCableEntity = lstICE;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);
                return false;
            }
            return true;
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_SignalMap_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dgv_Signal_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dgv_Signal.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgv_Signal.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dgv_Signal.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void dgv_SignalMap_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
        bool ExistInInsCable = false;
        private bool CheckExistInsCable()
        {
            if (RouteInfo == null || RouteInfo.DGV_LstInstrument == null) return false;
            RouteOptimizer.BL.SettingBL settingLBL = new BL.SettingBL();
            var lstInscable = settingLBL.GetInsCable();
            BL.SettingBL sbl = new BL.SettingBL();
            var LstInsType = sbl.GetInstrumetnType();

            var selectedValue = cbo_T2.SelectedValue.ToString();

            var seletecedinst = lstInscable.Where(a => a.InstType == selectedValue).ToList<InstCableEntity>();
            if (seletecedinst.Count == 0)
            {
                return false;
            }
            return true;
        }
        private void SetInsCable(String type)
        {
            if (RouteInfo == null || RouteInfo.DGV_LstInstrument == null) return;
            RouteOptimizer.BL.SettingBL settingLBL = new BL.SettingBL();
            var lstInscable = settingLBL.GetInsCable();
            BL.SettingBL sbl = new BL.SettingBL();
            var lstSignal = settingLBL.GetSignalType();
            var LstInsType = sbl.GetInstrumetnType();
            if (iie.Type == type) return;
            iie.Type = type;

            var seletecedinst = lstInscable.Where(a => a.InstType == iie.Type).ToList<InstCableEntity>();
            if (seletecedinst.Count == 0)
            {
                ExistInInsCable = false;
                MessageBox.Show("There is no data source relating with this selected signal. Please update 'InstCable.csv' file or select another signal.");
                return;
            }
            ExistInInsCable = true;
            iie.LstInstCableEntity = lstInscable.Where(a => a.InstType == iie.Type).ToList<InstCableEntity>();

            List<InstCableEntity> lstInsCa = new List<InstCableEntity>();
            int u = 0;
            foreach (var lsc in iie.LstInstCableEntity)
            {
                u++;
                InstCableEntity instCableEntity = new InstCableEntity();
                instCableEntity.No = u.ToString();
                instCableEntity.Abb = lsc.Abb;
                instCableEntity.Cable = lsc.Cable;
                instCableEntity.InstType = lsc.InstType;
                instCableEntity.No = lsc.No;
                instCableEntity.System = lsc.System;
                instCableEntity.S_Check = lsc.S_Check;
                instCableEntity.Type = lsc.Type;
                instCableEntity.From = iie.T1 + "_" + iie.T2;
                instCableEntity.To = iie.To.ToString();

                try
                {
                    instCableEntity.col_Signal = lstSignal.Where(x => x.Title == lsc.System).FirstOrDefault().AssignedCableDuct;
                }
                catch (Exception ex)
                {

                }

                lstInsCa.Add(instCableEntity);
            }
            iie.LstInstCableEntity = lstInsCa;
            dgv_Signal.DataSource = null;
            dgv_Signal.DataSource = RouteInfo.ListToDataTable(iie.LstInstCableEntity, null, "No");
            //dgv_SignalMap.DataSource = null; 
            //dgv_SignalMap.DataSource = RouteInfo.ListToDataTable(iie.LstInstCableEntity, null, "No");
            //var getInstrumentType = LstInsType.Where(a => a.Classification_3 == inst.Type).FirstOrDefault<InstrumentListEntity>();
            //inst.Classification_2 = getInstrumentType.Classification_2;
            //inst.Classification_3 = inst.Type;
        }

        private void dgv_Signal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_SignalMap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_Signal_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dgv_Signal_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgv_Signal.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
