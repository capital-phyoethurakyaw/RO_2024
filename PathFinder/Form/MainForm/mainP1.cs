using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vdControls;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Actions;
using RouteOptimizer;
using CsvHelper;
using RouteOptimizer.Entity;
using RouteOptimizer.PInfoForms;
using static RouteOptimizer.Entity.CableScheduleEntity;
using RouteOptimizer.Object;
using static RouteOptimizer.Object.RouteInfo;
using System.Xml.Serialization;
using System.Diagnostics;
using Aspose.Zip;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Drawing.Printing;
using RouteOptimizer.Analysis1;//using RouteOptimizer.Analysis;
using System.Reflection;
using VectorDraw.Render;
using System.Runtime.InteropServices;
//using DocumentFormat.OpenXml.Presentation;

namespace RouteOptimizer
{
    public partial class MainP1 : System.Windows.Forms.Form
    {
        static string DataSourceAnalysisDuctSchedule = Entity.StaticCache.DataSourceAnalysisDuctSchedule;
        static string DataSourceAnalysisDuctTypeSchedule = Entity.StaticCache.DataSourceAnalysisDuctTypeSchedule;
        static string DataSourceAnalysisDuctCableSchedule = Entity.StaticCache.DataSourceAnalysisDuctCableSchedule;

        static string DataSourceCableDuctSchedule = Entity.StaticCache.DataSourceCableDuctSchedule;
        static string DataSourceInstrumentSchedule = Entity.StaticCache.DataSourceInstrumentSchedule;
        static string DataSourceXml = Entity.StaticCache.path;
        static string DataSourceXmlBuffer = Entity.StaticCache.path.Replace("\\DS", "\\XMLDataBuffer");

        public List<Instrument> instruments = new List<Instrument>();
        public List<Instrument> allinstruemnts = new List<Instrument>();
        public List<TBBOXDestination> tbBoxes = new List<TBBOXDestination>();
        public List<vdPolyline> obstacles = new List<vdPolyline>();
        public List<vdCircle> instrument = new List<vdCircle>();
        public List<string> blkvdInsertCollection = new List<string>() { };
        public List<string> UsedblkvdInsertCollection = new List<string>() { };
        public gPoints gridPoints = new gPoints();
        public vdPolyline boundary;
        public vdPolyline offsetBoundary;
        public TBBOXDestination destination;
        public gPoints PreVPolyLine;
        public RouteInfo routeInfo;
        public List<PointF> Seeds = new List<PointF>();
        public List<PointF> Centroids = new List<PointF>();
        public vdControls.vdFramedControl vdFramedControl;
        List<PointData> Points = new List<PointData>();
        public gPoint OCentriod;
        double sx = 0;
        double sy = 0;
        public int counter = 0;
        public static DataTable dtInstrument;
        public static DataTable dtSchedule;
        public static DataTable dtSegmentation;
        RouteOptimizer.Util.Nearestpoint nearestpoint = new Util.Nearestpoint();
        Thread thread = null;
        public MainP1()
        {
            InitializeComponent();
            this.vdFC1.BaseControl.AfterAddItem += BaseControl_AfterAddItem;
            this.vdFC1.BaseControl.AddItem += BaseControl_AddItem;

            vdFramedControl = vdFC1;
            //Default
            txtMaxLengthInstrument.Enabled = false;
            txtMaxLengthInstrument.Text = "0";
            txtDuctOptimize.Text = "20";
            txtHTolerance.Text = "1";
            txtYTolerence.Enabled = false;
            txtYTolerence.Text = "1";
            CheckForIllegalCrossThreadCalls = false;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (routeInfo == null)
                return;

            if (string.IsNullOrEmpty(cboInst.Text))
            {
                MessageBox.Show("레이어를 선택해주세요.");
                return;
            }
            if (string.IsNullOrEmpty(txtT1Name.Text) || string.IsNullOrEmpty(txtT2Name.Text))
            {
                MessageBox.Show("태그 값을 입력해주세요.");
                if (string.IsNullOrEmpty(txtT1Name.Text)) txtT1Name.Focus();
                if (string.IsNullOrEmpty(txtT2Name.Text)) txtT2Name.Focus();
                return;
            }
            if (blkvdInsertCollection.Count == 0 || !blkvdInsertCollection.Where(x => x.Trim().Contains(routeInfo.TAGNAME)).Any())
            {
                MessageBox.Show("입력된 \"TagNo\" 이름의 블록이 입력 데이터 내에 없습니다. 파일을 재입력해주세요.");
                return;
            }
            var lstIns = routeInfo.GetAllInstrument(vdFC1, routeInfo);
            if (lstIns.Where(x => x.t1 == txtT1Name.Text && x.t2 == txtT2Name.Text).Any())
            {
                MessageBox.Show("입력하신 태그의 계측기기가 이미 존재합니다. 다른 계측기기 태그를 입력해주세요.");
                return;
            }
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.INSTRUMENT;
            SetCustomLayer(cboInst.Text);
            AddInsertObjects();
            //  AddBlockInstrument();
        }
        private void OpenBlocksForm(string blkName = null)
        {

            VectorDraw.Professional.Dialogs.InsertBlockDialog form = VectorDraw.Professional.Dialogs.InsertBlockDialog.Show(vdFC1.BaseControl.ActiveDocument, this, false, "");


            if (form.DialogResult == DialogResult.OK)
            {
                if (blkName != form.blockname)
                {
                    MessageBox.Show("계측기기 태그 블록을 선택해주세요.");
                    return;
                }
                if (form.scales is string)

                    vdFC1.BaseControl.ActiveDocument.CommandAction.CmdInsert(form.blockname,
                    form.insertionPoint, form.scales, form.scales, form.rotationAngle);
                else
                {
                    double[] scales = form.scales as double[];
                    IsManualDrawn = true;
                    SetCustomLayer(cboInst.Text.ToString());
                    var res = vdFC1.BaseControl.ActiveDocument.CommandAction.CmdInsert(routeInfo.TAGNAME, form.insertionPoint, scales[0], scales[1], form.rotationAngle);
                    if (res)
                    {
                        if (LeaderEndPoint == null)
                        {
                            MessageBox.Show("leader line과 교차된 내용이 존재합니다. 교차된 객체를 지우고 leader line을 확인해주세요.");
                            return;
                        }
                        Vertexes vertexes = new Vertexes();
                        vertexes.Add(LeaderStartPoint);
                        vertexes.Add(LeaderEndPoint);
                        if (vdFC1.BaseControl.ActiveDocument.CommandAction.CmdLeader(vertexes, ""))
                        {
                            if (LeaderInsert != null)
                            {
                                var pair = Tuple.Create(LeaderInsert, LeaderLine);
                                routeInfo.AllInstrument_Leader.Add(pair);
                            }
                        }
                        IsManualDrawn = !IsManualDrawn;
                    }
                }
            }
        }
        gPoint LeaderStartPoint = null;
        gPoint LeaderEndPoint = null;
        vdLeader LeaderLine = null;
        vdInsert LeaderInsert = null;
        private void AddInsertObjects(InstrumentParams instrumentParams = null)
        {
            var Ut1 = txtT1Name.Text; //instrumentParams.T1;
            var Ut2 = txtT2Name.Text; //instrumentParams.T2;
            var Ublk = routeInfo.TAGNAME; //instrumentParams.InsertName;
            if (string.IsNullOrEmpty(Ut1) || string.IsNullOrEmpty(Ut2) || string.IsNullOrEmpty(Ublk)) return;
            LeaderStartPoint = null;
            vdFC1.BaseControl.ActiveDocument.Prompt("Select a Point");

            var re = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out LeaderStartPoint);
            if (re != StatusCode.Success || LeaderStartPoint == null)
                return;
            vdFC1.BaseControl.ActiveDocument.Prompt(null);

            var PrepareDesEndPoint = new TBBOXDestination(null, LeaderStartPoint);
            nearestpoint.GetNearestpoint(PrepareDesEndPoint, routeInfo, unitGrid);
            LeaderStartPoint = PrepareDesEndPoint.gridPoint;

            if (LeaderStartPoint == null) return;

            if (vdFC1.BaseControl.ActiveDocument.Blocks.FindName(Ublk) != null)
            {
                VectorDraw.Professional.vdFigures.vdInsert ins;
                ins = new VectorDraw.Professional.vdFigures.vdInsert();
                ins.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
                ins.setDocumentDefaults();
                ins.Block = vdFC1.BaseControl.ActiveDocument.Blocks.FindName(Ublk);

                if (ins.Block.Entities.Count > 0)
                {
                    foreach (vdFigure vdfig in ins.Block.Entities.GetNotDeletedItems())
                    {
                        if (vdfig is vdAttribDef vdAttribDef)
                        {
                            SetValue(vdAttribDef, "T1", Ut1);
                            SetValue(vdAttribDef, "T2", Ut2);
                        }
                    }
                }
                //vdFC1.BaseControl.ActiveDocument.ActiveLayOut.ZoomExtents();
                //vdFC1.BaseControl.ActiveDocument.Redraw(true);
                OpenBlocksForm(Ublk);
                return;
            }
            else
                MessageBox.Show("블록을 찾을 수 없습니다.");
        }

        private void SetValue(vdAttribDef vdAttribDef, string Key, string value)
        {
            if (vdAttribDef.TextString == Key)
                vdAttribDef.ValueString = value;
        }
        static string DocPath;
        bool IsInfook = false;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Import("");
        }
        private void Import(string pth)
        {

            if (!ImportDialog(pth)) return;
            Initialize();

            string s1 = "";
            SystemBind_Layer(ref s1);
            TypeBind_Layer(ref s1);
            AnalysisBind();
            if (!ReadXML())
            {
                MessageBox.Show("파일을 불러오는 데에 오류가 발생했습니다. (오류 내용 - xml deserializing)");
                return;
            }
            SaveButtonState();
            if (!SettingAfterImported())
            {
                // MessageBox.Show("There is an error when binding data from source file. Please try to import again."+ Environment.NewLine+" If proceed as it is, the remaining process to be imported will be incomplete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // return;
            }
            ExplodeBlock();
            BindFrequency();
            BindDecimalplace();
            inputType = 0;
        }
        private void BindDecimalplace()
        {

            PutDecimal(dgv_InsAnalysis, new List<string> { "A_Length" });
            PutDecimal(dgv_SegAnalysis, new List<string> { "A_SLength", "A_TotalArea", "A_OptimalRatio", "A_UserDefinedRato" });
            PutDecimal(dgv_SubIns, new List<string> { "Sub_Length" });
            PutDecimal(dgv_1, new List<string> { "colTotalLength1" });
            PutDecimal(dgv_2, new List<string> { "colTotalLength2" });
            PutDecimal(dgv_3, new List<string> { "colTotalLength3" });
        }
        private bool ImportDialog(string pth)
        {

            string path = null;
            if (string.IsNullOrEmpty(pth))
            {
                path = vdFC1.BaseControl.ActiveDocument.GetOpenFileNameDlg(0, pth, 0) as string;
                //else
                //    path = vdFC1.BaseControl.ActiveDocument.Open(pth) ? pth : null ;
                if (string.IsNullOrEmpty(path))
                    return false;
                DocPath = path;
                bool success = vdFC1.BaseControl.ActiveDocument.Open(DocPath);
                if (!success)
                    return false;
            }
            else
            {
                DocPath = pth;
                bool success = vdFC1.BaseControl.ActiveDocument.Open(DocPath);
                if (!success)
                    return false;
            }
            return true;
            //var f= new vdControls.vdFramedControl();//  vdFramedControl = new vdFramedControl;
            // f.BaseControl
        }
        private bool ReadXML()
        {
            try
            {
                var lyrs = vdFC1.BaseControl.ActiveDocument.Layers.GetNotDeletedItems();
                vdLayer lyr = null;
                foreach (vdLayer l in lyrs)
                {
                    if (l.Name.Contains(routeInfo.SYSTEMABBRE))
                    {
                        lyr = l;
                        routeInfo.SystemFile = l.Name;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(routeInfo.SystemFile))
                {
                    vdFC1.BaseControl.ActiveDocument.Layers.RemoveItem(lyr);
                    XmlSerializer xs = new XmlSerializer(typeof(XMLBuffer.ID_Exchanger), new Type[] { });
                    // DataSourceXmlBuffer + "\\" + routeInfo.SYSTEMABBRE + SaveDialogFile  + "_" + GetFileName("") + ".xml";
                    var Fname = DataSourceXmlBuffer + "\\" + routeInfo.SystemFile.Replace(".xml", "") + ".xml";
                    if (File.Exists(Fname))
                    {
                        using (var sr = new StreamReader(Fname))
                        {
                            var xmlbuffer = xs.Deserialize(sr) as XMLBuffer.ID_Exchanger;
                            xmlbuffer.SystemFile = routeInfo.SystemFile;
                            lbl_ImportedLayer = routeInfo.SystemFile;
                            routeInfo = xmlbuffer.GetRouteInfo(vdFC1);

                            sr.Dispose();
                            sr.Close();
                        }
                        BindInsCombo();
                        BindGrid();
                        BindBasicInfo();
                        BindAlternativeGrid();
                        DebugLog.WriteLog(routeInfo.SystemFile + " was imported.");

                    }
                    else
                    {
                        MessageBox.Show("선택하신 경로에 파일이 존재하지 않습니다. 경로를 확인해주세요.");
                        DebugLog.WriteLog("선택하신 경로에 파일이 존재하지 않습니다.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);
                //   var msg = MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return false;
        }
        string lbl_ImportedLayer = "";
        private void BindAlternativeGrid()
        {
            CheckFilter();
            //try
            //{
            //   // var CaseRoute = routeInfo.CaseRoutes;

            //    chk_relation_CheckedChanged(null, null);
            //}
            //catch(Exception ex)
            //{

            //}
        }
        private void BindBasicInfo()
        {
            var bi = routeInfo.BasicInfo;
            txtMaxLengthInstrument.Text = bi.MaxLengthInstrument;
            txtDuctOptimize.Text = bi.DuctOptimizedRatio;
            txtHTolerance.Text = bi.HTolerence;
            txtYTolerence.Text = bi.YTolerence;
            txtConnectable.Text = string.IsNullOrEmpty(bi.Connectable.Trim()) ? "3" : (Convert.ToInt32(bi.Connectable.Trim()) > 100) ? "3" : bi.Connectable.Trim();
            txtRearClearence.Text = bi.RearClearence;
            txtPanelbay.Text = bi.PanelBay;
            txtFrontpart.Text = bi.FrontPart;
            txtWallFrontSeparate.Text = bi.WallFront;
            txtWallSideDistance.Text = bi.SideFront;
        }
        private void BindLayer()
        {
            foreach (var l in routeInfo.LstLayer)
            {
                if (l.SelectedInInstrument)
                {
                    instListbox.Rows.Add(new object[] {
                        l,
                      routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).Any()?  routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).FirstOrDefault().Type : "" ,
                      routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).Any()?  routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).FirstOrDefault().System : "" ,
                       routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).Any()?  routeInfo.LstLayerAllColumn.Where(x=>x.LayerName == l.name).FirstOrDefault().To  : ""
                    });
                    instListbox.Update();
                }
                if (l.SelectedInObstacle)
                {
                    ObstListbox.Items.Add(l);
                }
            }
            richTextBox1.Text = routeInfo.DetectedObstacles;
            cbo_layer_Setting.SelectedIndex = routeInfo.SelectedDestination;
            SegmentaionSetting(true);
        }
        private void BaseControl_ActionStart(object sender, string actionName, ref bool cancel)
        {

        }

        private bool SettingAfterImported()
        {
            bool IsBoundaryOk = true;
            try
            {
                var ets = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;

                cbo_layer_Setting.Items.Clear();
                foreach (vdLayer layer in vdFC1.BaseControl.ActiveDocument.Layers.GetNotDeletedItems())
                {
                    instLyrDGV.Rows.Add(layer.Name);
                    obstLyrDGV.Rows.Add(layer.Name);
                    cboInst.Items.Add(layer.Name);
                    cboObst.Items.Add(layer.Name);
                    cboDest.Items.Add(layer.Name);
                    cboBoun.Items.Add(layer.Name);
                    cbo_layer_Setting.Items.Add(layer.Name);
                }
                try
                {
                    cboInst.SelectedIndex = 1;
                    cboObst.SelectedIndex = 1;
                    cboDest.SelectedIndex = 1;
                    cboBoun.SelectedIndex = 1;
                    cbo_layer_Setting.SelectedIndex = 1;
                }
                catch
                {

                }
                if (string.IsNullOrEmpty(routeInfo.SystemFile) || routeInfo.MainBoundary == null)
                {
                    routeInfo.MainBoundary = GetMainBoundary();
                    if (routeInfo.MainBoundary == null)
                    {
                        MessageBox.Show("외각 라인을 추가해주세요.");
                        IsBoundaryOk = false;
                        //return false;
                    }
                }
                dgv_1.Rows.Clear();
                dgv_2.Rows.Clear();
                dgv_3.Rows.Clear();
                if (!String.IsNullOrEmpty(routeInfo.SystemFile))
                {
                    BindLayer();
                }
                else
                {
                    GetAllLayer();
                }
                routeInfo.LstLayer = routeInfo.GetAllLayer(vdFramedControl, routeInfo, instListbox, ObstListbox);
                cbo_layer_Setting.Items.Clear();
                foreach (var lyr in routeInfo.LstLayer)
                {
                    cbo_layer_Setting.Items.Add(lyr.name);
                }
                cboDestType.SelectedItem = "T/B Box";
                routeInfo.xgap = txtHTolerance.Text;
                routeInfo.ygap = txtYTolerence.Text;
                cbo_polyTypes.DataSource = new List<string>() { "Poly", "Rect" };
                var usedBlk = vdFC1.BaseControl.ActiveDocument.Blocks.GetNotDeletedItems();
                foreach (var ub in usedBlk)
                {
                    if (ub is vdBlock vb && !vb.Name.ToLower().Contains("vddim_"))
                    {
                        blkvdInsertCollection.Add((vb).Name.ToString().Replace("vdInsert ", "").TrimStart());
                    }
                }
                if (string.IsNullOrEmpty(DocPath.ToString()))
                {
                    this.Text = "Samoo Route Optimizer";
                }
                else
                {
                    this.Text = "Samoo Route Optimizer" + "   file:" + "\\\\" + DocPath.ToString();
                }
                if (routeInfo.DEFAULT_OPTION == eSCOPE_MODE.USERDEFINED_ROUTE)
                {
                    rdo_UserDefine.Checked = true;
                    //routeInfo.LstGuidedRoute = routeInfo.LstAllRoute();
                    ////foreach (var ri in routeInfo.LstGuidedRoute)
                    ////{
                    ////    if (ets.FindItem(ri))
                    ////    {
                    ////        ri.visibility = vdFigure.VisibilityEnum.Visible;
                    ////    }
                    ////}
                }
                if (routeInfo.DEFAULT_OPTION == eSCOPE_MODE.AUTO_ROUTE)
                {
                    rdo_WithoutRoute.Checked = true;
                    //routeInfo.LstAutoRoute = routeInfo.LstAllRoute();
                    //foreach (var ri in routeInfo.LstGuidedRoute)
                    //{
                    //    if (ets.FindItem(ri))
                    //    {
                    //        ri.visibility = vdFigure.VisibilityEnum.Visible;
                    //    }
                    //}
                }
                if (routeInfo.DEFAULT_OPTION == eSCOPE_MODE.AUTO_GUIDED_ROUTE)
                {
                    rdo_withroute.Checked = true;
                }

                BoxWidth = routeInfo.BoxWidth;
                BoxHeight = routeInfo.BoxHeight;
                IOColor = routeInfo.IOcolor;
                TBColor = routeInfo.TBColor;
                MCCColor = routeInfo.MCCcolor;

                BindResultTables();

                // rdo_UserDefine.Checked ? 0 : rdo_WithoutRoute.Checked ? 1 : rdo_withroute.Checked ? 2 : 0;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show(ex.Message);

            }

            if (!IsBoundaryOk)
                return false;
            return true;
        }

        private void SaveButtonState()
        {
            if (string.IsNullOrEmpty(vdFC1.BaseControl.ActiveDocument.FileName))
            {
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
            }
            else
            {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
            }
        }
        private void BindInsCombo()
        {
            int r1 = 0;
            string t1 = "";
            string s1 = "";
            DestinationBind(ref r1);
            TypeBind(ref t1);
            SystemBind(ref s1);
            //AnalysisBind();
        }
        private void AnalysisBind()
        {
            //DataTable type = new DataTable();
            //type.Columns.Add("A_UserDefinedSize");
            //BL.SettingBL sbl = new BL.SettingBL();
            //if (sbl.GetCableDuctList().Count > 0)
            //{
            //    foreach (var t in sbl.GetCableDuctList()) 
            //        type.Rows.Add(t.Title);
            //    //DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)dgv_SegAnalysis.Columns["A_UserDefinedSize"];
            //    ////DataGridViewComboBoxCell dc = new DataGridViewComboBoxCell();
            //    ////dc.DataSource
            //    //col.ValueMember = col.DisplayMember = col.DataPropertyName = "A_UserDefinedSize";
            //    //((DataGridViewComboBoxColumn)dgv_SegAnalysis.Columns["A_UserDefinedSize"]).DataSource = type;
            //}
        }
        private void ExplodeBlock()
        {
            SettingSource();
            RefreshUpdate();
            DrawPolyline();
        }

        private DataTable SettingTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BK");
            dt.Columns.Add("FG");
            dt.Columns.Add("SP", typeof(gPoint));
            dt.Columns.Add("EP", typeof(gPoint));
            dt.Columns.Add("GEO");
            dt.Columns.Add("Layer", typeof(vdLayer));

            return dt;
        }
        DataTable dtEntities;
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
        private void Initialize()
        {
            ListComboAllDestination = new List<string>();
            instLyrDGV.Rows.Clear();
            obstLyrDGV.Rows.Clear();
            instListbox.Rows.Clear();
            ObstListbox.Items.Clear();
            cboInst.Items.Clear();
            cboObst.Items.Clear();
            cboBoun.Items.Clear();
            cboDest.Items.Clear();
            cbo_layer_Setting.Items.Clear();
            var ctrls = GetAllControls(this);
            foreach (var ctrl in ctrls)
            {
                if (ctrl is ComboBox cbo)
                {
                    cbo.Text = "";
                    cbo.DataSource = null;
                }
                if (ctrl is DataGridView dgv)
                {
                    dgv.DataSource = null;
                }
                if (ctrl is ListBox lsb)
                {
                    lsb.DataSource = null;
                }
            }
            ClearGridView(dgv_destination);
            ClearGridView(dgv_alternaitve);
            ClearGridView(dgv_SegAnalysis);
            ClearGridView(dgv_SubIns);
            ClearGridView(dgv_InsAnalysis);
            ClearGridView(dgv_SubSeg);

            routeInfo = new RouteInfo();
            instruments = new List<Instrument>();
            obstacles = new List<vdPolyline>();
            instrument = new List<vdCircle>();
            blkvdInsertCollection = new List<string>() { };
            gridPoints = new gPoints();
            boundary = new vdPolyline();
            offsetBoundary = new vdPolyline();
            destination = null;
            IsRoute = IsRelocate = false;
            addLyr.Enabled = true;
            //txtExportInstrument.Enabled = true;
            SaveXmlFile = "";
            SaveDialogFile = "";
            btnExport.Enabled = false;
            rdo_withroute.Checked = true;
        }
        private void ClearGridView(DataGridView dgv)
        {
            List<DataGridViewRow> dgr = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                dgr.Add(dr);
            }
            foreach (var dr in dgr)
            {
                dgv.Rows.Remove(dr);
            }
            dgv.Refresh();
        }
        private void RefreshUpdate()
        {
            var prim = vdFC1.BaseControl.ActiveDocument.Blocks.GetNotDeletedItems();
            foreach (var blk in prim)
            {
                if ((blk as vdBlock).Entities != null && !UsedblkvdInsertCollection.Contains(blk.ToString()))
                {
                    (blk as vdBlock).Entities.RemoveAll();
                }
            }
            vdFC1.BaseControl.ActiveDocument.Update();
            vdFC1.BaseControl.ActiveDocument.Redraw(true);

        }
        private void SettingSource()
        {
            dtEntities = new DataTable();
            dtEntities = SettingTable();
            int obsOccurence = 0;
            foreach (vdFigure f in vdFC1.BaseControl.ActiveDocument.Model.Entities.GetNotDeletedItems())
            {
                if (f != null)
                {
                    if (f is vdInsert vi && blkvdInsertCollection.Contains(f.ToString().Replace("vdInsert ", "")))
                    {

                        var ent = vi.Explode();
                        if (ent != null)
                        {
                            string t1;
                            string t2;
                            if (routeInfo.Check_1C_2A(ent))
                            {
                                UsedblkvdInsertCollection.Add(f.ToString().Replace("vdInsert ", ""));
                            }
                            else if (routeInfo.Chek_1C_2A_1L(ent))
                            {
                                UsedblkvdInsertCollection.Add(f.ToString().Replace("vdInsert ", ""));
                            }
                            else if (CheckAllOnlyLines(ent)) // Only Rectangle block
                            {
                                obsOccurence++;
                                int entityOccurence = 0;

                                foreach (var e in ent)
                                {
                                    entityOccurence++;
                                    if (e is vdLine vl)
                                    {
                                        var s = vl.Layer.PenColor.ColorIndex;
                                        dtEntities.Rows.Add(new object[] { obsOccurence, entityOccurence, vl.StartPoint, vl.EndPoint, vi.BoundingBox.MidPoint, vl.Layer });
                                        vi.Invalidate();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool CheckAllOnlyLines(vdEntities ve)
        {
            foreach (var lines in ve)
            {
                if (!(lines is vdLine))
                    return false;
            }
            return true;
        }
        private void DrawPolyline()
        {
            int completeOnepolygon = 0;

            VectorDraw.Professional.vdFigures.vdPolyline onepoly = new VectorDraw.Professional.vdFigures.vdPolyline();
            foreach (DataRow dr in dtEntities.Rows)
            {
                completeOnepolygon++;
                if (completeOnepolygon <= 4)
                {
                    onepoly.VertexList.Add(dr["SP"] as gPoint);
                    onepoly.PenColor.ColorIndex = 7;
                }
                if (completeOnepolygon == 4)
                {
                    vdFC1.BaseControl.ActiveDocument.SetActiveLayer(dr["SP"] as vdLayer);
                    onepoly.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
                    onepoly.setDocumentDefaults();
                    onepoly.Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE;
                    vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(onepoly);
                    vdFC1.BaseControl.ActiveDocument.Redraw(true);
                    onepoly = new VectorDraw.Professional.vdFigures.vdPolyline();
                    if (completeOnepolygon == 4)
                        completeOnepolygon = 0;
                }
            }
        }
        protected static bool IsRoute = false;
        protected static bool IsRelocate = false;

        private void TextInsert(double x, double y, string txt, out int id, bool isSegment = false, vdColor vdColor = null)
        {
            VectorDraw.Professional.vdFigures.vdText text = new VectorDraw.Professional.vdFigures.vdText();
            text.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
            text.setDocumentDefaults();
            text.TextString = txt;
            text.Height = 250.0;
            if (vdColor == null)
            {
                if (isSegment)
                    text.PenColor = new vdColor(Color.White); //new vdColor(Color.Cyan);
                else
                    text.PenColor = new vdColor(Color.Red);
                text.InsertionPoint = new VectorDraw.Geometry.gPoint(x - 200, y - 200);
            }
            else
            {
                text.PenColor = vdColor;
                text.InsertionPoint = new VectorDraw.Geometry.gPoint(x - 200, y);
            }
            id = text.Id;
            vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(text);
            vdFC1.BaseControl.ActiveDocument.Redraw(true);
        }

        public static bool contains(Vertexes _pts, gPoint pt)
        {
            bool isIn = false;

            int NumberOfPoints = _pts.Count;
            if (true)
            {
                int i, j = 0;
                for (i = 0, j = NumberOfPoints - 1; i < NumberOfPoints; j = i++)
                {
                    if (
                    (
                    ((_pts[i].y <= pt.y) && (pt.y <= _pts[j].y)) || ((_pts[j].y <= pt.y) && (pt.y <= _pts[i].y))
                    ) &&
                    (pt.x <= (_pts[j].x - _pts[i].x) * (pt.y - _pts[i].y) / (_pts[j].y - _pts[i].y) + _pts[i].x)
                    )
                    {
                        isIn = !isIn;
                    }
                }
            }
            return isIn;
        }

        private void AddLayer(Control ctl, DataGridView dgv)
        {
            int ii = 0;
            DestinationBind(ref ii);
            var dgr = dgv.CurrentCell;
            if (dgr == null || dgv.Rows.Count == 0 || dgr.Value == null)
            {
                return;
            }
            string newlyrIns = dgr.Value.ToString();
            if (ctl is ListBox lsb)
            {
                if (!String.IsNullOrEmpty(newlyrIns) && !lsb.Items.OfType<Layer>().Where(i => i.ToString().Contains(newlyrIns)).Any())
                {
                    lsb.Items.Add(new Layer(vdFC1.BaseControl.ActiveDocument.Layers.FindName(newlyrIns), vdFC1.ScrollableControl, routeInfo));
                    lsb.Update();
                }
            }
            if (ctl is DataGridView dgvList)
            {
                if (!String.IsNullOrEmpty(newlyrIns))
                {
                    foreach (DataGridViewRow dr in dgvList.Rows)
                    {
                        if ((dr.Cells["dgv_Layer"].Value as Layer).name == newlyrIns)
                            return;
                    }
                    dgvList.Rows.Add(new object[] {
                        new Layer(vdFC1.BaseControl.ActiveDocument.Layers.FindName(newlyrIns), vdFC1.ScrollableControl, routeInfo),
                        "" });
                    dgvList.Update();
                }
            }
            instListbox.RefreshEdit();
            instListbox.Refresh();
            instListbox.Update();

        }
        public void addLyr_Click(object sender, EventArgs e)
        {


            AddLayer(instListbox, instLyrDGV);
            if (instListbox.Rows.Count > 0)
                instListbox.Rows[0].Selected = true;

        }

        private void removeLyr_Click_1(object sender, EventArgs e)
        {
            if (instListbox.Rows.Count == 0)
                return;
            //if (instListbox.CurrentCell == null)
            //    instListbox.Rows[0].Selected = true;

            var index = instListbox.SelectedCells[0].RowIndex;
            var lst = instListbox.Rows[index];

            if (lst == null)
            {
                return;
            }
            else
            {
                instListbox.Rows.Remove(lst);
            }
            if (instListbox.Rows.Count > 0)
                instListbox.Rows[0].Selected = true;
        }

        private void addObsLyr_Click(object sender, EventArgs e)
        {
            AddLayer(ObstListbox, obstLyrDGV);
            if (instListbox.Rows.Count > 0)
                instListbox.Rows[0].Selected = true;
        }

        private void removeObsLyr_Click_1(object sender, EventArgs e)
        {
            var lst = ObstListbox.SelectedItem;

            if (lst == null)
            {
            }
            else
            {
                ObstListbox.Items.Remove(lst);
            }
            if (ObstListbox.Items.Count > 0)
                ObstListbox.SelectedIndex = 0;
        }

        public static DataTable dtOptimalResult;

        private gPoint Gp(string pts)
        {
            return new gPoint(Convert.ToDouble(pts.Split(',')[0].ToString()), Convert.ToDouble(pts.Split(',')[1].ToString()), Convert.ToDouble(pts.Split(',')[2].ToString()));
        }

        private string GetFileName(string fileName = "")
        {
            return fileName + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "").Replace(":", "").Replace("-", "").Replace("/", "");
        }

        private void btConfirm_Click(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            RefreshCADSpace();
            if (!routeInfo.CheckDuplicatedInstrument(vdFramedControl))
            {
                DuplicateItem duplicateItem = new DuplicateItem(routeInfo, vdFramedControl);
                // duplicateItem.Show();
                duplicateItem.Owner = this;
                duplicateItem.Show();
                return;
            }
            IsInstrumentBox = false;
            if (instListbox.Rows.Count > 0)
            {
                // routeInfo.LstLayer = routeInfo.GetAllLayer(vdFramedControl, routeInfo, instListbox, ObstListbox);
                routeInfo.LstLayer = new List<Layer>();
                foreach (DataGridViewRow dr in instListbox.Rows)
                {
                    routeInfo.LstLayer.Add(dr.Cells["dgv_Layer"].Value as Layer);
                }
                routeInfo.LstallInstruments = routeInfo.GetAllInstrument(vdFramedControl, routeInfo);
                routeInfo.DGV_LstInstrument = null;
                BindInsCombo();
                int r1 = 0;
                string t1 = "";
                string s1 = "";
                DestinationBind(ref r1);
                TypeBind(ref t1);
                SystemBind(ref s1);
                var lstemp = new List<Instrument>();
                foreach (DataGridViewRow dr in instListbox.Rows)
                {
                    var lyr = new Layer(vdFC1.BaseControl.ActiveDocument.Layers.FindName((dr.Cells["dgv_Layer"].Value as Layer).name), vdFC1.ScrollableControl, routeInfo);
                    foreach (var ins in lyr.listInstrument)
                    {
                        if (ins.OwnerInsert.Deleted || ins.OwnerInsert.Layer.Name != lyr.name) continue;
                        lstemp.Add(ins);
                        InstrumentInfoEntity instrumentEntity = new InstrumentInfoEntity();
                        instrumentEntity.GUID = ins.guid;
                        //instrumentEntity.Iv = ins.OwnerInsert.Id;
                        instrumentEntity.T1 = ins.t1;
                        instrumentEntity.T2 = ins.t2;
                        //instrumentEntity.To = r1;
                        instrumentEntity.System = s1;
                        instrumentEntity.Type = t1;
                        instrumentEntity.Instrument = ins;
                        instrumentEntity.LayerName = lyr.name;
                        if (routeInfo.DGV_LstInstrument == null)
                        {
                            routeInfo.DGV_LstInstrument = new List<InstrumentInfoEntity>();
                        }
                        routeInfo.DGV_LstInstrument.Add(instrumentEntity);
                    }
                }

            }
            //  SetInsCable();
            isTriggerConfirmedClick = true;
            BindGrid();
            updateInstGrid();
            instDGV_CellEndEdit(null, null);
            isTriggerConfirmedClick = false;
            //  SetInsCable();
        }
        bool isTriggerConfirmedClick = false;
        public void DestinationBind(ref int r1)
        {
            if (routeInfo == null) return;
            if (routeInfo.LstTBBOXes.Count > 0)
            {
                r1 = routeInfo.LstTBBOXes[0].guid;
                DataTable type = new DataTable();
                type.Columns.Add("GUID");
                type.Columns.Add("To");
                type.Rows.Add(null, -1);
                foreach (var t in routeInfo.LstTBBOXes)
                    //if (t.IsIO != eDestinationType.MCC) 
                    type.Rows.Add(t.Name, t.guid);

                //Ins
                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instDGV.Columns["colTo"];
                col.DisplayMember = "GUID";
                col.ValueMember = col.DataPropertyName = "To";
                ((DataGridViewComboBoxColumn)instDGV.Columns["colTo"]).DataSource = type;

                //cbo_tbboxes_alternative.DisplayMember = "GUID";
                //cbo_tbboxes_alternative.ValueMember = col.DataPropertyName = "To";
                //cbo_tbboxes_alternative.DataSource = type;

                DataTable type1 = new DataTable();
                type1.Columns.Add("GUID");
                type1.Columns.Add("To");
                type1.Rows.Add(null, -1);
                foreach (var t in routeInfo.LstTBBOXes)
                    //if (t.IsIO != eDestinationType.MCC) 
                    type1.Rows.Add(t.Name, t.guid);
                DataGridViewComboBoxColumn col1 = (DataGridViewComboBoxColumn)instListbox.Columns["Cable1"];
                col1.DisplayMember = "GUID";
                col1.ValueMember = col1.DataPropertyName = "To";
                col1.DataSource = type1;




            }
            instDGV.Update();
            instListbox.Update();
        }
        public void TypeBind(ref string t1)
        {
            DataTable type = new DataTable();
            type.Columns.Add("Type");
            BL.SettingBL sbl = new BL.SettingBL();
            if (sbl.GetInstrumetnType().Count > 0)
            {
                t1 = sbl.GetInstrumetnType()[0].Classification_3;
                foreach (var t in sbl.GetInstrumetnType()) type.Rows.Add(t.Classification_3);
                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instDGV.Columns["colType"];
                col.ValueMember = col.DisplayMember = col.DataPropertyName = "Type";
                ((DataGridViewComboBoxColumn)instDGV.Columns["colType"]).DataSource = type;

            }
        }
        public void TypeBind_Layer(ref string t1)
        {
            DataTable type = new DataTable();
            type.Columns.Add("Type");
            BL.SettingBL sbl = new BL.SettingBL();
            if (sbl.GetInstrumetnType().Count > 0)
            {
                t1 = sbl.GetInstrumetnType()[0].Classification_3;
                foreach (var t in sbl.GetInstrumetnType())
                    type.Rows.Add(t.Classification_3);
                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instListbox.Columns["dgv_Type_Ins"];
                col.ValueMember = col.DisplayMember = col.DataPropertyName = "Type";
                ((DataGridViewComboBoxColumn)instListbox.Columns["dgv_Type_Ins"]).DataSource = type;
            }
        }
        public void SystemBind_Layer(ref string s1)
        {
            DataTable system = new DataTable();
            system.Columns.Add("dgv_System");
            BL.SettingBL sbl = new BL.SettingBL();
            if (sbl.GetSystemList().Count > 0)
            {
                s1 = sbl.GetSystemList()[0].Title;
                foreach (var t in sbl.GetSystemList())
                    system.Rows.Add(t.Title);
                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"];
                col.ValueMember = col.DisplayMember = col.DataPropertyName = "dgv_System";
                ((DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"]).DataSource = system;
            }
            //DataTable system = new DataTable();
            //system.Columns.Add("System");
            //system.Columns.Add("dgv_System");
            //BL.SettingBL sbl = new BL.SettingBL();
            //if (sbl.GetSystemList().Count > 0)
            //{
            //    s1 = sbl.GetSystemList()[0].Title; 
            //    foreach (var t in sbl.GetSystemList())
            //    {
            //        system.Rows.Add(t.Id, t.Title); 
            //    }
            //    DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"];
            //    col.ValueMember = col.DataPropertyName = "System";
            //    col.DisplayMember = "dgv_System";

            //    ((DataGridViewComboBoxColumn)instListbox.Columns["dgv_System"]).DataSource = system;
            //}
        }
        public void SystemBind(ref string s1)
        {
            DataTable system = new DataTable();
            system.Columns.Add("System");
            BL.SettingBL sbl = new BL.SettingBL();
            if (sbl.GetSystemList().Count > 0)
            {
                s1 = sbl.GetSystemList()[0].Title;
                foreach (var t in sbl.GetSystemList()) system.Rows.Add(t.Title);
                DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instDGV.Columns["colSystem"];
                col.ValueMember = col.DisplayMember = col.DataPropertyName = "System";
                ((DataGridViewComboBoxColumn)instDGV.Columns["colSystem"]).DataSource = system;
            }
            //DataTable system = new DataTable();
            //system.Columns.Add("System");
            //system.Columns.Add("colSystem");
            //BL.SettingBL sbl = new BL.SettingBL();
            //if (sbl.GetSystemList().Count > 0)
            //{
            //    s1 = sbl.GetSystemList()[0].Title; 
            //    foreach (var t in sbl.GetSystemList())
            //    {
            //        system.Rows.Add(t.Id, t.Title);
            //    }
            //    DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)instDGV.Columns["colSystem"]; 
            //    col.ValueMember = col.DataPropertyName = "System";
            //    col.DisplayMember = "colSystem";

            //    ((DataGridViewComboBoxColumn)instDGV.Columns["colSystem"]).DataSource = system;
            //}
        }
        List<string> ListComboAllDestination = new List<string>();
        private bool DrawPolyObs()
        {
            IsManualDrawn = true;
            gPoint userPoint;

            vdFC1.BaseControl.ActiveDocument.Prompt("Select a Point");

            StatusCode ret =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");
            if (userPoint == null) return false;
            var userPoint2 = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRefPointLine(userPoint) as gPoint;
            if (userPoint2 == null) return false;
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");

            gPoints gP = new gPoints();
            gP.Add(userPoint);
            gP.Add(userPoint2);
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
            var userPoint3 = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRefPointLine(userPoint2) as gPoint;
            if (userPoint3 == null) return false;
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");
            gP.Add(userPoint3);
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
            var userPoint4 = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRefPointLine(userPoint3) as gPoint;
            if (userPoint4 == null) return false;
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");
            gP.Add(userPoint4);
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
            gP.Add(userPoint);
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
            vdFC1.BaseControl.ActiveDocument.Update();
            vdFC1.BaseControl.ActiveDocument.Redraw(true);


            IsManualDrawn = false;
            return true;
        }
        private void DrawRecObs()
        {
            IsManualDrawn = true;
            gPoint userPoint;

            vdFC1.BaseControl.ActiveDocument.Prompt("Select a Point");

            StatusCode ret =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");

            object ret2 =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRect(userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);

            Vector v = ret2 as Vector;

            if (v != null)
            {
                double angle = v.x;
                double width = v.y;
                double height = v.z;

                gPoint userpoint2 = userPoint.Polar(0.0, width);

                userpoint2 = userpoint2.Polar(VectorDraw.Geometry.Globals.HALF_PI, height);
                // this.status = STATUS_IOROOM;
                //vdFC1.BaseControl.ActiveDocument.CommandAction.CmdRect(userPoint, userpoint2);

                gPoints gp = new gPoints();
                gp.Add(userPoint);
                gp.Add(userpoint2);
                gp = gp.GetBox().GetPoints();
                gp.Add(gp.GetBox().GetPoints()[0]);
                vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gp);
                //if (vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gp) && !string.IsNullOrEmpty(TBName))
                //{
                //    var midPt = gp.GetBox().MidPoint;
                //    if (TBName != "")
                //        TextInsert(midPt.x, midPt.y, TBName, false, routeInfo.MainDestination.polyline.Layer.PenColor);
                //}
            }
            IsManualDrawn = false;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
            if (routeInfo == null) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.OBSTACLE;

            if (string.IsNullOrEmpty(cboObst.Text) || string.IsNullOrEmpty(cbo_polyTypes.Text))
            {
                MessageBox.Show("입력 값을 확인해주세요.");
                return;
            }
            SetCustomLayer(cboObst.Text);
            bool res = false;
            if (cbo_polyTypes.Text == "Poly")
            {
                res = DrawPolyObs();
                if (!res)
                {
                    foreach (var lst in lstPolyIDTemp)
                    {
                        vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(lst);
                        routeInfo.SyncronyzeRelatedUI(new List<int> { lst.Id }, vdFC1);
                    }
                    lstPolyIDTemp.Clear();
                }
            }
            if (cbo_polyTypes.Text == "Rect")
                DrawRecObs();
            //if (cbo_polyTypes.Text == "Circle")
            //    DrawCircleObs();

            // routeInfo.DoCheckOverlapping(vdFC1);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (routeInfo == null || GetMainBoundary() == null)
            {
                MessageBox.Show("외각 라인 설정이 필요합니다. 외각라인 추가를 해주시거나, 외각 라인이 있는 캐드 파일을 재입력해주세요. 외각 라인이 Boundary 레이어 내에 폴리라인으로 있을 경우 자동으로 인식됩니다.");
                return;
            }
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.DESTINSTION;
            if (string.IsNullOrEmpty(cboDest.Text) || string.IsNullOrEmpty(cboDestType.Text))
            {
                MessageBox.Show("레이어와 목적지를 선택해주세요.");
                return;
            }
            int selectedCboDestTypeIndex = cboDestType.SelectedIndex;
            if (!chk_Manual.Checked)
            {
                DrawDestination();
                routeInfo.DoCheckOverlapping(vdFC1);
            }
            else
            {
                DrawDestination_Manual();
                routeInfo.DoCheckOverlapping(vdFC1);
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (routeInfo == null || routeInfo.DGV_LstInstrument == null || routeInfo.DGV_LstInstrument.Count == 0)
            {
                MessageBox.Show("인식된 계측기기가 없습니다. 다시 한 번 확인해주세요.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("T1");
            dt.Columns.Add("T2");
            dt.Columns.Add("Type");
            dt.Columns.Add("System");
            dt.Columns.Add("To");
            if (!Directory.Exists(DataSourceInstrumentSchedule))
            {
                Directory.CreateDirectory(DataSourceInstrumentSchedule);
            }
            int o = 0;
            foreach (var dgv in routeInfo.DGV_LstInstrument)
            {
                o++;
                dt.Rows.Add(
                    new object[] {
                    o.ToString(),
                    dgv.T1,
                    dgv.T2,
                    dgv.Type,
                    dgv.System,
                    dgv.To
                    }
                    );
            }
            using (var writer = new StreamWriter(DataSourceInstrumentSchedule + GetFileName("InstrumentSchedule_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<DetectedInstrumentList> lst = new List<DetectedInstrumentList>();
                DetectedInstrumentList ie;
                foreach (DataRow dr in dt.Rows)
                {
                    ie = new DetectedInstrumentList();
                    ie.No = dr["No"].ToString().Trim();
                    ie.T1 = dr["T1"].ToString().Trim();
                    ie.T2 = dr["T2"].ToString().Trim();
                    ie.Type = dr["Type"].ToString().Trim();
                    ie.System = dr["System"].ToString().Trim();
                    try
                    {
                        if (dr["To"] != null && Int32.TryParse(dr["To"].ToString(), out int r))
                        {
                            ie.To = routeInfo.FindTBBox(Convert.ToInt32(dr["To"].ToString().Trim())).Name;
                        }
                    }
                    catch
                    {

                    }
                    lst.Add(ie);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();

            }
            MessageBox.Show("선택한 계측기기 데이터가 성공적으로 추출되었습니다.");
            System.Diagnostics.Process.Start(DataSourceInstrumentSchedule);
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }
        private void button7_Click(object sender, EventArgs e)
        {

            if (routeInfo == null || string.IsNullOrEmpty(cboBoun.Text)) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.BOUNDARY;
            routeInfo.MainBoundary = GetMainBoundary();
            if (routeInfo.MainBoundary != null) return;
            SetCustomLayer(cboBoun.Text);
            gPoint userPoint;

            vdFC1.BaseControl.ActiveDocument.Prompt("Select a Point");
            StatusCode ret =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");

            object ret2 =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRect(userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);

            Vector v = ret2 as Vector;

            if (v != null)
            {
                double angle = v.x;
                double width = v.y;
                double height = v.z;
                if (userPoint == null) return;
                gPoint userpoint2 = userPoint.Polar(0.0, width);

                userpoint2 = userpoint2.Polar(VectorDraw.Geometry.Globals.HALF_PI, height);
                //this.status = STATUS_IOROOM;
                gPoints gp = new gPoints();
                gp.Add(userPoint);
                gp.Add(userpoint2);
                gp = gp.GetBox().GetPoints();
                gp.Add(gp.GetBox().GetPoints()[0]);
                //PreVPolyLine = gp;
                if (vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gp))
                {
                    var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                    if (et.FindFromId(routeInfo.PreviousVdFigure) is vdPolyline p)
                    {
                        routeInfo.MainBoundary = p;
                    }
                    //routeInfo.DoCheckOverlapping(vdFC1, true); //PTK commented for allowing boundable state even inside Objects 
                }

            }
        }
        private void MainP1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            instDGV.AutoGenerateColumns = false;
            dgv_alternaitve.AutoGenerateColumns = false;
            dgv_InsAnalysis.AutoGenerateColumns = false;
            dgv_SegAnalysis.AutoGenerateColumns = false;
            dgv_1.AutoGenerateColumns = false;
            dgv_2.AutoGenerateColumns = false;
            dgv_3.AutoGenerateColumns = false;

            //lblInsertName.Text = "TAG No.";
            vdFC1.BaseControl.GripSelectionModified += BaseControl_GripSelectionModified;
            vdFC1.BaseControl.AfterOpenDocument += BaseControl_AfterOpenDocument;
            vdFC1.BaseControl.EraseObject += BaseControl_EraseObject;
            vdFC1.BaseControl.ActionStart += BaseControl_ActionStart;
            //vdFC1.BaseControl.DrawAfter += BaseControl_DrawAfter;
            vdFC1.BaseControl.ActiveDocument.OnCommandExecute += ActiveDocument_OnCommandExecute1;
            vdFC1.BaseControl.vdKeyUp += BaseControl_vdKeyUp;
            vdFC1.BaseControl.ActiveDocument.ActionStart += ActiveDocument_ActionStart1;
            vdFC1.BaseControl.ActionEnd += ActiveDocument_ActionEnd;
            vdFC1.BaseControl.MouseDown += BaseControl_MouseDown;
            this.MouseDown += BaseControl_MouseDown;
            vdFC1.SetStatusBarOption(vdControls.vdFramedControl.StatusBarOptions.Menu, false);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.StatusBar, false);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.LayoutPopupMenu, false);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.CommandLine, true);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.VericalScroll, false);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.HorizodalScroll, false);
            vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, false);
            tab_Analysis.Click += Tab_Analysis_Click;

            AdjustColumnSize(new DataGridView[] { dgv_SegAnalysis, dgv_InsAnalysis, dgv_SubIns, dgv_SubSeg, instListbox, dgv_1, dgv_2, dgv_3 });

            //dgv_InsAnalysis
            if (!System.IO.Directory.Exists(DataSourceXmlBuffer))
            {
                MakeDirectory(DataSourceXmlBuffer);
            }
            BindFrequency();
            progressBar1.Visible = false;

        }
        private void BaseControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (di != null)
            {
                di.BringToFront();
            }
        }

        private void BindFrequency()
        {
            DataTable dtFre = new DataTable();
            dtFre.Columns.Add("cboFrequency");
            dtFre.Rows.Add(new object[] { 100 });
            dtFre.Rows.Add(new object[] { 200 });
            dtFre.Rows.Add(new object[] { 300 });
            dtFre.Rows.Add(new object[] { 400 });
            dtFre.Rows.Add(new object[] { 500 });
            dtFre.Rows.Add(new object[] { 600 });
            dtFre.Rows.Add(new object[] { 700 });
            dtFre.Rows.Add(new object[] { 800 });
            dtFre.Rows.Add(new object[] { 900 });
            dtFre.Rows.Add(new object[] { 1000 });
            cbo_Frequency.DisplayMember = cbo_Frequency.ValueMember = "cboFrequency";
            cbo_Frequency.DataSource = dtFre;
            cbo_Frequency.SelectedIndex = 0;


            DataTable dtColor = new DataTable();
            dtColor.Columns.Add("cboColor");
            dtColor.Rows.Add(new object[] { Color.Cyan.Name.ToString() });
            dtColor.Rows.Add(new object[] { Color.Yellow.Name.ToString() });
            dtColor.Rows.Add(new object[] { Color.Green.Name.ToString() });
            dtColor.Rows.Add(new object[] { Color.Red.Name.ToString() });
            dtColor.Rows.Add(new object[] { Color.Blue.Name.ToString() });
            dtColor.Rows.Add(new object[] { Color.White.Name.ToString() });
            cbo_Colorhighlight.DisplayMember = cbo_Colorhighlight.ValueMember = "cboColor";
            cbo_Colorhighlight.DataSource = dtColor;
            cbo_Colorhighlight.SelectedIndex = 0;


            DataTable dtLineWeight = new DataTable();
            dtLineWeight.Columns.Add("cboLineWeight");
            dtLineWeight.Rows.Add(new object[] { "25" });
            dtLineWeight.Rows.Add(new object[] { "50" });
            dtLineWeight.Rows.Add(new object[] { "100" });
            dtLineWeight.Rows.Add(new object[] { "150" });
            dtLineWeight.Rows.Add(new object[] { "200" });
            dtLineWeight.Rows.Add(new object[] { "250" });
            dtLineWeight.Rows.Add(new object[] { "300" });
            cbo_MaxLineWeight.DisplayMember = cbo_MaxLineWeight.ValueMember = "cboLineWeight";
            cbo_MaxLineWeight.DataSource = dtLineWeight;
            cbo_MaxLineWeight.SelectedIndex = 2;

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
        public void MakeDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                try
                {
                    if (!UseAdmin) // use admin
                    {
                        var p = "'" + dir + "'";
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            CreateNoWindow = true,
                            FileName = "powershell.exe",
                            Arguments = $"mkdir \"{p}\"",
                            Verb = "runas"
                        };

                        Process.Start(psi);
                    }
                }
                catch (Exception exx)
                {
                    DebugLog.WriteLog(ex);
                    // MessageBox.Show(exx.StackTrace);
                }
            }
        }
        private void BaseControl_vdKeyUp(KeyEventArgs e, ref bool cancel)
        {
            if (routeInfo == null) return;
            if (e.KeyData == Keys.Escape)
            {
                foreach (var f in routeInfo.LstallInstruments)//vdFC1.BaseControl.ActiveDocument.ActionLayout.Entities
                {
                    if (f is Instrument vs)
                    {
                        var obj = vdFC1.BaseControl.ActiveDocument.ActionLayout.Entities.FindFromId(vs.OwnerInsert.Id);
                        if (obj != null)
                            obj.HighLight = false;
                    }
                }
                vdFC1.BaseControl.ActiveDocument.Redraw(true);
                vdFC1.BaseControl.ActiveDocument.Update();
                vdFC1.BaseControl.ActiveDocument.Prompt(null);
            }
        }

        private void ActiveDocument_ActionEnd(object sender, string actionName)
        {
            if (actionName == "BaseAction_ActionGetRectFromPointSelectDCS")
            {
                return;
            }
        }
        private void ActiveDocument_ActionStart1(object sender, string actionName, ref bool cancel)
        {
            if (actionName == "BaseAction_ActionGetRectFromPointSelectDCS")
            {
                return;
            }
        }

        private void ActiveDocument_OnCommandExecute1(object sender, string parseString, ref bool succeed)
        {

        }
        List<int> LstSelectedID = new List<int>();
        private void BaseControl_EraseObject(object sender, ref bool cancel)
        {
            if (sender is vdInsert)
            {
                LstSelectedID.Add((sender as vdInsert).Id);
            }
            else
            {
                if (sender is vdFigure vf)
                    LstSelectedID.Add((vf).Id);
            }

        }

        private void BaseControl_AfterOpenDocument(object sender)
        {

        }

        private void BaseControl_GripSelectionModified(object sender, vdLayout layout, vdSelection gripSelection)
        {
            if (routeInfo is null) return;
            routeInfo.LstHighlightedRoute.Clear();
            if (gripSelection == null || gripSelection.Count == 0)
            {
                foreach (vdFigure val in vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems())
                {
                    if (val is vdLine vs)
                    {
                        vs.ToolTip = null;
                    }
                }
            }
            if (btn_Measure.Text == "선 길이 측정 - On")
            {
                var value = 0.0;
                foreach (vdFigure val in gripSelection)
                {
                    if (val is vdLine vl)
                    {
                        value += Math.Round(vl.Length() / 1000, 2);
                        vl.ToolTip = value + " m";
                    }
                }

                return;
            }
            foreach (vdFigure val in gripSelection)
            {

                if (val is vdInsert vs && vs.Block.Name == "TAG No.")
                {
                    var v1 = "";
                    var v2 = "";
                    if (vs.Attributes.Count > 0)
                    {

                        foreach (vdAttrib vda in vs.Attributes)
                        {

                            if (vda.TagString == "T1")
                            {
                                v1 = vda.ValueString.ToString().Trim();
                            }
                            if (vda.TagString == "T2")
                            {
                                v2 = vda.ValueString.ToString().Trim();
                            }
                        }
                    }
                    foreach (DataGridViewRow gr in instDGV.Rows)
                    {

                        var t1 = gr.Cells["colT1"].Value.ToString().Trim();
                        var t2 = gr.Cells["colT2"].Value.ToString().Trim();
                        if (t1 == v1 && t2 == v2)
                        {
                            //gr.DefaultCellStyle.BackColor = Color.Yellow;
                            gr.Cells["colT2"].Style.BackColor = Color.Yellow;
                            gr.Cells["colT1"].Style.BackColor = Color.Yellow;
                            ////gr.Cells["colType"].Style.BackColor = Color.Yellow;
                            ////gr.Cells["colSystem"].Style.BackColor = Color.Yellow;
                            ////gr.Cells["colTo"].Style.BackColor = Color.Yellow;
                            gr.Cells["col_Check_Instrument"].Value = true;

                        }
                        else
                        {
                            //  gr.Cells["col_Check_Instrument"].Value = false;

                            gr.DefaultCellStyle.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                        }

                    }
                }
            }
            if (gripSelection == null || gripSelection.Count == 0)
            {
                foreach (DataGridViewRow gr in instDGV.Rows)
                {
                    gr.Cells["colT2"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    gr.Cells["colT1"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    gr.Cells["colType"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    gr.Cells["colSystem"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    gr.Cells["colTo"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    gr.Cells["col_Check_Instrument"].Value = false;

                }
                foreach (vdFigure val in vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems())
                {
                    if (val is vdInsert vs && vs.Block.Name == "TAG No.")
                    {
                        val.HighLight = false;
                    }
                }
            }
            //foreach (var pl in routeInfo.LstHighlightedRoute)
            //{
            //    pl.HighLight = true;
            //    pl.PenWidth = 5;
            //    pl.PenWidth = 15;
            //}

            vdFC1.BaseControl.ActiveDocument.ActiveLayOut.RefreshGraphicsControl(vdFC1.BaseControl.ActiveDocument.ActiveLayOut.ActiveActionRender.control);
            vdFC1.BaseControl.Redraw();
        }
        bool IsMainRouteIntersect = false;
        private void BaseControl_AfterAddItem(object obj)
        {
            if (routeInfo == null) return;
            IsMainRouteIntersect = false;
            routeInfo.PreviousVdFigure = 0;
            if (obj is vdPolyline vdPoly) // Desti/ Obs / Bound / Ins
            {
                switch (routeInfo.CURRENT_MODE)

                {
                    case RouteInfo.eACTION_MODE.BOUNDARY:
                        {
                            if (vdPoly.Layer.Name.ToLower().Contains("boundary"))
                            {
                                bool IsLegal = true;
                                routeInfo.ExecuteBoundaryDM(vdPoly, vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems(), ref IsLegal);
                                IsDeleted = !IsLegal;
                                if (IsDeleted)
                                {
                                    System.Windows.Forms.MessageBox.Show("생성하신 폴리라인이 기존 객체와 교차됩니다. 다시 한 번 생성해주세요.");
                                    this.vdFC1.BaseControl.ActiveDocument.Update();
                                    this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
                                }
                            }
                            break;
                        }
                    case RouteInfo.eACTION_MODE.INSTRUMENT:
                        {

                        }
                        break;
                    case RouteInfo.eACTION_MODE.OBSTACLE:
                        {
                            if (vdPoly.GetGripPoints().Count < 5)
                                lstPolyIDTemp.Add(vdPoly);
                            if (vdPoly.GetGripPoints().Count == 5)
                            {
                                foreach (var lst in lstPolyIDTemp)
                                    vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(lst);
                                lstPolyIDTemp.Clear();
                            }
                            break;
                        }
                    case RouteInfo.eACTION_MODE.DESTINSTION:
                        {
                            bool IsLegal = true;
                            routeInfo.ExecuteDestinationDM(vdPoly, vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems(), ref IsLegal);
                            // IsDeleted = !IsLegal; 
                            if (!IsLegal)
                            {
                                System.Windows.Forms.MessageBox.Show("생성하신 폴리라인이 기존 객체와 교차됩니다. 다시 한 번 생성해주세요.");
                                TBName = "";
                                this.vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.RemoveItem(vdPoly);
                                vdFC1.BaseControl.Update();
                                this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
                                return;
                            }
                            DestinationSettingEntity destinationSettingEntity = new DestinationSettingEntity();
                            destinationSettingEntity.TBBOXDestination = null;
                            destinationSettingEntity.RouteInfo = routeInfo;
                            int selectedCboDestTypeIndex = cboDestType.SelectedIndex;
                            vdFigureTemp = vdPoly;
                            var tbox = routeInfo.FindTBBox(vdPoly.Id);
                            if (selectedCboDestTypeIndex == 0)
                            {
                                destinationSettingEntity.TBBOXDestination = new TBBOXDestination();
                                //IoDetails Des = new IoDetails(destinationSettingEntity);
                                Ior Des = new Ior(destinationSettingEntity);
                                var res = Des.ShowDialog();
                                if (res == DialogResult.OK)
                                {
                                    tbox.IsIO = eDestinationType.IORoom;
                                    var name = Des.dse.TBBOXDestination.Name;
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        return;
                                    }
                                    TBName = routeInfo.TBABBRE + name;
                                }
                                else
                                {
                                    TBName = "";
                                    LstSelectedID.Add(vdPoly.Id);
                                }
                            }
                            else if (selectedCboDestTypeIndex == 1)
                            {
                                DestSetForm Des = new DestSetForm(destinationSettingEntity);
                                Des.Owner = this;

                                var res = Des.ShowDialog();
                                if (res == DialogResult.OK)
                                {
                                    var name = Des.des.TBBOXDestination.Name.Replace(string.IsNullOrEmpty(routeInfo.TBABBRE.ToString()) ? "____" : routeInfo.TBABBRE, "");
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        return;
                                    }
                                    tbox.IsIO = eDestinationType.TBBox;
                                    TBName = routeInfo.TBABBRE + name;
                                    tbox.AutoCheck = Des.des.TBBOXDestination.AutoCheck;
                                    tbox.EachCheck = Des.des.TBBOXDestination.EachCheck;
                                    tbox.CableType = Des.des.TBBOXDestination.CableType;
                                    tbox.OwnDestination = Des.des.TBBOXDestination.OwnDestination;
                                    tbox.LstmCCEntities = Des.des.TBBOXDestination.LstmCCEntities;

                                }
                                else
                                {
                                    TBName = "";
                                    LstSelectedID.Add(vdPoly.Id);
                                }
                            }
                            else if (selectedCboDestTypeIndex == 2)
                            {
                                MCCSetForm Des = new MCCSetForm(destinationSettingEntity);
                                Des.Owner = this;

                                var res = Des.ShowDialog();
                                if (res == DialogResult.OK)
                                {
                                    var name = Des.des.TBBOXDestination.Name.Replace(string.IsNullOrEmpty(routeInfo.TBABBRE.ToString()) ? "____" : routeInfo.TBABBRE, "");
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        return;
                                    }
                                    tbox.IsIO = eDestinationType.MCC;
                                    TBName = routeInfo.TBABBRE + name;
                                    tbox.AutoCheck = Des.des.TBBOXDestination.AutoCheck;
                                    tbox.EachCheck = Des.des.TBBOXDestination.EachCheck;
                                    tbox.LstmCCEntities = Des.des.TBBOXDestination.LstmCCEntities;
                                    tbox.LstmCCEntitiesHeader = Des.des.TBBOXDestination.LstmCCEntitiesHeader;
                                    tbox.OwnDestination = null;
                                }
                                else
                                {
                                    TBName = "";
                                    LstSelectedID.Add(vdPoly.Id);
                                }
                            }
                            int r1 = 0;
                            DestinationBind(ref r1);
                            RefreshCADSpace();
                            break;
                        }
                    case RouteInfo.eACTION_MODE.MAINROUTE:
                        {
                            vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                            vdPoly.PenColor.ByColorIndex = true;
                            vdPoly.PenColor.ColorIndex = 1;
                            vdPoly.PenWidth = 30;

                            //if (tbTemp == null)
                            //{
                            //    routeInfo.LstExternalRoute.Add(vdPoly);
                            //    // break;
                            //}
                            //else
                            //{
                            //    routeInfo.FindIO_TBBox(tbTemp.guid).MainRouteCollection.Add(vdPoly);
                            //}
                            foreach (var lst in lstPolyIDTemp)
                            {
                                vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(lst);
                                routeInfo.SyncronyzeRelatedUI(new List<int>() { lst.Id }, vdFC1);
                            }
                            lstPolyIDTemp.Clear();

                            lstPolyIDTemp.Add(vdPoly);
                            routeInfo.PreviousVdFigure = vdPoly.Id;
                            break;
                        }
                    default:
                        break;
                }

                this.vdFC1.BaseControl.ActiveDocument.Update();
                this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
            }
            if (obj is vdLine p)
            {
                switch (routeInfo.CURRENT_MODE)
                {
                    case RouteInfo.eACTION_MODE.LINE:
                        {
                            vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                            p.PenColor.ByColorIndex = true;
                            p.PenColor.ColorIndex = 1;
                            p.PenWidth = 30;
                            var tbFirstPoint = routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(p.getStartPoint()) == true);
                            var tbSecondPoint = routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(p.getEndPoint()) == true);
                            if (!tbFirstPoint.Any() && !tbSecondPoint.Any())
                            {
                                if (routeInfo.LstExternalRoute == null)
                                    routeInfo.LstExternalRoute = new List<vdLine>();
                                routeInfo.LstExternalRoute.Add(p);
                                LstTempvdline.Add(p);
                            }
                            else if (tbFirstPoint.Any() && !tbSecondPoint.Any())
                            {
                                var tbm = tbFirstPoint.FirstOrDefault().MainRouteCollection;
                                if (tbm == null)
                                    tbm = new List<vdLine>();
                                tbm.Add(p);
                                LstTempvdline.Add(p);

                                //**** This should surely add "one end is for first tb and other end is for second point" and direct vdline with 2 TB boxes  if time is available add this
                                //**** PTK reminder 2023/06/17 00:20 AM

                            }
                            else if ((!tbFirstPoint.Any() && tbSecondPoint.Any()))
                            {
                                var tbm = tbSecondPoint.FirstOrDefault().MainRouteCollection;
                                if (tbm == null)
                                    tbm = new List<vdLine>();
                                var ep = p.getEndPoint();
                                var sp = p.getStartPoint();
                                var newVdline = new vdLine(vdFC1.BaseControl.ActiveDocument);
                                newVdline.EndPoint = sp;
                                newVdline.StartPoint = ep;
                                var ev = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                                ev.AddItem(newVdline);
                                var rl = routeInfo.LstTBBOXes.Where(x => x.routeLines.Where(l => l.Id == p.Id).Any()).Any();
                                if (rl)
                                {
                                    var r = routeInfo.LstTBBOXes.Where(x => x.routeLines.Where(l => l.Id == p.Id).Any()).FirstOrDefault();
                                    r.routeLines.Remove(p);
                                    r.routeLines.Add(newVdline);
                                    var selectedtb = routeInfo.LstTBBOXes.Where(x => x.selectedRoute.connectors.Where(l => l.line.Id == p.Id).Any()).FirstOrDefault().selectedRoute.connectors;
                                    var selectedLine = selectedtb.Where(x => x.line == p);
                                    if (selectedLine.Any())
                                    {
                                        selectedLine.FirstOrDefault().line = (newVdline);
                                    }
                                }
                                ev.RemoveItem(p);
                                LstTempvdline.Add(newVdline);
                            }
                            RefreshCADSpace();
                            break;
                        }
                    default:
                        break;
                }

            }
            switch (routeInfo.CURRENT_MODE)
            {
                case RouteInfo.eACTION_MODE.BOUNDARY:
                    {
                        if (obj is vdPolyline pl)
                            routeInfo.PreviousVdFigure = pl.Id;
                        break;
                    }
                case RouteInfo.eACTION_MODE.DESTINSTION:
                    {
                        if (obj is vdFigure vf && !(vf is vdText))
                        {
                            routeInfo.PreviousVdFigure = vf.Id;

                        }
                        break;
                    }
                case RouteInfo.eACTION_MODE.OBSTACLE:
                    {
                        if (obj is vdFigure vf && !(vf is vdText))
                        {
                            routeInfo.PreviousVdFigure = vf.Id;
                            if (vf is vdRect vr)
                                routeInfo.LstObstacles.Add(new Obstacle(vf));
                            if (vf is vdPolyline vpl)
                            {
                                if (vpl.VertexList.Count == 5)
                                    routeInfo.LstObstacles.Add(new Obstacle(vf));
                            }
                        }
                        break;
                    }
                case RouteInfo.eACTION_MODE.INSTRUMENT:
                    {
                        if (obj is vdInsert vdInsert)
                        {
                            IsInsAdd = true;
                            var res = routeInfo.GetCenterInstrument(vdInsert, routeInfo);
                            if (res != null)
                            {
                                LeaderEndPoint = res.Center;
                                LeaderInsert = null;
                                vdPolyline vp = new vdPolyline();
                                gPoints gPoints = new gPoints();
                                gPoints.Add(LeaderStartPoint);
                                gPoints.Add(LeaderEndPoint);
                                vp.VertexList.AddRange(gPoints);
                                gPoints trypoints = new gPoints();
                                var totalIntersection = vp.IntersectWith(res, VdConstInters.VdIntExtendArg, trypoints);
                                if (totalIntersection)
                                {
                                    if (trypoints.Count == 1)
                                    {
                                        LeaderEndPoint = trypoints[0];
                                        LeaderInsert = vdInsert;
                                        //  var pair = Tuple.Create(vdInsert, LeaderStartPoint) ; 
                                        // routeInfo.Instrument_Leader.Add(pair);
                                    }
                                    else
                                    {
                                        LeaderEndPoint = null;
                                    }
                                }
                            }
                            //ptk 4/28 
                            GetAllLayer();// routeInfo.LstLayer = GetAllLayer();
                            foreach (DataGridViewRow lb in instListbox.Rows)
                            {
                                lb.Cells["dgv_Layer"].Value = routeInfo.FinLayer((lb.Cells["dgv_Layer"].Value as Layer).name);
                            }
                            IsInsAdd = false;
                        }
                        if (obj is vdLeader vl)
                        {
                            LeaderLine = vl;
                        }
                        break;
                    }

            }

            //this.vdFC1.BaseControl.ActiveDocument.Update();
            this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
        }
        bool IsDeleted = false;
        List<vdPolyline> lstPolyIDTemp = new List<vdPolyline>();
        private vdFigure vdFigureTemp = null;
        private void BaseControl_AddItem(object obj, ref bool Cancel)
        {
            if (!IsManualDrawn)
                return;

            ////if (obj is vdPolyline vdPoly) // Desti/ Obs / Bound / Ins
            ////{
            ////    switch (routeInfo.CURRENT_MODE)

            ////    {
            ////        case RouteInfo.eACTION_MODE.BOUNDARY:
            ////            {
            ////                if (vdPoly.Layer.Name.ToLower().Contains("boundary"))
            ////                {
            ////                    bool IsLegal = true;
            ////                    routeInfo.ExecuteBoundaryDM(vdPoly, vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems(), ref IsLegal);
            ////                    IsDeleted = !IsLegal;
            ////                    Cancel = !IsLegal;
            ////                    if (Cancel)
            ////                    {
            ////                        System.Windows.Forms.MessageBox.Show("The created polyline is intersecting or inscribed with existing ones. Please draw a suitable boundary.");
            ////                        this.vdFC1.BaseControl.ActiveDocument.Update();
            ////                        this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
            ////                    }
            ////                }
            ////                break;
            ////            }
            ////        case RouteInfo.eACTION_MODE.INSTRUMENT:
            ////            break;
            ////        case RouteInfo.eACTION_MODE.OBSTACLE:
            ////            {
            ////                if (vdPoly.GetGripPoints().Count < 5) lstPolyIDTemp.Add(vdPoly);
            ////                if (vdPoly.GetGripPoints().Count == 5)
            ////                {
            ////                    foreach (var lst in lstPolyIDTemp)
            ////                        vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(lst);
            ////                    lstPolyIDTemp.Clear();
            ////                }

            ////                break;
            ////            }
            ////        case RouteInfo.eACTION_MODE.DESTINSTION:
            ////            {
            ////                bool IsLegal = true;
            ////                routeInfo.ExecuteDestinationDM(vdPoly, vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems(), ref IsLegal);
            ////                IsDeleted = !IsLegal; 
            ////                if (!IsLegal)
            ////                {
            ////                    System.Windows.Forms.MessageBox.Show("The created polyline is intersecting or inscribed with existing ones. Please draw a suitable boundary.");

            ////                    TBName = "";
            ////                    this.vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.RemoveItem(vdPoly);

            ////                    this.vdFC1.BaseControl.ActiveDocument.Update();
            ////                    this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
            ////                    return;
            ////                }

            ////                DestinationNamingForm Des = new DestinationNamingForm();
            ////                Des.Owner = this;

            ////                var res = Des.ShowDialog();
            ////                if (res == DialogResult.OK)
            ////                {

            ////                    int selectedCboDestTypeIndex = cboDestType.SelectedIndex;
            ////                    if (selectedCboDestTypeIndex == 0)
            ////                    {
            ////                        if (routeInfo.MainDestination != null)
            ////                        {
            ////                            MessageBox.Show("IO room should be only one."); 
            ////                            this.vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.RemoveItem(vdPoly);

            ////                            this.vdFC1.BaseControl.ActiveDocument.Update();
            ////                            this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
            ////                            return;
            ////                        }
            ////                        if (!string.IsNullOrEmpty(Des.TBName))
            ////                        {
            ////                            TBName = routeInfo.IOABBRE + Des.TBName;
            ////                            routeInfo.FindIORoom(vdPoly.Id).SetName(TBName, TBName_Id);
            ////                        }
            ////                        else
            ////                            return;

            ////                    }
            ////                    else if (selectedCboDestTypeIndex > 0)
            ////                    {
            ////                        TBName = routeInfo.TBABBRE + Des.TBName;
            ////                        vdFigureTemp = vdPoly;
            ////                        routeInfo.FindTBBox(vdPoly.Id).SetName(TBName, TBName_Id);
            ////                    }
            ////                    int r1 = 0;
            ////                    DestinationBind(ref r1);
            ////                }
            ////                ////else if (res == DialogResult.Cancel)
            ////                ////{
            ////                ////    Cancel = true;
            ////                ////}
            ////                else
            ////                {
            ////                    TBName = "";
            ////                    Cancel = true;
            ////                }

            ////                if (Cancel)
            ////                {
            ////                    this.vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.RemoveItem(vdPoly);
            ////                    this.vdFC1.BaseControl.Redraw();
            ////                    this.vdFC1.BaseControl.Update();
            ////                }
            ////                break;
            ////            }
            ////        case RouteInfo.eACTION_MODE.MAINROUTE:
            ////            {
            ////                routeInfo.FindIO_TBBox(tbTemp.guid).MainRouteCollection.Add(vdPoly);//routeInfo.FindIO_TBBox(tbTemp.guid).MainRoute = vdPoly;
            ////                foreach (var lst in lstPolyIDTemp)
            ////                {
            ////                    vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(lst);
            ////                    routeInfo.SyncronyzeRelatedUI(new List<int>() { lst.Id }, vdFC1);
            ////                }
            ////                lstPolyIDTemp.Clear();
            ////                lstPolyIDTemp.Add(vdPoly);
            ////                break;
            ////            }
            ////        default:
            ////            break;
            ////    }

            ////    this.vdFC1.BaseControl.ActiveDocument.Update();
            ////    this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
            ////}

        }
        private vdPolyline GetMainBoundary()
        {

            if (routeInfo.MainBoundary != null)
                return routeInfo.MainBoundary;
            double area = 0.0;
            vdPolyline vdPoly = new vdPolyline();

            foreach (vdFigure f in vdFC1.BaseControl.ActiveDocument.Model.Entities.GetNotDeletedItems())
            {
                if (f is vdPolyline ply && f.Layer.Name.ToLower().Contains("boundary"))
                {
                    vdPoly = ply;
                    return vdPoly;
                }
            }
            return null;
        }

        private void SetCustomLayer(string LayerName)
        {
            vdLayer layer = this.vdFC1.BaseControl.ActiveDocument.Layers.FindName(LayerName);
            try
            {
                vdFC1.BaseControl.ActiveDocument.SetActiveLayer(layer);
            }
            catch { }
        }
        //
        private List<Layer> GetAllLayer()
        {
            return routeInfo.GetAllLayer(vdFC1, routeInfo, instListbox, ObstListbox);
        }
        string TBName = "";
        bool IsManualDrawn = false;
        private void DrawDestination()
        {
            LstSelectedID = new List<int>();
            IsManualDrawn = true;
            LstSelectedID = new List<int>();
            SetCustomLayer(cboDest.Text.ToString());
            var lyr = vdFC1.BaseControl.ActiveDocument.ActiveLayer;
            gPoint userPoint;
            StatusCode ret = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            if (userPoint == null) return;
            TBBOXDestination des = new TBBOXDestination();
            des.centerPoint = userPoint;
            nearestpoint.GetNearestpoint(des, routeInfo, unitGrid);
            userPoint = des.gridPoint;
            gPoints gp = new gPoints();
            var sp = new gPoint(userPoint.x - Convert.ToDouble(BoxWidth / 2), userPoint.y + Convert.ToDouble(BoxHeight / 2));
            var ep = new gPoint(userPoint.x + Convert.ToDouble(BoxWidth / 2), userPoint.y - Convert.ToDouble(BoxHeight / 2));
            gp.Add(sp);
            gp.Add(ep);
            gp = gp.GetBox().GetPoints();
            gp.Add(gp.GetBox().GetPoints()[0]);
            if (vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gp) && !string.IsNullOrEmpty(TBName))
            {
                var midPt = gp.GetBox().MidPoint;
                if (TBName != "")
                {
                    TextInsert(midPt.x, midPt.y, TBName, out int id, false, lyr.PenColor);
                    if (routeInfo.FindTBBox(vdFigureTemp.Id) != null)
                    {
                        var tbbox = routeInfo.FindTBBox(vdFigureTemp.Id);
                        tbbox.SetName(TBName, id);
                        if (tbbox.IsIO == eDestinationType.MCC)
                        {
                            vdFigureTemp.PenColor = new vdColor(Color.FromName(MCCColor.ToLower()));//.FromSystemColor(Color.DarkBlue);
                        }
                        if (tbbox.IsIO == eDestinationType.TBBox)
                        {
                            vdFigureTemp.PenColor = new vdColor(Color.FromName(TBColor.ToLower()));//.FromSystemColor(Color.DarkBlue);
                        }
                        if (tbbox.IsIO == eDestinationType.IORoom)
                        {
                            vdFigureTemp.PenColor = new vdColor(Color.FromName(IOColor.ToLower()));//.FromSystemColor(Color.DarkBlue);
                        }
                        SendKeys.Send("{ESC}");
                        SendKeys.Send("{ESC}");
                        RefreshCADSpace();
                    }
                    else
                    {
                        TBName = "";
                        TBName_Id = 0;
                        return;
                    }
                    UpdateDestinationGrid();
                    TBName_Id = id;
                }
                else
                    TBName_Id = 0;

            }
            //}
            routeInfo.SyncronyzeRelatedUI(LstSelectedID, vdFC1);  // Prepare TBBox Info 
            IsManualDrawn = false;
        }
        private void DrawDestination_Manual()
        {
            LstSelectedID = new List<int>();
            IsManualDrawn = true;
            LstSelectedID = new List<int>();
            SetCustomLayer(cboDest.Text.ToString());
            var lyr = vdFC1.BaseControl.ActiveDocument.ActiveLayer;
            gPoint userPoint;

            vdFC1.BaseControl.ActiveDocument.Prompt("Select a Point");

            StatusCode ret =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt("Other corner:");

            object ret2 =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRect(userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);

            Vector v = ret2 as Vector;

            if (v != null)
            {
                double angle = v.x;
                double width = v.y;
                double height = v.z;

                gPoint userpoint2 = userPoint.Polar(0.0, width);

                userpoint2 = userpoint2.Polar(VectorDraw.Geometry.Globals.HALF_PI, height);
                gPoints gp = new gPoints();
                gp.Add(userPoint);
                gp.Add(userpoint2);
                gp = gp.GetBox().GetPoints();
                gp.Add(gp.GetBox().GetPoints()[0]);
                if (vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gp) && !string.IsNullOrEmpty(TBName))
                {
                    var midPt = gp.GetBox().MidPoint;
                    if (TBName != "")
                    {
                        TextInsert(midPt.x, midPt.y, TBName, out int id, false, lyr.PenColor);
                        if (routeInfo.FindTBBox(vdFigureTemp.Id) != null)
                        {
                            var tbbox = routeInfo.FindTBBox(vdFigureTemp.Id);
                            tbbox.SetName(TBName, id);
                            if (tbbox.IsIO == eDestinationType.MCC)
                            {
                                vdFigureTemp.PenColor = new vdColor(Color.DarkBlue);//.FromSystemColor(Color.DarkBlue);
                            }
                            if (tbbox.IsIO == eDestinationType.TBBox)
                            {
                                vdFigureTemp.PenColor = new vdColor(Color.Cyan);//.FromSystemColor(Color.DarkBlue);
                            }
                            if (tbbox.IsIO == eDestinationType.IORoom)
                            {
                                vdFigureTemp.PenColor = new vdColor(Color.Red);//.FromSystemColor(Color.DarkBlue);
                            }
                            SendKeys.Send("{ESC}");
                            SendKeys.Send("{ESC}");
                            RefreshCADSpace();
                        }
                        else
                        {
                            TBName = "";
                            TBName_Id = 0;
                            return;
                        }
                        UpdateDestinationGrid();
                        TBName_Id = id;
                    }
                    else
                        TBName_Id = 0;
                }
            }
            routeInfo.SyncronyzeRelatedUI(LstSelectedID, vdFC1);  // Prepare TBBox Info 
            IsManualDrawn = false;
        }
        private int TBName_Id = 0;
        System.Windows.Forms.Form di = null;
        private bool AnalyzeSetting()
        {
            try
            {
                if (routeInfo == null)
                    return false;
                makeGrid(out Tuple<int, int> tp1, false);
                if (string.IsNullOrEmpty(txtHTolerance.Text))
                {
                    txtHTolerance.Text = "1";
                }
                var lst = routeInfo.DetectInstrumentInsideRange(vdFC1, unit, tp1, unitGrid);

                if (lst.Count > 0)
                {
                    var msg = lst.Count.ToString() + "개의 계측기기가 분석 범위 안에 포함되지 못했습니다." + Environment.NewLine + "확인하시겠습니까?";
                    var res = MessageBox.Show(msg, "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.Yes)
                    {
                        DuplicateItem duplicateItem = new DuplicateItem(routeInfo, vdFramedControl, false, lst);
                        di = duplicateItem;
                        duplicateItem.ShowDialog();
                        duplicateItem.Owner = this;
                        // duplicateItem.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                return false;
            }
            return true;
        }

        public void makeGrid(out Tuple<int, int> tuple, bool IsAlternative = false)
        {
            tuple = null;
            try
            {

                if (offsetBoundary == null)
                {
                    MessageBox.Show("Cant Detect boundary or no exist that kind of boundary layer." + Environment.NewLine + "Please make sure boundary layer exist.");
                    return;
                }
                Box box = this.offsetBoundary.BoundingBox;

                sx = box.Left;
                sy = box.Bottom;
                //Max gap for Obstacle
                double LeastWBand = 0.0;
                double LeastHBand = 0.0;
                foreach (vdPolyline obstacle in this.obstacles)
                {
                    var leastwh = obstacle.BoundingBox.Width;
                    var leastht = obstacle.BoundingBox.Height;
                    if (leastht == leastwh)
                    {
                        if (LeastWBand == 0.0 && LeastHBand == 0.0)
                        {
                            LeastWBand = leastwh;
                            LeastHBand = leastht;
                        }
                        if (LeastWBand > leastwh) LeastWBand = leastwh;
                        if (LeastHBand > leastht) LeastHBand = leastht;
                    }
                }
                int wGap = int.Parse(routeInfo.xgap);
                int hGap = int.Parse(routeInfo.ygap);
                //int wGap = (200 * 2) * int.Parse(routeInfo.ygap); 
                //int hGap = (200 * 2) * int.Parse(routeInfo.ygap); 
                int wc = (int)box.Width / (wGap) + 1;
                int hc = (int)box.Height / (hGap) + 1;
                var tp = new Tuple<int, int>(wc, hc);
                tuple = tp;
                if (!IsAlternative) return;
                int pCount = wc * hc;



                double[,] adjMatrix = new double[pCount, pCount];
                for (int i = 0; i < pCount; i++)
                {
                    for (int j = 0; j < pCount; j++)
                    {
                        adjMatrix[i, j] = double.MaxValue;
                    }
                }


                for (int i = 0; i < pCount - 1; i++)
                {
                    if ((i / wc) == ((i + 1) / wc))
                    {
                        adjMatrix[i, i + 1] = wGap;
                        adjMatrix[i + 1, i] = wGap;
                    }
                    if (i + wc < pCount)
                    {
                        adjMatrix[i, i + wc] = hGap;
                        adjMatrix[i + wc, i] = hGap;
                    }
                }

                for (int i = 0; i < hc; i++)
                {
                    for (int j = 0; j < wc; j++)
                    {
                        int index = i * wc + j;
                        gPoint p = new gPoint(sx + j * wGap, sy + i * hGap);
                        gridPoints.Add(p);
                        bool isIn = contains(this.boundary.VertexList, p);
                        bool isIn2 = false;
                        foreach (vdPolyline obstacle in this.obstacles)
                        {
                            isIn2 = contains(obstacle.VertexList, p);
                            if (isIn2) break;
                        }

                        if (!isIn || isIn2)
                        {
                            if ((index - wc) > 0)
                            {
                                adjMatrix[index - wc, index] = double.MaxValue;
                                adjMatrix[index, index - wc] = double.MaxValue;
                            }
                            if ((index - 1) > 0 && ((index - 1) / wc == index / wc))
                            {
                                adjMatrix[index - 1, index] = double.MaxValue;
                                adjMatrix[index, index - 1] = double.MaxValue;
                            }
                            if ((index + wc) < pCount)
                            {
                                adjMatrix[index + wc, index] = double.MaxValue;
                                adjMatrix[index, index + wc] = double.MaxValue;
                            }
                            if ((index + 1) < pCount && ((index + 1) / wc == index / wc))
                            {
                                adjMatrix[index + 1, index] = double.MaxValue;
                                adjMatrix[index, index + 1] = double.MaxValue;
                            }
                        }


                    }
                }

                double minDistance = double.MaxValue;
                int count = 0;
                foreach (gPoint gridPoint in this.gridPoints)
                {
                    double dis = this.destination.centerPoint.Distance2D(gridPoint);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        //minDistance = dis;
                        destination.gridPoint = gridPoint;
                        destination.gridIndex = count;
                    }
                    count++;
                }

                foreach (Instrument ins in this.instruments)
                {
                    minDistance = double.MaxValue;
                    count = 0;
                    foreach (gPoint gridPoint in this.gridPoints)
                    {
                        double dis = ins.centerPoint.Distance2D(gridPoint);
                        if (minDistance > dis)
                        {
                            minDistance = dis;
                            ins.distance = dis;
                            ins.gridPoint = gridPoint;
                            ins.gridIndex = count;
                        }
                        count++;
                    }
                }


                foreach (Instrument ins in this.instruments)
                {
                    //   ins.distanceFromDestination = this.destination.center.Distance2D(ins.centerPoint);
                    //   Console.WriteLine("gridIndex   "+ins.gridIndex);
                    double[] result = Dijkstra.analysis(ins.gridIndex, destination.gridIndex, adjMatrix);
                    if (result != null) // it might seen an error  // ***PTK 
                        ins.distanceFromDestination = result[result.Length - 1];
                }

                this.instruments = this.instruments.OrderBy(x => x.distanceFromDestination).ToList();
                txtMaxLengthInstrument.Text = (Convert.ToInt32(this.instruments.Max(x => x.distanceFromDestination) / 200)).ToString();
                // this.instruments.Reverse();
                dtInstrument = new DataTable();
                dtInstrument.Columns.Add("Id");
                dtInstrument.Columns.Add("No");
                dtInstrument.Columns.Add("Instrument");
                dtInstrument.Columns.Add("Dimension");
                //dtInstrument.Columns.Add("SquareArea");//
                dtInstrument.Columns.Add("InstrumentType");//InstrumentType
                int counterText = 0;
                foreach (Instrument ins in this.instruments)
                {
                    counterText++;
                    double[] result = Dijkstra.analysis(ins.gridIndex, destination.gridIndex, adjMatrix);
                    if (result == null)
                        continue;

                    gPoints ps = new gPoints();
                    try
                    {
                        for (int i = 0; i < result.Length - 1; i++)
                        {
                            gPoint gp = gridPoints[(int)result[i]];
                            ps.Add(gp);
                        }
                    }
                    catch { }
                    try
                    {
                        for (int i = 0; i < result.Length - 2; i++)
                        {
                            adjMatrix[(int)result[i], (int)result[i + 1]] = adjMatrix[(int)result[i], (int)result[i + 1]] * 0.1;
                            adjMatrix[(int)result[i + 1], (int)result[i]] = adjMatrix[(int)result[i + 1], (int)result[i]] * 0.1;
                        }
                    }
                    catch { }

                    vdPolyline line = new vdPolyline(this.vdFC1.BaseControl.ActiveDocument, ps);
                    //line.PenColor.ByLayer = line.PenColor.ByBlock = false;
                    // line.PenColor.ByColorIndex = true;
                    //line.PenColor = new vdColor(Color.Yellow);
                    vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(line);
                    line.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
                    line.setDocumentDefaults();
                    //var ids = line.Id;

                    if (IsAlternative)
                    {
                        //Texting / Dimensioning
                        // var ctxt = counterText.ToString();
                        if (String.IsNullOrEmpty(ins.t1) && String.IsNullOrEmpty(ins.t2) && !ins.IsTBBox)
                            TextInsert(ins.centerPoint.x, ins.centerPoint.y, ("Instrument_" + counterText.ToString()), out int id);//analyzer
                                                                                                                                   //if (Convert.ToInt32(ins.distanceFromDestination) == Convert.ToInt32(this.instruments.Max(x => x.distanceFromDestination).ToString().Contains("E") ? "10000000000" : this.instruments.Max(x => x.distanceFromDestination).ToString()))
                                                                                                                                   // lblMaxLengthInstrument.Text = "Instrument_" + counterText.ToString();  
                        dtInstrument.Rows.Add(new object[] {   line.Id , counterText ,
                            ( (String.IsNullOrEmpty(ins.t1) && String.IsNullOrEmpty(ins.t2) ) ?   ("Instrument_" + counterText.ToString()):   (ins.t1 + "_" + ins.t2 ) )

                 ,  Convert.ToInt32(ins.distanceFromDestination/200).ToString(), "0_121"
                });
                    }
                }
                this.vdFC1.BaseControl.Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Error at Grid Algorithm. " + Environment.NewLine + "Close and Try with Relocate action.");
            }
        }

        private void OpenLayersDialog()
        {
            VectorDraw.Professional.Dialogs.LayersDialog.Show
           (vdFC1.BaseControl.ActiveDocument);

        }
        private void btnLayer_Click(object sender, EventArgs e)
        {
            OpenLayersDialog();
        }

        private void cboInst_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            IsCellEditing = true;

            var msg = MessageBox.Show("계측기기를 삭제하시겠습니까? 삭제하신 계측기기는 복구되지 않습니다.", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (msg != DialogResult.Yes) return;
            if (routeInfo == null || routeInfo.DGV_LstInstrument == null || routeInfo.DGV_LstInstrument.Count == 0) return;
            UpdateCurrentState();
            if (instDGV.SelectedCells.Count == 0)
                return;
            List<DataGridViewRow> dgr = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in instDGV.Rows)
            {
                if (dr.Cells["col_Check_Instrument"].EditedFormattedValue.ToString() == "True")
                {
                    dgr.Add(dr);
                    var index = dr.Index;
                    var guid = Convert.ToInt32(instDGV.Rows[index].Cells["GUID"].Value);
                    routeInfo.DGV_LstInstrument.Remove(routeInfo.FindDGV_Instrument(guid));
                }
            }
            foreach (var dr in dgr)
            {
                instDGV.Rows.Remove(dr);
            }
            instDGV.Update();
            BindGrid(true);
            //IsCellEditing = true;
            //if (routeInfo == null || routeInfo.DGV_LstInstrument == null || routeInfo.DGV_LstInstrument.Count == 0) return;
            //UpdateCurrentState();
            //if (instDGV.SelectedCells.Count == 0)
            //    return;
            //var index = instDGV.SelectedCells[0].RowIndex;
            //var guid = Convert.ToInt32(instDGV.Rows[index].Cells["GUID"].Value);
            //routeInfo.DGV_LstInstrument.Remove(routeInfo.FindDGV_Instrument(guid));
            //SetInsCable();
            //BindGrid(true);

        }
        private void UpdateCurrentState()
        {
            try
            {
                if (instDGV.SelectedCells.Count == 0)
                    return;
                var index = instDGV.SelectedCells[0].RowIndex;
                var guid = Convert.ToInt32(instDGV.Rows[index].Cells["GUID"].Value);

                routeInfo.SelectedGUID = guid;
                // instDGV_CellEndEdit(null,null);
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (routeInfo == null || routeInfo.DGV_LstInstrument == null || routeInfo.DGV_LstInstrument.Count == 0) return;
            UpdateCurrentState();
            //SetInsCable();
            InstSetForm instSetForm = new InstSetForm(routeInfo);
            instSetForm.Owner = this;
            instSetForm.ShowDialog();
            if (instSetForm.DialogResult == DialogResult.OK)
            {
                //IsInstrumentBox = true;
                routeInfo = instSetForm.RouteInfo;
                BindGrid(true);
            }
        }
        bool IsInstrumentBox = false;
        public void BindGrid(bool IsRemove = false)
        {

            if (routeInfo == null || routeInfo.DGV_LstInstrument == null) return;
            instDGV.DataSource = routeInfo.ListToDataTable(routeInfo.DGV_LstInstrument);
            if (!IsRemove)
                foreach (DataGridViewRow dr in instDGV.Rows)
                {
                    dr.Cells["col_Check_Instrument"].Value = true;
                }
            if (routeInfo.DGV_LstInstrument.Count > 0)
                txtDetectedCount.Text = routeInfo.DGV_LstInstrument.Count.ToString() + " instruments are detected.";
            else
                txtDetectedCount.Text = "No instrument is detected.";
            IsCellEditing = true;

            //dtMainTable = new DataGridView();
            //foreach(DataGridViewRow gr in instDGV.Rows)
            //{
            //    dtMainTable.Rows.Add(gr);
            //}
        }
        bool IsCellEditing = false;
        private void UpdateGridValueChange()
        {
            foreach (DataGridViewRow dr in instDGV.Rows)
            {
                try
                {

                    var GUIDGRID = Convert.ToInt32(dr.Cells["GUID"].Value);
                    var To = dr.Cells["colTo"].Value.ToString() == "" ? -1 : Convert.ToInt32(dr.Cells["colTo"].Value);
                    var type = Convert.ToString(dr.Cells["colType"].Value);
                    var system = Convert.ToString(dr.Cells["colSystem"].Value);
                    if (routeInfo.FindDGV_Instrument(GUIDGRID) == null) return;
                    routeInfo.FindDGV_Instrument(GUIDGRID).To = To;
                    routeInfo.FindDGV_Instrument(GUIDGRID).Type = type;
                    routeInfo.FindDGV_Instrument(GUIDGRID).System = system;// inst.Classification_3 = inst.Type;
                    routeInfo.FindDGV_Instrument(GUIDGRID).Classification_3 = type;// inst.Classification_3 = inst.Type;
                }
                catch { }
            }
        }
        private void instDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            VisibleAll();
            if (IsCellEndEditing)
            {
                IsCellEndEditing = false;
                return;
            }
            IsCellEditing = true;
            if (routeInfo == null) return;
            foreach (DataGridViewRow dr in instDGV.Rows)
            {
                try
                {

                    var GUIDGRID = Convert.ToInt32(dr.Cells["GUID"].Value);
                    var To = dr.Cells["colTo"].Value.ToString() == "" ? -1 : Convert.ToInt32(dr.Cells["colTo"].Value);
                    var type = Convert.ToString(dr.Cells["colType"].Value);
                    var system = Convert.ToString(dr.Cells["colSystem"].Value);
                    if (routeInfo.FindDGV_Instrument(GUIDGRID) == null)
                        return;
                    routeInfo.FindDGV_Instrument(GUIDGRID).To = To;
                    routeInfo.FindDGV_Instrument(GUIDGRID).Type = type;
                    routeInfo.FindDGV_Instrument(GUIDGRID).System = system;
                    routeInfo.FindDGV_Instrument(GUIDGRID).Classification_3 = type;
                }
                catch
                {
                }
            }
            if (sender == null || instDGV[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "colType" || instDGV[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "colTo")
            {
                SetInsCable(e);
            }
            UpdateGridValueChange();
            IsCellEditing = false;
        }

        private void SetInsCable(DataGridViewCellEventArgs e, bool IsMulti = false)
        {
            if (routeInfo == null || routeInfo.DGV_LstInstrument == null)
                return;
            RouteOptimizer.BL.SettingBL settingLBL = new BL.SettingBL();
            var lstInscable = settingLBL.GetInsCable();
            var lstSignal = settingLBL.GetSignalType();
            //  BL.SettingBL sbl = new BL.SettingBL();
            var LstInsType = settingLBL.GetInstrumetnType();

            foreach (var inst in routeInfo.DGV_LstInstrument)
            {
                // if (instDGV[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "colType")
                if (e != null)
                {
                    if (!(bool)(instDGV.CurrentRow.Cells["col_Check_Instrument"].FormattedValue))
                    {
                        if (inst.T1 + inst.T2 != instDGV.CurrentRow.Cells["colT1"].Value.ToString() + instDGV.CurrentRow.Cells["colT2"].Value.ToString())
                            continue;
                    }
                    if ((bool)(instDGV.CurrentRow.Cells["col_Check_Instrument"].FormattedValue))
                    {
                        if (inst.T1 + inst.T2 != instDGV.CurrentRow.Cells["colT1"].Value.ToString() + instDGV.CurrentRow.Cells["colT2"].Value.ToString())
                            continue;
                    }
                }
                List<InstCableEntity> newvalue = new List<InstCableEntity>();
                if (e == null || instDGV[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "colType" || inst.LstInstCableEntity == null || inst.LstInstCableEntity.Count == 0)
                {
                    //if (inst.LstInstCableEntity == null || inst.LstInstCableEntity.Count == 0)
                    //{
                    inst.LstInstCableEntity = newvalue;
                    inst.LstInstCableEntity = lstInscable.Where(a => a.InstType == inst.Type).ToList<InstCableEntity>();
                    //}
                }

                List<InstCableEntity> lstInsCa = new List<InstCableEntity>();
                int u = 0;
                foreach (var lsc in inst.LstInstCableEntity)
                {
                    u++;
                    InstCableEntity instCableEntity = new InstCableEntity();
                    instCableEntity.Abb = lsc.Abb;
                    instCableEntity.Cable = lsc.Cable;
                    instCableEntity.InstType = lsc.InstType;
                    instCableEntity.No = u.ToString(); // lsc.No;
                    instCableEntity.System = lsc.System;
                    instCableEntity.S_Check = lsc.S_Check;
                    instCableEntity.Type = lsc.Type;
                    instCableEntity.From = inst.T1 + "_" + inst.T2;
                    //if (instDGV[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "colTo")
                    instCableEntity.To = inst.To.ToString();
                    //instCableEntity.col_Catetegory = inst.ty
                    var sig = lstSignal.Where(x => x.Title == lsc.System).FirstOrDefault();
                    if ((sig != null))
                    {
                        instCableEntity.col_Signal = sig.AssignedCableDuct;
                    }
                    else
                    {
                        instCableEntity.col_Signal = "";
                    }
                    lstInsCa.Add(instCableEntity);
                }
                inst.LstInstCableEntity = lstInsCa;
                var getInstrumentType = LstInsType.Where(a => a.Classification_3 == inst.Type).FirstOrDefault<InstrumentListEntity>();

                if (getInstrumentType != null)
                    inst.Classification_2 = getInstrumentType.Classification_2;
                else
                {
                    inst.Classification_2 = "";
                }

                inst.Classification_3 = inst.Type;

            }
        }

        #region External References
        private void AddExternalReferences()
        {
            //We will add a vdblock object as an external reference to the blocks dialog.

            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "wmap.vdcl";
            VectorDraw.Professional.vdPrimaries.vdBlock xref = new VectorDraw.Professional.vdPrimaries.vdBlock();
            xref.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
            xref.setDocumentDefaults();
            xref.Name = "wmap";
            xref.ExternalReferencePath = path;
            //With update the file is opened and the document of the file is added to the External references of the file.
            xref.Update();
            vdFC1.BaseControl.ActiveDocument.Blocks.AddItem(xref);
        }
        private void AddReferencesInserts()
        {
            //Now we will add the vdinsert object that will show the external reference that we created.
            VectorDraw.Professional.vdFigures.vdInsert ins = new VectorDraw.Professional.vdFigures.vdInsert();
            ins.SetUnRegisterDocument(vdFC1.BaseControl.ActiveDocument);
            ins.setDocumentDefaults();
            //We check if the block exists and then give it to the insert.
            VectorDraw.Professional.vdPrimaries.vdBlock blk = vdFC1.BaseControl.ActiveDocument.Blocks.FindName("wmap");
            if (blk != null)
            {
                ins.Block = blk;

                vdFC1.BaseControl.ActiveDocument.Model.Entities.AddItem(ins);
            }

            //Note that the Same operation like above could have been done with the following function cmdXref.
            //string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "wmap.vdcl";
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdXref("A", path, new VectorDraw.Geometry.gPoint(), new double[] { 1.0, 1.0 }, 0.0, 0);


            //Zoom the model to show the entity.
            vdFC1.BaseControl.ActiveDocument.Model.ZoomExtents();
            vdFC1.BaseControl.ActiveDocument.Redraw(true);
        }
        private void OpenExternalReferencesDialog()
        {
            VectorDraw.Professional.Dialogs.frmXrefManager.Show(vdFC1.BaseControl.ActiveDocument);
        }
        #endregion


        private void btnXRef_Click(object sender, EventArgs e)
        {
            OpenExternalReferencesDialog();
            SendKeys.Send("{ESC}");
            //AddExternalReferences();
            //AddReferencesInserts();
            ChangeOrder();
        }
        private void ChangeOrder()
        {
            var lst = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.Last; //vdFC1.BaseControl.ActiveDocument.GetGripSelection();

            if (lst is vdInsert ist)
            {
                //if (ist.Block.Name == "wmap")
                //{
                vdFC1.BaseControl.ActiveDocument.CommandAction.CmdChangeOrder(lst, true);
                RefreshCADSpace();
                //}
            }
        }
        private void cbo_layer_Setting_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbo_layer_Setting.Text) || cbo_layer_Setting.SelectedItem == null) return;

                var lyr = routeInfo.FinLayer(cbo_layer_Setting.SelectedItem.ToString());
                //var lst = routeInfo.LstTBBOXes.Where(v => v.polyline.Layer.Name == lyr.name);
                dgv_destination.Rows.Clear();
                foreach (var des in routeInfo.LstTBBOXes)
                {
                    if (des.polyline.Layer.Name == lyr.name)
                    {
                        //var type = !des.IsIO ? eDestinationType.TBBox.ToString() : eDestinationType.IORoom.ToString();  // Assume TB are Tb and others as Io 
                        var type = "";// !des.IsIO ? eDestinationType.TBBox.ToString() : eDestinationType.IORoom.ToString();
                        if (des.IsIO == eDestinationType.TBBox)
                        {
                            type = eDestinationType.TBBox.ToString();
                        }
                        else if (des.IsIO == eDestinationType.IORoom)
                        {
                            type = eDestinationType.IORoom.ToString();
                        }
                        else if (des.IsIO == eDestinationType.MCC)
                        {
                            type = eDestinationType.MCC.ToString();
                        }
                        dgv_destination.Rows.Add(des.guid, des.NameId, lyr.name, type, des.Name.Replace("TB-", "").Replace("IO-", ""), des.CableType, des.OwnDestination == null ? "" : des.OwnDestination.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                var msg = ex.Message;
            }
        }


        private void btnDestDetails_Click(object sender, EventArgs e)
        {
            if (routeInfo == null || dgv_destination.Rows.Count == 0) return;
            if (dgv_destination.Rows.Count > 0)
            {
                if (dgv_destination.SelectedCells.Count > 0)
                {
                    DestinationSettingEntity dse = new DestinationSettingEntity();
                    //dse.Title = dgv_destination.CurrentRow.Cells["Title"].Value.ToString();
                    //dse.lstTBBox = routeInfo.LstTBBOXes;
                    var type = dgv_destination.CurrentRow.Cells["To"].Value.ToString();
                    var guid = Convert.ToInt32(dgv_destination.CurrentRow.Cells["polyGUID"].Value.ToString());
                    dse.TBBOXDestination = routeInfo.FindTBBox(guid);
                    dse.RouteInfo = routeInfo;
                    if (type == eDestinationType.TBBox.ToString())
                    {
                        DestSetForm destinationNamingForm = new DestSetForm(dse);
                        var res = destinationNamingForm.ShowDialog();
                        if (res == DialogResult.OK)
                        {
                            if (!string.IsNullOrEmpty(cbo_layer_Setting.Text))
                            {
                                var index = cbo_layer_Setting.SelectedIndex;
                                cbo_layer_Setting.Items.Clear();
                                foreach (var lyr in routeInfo.LstLayer)
                                {
                                    cbo_layer_Setting.Items.Add(lyr.name);
                                }
                                cbo_layer_Setting.SelectedIndex = index;
                            }
                            else
                            {
                                btnAllDest.PerformClick();
                            }
                        }
                    }
                    if (type == eDestinationType.MCC.ToString())
                    {
                        MCCSetForm destinationNamingForm = new MCCSetForm(dse);
                        var res = destinationNamingForm.ShowDialog();
                        if (res == DialogResult.OK)
                        {
                            if (!string.IsNullOrEmpty(cbo_layer_Setting.Text))
                            {
                                var index = cbo_layer_Setting.SelectedIndex;
                                cbo_layer_Setting.Items.Clear();
                                foreach (var lyr in routeInfo.LstLayer)
                                {
                                    cbo_layer_Setting.Items.Add(lyr.name);
                                }
                                cbo_layer_Setting.SelectedIndex = index;
                            }
                            else
                            {
                                btnAllDest.PerformClick();
                            }
                            //routeInfo.LstTBBOXes.Where(x=>x.guid == guid).FirstOrDefault() =   destinationNamingForm.des.TBBOXDestination;
                        }
                    }
                    if (type == eDestinationType.IORoom.ToString())
                    {
                        Ior ior = new Ior(dse);// IoDetails ior = new IoDetails(dse);  // 
                        ior.ShowDialog();
                    }
                }
            }
        }
        private void btnDestSave_Click(object sender, EventArgs e)
        {
            if (routeInfo == null || dgv_destination.Rows.Count == 0) return;
            try
            {
                foreach (DataGridViewRow dr in dgv_destination.Rows)
                {
                    if ((dr.Cells["Title"].Value == null || string.IsNullOrEmpty(dr.Cells["Title"].Value.ToString())))
                    {
                        MessageBox.Show("모든 필드가 채워졌는지 확인해주세요.");
                        return;
                    }
                }

                foreach (DataGridViewRow dr in dgv_destination.Rows)
                {
                    var t = dr.Cells["Title"].Value.ToString();
                    var type = dr.Cells["To"].Value.ToString();
                    var tbid = Convert.ToInt32(dr.Cells["destination_GUID"].Value);
                    var polyid = Convert.ToInt32(dr.Cells["polyGUID"].Value);

                    var tb = routeInfo.LstTBBOXes.Where(x => x.Name == (routeInfo.TBABBRE + t));
                    if (tb.Any() && tb.FirstOrDefault().guid != polyid)
                    {
                        MessageBox.Show("이미 입력된 이름이 있습니다. 다른 이름을 입력해주세요.");
                        BindDestinationSettningForm();
                        return;
                    }

                    var fig = vdFC1.BaseControl.ActiveDocument.Model.Entities.FindFromId(tbid);
                    t = (type.Contains(eDestinationType.TBBox.ToString()) ? routeInfo.TBABBRE : routeInfo.IOABBRE) + t;
                    routeInfo.FindTBBox(polyid).SetName(t, 0);
                    if (fig is vdText txt) txt.TextString = t;
                }

                this.vdFC1.BaseControl.ActiveDocument.Update();
                this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
                BindDestinationSettningForm();
                MessageBox.Show("성공적으로 업데이트되었습니다!");
            }
            catch
            {
                MessageBox.Show("업데이트에 실패했습니다.");
            }
        }
        private void BindDestinationSettningForm()
        {
            if (!string.IsNullOrEmpty(cbo_layer_Setting.Text))
            {
                var index = cbo_layer_Setting.SelectedIndex;
                cbo_layer_Setting.Items.Clear();
                foreach (var lyr in routeInfo.LstLayer)
                {
                    cbo_layer_Setting.Items.Add(lyr.name);
                }
                cbo_layer_Setting.SelectedIndex = index;
            }
            else
            {
                btnAllDest.PerformClick();
            }
        }
        private void dgv_destination_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_destination.CurrentCell is DataGridViewTextBoxCell) dgv_destination.BeginEdit(false);
        }
        private void UpdateDestinationGrid()
        {
            //  routeInfo.LstTBBOXes = tb
            cbo_layer_Setting_SelectedIndexChanged(null, null);
            int i = 0;
            DestinationBind(ref i);
        }
        private List<vdLine> LstTempvdline = new List<vdLine>();
        private void ConvertStraightLinePoly(int id)
        {

            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            var p = new vdPolyline();
            var vdl = new vdLine();
            var fig = e.FindFromId(id);
            //if (fig == null) 
            //    fig = routeInfo.LstTBBOXes.Where(x => x.selectedRoute.connectors.Where(l => l.line.Id == id).Any()).FirstOrDefault().selectedRoute.connectors.Where(x => x.line.Id == id).FirstOrDefault();
            var vdEntities = new vdEntities();
            if (fig is vdPolyline gp)
            {
                vdEntities = gp.Explode();
            }
            else if (fig is vdLine gl)
            {
                vdl = gl;
                vdEntities.AddItem(vdl);
            }
            ChangeSegmentsIntoLine(vdEntities, id);

        }
        private void ChangeSegmentsIntoLine(vdEntities ets, int id)
        {
            LstTempvdline = new List<vdLine>();
            routeInfo.SyncronyzeRelatedUI(new List<int>() { id }, vdFC1);

            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            foreach (vdLine l in ets)
            {
                Dictionary<vdLine, gPoint> dicIntersec = new Dictionary<vdLine, gPoint>();
                routeInfo.CURRENT_MODE = eACTION_MODE.LINE;
                foreach (var r in routeInfo.LstTBBOXes)
                {
                    foreach (var ml in r.MainRouteCollection)
                    {
                        var gp1 = new gPoints();
                        var res1 = ml.IntersectWith(l, VdConstInters.VdIntOnBothOperands, gp1);
                        if (res1)
                        {
                            if (ml.getStartPoint() != gp1[0] && ml.getEndPoint() != gp1[0])
                                dicIntersec.Add(ml, gp1[0]);
                        }
                    }
                }
                foreach (var ml in routeInfo.LstExternalRoute)
                {
                    var gp1 = new gPoints();
                    var res1 = ml.IntersectWith(l, VdConstInters.VdIntOnBothOperands, gp1);
                    if (res1)
                    {
                        try
                        {
                            if (ml.getStartPoint() != gp1[0] && ml.getEndPoint() != gp1[0])
                                dicIntersec.Add(ml, gp1[0]);
                        }
                        catch { }
                    }
                }
                var newdic = (dicIntersec.OrderBy(x => x.Value.Distance2D(l.StartPoint)));


                if (newdic.Count() > 0)
                {
                    gPoint sp = null;
                    foreach (var din in newdic)
                    {
                        try
                        {
                            var line = e.FindFromId(din.Key.Id) as vdLine;
                            LstRemovedLine.Add(new vdLine(line.StartPoint, line.EndPoint));
                            routeInfo.SyncronyzeRelatedUI(new List<int>() { din.Key.Id }, vdFC1);
                            var newline1 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                            newline1.EndPoint = din.Key.EndPoint;
                            newline1.StartPoint = din.Value;
                            e.AddItem(newline1);

                            var newline2 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                            newline2.EndPoint = din.Value;
                            newline2.StartPoint = din.Key.StartPoint;
                            e.AddItem(newline2);
                            if (sp == null) sp = l.StartPoint;

                            var newline3 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                            newline3.EndPoint = din.Value;
                            newline3.StartPoint = sp;
                            e.AddItem(newline3);
                            sp = newline3.EndPoint;

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    var newline4 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                    newline4.EndPoint = l.EndPoint;
                    newline4.StartPoint = sp;
                    e.AddItem(newline4);
                }
                else
                    e.Add(l);
            }
            RefreshCADSpace();
        }
        List<vdLine> LstRemovedLine = new List<vdLine>();
        private void btnMainRoute_Click(object sender, EventArgs e)
        {
            try
            {
                if (routeInfo == null) return;
                if (!rdo_UserDefine.Checked)
                {
                    MessageBox.Show("The system will operate with User-Defined option.");
                    rdo_UserDefine.Checked = true;
                    // return;
                }
                vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.MAINROUTE;
                DrawMainRoute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private void cmd_point(string prompt, out gPoint userPoint)
        {
            StatusCode ret =
            vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out userPoint);
            vdFC1.BaseControl.ActiveDocument.Prompt(null);
            vdFC1.BaseControl.ActiveDocument.Prompt(prompt);
        }
        TBBOXDestination tbTemp;
        private void DrawRouteConverter()
        {
            if (lstPolyIDTemp.Count > 0)
            {
                lstPolyIDTemp.Clear();
                vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                ConvertStraightLinePoly(routeInfo.PreviousVdFigure);
                ////ConvertStraightLinePoly(routeInfo.PreviousVdFigure);
            }

        }
        private void DrawMainRoute()
        {
            lstPolyIDTemp.Clear();
            IsManualDrawn = true;
            gPoint userPoint = null;
            gPoints gP = new gPoints();
            vdFC1.BaseControl.ActiveDocument.Prompt("Select a point inside a destination");

            cmd_point("Other point: ", out userPoint);
            if (userPoint == null)
                return;
            bool IsInside = false;
            routeInfo.xgap = txtHTolerance.Text;
            routeInfo.ygap = txtHTolerance.Text;
            tbTemp = null;

            vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
            foreach (var des in routeInfo.LstTBBOXes)
            {
                if (des.polyline.BoundingBox.PointInBox(userPoint))
                {
                    nearestpoint.GetNearestpoint(des, routeInfo, unitGrid);
                    userPoint = des.gridPoint; //des.centerPoint;
                    IsInside = true;
                    tbTemp = des;

                    break;
                }
            }
            var PrepareDesEndPoint = new TBBOXDestination(null, userPoint);

            if (!IsInside)
            {
                nearestpoint.GetNearestpoint(PrepareDesEndPoint, routeInfo, unitGrid);
                userPoint = PrepareDesEndPoint.gridPoint;
                //   MessageBox.Show("Please pick a point inside TB Box desitnation or make a touch point with existing drawn route.");
                // return;
            }
            gP.Add(userPoint);
            for (int i = 0; i < routeInfo.MaxPoly; i++)
            {
                vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                var userPoint2 = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRefPointLine(userPoint) as gPoint;
                if (userPoint2 == null)
                {
                    var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                    var IstooShort = IsTooShortinParent(gP, userPoint2);
                    if (IstooShort)
                    {
                        foreach (var f in lstPolyIDTemp)
                        {
                            e.RemoveItem(f);
                            var pid = routeInfo.FindTBBox(tbTemp.guid).MainRouteCollection.Where(x => x.Id == f.Id);
                            if (pid.Any())
                            {
                                routeInfo.FindTBBox(tbTemp.guid).MainRouteCollection.Remove(pid.FirstOrDefault());
                            }
                        }
                    }
                    DrawRouteConverter();
                    //lstPolyIDTemp.Clear();
                    //vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                    //ConvertStraightLinePoly(routeInfo.PreviousVdFigure);
                    // ConvertStraightLine(routeInfo.PreviousVdFigure);
                    break;
                }
                vdFC1.BaseControl.ActiveDocument.Prompt(null);
                vdFC1.BaseControl.ActiveDocument.Prompt("Other point: ");
                PrepareDesEndPoint = new TBBOXDestination(null, userPoint2);
                nearestpoint.GetNearestpoint(PrepareDesEndPoint, routeInfo, unitGrid);
                userPoint2 = PrepareDesEndPoint.gridPoint;

                var p1 = (new vdPolyline());
                p1.VertexList.AddRange(gP);

                var p2 = (new vdPolyline());
                p2.VertexList.AddRange(new gPoints() { gP.Last(), userPoint2 });

                var gs = new gPoints();
                var res = p1.IntersectWith(p2, VdConstInters.VdIntOnBothOperands, gs);
                if (res)
                {
                    if (gs.Count > 1)
                    {
                        //if (lstPolyIDTemp.Count > 0 )
                        //{
                        DrawRouteConverter();
                        ////lstPolyIDTemp.Clear();
                        ////vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                        ////ConvertStraightLinePoly(routeInfo.PreviousVdFigure);
                        //}
                        break;
                    }
                }

                if (tbTemp != null)
                {
                    var IsIntParent = IsParentRouteIntersect(gP, userPoint2); // check parent 
                    var IsInsideTBBoxes = IsLINKInsideTBBoxes(gP, userPoint2); // check in Parent TB or already existed TB
                    if (IsIntParent || IsInsideTBBoxes)
                    {
                        break;
                    }

                    var IsIntOther = IsOthersRouteIntersect(gP, userPoint2, out gPoint targetPoint, out bool IsAlreadyJoined); // check other Parents
                    if (IsIntOther)
                    {
                        if (IsAlreadyJoined)
                        {
                            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
                            userPoint = null;
                            DrawRouteConverter(); //ptk added forremaining polylines

                            break;
                        }
                        gP.Add(targetPoint);
                        vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
                        userPoint = null;
                        DrawRouteConverter(); //ptk added forremaining polylines

                        break;

                    }
                    else
                    {
                        vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                        gP.Add(userPoint2);
                        vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
                        userPoint = userPoint2;
                    }
                }
                else
                {
                    #region BOX
                    bool IsEPInTBBOx = false;
                    foreach (var tbx in routeInfo.LstTBBOXes)
                    {
                        if (tbx.polyline.BoundingBox.PointInBox(userPoint2))
                        {
                            IsEPInTBBOx = true;
                            break;
                        }
                    }
                    if (IsEPInTBBOx) break;

                    foreach (var r in routeInfo.LstAllRoute()) // Check Overlapped Logic
                    {
                        if (IsEPInTBBOx) break;
                        //foreach (var r in tbx.MainRouteCollection)
                        //{
                        if (r.PointOnCurve(gP[gP.Count - 1], true))
                        {
                            //if (r.PointOnCurve(userPoint2,true))
                            //{
                            var pa = gP[gP.Count - 1];
                            if (pa.y == userPoint2.y)
                            {
                                if (userPoint2.x >= pa.x)
                                {
                                    var pi1 = new gPoint(pa.x - 1, pa.y);
                                    // var pi2 = new gPoint(pa.x + 1, pa.y);
                                    var r1 = r.PointOnCurve(new vdLine(gP[gP.Count - 1], userPoint2).BoundingBox.MidPoint, true);
                                    if (r.PointOnCurve(pi1, true) && r1)
                                    {
                                        IsEPInTBBOx = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    // var pi1 = new gPoint(pa.x - 1, pa.y);
                                    var pi2 = new gPoint(pa.x + 1, pa.y);
                                    var r1 = r.PointOnCurve(new vdLine(gP[gP.Count - 1], userPoint2).BoundingBox.MidPoint, true);
                                    if (r.PointOnCurve(pi2, true) && r1)
                                    {
                                        IsEPInTBBOx = true;
                                        break;
                                    }
                                }
                            }
                            if (pa.x == userPoint2.x)
                            {
                                if (userPoint2.y >= pa.y)
                                {
                                    var r1 = r.PointOnCurve(new vdLine(gP[gP.Count - 1], userPoint2).BoundingBox.MidPoint, true);
                                    var pi1 = new gPoint(pa.x, pa.y - 1);
                                    // var pi2 = new gPoint(pa.x + 1, pa.y);
                                    if (r.PointOnCurve(pi1, true) && r1)
                                    {
                                        IsEPInTBBOx = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    //var pi1 = new gPoint(pa.x - 1, pa.y);
                                    var pi2 = new gPoint(pa.x, pa.y + 1);
                                    var r1 = r.PointOnCurve(new vdLine(gP[gP.Count - 1], userPoint2).BoundingBox.MidPoint, true);
                                    if (r.PointOnCurve(pi2, true) && r1)
                                    {

                                        IsEPInTBBOx = true;
                                        break;
                                    }
                                }
                            }
                            //var midPointhorizontal = new gPoint((gP[gP.Count - 1].x + userPoint2.x) / 2, userPoint2.y);
                            //var midPointvertical = new gPoint(userPoint2.x, (gP[gP.Count - 1].y + userPoint2.y) / 2);
                            //if (r.PointOnCurve(midPointhorizontal, true) || r.PointOnCurve(midPointvertical, true))
                            //{
                            //    IsEPInTBBOx = true;
                            //     break;
                            //}
                            //}
                            //}
                        }

                    }
                    if (IsEPInTBBOx)
                    {
                        DrawRouteConverter();// ptk added for remaining polylines 
                        break;
                    }
                    #endregion
                    //foreach (var x in routeInfo.LstExternalRoute)
                    //{
                    //    if (r.PointOnCurve(gP[gP.Count - 1], true))
                    //    {
                    //        //if (r.PointOnCurve(userPoint2,true))
                    //        //{
                    //        var midPointhorizontal = new gPoint((gP[gP.Count - 1].x - userPoint2.x) / 2, userPoint2.y);
                    //        var midPointvertical = new gPoint(userPoint2.x, (gP[gP.Count - 1].y - userPoint2.y) / 2);
                    //        if (r.PointOnCurve(midPointhorizontal, true) || r.PointOnCurve(midPointvertical, true))
                    //        {
                    //            IsEPInTBBOx = true;
                    //            break;
                    //        }
                    //        //}
                    //    }
                    //}
                    vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
                    gP.Add(userPoint2);
                    gP.RemoveEqualPoints();
                    gP.RemoveInLinePoints();
                    vdFC1.BaseControl.ActiveDocument.CommandAction.CmdPolyLine(gP);
                    userPoint = userPoint2;
                }
            }


            IsManualDrawn = false;
        }
        private bool IsTooShortinParent(gPoints gPoints, gPoint gp)
        {
            if (tbTemp == null)
                return false;
            var targetRoute = new vdPolyline();
            var p = (new vdPolyline());
            p.VertexList.AddRange(gPoints);
            if (gp != null)
                p.VertexList.Add(gp);
            gPoints gPoints1 = new gPoints();
            var res = p.IntersectWith(tbTemp.polyline, VdConstInters.VdIntOnBothOperands, gPoints1);
            if (!res)
            {
                return true;
            }
            return false;
        }
        private bool IsLINKInsideTBBoxes(gPoints gPoints, gPoint gp)
        {
            var targetRoute = new vdPolyline();
            var p = (new vdPolyline());
            p.VertexList.AddRange(gPoints);
            p.VertexList.Add(gp);
            bool isOnePointOutsite = false;
            foreach (gPoint pt in p.GetGripPoints())
            {
                if (!tbTemp.polyline.BoundingBox.PointInBox(pt))
                {
                    isOnePointOutsite = true;
                    break;
                }
            }
            if (isOnePointOutsite) // check inside which was back from outisde
            {
                if (tbTemp.polyline.BoundingBox.PointInBox(gp))
                {
                    return true;
                }
            }


            // For Other Side Repeat
            TBBOXDestination tbCurrent = null;// new TBBOXDestination();
            foreach (var tb in routeInfo.LstTBBOXes)
            {
                if (tb.Name == tbTemp.Name)
                    continue;
                if (tb.polyline.BoundingBox.PointInBox(gp))
                {
                    tbCurrent = tb;
                    break;
                }
            }

            if (tbCurrent == null)
                return false;
            foreach (var ml in tbCurrent.MainRouteCollection)  // Reversed Line ( if and Only if)
            {
                if (tbTemp.polyline.BoundingBox.PointInBox(ml.getEndPoint()))
                {
                    return true;
                }
            }


            foreach (var ml in tbTemp.MainRouteCollection)  // 2 point in Alien TB
            {
                if (tbCurrent.polyline.BoundingBox.PointInBox(ml.getEndPoint()))
                {
                    return true;
                }
            }
            //For reversed Side repeat





            return false;
        }
        private bool IsOthersRouteIntersect(gPoints gPoints, gPoint gp, out gPoint ispoint, out bool IsAlreadyJoined)
        {
            IsAlreadyJoined = false;
            ispoint = null;
            var targetRoute = new vdLine();
            var p = (new vdPolyline());
            p.VertexList.AddRange(gPoints);
            p.VertexList.Add(gp);
            foreach (var tt in routeInfo.LstTBBOXes)
            {
                if (tt.Name == tbTemp.Name)
                    continue;
                foreach (var l in tt.MainRouteCollection)
                {
                    if (l.Explode() == null || l.Explode().Count == 0)
                        continue;
                    var gs = new gPoints();
                    var res = l.IntersectWith(p, VdConstInters.VdIntOnBothOperands, gs);
                    if (res)
                    {
                        ispoint = gs[0];
                        targetRoute = l;
                        // return true;
                    }
                }
            }

            if (ispoint != null)
            {
                foreach (var ep in tbTemp.MainRouteCollection)
                {
                    if (ep.Explode() == null || ep.Explode().Count == 0)
                        continue;
                    if (targetRoute.PointOnCurve(ep.getEndPoint(), true))
                    {
                        IsAlreadyJoined = true;
                        break;
                    }
                }
                return true;
            }
            return false;
        }
        private bool IsParentRouteIntersect(gPoints gPoints, gPoint gp)
        {
            var p = (new vdPolyline());
            p.VertexList.AddRange(gPoints);
            p.VertexList.Add(gp);
            foreach (var l in tbTemp.MainRouteCollection)
            {
                if (l.Explode() == null || l.Explode().Count == 0)
                    continue;
                //if (l.Explode()[0] is vdPolyline vp)
                //{

                //}
                if (l.Explode()[0] is vdLine vl)
                {
                    var p1 = (vl).getStartPoint();
                    var p2 = (vl).getEndPoint();
                    if (gPoints.Count >= 2)
                    {
                        if (p1 == gPoints[0] && p2 == gPoints[1])
                        {
                            continue;
                        }
                    }
                }
                var gs = new gPoints();
                var res = l.IntersectWith(p, VdConstInters.VdIntOnBothOperands, gs);


                if (res)
                {
                    if (gs.Count == 1)
                    {
                        if (gs[0] != tbTemp.gridPoint)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;

                    }

                }
            }

            return false;
        }
        private void instDGV_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            instDGV.CommitEdit(DataGridViewDataErrorContexts.Commit);

        }
        string DestinationID = "";
        private void instDGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                //ptk mented 18 august
                //if (datagridview.Columns[e.ColumnIndex].Name == "colTo")
                //{
                //    DestinationID = datagridview.CurrentCell.Value.ToString();
                //    IsCellEditing = false;
                //}
                //else
                //{
                //    IsCellEditing = true;
                //}
                //datagridview.BeginEdit(true);
                //((ComboBox)datagridview.EditingControl).DroppedDown = true;

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int vp = 0;
            int vl = 0;
            int va = 0;
            int vi = 0;
            int vc = 0;
            int vo = 0;
            int vld = 0;
            foreach (vdFigure f in vdFC1.BaseControl.ActiveDocument.Model.Entities.GetNotDeletedItems())
            {
                foreach (var l in ObstListbox.Items)
                {
                    if (f.Layer.Name == (l as Layer).name.ToString())
                    {
                        if (f is vdPolyline)
                        {
                            vp++;
                        }
                        else if (f is vdLine)
                        {
                            vl++;
                        }
                        else if (f is vdArc)
                        {
                            va++;
                        }
                        else if (f is vdInsert)
                        {
                            vi++;
                        }
                        else if (f is vdCircle)
                        {
                            vc++;
                        }
                        else if (f is vdLeader)
                        {
                            vld++;
                        }
                        else
                        {
                            vo++;
                        }
                    }
                }
            }
            var lbl = "";
            lbl += vp == 0 ? "" : vp.ToString() + " polylines" + Environment.NewLine;
            lbl += vl == 0 ? "" : vl.ToString() + " lines" + Environment.NewLine;
            lbl += va == 0 ? "" : va.ToString() + " arcs" + Environment.NewLine;
            lbl += vi == 0 ? "" : vi.ToString() + " blocks" + Environment.NewLine;
            lbl += vc == 0 ? "" : vc.ToString() + " circles" + Environment.NewLine;
            lbl += vld == 0 ? "" : vld.ToString() + " leaders" + Environment.NewLine;
            lbl += vo == 0 ? "" : vo.ToString() + " other items" + Environment.NewLine;
            lbl = lbl == "" ? "No Obstacles were detected." : lbl;
            richTextBox1.Text = lbl;
        }

        private void vdFC1_Click(object sender, EventArgs e)
        {

        }

        private void vdFC1_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private bool WriteXML(bool IsSaveas = true)
        {
            try
            {
                var fname = "";
                RemoveHightlightedLines();
                List<vdLayer> lyr = new List<vdLayer>();
                foreach (vdLayer l in vdFC1.BaseControl.ActiveDocument.Layers.GetNotDeletedItems())
                {
                    foreach (DataGridViewRow r in instListbox.Rows)
                    {
                        Layer c = r.Cells["dgv_Layer"].Value as Layer;
                        if (c.name == l.Name)
                            routeInfo.LstLayer.Where(v => v.name == l.Name).FirstOrDefault().SelectedInInstrument = true;
                    }
                    foreach (Layer c in ObstListbox.Items)
                    {
                        if (c.name == l.Name)
                            routeInfo.LstLayer.Where(v => v.name == l.Name).FirstOrDefault().SelectedInObstacle = true;
                    }
                    if (l.Name.Contains(routeInfo.SYSTEMABBRE))
                    {
                        lyr.Add(l);
                    }
                }

                foreach (var l in lyr) vdFC1.BaseControl.ActiveDocument.Layers.RemoveItem(l);
                routeInfo.DetectedObstacles = richTextBox1.Text;
                routeInfo.SelectedDestination = cbo_layer_Setting.SelectedIndex;
                routeInfo.DEFAULT_OPTION = rdo_UserDefine.Checked ? (eSCOPE_MODE)0 : rdo_WithoutRoute.Checked ? (eSCOPE_MODE)1 : rdo_withroute.Checked ? (eSCOPE_MODE)2 : (eSCOPE_MODE)0;
                if (!rdo_WithoutRoute.Checked)
                {
                    routeInfo.DEFAULT_OPTION = eSCOPE_MODE.USERDEFINED_ROUTE;
                }
                if (routeInfo.DEFAULT_OPTION == eSCOPE_MODE.USERDEFINED_ROUTE) // PTK added 2023-09-22  Cox No additionally route added by user cant be shown in CAD when reimporting
                {
                    routeInfo.LstGuidedRoute = routeInfo.LstAllRoute();
                }
                routeInfo.BoxWidth = BoxWidth;
                routeInfo.BoxHeight = BoxHeight;

                routeInfo.IOcolor = IOColor;
                routeInfo.MCCcolor = MCCColor;
                routeInfo.TBColor = TBColor;
                routeInfo.DefaultDuctCheck = rdo_defaultDuct.Checked ? 1 : 0;
                routeInfo.DefaultAnalysisCheck = 0; //chk_SignalSegment.Checked ? 1 : 0;
                routeInfo.BasicInfo = new BasicSettingEntity
                {
                    MaxLengthInstrument = txtMaxLengthInstrument.Text,
                    DuctOptimizedRatio = txtDuctOptimize.Text,
                    HTolerence = txtHTolerance.Text,
                    YTolerence = txtYTolerence.Text,
                    Connectable = string.IsNullOrEmpty(txtConnectable.Text) ? "3" : (Convert.ToInt32(txtConnectable.Text) > 100) ? "3" : txtConnectable.Text.Trim(),// txtConnectable.Text,

                    PanelBay = txtPanelbay.Text,
                    FrontPart = txtFrontpart.Text,
                    RearClearence = txtRearClearence.Text,
                    WallFront = txtWallFrontSeparate.Text,
                    SideFront = txtWallSideDistance.Text
                };

                if (!Directory.Exists(DataSourceXmlBuffer))
                {
                    MessageBox.Show("Datasource가 존재하지 않습니다." + Environment.NewLine + "관리자에게 문의해주세요.");
                    return false;
                }

                //PTK mented on 8/17
                //if ((int)routeInfo.DEFAULT_OPTION == 0)
                //routeInfo.LstGuidedRoute = routeInfo.LstAllRoute();



                var FileName = routeInfo.SystemFile;
                if (!IsSaveas)
                {
                    if (string.IsNullOrEmpty(FileName))
                    {
                        FileName = DataSourceXmlBuffer + "\\" + routeInfo.SYSTEMABBRE + Path.GetFileNameWithoutExtension(SaveDialogFile) + "_" + GetFileName("") + ".xml";
                    }
                    else
                        FileName = DataSourceXmlBuffer + "\\" + FileName.Replace(".xml", "") + ".xml";
                    DeleteWithAdminRight(FileName);
                }
                else
                {
                    if (string.IsNullOrEmpty(FileName) || IsSaveas)
                    {
                        FileName = DataSourceXmlBuffer + "\\" + routeInfo.SYSTEMABBRE + Path.GetFileNameWithoutExtension(SaveDialogFile) + "_" + GetFileName("") + ".xml";
                    }
                }

                if (!File.Exists(FileName))
                {
                    var res = File.Create(FileName);
                    res.Flush();
                    res.Close();
                }

                XmlSerializer xs = new XmlSerializer(typeof(XMLBuffer.ID_Exchanger));

                TextWriter tw = new StreamWriter(FileName);
                DebugLog.WriteLog(FileName + " was exported.");
                var res1 = routeInfo.GetID_Changer(vdFC1, routeInfo, instListbox, ObstListbox);
                if (res1 == null)
                {
                    return false;
                }

                xs.Serialize(tw, res1);
                tw.Flush();
                tw.Close();
                fname = FileName;
                vdLayer vl = new vdLayer();
                vl.Name = Path.GetFileNameWithoutExtension(fname);// Fname;
                                                                  //lbl_savedLayer.Text = fname;
                lbl_savedLayer = fname;
                vl.SetUnRegisterDocument(this.vdFC1.BaseControl.ActiveDocument);
                vl.setDocumentDefaults();
                vl.VisibleOnForms = false;
                vdFC1.BaseControl.ActiveDocument.Layers.Add(vl);
                SaveXmlFile = FileName;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                MessageBox.Show(ex.StackTrace);
                return false;
            }
            return true;
        }
        string lbl_savedLayer = "";

        private void instDGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = instDGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)instDGV.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)instDGV.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            if (routeInfo == null || string.IsNullOrEmpty(vdFC1.BaseControl.ActiveDocument.FileName))
                return;

            var dlg = MessageBox.Show("현재 프로그램 상태를 저장하시겠습니까?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dlg != DialogResult.Yes)
                return;
            //if (rdo_WithoutRoute.Checked)
            //{
            //    MessageBox.Show("Saving for Auto Route option is still developing.");
            //    return;
            //}
            SaveDialogFile = vdFC1.BaseControl.ActiveDocument.FileName;

            if (WriteXML(false))
            {
                DeleteWithAdminRight(vdFC1.BaseControl.ActiveDocument.FileName);
                vdFC1.BaseControl.ActiveDocument.SaveAs(vdFC1.BaseControl.ActiveDocument.FileName);
                MessageBox.Show("성공적으로 저장되었습니다!");
                btnExport.Enabled = true;
                btnExport.ToolTipText = "Export Save";
            }
            else
            {
                MessageBox.Show("저장에 실패하였습니다. 다시 한 번 시도해주세요.");
            }
        }
        private void DeleteWithAdminRight(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                try
                {
                    if (!UseAdmin) // use admin
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            CreateNoWindow = true,
                            FileName = "cmd.exe",
                            Arguments = $"/c del \"{path}\"",
                            Verb = "runas"
                        };

                        Process.Start(psi);
                    }
                }
                catch (Exception exx)
                {
                }
            }
        }
        string SaveDialogFile = "";
        string SaveXmlFile = "";
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (routeInfo == null || string.IsNullOrEmpty(vdFC1.BaseControl.ActiveDocument.FileName))
                return;
            //if (rdo_WithoutRoute.Checked)
            //{
            //    MessageBox.Show("Saving for Auto Route option is still developing.");
            //    return;
            //}
            SaveFileDialog sd = new SaveFileDialog();

            sd.Filter = "AutoCAD|*.dwg|VDCL(*.vdcl)|*.vdcl";
            if (sd.ShowDialog(this) == DialogResult.OK)
            {
                SaveDialogFile = sd.FileName;// ;
                if (WriteXML())
                {
                    vdFC1.BaseControl.ActiveDocument.SaveAs(sd.FileName);
                    MessageBox.Show("성공적으로 저장되었습니다!");
                    btnExport.Enabled = true;
                    btnExport.ToolTipText = "Export Saveas";
                }
                else
                {
                    MessageBox.Show("저장에 실패하였습니다. 다시 한 번 시도해주세요.");
                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e) // Erase Function
        {
            if (routeInfo == null) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.ERASE;
            LstSelectedID = new List<int>();
            var res = vdFC1.BaseControl.ActiveDocument.CommandAction.CmdErase(null);
            if (res)
            {
                routeInfo.SyncronyzeRelatedUI(LstSelectedID, vdFC1);  // Prepare TBBox Info
                button10_Click(null, null); // Obstables
                cbo_layer_Setting_SelectedIndexChanged(null, null);// Customized Destination 
                int r1 = 0;
                DestinationBind(ref r1);
                routeInfo.UpdateInstrument(vdFC1, instDGV); // Instrument

            }

        }

        private void instLyrDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void instListbox_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            instListbox.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void instListbox_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                if ((ComboBox)datagridview.EditingControl == null) return;
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }
        bool IsInsAdd = false;
        private void instListbox_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            VisibleAll();
            IsCellEditing = true;
            if (instListbox.CurrentCell != null)
            {
                if (instListbox.CurrentCell.ColumnIndex > 0)
                {
                    if (chk_Notify.Checked && !IsInsAdd)
                    {
                        var msg = MessageBox.Show("**주의선택하신 레이어 내의 모든 설정 내용이 일괄 변경됩니다. 변경 내용은 복구되지 않습니다. 진행하시겠습니까?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (msg != DialogResult.OK) return;
                    }
                    foreach (DataGridViewRow r in instListbox.Rows)
                    {
                        foreach (DataGridViewRow dr in instDGV.Rows)
                        {
                            if (!(bool)(dr.Cells["col_Check_Instrument"].FormattedValue))
                                continue;
                            if (dr.Cells["LayerName"].Value != null)
                            {
                                if (r.Cells["dgv_Layer"].Value.ToString() == dr.Cells["LayerName"].Value.ToString())
                                {
                                    if (e.ColumnIndex == 2)
                                        dr.Cells["colSystem"].Value = r.Cells["dgv_System"].Value;
                                    if (e.ColumnIndex == 3)
                                        dr.Cells["colTo"].Value = r.Cells["Cable1"].Value;
                                    if (e.ColumnIndex == 1)
                                        dr.Cells["colType"].Value = r.Cells["dgv_Type_Ins"].Value;
                                }
                            }
                        }
                    }
                }
            }
            // SetInsCable();
            instDGV_CellEndEdit(null, null);
            //UpdateCurrentState();
        }
        private void updateInstGrid()
        {

            foreach (DataGridViewRow r in instListbox.Rows)
            {
                foreach (DataGridViewRow dr in instDGV.Rows)
                {
                    if (dr.Cells["LayerName"].Value != null)
                    {
                        if (r.Cells["dgv_Layer"].Value.ToString() == dr.Cells["LayerName"].Value.ToString())
                        {
                            dr.Cells["colType"].Value = r.Cells["dgv_Type_Ins"].Value;
                            dr.Cells["colSystem"].Value = r.Cells["dgv_System"].Value;
                            dr.Cells["colTo"].Value = r.Cells["Cable1"].Value;
                        }
                    }
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void MainP1_KeyUp(object sender, KeyEventArgs e)
        {
            var r = true;
            BaseControl_vdKeyUp(e, ref r);
        }

        private void vdFC1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            // ClearRouteCacheSetting();
            dgv_1.Rows.Clear();
            dgv_2.Rows.Clear();
            dgv_3.Rows.Clear();
            ClearGridView(dgv_SegAnalysis);
            ClearGridView(dgv_SubIns);
            ClearGridView(dgv_InsAnalysis);
            ClearGridView(dgv_SubSeg);
            progressBar1.Visible = true;
            if (routeInfo == null || GetMainBoundary() == null)
            {
                MessageBox.Show("외각 라인 설정이 필요합니다. 외각라인 추가를 해주시거나, 외각 라인이 있는 캐드 파일을 재입력해주세요. 외각 라인이 Boundary 레이어 내에 폴리라인으로 있을 경우 자동으로 인식됩니다.");
                return;
            }
            if (!routeInfo.CheckDuplicatedInstrument(vdFramedControl))
            {
                return;
            }
            if (thread != null) thread.Abort();
            //thread = null;
            thread = new Thread(delegate ()
            {
                analysis();
                this.Cursor = Cursors.Arrow;
            });
            thread.Start();
            this.Cursor = Cursors.Arrow;
            progressBar1.Value = 0;

        }
        private void DatagridSetting()
        {
            dgv_SubIns.EndEdit();
            try
            {
                if (dgv_SubIns.InvokeRequired)
                {
                    //Action safeWrite = delegate { dgv_SubIns.DataSource = null; };
                    //dgv_SubIns.Invoke(safeWrite);
                }
                else
                {
                    List<DataGridViewRow> gr = new List<DataGridViewRow>();
                    foreach (DataGridViewRow grr in dgv_SubIns.Rows)
                    {
                        gr.Add(grr);
                    }
                    foreach (DataGridViewRow grr in gr)
                    {
                        dgv_SubIns.Rows.Remove(grr);
                    }
                    //  dgv_SubIns.DataSource = null;
                }
                if (dgv_SubSeg.InvokeRequired)
                {
                    try
                    {
                        //Action safeWrite = delegate { dgv_SubSeg.DataSource = null; };
                        //dgv_SubSeg.Invoke(safeWrite);
                    }
                    catch { }
                }
                else
                {
                    List<DataGridViewRow> gr = new List<DataGridViewRow>();
                    foreach (DataGridViewRow grr in dgv_SubSeg.Rows)
                    {
                        gr.Add(grr);
                    }
                    foreach (DataGridViewRow grr in gr)
                    {
                        dgv_SubSeg.Rows.Remove(grr);
                    }
                    //  dgv_SubSeg.DataSource = null;
                }
            }
            catch
            {

            }
        }
        private void ScopeCheckSetting()
        {
            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            // routeInfo.LstGuidedRoute = routeInfo.LstAllRoute();
            if (rdo_WithoutRoute.Checked)
            {


                routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.LINE;
                foreach (vdLine connector in routeInfo.LstAutoRoute)
                {
                    foreach (var tb in routeInfo.LstTBBOXes)
                    {
                        var vdline = new vdLine();
                        vdline = connector;
                        var l = tb.MainRouteCollection.Where(x => x.getEndPoint() == vdline.getEndPoint() && x.getStartPoint() == vdline.getStartPoint());
                        var m = tb.MainRouteCollection.Where(x => x.getStartPoint() == vdline.getEndPoint() && x.getEndPoint() == vdline.getStartPoint());
                        var yl = tb.MainRouteCollection.Where(x => x.getEndPoint() == vdline.getEndPoint() && x.getStartPoint() == vdline.getStartPoint());
                        var xl = routeInfo.LstExternalRoute.Where(x => x.getStartPoint() == vdline.getEndPoint() && x.getEndPoint() == vdline.getStartPoint());
                        bool skip = false;
                        foreach (var t in routeInfo.LstTBBOXes)
                        {
                            if (t.Name != tb.Name)
                            {
                                foreach (var mr in t.MainRouteCollection)
                                {
                                    var gps = new gPoints();
                                    var res = mr.IntersectWith(vdline, VdConstInters.VdIntOnBothOperands, gps);
                                    if (res && gps.Count > 1)
                                    {
                                        skip = true;
                                        goto here;
                                    }
                                }
                            }
                        }

                        foreach (var x in routeInfo.LstExternalRoute)
                        {
                            var gps = new gPoints();
                            var res = x.IntersectWith(vdline, VdConstInters.VdIntOnBothOperands, gps);
                            if (res && gps.Count > 1)
                            {
                                skip = true;
                                goto here;
                            }
                        }
                    here:

                        if (!l.Any() && !m.Any() && !xl.Any() && !yl.Any() && !skip)
                        {
                            ConvertStraightLinePoly(connector.Id);
                        }
                    }
                }
            }
            if (rdo_UserDefine.Checked)
            {
                try
                {
                    var lstLine = routeInfo.LstAllRoute();

                    routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.LINE;
                    var en = new vdEntities();
                    try
                    {

                        foreach (vdLine connector in lstLine)
                        {
                            foreach (var tb in routeInfo.LstTBBOXes)
                            {
                                var vdline = new vdLine();
                                vdline = connector;
                                var l = tb.MainRouteCollection.Where(x => x.getEndPoint() == vdline.getEndPoint() && x.getStartPoint() == vdline.getStartPoint());
                                var m = tb.MainRouteCollection.Where(x => x.getStartPoint() == vdline.getEndPoint() && x.getEndPoint() == vdline.getStartPoint());
                                var yl = tb.MainRouteCollection.Where(x => x.getEndPoint() == vdline.getEndPoint() && x.getStartPoint() == vdline.getStartPoint());
                                var xl = routeInfo.LstExternalRoute.Where(x => x.getStartPoint() == vdline.getEndPoint() && x.getEndPoint() == vdline.getStartPoint());
                                bool skip = false;
                                foreach (var t in routeInfo.LstTBBOXes)
                                {
                                    if (t.Name != tb.Name)
                                    {
                                        foreach (var mr in t.MainRouteCollection)
                                        {
                                            if (mr.Id == vdline.Id) continue;
                                            var gps = new gPoints();
                                            var res = mr.IntersectWith(vdline, VdConstInters.VdIntOnBothOperands, gps);
                                            if (res && gps.Count > 1)
                                            {
                                                skip = true;
                                                goto here;
                                            }
                                        }
                                    }
                                }

                                foreach (var x in routeInfo.LstExternalRoute)
                                {
                                    if (x.Id == vdline.Id) continue;
                                    var gps = new gPoints();
                                    var res = x.IntersectWith(vdline, VdConstInters.VdIntOnBothOperands, gps);
                                    if (res && gps.Count > 1)
                                    {
                                        skip = true;
                                        goto here;
                                    }
                                }
                            here:

                                if (!l.Any() && !m.Any() && !xl.Any() && !yl.Any() && !skip)
                                {
                                    ConvertStraightLinePoly(connector.Id);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // DebugLog.WriteLog(ex);

                    }
                    lstLine = routeInfo.LstAllRoute();

                    routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.LINE;
                    List<int> deleteLines = new List<int>();
                    foreach (vdLine c1 in lstLine)
                    {
                        foreach (vdLine c2 in lstLine)
                        {
                            if (c1 == c2) continue;

                            var gps = new gPoints();
                            var res = c1.IntersectWith(c2, VdConstInters.VdIntOnBothOperands, gps);
                            if (res && gps.Count > 1)
                            {
                                var cx_1 = c1.getStartPoint().x >= c2.getStartPoint().x && c1.getEndPoint().x <= c2.getEndPoint().x && c1.getStartPoint().y == c2.getStartPoint().y;
                                var cx_2 = c1.getStartPoint().x >= c2.getEndPoint().x && c1.getEndPoint().x <= c2.getStartPoint().x && c1.getStartPoint().y == c2.getStartPoint().y;

                                var cy_1 = c1.getStartPoint().y >= c2.getStartPoint().y && c1.getEndPoint().y <= c2.getEndPoint().y && c1.getStartPoint().x == c2.getStartPoint().x;
                                var cy_2 = c1.getStartPoint().y >= c2.getEndPoint().y && c1.getEndPoint().y <= c2.getStartPoint().y && c1.getStartPoint().x == c2.getStartPoint().x;

                                if (cx_1 || cx_2 || cy_1 || cy_2)
                                {
                                    if (c1.Length() > c2.Length())
                                    {
                                        if (!deleteLines.Contains(c2.Id))
                                            deleteLines.Add(c2.Id);
                                    }
                                    else
                                    {
                                        if (!deleteLines.Contains(c1.Id))
                                            deleteLines.Add(c1.Id);

                                    }
                                }

                            }
                        }
                    }
                    routeInfo.SyncronyzeRelatedUI(deleteLines, vdFC1);
                }
                catch (Exception ex)
                {

                }
            }

            var lstRoute = routeInfo.LstAllRoute();
            foreach (var line in lstRoute)
            {
                foreach (var tb in routeInfo.LstTBBOXes)
                {
                    var gps = new gPoints();
                    var res = tb.polyline.IntersectWith(line, VdConstInters.VdIntOnBothOperands, gps);
                    if (res && gps.Count > 1)
                    {
                        Dictionary<vdLine, gPoint> dicIntersec = new Dictionary<vdLine, gPoint>();
                        try
                        {
                            var removeLine = new vdLine(line.StartPoint, line.EndPoint);
                            routeInfo.SyncronyzeRelatedUI(new List<int>() { line.Id }, vdFC1);

                            var newline1 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                            var midPoint = (new vdLine(gps[0], gps[1])).BoundingBox.MidPoint;
                            newline1.StartPoint = midPoint;
                            newline1.EndPoint = removeLine.StartPoint;
                            et.AddItem(newline1);

                            var newline2 = new vdLine(this.vdFC1.BaseControl.ActiveDocument);
                            newline2.StartPoint = midPoint;
                            newline2.EndPoint = removeLine.EndPoint;
                            et.AddItem(newline2);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
        private void InternalSegmentSetting()
        {
            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems();
            List<int> lstInternalLine = new List<int>();
            foreach (vdFigure vf in et)
            {
                if (vf is vdLine vl)
                {
                    var isInside = routeInfo.LstTBBOXes.Where(x => (x.polyline.BoundingBox.PointInBox(vl.StartPoint)) && (x.polyline.BoundingBox.PointInBox(vl.EndPoint)));
                    if (isInside.Any())
                    {
                        lstInternalLine.Add(vl.Id);
                    }
                }
            }
            routeInfo.SyncronyzeRelatedUI(lstInternalLine, vdFramedControl);
        }
        private void UpdateRoutetoCADSetting()
        {
            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            List<vdLine> lst = new List<vdLine>();
            foreach (vdLine vf in routeInfo.LstExternalRoute)
            {

                if (vf is vdLine vl)
                {
                    if (!et.FindItem(vl) || vl.Deleted || Convert.ToInt32(vl.Length()) == 0)
                    {
                        lst.Add(vl);
                        // routeInfo.SyncronyzeRelatedUI(new List<int> { vl.Id }, vdFramedControl);
                    }
                }
            }

            foreach (var l in lst)
            {
                try
                {
                    routeInfo.LstExternalRoute.Remove(l);
                }
                catch
                { }
                routeInfo.SyncronyzeRelatedUI(new List<int> { l.Id }, vdFramedControl);
            }

        }
        private void analysis()
        {
            try
            {
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = 1; }));
                }
                routeInfo.DoneAnalysis = false;
                progressBar1.Value = 2;
                progressBar1.Value = 5;
                DatagridSetting();
                progressBar1.Value = 8;
                ClearRouteCacheSetting();
                progressBar1.Value = 10;
                ScopeCheckSetting();
                progressBar1.Value = 15;
                InternalSegmentSetting();
                //MessageBox.Show("wwe");
                // progressBar1.Value = 15;
                //  SinglePolySetting();
                progressBar1.Value = 18;
                UpdateRoutetoCADSetting();
                progressBar1.Value = 25;
                if (!AnalyzeSetting())
                {
                    progressBar1.Value = 100;
                    progressBar1.Value = 0;
                    progressBar1.Visible = false;

                    routeInfo.DoneAnalysis = true;
                    return;
                }
                progressBar1.Value = 30;
                LabelSegementSetting();
                progressBar1.Value = 40;
                RouteSetting();
                progressBar1.Value = 50;
                DestinationSetting();
                progressBar1.Value = 60;
                MCCPkgSetting();
                progressBar1.Value = 80;
                SegmentaionSetting();
                progressBar1.Value = 90;

                //  DrawColorSetting();
                progressBar1.Value = 100;

                MessageBox.Show("경로 분석이 성공적으로 완료되었습니다!");
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show("경로 분석에 실패하였습니다." + Environment.NewLine + ex.Message + ex.StackTrace);
            }
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = 0; }));
            }
            progressBar1.Visible = false;

            routeInfo.DoneAnalysis = true;
            BindDecimalplace();
        }
        private void ClearRouteCacheSetting()
        {
            routeInfo.AllNodeInEachDuct = new Dictionary<vdLine, List<gPoint>>();
            routeInfo.LstInsRouteInfo = new List<InstrumentRouteInfoEntity>();
            routeInfo.LstSegmentInfo = new List<SegmentInfoEntity>();
            routeInfo.AllRouteCollections = new Dictionary<vdLine, gPoints>();
            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            List<vdText> lttxt = new List<vdText>();
            foreach (var f in e)
            {
                try
                {
                    if (f is vdText vdText && vdText.TextString.Contains(routeInfo.SEGMENTABBRE)) lttxt.Add(vdText);
                }
                catch
                {

                }
            }
            foreach (var t in lttxt)
            {
                e.RemoveItem(t);
            }

            ClearConnector();
            foreach (var t in routeInfo.LstTextName_Cricle_Cache)
            {
                e.RemoveItem(t);
            }

            RefreshCADSpace();
        }

        private void ClearConnector()
        {
            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            List<int> ids = new List<int>();
            foreach (var c in routeInfo.lstConnector)
            {
                ids.Add(c.Id);
            }
            routeInfo.SyncronyzeRelatedUI(ids, vdFramedControl, true);
            routeInfo.lstConnector.Clear();
        }
        private void RefreshCADSpace()
        {
            try
            {
                vdFC1.BaseControl.Redraw();
                vdFC1.BaseControl.Update();
                vdFC1.Update();
                //vdFC1.BaseControl.ActiveDocument.Redraw(true);
                //vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Refresh();
                vdFC1.BaseControl.ActiveDocument.ActiveLayer.Update();
            }
            catch
            {

            }
        }
        private void MCCPkgSetting()
        {
            lstRouteInfoTemp = new List<InstrumentRouteInfoEntity>();
            var sbl = new RouteOptimizer.BL.SettingBL();
            var sigDuct = sbl.GetSignalType();
            foreach (var lr in routeInfo.LstTBBOXes)
            {
                //var selectedIns = routeInfo.LstallInstruments.Where(x => x.t1 == lr.Instrument.t1 && x.t2 == lr.Instrument.t2);

                //if (!selectedIns.Any()) continue;
                if (lr.IsIO != eDestinationType.MCC) continue;
                var cableIns = lr.LstmCCEntities;

                foreach (var ins in cableIns)
                {
                    if (String.IsNullOrEmpty(ins.Signal)) continue;
                    if (!sigDuct.Where(x => x.Title == ins.Signal).Any())  // AI/DI/ DO  System
                    {
                        MessageBox.Show("The system need to make a setting in Signal Form. It might be there is no respective assigned duct or no system type setting in Signal list. Please check in SignalList form.");
                        return;
                    }
                    else
                    {
                        ins.SignalType = sigDuct.Where(x => x.Title == ins.Signal).FirstOrDefault().AssignedCableDuct;
                    }
                }
            }
            //string Type = "Destination";
            //string System = "Destination";
            foreach (var tb in routeInfo.LstTBBOXes)
            {
                if (tb.IsIO != eDestinationType.MCC) continue;
                lstRouteInfoTemp = new List<InstrumentRouteInfoEntity>();
                if (tb.LstmCCEntities != null && tb.LstmCCEntities.Count > 0)
                {
                    foreach (var mcce in tb.LstmCCEntities)
                    {
                        if (string.IsNullOrEmpty(mcce.To) || mcce.To == "-1") continue;
                        InstCableEntity ins = new InstCableEntity();
                        ins.Cable = mcce.CableSpecifications; // PTK changed 20
                        ins.To = routeInfo.FindIO_TBBox(Convert.ToInt32(mcce.To)).Name;
                        ins.col_Signal = mcce.SignalType;
                        ins.System = mcce.Signal;

                        //Type = mcce.SignalType;
                        //System = mcce.Signal;

                        LstDuctLinked = new List<vdLine>();
                        bool IsAlreadyFound = false;

                        if (IsAlreadyFound)
                        {
                            foreach (var p in tb.MainRouteCollection) // check Endpoint is Owner if start point is this 
                            {
                                if (tb.OwnDestination.polyline.GetGripPoints().IsPointInside(p.getEndPoint()))
                                {
                                    var startpoint = p.getEndPoint();
                                    List<string> subSeg = new List<string>();
                                    double len = 0.0;
                                    while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.Name).Any())
                                    {
                                        var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.Name);
                                        subSeg.Add(selectSeg.FirstOrDefault().SegmentName);
                                        startpoint = selectSeg.FirstOrDefault().EndPoint;
                                        IsAlreadyFound = true;

                                    }
                                    InstrumentRouteInfoEntity ire = new InstrumentRouteInfoEntity()
                                    {
                                        Instrument = new Instrument(new vdCircle(tb.Circle.Document, tb.polyline.BoundingBox.MidPoint, tb.polyline.BoundingBox.Height / 2), tb.Name),  // convert to instrument from TBBoxes
                                        CableType = ins.Cable,
                                        Length = len,
                                        LstSegment = subSeg,
                                        To = tb.OwnDestination.Name

                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    break;
                                }
                                //}

                                //if (!IsAlreadyFound)
                                //{
                                foreach (var tb1 in routeInfo.LstTBBOXes) // check Endpoint is target if start point is this 
                                {
                                    if (tb1.Name != tb.Name)
                                    {
                                        foreach (var p1 in tb1.MainRouteCollection)
                                        {
                                            if (tb.polyline.GetGripPoints().IsPointInside(p1.getEndPoint()))
                                            {
                                                var startpoint = p1.getEndPoint();

                                                List<string> subSeg = new List<string>();
                                                double len = 0.0;
                                                while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name).Any())
                                                {
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SegmentName);
                                                    startpoint = selectSeg.FirstOrDefault().EndPoint;
                                                    IsAlreadyFound = true;
                                                }
                                                InstrumentRouteInfoEntity ire = new InstrumentRouteInfoEntity()
                                                {
                                                    Instrument = new Instrument(new vdCircle(tb.Circle.Document, tb.polyline.BoundingBox.MidPoint, tb.polyline.BoundingBox.Height / 2), tb.Name),  // convert to instrument from TBBoxes
                                                    CableType = ins.Cable,
                                                    Length = len,
                                                    LstSegment = subSeg,
                                                    To = tb.OwnDestination.Name
                                                };
                                                lstRouteInfoTemp.Add(ire);
                                                break;
                                            }
                                        }
                                        if (IsAlreadyFound)
                                            break;
                                    }
                                }

                            }
                        }
                        List<DestinationPathEntity> Lstdpe = new List<DestinationPathEntity>();
                        foreach (var pr in tb.MainRouteCollection)
                        {
                            LstDuctLinked = new List<vdLine>();
                            var ie = new Instrument();
                            ie.PosssessDestination = tb;
                            ie.PossessPoint = pr.getStartPoint();
                            ie.PosssessRoute = pr;
                            ie.t1 = tb.Name;
                            List<Instrument> iel = new List<Instrument>() { };
                            iel.Add(ie);
                            IEnumerable<Instrument> selectedIns = iel;
                            var refPoly = selectedIns.FirstOrDefault().PosssessRoute;
                            LstDuctLinked.Add(refPoly);
                            LstDuctLinkedtemp = new List<vdLine>();
                            LstDuctLinkedtemp.Add(LstDuctLinked[0]);
                            LstAllPath = new List<List<vdLine>>() { };
                            Recursive_DFS(selectedIns, ins.To, selectedIns.FirstOrDefault().PosssessDestination, refPoly);
                            if (LstAllPath.Count > 0)
                            {
                                DestinationPathEntity destinationPathEntity = new DestinationPathEntity()
                                {
                                    LstPath = LstAllPath,
                                    selectedIns = selectedIns
                                };
                                Lstdpe.Add(destinationPathEntity);
                            }
                        }
                        foreach (var lspath in Lstdpe)
                        {
                            // filter coinccident point 3 or 4 point joints

                            var selectedIns = lspath.selectedIns;
                            foreach (var lsp in lspath.LstPath)
                            {
                                var e_tri = lsp.GroupBy(w => w.getEndPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                var s_tri = lsp.GroupBy(w => w.getStartPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                List<gPoint> lstCheckDuplicate = new List<gPoint>();
                                foreach (var l in lsp)
                                {
                                    lstCheckDuplicate.Add(l.StartPoint);
                                    lstCheckDuplicate.Add(l.EndPoint);
                                }
                                var duplicatecheck = lstCheckDuplicate.GroupBy(w => w).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                if (e_tri.Any() || s_tri.Any() || duplicatecheck.Any())
                                {
                                    continue;
                                }
                                LstDuctLinkedtemp = RemoveExtraLeadingLine(tb.centerPoint, lsp, selectedIns.FirstOrDefault(), true);  // removing leading lines (eg actual is 2,3  but in program 10,2,3    10 and 2 have same zipzap condition )

                                //if (e_tri.Any() || s_tri.Any())
                                //{
                                //    continue;
                                //}
                                // LstDuctLinkedtemp = lsp;
                                InstrumentRouteInfoEntity ire = null;
                                if (LstDuctLinkedtemp.Count > 1)
                                {
                                    double len = 0.0;
                                    List<string> subSeg = new List<string>();
                                    var siLine = selectedIns.FirstOrDefault().PosssessRoute;
                                    gPoint FinalInsterSection = null;
                                    foreach (var cline in LstDuctLinkedtemp)
                                    {
                                        if (siLine.Id == cline.Id) continue;
                                        gPoints rp1 = new gPoints();
                                        var res1 = siLine.IntersectWith(cline, VdConstInters.VdIntOnBothOperands, rp1);
                                        if (res1)
                                        {
                                            DataTable dataTable = new DataTable();
                                            dataTable.Columns.Add("SG");
                                            dataTable.Columns.Add("SP");
                                            dataTable.Columns.Add("EP");
                                            dataTable.Columns.Add("ID");
                                            dataTable.Columns.Add("TB");
                                            foreach (var l in routeInfo.LstSegmentInfo)
                                            {
                                                dataTable.Rows.Add(
                                                new object[] {
                                            l.SegmentName, l.StartPoint, l.EndPoint, l.ParentRouteID, l.ParentDestination
                                                }
                                                );
                                            }

                                            bool IsReachIntersection = false;
                                            if (FinalInsterSection == null) FinalInsterSection = selectedIns.FirstOrDefault().PossessPoint;
                                            try
                                            {
                                                var r = siLine.getDistAtPoint(FinalInsterSection);
                                            }
                                            catch (Exception ex)
                                            {
                                                var msg = ex.StackTrace;
                                                continue;
                                            }
                                            if (siLine.getDistAtPoint(rp1[0]) > siLine.getDistAtPoint(FinalInsterSection))
                                            {
                                                var EndPoint = FinalInsterSection; //selectedIns.FirstOrDefault().PossessPoint;
                                                                                   // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                while (routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                {
                                                    if (IsReachIntersection) break;
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                    if (selectSeg.FirstOrDefault().StartPoint == rp1[0])
                                                    {

                                                        IsReachIntersection = true;
                                                        FinalInsterSection = rp1[0];
                                                    }
                                                    else
                                                    {
                                                        FinalInsterSection = selectSeg.FirstOrDefault().StartPoint;
                                                    }
                                                    //len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                    EndPoint = selectSeg.FirstOrDefault().StartPoint;

                                                }
                                            }
                                            else
                                            {
                                                var Startpoint = FinalInsterSection; ///selectedIns.FirstOrDefault().PossessPoint;
                                                // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == Startpoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                {
                                                    if (IsReachIntersection) break;
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == Startpoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                    if (selectSeg.FirstOrDefault().EndPoint == rp1[0])
                                                    {
                                                        IsReachIntersection = true;
                                                        FinalInsterSection = rp1[0];
                                                    }
                                                    else
                                                    {
                                                        FinalInsterSection = selectSeg.FirstOrDefault().EndPoint;
                                                    }
                                                    subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                    Startpoint = selectSeg.FirstOrDefault().EndPoint;
                                                }
                                            }
                                        }
                                        siLine = cline;
                                    }

                                    var startpoint = FinalInsterSection;
                                    while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == ins.To && x.SignalType == ins.col_Signal).Any())
                                    {
                                        var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == ins.To && x.SignalType == ins.col_Signal);

                                        subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                        startpoint = selectSeg.FirstOrDefault().EndPoint;
                                    }

                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = len,
                                        LstSegment = subSeg,
                                        To = ins.To
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    // routeInfo.LstInsRouteInfo.Add(ire);
                                }
                                else if (LstDuctLinkedtemp.Count != 1)
                                {

                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = 0.0,
                                        LstSegment = new List<string>() { },
                                        To = ins.To
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    //routeInfo.LstInsRouteInfo.Add(ire);
                                }
                            }
                        }

                        foreach (var lrtemp in lstRouteInfoTemp)
                        {
                            var len = 0.0;
                            foreach (var seg in lrtemp.LstSegment)
                            {
                                var s = routeInfo.LstSegmentInfo.Where(x => x.SegmentName == seg.Split(',').Last() && x.SignalType == seg.Split(',').First()).FirstOrDefault();
                                len += s.Length;
                            }
                            lrtemp.Length = len;
                        }
                        if (lstRouteInfoTemp.Count > 0)
                        {
                            var zeroLength = lstRouteInfoTemp.Where(x => Convert.ToInt32(x.Length) == 0);
                            if (zeroLength.Any())
                            {

                                var lsttemp = new List<InstrumentRouteInfoEntity>() { };
                                foreach (var f in zeroLength)
                                {
                                    lsttemp.Add(f);
                                }
                                foreach (var f in lsttemp)
                                {
                                    lstRouteInfoTemp.Remove(f);
                                }
                            }
                        }
                        {
                            var GetShortestPath = from c in lstRouteInfoTemp
                                                  group c by new
                                                  {
                                                      c.SignalType,
                                                      c.SystemType,

                                                      c.Instrument.t1,
                                                      c.Instrument.t2,
                                                      c.CableType,
                                                      c.To
                                                  } into gcs
                                                  select new InstrumentRouteInfoEntity()
                                                  {
                                                      SignalType = gcs.Key.SignalType,
                                                      SystemType = gcs.Key.SystemType,

                                                      Instrument = gcs.FirstOrDefault().Instrument,
                                                      CableType = gcs.Key.CableType,
                                                      To = gcs.Key.To,
                                                      Length = gcs.Min(c => c.Length),
                                                      LstSegment = gcs.Where(c => c.Length == (gcs.Min(c1 => c1.Length))).FirstOrDefault().LstSegment,
                                                  };
                            foreach (var pi in GetShortestPath)
                                routeInfo.LstInsRouteInfo.Add(pi);
                        }
                    }
                }
            }
        }
        private void DestinationSetting()
        {
            var sbl = new RouteOptimizer.BL.SettingBL();
            var sigDuct = sbl.GetSignalType();
            //string Type = "Destination";
            //string System = "Destination";
            //  lstRouteInfoTemp = new List<InstrumentRouteInfoEntity>();
            foreach (var tb in routeInfo.LstTBBOXes)
            {

            }

            foreach (var tb in routeInfo.LstTBBOXes)
            {


                if (tb.IsIO == eDestinationType.MCC) continue;
                lstRouteInfoTemp = new List<InstrumentRouteInfoEntity>();
                if (tb.OwnDestination != null && !string.IsNullOrEmpty(tb.OwnDestination.Name))
                {
                    foreach (var cbl in tb.LstmCCEntities)
                    {
                        InstCableEntity ins = new InstCableEntity();
                        ins.Cable = cbl.CableSpecifications;//tb.CableType; // PTK changed 20
                        ins.To = tb.OwnDestination.Name;
                        //ins.col_Signal = cbl.Signal; //Type; eg Power/Comm // Number of Ductypes
                        ins.System = cbl.Signal; //System;  eg AI/DI

                        if (!sigDuct.Where(x => x.Title == ins.System).Any())
                        {
                            MessageBox.Show("The system need to make a setting in Signal Form. It might be there is no respective assigned duct or no system type setting in Signal list. Please check in SignalList form.");
                            return;
                        }
                        else
                        {
                            ins.col_Signal = sigDuct.Where(x => x.Title == ins.System).FirstOrDefault().AssignedCableDuct;
                        }



                        LstDuctLinked = new List<vdLine>();
                        bool IsAlreadyFound = false;
                        #region Off
                        if (IsAlreadyFound)
                        {
                            foreach (var p in tb.MainRouteCollection) // check Endpoint is Owner if start point is this 
                            {
                                if (tb.OwnDestination.polyline.GetGripPoints().IsPointInside(p.getEndPoint()))
                                {
                                    var startpoint = p.getEndPoint();
                                    List<string> subSeg = new List<string>();
                                    double len = 0.0;
                                    while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.Name).Any())
                                    {
                                        var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.Name);
                                        subSeg.Add(selectSeg.FirstOrDefault().SegmentName);
                                        startpoint = selectSeg.FirstOrDefault().EndPoint;
                                        IsAlreadyFound = true;

                                    }
                                    InstrumentRouteInfoEntity ire = new InstrumentRouteInfoEntity()
                                    {
                                        Instrument = new Instrument(new vdCircle(tb.Circle.Document, tb.polyline.BoundingBox.MidPoint, tb.polyline.BoundingBox.Height / 2), tb.Name),  // convert to instrument from TBBoxes
                                        CableType = ins.Cable,
                                        Length = len,
                                        LstSegment = subSeg,
                                        To = tb.OwnDestination.Name

                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    break;
                                }
                                //}

                                //if (!IsAlreadyFound)
                                //{
                                foreach (var tb1 in routeInfo.LstTBBOXes) // check Endpoint is target if start point is this 
                                {
                                    if (tb1.Name != tb.Name)
                                    {
                                        foreach (var p1 in tb1.MainRouteCollection)
                                        {
                                            if (tb.polyline.GetGripPoints().IsPointInside(p1.getEndPoint()))
                                            {
                                                var startpoint = p1.getEndPoint();

                                                List<string> subSeg = new List<string>();
                                                double len = 0.0;
                                                while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name).Any())
                                                {
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SegmentName);
                                                    startpoint = selectSeg.FirstOrDefault().EndPoint;
                                                    IsAlreadyFound = true;
                                                }
                                                InstrumentRouteInfoEntity ire = new InstrumentRouteInfoEntity()
                                                {
                                                    Instrument = new Instrument(new vdCircle(tb.Circle.Document, tb.polyline.BoundingBox.MidPoint, tb.polyline.BoundingBox.Height / 2), tb.Name),  // convert to instrument from TBBoxes
                                                    CableType = ins.Cable,
                                                    Length = len,
                                                    LstSegment = subSeg,
                                                    To = tb.OwnDestination.Name
                                                };
                                                lstRouteInfoTemp.Add(ire);
                                                break;
                                            }
                                        }
                                        if (IsAlreadyFound)
                                            break;
                                    }
                                }

                            }
                        }
                        //if (!IsAlreadyFound) //if not joined directly
                        //{
                        // LstAllPath = new List<List<vdPolyline>>() { }; 
                        #endregion
                        List<DestinationPathEntity> Lstdpe = new List<DestinationPathEntity>();
                        foreach (var pr in tb.MainRouteCollection)
                        {
                            LstDuctLinked = new List<vdLine>();
                            var ie = new Instrument();
                            ie.PosssessDestination = tb;
                            ie.PossessPoint = pr.getStartPoint();
                            ie.PosssessRoute = pr;
                            ie.t1 = tb.Name;
                            List<Instrument> iel = new List<Instrument>() { };
                            iel.Add(ie);
                            IEnumerable<Instrument> selectedIns = iel;
                            var refPoly = selectedIns.FirstOrDefault().PosssessRoute;
                            LstDuctLinked.Add(refPoly);
                            LstDuctLinkedtemp = new List<vdLine>();
                            LstDuctLinkedtemp.Add(LstDuctLinked[0]);
                            LstAllPath = new List<List<vdLine>>() { };
                            Recursive_DFS(selectedIns, tb.OwnDestination.Name, selectedIns.FirstOrDefault().PosssessDestination, refPoly);
                            if (LstAllPath.Count > 0)
                            {
                                DestinationPathEntity destinationPathEntity = new DestinationPathEntity()
                                {
                                    LstPath = LstAllPath,
                                    selectedIns = selectedIns
                                };
                                Lstdpe.Add(destinationPathEntity);
                            }
                        }
                        foreach (var lspath in Lstdpe)
                        {
                            // filter coinccident point 3 or 4 point joints

                            var selectedIns = lspath.selectedIns;
                            foreach (var lsp in lspath.LstPath)
                            {
                                var e_tri = lsp.GroupBy(w => w.getEndPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                var s_tri = lsp.GroupBy(w => w.getStartPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                List<gPoint> lstCheckDuplicate = new List<gPoint>();
                                foreach (var l in lsp)
                                {
                                    lstCheckDuplicate.Add(l.StartPoint);
                                    lstCheckDuplicate.Add(l.EndPoint);
                                }
                                var duplicatecheck = lstCheckDuplicate.GroupBy(w => w).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                if (e_tri.Any() || s_tri.Any() || duplicatecheck.Any())
                                {
                                    continue;
                                }
                                LstDuctLinkedtemp = RemoveExtraLeadingLine(tb.centerPoint, lsp, selectedIns.FirstOrDefault(), true);  // removing leading lines (eg actual is 2,3  but in program 10,2,3    10 and 2 have same zipzap condition )

                                //if (e_tri.Any() || s_tri.Any())
                                //{
                                //    continue;
                                //}
                                //LstDuctLinkedtemp = lsp;
                                InstrumentRouteInfoEntity ire = null;
                                if (LstDuctLinkedtemp.Count > 1)
                                {
                                    double len = 0.0;
                                    List<string> subSeg = new List<string>();
                                    var siLine = selectedIns.FirstOrDefault().PosssessRoute;
                                    gPoint FinalInsterSection = null;
                                    foreach (var cline in LstDuctLinkedtemp)
                                    {
                                        if (siLine.Id == cline.Id) continue;
                                        gPoints rp1 = new gPoints();
                                        var res1 = siLine.IntersectWith(cline, VdConstInters.VdIntOnBothOperands, rp1);
                                        if (res1)
                                        {
                                            DataTable dataTable = new DataTable();
                                            dataTable.Columns.Add("SG");
                                            dataTable.Columns.Add("SP");
                                            dataTable.Columns.Add("EP");
                                            dataTable.Columns.Add("ID");
                                            dataTable.Columns.Add("TB");
                                            foreach (var l in routeInfo.LstSegmentInfo)
                                            {
                                                dataTable.Rows.Add(
                                                new object[] {
                                            l.SegmentName, l.StartPoint, l.EndPoint, l.ParentRouteID, l.ParentDestination
                                                }
                                                );
                                            }

                                            bool IsReachIntersection = false;
                                            if (FinalInsterSection == null) FinalInsterSection = selectedIns.FirstOrDefault().PossessPoint;
                                            try
                                            {
                                                var r = siLine.getDistAtPoint(FinalInsterSection);
                                            }
                                            catch (Exception ex)
                                            {
                                                var msg = ex.StackTrace;
                                                continue;
                                            }
                                            if (siLine.getDistAtPoint(rp1[0]) > siLine.getDistAtPoint(FinalInsterSection))
                                            {
                                                var EndPoint = FinalInsterSection; //selectedIns.FirstOrDefault().PossessPoint;
                                                                                   // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                while (routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                {
                                                    if (IsReachIntersection) break;
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                    if (selectSeg.FirstOrDefault().StartPoint == rp1[0])
                                                    {

                                                        IsReachIntersection = true;
                                                        FinalInsterSection = rp1[0];
                                                    }
                                                    else
                                                    {
                                                        FinalInsterSection = selectSeg.FirstOrDefault().StartPoint;
                                                    }
                                                    //len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                    EndPoint = selectSeg.FirstOrDefault().StartPoint;

                                                }
                                            }
                                            else
                                            {
                                                var Startpoint = FinalInsterSection; ///selectedIns.FirstOrDefault().PossessPoint;
                                                // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == Startpoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                {
                                                    if (IsReachIntersection) break;
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == Startpoint && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                    if (selectSeg.FirstOrDefault().EndPoint == rp1[0])
                                                    {
                                                        IsReachIntersection = true;
                                                        FinalInsterSection = rp1[0];
                                                    }
                                                    else
                                                    {
                                                        FinalInsterSection = selectSeg.FirstOrDefault().EndPoint;
                                                    }
                                                    //len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                    Startpoint = selectSeg.FirstOrDefault().EndPoint;
                                                }
                                            }
                                            //FinalInsterSection = rp1[0];
                                        }
                                        siLine = cline;
                                        //  UsedLine.Add(siLine);
                                        //}
                                    }

                                    var startpoint = FinalInsterSection;
                                    //   var resDe = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == routeInfo.LstTBBOXes.Where(y => y.guid.ToString() == ins.To).FirstOrDefault().Name);
                                    while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name && x.SignalType == ins.col_Signal).Any())
                                    {
                                        // var selectSeg = resDe;
                                        var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == tb.OwnDestination.Name && x.SignalType == ins.col_Signal);

                                        //len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                        subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                        startpoint = selectSeg.FirstOrDefault().EndPoint;
                                    }

                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = len,
                                        LstSegment = subSeg,
                                        To = tb.OwnDestination.Name
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    // routeInfo.LstInsRouteInfo.Add(ire);
                                }
                                else if (LstDuctLinkedtemp.Count != 1)
                                {

                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = 0.0,
                                        LstSegment = new List<string>() { },
                                        To = tb.OwnDestination.Name
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    //routeInfo.LstInsRouteInfo.Add(ire);
                                }
                            }
                        }

                        foreach (var lrtemp in lstRouteInfoTemp)
                        {
                            var len = 0.0;
                            foreach (var seg in lrtemp.LstSegment)
                            {
                                var s = routeInfo.LstSegmentInfo.Where(x => x.SegmentName == seg.Split(',').Last() && x.SignalType == seg.Split(',').First()).FirstOrDefault();
                                len += s.Length;
                            }
                            lrtemp.Length = len;
                        }

                        #region Filter Shortest
                        if (lstRouteInfoTemp.Count > 0)
                        {
                            var zeroLength = lstRouteInfoTemp.Where(x => Convert.ToInt32(x.Length) == 0);
                            if (zeroLength.Any())
                            {

                                var lsttemp = new List<InstrumentRouteInfoEntity>() { };
                                foreach (var f in zeroLength)
                                {
                                    lsttemp.Add(f);
                                }
                                foreach (var f in lsttemp)
                                {
                                    lstRouteInfoTemp.Remove(f);
                                }
                            }
                        }

                        var GetShortestPath = from c in lstRouteInfoTemp
                                              group c by new
                                              {
                                                  c.SignalType,
                                                  c.SystemType,

                                                  c.Instrument.t1,
                                                  c.Instrument.t2,
                                                  c.CableType,
                                                  c.To
                                              } into gcs
                                              select new InstrumentRouteInfoEntity()
                                              {
                                                  SignalType = gcs.Key.SignalType,
                                                  SystemType = gcs.Key.SystemType,

                                                  Instrument = gcs.FirstOrDefault().Instrument,
                                                  CableType = gcs.Key.CableType,
                                                  To = gcs.Key.To,
                                                  Length = gcs.Min(c => c.Length),
                                                  LstSegment = gcs.Where(c => c.Length == (gcs.Min(c1 => c1.Length))).FirstOrDefault().LstSegment,
                                              };
                        foreach (var pi in GetShortestPath)
                            routeInfo.LstInsRouteInfo.Add(pi);
                        #endregion
                    }
                }
            }
        }
        delegate void SetTableCallback(DataTable dt);
        private void SegmentaionSetting(bool isSystemFile = false)
        {
            try
            {
                BL.SettingBL sbl = new BL.SettingBL();
                //var lstInscable = sbl.GetInsCable();
                var lstCable = sbl.GetCableList();
                var TileDuctlist = sbl.GetCableDuctList();
                DataTable dtIns = new DataTable();
                dtIns.Columns.Add("A_SignalType");
                dtIns.Columns.Add("A_SystemType");
                dtIns.Columns.Add("Instrument");
                dtIns.Columns.Add("Type");
                dtIns.Columns.Add("Length", typeof(double));
                dtIns.Columns.Add("TotalSegment");
                dtIns.Columns.Add("To");

                foreach (var f in routeInfo.LstInsRouteInfo)
                {
                    double len = 0.0;
                    foreach (var g in f.LstSegment)
                    {
                        if (routeInfo.LstSegmentInfo == null || routeInfo.LstSegmentInfo.Count == 0) continue;
                        len += routeInfo.LstSegmentInfo.Where(x => x.SegmentName == g.Split(',').Last() && x.SignalType == g.Split(',').First()).FirstOrDefault().Length;
                    }
                    f.Length = len;
                    if (f.Instrument != null)
                        dtIns.Rows.Add(
                            new object[] {
                            f.SignalType,
                            f.SystemType,
                    !String.IsNullOrEmpty(f.Instrument.t2) ?  ( f.Instrument.t1+"_" + f.Instrument.t2 ) : f.Instrument.t1 ,
                    f.CableType,
                    len,
                    f.LstSegment.Count.ToString(),
                    f.To
                            }
                            );
                }
                // For Duplicate Routes
                var groupby = dtIns.AsEnumerable().GroupBy(row => new
                {
                    asigt = row.Field<string>("A_SignalType"),
                    asyst = row.Field<string>("A_SystemType"),

                    ism = row.Field<string>("Instrument"),
                    tye = row.Field<string>("Type"),
                    To = row.Field<string>("To"),
                    //Len = row.Field<string>("TotalSegment")
                }).Select(grp => new
                {
                    A_SignalType = grp.Key.asigt,
                    A_SystemType = grp.Key.asyst,

                    Instrument = grp.Key.ism,
                    CableType = grp.Key.tye,
                    To = grp.Key.To,
                    Length = grp.Min(s => s.Field<double>("Length")),
                    Count = grp.Count()
                }).Where(
                 c => c.Count > 1
                 ).ToList();


                foreach (var g in groupby)
                {
                    var dr = dtIns.Select(" A_SignalType = '" + g.A_SignalType + "' and  A_SystemType = '" + g.A_SystemType + "' and Instrument = '" + g.Instrument + "' and Type = '" + g.CableType + "' and To = '" + g.To + "' and Length <> '" + g.Length + "' ");
                    if (dr.FirstOrDefault() == null)
                    {
                        dr = dtIns.Select("  A_SignalType = '" + g.A_SignalType + "' and  A_SystemType = '" + g.A_SystemType + "' and  Instrument = '" + g.Instrument + "' and Type = '" + g.CableType + "' and To = '" + g.To + "' and Length = '" + g.Length + "' ");
                    }
                    var drRow = dr.FirstOrDefault();
                    dtIns.Rows.Remove(drRow);

                    var selectedRoute = routeInfo.LstInsRouteInfo.Where(x => x.SignalType == g.A_SignalType && x.SystemType == g.A_SystemType && (!String.IsNullOrEmpty(x.Instrument.t2) ? (x.Instrument.t1 + "_" + x.Instrument.t2) : x.Instrument.t1) == g.Instrument && x.CableType == g.CableType && x.To == g.To && x.Length != g.Length);
                    if (selectedRoute.FirstOrDefault() == null)
                    {
                        selectedRoute = routeInfo.LstInsRouteInfo.Where(x => x.SignalType == g.A_SignalType && x.SystemType == g.A_SystemType && (!String.IsNullOrEmpty(x.Instrument.t2) ? (x.Instrument.t1 + "_" + x.Instrument.t2) : x.Instrument.t1) == g.Instrument && x.CableType == g.CableType && x.To == g.To && x.Length == g.Length);
                    }
                    var selectedRouteIns = selectedRoute.FirstOrDefault();
                    routeInfo.LstInsRouteInfo.Remove(selectedRouteIns);
                }
                dtIns.AcceptChanges();


                foreach (DataRow dr in dtIns.Rows)
                {
                    dr["Length"] = Math.Round(Convert.ToDouble(dr["Length"].ToString()) / 1000, 2);
                }
                dgv_InsAnalysis.EndEdit();
                dgv_InsAnalysis.Update();
                if (dgv_InsAnalysis.InvokeRequired)
                {
                    Action safeWrite = delegate { dgv_InsAnalysis.DataSource = dtIns; };
                    dgv_InsAnalysis.Invoke(safeWrite);
                }
                else
                    dgv_InsAnalysis.DataSource = dtIns;









                foreach (var f in routeInfo.LstSegmentInfo)
                {
                    f.CableList = new List<string>();
                    foreach (var d in routeInfo.LstInsRouteInfo)
                    {
                        var sgPersist = d.LstSegment.Where(x => x.Split(',').Last() == f.SegmentName && x.Split(',').First() == f.SignalType);
                        if (sgPersist.Any())
                        {
                            if (d.Instrument != null)
                                f.CableList.Add((!string.IsNullOrEmpty(d.Instrument.t2) ? (d.Instrument.t1 + "_" + d.Instrument.t2) : (d.Instrument.t1)) + "_" + d.SystemType + "_" + d.SignalType + "_" + d.CableType);
                        }
                    }
                }

                DataTable dtSeg = new DataTable();

                dtSeg.Columns.Add("A_SegSignalType");

                dtSeg.Columns.Add("Segment");
                dtSeg.Columns.Add("TotalCable");
                dtSeg.Columns.Add("Length");

                //dtSeg.Columns.Add("A_OptimalSize");

                dtSeg.Columns.Add("A_TotalArea");
                dtSeg.Columns.Add("A_OptimalSize"); //Name
                dtSeg.Columns.Add("A_ActualOptimalResult");

                dtSeg.Columns.Add("A_OptimalRatio");
                dtSeg.Columns.Add("A_UserDefinedSize");
                dtSeg.Columns.Add("A_UserDefinedRato");


                foreach (var f in routeInfo.LstSegmentInfo)
                {
                    if (string.IsNullOrEmpty(f.SignalType)) continue;
                    var optimalArea = 0.0;
                    foreach (string cbl in f.CableList)
                    {
                        var cable = cbl.Split('_').Last();
                        var selectedCables = lstCable.Where(x => x.Title == cable);
                        if (selectedCables.Any())
                        {
                            try
                            {
                                var Dia = Convert.ToDouble(selectedCables.FirstOrDefault().Diameter);
                                var area = Dia * Dia;
                                optimalArea += area;
                            }
                            catch
                            {
                                MessageBox.Show("케이블 지름 값을 확인해주세요." + cable + ".");
                            }
                        }
                        else
                        {
                            var msg = MessageBox.Show("다음 케이블의 데이터가 확인되지 않습니다. -'" + cable + "'. 케이블 지름 값을 확인해주세요'" + cable + "'. 남은 케이블로 계속하시겠습니까? 혹은 데이터를 수정하시겠습니까??", "Information!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (msg == DialogResult.Yes)
                            {
                                optimalArea += 0;
                            }
                            else
                                this.Close();
                        }
                    }
                    if (string.IsNullOrEmpty(txtDuctOptimize.Text))
                    {
                        MessageBox.Show("케이블 덕트 사용률을 지정해주세요. 20%가 초기 값으로 설정되어있습니다.");
                        txtDuctOptimize.Text = "20";
                    }
                    var OptimalDuct = optimalArea / (Convert.ToDouble(txtDuctOptimize.Text) / 100);//txtDuctOptimize
                    var res = TileDuctlist.Where(x => x.Type == f.SignalType.ToString());

                    var r1 = new CableDuctList();
                    if (res.Any())
                    {
                        r1 = (from n in res.ToList() where (n.Width * n.Height) >= OptimalDuct orderby (n.Width * n.Height) select n).FirstOrDefault();
                    }
                    else
                    {
                        r1 = (from n in TileDuctlist where (n.Width * n.Height) >= OptimalDuct orderby (n.Width * n.Height) select n).FirstOrDefault();
                    }
                    var rtype = "";
                    var rratio = 0.0; // Math.Round((optimalArea / (r1.Width * r1.Height)) * 100, 2)  ;
                    rtype = r1 == null ? "" : r1.Title;//r1.Type; PTK changed due to not adaption of Saw lay 
                    rratio = r1 == null ? 0 : Math.Round((optimalArea / (r1.Width * r1.Height)) * 100, 2);
                    if (!chk_EmtpyDucts.Checked)
                        if (f.CableList.Count.ToString() == "0") continue;  // PTK Commmected for Allowing empty cables
                    dtSeg.Rows.Add(
             new object[] {
             f.SignalType,
                   f.SegmentName,
                   f.CableList.Count.ToString(),
                   f.Length,

                   optimalArea,
                   rtype ,//r1.Type,
                   OptimalDuct,
                   rratio,
                rtype , //  isSystemFile? f.A_UserDefinedSize : "",
                 rratio // isSystemFile? f.A_UserDefinedRato : 0

             }
             );

                    //  f.A_UserDefinedRato = 
                }
                foreach (DataRow dr in dtSeg.Rows)
                {

                    dr["Length"] = Math.Round(Convert.ToDouble(dr["Length"].ToString()) / 1000, 2);
                    //dr["A_UserDefinedSize"]= 
                    //dr["A_UserDefinedSize"] =
                    //OptimalSize
                    //dr["OptimalSize"] = Math.Round(Convert.ToDouble(dr["OptimalSize"].ToString()) / 1000, 10)*1000000;//OptimalSize
                }


                dgv_SegAnalysis.EndEdit();
                dgv_SegAnalysis.Update();
                //dgv_SegAnalysis.DataSource = null;
                try
                {
                    if (dgv_SegAnalysis.InvokeRequired)
                    {
                        Action safeWrite = delegate { dgv_SegAnalysis.DataSource = dtSeg; };
                        try
                        {
                            dgv_SegAnalysis.Invoke(safeWrite);
                        }
                        catch
                        {
                            dgv_SegAnalysis.DataSource = dtSeg;
                        }
                    }
                    else
                        dgv_SegAnalysis.DataSource = dtSeg;
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + dtSeg.Rows.Count.ToString());
                }

                Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                foreach (DataRow dr in dtSeg.Rows)
                {
                    DataTable type = new DataTable();
                    type.Columns.Add("A_UserDefinedSize");
                    // BL.SettingBL sbl = new BL.SettingBL();
                    var res = TileDuctlist.Where(x => x.Type == dr["A_SegSignalType"].ToString());
                    if (res.Any())
                    {
                        foreach (var t in res.ToList())
                            type.Rows.Add(t.Title);
                    }
                    else
                    {
                        foreach (var t in TileDuctlist)
                            type.Rows.Add(t.Title);
                    }

                    dic.Add(dr["A_SegSignalType"].ToString() + "_" + dr["Segment"].ToString(), type);
                }

                foreach (DataGridViewRow dr in dgv_SegAnalysis.Rows)
                {
                    var Type = dic.Where(x => x.Key.ToString() == dr.Cells["A_SegSignalType"].Value.ToString() + "_" + dr.Cells["A_SegName"].Value.ToString()).FirstOrDefault().Value;
                    DataGridViewComboBoxCell cbCell = (DataGridViewComboBoxCell)dgv_SegAnalysis.Rows[dr.Index].Cells["A_UserDefinedSize"];

                    cbCell.ValueMember = "A_UserDefinedSize";

                    cbCell.DisplayMember = "A_UserDefinedSize";
                    cbCell.DataSource = Type;
                }

                dgv_SegAnalysis.Refresh();
                dgv_SegAnalysis.RefreshEdit();
                dgv_SegAnalysis.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            try
            {

                BindFinalSetting();
                chk_SignalSegment.Checked = false;
            }
            catch
            {

            }
        }
        List<vdLine> LstDuctLinked = new List<vdLine>();
        List<vdLine> LstDuctLinkedtemp = new List<vdLine>();
        List<InstrumentRouteInfoEntity> lstRouteInfoTemp = null;
        DataTable CheckdataTable = new DataTable();
        private List<vdLine> RemoveExtraLeadingLine(gPoint iie, List<vdLine> lines, Instrument ins, bool IsDestination = false)
        {
            if (IsDestination)
            {
                //var IsContain_UnitGRid = lines.Where(x => Convert.ToInt32(x.Length()) == unitGrid).Any();
                //if (IsContain_UnitGRid)
                //{
                return lines;
                //}
            }
            //2023-09-16  Added this code for finding error about 1m length containing length cant involve in analysis. It is (1m) lenght is always left in this function //PTK
            List<vdLine> vdLines = new List<vdLine>();
            ////var sp = new gPoint();
            ////var ep = new gPoint();
            int i = 0;
            foreach (var l in lines)
            {
                if (i + 1 == lines.Count())
                {
                    vdLines.Add(l);
                    break;
                }

                var ep = l.EndPoint;
                var sp = lines[i + 1].StartPoint;
                var inRange = false;
                if (iie.Distance2D(sp) <= (double)unit)
                {
                    inRange = true;
                }
                if (sp == ep && inRange)
                {
                    i++;
                    continue;
                }
                sp = lines[i + 1].EndPoint;
                inRange = false;
                if (iie.Distance2D(sp) <= (double)unit)
                {
                    inRange = true;
                }
                if (sp == ep && inRange)
                {
                    i++;
                    continue;
                }



                ep = l.StartPoint;
                sp = lines[i + 1].StartPoint;
                inRange = false;
                if (iie.Distance2D(sp) <= (double)unit)
                {
                    inRange = true;
                }
                if (sp == ep && inRange)
                {
                    i++;
                    continue;
                }
                sp = lines[i + 1].EndPoint;
                inRange = false;
                if (iie.Distance2D(sp) <= (double)unit)
                {
                    inRange = true;
                }
                if (sp == ep && inRange)
                {
                    i++;
                    continue;
                }


                vdLines.Add(l);
                i++;
            }

            var ro = ins.PosssessRoute;

            if (!vdLines.Where(l => l == ro).Any())
            {
                ins.PosssessRoute = vdLines[0];
            }

            return vdLines;
        }
        private void RouteSetting()
        {
            if (routeInfo.DGV_LstInstrument == null) return;
            CheckdataTable = new DataTable();
            CheckdataTable.Columns.Add("SG");
            CheckdataTable.Columns.Add("SP");
            CheckdataTable.Columns.Add("EP");
            CheckdataTable.Columns.Add("ID");
            CheckdataTable.Columns.Add("TB");
            CheckdataTable.Columns.Add("Sginal");
            foreach (var l in routeInfo.LstSegmentInfo)
            {
                CheckdataTable.Rows.Add(
                new object[] {
                    l.SegmentName,
                    l.StartPoint,
                    l.EndPoint,
                    l.ParentRouteID,
                    l.ParentDestination,
                    l.SignalType
                }
                );
            }
            lstRouteInfoTemp = new List<InstrumentRouteInfoEntity>();
            //var ins = routeInfo.allInstruments;
            var sbl = new RouteOptimizer.BL.SettingBL();
            var lstInscable = sbl.GetInsCable();
            // var lstSignal = sbl.GetSignalType(); 
            var LstInsType = sbl.GetInstrumetnType();
            var sigDuct = sbl.GetSignalType();
            routeInfo.SetvaluesToNullCablesFromMultiCheck();
            foreach (var inst in routeInfo.DGV_LstInstrument)
            {
                var selectedIns = routeInfo.LstallInstruments.Where(x => x.t1 == inst.Instrument.t1 && x.t2 == inst.Instrument.t2);//x => x.t1 == inst.Instrument.t1 && x.t2 == inst.Instrument.t2

                if (!selectedIns.Any()) continue;

                foreach (var ins in inst.LstInstCableEntity)
                {
                    if (String.IsNullOrEmpty(ins.col_Signal)) continue;
                    if (!sigDuct.Where(x => x.Title == ins.System).Any())
                    {
                        MessageBox.Show("The system need to make a setting in Signal Form. It might be there is no respective assigned duct or no system type setting in Signal list. Please check in SignalList form.");
                        return;
                    }
                    else
                    {
                        ins.col_Signal = sigDuct.Where(x => x.Title == ins.System).FirstOrDefault().AssignedCableDuct;//
                    }
                }
            }

            foreach (var lr in routeInfo.DGV_LstInstrument)
            {
                var selectedIns = routeInfo.LstallInstruments.Where(x => x.t1 == lr.Instrument.t1 && x.t2 == lr.Instrument.t2);/// x => x.t1 == inst.Instrument.t1 && x.t2 == inst.Instrument.t2
                if (!selectedIns.Any()) continue;

                var cableIns = lr.LstInstCableEntity;


                foreach (var ins in cableIns)
                {
                    LstDuctLinked = new List<vdLine>();
                    if (!string.IsNullOrEmpty(ins.To))
                    {
                        if (routeInfo.LstTBBOXes.Where(x => x.guid.ToString() == ins.To).FirstOrDefault() == null)
                            continue;
                        var selectedTBDestination = selectedIns.FirstOrDefault().PosssessDestination;
                        if (selectedTBDestination != null && routeInfo.LstTBBOXes.Where(x => x.guid.ToString() == ins.To).FirstOrDefault().Name == selectedTBDestination.Name) // target == parent
                        {
                            var selectedTB = selectedTBDestination.Name;
                            var startpoint = selectedIns.FirstOrDefault().PossessPoint;

                            // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint);
                            List<string> subSeg = new List<string>();
                            double len = 0.0;
                            while (routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == selectedTB && x.SignalType == ins.col_Signal).Any())
                            {
                                var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == selectedTB && x.SignalType == ins.col_Signal);
                                len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                startpoint = selectSeg.FirstOrDefault().EndPoint;
                            }
                            InstrumentRouteInfoEntity ire = new InstrumentRouteInfoEntity()
                            {
                                SignalType = ins.col_Signal,
                                SystemType = ins.System,

                                Instrument = selectedIns.FirstOrDefault(),
                                CableType = ins.Cable,
                                Length = len,
                                LstSegment = subSeg,
                                To = routeInfo.LstTBBOXes.Where(x => x.guid.ToString() == ins.To).FirstOrDefault().Name

                            };

                            lstRouteInfoTemp.Add(ire);
                            // routeInfo.LstInsRouteInfo.Add(ire);

                        } // Check all by recursive function
                        else
                        {
                            var refPoly = selectedIns.FirstOrDefault().PosssessRoute;
                            LstDuctLinked.Add(refPoly);
                            LstDuctLinkedtemp = new List<vdLine>();
                            //refP = new List<vdPolyline>();
                            //RetFlg = false;
                            LstDuctLinkedtemp.Add(LstDuctLinked[0]);
                            LstAllPath = new List<List<vdLine>>() { };
                            Recursive_DFS(selectedIns, routeInfo.FindIO_TBBox(Convert.ToInt32(ins.To)).Name, selectedIns.FirstOrDefault().PosssessDestination, refPoly);
                            //Recursive(selectedIns, routeInfo.LstTBBOXes.Where(x => x.guid.ToString() == ins.To).FirstOrDefault().Name, selectedIns.FirstOrDefault().PosssessDestination, refPoly);
                            //continue;


                            foreach (var lsp in LstAllPath)
                            {
                                LstDuctLinkedtemp = lsp;

                                var e_tri = lsp.GroupBy(w => w.getEndPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                var s_tri = lsp.GroupBy(w => w.getStartPoint()).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                List<gPoint> lstCheckDuplicate = new List<gPoint>();
                                foreach (var l in lsp)
                                {
                                    lstCheckDuplicate.Add(l.StartPoint);
                                    lstCheckDuplicate.Add(l.EndPoint);
                                }
                                var duplicatecheck = lstCheckDuplicate.GroupBy(w => w).Where(g => g.Count() > 2).Select(y => new { Name = y.Key, Count = y.Count() });
                                if (e_tri.Any() || s_tri.Any() || duplicatecheck.Any())
                                {
                                    continue;
                                }
                                LstDuctLinkedtemp = RemoveExtraLeadingLine(lr.Instrument.centerPoint, lsp, selectedIns.FirstOrDefault());  // removing leading lines (eg actual is 2,3  but in program 10,2,3    10 and 2 have same zipzap condition )
                                                                                                                                           // foreach (   ) selectedIns.FirstOrDefault().PosssessRoute

                                InstrumentRouteInfoEntity ire = null;

                                if (LstDuctLinkedtemp.Count > 1)
                                {
                                    // if ((LstDuctLinkedtemp[0] as vdLine).Length() == unitGrid) continue;
                                    double len = 0.0;
                                    List<string> subSeg = new List<string>();
                                    List<vdPolyline> UsedLine = new List<vdPolyline>() { };


                                    //Check EndPointisDirectOnCurve
                                    var LstDuctLinkedtempRes = new List<vdPolyline>() { };
                                    var refRoute = selectedIns.FirstOrDefault().PosssessRoute;
                                    //foreach (var g in LstDuctLinkedtemp)  //Work when Duplicate
                                    //{
                                    //    if (g.Id != refRoute.Id)
                                    //    {
                                    //        if (routeInfo.LstSegmentInfo.Where(x => x.ParentRouteID == g.Id).FirstOrDefault().ParentDestination.Name == routeInfo.LstTBBOXes.Where(x => x.guid.ToString() == ins.To).FirstOrDefault().Name)
                                    //        {
                                    //            if (g.PointOnCurve(refRoute.getEndPoint(), true) || refRoute.PointOnCurve(g.getEndPoint(), true))
                                    //            {
                                    //                LstDuctLinkedtempRes.Add(g);
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //if (LstDuctLinkedtempRes.Count == 1)
                                    //{
                                    //    LstDuctLinkedtempRes.Add(refRoute);
                                    //    LstDuctLinkedtemp = LstDuctLinkedtempRes;
                                    //}
                                    //UsedLine.Add(selectedIns.FirstOrDefault().PosssessRoute);
                                    var siLine = selectedIns.FirstOrDefault().PosssessRoute;
                                    gPoint FinalInsterSection = null;
                                    foreach (var cline in LstDuctLinkedtemp)
                                    {
                                        //if (!UsedLine.Where(x => x.Id == cline.Id).Any())
                                        //{
                                        if (siLine.Id == cline.Id) continue;
                                        gPoints rp1 = new gPoints();
                                        var res1 = siLine.IntersectWith(cline, VdConstInters.VdIntOnBothOperands, rp1);
                                        if (res1)
                                        {



                                            bool IsReachIntersection = false;
                                            if (FinalInsterSection == null) FinalInsterSection = selectedIns.FirstOrDefault().PossessPoint;

                                            bool IsDecimalIssue = false;
                                            try
                                            {
                                                var r = siLine.getDistAtPoint(FinalInsterSection);// new gPoint(Math.Round(FinalInsterSection.x,5), Math.Round(FinalInsterSection.y, 5)) 
                                            }
                                            catch
                                            {
                                                IsDecimalIssue = true;
                                                // continue;
                                            }

                                            if (IsDecimalIssue)
                                            {
                                                var fis = selectedIns.FirstOrDefault().PossessPoint.Distance2D(siLine.StartPoint);
                                                var sec = selectedIns.FirstOrDefault().PossessPoint.Distance2D(siLine.EndPoint);
                                                FinalInsterSection = siLine.StartPoint;
                                                if (fis > sec)
                                                    FinalInsterSection = siLine.EndPoint;
                                                //FinalInsterSection = new gPoint(Math.Round(FinalInsterSection.x, 5), Math.Round(FinalInsterSection.y, 5));
                                            }
                                            if (siLine.getDistAtPoint(rp1[0]) > siLine.getDistAtPoint(FinalInsterSection))
                                            //if (siLine.getDistAtPoint(   rp1[0]) > siLine.getDistAtPoint(FinalInsterSection))
                                            {
                                                var EndPoint = FinalInsterSection; //selectedIns.FirstOrDefault().PossessPoint;
                                                                                   // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                var res_a = routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.EndPoint.x, 5) == Math.Round(EndPoint.x, 5) && Math.Round(x.EndPoint.y, 5) == Math.Round(EndPoint.y, 5))

               && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                if (res_a.Any())
                                                {

                                                    while (routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.EndPoint.x, 5) == Math.Round(EndPoint.x, 5) && Math.Round(x.EndPoint.y, 5) == Math.Round(EndPoint.y, 5))

                                                                                              && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                    {
                                                        if (IsReachIntersection) break;
                                                        var selectSeg = routeInfo.LstSegmentInfo.Where(x =>// x.EndPoint == EndPoint 
                                                          (Math.Round(x.EndPoint.x, 5) == Math.Round(EndPoint.x, 5) && Math.Round(x.EndPoint.y, 5) == Math.Round(EndPoint.y, 5))
                                                        && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                        if (selectSeg.FirstOrDefault().StartPoint == rp1[0])
                                                        {

                                                            IsReachIntersection = true;
                                                            FinalInsterSection = rp1[0];
                                                        }
                                                        else
                                                        {
                                                            FinalInsterSection = selectSeg.FirstOrDefault().StartPoint;
                                                        }
                                                        len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                                        subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                        EndPoint = selectSeg.FirstOrDefault().StartPoint;

                                                    }
                                                }

                                            }
                                            else
                                            {
                                                var Startpoint = FinalInsterSection; ///selectedIns.FirstOrDefault().PossessPoint;
                                                // var selectSeg = routeInfo.LstSegmentInfo.Where(x => x.EndPoint == EndPoint);
                                                while (routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.StartPoint.x, 5) == Math.Round(Startpoint.x, 5) && Math.Round(x.StartPoint.y, 5) == Math.Round(Startpoint.y, 5)) && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal).Any())
                                                {
                                                    if (IsReachIntersection) break;
                                                    var selectSeg = routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.StartPoint.x, 5) == Math.Round(Startpoint.x, 5) && Math.Round(x.StartPoint.y, 5) == Math.Round(Startpoint.y, 5)) && x.ParentRouteID == siLine.Id && x.SignalType == ins.col_Signal);
                                                    if (selectSeg.FirstOrDefault().EndPoint == rp1[0])
                                                    {
                                                        IsReachIntersection = true;
                                                        FinalInsterSection = rp1[0];
                                                    }
                                                    else
                                                    {
                                                        FinalInsterSection = selectSeg.FirstOrDefault().EndPoint;
                                                    }
                                                    len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                                    subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                                    Startpoint = selectSeg.FirstOrDefault().EndPoint;
                                                }
                                            }
                                            //FinalInsterSection = rp1[0];
                                        }
                                        siLine = cline;
                                        //  UsedLine.Add(siLine);
                                        //}
                                    }
                                    try
                                    {
                                        var startpoint = FinalInsterSection;// routeInfo.LstTBBOXes.Where(y => y.guid.ToString() == ins.To).FirstOrDefault().Name 
                                                                            //   var resDe = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == startpoint && x.ParentDestination.Name == routeInfo.LstTBBOXes.Where(y => y.guid.ToString() == ins.To).FirstOrDefault().Name);
                                        while (routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.StartPoint.x, 5) == Math.Round(startpoint.x, 5) && Math.Round(x.StartPoint.y, 5) == Math.Round(startpoint.y, 5)) && x.ParentDestination.Name == routeInfo.FindIO_TBBox(Convert.ToInt32(ins.To)).Name && x.SignalType == ins.col_Signal).Any())
                                        {
                                            // var selectSeg = resDe;
                                            var selectSeg = routeInfo.LstSegmentInfo.Where(x => (Math.Round(x.StartPoint.x, 5) == Math.Round(startpoint.x, 5) && Math.Round(x.StartPoint.y, 5) == Math.Round(startpoint.y, 5)) && x.ParentDestination.Name == routeInfo.FindIO_TBBox(Convert.ToInt32(ins.To)).Name && x.SignalType == ins.col_Signal);

                                            len += selectSeg.FirstOrDefault().StartPoint.Distance2D(selectSeg.FirstOrDefault().EndPoint);
                                            subSeg.Add(selectSeg.FirstOrDefault().SignalType + "," + selectSeg.FirstOrDefault().SegmentName);
                                            startpoint = selectSeg.FirstOrDefault().EndPoint;
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = len,
                                        LstSegment = subSeg,
                                        To = routeInfo.FindIO_TBBox(Convert.ToInt32(ins.To)).Name
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    // routeInfo.LstInsRouteInfo.Add(ire);
                                }
                                else
                                {
                                    ire = new InstrumentRouteInfoEntity()
                                    {
                                        SignalType = ins.col_Signal,
                                        SystemType = ins.System,

                                        Instrument = selectedIns.FirstOrDefault(),
                                        CableType = ins.Cable,
                                        Length = 0.0,
                                        LstSegment = new List<string>() { },
                                        To = routeInfo.FindIO_TBBox(Convert.ToInt32(ins.To)).Name
                                    };
                                    lstRouteInfoTemp.Add(ire);
                                    // routeInfo.LstInsRouteInfo.Add(ire);
                                }
                            }
                        }
                    }
                }
            }

            var GetShortestPath = from c in lstRouteInfoTemp
                                  group c by new
                                  {
                                      c.SignalType,
                                      c.SystemType,

                                      c.Instrument.t1,
                                      c.Instrument.t2,
                                      c.CableType,
                                      c.To
                                  } into gcs
                                  select new InstrumentRouteInfoEntity()
                                  {
                                      SignalType = gcs.Key.SignalType,
                                      SystemType = gcs.Key.SystemType,

                                      Instrument = gcs.FirstOrDefault().Instrument,
                                      CableType = gcs.Key.CableType,
                                      To = gcs.Key.To,
                                      Length = gcs.Min(c => c.Length),
                                      LstSegment = gcs.Where(c => c.Length == (gcs.Min(c1 => c1.Length))).FirstOrDefault().LstSegment,
                                  };
            foreach (var pi in GetShortestPath)
                routeInfo.LstInsRouteInfo.Add(pi);

        }
        bool RetFlg = false;
        List<List<vdLine>> LstAllPath = null;
        private void Recursive_DFS(IEnumerable<Instrument> selectedIns, string TargetDB, TBBOXDestination CurrentDB, vdLine refPoly)
        {
            //if (RetFlg) return;
            foreach (var sp1 in routeInfo.LstAllRouteDrawn)
            {
                if ((LstDuctLinkedtemp.Where(x => x == sp1).Any()) || (refPoly == sp1))
                    continue;
                //else
                //    LstDuctLinkedtemp.Add(sp1);
                var gp1 = new gPoints();
                var res1 = refPoly.IntersectWith(sp1, VdConstInters.VdIntOnBothOperands, gp1);
                if (res1)
                {

                    if (gp1.Count > 1) continue;
                    bool IsPointInside = false;
                    foreach (var t in routeInfo.LstTBBOXes)
                    {
                        if (t.polyline.BoundingBox.PointInBox(gp1[0]))
                        {
                            IsPointInside = true;
                            break;
                        }
                    }
                    if (IsPointInside) continue;
                    LstDuctLinkedtemp.Add(sp1);

                    var tbName = routeInfo.LstTBBOXes.Where(x => x.MainRouteCollection.Where(c => c == sp1).Any());
                    if (tbName.Any() && tbName.FirstOrDefault() != null)// || (tbName.FirstOrDefault() != null && tbName.FirstOrDefault().polyline.BoundingBox.PointInBox(sp1.getEndPoint())))
                    {
                        var tb = tbName.FirstOrDefault();
                        if ((tb != null && tb.Name == TargetDB))// || (tbName.FirstOrDefault() != null && tbName.FirstOrDefault().polyline.BoundingBox.PointInBox(sp1.getEndPoint())))) // lnked, right tb, Found Linked at First TB  //|| (tbName.FirstOrDefault() != null && tbName.FirstOrDefault().polyline.BoundingBox.PointInBox(sp1.getEndPoint()))
                        {
                            List<vdLine> temp = new List<vdLine>() { };
                            foreach (var l in LstDuctLinkedtemp) temp.Add(l);
                            LstAllPath.Add(temp);
                            LstDuctLinkedtemp.RemoveAt(LstDuctLinkedtemp.Count - 1);
                            continue;
                        }
                        else // first inter linked Loop
                        {
                            Recursive_DFS(selectedIns, TargetDB, tb, sp1);
                        }
                    }
                    else if (routeInfo.LstExternalRoute.Where(x => x == sp1).Any())
                    {
                        Recursive_DFS(selectedIns, TargetDB, null, sp1);
                    }
                    LstDuctLinkedtemp.Remove(sp1);
                }
            }
        }
        private void Recursive(IEnumerable<Instrument> selectedIns, string TargetDB, TBBOXDestination CurrentDB, vdLine refPoly)
        {
            if (RetFlg) return;
            foreach (var tb in routeInfo.LstTBBOXes)
            {
                if (tb.Name != selectedIns.FirstOrDefault().PosssessDestination.Name && tb != CurrentDB)
                {
                    foreach (var sp1 in tb.MainRouteCollection)
                    {
                        if (LstDuctLinkedtemp.Where(x => x.Id == sp1.Id).Any()) continue;
                        var gp1 = new gPoints();
                        var res1 = refPoly.IntersectWith(sp1, VdConstInters.VdIntOnBothOperands, gp1);
                        if (res1)
                        {
                            if (tb.Name == TargetDB) // lnked, right tb, Found Linked at First TB
                            {
                                LstDuctLinkedtemp.Add(sp1);
                                RetFlg = true;
                                return;
                            }
                            else // first inter linked Loop
                            {
                                RetFlg = false;
                                //refP.Add(sp1);
                                LstDuctLinkedtemp.Add(sp1);
                                Recursive(selectedIns, TargetDB, tb, sp1);
                                if (!RetFlg)
                                    LstDuctLinkedtemp.Remove(sp1);


                            }
                        }
                    }
                }
            }
        }
        private void LabelSegementSetting()
        {
            var types = (new BL.SettingBL()).GetCableDuctTypeList();//new string[] { "Signal", "Power", "SMCS", "Communication", "Destination" }; // get from List

            foreach (var vdCir in routeInfo.LstTextName_Cricle_Cache) vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.RemoveItem(vdCir);
            routeInfo.LstTextName_Cricle_Cache = new List<vdFigure>();
            string sgAbbre = routeInfo.SEGMENTABBRE;
            int Seg = 0;
            foreach (var tb in routeInfo.LstTBBOXes)
            {
                foreach (var pl in tb.MainRouteCollection)
                {
                    gPoints Respoint = new gPoints();
                    gPoints IpPoints = new gPoints();


                    foreach (var tb2 in routeInfo.LstTBBOXes)  // Add intersection
                    {
                        if (tb.Name == tb2.Name) continue;

                        foreach (var pl2 in tb2.MainRouteCollection)
                        {
                            gPoints testGp = new gPoints();
                            var resSeg = pl.IntersectWith(pl2, VdConstInters.VdIntOnBothOperands, testGp);
                            if (resSeg)
                            {
                                IpPoints.Add(testGp[0]);
                            }
                        }
                    }


                    foreach (var lineExternal in routeInfo.LstExternalRoute) // from External Route
                    {
                        gPoints testGp = new gPoints();
                        var resSeg = pl.IntersectWith(lineExternal, VdConstInters.VdIntOnBothOperands, testGp);
                        if (resSeg)
                        {
                            foreach (var g in testGp)
                            {
                                IpPoints.Add(g);
                            }
                            //   IpPoints.Add(testGp[0]);
                        }
                    }

                    var InsLocation = routeInfo.AllRouteCollections.Where(x => x.Key == pl);  //   intersection point +   instrument location point added 
                    if (InsLocation.Any())
                    {
                        foreach (gPoint r in InsLocation.FirstOrDefault().Value)
                        {
                            if (pl.PointOnCurve(r, true))
                            {
                                IpPoints.Add(r);
                                vdCircle vdCircle = new vdCircle(vdFC1.BaseControl.ActiveDocument, r, 100);
                                vdCircle.PenColor = new vdColor(Color.Cyan);
                                //vdCircle.URL = "circleReference.wyzrs";
                                vdCircle.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);
                                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(vdCircle);
                                vdFC1.BaseControl.Redraw();
                                routeInfo.LstTextName_Cricle_Cache.Add(vdCircle);
                            }
                        }
                    }

                    foreach (gPoint p in IpPoints)
                    {
                        if (Math.Round(p.x) == Math.Round(pl.getEndPoint().x) && Math.Round(p.y) == Math.Round(pl.getEndPoint().y))
                        {
                            //
                        }
                        else
                        {
                            Respoint.Add(p);
                        }
                    }
                    var len = new gPoint();

                    if (Respoint.Count == 0)
                    {
                        Seg++;
                        var leng = pl.Length() / 2;
                        SegmentInfoEntity se = new SegmentInfoEntity
                        {
                            SegmentName = sgAbbre + Seg.ToString(),
                            //StartPoint = pl.getStartPoint(),   // **** It was commentted when we were using with polyline, later prof's suggestion, uncommented  2023/06/19 by PTK
                            //EndPoint = pl.getEndPoint(),     // // **** It was commentted when we were using with polyline, later prof's suggestion, uncommented  2023/06/19 by PTK
                            StartPoint = pl.getEndPoint(),   // **** It was uncommented when we were using with polyline, later prof's suggestion, commented  2023/06/19 by PTK
                            EndPoint = pl.getStartPoint(), // **** It was uncommented when we were using with polyline, later prof's suggestion, commented  2023/06/19 by PTK
                            Length = pl.Length(),
                            ParentRouteID = pl.Id,
                            ParentDestination = tb
                        };
                        if (se.StartPoint == se.EndPoint)
                        {
                            Seg = Seg - 1;
                        }
                        else
                        {
                            TextInsert(pl.getPointAtDist(leng).x, pl.getPointAtDist(leng).y, sgAbbre + Seg, out int id, true, new vdColor(Color.White));
                            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                            routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id));

                            foreach (var t in types)
                            {
                                SegmentInfoEntity se1 = new SegmentInfoEntity
                                {
                                    SegmentName = sgAbbre + Seg.ToString(),
                                    //StartPoint = pl.getStartPoint(),   // ****
                                    //EndPoint = pl.getEndPoint(),     // // ***
                                    StartPoint = pl.getEndPoint(),   // **** It 
                                    EndPoint = pl.getStartPoint(), // **** It wa
                                    Length = pl.Length(),
                                    ParentRouteID = pl.Id,
                                    ParentDestination = tb,
                                    SignalType = t.Type
                                };
                                routeInfo.LstSegmentInfo.Add(se1);
                            }
                        }

                    }
                    else
                    {
                        Dictionary<double, gPoint> Reskp = new Dictionary<double, gPoint>() { };
                        foreach (gPoint gp in Respoint)
                        {
                            try
                            {

                                //Need to check at same points 
                                if (!Reskp.Where(x => x.Value == gp).Any())
                                    Reskp.Add(pl.getDistAtPoint(gp), gp);
                            }
                            catch { }
                        }
                        Reskp.OrderByDescending(o => o.Key);

                        var startPoint = pl.getEndPoint();
                        var EndPoint = new gPoint();
                        gPoints gpt = null;

                        //Segs Before last Seg
                        foreach (var val in Reskp.OrderByDescending(o => o.Key))
                        {
                            Seg++;
                            EndPoint = val.Value;
                            gpt = new gPoints() { startPoint, EndPoint };
                            var leng_ = (pl.getDistAtPoint(startPoint) + pl.getDistAtPoint(EndPoint)) / 2;
                            len = pl.getPointAtDist(leng_);

                            SegmentInfoEntity se11 = new SegmentInfoEntity
                            {
                                SegmentName = sgAbbre + Seg.ToString(),
                                StartPoint = startPoint,
                                EndPoint = EndPoint,
                                Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                                ParentRouteID = pl.Id,
                                ParentDestination = tb
                            };

                            if (se11.StartPoint == se11.EndPoint)
                            {
                                Seg = Seg - 1;
                            }
                            else
                            {
                                TextInsert(len.x, len.y, sgAbbre + Seg, out int id, true, new vdColor(Color.White));
                                var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                                routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id));

                                foreach (var t in types)
                                {
                                    SegmentInfoEntity se1 = new SegmentInfoEntity
                                    {
                                        SegmentName = sgAbbre + Seg.ToString(),
                                        StartPoint = startPoint,
                                        EndPoint = EndPoint,
                                        Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                                        ParentRouteID = pl.Id,
                                        ParentDestination = tb,
                                        SignalType = t.Type
                                    };
                                    routeInfo.LstSegmentInfo.Add(se1);
                                }
                                //foreach (var t in types)
                                //{
                                //    se11.SignalType = t;
                                //    routeInfo.LstSegmentInfo.Add(se11);
                                //}
                                //routeInfo.LstSegmentInfo.Add(se11);
                                startPoint = EndPoint;
                            }
                            //routeInfo.LstSegmentInfo.Add(se11);
                            //startPoint = EndPoint;
                        }

                        //Last Seg
                        Seg++;
                        EndPoint = pl.getStartPoint();
                        gpt = new gPoints() { startPoint, EndPoint };
                        var leng = (pl.getDistAtPoint(startPoint) + pl.getDistAtPoint(EndPoint)) / 2;
                        len = pl.getPointAtDist(leng);

                        SegmentInfoEntity se = new SegmentInfoEntity
                        {
                            SegmentName = sgAbbre + Seg.ToString(),
                            StartPoint = startPoint,
                            EndPoint = EndPoint,
                            Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                            ParentRouteID = pl.Id,
                            ParentDestination = tb
                        };
                        if (se.StartPoint == se.EndPoint)
                        {
                            Seg = Seg - 1;
                        }
                        else
                        {
                            TextInsert(len.x, len.y, sgAbbre + Seg, out int id1, true, new vdColor(Color.White));
                            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                            routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id1));
                            foreach (var t in types)
                            {
                                SegmentInfoEntity se1 = new SegmentInfoEntity
                                {
                                    SegmentName = sgAbbre + Seg.ToString(),
                                    StartPoint = startPoint,
                                    EndPoint = EndPoint,
                                    Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                                    ParentRouteID = pl.Id,
                                    ParentDestination = tb,
                                    SignalType = t.Type
                                };
                                routeInfo.LstSegmentInfo.Add(se1);
                            }
                        }
                    }
                }
            }

            foreach (var pl in routeInfo.LstExternalRoute)
            {
                gPoints Respoint = new gPoints();
                gPoints IpPoints = new gPoints();
                TBBOXDestination tb = null;
                foreach (var lineExternal in routeInfo.LstExternalRoute) // from External Route
                {
                    if (lineExternal == pl) continue;
                    gPoints testGp = new gPoints();
                    var resSeg = pl.IntersectWith(lineExternal, VdConstInters.VdIntOnBothOperands, testGp);
                    if (resSeg)
                    {
                        foreach (var g in testGp)
                        {
                            IpPoints.Add(g);
                        }
                        //   IpPoints.Add(testGp[0]);
                    }
                }

                foreach (var tb2 in routeInfo.LstTBBOXes)  // Add intersection
                {
                    foreach (var pl2 in tb2.MainRouteCollection)
                    {
                        gPoints testGp = new gPoints();
                        var resSeg = pl.IntersectWith(pl2, VdConstInters.VdIntOnBothOperands, testGp);
                        if (resSeg)
                        {
                            foreach (var g in testGp)
                            {
                                IpPoints.Add(g);
                            }
                            //IpPoints.Add(testGp[0]);
                        }
                    }
                }

                var InsLocation = routeInfo.AllRouteCollections.Where(x => x.Key == pl);  //   intersection point +   instrument location point added 
                if (InsLocation.Any())
                {
                    foreach (gPoint r in InsLocation.FirstOrDefault().Value)
                    {
                        if (pl.PointOnCurve(r, true))
                        {
                            IpPoints.Add(r);
                            vdCircle vdCircle = new vdCircle(vdFC1.BaseControl.ActiveDocument, r, 100);
                            vdCircle.PenColor = new vdColor(Color.Cyan);
                            vdCircle.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);
                            vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(vdCircle);
                            vdFC1.BaseControl.Redraw();
                            routeInfo.LstTextName_Cricle_Cache.Add(vdCircle);
                        }
                    }
                }

                foreach (gPoint p in IpPoints)
                {
                    if (Math.Round(p.x) == Math.Round(pl.getEndPoint().x) && Math.Round(p.y) == Math.Round(pl.getEndPoint().y))
                    {
                        //
                    }
                    else
                    {
                        Respoint.Add(p);
                    }
                }
                var len = new gPoint();

                if (Respoint.Count == 0)
                {
                    Seg++;
                    var leng = pl.Length() / 2;

                    SegmentInfoEntity se = new SegmentInfoEntity
                    {
                        SegmentName = sgAbbre + Seg.ToString(),
                        //StartPoint = pl.getStartPoint(),   // **** It was commentted when we were using with polyline, later prof's suggestion, uncommented  2023/06/19 by PTK
                        //EndPoint = pl.getEndPoint(),     // // **** It was commentted when we were using with polyline, later prof's suggestion, uncommented  2023/06/19 by PTK
                        StartPoint = pl.getEndPoint(),   // **** It was uncommented when we were using with polyline, later prof's suggestion, commented  2023/06/19 by PTK
                        EndPoint = pl.getStartPoint(), // **** It was uncommented when we were using with polyline, later prof's suggestion, commented  2023/06/19 by PTK
                        Length = pl.Length(),
                        ParentRouteID = pl.Id,
                        ParentDestination = tb
                    };
                    if (se.StartPoint == se.EndPoint)
                    {
                        Seg = Seg - 1;
                    }
                    else
                    {
                        TextInsert(pl.getPointAtDist(leng).x, pl.getPointAtDist(leng).y, sgAbbre + Seg, out int id, true, new vdColor(Color.White));
                        var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                        routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id));
                        foreach (var t in types)
                        {
                            SegmentInfoEntity se1 = new SegmentInfoEntity
                            {
                                SegmentName = sgAbbre + Seg.ToString(),
                                //StartPoint = pl.getStartPoint(),   // ****
                                //EndPoint = pl.getEndPoint(),     // // ***
                                StartPoint = pl.getEndPoint(),   // **** It 
                                EndPoint = pl.getStartPoint(), // **** It wa
                                Length = pl.Length(),
                                ParentRouteID = pl.Id,
                                ParentDestination = tb,
                                SignalType = t.Type
                            };
                            routeInfo.LstSegmentInfo.Add(se1);
                        }
                        //foreach (var t in types)
                        //{
                        //    se.SignalType = t;
                        //    routeInfo.LstSegmentInfo.Add(se);
                        //}
                        // routeInfo.LstSegmentInfo.Add(se);
                    }


                }
                else
                {
                    Dictionary<double, gPoint> Reskp = new Dictionary<double, gPoint>() { };
                    foreach (gPoint gp in Respoint)
                    {
                        try
                        {

                            //Need to check at same points 
                            if (!Reskp.Where(x => x.Value == gp).Any())
                                Reskp.Add(pl.getDistAtPoint(gp), gp);
                        }
                        catch { }
                    }
                    Reskp.OrderByDescending(o => o.Key);

                    var startPoint = pl.getEndPoint();
                    var EndPoint = new gPoint();
                    gPoints gpt = null;

                    //Segs Before last Seg
                    foreach (var val in Reskp.OrderByDescending(o => o.Key))
                    {
                        Seg++;
                        EndPoint = val.Value;
                        gpt = new gPoints() { startPoint, EndPoint };
                        var leng_ = (pl.getDistAtPoint(startPoint) + pl.getDistAtPoint(EndPoint)) / 2;
                        len = pl.getPointAtDist(leng_);

                        SegmentInfoEntity se11 = new SegmentInfoEntity
                        {
                            SegmentName = sgAbbre + Seg.ToString(),
                            StartPoint = startPoint,
                            EndPoint = EndPoint,
                            Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                            ParentRouteID = pl.Id,
                            ParentDestination = tb
                        };

                        if (se11.StartPoint == se11.EndPoint)
                        {
                            Seg = Seg - 1;
                        }
                        else
                        {
                            TextInsert(len.x, len.y, sgAbbre + Seg, out int id, true, new vdColor(Color.White));
                            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                            routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id));
                            foreach (var t in types)
                            {
                                SegmentInfoEntity se1 = new SegmentInfoEntity
                                {
                                    SegmentName = sgAbbre + Seg.ToString(),
                                    StartPoint = startPoint,
                                    EndPoint = EndPoint,
                                    Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                                    ParentRouteID = pl.Id,
                                    ParentDestination = tb,
                                    SignalType = t.Type
                                };
                                routeInfo.LstSegmentInfo.Add(se1);
                            }
                            //foreach (var t in types)
                            //{
                            //    se11.SignalType = t;
                            //    routeInfo.LstSegmentInfo.Add(se11);
                            //}
                            // routeInfo.LstSegmentInfo.Add(se11);
                            startPoint = EndPoint;
                        }
                    }

                    //Last Seg
                    Seg++;
                    EndPoint = pl.getStartPoint();
                    gpt = new gPoints() { startPoint, EndPoint };
                    var leng = (pl.getDistAtPoint(startPoint) + pl.getDistAtPoint(EndPoint)) / 2;
                    len = pl.getPointAtDist(leng);

                    SegmentInfoEntity se = new SegmentInfoEntity
                    {
                        SegmentName = sgAbbre + Seg.ToString(),
                        StartPoint = startPoint,
                        EndPoint = EndPoint,
                        Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                        ParentRouteID = pl.Id,
                        ParentDestination = tb
                    };
                    if (se.StartPoint == se.EndPoint)
                    {
                        Seg = Seg - 1;
                    }
                    else
                    {
                        TextInsert(len.x, len.y, sgAbbre + Seg, out int id1, true, new vdColor(Color.White));
                        var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                        routeInfo.LstTextName_Cricle_Cache.Add(e.FindFromId(id1));
                        foreach (var t in types)
                        {
                            SegmentInfoEntity se1 = new SegmentInfoEntity
                            {
                                SegmentName = sgAbbre + Seg.ToString(),
                                StartPoint = startPoint,
                                EndPoint = EndPoint,
                                Length = (pl.getDistAtPoint(startPoint) - pl.getDistAtPoint(EndPoint)),
                                ParentRouteID = pl.Id,
                                ParentDestination = tb,
                                SignalType = t.Type
                            };
                            routeInfo.LstSegmentInfo.Add(se);
                        }
                        //foreach (var t in types)
                        //{
                        //    se.SignalType = t;
                        //    routeInfo.LstSegmentInfo.Add(se);
                        //}
                        // routeInfo.LstSegmentInfo.Add(se);
                    }

                }
            }

        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dgv_SegAnalysis_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //return;
            if (e.RowIndex > -1)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("No");
                dt.Columns.Add("Sub_InsSignalType");
                dt.Columns.Add("Sub_SystemType");
                dt.Columns.Add("Instrument");
                dt.Columns.Add("Type");
                var seg = dgv_SegAnalysis.Rows[e.RowIndex].Cells["A_SegName"].Value.ToString();
                var segSingal = dgv_SegAnalysis.Rows[e.RowIndex].Cells["A_SegSignalType"].Value.ToString();
                int i = 0;
                foreach (var f in routeInfo.LstInsRouteInfo)
                {
                    var res = f.LstSegment.Where(x => x.ToString().Split(',').Last() == seg && x.Split(',').First() == segSingal);
                    if (res.Any())
                    {
                        i++;
                        dt.Rows.Add(new object[] {
                           i.ToString(),
                           f.SignalType,
                           f.SystemType,
                       !String.IsNullOrEmpty( f.Instrument.t2)?  f.Instrument.t1 + "_" + f.Instrument.t2 : f.Instrument.t1  ,
                        f.CableType
                        });
                    }
                }
                dgv_SubSeg.DataSource = dt;

                if (dgv_SubSeg.SelectedCells == null || dgv_SubSeg.SelectedCells.Count == 0) return;
                var selectedSeg = dgv_SegAnalysis.Rows[e.RowIndex].Cells["A_SegName"].Value.ToString();
                var selectedSignalType = dgv_SegAnalysis.Rows[e.RowIndex].Cells["A_SegSignalType"].Value.ToString();
                //var selectedSignalType = dgv_SegAnalysis.Rows[e.RowIndex].Cells["A_SegSignalType"].Value.ToString();
                var val = routeInfo.LstSegmentInfo.Where(z => z.SegmentName.ToString() == selectedSeg).FirstOrDefault();

                if (rdo_defaultDuct.Checked)
                {
                    var p = new vdLine(vdFC1.BaseControl.ActiveDocument);
                    p.StartPoint = val.StartPoint;
                    p.EndPoint = val.EndPoint;
                    p.Layer = routeInfo.MainBoundary.Layer;
                    ExecuteHightlight(new List<vdLine> { p }, false);
                }
                var lstColors = KnowColors();
                if (rdo_CableDuct.Checked)
                {
                    var lines = routeInfo.lstDuctLines.Where(x => x.SegmentName == selectedSeg && x.SignalType == selectedSignalType);
                    if (lines.Any())
                    {
                        var l = lines.FirstOrDefault();
                        var p = new vdLine(vdFC1.BaseControl.ActiveDocument);
                        p.StartPoint = l.sp;
                        p.EndPoint = l.ep;
                        var pc = routeInfo.dicLinesPros.Where(x => x.Item1 == lines.FirstOrDefault().SignalType).FirstOrDefault().Item2;
                        p.PenColor.FromSystemColor(lstColors[pc]);
                        p.Layer = routeInfo.MainBoundary.Layer;
                        ExecuteHightlight(new List<vdLine> { p }, true);
                    }
                }
            }
        }
        List<vdLine> hightlightedLines = new List<vdLine>();
        private void RemoveHightlightedLines()
        {
            var en = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            foreach (var hl in hightlightedLines)
            {
                var re = en.RemoveItem(hl);
            }

            hightlightedLines.Clear();
            RefreshCADSpace();
        }
        private void ExecuteHightlight(List<vdLine> lines, bool isDuct = false)
        {
            RemoveHightlightedLines();
            var en = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            if (isDuct)
            {
                foreach (var l in lines)
                {
                    l.PenWidth = Convert.ToInt32(cbo_MaxLineWeight.Text);// l.PenWidth *3; 
                    routeInfo.CURRENT_MODE = eACTION_MODE.NONE;
                    l.PenColor = new vdColor(Color.FromName(cbo_Colorhighlight.Text));
                    en.AddItem(l);
                    hightlightedLines.Add(l);
                }
            }
            else
            {
                foreach (var l in lines)
                {
                    l.PenWidth = Convert.ToInt32(cbo_MaxLineWeight.Text); ;
                    // l.LineWeight = VdConstLineWeight.LW_200;
                    l.PenColor = new vdColor(Color.FromName(cbo_Colorhighlight.Text));  //new vdColor(Color.Cyan);
                    routeInfo.CURRENT_MODE = eACTION_MODE.NONE;
                    en.AddItem(l);
                    hightlightedLines.Add(l);
                }
            }
            RefreshCADSpace();


        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //return;
            if (e.RowIndex > -1)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("No");
                dt.Columns.Add("Sub_SegSignalType");
                dt.Columns.Add("Segment");
                dt.Columns.Add("Length");//A_SignalType
                var Ins = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_Ins"].Value.ToString();
                var A_SignalType = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_SignalType"].Value.ToString();
                var A_SystemType = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_SystemType"].Value.ToString();
                var To = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_To"].Value.ToString();
                var Type = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_Type"].Value.ToString();
                var TotalLen = dgv_InsAnalysis.Rows[e.RowIndex].Cells["A_Length"].Value.ToString();
                int i = 0;

                routeInfo.LstSelectedSegmentInfo = new List<SegmentInfoEntity>();

                routeInfo.LstSelectedSegmentInfo.Clear();
                var res = routeInfo.LstInsRouteInfo.Where(x => x.SignalType == A_SignalType && x.SystemType == A_SystemType && (!String.IsNullOrEmpty(x.Instrument.t2) ? (x.Instrument.t1 + "_" + x.Instrument.t2) : (x.Instrument.t1)) == Ins && x.CableType == Type);
                List<string> lstSeg = new List<string>();

                if (res.Any())
                {
                    lstSeg = res.FirstOrDefault().LstSegment;
                    if (res.ToList().Count > 1)
                    {

                        foreach (InstrumentRouteInfoEntity v in res.ToList())
                        {
                            var resLength = 0.0;
                            foreach (var f in v.LstSegment)
                            {
                                try
                                {
                                    resLength += routeInfo.LstSegmentInfo.Where(x => x.SegmentName == f.Split(',').Last() && x.SignalType == f.Split(',').First()).FirstOrDefault().Length; //routeInfo.LstSegmentInfo.Where(x => x.SegmentName == f).FirstOrDefault().Length;
                                }
                                catch (Exception ex)
                                {
                                    var smg = ex.Message;
                                }
                            }
                            if (Math.Round((resLength / 1000), 2).ToString() == TotalLen)
                            {

                                lstSeg = v.LstSegment;
                                break;
                            }
                        }
                    }
                    foreach (var f in lstSeg)
                    {
                        var selectedSeg = routeInfo.LstSegmentInfo.Where(x => x.SegmentName == f.Split(',').Last() && x.SignalType == f.Split(',').First()).FirstOrDefault();
                        routeInfo.LstSelectedSegmentInfo.Add(selectedSeg);
                        i++;
                        dt.Rows.Add(new object[] {
                           i.ToString(),
                           selectedSeg.SignalType,
                       f.Split(',')[1],
                        selectedSeg.Length
                        });
                    }
                }


                foreach (DataRow dr in dt.Rows)
                {
                    dr["Length"] = Math.Round(Convert.ToDouble(dr["Length"].ToString()) / 1000, 2);

                }
                dgv_SubIns.DataSource = dt;


                List<vdLine> ids = new List<vdLine>();

                var en = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                //if (rdo_defaultDuct.Checked)
                //{
                foreach (DataRow dr in dt.Rows)
                {
                    var val = routeInfo.LstSegmentInfo.Where(z => z.SegmentName.ToString() == dr["Segment"].ToString()).FirstOrDefault();//.ParentRouteID;
                    var p = new vdLine(vdFC1.BaseControl.ActiveDocument);
                    p.StartPoint = val.StartPoint;
                    p.EndPoint = val.EndPoint;
                    p.Layer = routeInfo.MainBoundary.Layer;
                    ids.Add(p);
                }
                ExecuteHightlight(ids, false);
                //}

                //var lstColors = KnowColors();
                //if (rdo_CableDuct.Checked)
                //{
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        var val = routeInfo.LstSegmentInfo.Where(z => z.SegmentName.ToString() == dr["Segment"].ToString()).FirstOrDefault();//.ParentRouteID;

                //        var lines = routeInfo.lstDuctLines.Where(x => x.SegmentName == val.SegmentName && x.SignalType == val.SignalType);
                //        if (lines.Any())
                //        {
                //            var l = lines.FirstOrDefault();
                //            var p = new vdLine(vdFC1.BaseControl.ActiveDocument);
                //            p.StartPoint = l.sp;
                //            p.EndPoint = l.ep;
                //            var pc = routeInfo.dicLinesPros.Where(x => x.Item1 == lines.FirstOrDefault().SignalType).FirstOrDefault().Item2;
                //            p.PenColor.FromSystemColor(lstColors[pc]);
                //            p.Layer = routeInfo.MainBoundary.Layer;

                //            ids.Add(p);
                //        }
                //    }
                //    ExecuteHightlight(ids, true);
                //}
                BindDecimalplace();
                vdFC1.BaseControl.Redraw();
            }
        }

        private void instListbox_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {

            List<DataGridViewRow> lstCounted = new List<DataGridViewRow>() { };
            dtSchedule = new DataTable();
            //dtSchedule.Columns.Add("No"); 

            dtSchedule.Columns.Add("SignalType");
            dtSchedule.Columns.Add("SegmentName");
            dtSchedule.Columns.Add("TotalCable");
            dtSchedule.Columns.Add("Length");

            dtSchedule.Columns.Add("A_TotalArea");
            dtSchedule.Columns.Add("A_OptimalSize");
            dtSchedule.Columns.Add("A_OptimalRatio");
            dtSchedule.Columns.Add("A_UserDefinedSize");
            dtSchedule.Columns.Add("A_UserDefinedRato");

            dtSchedule.Columns.Add("SystemType");
            dtSchedule.Columns.Add("TagName");
            dtSchedule.Columns.Add("To");
            dtSchedule.Columns.Add("Cable");

            var set = dgv_SegAnalysis.SelectedCells;
            if (set.Count == 0) return;
            foreach (DataGridViewCell c in set)
            {
                if (lstCounted.Where(x => x == c.OwningRow).Any()) continue;
                var slc = c.OwningRow;
                var A_SegSignalType = slc.Cells["A_SegSignalType"].Value.ToString();
                var seg = slc.Cells["A_SegName"].Value.ToString();
                var totalCables = slc.Cells["A_AllCable"].Value.ToString();

                if (Convert.ToInt32(totalCables) == 0) continue;
                var Length = slc.Cells["A_SLength"].Value.ToString();



                var A_TotalArea = slc.Cells["A_TotalArea"].Value.ToString();
                var A_OptimalSize = slc.Cells["A_OptimalSize"].Value.ToString();
                var A_OptimalRatio = slc.Cells["A_OptimalRatio"].Value.ToString();
                var A_UserDefinedSize = slc.Cells["A_UserDefinedSize"].Value.ToString();
                var A_UserDefinedRato = slc.Cells["A_UserDefinedRato"].Value.ToString();
                int i = 0;
                var rout = routeInfo.LstInsRouteInfo.Where(d => d.LstSegment.Where(f => f.ToString().Split(',').Last() == seg && f.ToString().Split(',').First() == A_SegSignalType).Any());
                if (!rout.Any())
                {
                    dtSchedule.Rows.Add(new object[] {
                           //i.ToString(),
                           A_SegSignalType,
                           seg,
                           totalCables,
                           Length,

                           A_TotalArea,
                           A_OptimalSize,
                           A_OptimalRatio,
                           A_UserDefinedSize,
                           A_UserDefinedRato,
                           "",
                       ""  ,
                       "",
                       ""
                        });
                    lstCounted.Add(c.OwningRow);
                    continue;
                }
                foreach (var f in routeInfo.LstInsRouteInfo)
                {
                    var res = f.LstSegment.Where(x => x.ToString().Split(',').Last() == seg && x.Split(',').First() == A_SegSignalType);
                    if (res.Any())
                    {
                        i++;
                        dtSchedule.Rows.Add(new object[] {
                           //i.ToString(),
                           A_SegSignalType,
                           seg,
                           totalCables,
                           Length,
                           A_TotalArea,
                           A_OptimalSize,
                           A_OptimalRatio,
                           A_UserDefinedSize,
                           A_UserDefinedRato,
                           f.SystemType,
                       !String.IsNullOrEmpty( f.Instrument.t2)?  f.Instrument.t1 + "_" + f.Instrument.t2 : f.Instrument.t1  ,
                       f.To,
                        f.CableType
                        });
                    }
                }
                lstCounted.Add(c.OwningRow);
            }

            if (dtSchedule.Rows.Count == 0)
            {
                MessageBox.Show("Please select some cable duct routes to export.");
                return;
            }

            dtSchedule.DefaultView.Sort = "SegmentName asc";
            DataView view = dtSchedule.DefaultView;
            view.Sort = "SegmentName ASC";
            dtSchedule = view.ToTable();
            if (!Directory.Exists(DataSourceCableDuctSchedule))
            {
                Directory.CreateDirectory(DataSourceCableDuctSchedule);
            }

            using (var writer = new StreamWriter(DataSourceCableDuctSchedule + GetFileName("SegmentCableSchedule_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<CableScheduleExport> lst = new List<CableScheduleExport>();
                CableScheduleExport ie;

                foreach (DataRow dr in dtSchedule.Rows)
                {
                    ie = new CableScheduleExport();
                    ie.AssigedDuct = dr["SignalType"].ToString().Trim();
                    //ie.No = dr["No"].ToString().Trim();
                    ie.SegmentName = dr["SegmentName"].ToString().Trim();
                    ie.TotalCable = dr["TotalCable"].ToString().Trim();
                    ie.Length = dr["Length"].ToString().Trim();
                    //ie.OptimalDuctSize = dr["OptimalDuctSize"].ToString().Trim();

                    ie.TotalArea = dr["A_TotalArea"].ToString().Trim();
                    ie.OptimalSize = dr["A_OptimalSize"].ToString().Trim();
                    ie.OptimalRatio = dr["A_OptimalRatio"].ToString().Trim();
                    ie.UserDefinedSize = dr["A_UserDefinedSize"].ToString().Trim();
                    ie.UserDefinedRato = dr["A_UserDefinedRato"].ToString().Trim();

                    ie.Signal = dr["SystemType"].ToString().Trim();
                    ie.TagName = dr["TagName"].ToString().Trim();
                    ie.To = dr["To"].ToString().Trim();
                    ie.Cable = dr["Cable"].ToString().Trim();

                    lst.Add(ie);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();

            }
            System.Diagnostics.Process.Start(DataSourceCableDuctSchedule);


            // Datatable for selected part is maded.
            // CSV Writer is not in the packate.. // ***Jeaeun
        }
        private void button7_Click_1(object sender, EventArgs e)
        {

            List<DataGridViewRow> lstCounted = new List<DataGridViewRow>() { };
            dtSchedule = new DataTable();
            dtSchedule.Columns.Add("DuctType");
            dtSchedule.Columns.Add("SystemType");
            dtSchedule.Columns.Add("TagName");
            dtSchedule.Columns.Add("To");
            dtSchedule.Columns.Add("Cable");
            dtSchedule.Columns.Add("TotalLength");
            dtSchedule.Columns.Add("TotalSegment");
            dtSchedule.Columns.Add("SegmentName");
            dtSchedule.Columns.Add("Length");

            var set = dgv_InsAnalysis.SelectedCells;
            if (set.Count == 0) return;
            foreach (DataGridViewCell c in set)
            {
                if (lstCounted.Where(x => x == c.OwningRow).Any()) continue;
                var slc = c.OwningRow;

                var A_SignalType = slc.Cells["A_SignalType"].Value.ToString();
                var A_SystemType = slc.Cells["A_SystemType"].Value.ToString();

                var Ins = slc.Cells["A_Ins"].Value.ToString();
                var To = slc.Cells["A_To"].Value.ToString();
                var Cable = slc.Cells["A_Type"].Value.ToString();
                var Length = slc.Cells["A_Length"].Value.ToString();
                var Segment = slc.Cells["A_Seg"].Value.ToString();
                int i = 0;


                //var rout = routeInfo.LstInsRouteInfo.Where(d => d.LstSegment.Where(f => f.ToString() == seg).Any());
                if (Length == "0" || Segment == "0")
                {
                    dtSchedule.Rows.Add(new object[] {
                          A_SignalType,
                          A_SystemType,
                           Ins,
                            To,
                           Cable,
                           "0",
                        "0"  ,
                       "",
                       ""
                        });
                    lstCounted.Add(c.OwningRow);
                    continue;
                }
                // var res = routeInfo.LstInsRouteInfo.Where(x => (!String.IsNullOrEmpty(x.Instrument.t2) ? (x.Instrument.t1 + "_" + x.Instrument.t2) : (x.Instrument.t1)) == Ins && x.CableType == Type);
                foreach (var f in routeInfo.LstSegmentInfo)
                {
                    //       f.CableList.Add((!string.IsNullOrEmpty(d.Instrument.t2) ? (d.Instrument.t1 + "_" + d.Instrument.t2) : (d.Instrument.t1)) + "_" + d.SystemType + "_" + d.SignalType +"_" + d.CableType);
                    var res = true;
                    try
                    {
                        res = f.CableList.Where(s => s.Split('_')[0] == Ins).Any();
                    }
                    catch
                    {
                        res = false;
                    }
                    var res1 = true;
                    try
                    {
                        res1 = f.CableList.Where(s => (s.Split('_')[0] + "_" + s.Split('_')[1]) == Ins).Any();
                    }
                    catch
                    {
                        res1 = false;
                    }

                    // var  res = f.CableList.Where(s => s.Split('_')[4] == Ins && s.Split('_')[3] == A_SignalType && s.Split('_')[2] == A_SystemType ); //f.CableList.Where(s => s.Replace("_" + s.Split('_').Last(), "") == Ins );
                    //  var res1 = f.CableList.Where(s => s.Split('_')[3] == Ins && s.Split('_')[2] == A_SignalType && s.Split('_')[1] == A_SystemType ); //f.CableList.Where(s => s.Replace("_" + s.Split('_').Last(), "") == Ins );

                    if (res || res1)
                    {
                        dtSchedule.Rows.Add(new object[] {
                               A_SignalType,
                          A_SystemType,
                            Ins,
                            To,
                           Cable,
                           Length,
                           Segment,
                           f.SegmentName,
                           f.Length
                        });
                    }
                }
                lstCounted.Add(c.OwningRow);
            }

            if (dtSchedule.Rows.Count == 0)
            {
                MessageBox.Show("추출하고자 하는 케이블 덕트 경로를 선택해주세요.");
                return;
            }
            foreach (DataRow dr in dtSchedule.Rows)
            {
                if (!string.IsNullOrEmpty(dr["Length"].ToString()))
                    dr["Length"] = Math.Round(Convert.ToDouble(dr["Length"].ToString()) / 1000, 2);
            }
            //dtSchedule.DefaultView.Sort = "SegmentName asc";
            DataView view = dtSchedule.DefaultView;
            view.Sort = "TagName ASC";
            dtSchedule = view.ToTable();
            if (!Directory.Exists(DataSourceCableDuctSchedule))
            {
                Directory.CreateDirectory(DataSourceCableDuctSchedule);
            }

            using (var writer = new StreamWriter(DataSourceCableDuctSchedule + GetFileName("InstrumentSchedule_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<InstrumentCableScheduleExport> lst = new List<InstrumentCableScheduleExport>();
                InstrumentCableScheduleExport ie;

                foreach (DataRow dr in dtSchedule.Rows)
                {
                    ie = new InstrumentCableScheduleExport();
                    ie.AssignedDuct = dr["DuctType"].ToString().Trim();
                    ie.Signal = dr["SystemType"].ToString().Trim();
                    ie.TagName = dr["TagName"].ToString().Trim();
                    ie.To = dr["To"].ToString().Trim();
                    ie.Cable = dr["Cable"].ToString().Trim();
                    ie.TotalLength = dr["TotalLength"].ToString().Trim();
                    ie.TotalSegement = dr["TotalSegment"].ToString().Trim();
                    ie.SegmentName = dr["SegmentName"].ToString().Trim();
                    ie.Length = dr["Length"].ToString().Trim();
                    var nxtstring = ie.AssignedDuct + ie.Signal + ie.TagName + ie.To + ie.Cable + ie.TotalLength + ie.TotalSegement + ie.SegmentName + ie.Length;
                    if (lst.Where(x => (x.AssignedDuct + x.Signal + x.TagName + x.To + x.Cable + x.TotalLength + x.TotalSegement + x.SegmentName + x.Length) == nxtstring).Any())
                    {
                        continue;
                    }
                    lst.Add(ie);
                }
                //  lst = lst.Distinct<InstrumentCableScheduleExport>().ToList();
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();

            }
            System.Diagnostics.Process.Start(DataSourceCableDuctSchedule);

        }
        DataGridViewRow tempInsRow = null;
        private void instDGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            VisibleAll();
            if (e.RowIndex > -1 && instDGV[e.ColumnIndex, e.RowIndex] is DataGridViewCheckBoxCell chk)
            {
                var cr = instDGV.Rows[e.RowIndex];
                if ((bool)chk.FormattedValue)
                {
                    //gr.DefaultCellStyle.BackColor = Color.Yellow;
                    cr.Cells["colT1"].Style.BackColor = Color.Yellow;
                    cr.Cells["colT2"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colType"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colSystem"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colTo"].Style.BackColor = Color.Yellow; 

                }
                else
                {
                    cr.Cells["colT2"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    cr.Cells["colT1"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    // cr.Cells["col_Check_Instrument"].Value = false; 
                    // cr.DefaultCellStyle.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    return;
                }
            }
            if (e.RowIndex > -1 && instDGV[e.ColumnIndex, e.RowIndex] is DataGridViewComboBoxCell cbo)
            {
                if (!(bool)(instDGV.CurrentRow.Cells["col_Check_Instrument"].FormattedValue))
                    return;
                MultiSelectionDropdown(sender, e);
            }
            //SetInsCable();
            // instListbox_CellValueChanged(null, null); // PTK mented on  2023 08 21
            // instDGV_CellEndEdit(null,null);
        }
        private void MultiSelectionDropdown(object sender, DataGridViewCellEventArgs e)
        {
            if (tempInsRow == null && !isTriggerConfirmedClick)
            {
                tempInsRow = (sender as DataGridView).CurrentRow;
                //instListbox_CellValueChanged(null, null); // Multiple selection
                foreach (DataGridViewRow dr in instDGV.Rows)
                {
                    if (!(bool)(dr.Cells["col_Check_Instrument"].FormattedValue))
                        continue;
                    if (dr.Cells["LayerName"].Value != null)
                    {
                        //if (tempInsRow.Cells["dgv_Layer"].Value.ToString() == dr.Cells["LayerName"].Value.ToString())
                        //{
                        if (instDGV.CurrentCell.OwningColumn.Name == "colSystem")
                            dr.Cells["colSystem"].Value = tempInsRow.Cells["colSystem"].Value;
                        if (instDGV.CurrentCell.OwningColumn.Name == "colTo") // This Columns should be assigned or operated lastly.  requested by PTK // 
                        {
                            dr.Cells["colTo"].Value = tempInsRow.Cells["colTo"].Value;
                            var ins = dr.Cells["colT1"].Value.ToString().Trim() + dr.Cells["colT2"].Value.ToString().Trim();
                            routeInfo.DGV_LstInstrument.Where(x => x.T1.Trim() + x.T2.Trim() == ins).FirstOrDefault().LstInstCableEntity = null;
                            //SetInsCable(null,null);
                            //foreach (var inst in routeInfo.DGV_LstInstrument)
                            //{
                            //     inst
                            //}
                        }
                        if (instDGV.CurrentCell.OwningColumn.Name == "colType")
                            dr.Cells["colType"].Value = tempInsRow.Cells["colType"].Value;
                        //}
                    }
                }
                tempInsRow = null;
            }
        }
        private void instDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        public void SimulateRightClick(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }
        private void MainP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (routeInfo == null) return;

            if (e.Control && e.Alt && e.KeyCode == Keys.I)
            {
                string Msg = "Imported layer is " + lbl_ImportedLayer + Environment.NewLine + "Saved layer is " + lbl_savedLayer;
                MessageBox.Show(Msg);
                return;
            }
            if (e.Control && e.Alt && e.KeyCode == Keys.D)
            {

                string Msg = "Are you sure to remove all drawn paths?";
                var resul = MessageBox.Show(Msg, "", MessageBoxButtons.YesNo);
                if (resul == DialogResult.Yes)
                {
                    var allLine = routeInfo.LstAllRoute();
                    foreach (var l in allLine)
                    {
                        routeInfo.SyncronyzeRelatedUI(new List<int> { l.Id }, vdFramedControl, true);
                    }
                }
                return;
            }
            if (e.Control && e.Alt && e.KeyCode == Keys.R)
            {
                string Msg = "Are you sure to remove all URL Setters?";
                var resul = MessageBox.Show(Msg, "", MessageBoxButtons.YesNo);
                if (resul == DialogResult.Yes)
                {
                    foreach (vdFigure f in vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities)
                    {
                        if (!string.IsNullOrEmpty(f.URL))
                        {
                            f.URL = null;
                        }
                    }

                    if (string.IsNullOrEmpty(vdFramedControl.BaseControl.ActiveDocument.FileName))
                        return;
                    //if (rdo_WithoutRoute.Checked)
                    //{
                    //    MessageBox.Show("Saving for Auto Route option is still developing.");
                    //    return;
                    //}
                    SaveFileDialog sd = new SaveFileDialog();

                    sd.Filter = "AutoCAD|*.dwg|VDCL(*.vdcl)|*.vdcl";
                    var newName = DateTime.Now.ToString("yyyMMddhhmmss");
                    sd.FileName = newName;
                    if (sd.ShowDialog(this) == DialogResult.OK)
                    {

                        //SaveDialogFile = sd.FileName;
                        vdFramedControl.BaseControl.ActiveDocument.SaveAs(sd.FileName);
                        MessageBox.Show("New Saving was done!!!");
                    }
                }
                return;
            }
            if (e.KeyCode == Keys.L)
            {
                btnMainRoute_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.M)
            {

                button15_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.U)
            {
                button16_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.E)
            {
                button5_Click_1(null, null);
                return;
            }
            if (e.KeyCode == Keys.R)
            {
                button17_Click_1(null, null);
                return;
            }
            if (e.KeyCode == Keys.Space)
            {
                if (routeInfo.CURRENT_MODE == RouteInfo.eACTION_MODE.MAINROUTE)
                {
                    SendKeys.Send("{ESC}");
                    SendKeys.Send("{ESC}");
                    RefreshCADSpace();
                }
                if (routeInfo.CURRENT_MODE == eACTION_MODE.MOVE)
                {
                    SimulateRightClick(0, 0);
                }
                if (routeInfo.CURRENT_MODE == eACTION_MODE.ERASE)
                {
                    SimulateRightClick(0, 0);
                }
                return;
            }
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.ERASE;
            LstSelectedID = new List<int>();
            var res = vdFC1.BaseControl.ActiveDocument.Selections[0];// ;
            if (e.KeyCode == Keys.Delete && res.Count > 0)
            {
                foreach (vdFigure l in res)
                {
                    vdFC1.BaseControl.ActiveDocument.CommandAction.CmdErase(l);
                }

                routeInfo.SyncronyzeRelatedUI(LstSelectedID, vdFC1);  // Prepare TBBox Info
                button10_Click(null, null); // Obstables
                cbo_layer_Setting_SelectedIndexChanged(null, null);// Customized Destination 
                int r1 = 0;
                DestinationBind(ref r1);
                routeInfo.UpdateInstrument(vdFC1, instDGV); // Instrument
                return;
            }
        }

        public void PutDecimal(DataGridView dgv, List<string> lst)
        {
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    if (lst.Contains(dc.OwningColumn.Name))
                    {
                        dc.Value = String.Format("{0:0.00}", Convert.ToDouble(dc.Value));
                    }
                }
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var zp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    zp = folderBrowserDialog.SelectedPath;
                }
                else
                    return;
                using (var archive = new Aspose.Zip.SevenZip.SevenZipArchive())
                {
                    var dir = DataSourceXml.Replace("\\DS", "") + "\\EXPORT";
                    if (!Directory.Exists(dir)) MakeDirectory(dir);
                    foreach (var d in Directory.GetFiles(dir)) DeleteWithAdminRight(d);

                    //routeInfo.SYSTEMABBRE + Path.GetFileNameWithoutExtension(SaveDialogFile) + "_" + GetFileName("") + ".xml"
                    var fname = dir + "\\" + routeInfo.SYSTEMABBRE + Path.GetFileNameWithoutExtension(vdFC1.BaseControl.ActiveDocument.FileName) + "_" + GetFileName("") + ".xml";

                    //Save XML
                    File.Copy(SaveXmlFile, fname, true);


                    //Save DWG
                    // File.Copy(SaveDialogFile, dir + "\\" + Path.GetFileName(SaveDialogFile), true);
                    SaveDWG(Path.GetFileNameWithoutExtension(fname), (dir + "\\" + Path.GetFileName(this.vdFC1.BaseControl.ActiveDocument.FileName)));

                    archive.CreateEntries(dir, false);

                    //Save and Archive both as a single zip
                    archive.Save(zp + "\\" + "RouteOptimizerData_" + Path.GetFileNameWithoutExtension(SaveDialogFile) + ".zip");
                    Process.Start("explorer.exe", zp);
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void SaveDWG(string layerName, string path)
        {
            var lyrs = vdFC1.BaseControl.ActiveDocument.Layers.GetNotDeletedItems();
            vdLayer lyr = null;
            foreach (vdLayer l in lyrs)
            {
                if (l.Name.Contains(routeInfo.SYSTEMABBRE))
                {
                    lyr = l;
                    break;
                }
            }
            if (lyr != null)
            {
                vdFC1.BaseControl.ActiveDocument.Layers.RemoveItem(lyr);
            }

            if (!string.IsNullOrEmpty(layerName))
            {
                var newlyr = new vdLayer(this.vdFC1.BaseControl.ActiveDocument, layerName);
                vdFC1.BaseControl.ActiveDocument.Layers.Add(newlyr);
                vdFC1.BaseControl.ActiveDocument.SaveAs(path);
                var alllayers = vdFC1.BaseControl.ActiveDocument.Layers;
                var addedlayer = alllayers.FindName(layerName);
                alllayers.RemoveItem(addedlayer);
            }
            if (lyr != null)
            {
                lyr = new vdLayer(this.vdFC1.BaseControl.ActiveDocument, lyr.Name);
                vdFC1.BaseControl.ActiveDocument.Layers.Add(lyr);
            }
        }
        static int inputType = 0;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ZIP files (*.zip)|*.zip";
            openFileDialog.Title = "Select a ZIP file";
            string selectedFilePath = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
            if (string.IsNullOrEmpty(selectedFilePath)) return;
            try
            {
                using (var archive = new Aspose.Zip.SevenZip.SevenZipArchive(selectedFilePath))
                {
                    var exDir = selectedFilePath.Replace(".zip", "");
                    if (!Directory.Exists(exDir)) Directory.CreateDirectory(exDir);
                    foreach (var f in Directory.GetFiles(exDir)) DeleteWithAdminRight(f);
                    archive.ExtractToDirectory(exDir);
                    var AllFile = Directory.GetFiles(exDir);
                    if (AllFile.Count() != 2)
                    {
                        MessageBox.Show("Datasource 파일을 읽어들이는 데에 오류가 발생했습니다.");
                        return;
                    }
                    var pathSourceXML = "";
                    var pathSourceCadFile = "";
                    foreach (var p in Directory.GetFiles(exDir))
                    {
                        if (Path.GetExtension(p).ToLower().Contains("xml"))
                        {
                            pathSourceXML = DataSourceXmlBuffer + "\\" + Path.GetFileName(p);
                            FileCopy(p, pathSourceXML);
                            //FileCopy(p, DataSourceXmlBuffer);
                            //MessageBox.Show(p + Environment.NewLine + DataSourceXmlBuffer);
                            DeleteWithAdminRight(p);
                        }
                        if (Path.GetExtension(p).ToLower().Contains("dwg") || Path.GetExtension(p).ToLower().Contains("vdcl"))
                        {
                            pathSourceCadFile = p;
                        }
                    }
                    if (string.IsNullOrEmpty(pathSourceCadFile))
                    {
                        MessageBox.Show("Datasource 파일을 읽어들이는 데에 오류가 발생했습니다.");
                        return;
                    }
                    Import(pathSourceCadFile);
                    inputType = 1;
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show(ex.StackTrace);
                return;

            }


        }
        public void FileCopy(string source, string desti)
        {
            try
            {
                File.Copy(source, desti);
            }
            catch
            {
                try
                {
                    if (!UseAdmin) // use admin
                    {
                        desti = "'" + desti + "'";
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            CreateNoWindow = true,
                            FileName = "powershell.exe",
                            Arguments = $"copy \"{source}\" \"{desti}\"",
                            Verb = "runas"
                        };
                        Process.Start(psi);
                    }
                }
                catch (Exception exx)
                {
                    // MessageBox.Show(exx.StackTrace);
                }
            }
        }

        private void btn_New_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("새 파일을 만드시겠습니까? 이전 작업 내용은 삭제됩니다.", "New document", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                Initialize();
                vdFC1.BaseControl.ActiveDocument.New();
                vdFC1.BaseControl.Redraw();
                vdFC1.BaseControl.Update();
                vdFC1.Update();
                vdFC1.BaseControl.ActiveDocument.Redraw(true);
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Refresh();
                vdFC1.BaseControl.ActiveDocument.ActiveLayer.Update();
                //RefreshUpdate();
                //this.Refresh();
            }
        }

        private void instListbox_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            VisibleAll();
            if (e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //ComboBox comboBox = sender as ComboBox;
            //string selectedValue = comboBox.SelectedItem.ToString();
            //MessageBox.Show("Selected value: " + selectedValue);
        }

        private void instListbox_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = instListbox.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)instListbox.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)instListbox.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void dgv_SegAnalysis_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (routeInfo == null) return;
            if (e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(dgv_AnalysisSegcomboBox_SelectedIndexChanged);
            }
        }
        private void dgv_AnalysisSegcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            BL.SettingBL sbl = new BL.SettingBL();
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem == null) return;
            string selectedValue = comboBox.Text.ToString();

            if (!string.IsNullOrEmpty(selectedValue))
            {
                var TileDuctlist = sbl.GetCableDuctList();
                var SelectedDuct = TileDuctlist.Where(x => x.Title == selectedValue);
                if (!SelectedDuct.Any())
                {
                    // MessageBox.Show("There is no related datasource with CableDuctList.csv.");
                    return;
                }
                var DuctArea = SelectedDuct.FirstOrDefault();
                DataGridViewComboBoxEditingControl ecs = sender as DataGridViewComboBoxEditingControl;
                var r = dgv_SegAnalysis.Rows[ecs.EditingControlRowIndex];
                var TotalArea = Convert.ToDouble(r.Cells["A_TotalArea"].Value.ToString());
                r.Cells["A_UserDefinedRato"].Value = Math.Round(TotalArea / (DuctArea.Width * DuctArea.Height) * 100, 2);
                var lstsgi = routeInfo.LstSegmentInfo.Where(x => x.SegmentName == r.Cells["A_SegName"].Value.ToString()).FirstOrDefault();

                lstsgi.A_UserDefinedSize = r.Cells["A_UserDefinedSize"].Value.ToString();
                lstsgi.A_UserDefinedRato = Convert.ToDouble(r.Cells["A_UserDefinedRato"].Value.ToString());
            }

        }

        private void dgv_SegAnalysis_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dgv_SegAnalysis.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgv_SegAnalysis.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dgv_SegAnalysis.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
            e.Cancel = true;
        }

        private void dgv_SegAnalysis_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (routeInfo == null) return;
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                if ((ComboBox)datagridview.EditingControl == null) return;
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dgv_SegAnalysis_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            dgv_SegAnalysis.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void btnAllDest_Click(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            cbo_layer_Setting.Text = "";
            cbo_layer_Setting.SelectedIndex = -1;
            dgv_destination.Rows.Clear();
            foreach (var l in routeInfo.LstLayer)
            {
                var lyr = l.name;

                foreach (var des in routeInfo.LstTBBOXes)
                {
                    if (des.polyline.Layer.Name == lyr)
                    {
                        var type = "";// !des.IsIO ? eDestinationType.TBBox.ToString() : eDestinationType.IORoom.ToString();
                        if (des.IsIO == eDestinationType.TBBox)
                        {
                            type = eDestinationType.TBBox.ToString();
                        }
                        else if (des.IsIO == eDestinationType.IORoom)
                        {
                            type = eDestinationType.IORoom.ToString();

                        }
                        else if (des.IsIO == eDestinationType.MCC)
                        {
                            type = eDestinationType.MCC.ToString();
                        }
                        //   var type = des.Name.ToLower().Contains("tb") ? eDestinationType.TBBox.ToString() : eDestinationType.IORoom.ToString();  // Assume TB are Tb and others as Io
                        dgv_destination.Rows.Add(des.guid, des.NameId, lyr, type, des.Name.Replace("TB-", "").Replace("IO-", ""), des.CableType, des.OwnDestination == null ? "" : des.OwnDestination.Name);
                    }
                }
            }
        }
        public bool UseAdmin { get; set; } = true;
        private void btn_Runas_Click(object sender, EventArgs e)
        {
            if (UseAdmin)
            {

                MessageBox.Show("지금부터 관리자 모드로 전환됩니다.", "Security information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btn_Runas.Font = new Font(btn_Runas.Font, FontStyle.Bold);
                btn_Runas.ForeColor = Color.BlueViolet;
                UseAdmin = false;

                var sm = new SecurityManager.Permission();
                sm.AddRightsByPS();
            }
            else
            {
                MessageBox.Show("지금부터 관리자 모드로 전환됩니다.", "Security information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btn_Runas.ForeColor = (new ToolStripButton()).ForeColor;
                UseAdmin = true;
                btn_Runas.Font = new Font(btn_Runas.Font, FontStyle.Regular);
            }
        }

        private void txtHTolerance_TextChanged(object sender, EventArgs e)
        {
            txtYTolerence.Text = txtHTolerance.Text;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.PrinterName = (new PrinterSettings()).PrinterName;//"c:\\test_extends.emf"; // or PDF bmp jpg svg
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.Resolution = 96; //Screen DPI
                VectorDraw.Geometry.Box Extends = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetBoundingBox(true, true);
                int EMFwidth = 500; //fixed width
                int EMFheight = (int)(EMFwidth * Extends.Height / Extends.Width); // keep propotions
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.paperSize = new Rectangle(0, 0, (int)(EMFwidth * 100 / vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.Resolution), (int)(EMFheight * 100 / vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.Resolution));
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.PrintExtents();
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.PrintScaleToFit();
                vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.CenterDrawingToPaper();
                var dl = MessageBox.Show("현재 작업 내용을 확인하시겠습니까?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dl == DialogResult.Yes)
                {
                    vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.InitializePreviewFormProperties(true, true, false, false);
                    var Isdview = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.DialogPreview();
                    if (Isdview)
                    {
                        MessageBox.Show("인쇄가 성공적으로 완료되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show("인쇄에 실패하였습니다.");
                    }
                }
                else
                {
                    vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Printer.PrintOut();
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show(ex.Message);
            }
        }


        private void MainP1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (inputType == 0) // input dwg
            {
                var dl = MessageBox.Show("현재 작업 내용을 저장하시겠습니까?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, defaultButton: MessageBoxDefaultButton.Button2);

                if (dl == DialogResult.Yes)
                {
                    toolStripButton3_Click(null, null);
                }
                if (dl == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else//// input zip
            {
                var dl = MessageBox.Show("정말 닫으시겠어요?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, defaultButton: MessageBoxDefaultButton.Button1);
                 
                if (dl == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            DebugLog.WriteLog("Program ended ***************************************");
        }

        #region Prof's Phase2
        private void VisibleAllRoute(bool visible, int Flg = 0)
        {
            if (routeInfo == null) return;
            //btn_next.Enabled = visible;
            if (Flg == 1)
            {
                btn_GenerateRoute.Enabled = pnl_Alternative.Visible = btn_Restart.Enabled = btn_DeleteConnector.Enabled = visible;
            }
            else if (Flg == 0)
            {
                btn_GenerateRoute.Enabled = pnl_Alternative.Visible = btn_Restart.Enabled = btn_DeleteConnector.Enabled = !visible;
            }
        }

        private void tabControl3_Selected(object sender, TabControlEventArgs e)
        {
            //btn_next.Enabled = false;
        }

        private void tabControl3_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2)
            {
                e.Cancel = true;
            }
        }
        bool IsAllowAnalysis = false;
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (((System.Windows.Forms.TabControl)sender).SelectedTab.Text == "Analyze")
            {
                // IsAllowAnalysis = false;
                tabControl3.SelectedIndex = 0;
                tabControl3.SelectedTab = tab_Scope;
                //btn_next.Enabled = btn_GenerateRoute.Enabled;
                btn_GenerateRoute.Enabled = (rdo_WithoutRoute.Checked || rdo_withroute.Checked);
            }
        }

        private void Tab_Analysis_Click(object sender, EventArgs e)
        {

        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btn_next.Enabled = rdo_UserDefine.Checked;
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            // if (routeInfo == null || (t != null && t.IsAlive)) return;
            IsAllowAnalysis = true;
            tabControl3.SelectedIndex = 1;

        }

        private void ResetAlternativeDestination()
        {

            dgv_alternaitve.Rows.Clear();
            //dgv_alternative_tb.Rows.Clear();

            foreach (var tb in routeInfo.LstTBBOXes)
            {
                tb.lstInstrument.Clear();
            }
            if (selectedRoute != null)
            {
                foreach (Connector connector1 in selectedRoute.disconnectedConnectors)
                {
                    connector1.line.Update();
                }
            }
            if (selectedRoute != null)
            {
                selectedRoute.disconnectedConnectors.Clear();
                selectedRoute.disconnectedInstruments.Clear();
            }
            selectedRoute = new Route(null);
            selectedRoutes = new Routes();
            mMatrix = null;
            mps = null;
            routeInfo.CaseRoutes = new List<Routes>();
            routeInfo.CaseRoutes_WithRoute = new List<Routes>();
            foreach (vdLine l in routeInfo.LstAutoRoute)
            {
                //var res= routeInfo.SyncronyzeRelatedUI(new List<int> { l.Id },vdFC1);
                var res = this.vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(l);
                //if (!res)
                //{

                //}
            }
            routeInfo.LstAutoRoute = new List<vdLine>();
            ClearRouteCacheSetting();
            RefreshCADSpace();
        }
        double Progressrate = 0;
        Thread t;
        bool isRun = true;
        public void setMatrix(TBBOXDestination tBBOXDestination, List<Instrument> instruments)
        {

            this.boundary = GetMainBoundary();
            vdCurves c = this.boundary.getOffsetCurve(1000);
            routeInfo.OffSetBoundary = new vdPolyline(c.Document, c[0].GetGripPoints());
            Box box = routeInfo.OffSetBoundary.BoundingBox;
            sx = box.Left;
            sy = box.Bottom;

            int wGap = unitGrid * Convert.ToInt32(txtHTolerance.Text);
            int hGap = unitGrid * Convert.ToInt32(txtYTolerence.Text);
            int wc = (int)box.Width / (wGap + 1);
            int hc = (int)box.Height / (hGap + 1);
            int pCount = wc * hc;
            //    tBBOXDestination.mMatrix = new int[hc, wc];
            //    tBBOXDestination.mps = new MatrixPoint[hc, wc];//
            mps = new MatrixPoint[hc, wc];//
            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < wc; j++)
                {
                    gPoint p = new gPoint(sx + j * wGap, sy + i * hGap);
                    MatrixPoint mp = new MatrixPoint(p, i, j);
                    mps[i, j] = mp;
                    //   tBBOXDestination.mps[i, j] = mp;

                    bool isIn = contains(routeInfo.MainBoundary.VertexList, p);
                    // 바운더리 안에 있는지 없는지가 true / false로 나타남.
                    bool isIn2 = false;

                    foreach (var obstacle in routeInfo.GetSelectedObstacles(vdFC1, ObstListbox))
                    {
                        Vertexes vertexes = new Vertexes();
                        if (obstacle != null)
                        {
                            foreach (var pt in obstacle.vdObstacle.BoundingBox.GetPoints())
                            {
                                vertexes.Add(pt);
                            }
                        }
                        isIn2 = contains(vertexes, p);
                        if (isIn2) break;
                    }
                    // obstacle 안에 노드가 있는지 없는지 true / false로 나타남.
                    //tBBOXDestination.mMatrix[i, j] = Graph.regularDistance;
                    mMatrix[i, j] = Graph.regularDistance;
                    // 갈 수 있으면 true, 못 가면 false.

                    if (!isIn || isIn2)
                    {
                        mMatrix[i, j] = int.MaxValue;
                        //tBBOXDestination.mMatrix[i, j] = int.MaxValue;
                    }
                }
            }
        }

        public void StartGrid(TBBOXDestination tBBOXDestination, List<Instrument> instruments)
        {
            if (tBBOXDestination == null || instruments == null) return;
            setMatrix(tBBOXDestination, instruments);
            int[,] matrix = (int[,])tBBOXDestination.mMatrix.Clone();
            int minDistance = int.MaxValue;
            int count = 0;
            foreach (MatrixPoint mp in tBBOXDestination.mps)
            {
                var dis = (int)tBBOXDestination.centerPoint.Distance2D(mp.gp);
                if (minDistance > dis)
                {
                    minDistance = dis;
                    minDistance = dis;
                    tBBOXDestination.mp = mp;
                    tBBOXDestination.gridPoint = mp.gp;
                    tBBOXDestination.gridIndex = count;
                }
                count++;
            }
            foreach (Instrument ins in instruments)
            {
                minDistance = int.MaxValue;
                count = 0;
                foreach (MatrixPoint mp in tBBOXDestination.mps)
                {
                    int dis = (int)ins.centerPoint.Distance2D(mp.gp);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        ins.distance = dis;
                        ins.gridIndex = count;
                        ins.mp = mp;
                    }
                    if (dis < Convert.ToInt32(txtHTolerance.Text) * unitGrid) // units
                    {
                        ins.mps.Add(mp);
                    }
                    count++;
                }
            }
            instruments = instruments.OrderBy(x => x.distanceFromDestination).ToList();
            tBBOXDestination.lstInstrument = instruments;

            //주변으로 이동하도록 주변의 거리 매트릭스의 값을 낮춤
            foreach (Instrument ins in instruments)
            {
                Point sp1 = new Point(ins.mp.x, ins.mp.y);
                for (int o = -1; o < 1; o++)
                {
                    for (int p = -1; p < 1; p++)
                    {
                        if (((ins.mp.x + o > 0) && (ins.mp.x + o) < matrix.GetLength(0)) && ((ins.mp.y + p > 0) && (ins.mp.y + p) < matrix.GetLength(1)))
                        {
                            if (matrix[ins.mp.x + o, ins.mp.y + p] != int.MaxValue) matrix[ins.mp.x + o, ins.mp.y + p] = 4;
                        }
                    }
                }
            }


            int[,] matrix2 = (int[,])matrix.Clone();
            DisForm disform = new DisForm();
            double cou = -1;
            isRun = true;
            Random r = new Random();
            int maxCount = 1000;
            while (cou < maxCount)//cou < 100000&& isRun == true
            {

                cou++;
                Console.WriteLine("count " + cou);
                //Random으로 순서를 변경
                List<Instrument> instruments2 = new List<Instrument>();
                int[] array = routeInfo.getArray(instruments.Count);
                string ar = "";
                for (int g = 0; g < array.Length; g++)
                {
                    instruments2.Add(instruments[array[g]]);
                    ar += array[g] + ",";
                }


                Instrument ins0 = instruments2[0]; //3.4 7, 9 , 10* 11*

                //if (ins0.mps.Count == 0) 
                //    continue;
                int v = r.Next(ins0.mps.Count - 1);
                MatrixPoint mp = ins0.mps[v];
                while (mp == null) // PTK added because sometimes it caused MP'null with random value 
                {
                    v = r.Next(ins0.mps.Count - 1);
                    mp = ins0.mps[v];
                }
                while (matrix[mp.x, mp.y] == int.MaxValue)
                {
                    v = r.Next(ins0.mps.Count - 1);
                    mp = ins0.mps[v];
                }
                /////////////
                Point sp0 = new Point(mp.x, mp.y);

                ins0.mp = mp;
                Point ep0 = new Point(tBBOXDestination.mp.x, tBBOXDestination.mp.y);
                //Point sp0 = new Point(ins0.mp.x, ins0.mp.y);

                List<SubRoute> mainRoutes = new List<SubRoute>();

                List<Point> gs1 = FindRoute.findWay1(matrix, sp0, ep0);
                if (gs1 != null)
                {
                    SubRoute route0 = new SubRoute(ins0, tBBOXDestination, ins0.mp);

                    route0.setPoints(gs1, tBBOXDestination.mps);
                    mainRoutes.Add(route0);
                }

                List<Point> gs2 = FindRoute.findWay2(matrix, sp0, ep0);
                if (gs2 != null)
                {
                    SubRoute route0 = new SubRoute(ins0, tBBOXDestination, ins0.mp);

                    route0.setPoints(gs2, tBBOXDestination.mps);
                    mainRoutes.Add(route0);
                }
                if (mainRoutes.Count == 0)
                {
                    SubRoute route0 = new SubRoute(ins0, tBBOXDestination, ins0.mp);

                    List<Point> pList = null;
                    try
                    {
                        pList = disform.setInit(matrix, sp0, ep0, 0);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    route0.setPoints(pList, tBBOXDestination.mps);
                    mainRoutes.Add(route0);
                }

                foreach (SubRoute mainRoute in mainRoutes)
                {
                    Route route = new Route(null);
                    route.id = ar;// permutation.ToString();
                    tBBOXDestination.routes.Add(route);
                    route.subRoutes.Add(mainRoute);
                    route.name = mainRoute.name;
                    matrix = (int[,])matrix2.Clone(); //거리 매트릭스 초기화

                    //갔던 길을 갈 수 있도록 거리값을 2로 줄임
                    foreach (Point p in mainRoute.points)
                    {
                        matrix[p.X, p.Y] = 2;
                    }

                    for (int i = 1; i < instruments2.Count; i++)// instruments2.Count
                    {
                        Instrument ins = instruments2[i]; //3.4 7, 9 , 10* 11*
                        int v1 = r.Next(ins.mps.Count - 1);
                        MatrixPoint mp1 = ins.mps[v1];
                        while (matrix[mp1.x, mp1.y] == int.MaxValue)
                        {
                            v1 = r.Next(ins.mps.Count - 1);
                            mp1 = ins.mps[v1];
                        }
                        Point sp = new Point(mp1.x, mp1.y);
                        SubRoute subRoute = new SubRoute(ins, tBBOXDestination, mp1);

                        List<Point> ps = null;
                        try
                        {
                            ps = disform.setInit(matrix, sp, ep0, 0);
                        }
                        catch (Exception)
                        {

                            break;
                        }
                        ps.Reverse();

                        List<Point> newPs = new List<Point>();
                        foreach (Point p in ps)
                        {
                            if (matrix[p.X / 2, p.Y / 2] == 2)
                            {
                                newPs.Add(p);
                                break;
                            }
                            else
                            {
                                newPs.Add(p);
                                matrix[p.X / 2, p.Y / 2] = 2;
                            }
                        }
                        subRoute.setPoints(newPs, tBBOXDestination.mps);
                        route.subRoutes.Add(subRoute);
                    }

                    AnaylisRoute.analysisRoute(this.vdFC1.BaseControl.ActiveDocument, route, instruments);
                    //int cCount = 0;

                    //while (cCount != route.connectors.Count)
                    //{
                    //    cCount = route.connectors.Count;
                    //    AnaylisRoute.removeSubRoute(this.vdFC1.BaseControl.ActiveDocument, route, instruments);
                    //}
                    AnaylisRoute.joinLines(this.vdFC1.BaseControl.ActiveDocument, route, instruments);
                }
                double val = Progressrate * (cou + 1) / maxCount;
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = (int)(val * 100); }));
                }
                else
                    progressBar1.Value = (int)(val * 100);
            }
            // remove and filter route's connecter which do not startd or ended at Destination , same bends and same lengths
            if (chk_relation.Checked) FilterRoute(tBBOXDestination, instruments);
            // int no = 0;
            //foreach (Route route in CurrentDestination.routes)
            //{
            //    no++;
            //    object[] ob = { route, no.ToString(), Math.Round(route.getLength() / 1000, 2), Math.Round(route.MaxLengthSegment() / 1000, 2), route.getBendCount() };
            //    this.dgv_alternaitve.Rows.Add(ob);
            //}
            //this.vdFC1.BaseControl.Redraw();
        }
        private void FilterRouteNew(ref List<Routes> rts)
        {
            //ptk added 
            #region remove and filter route's connecter which do not startd or ended at Destination , same bends and same lengths
            rts = rts.OrderBy(x => x.bend).ToList();
            List<Routes> lsttemp = new List<Routes>();
            foreach (var ro in rts) // remove without contact with destination
            {
                var rs = ro.connectors.Where(c => (routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(c.line.getEndPoint())).Any() || routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(c.line.getStartPoint())).Any()));
                if (!rs.Any())
                {
                    lsttemp.Add(ro);
                }
                var rsBoth = ro.connectors.Where(c => (routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(c.line.getEndPoint())).Any() && routeInfo.LstTBBOXes.Where(x => x.polyline.BoundingBox.PointInBox(c.line.getStartPoint())).Any()));
                if (rsBoth.Any())
                {
                    lsttemp.Add(ro);
                }
            }
            foreach (var lt in lsttemp) rts.Remove(lt);
            var rd = from row in rts.AsEnumerable()
                     group row by new { bend = row.getBendCount(), length = row.getLength() } into g
                     select new { Bend = g.Key.bend, Length = g.Key.length };
            List<Routes> filterRoute = new List<Routes>();
            foreach (var l in rd.ToList())
            {
                var newR = rts.Where(x => x.getBendCount() == l.Bend && x.getLength() == l.Length).FirstOrDefault();
                filterRoute.Add(newR);
            }
            // CurrentDestination.routes = new List<Route>();
            rts = filterRoute;




            //int no = 0;
            //foreach (Route route in CurrentDestination.routes)
            //{
            //    no++;
            //    object[] ob = { route, no.ToString(), Math.Round(route.getLength() / 1000, 2), Math.Round(route.MaxLengthSegment() / 1000, 2), route.getBendCount() };
            //    this.dgv_alternaitve.Rows.Add(ob);
            //}
            //this.vdFC1.BaseControl.Redraw();

            #endregion
        }
        private void FilterRoute(TBBOXDestination CurrentDestination, List<Instrument> instruments)
        {
            //ptk added 
            #region remove and filter route's connecter which do not startd or ended at Destination , same bends and same lengths
            CurrentDestination.routes = CurrentDestination.routes.OrderBy(x => x.bend).ToList();
            List<Route> lsttemp = new List<Route>();
            foreach (var ro in CurrentDestination.routes) // remove without contact with destination
            {
                var rs = ro.connectors.Where(c => (CurrentDestination.polyline.BoundingBox.PointInBox(c.line.getEndPoint()) || CurrentDestination.polyline.BoundingBox.PointInBox(c.line.getStartPoint())));
                if (!rs.Any())
                {
                    lsttemp.Add(ro);
                }
                var rsBoth = ro.connectors.Where(c => (CurrentDestination.polyline.BoundingBox.PointInBox(c.line.getEndPoint()) && CurrentDestination.polyline.BoundingBox.PointInBox(c.line.getStartPoint())));
                if (rsBoth.Any())
                {
                    lsttemp.Add(ro);
                }
            }
            foreach (var lt in lsttemp)
            {
                CurrentDestination.routes.Remove(lt);
            }
            var rd = from row in CurrentDestination.routes.AsEnumerable() group row by new { bend = row.getBendCount(), length = row.getLength() } into g select new { Bend = g.Key.bend, Length = g.Key.length };
            List<Route> filterRoute = new List<Route>();
            foreach (var l in rd.ToList())
            {
                var newR = CurrentDestination.routes.Where(x => x.getBendCount() == l.Bend && x.getLength() == l.Length).FirstOrDefault();
                filterRoute.Add(newR);
            }
            // CurrentDestination.routes = new List<Route>();
            CurrentDestination.routes = filterRoute;




            //int no = 0;
            //foreach (Route route in CurrentDestination.routes)
            //{
            //    no++;
            //    object[] ob = { route, no.ToString(), Math.Round(route.getLength() / 1000, 2), Math.Round(route.MaxLengthSegment() / 1000, 2), route.getBendCount() };
            //    this.dgv_alternaitve.Rows.Add(ob);
            //}
            //this.vdFC1.BaseControl.Redraw();

            #endregion
        }
        private void dgv_alternaitve_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1 || (t != null && t.IsAlive)) return;
                if (routeInfo.DoneAnalysis)
                {
                    var msg = MessageBox.Show("다른 대안을 선택하실 경우 분석 내용이 사라집니다. 진행하시겠습니까?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (msg == DialogResult.No)
                    {
                        return;
                    }
                    if (msg == DialogResult.Yes)
                    {
                        rdo_defaultDuct.Checked = true;
                        routeInfo.LstSegmentInfo.Clear();
                        routeInfo.LstInsRouteInfo.Clear();
                        routeInfo.LstSelectedSegmentInfo.Clear();
                        routeInfo.DoneAnalysis = false;
                        ClearAllRoutes(true);
                    }
                }
                Routes r = (Routes)this.dgv_alternaitve[0, e.RowIndex].Value;
                this.selectedRoutes = r;

                drawRoute();

            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void ClearText()
        {
            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            List<vdText> lttxt = new List<vdText>();
            foreach (var f in e)
            {
                try
                {
                    if (f is vdText vdText && vdText.TextString.Contains(routeInfo.SEGMENTABBRE)) lttxt.Add(vdText);
                }
                catch
                {
                }
            }
            foreach (var t in lttxt)
            {
                e.RemoveItem(t);
            }
        }
        private void dgv_alternaitve_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void drawRoute()
        {
            vdLayer layer = vdFC1.BaseControl.ActiveDocument.Layers.FindName("Route");
            if (layer == null)
            {
                layer = new vdLayer(this.vdFC1.BaseControl.ActiveDocument, "Route");
                vdFC1.BaseControl.ActiveDocument.Layers.Add(layer);
            }

            foreach (vdLine l in routeInfo.LstAutoRoute)
            {
                this.vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(l);
            }

            this.routeInfo.LstAutoRoute.Clear();
            foreach (Connector connector in selectedRoutes.connectors)
            {
                routeInfo.CURRENT_MODE = eACTION_MODE.NONE;
                this.vdFC1.BaseControl.ActiveDocument.Model.Entities.AddItem(connector.line);
                this.routeInfo.LstAutoRoute.Add(connector.line);
                connector.line.Layer = layer;
            }
            this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
        }
        public void drawRoute(VectorDraw.Render.vdRender render)
        {
            //foreach (Connector connector1 in this.selectedRoute.connectors)
            //{
            //    foreach (Instrument ins in connector1.instruments)
            //    {
            //        gPoint gp = connector1.line.getClosestPointTo(ins.centerPoint);

            //        render.DrawLine(this.vdFC1.BaseControl.ActiveDocument, ins.centerPoint, gp);
            //    }
            //}
        }
        private void button3_Click_1(object sender, EventArgs e)
        {

            //  if (Off) return;
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            vdSelection sel = this.vdFC1.BaseControl.ActiveDocument.Selections[0];
            foreach (vdFigure f in sel)
            {

                f.PenColor = new vdColor(Color.Red);
                foreach (Connector connector1 in this.selectedRoutes.connectors)
                {
                    if (!(f is vdLine)) continue;
                    //     var overlapped =  (
                    //         (connector1.line.StartPoint == ((f as vdLine).StartPoint) &&   connector1.line.EndPoint == ((f as vdLine).EndPoint))
                    //||       (connector1.line.EndPoint == ((f as vdLine).StartPoint) &&  connector1.line.StartPoint == ((f as vdLine).EndPoint))
                    //);
                    if (connector1.line == f)//|| overlapped)
                    {
                        f.PenColor = new vdColor(Color.Red);
                        f.Update();
                        if (!this.selectedRoutes.selectedConnectors.Contains(connector1))
                            this.selectedRoutes.selectedConnectors.Add(connector1);
                    }
                }
            }

            foreach (Connector connector1 in this.selectedRoutes.selectedConnectors)
            {
                foreach (Instrument instrument in connector1.instruments)
                    if (!this.selectedRoutes.selectedInstruments.Contains(instrument))
                        this.selectedRoutes.selectedInstruments.Add(instrument);
            }

            this.vdFC1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
            this.vdFC1.BaseControl.ActiveDocument.Redraw(true);

            //this.vdFC1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
            //this.vdFC1.BaseControl.ActiveDocument.Redraw(true);
        }
        bool Off = true;
        #endregion

        private void btn_GenerateRoute_EnabledChanged(object sender, EventArgs e)
        {
            //  pnl_alternative_tbbox.Visible = pnl_Alternative.Visible= btn_GenerateRoute.Enabled;
        }
        private void dgv_alternaitve_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = dgv_alternaitve.HitTest(e.X, e.Y);
            if (hti.Type == DataGridViewHitTestType.Cell && hti.ColumnIndex == 5)
            {
                CheckState(hti);
            }
        }
        private void CheckState(DataGridView.HitTestInfo hti = null)
        {
            DataGridViewCheckBoxCell cell = dgv_alternaitve[hti.ColumnIndex, hti.RowIndex] as DataGridViewCheckBoxCell;
            if (cell != null)
            {
                var value = true;// !(bool)cell.EditedFormattedValue;
                                 // cell.Value = value;

                if (value)
                {
                    foreach (DataGridViewRow row in dgv_alternaitve.Rows)
                    {
                        if (hti.RowIndex != row.Index)
                        {
                            (row.Cells[5] as DataGridViewCheckBoxCell).Value = !value;
                            row.DefaultCellStyle.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                        }
                        else
                        {
                            (row.Cells[5] as DataGridViewCheckBoxCell).Value = value;

                            row.DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }
                }
                dgv_alternaitve.EndEdit();
            }
        }
        private void dgv_alternaitve_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgv_alternaitve_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 5) return;
            DataGridViewCheckBoxCell cell = dgv_alternaitve[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;
            if (cell != null)
            {
                var value = true;//!(bool)cell.EditedFormattedValue;
                //cell.Value = value;

                if (value)
                {
                    foreach (DataGridViewRow row in dgv_alternaitve.Rows)
                    {
                        if (e.RowIndex != row.Index)
                        {
                            (row.Cells[5] as DataGridViewCheckBoxCell).Value = !value;
                            row.DefaultCellStyle.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                        }
                        else
                        {
                            (row.Cells[5] as DataGridViewCheckBoxCell).Value = value;
                            row.DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }
                }
                dgv_alternaitve.EndEdit();
            }
        }


        private void ClearAllRoutes(bool IsincludeText = false, TBBOXDestination tbb = null)
        {
            if (routeInfo == null) return;
            var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            if (tbb == null)
            {

                var alllist = routeInfo.LstAllRoute();
                foreach (var le in alllist)
                {
                    if (le != null)
                        routeInfo.SyncronyzeRelatedUI(new List<int>() { le.Id }, vdFC1);
                }
                routeInfo.LstExternalRoute = new List<vdLine>();
                foreach (var tb in routeInfo.LstTBBOXes)
                {
                    tb.MainRouteCollection = new List<vdLine>();
                }
                List<vdLine> lstLineCache = new List<vdLine>();   // to remove remainingline that cant be caught by Routelines in TBBox still when Analyzer use to divide the Line through
                foreach (vdFigure vdFigure in e)
                {
                    if (vdFigure is vdLine vdLine)
                        foreach (var b in routeInfo.LstTBBOXes)
                        {
                            var g = new gPoints();
                            var res = b.polyline.IntersectWith(vdLine, VdConstInters.VdIntOnBothOperands, g);
                            if (res && g.Count == 1 && (b.polyline.BoundingBox.PointInBox(vdLine.StartPoint) || b.polyline.BoundingBox.PointInBox(vdLine.EndPoint)))
                            {
                                lstLineCache.Add(vdLine);
                            }
                        }
                }

                foreach (var l in lstLineCache)
                {
                    e.RemoveItem(l);
                }

            }
            else
            {
                foreach (var le in routeInfo.LstExternalRoute)
                    e.RemoveItem(le);
                routeInfo.LstExternalRoute = new List<vdLine>();

                foreach (var tb in routeInfo.LstTBBOXes)
                {
                    foreach (var le in tb.MainRouteCollection)
                        e.RemoveItem(le);
                    tb.MainRouteCollection = new List<vdLine>();
                }
            }
            if (IsincludeText)
                ClearText();
            RefreshCADSpace();
        }
        private void btn_Reset(object sender, EventArgs e)
        {
            if (Off) return;
            if (routeInfo == null || dgv_alternaitve.Rows.Count == 0) return;
            ResetAllDrawnLine();
        }
        private void ResetAllDrawnLine()
        {
            if (rdo_WithoutRoute.Checked)
            {
                ResetAllRouteLine();
                foreach (var r in routeInfo.LstAutoRoute)
                {
                    routeInfo.SyncronyzeRelatedUI(new List<int>() { r.Id }, vdFC1);
                }
            }
            if (rdo_withroute.Checked)
            {
                //  ResetAllRoute();
                foreach (var r in routeInfo.LstAutoGuidedRoute)
                {
                    routeInfo.SyncronyzeRelatedUI(new List<int>() { r.Id }, vdFC1);
                }
            }
            if (rdo_UserDefine.Checked)
            {
                //   ResetAllRoute();
                foreach (var r in routeInfo.LstGuidedRoute)
                {
                    routeInfo.SyncronyzeRelatedUI(new List<int>() { r.Id }, vdFC1);
                }
            }
        }
        private void ResetAllRouteLine()
        {
            foreach (var t in routeInfo.LstTBBOXes)
            {
                foreach (vdLine l in t.routeLines)
                {
                    var res = this.vdFC1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(l);
                }
                //tb.routes.Clear();// = new List<vdLine>();
                t.routeLines.Clear();// = new List<Route>();
                t.selectedRoute.disconnectedConnectors.Clear();
                t.selectedRoute.disconnectedInstruments.Clear();
            }
            RefreshCADSpace();
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vdFC1.GetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid))
            {
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, false);
                txt_PropertiesGrid.Text = "속성 창 - Off"; //Properties Grid - On Properties Grid - Off
                                                        // Properties Grid -On 속성 창 - Off
            }
            else
            {
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, true);
                txt_PropertiesGrid.Text = "속성 창 - On"; //Properties Grid - On Properties Grid - Off 
            }
            RefreshCADSpace();

        }

        private void txt_MenuPopup_Click(object sender, EventArgs e)
        {
            if (vdFC1.GetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.LayoutPopupMenu))
            {
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.StatusBar, false);
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.LayoutPopupMenu, false);
                txt_MenuPopup.Text = "팝업 메뉴 - Off"; //팝업 메뉴 - Off
            }
            else
            {
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.LayoutPopupMenu, true);
                vdFC1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.StatusBar, true);
                txt_MenuPopup.Text = "팝업 메뉴 - On";
            }
            RefreshCADSpace();
        }
      
        private void chk_CheckAll_CheckedChanged(object sender, EventArgs e)
        {
            VisibleAll();
            IsCellEditing = true;
            foreach (DataGridViewRow dr in instDGV.Rows)
            {
                // dr.Cells["col_Check_Instrument"].Value = chk_CheckAll.Checked;
                if (chk_CheckAll.Checked)
                {
                    //gr.DefaultCellStyle.BackColor = Color.Yellow;
                    dr.Cells["colT2"].Style.BackColor = Color.Yellow;
                    dr.Cells["colT1"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colType"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colSystem"].Style.BackColor = Color.Yellow;
                    //dr.Cells["colTo"].Style.BackColor = Color.Yellow;
                    dr.Cells["col_Check_Instrument"].Value = true;
                }
                else
                {
                    dr.Cells["colT2"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    dr.Cells["colT1"].Style.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    //dr.DefaultCellStyle.BackColor = System.Drawing.Color.Red;//  (new DataGridViewRow()).DefaultCellStyle.BackColor;
                    dr.Cells["col_Check_Instrument"].Value = false;
                    //dr.Cells["colT2"].Style.BackColor = System.Drawing.Color.FromArgb(0,0,0,0);
                    //dr.Cells["colT1"].Style.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                    // dr.DefaultCellStyle.BackColor = (new DataGridViewRow()).DefaultCellStyle.BackColor;
                }
            }
            instDGV.Refresh();
            instDGV.RefreshEdit();
            RefreshCADSpace();
        }


        List<InstrumentInfoEntity> iiet = new List<InstrumentInfoEntity>();
        private void AlternativeChangemmTom()
        {
            foreach (DataGridViewRow dr in dgv_alternaitve.Rows)
            {
                dr.Cells["col_longest"].Value = Math.Round(Convert.ToDouble(dr.Cells["col_longest"].Value) / 1000, 2);
                dr.Cells["col_length"].Value = Math.Round(Convert.ToDouble(dr.Cells["col_length"].Value) / 1000, 2);
            }
        }
        //Combination Algorithm
        List<TBBOXDestination> desitnations_ana = new List<TBBOXDestination>();
        Route selectedRoute = new Route(null);
        Routes selectedRoutes = new Routes();
        int[,] mMatrix = null;
        MatrixPoint[,] mps = null;
        public int unit { get; set; } = 3000;  // For Instrument's Nearest distance to its center  


        public int unitDestinationNeareast { get; set; } = 1000;  // For destination's nearest distance to its center as Instrument
        public int unitGrid { get; set; } = 1000;// For Grid Scale

        private void chk_relation_CheckedChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void CheckFilter()
        {
            foreach (var r in routeInfo.CaseRoutes)
            {
                if (String.IsNullOrEmpty(r.routeID))
                    r.routeID = Guid.NewGuid().ToString();
                foreach (var cntr in r.connectors)
                {
                    if (string.IsNullOrEmpty(cntr.connectorID))
                        cntr.connectorID = Guid.NewGuid().ToString();
                }
            }
            dgv_alternaitve.Rows.Clear();

            List<Routes> lstr = new List<Routes>();
            if (chk_relation.Checked)
            {

                foreach (var r in routeInfo.CaseRoutes)
                    lstr.Add(r);
                FilterRouteNew(ref lstr);
            }
            else
            {
                lstr = routeInfo.CaseRoutes;
            }
            List<Routes> rmoveList = new List<Routes>();
            if (!chk_TinySegment.Checked)
            {
                foreach (var r in lstr)
                {
                    try
                    {
                        var f = r.connectors.Where(v => v.line != null).Min(l => l.line.Length());
                        if (f == unitGrid)
                        {
                            rmoveList.Add(r);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Stack<(int, int, bool)> b = new Stack<(int, int, bool)>(); ;
                    }
                }
            }
            //if (rmoveList.Count > 0)
            //{
            //    foreach(var g in rmoveList)
            //    {
            //        lstr.Remove(g);
            //    }
            //}
            int c = 0;

            foreach (Routes routes in lstr.OrderBy(x => x.bend))
            {
                if (rmoveList.Contains(routes))
                    continue;
                c++;
                object[] ob = { routes, c, routes.getLength(), routes.MaxLengthSegment(), routes.getBendCount(), true };
                this.dgv_alternaitve.Rows.Add(ob);
            }

            AlternativeChangemmTom();
            this.vdFC1.BaseControl.Redraw();
        }
        private void ribbon_Click(object sender, EventArgs e)
        {

            if (splitContainer1.SplitterDistance > 300)
            {
                splitContainer1.SplitterDistance = 20;
            }
            else
            {
                splitContainer1.SplitterDistance = 1200;
            }
        }

        private void button9_Click_3(object sender, EventArgs e)
        {
            if (routeInfo != null && vdFramedControl.BaseControl.ActiveDocument != null)
            {

                var alreadyOpened = Application.OpenForms.Cast<System.Windows.Forms.Form>().Select(f => f.Name == "DuplicateItem").Where(x => x == true);
                if (alreadyOpened.Any() && alreadyOpened.ToList()[0])
                {
                    return;
                }
                DuplicateItem duplicateItem = new DuplicateItem(routeInfo, vdFramedControl, true);
                duplicateItem.Owner = this;
                duplicateItem.Show();

                CheckDuplicate();

            }

        }
        private void CheckDuplicate()
        {
            if ((thread != null && (thread.IsAlive)) || routeInfo == null) return;
            thread = new Thread(delegate ()
            {

                var res = routeInfo.CheckDuplicatedInstrumentNoMessage(vdFramedControl);
                if (res == 0)
                {
                    btn_Duplicate.BackColor = (new Button()).BackColor;
                    btn_Duplicate.Enabled = false;
                    btn_Duplicate.Text = "No Duplicated Item";
                }
                else
                {
                    btn_Duplicate.BackColor = Color.Yellow;
                    btn_Duplicate.Enabled = true;
                    btn_Duplicate.Text = res + " Duplicated Items";
                }
            });
            thread.Start();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 1)
            {
                CheckDuplicate();
            }
            if (tabControl2.SelectedIndex == 2) 
            {
                btnAllDest_Click(null, null);
                //CheckDuplicate();
            }
        }

        private void instDGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            VisibleAll();
            try
            {
                if (e.Control is ComboBox && instDGV.CurrentCell.OwningColumn.Name == "colTo")
                {
                    ComboBox comboBox = e.Control as ComboBox;
                    try
                    {
                        comboBox.SelectedIndexChanged -= DGVcomboBox_SelectedIndexChanged;
                    }
                    catch { }
                    comboBox.SelectedIndexChanged += new EventHandler(DGVcomboBox_SelectedIndexChanged);
                }
            }
            catch(Exception ex)
            {
                var exx = ex;
            }
        }
        bool IsCellEndEditing = false;
        private void DGVcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsCellEditing && instDGV.CurrentCell.OwningColumn.Name == "colTo")
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox.SelectedItem == null || ((System.Data.DataRowView)comboBox.SelectedItem).IsEdit) return;
                var stringv = comboBox.SelectedValue;
                string selectedValue = string.IsNullOrEmpty(comboBox.Text.ToString().Trim()) ? "empty destination." : "destination " + comboBox.Text.ToString().Trim() + ".";
                if (string.IsNullOrEmpty(comboBox.Text.ToString().Trim()) || (DestinationID == comboBox.SelectedValue.ToString().Trim()))
                {
                    DestinationID = "";
                    IsCellEndEditing = false;
                    return;
                }
                //var msg =new DialogResult();// 
                if (chk_Notify.Checked)
                {
                    var msg = MessageBox.Show("이 계측기기 시그널의 모든 목적지가 한 번에 수정됩니다. - " + selectedValue + "저장하시겠습니까?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1); ;

                    // msg = DialogResult.Yes;
                    if (msg == DialogResult.Yes)
                    {
                        IsCellEndEditing = false;
                    }
                    else
                    {
                        IsCellEndEditing = true;
                    }
                }
                else
                {
                    IsCellEndEditing = false;
                }
            }
            DestinationID = "";
        }

        private void instDGV_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.RowIndex > -1 && instDGV.CurrentCell.OwningColumn.Name == "colTo")
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void measureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btn_Measure.Text == "선 길이 측정 - On") //선 길이 측정 - Off
            {
                btn_Measure.Text = "선 길이 측정 - Off"; //선 길이 측정 - Off
            }
            else
            {
                btn_Measure.Text = "선 길이 측정 - On";
            }
        }

        private void chk_TinySegment_CheckedChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 1)
            {
                btnAllDest_Click(null, null);
                //CheckDuplicate();
            }
        }
        int BoxWidth = (new RouteInfo()).BoxWidth;
        int BoxHeight = (new RouteInfo()).BoxHeight;
        string TBColor = (new RouteInfo()).TBColor;
        string MCCColor = (new RouteInfo()).MCCcolor;
        string IOColor = (new RouteInfo()).IOcolor;
        private void btn_BoxSize_Click(object sender, EventArgs e)
        {
            int w = BoxWidth;
            int h = BoxHeight;
            BoxSizing boxSizing = new BoxSizing(w, h, IOColor, MCCColor, TBColor);
            boxSizing.ShowDialog();
            if (boxSizing.DialogResult == DialogResult.OK)
            {
                try
                {
                    BoxWidth = boxSizing.Width;
                    BoxHeight = boxSizing.Height;
                    IOColor = boxSizing.IOColor;
                    MCCColor = boxSizing.MCCColor;
                    TBColor = boxSizing.TBColor;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);

                }
            }
        }

        private void btn_GappedItem_Click(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            // var alreadyOpened = Application.OpenForms.Cast<System.Windows.Forms.Form>().Select(f => f.Name == "Gapped_Item");
            var alreadyOpened = Application.OpenForms.Cast<System.Windows.Forms.Form>().Select(f => f.Name == "Gapped_Item").Where(x => x == true);
            if (alreadyOpened.Any() && alreadyOpened.ToList()[0])
            {
                return;
            }

            Gapped_Item gapped_Item = new Gapped_Item(vdFC1, routeInfo);
            gapped_Item.Owner = this;
            gapped_Item.Show();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.MOVE;
            vdFC1.BaseControl.ActiveDocument.OrthoMode = false;
            LstSelectedID = new List<int>();
            var res = vdFC1.BaseControl.ActiveDocument.CommandAction.CmdMove(null, null, null);
            //var res = vdFC1.BaseControl.ActiveDocument.CommandAction.CmdErase(null, null, null);
            if (res)
            {
                vdFC1.BaseControl.Redraw();
                //
            }
            vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.UNDO;
            //  vdFC1.BaseControl.ActiveDocument.OrthoMode = false;
            //  LstSelectedID = new List<int>();
            var res = vdFC1.BaseControl.ActiveDocument.CommandAction.Undo(null);
            if (res)
            {
                RefreshCADSpace();
                // vdFC1.BaseControl.Redraw();
                //
            }
            //  vdFC1.BaseControl.ActiveDocument.OrthoMode = true;
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            if (routeInfo == null) return;
            routeInfo.CURRENT_MODE = RouteInfo.eACTION_MODE.REDO;
            vdFC1.BaseControl.ActiveDocument.CommandAction.Redo();
            RefreshCADSpace();
            // vdFC1.BaseControl.Redraw(); 
        }
        private void Doc_OnDrawAfter(object sender, vdRender render)
        {
         
        }
          int testLayer = 0;
        private void btnErase_Click(object sender, EventArgs e)
        {
            try
            {
                var re = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserPoint(out LeaderStartPoint);
                if (re == StatusCode.Success)
                {
                    testLayer++;
                    vdLayer vl = new vdLayer();
                    vl.Name = "Test_" + testLayer.ToString(); 
                    vl.SetUnRegisterDocument(this.vdFC1.BaseControl.ActiveDocument);
                    vl.setDocumentDefaults();
                    vl.VisibleOnForms = true;
                    vdFC1.BaseControl.ActiveDocument.Layers.Add(vl);
                    SetCustomLayer(vl.Name);
                    vl.Update(); 
                    var userPoint2 = vdFC1.BaseControl.ActiveDocument.ActionUtility.getUserRefPointLine(LeaderStartPoint) as gPoint;
                    gPoints gPoints = new gPoints();
                    gPoints.Add(LeaderStartPoint);
                    gPoints.Add(userPoint2);
                    vdFC1.BaseControl.ActiveDocument.CommandAction.CmdLine(gPoints);
                   var l= vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.Last as vdLine;
                    l.PenColor.ColorIndex = (short)(testLayer);
                    l.PenWidth = 100;
                    l.Update();
                    RefreshCADSpace();
                }
                //var l = routeInfo.LstAllRoute();
                //foreach (var ls in l)
                //{
                //  var s=  vdFC1.BaseControl.ActiveDocument.CommandAction.CmdSelect(ls);
                //    vdSelection vdSelection = new vdSelection();
                //    vdSelection.Add(ls);
                //    vdFC1.BaseControl.ActiveDocument.Selections.AddItem(vdSelection);

                //}
                //RefreshCADSpace();
                //var set = vdFC1.BaseControl.ActiveDocument.Selections[0];
                //if (set == null) return;
                //var se = set[0];
                //if (se == null) return;
                //if (se is vdLine p)
                //{
                //    MessageBox.Show(se.Id.ToString() + "__" + se._TypeName);
                //    //p.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);
                //    //vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(p);
                //    //RefreshCADSpace();
                //}
                //MessageBox.Show(se.Id.ToString() + "__" + se._TypeName);
            }
            catch
            {
            }
        }

        private void fileConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VD_File_Converter vD_File_Converter = new VD_File_Converter();
            vD_File_Converter.ShowDialog();
        }

        private void dgv_alternaitve_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void btn_DeleteConnector_EnabledChanged(object sender, EventArgs e)
        {
            panel1.Visible = btn_DeleteConnector.Enabled;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void StartRoutedThreading()
        {
            try
            {
                setMatrix();
                ObjectSetting();
                //PTK mented 8/10 Cox Prof's 7/11 COde replacement
                // StartMatrix();
                ReStartRouteThreading(mMatrix, null); //7/11 
                                                      // CheckFilter();
                MessageBox.Show("예측 가능한 경로가 성공적으로 생성되었습니다.");
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                var msg = ex.Message;
            }
        }
        private void btn_GenerateRoute_Click(object sender, EventArgs e)
        {

            if (routeInfo == null || routeInfo.DGV_LstInstrument == null || (t != null && t.IsAlive)) return;
            if (!routeInfo.CheckDuplicatedInstrument(vdFramedControl)) return;
            chk_relation.Checked = true;
            ResetAlternativeDestination();
            t = new Thread(StartRoutedThreading);
            t.Start();
        }

        private void button9_Click_2(object sender, EventArgs e)
        {
            //if (Off) return;
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            //  routeInfo.LstAutoRoute.Clear();
            this.routeInfo.CaseRoutes.Clear();
            this.dgv_alternaitve.Rows.Clear();
            int[,] newMatrix = (int[,])this.mMatrix.Clone();
            List<vdLine> lines = AnaylisRoute.getSelectedLine(this.selectedRoutes);
            AnaylisRoute.getNewMatrix(newMatrix, lines, this.mps);
            t = new Thread(() => Regenerate(newMatrix, lines));
            // t = new Thread(ReStartRouteThreading);
            t.Start();
        }
        public void Regenerate(int[,] matrix, List<vdLine> selectedLines)
        {
            try
            {
                ReStartRouteThreading(matrix, selectedLines);
                MessageBox.Show("예측 가능한 경로가 성공적으로 생성되었습니다.");
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

                var msg = ex.Message;
                MessageBox.Show("경로 재생성에 실패하였습니다. 설정 내용을 다시 확인해주세요.");
            }
        }
        private void ObjectSetting()
        {
            int minDistance = int.MaxValue;

            // TO DEFINE THAT WHICH INSTRUMENT GOES TO THE DESTINATION
            foreach (TBBOXDestination destination in routeInfo.LstTBBOXes)
            {
                var dest = routeInfo.DGV_LstInstrument.Where(x => x.LstInstCableEntity.Where(y => y.To == destination.guid.ToString()).Any());
                //  if (destination.IsIO == eDestinationType.MCC) continue;
                if (!dest.Any()) continue;
                minDistance = int.MaxValue;
                int count = 0;
                foreach (MatrixPoint mp in mps)
                {
                    int dis = (int)destination.centerPoint.Distance2D(mp.gp);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        minDistance = dis;
                        destination.mp = mp;
                        destination.gridPoint = mp.gp;
                        destination.gridIndex = count;
                    }
                    count++;
                }
            }
            iiet = new List<InstrumentInfoEntity>();

            //1>Create and Copy selected Instruments(InstrumentInfoEntity)
            foreach (InstrumentInfoEntity iie in routeInfo.DGV_LstInstrument)
            {
                iiet.Add(iie);
            }
            //2> Convert and add all MCC and TBBoxes as Instrument(InstrumentInfoEntity)
            foreach (var tb in routeInfo.LstTBBOXes)
            {
                //for MCC to desitnation
                if (tb.OwnDestination == null || string.IsNullOrEmpty(tb.OwnDestination.guid.ToString()))
                {
                    if (tb.IsIO == eDestinationType.MCC)
                    {
                        var e_tri = tb.LstmCCEntities.GroupBy(w => w.To);
                        if (e_tri.Any())
                        {
                            InstrumentInfoEntity ie = new InstrumentInfoEntity() { };
                            ie.Instrument = new Instrument(new vdCircle(null, tb.centerPoint, tb.polyline.BoundingBox.Width / 2), tb.Name, "Destination", true);
                            List<InstCableEntity> lstine = new List<InstCableEntity>();
                            foreach (var mCCEntity in e_tri.FirstOrDefault())
                            {
                                InstCableEntity instCableEntity = new InstCableEntity();
                                instCableEntity.To = mCCEntity.To;
                                if (!string.IsNullOrEmpty(mCCEntity.To))
                                    lstine.Add(instCableEntity);
                            }
                            ie.LstInstCableEntity = lstine;
                            iiet.Add(ie);
                        }
                    }
                }
                //For Destination to destination
                else
                {
                    InstrumentInfoEntity ie = new InstrumentInfoEntity() { };
                    ie.Instrument = new Instrument(new vdCircle(null, tb.centerPoint, tb.polyline.BoundingBox.Width / 2), tb.Name, "Destination", true);
                    List<InstCableEntity> lstine = new List<InstCableEntity>();
                    InstCableEntity instCableEntity = new InstCableEntity();
                    instCableEntity.To = tb.OwnDestination == null ? "" : tb.OwnDestination.guid.ToString();
                    lstine.Add(instCableEntity);
                    ie.LstInstCableEntity = lstine;
                    iiet.Add(ie);
                }
            }

            //3>Check and take "UNIT" value by checking its original is destination or pure instrument 
            // TO DIFINE EACH INST'S DESTINATION
            //foreach (InstrumentInfoEntity iie in routeInfo.DGV_LstInstrument)
            foreach (InstrumentInfoEntity iie in iiet)
            {
                var ins = new Instrument();
                ins = iie.Instrument;
                minDistance = int.MaxValue;
                int count = 0;
                foreach (MatrixPoint mp in mps)
                {
                    int dis = (int)ins.centerPoint.Distance2D(mp.gp);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        ins.distance = dis;
                        ins.gridIndex = count;
                        ins.mp = mp;
                    }
                    var gap = unit;
                    if (ins.t2 == "Destination")
                    {
                        gap = unitDestinationNeareast;
                    }
                    if (dis < gap)
                    {
                        ins.mps.Add(mp);
                    }
                    count++;
                }
            }

            // To assign the destination's Instrument
            foreach (TBBOXDestination destination in routeInfo.LstTBBOXes)
            {
                // if (destination.IsIO == eDestinationType.MCC) continue;
                var dest = iiet.Where(x => x.LstInstCableEntity.Where(y => y.To == destination.guid.ToString()).Any());

                // var dest = routeInfo.DGV_LstInstrument.Where(x => x.LstInstCableEntity.Where(y => y.To == destination.guid.ToString()).Any());
                if (!dest.Any()) continue;
                foreach (InstrumentInfoEntity iie in iiet)//routeInfo.DGV_LstInstrument)
                {
                    var ExistDestination = iie.LstInstCableEntity.Where(x => x.To == destination.guid.ToString());
                    //foreach (var cable in iie.LstInstCableEntity)
                    //{ 
                    if (ExistDestination.Any())
                    {
                        var ins = new Instrument();
                        ins = iie.Instrument;
                        //if (destination == routeInfo.FindIO_TBBox(iie.To))
                        //{
                        destination.lstInstrument.Add(ins);
                        //}
                    }
                    //}
                }
            }
        }
        public void setMatrix()
        {
            //Box box = this.offsetBoundary.BoundingBox;
            this.boundary = GetMainBoundary();
            vdCurves c = this.boundary.getOffsetCurve(1000);
            routeInfo.OffSetBoundary = new vdPolyline(c.Document, c[0].GetGripPoints());
            Box box = routeInfo.OffSetBoundary.BoundingBox;
            sx = box.Left;
            sy = box.Bottom;
            int wGap = unitGrid * Convert.ToInt32(txtHTolerance.Text);
            int hGap = unitGrid * Convert.ToInt32(txtYTolerence.Text);
            int wc = (int)box.Width / wGap + 1;
            int hc = (int)box.Height / hGap + 1;
            int pCount = wc * hc;

            mMatrix = new int[hc, wc];


            mps = new MatrixPoint[hc, wc];//
            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < wc; j++)
                {
                    gPoint p = new gPoint(sx + j * wGap, sy + i * hGap);
                    MatrixPoint mp = new MatrixPoint(p, i, j);
                    mps[i, j] = mp;

                    bool isIn = contains(this.boundary.VertexList, p);
                    // 바운더리 안에 있는지 없는지가 true / false로 나타남.
                    bool isIn2 = false;
                    foreach (vdPolyline obstacle in this.obstacles)
                    {
                        isIn2 = contains(obstacle.VertexList, p);
                        if (isIn2) break;
                    }
                    // obstacle 안에 노드가 있는지 없는지 true / false로 나타남.
                    mMatrix[i, j] = Graph.regularDistance;

                    // 갈 수 있으면 true, 못 가면 false.

                    if (!isIn || isIn2)
                    {
                        mMatrix[i, j] = int.MaxValue;

                    }



                }
            }

        }
        public void ReStartRouteThreading(int[,] matrix, List<vdLine> selectedLines)
        {
            if (matrix == null) return;
            int[,] matrix2 = (int[,])matrix.Clone();
            DisForm disform = new DisForm();
            double cou = -1;
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            Random r = new Random();
            routeInfo.MaxFrequency = Convert.ToInt32(cbo_Frequency.Text);
            while (cou < routeInfo.MaxFrequency)//100000
            {
                try
                {
                    progressBar1.Value = (100 * (Convert.ToInt32(cou) + 1)) / routeInfo.MaxFrequency;
                }
                catch
                {
                }
                cou++;
                Routes routes = new Routes();


                bool isSucceed = false;
                foreach (TBBOXDestination destination in routeInfo.LstTBBOXes)
                {
                    if (destination.mp == null) continue;
                    //Random으로 순서를 변경
                    List<Instrument> instruments2 = new List<Instrument>();
                    int[] array = routeInfo.getArray(destination.lstInstrument.Count);
                    string ar = "";


                    for (int g = 0; g < array.Length; g++)
                    {
                        instruments2.Add(destination.lstInstrument[array[g]]);
                        ar += array[g] + ",";
                    }
                    if (instruments2.Count == 0) continue;
                    Instrument ins0 = instruments2[0]; //3.4 7, 9 , 10* 11*
                    int v = r.Next(ins0.mps.Count - 1);
                    MatrixPoint mp = ins0.mps[v];
                    while (matrix[mp.x, mp.y] == int.MaxValue)
                    {
                        v = r.Next(ins0.mps.Count - 1);
                        mp = ins0.mps[v];
                    }


                    Point sp0 = new Point(mp.x, mp.y);

                    ins0.mp = mp;
                    Point ep0 = new Point(destination.mp.x, destination.mp.y);
                    //Point sp0 = new Point(ins0.mp.x, ins0.mp.y);

                    List<SubRoute> mainRoutes = new List<SubRoute>();

                    int type = r.Next(2);
                    /////////////////////////////////
                    // ㄱ shape route 
                    if (type == 0)
                    {
                        List<Point> gs1 = FindRoute.findWay1(matrix, sp0, ep0);
                        if (gs1 != null)
                        {
                            SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                            route0.setPoints(gs1, mps);
                            mainRoutes.Add(route0);
                        }
                    }
                    else
                    {
                        // L shape route 
                        List<Point> gs2 = FindRoute.findWay2(matrix, sp0, ep0);
                        if (gs2 != null)
                        {
                            SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                            route0.setPoints(gs2, mps);
                            mainRoutes.Add(route0);
                        }
                    }
                    ////////////////////////////////////////////

                    if (mainRoutes.Count == 0)
                    {
                        SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                        List<Point> pList = null;
                        try
                        {
                            pList = disform.setInit(matrix, sp0, ep0, 0);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                        route0.setPoints(pList, mps);
                        mainRoutes.Add(route0);
                    }

                    foreach (SubRoute mainRoute in mainRoutes)
                    {
                        Route route = new Route(destination);
                        route.id = ar;// permutation.ToString();
                        routes.routes.Add(route);
                        route.subRoutes.Add(mainRoute);
                        route.name = mainRoute.name;
                        matrix = (int[,])matrix2.Clone(); //거리 매트릭스 초기화

                        //갔던 길을 갈 수 있도록 거리값을 2로 줄임
                        foreach (Point p in mainRoute.points)
                        {
                            matrix[p.X, p.Y] = 2;
                        }

                        for (int i = 1; i < instruments2.Count; i++)// instruments2.Count
                        {
                            Instrument ins = instruments2[i]; //3.4 7, 9 , 10* 11*
                            int v1 = r.Next(ins.mps.Count - 1);
                            MatrixPoint mp1 = ins.mps[v1];
                            while (matrix[mp1.x, mp1.y] == int.MaxValue)
                            {
                                v1 = r.Next(ins.mps.Count - 1);
                                mp1 = ins.mps[v1];
                            }
                            /////////////
                            Point sp = new Point(mp1.x, mp1.y);
                            SubRoute subRoute = new SubRoute(ins, destination, mp1);

                            List<Point> ps = null;
                            try
                            {
                                ps = disform.setInit(matrix, sp, ep0, 0);
                            }
                            catch (Exception)
                            {

                                break;
                            }
                            ps.Reverse();

                            List<Point> newPs = new List<Point>();
                            foreach (Point p in ps)
                            {
                                if (matrix[p.X / 2, p.Y / 2] == 2)
                                {
                                    newPs.Add(p);
                                    break;
                                }
                                else
                                {
                                    newPs.Add(p);
                                    matrix[p.X / 2, p.Y / 2] = 2;
                                }
                            }
                            subRoute.setPoints(newPs, mps);
                            route.subRoutes.Add(subRoute);
                        }
                        AnaylisRoute.analysisRoute(this.vdFC1.BaseControl.ActiveDocument, route, destination.lstInstrument);
                        int cCount = 0;
                        //PTK mented 8/10 Cos it make no centering to TBBoxes
                        while (cCount != route.connectors.Count)
                        {
                            cCount = route.connectors.Count;
                            //0711 modified
                            AnaylisRoute.removeSubRoute(this.vdFC1.BaseControl.ActiveDocument, route, destination.lstInstrument, destination, unit);
                        }
                        AnaylisRoute.joinLines(this.vdFC1.BaseControl.ActiveDocument, route, destination.lstInstrument);
                    }
                }


                /// join routes 
                List<vdLine> lines = new List<vdLine>();
                List<Instrument> instruments = new List<Instrument>();
                foreach (Route route in routes.routes)
                {
                    instruments.AddRange(route.destination.lstInstrument);
                    routes.connectors.AddRange(route.connectors);
                    routes.nodes.AddRange(route.nodes);
                }
                for (int i = 0; i < routes.connectors.Count; i++)
                {
                    Connector connector = (Connector)routes.connectors[i];
                    lines.Add(connector.line);
                }

                AnaylisRoute.removeLines(lines);
                int linesCount = 0;

                while (linesCount != lines.Count)
                {
                    AnaylisRoute.breakLine(lines);
                    linesCount = lines.Count;
                }
                AnaylisRoute.removeLines(lines);
                AnaylisRoute.joinSameParallelLine(lines);
                routes.lines = lines;
                isSucceed = AnaylisRoute.joinLines2(this.vdFC1.BaseControl.ActiveDocument, routes, instruments, unit, routeInfo, iiet);
                ///////////////////////

                //restart(pin)
                bool isInAll = true;
                if (selectedLines != null)
                {
                    foreach (vdLine selectedLine in selectedLines)
                    {
                        bool isIn = false;
                        foreach (vdLine line in routes.lines)
                        {
                            if ((selectedLine.StartPoint == line.StartPoint && selectedLine.EndPoint == line.EndPoint) ||
                                    (selectedLine.EndPoint == line.StartPoint && selectedLine.StartPoint == line.EndPoint))
                            {
                                isIn = true;
                                break;
                            }
                        }
                        if (!isIn)
                        {
                            isInAll = false;
                            break;
                        }
                    }
                }
                if (isSucceed && isInAll)
                    this.routeInfo.CaseRoutes.Add(routes); //this.caseRoutes.Add(routes);
            }
            this.routeInfo.CaseRoutes = this.routeInfo.CaseRoutes.OrderBy(x => x.getBendCount()).ToList();
            //remove error
            for (int i = 0; i < this.routeInfo.CaseRoutes.Count; i++)
            {
                Routes routes = this.routeInfo.CaseRoutes[i];
                if (routes.getLength() % 10 != 0)
                {
                    this.routeInfo.CaseRoutes.Remove(routes);
                    i--;
                }
            }



            this.vdFC1.BaseControl.Redraw();
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            CheckFilter();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        private void rdo_WithoutRoute_CheckedChanged(object sender, EventArgs e)
        {
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            var ets = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            if (rdo_WithoutRoute.Checked)
            {
                routeInfo.DEFAULT_OPTION = eSCOPE_MODE.AUTO_ROUTE;

                foreach (var l in routeInfo.LstAutoRoute)
                {
                    routeInfo.CURRENT_MODE = eACTION_MODE.NONE;  //  ::::::::: for Algorithm Gerenerated mode , will use from Alternative values
                    try
                    {
                        ets.Add(l);
                    }
                    catch { }
                }
            }
            else
            {
                foreach (vdLine connector in routeInfo.LstAutoRoute)
                {
                    routeInfo.SyncronyzeRelatedUI(new List<int> { connector.Id }, vdFC1);
                }
                ClearAllRoutes();
            }
            ClearText();
            RefreshCADSpace();
        }
        private void rdo_withroute_CheckedChanged(object sender, EventArgs e)
        {
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            var ets = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            if (rdo_withroute.Checked)
            {
                routeInfo.DEFAULT_OPTION = eSCOPE_MODE.AUTO_GUIDED_ROUTE;
                foreach (var vl in routeInfo.LstAutoGuidedRoute)
                {
                    routeInfo.CURRENT_MODE = eACTION_MODE.NONE;    //// ::::::::: for Algorithm Gerenerated mode , will use from Alternative values
                    try
                    {
                        ets.Add(vl);
                    }
                    catch { }
                }
            }
            else
            {
                routeInfo.LstAutoGuidedRoute = routeInfo.LstAllRoute();
                foreach (vdLine connector in routeInfo.LstAutoGuidedRoute)
                {
                    if (connector != null )
                    routeInfo.SyncronyzeRelatedUI(new List<int> { connector.Id }, vdFC1);
                }
                ClearAllRoutes();
            }
            ClearText();
            RefreshCADSpace();
        }
        private void rdo_UserDefine_CheckedChanged(object sender, EventArgs e)
        {
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            var ets = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;

            if (rdo_UserDefine.Checked)
            {
                routeInfo.DEFAULT_OPTION = eSCOPE_MODE.USERDEFINED_ROUTE;
                foreach (var vl in routeInfo.LstGuidedRoute)
                {
                    routeInfo.CURRENT_MODE = eACTION_MODE.LINE;
                    try
                    {
                        ets.Add(vl);
                    }
                    catch { }
                }
            }
            else
            {
                routeInfo.LstGuidedRoute = routeInfo.LstAllRoute();
                foreach (var vl in routeInfo.LstGuidedRoute)
                {
                    if (vl != null)
                        routeInfo.SyncronyzeRelatedUI(new List<int> { vl.Id }, vdFC1);
                }
                ClearAllRoutes();
            }
            ClearText();
            RefreshCADSpace();

            VisibleAllRoute(rdo_UserDefine.Checked);
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dgv_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            tabControl3.SelectedIndex = 1;
        }

        private void BindFinalResult1()
        {
            var dtAnalysis = dgv_SegAnalysis.DataSource as DataTable;
            if (dtAnalysis == null || dtAnalysis.Rows.Count == 0) return;
            var dtres = dtAnalysis.Clone();
            List<SegmentInfoEntity> sfo = new List<SegmentInfoEntity>();
            foreach (DataRow dr in dtAnalysis.Rows)
            {
                SegmentInfoEntity segmentInfoEntity = new SegmentInfoEntity();
                segmentInfoEntity.SignalType = dr["A_SegSignalType"].ToString();
                segmentInfoEntity.Length = Convert.ToDouble(dr["Length"].ToString());
                segmentInfoEntity.A_OptimalDuctSize = dr["A_OptimalSize"].ToString();
                sfo.Add(segmentInfoEntity);
            }

            // colQuantity1.Visible = chk_SignalSegment.Checked;
            routeInfo.lstResult1 = new List<AnalysisResultEntity>();
            if (chk_SignalSegment.Checked)
            {
                var newList = sfo.GroupBy(x => new { x.SignalType, x.Length }).Select(y => new SegmentInfoEntity()
                {
                    SignalType = y.Key.SignalType,
                    Length = y.Key.Length,
                    A_OptimalDuctSize = y.FirstOrDefault().A_OptimalDuctSize,
                    ParentRouteID = y.Count()
                }
      );

                dgv_1.Rows.Clear();


                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_1.Rows.Add(new object[] {
                u ,
                l.SignalType,
                l.Length,
                l.ParentRouteID
                });

                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    ae.AssignedDuct = l.SignalType;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = l.ParentRouteID.ToString();
                    routeInfo.lstResult1.Add(ae);
                }
            }
            else
            {
                var newList = sfo.GroupBy(x => new { x.SignalType }).Select(y => new SegmentInfoEntity()
                {
                    SignalType = y.Key.SignalType,
                    Length = y.Sum(x => x.Length),
                    // A_OptimalDuctSize = y.FirstOrDefault().A_OptimalDuctSize,
                    // ParentRouteID = y.Count()
                }
);
                dgv_1.Rows.Clear();
                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_1.Rows.Add(new object[] {
                u ,
                l.SignalType,
                l.Length,
                1
               // l.ParentRouteID
                });

                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    ae.AssignedDuct = l.SignalType;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = (1).ToString();
                    routeInfo.lstResult1.Add(ae);
                }
            }
        }
        private void BindFinalResult2()
        {
            // A_SegSignalType
            //Length
            //A_OptimalSize

            var dtAnalysis = dgv_SegAnalysis.DataSource as DataTable;
            if (dtAnalysis == null || dtAnalysis.Rows.Count == 0) return;
            var dtres = dtAnalysis.Clone();
            List<SegmentInfoEntity> sfo = new List<SegmentInfoEntity>();
            foreach (DataRow dr in dtAnalysis.Rows)
            {
                SegmentInfoEntity segmentInfoEntity = new SegmentInfoEntity();
                segmentInfoEntity.SignalType = dr["A_SegSignalType"].ToString();
                segmentInfoEntity.Length = Convert.ToDouble(dr["Length"].ToString());
                segmentInfoEntity.A_OptimalDuctSize = dr["A_OptimalSize"].ToString();
                sfo.Add(segmentInfoEntity);
            }
            routeInfo.lstResult2 = new List<AnalysisResultEntity>();
            //colQuantity2.Visible = chk_SignalSegment.Checked;
            if (chk_SignalSegment.Checked)
            {
                var newList = sfo.GroupBy(x => new { x.SignalType, x.Length, x.A_OptimalDuctSize }).Select(y => new SegmentInfoEntity()
                {
                    SignalType = y.Key.SignalType,
                    Length = y.Key.Length,
                    A_OptimalDuctSize = y.Key.A_OptimalDuctSize,
                    ParentRouteID = y.Count()
                }
      );
                dgv_2.Rows.Clear();
                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_2.Rows.Add(new object[] {
                u,
                l.SignalType,
                l.A_OptimalDuctSize,
                l.Length,
                l.ParentRouteID,
                });

                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    ae.AssignedDuct = l.SignalType;
                    ae.DuctType = l.A_OptimalDuctSize;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = l.ParentRouteID.ToString();
                    routeInfo.lstResult2.Add(ae);

                }
            }
            else
            {
                var newList = sfo.GroupBy(x => new { x.SignalType, x.A_OptimalDuctSize }).Select(y => new SegmentInfoEntity()
                {
                    SignalType = y.Key.SignalType,
                    Length = y.Sum(x => x.Length),
                    A_OptimalDuctSize = y.Key.A_OptimalDuctSize,
                    //  ParentRouteID = y.Count()
                }
);
                dgv_2.Rows.Clear();
                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_2.Rows.Add(new object[] {
                u,
                l.SignalType,
                l.A_OptimalDuctSize,
                l.Length,
                1
              //  l.ParentRouteID,
                });
                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    ae.AssignedDuct = l.SignalType;
                    ae.DuctType = l.A_OptimalDuctSize;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = (1).ToString();
                    routeInfo.lstResult2.Add(ae);

                }
            }
        }
        private void BindFinalResult3()
        {
            // A_SegSignalType
            //Length
            //A_OptimalSize

            var dtAnalysis = dgv_InsAnalysis.DataSource as DataTable;
            if (dtAnalysis == null || dtAnalysis.Rows.Count == 0) return;
            List<InstrumentRouteInfoEntity> sfo = new List<InstrumentRouteInfoEntity>();
            foreach (DataRow dr in dtAnalysis.Rows)
            {
                InstrumentRouteInfoEntity segmentInfoEntity = new InstrumentRouteInfoEntity();
                segmentInfoEntity.CableType = dr["Type"].ToString();
                segmentInfoEntity.Length = Convert.ToDouble(dr["Length"].ToString());
                sfo.Add(segmentInfoEntity);
            }
            routeInfo.lstResult3 = new List<AnalysisResultEntity>();
            // colQuantity3.Visible = chk_SignalSegment.Checked;
            if (chk_SignalSegment.Checked)
            {

                var newList = sfo.GroupBy(x => new { x.CableType, x.Length }).Select(y => new InstrumentRouteInfoEntity()
                {
                    CableType = y.Key.CableType,
                    Length = y.Key.Length,
                    To = y.Count().ToString()
                }
      );
                dgv_3.Rows.Clear();
                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_3.Rows.Add(new object[] {
                u,
                l.CableType,
                l.Length,
                l.To
                });

                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    //ae.AssignedDuct = l.SignalType;
                    ae.CableType = l.CableType;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = l.To.ToString();
                    routeInfo.lstResult3.Add(ae);
                }
            }
            else
            {
                var newList = sfo.GroupBy(x => new { x.CableType }).Select(y => new InstrumentRouteInfoEntity()
                {
                    CableType = y.Key.CableType,
                    Length = y.Sum(x => x.Length),
                    //To = y.Count().ToString()
                }
);
                dgv_3.Rows.Clear();
                int u = 0;
                foreach (var l in newList)
                {
                    u++;
                    dgv_3.Rows.Add(new object[] {
                u,
                l.CableType,
                l.Length,
               1
                });
                    AnalysisResultEntity ae = new AnalysisResultEntity();
                    ae.No = u.ToString();
                    //ae.AssignedDuct = l.SignalType;
                    ae.CableType = l.CableType;
                    ae.TotalLength = l.Length.ToString();
                    ae.Quantity = (1).ToString();
                    routeInfo.lstResult3.Add(ae);
                }
            }

        }
        private void BindFinalSetting()
        {

            BindFinalResult1();
            BindFinalResult2();
            BindFinalResult3();
        }
        private void dgv_3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void btnDuctExport_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(DataSourceAnalysisDuctSchedule))
            {
                Directory.CreateDirectory(DataSourceAnalysisDuctSchedule);
            }

            using (var writer = new StreamWriter(DataSourceAnalysisDuctSchedule + GetFileName("AssignedDuct_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<DuctSchedule> lst = new List<DuctSchedule>();
                DuctSchedule ie;

                foreach (DataGridViewRow dr in dgv_1.Rows)
                {
                    ie = new DuctSchedule();
                    ie.No = dr.Cells["colNo1"].Value.ToString();
                    ie.AssignedDuct = dr.Cells["colAssignedDuct1"].Value.ToString();
                    ie.Length = dr.Cells["colTotalLength1"].Value.ToString();
                    ie.Quantity = dr.Cells["colQuantity1"].Value.ToString();
                    lst.Add(ie);
                }

                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();

            }
            System.Diagnostics.Process.Start(DataSourceAnalysisDuctSchedule);
        }

        private void btnDuctTypeExport_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(DataSourceAnalysisDuctTypeSchedule))
            {
                Directory.CreateDirectory(DataSourceAnalysisDuctTypeSchedule);
            }

            using (var writer = new StreamWriter(DataSourceAnalysisDuctTypeSchedule + GetFileName("AssignedDuctType_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<DuctTypeSchedule> lst = new List<DuctTypeSchedule>();
                DuctTypeSchedule ie;

                foreach (DataGridViewRow dr in dgv_2.Rows)
                {
                    ie = new DuctTypeSchedule();
                    ie.No = dr.Cells["colNo2"].Value.ToString();
                    ie.AssignedDuct = dr.Cells["colAssignedDuct2"].Value.ToString();
                    ie.Type = dr.Cells["colDuctType2"].Value.ToString();

                    ie.Length = dr.Cells["colTotalLength2"].Value.ToString();
                    ie.Quantity = dr.Cells["colQuantity2"].Value.ToString();
                    lst.Add(ie);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            System.Diagnostics.Process.Start(DataSourceAnalysisDuctTypeSchedule);
        }

        private void btnCableTypeExport_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(DataSourceAnalysisDuctCableSchedule))
            {
                Directory.CreateDirectory(DataSourceAnalysisDuctCableSchedule);
            }

            using (var writer = new StreamWriter(DataSourceAnalysisDuctCableSchedule + GetFileName("AssignedDuctCable_") + ".csv"))
            using (var csvWriter = new CsvWriter(writer))
            {

                List<DuctCableSchedule> lst = new List<DuctCableSchedule>();
                DuctCableSchedule ie;

                foreach (DataGridViewRow dr in dgv_3.Rows)
                {
                    ie = new DuctCableSchedule();
                    ie.No = dr.Cells["colNo3"].Value.ToString();
                    ie.Type = dr.Cells["colCableType3"].Value.ToString();
                    ie.Length = dr.Cells["colTotalLength3"].Value.ToString();
                    ie.Quantity = dr.Cells["colQuantity3"].Value.ToString();
                    lst.Add(ie);
                }
                csvWriter.WriteRecords(lst);
                csvWriter.Flush();
                writer.Flush();
                writer.Close();
            }
            System.Diagnostics.Process.Start(DataSourceAnalysisDuctCableSchedule);
        }

        private void chk_SignalSegment_CheckedChanged(object sender, EventArgs e)
        {
            BindFinalSetting();
            BindDecimalplace();
        }

        private List<Color> KnowColors()
        {
            List<Color> colorList = new List<Color>();
            for (int i = 0; i < 15; i++)
            {
                var c = (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), i);
                if (c.ToString().ToUpper().Contains("WH") || c.ToString().ToUpper().Contains("BL") || c.ToString().ToUpper().Contains("DA") || c.ToString().ToUpper().Contains("TEXT") || c.ToString().ToUpper().Contains("TRAN"))
                {
                    continue;
                }
                colorList.Add(Color.FromName(c.ToString()));
            }
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                Color c = Color.FromKnownColor(knownColor);
                if (c.ToString().ToUpper().Contains("WH") || c.ToString().ToUpper().Contains("BL") || c.ToString().ToUpper().Contains("DA") || c.ToString().ToUpper().Contains("TEXT") || c.ToString().ToUpper().Contains("TRAN"))

                {
                    continue;
                }
                if (!colorList.Where(x => x == c).Any())
                {
                    colorList.Add(c);
                }
            }
            return colorList;
        }
        protected int gapduct { get; set; } = 200;

        double ConnectorAquare = 0;
        private void ShowDuctWithCables()
        {
            if (routeInfo == null || vdFC1.BaseControl.ActiveDocument.FileName == null)
            {
                MessageBox.Show("경로 정보 혹은 입력 데이터가 없습니다. 데이터를 다시 확인해주세요.");

                return;
            }
            var dtAnalysis = dgv_SegAnalysis.DataSource as DataTable;
            if (dtAnalysis == null || dtAnalysis.Rows.Count == 0)
            {
                MessageBox.Show("분석된 데이터가 없습니다. 설정 확인 후 다시 분석해주세요.");
                return;
            }
            List<SegmentInfoEntity> sfo = new List<SegmentInfoEntity>();
            foreach (DataRow dr in dtAnalysis.Rows)
            {
                SegmentInfoEntity segmentInfoEntity = new SegmentInfoEntity();
                segmentInfoEntity.SignalType = dr["A_SegSignalType"].ToString();
                segmentInfoEntity.Length = Convert.ToDouble(dr["Length"].ToString());
                //segmentInfoEntity.A_OptimalDuctSize = dr["A_OptimalSize"].ToString();
                segmentInfoEntity.SegmentName = dr["Segment"].ToString();
                segmentInfoEntity.A_OptimalDuctSize = dr["A_UserDefinedSize"].ToString();

                sfo.Add(segmentInfoEntity);
            }

            var Segs = sfo.GroupBy(x => new { x.SegmentName }).Select(y => new SegmentInfoEntity()
            {
                SegmentName = y.Key.SegmentName,
                // SegmentName = y.Key.SegmentName,
            });
            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;

            foreach (var dl in routeInfo.lstDuctLines)
            {
                var f = et.FindFromId(Convert.ToInt32(dl.ductId));
                if (f != null)
                    et.RemoveItem(f);
            }
            routeInfo.lstDuctLines = new List<DuctLineEntity>();

            foreach (var sg in Segs) // SegmentGroup1/2/3/4/5 . . . 
            {
                //var seg = dgv_SegAnalysis.CurrentRow.Cells["A_SegName"].Value.ToString();
                //var segSingal = dgv_SegAnalysis.CurrentRow.Cells["A_SegSignalType"].Value.ToString();
                var segInfo = routeInfo.LstSegmentInfo.Where(c => c.SegmentName == sg.SegmentName).FirstOrDefault();
                var parentId = segInfo.ParentRouteID;

                if (Convert.ToInt32(parentId) == 0)
                {
                    continue;
                }
                var parentLine = et.FindFromId(parentId) as vdLine;
                if (parentLine == null)
                {
                    continue;
                }
                sg.StartPoint = segInfo.StartPoint.Clone() as gPoint;
                sg.EndPoint = segInfo.EndPoint.Clone() as gPoint;
                parentLine.visibility = vdFigure.VisibilityEnum.Invisible;

                //var firstHalf = DuctList.Count() / 2;

                //double increasement = 0.0;
                gPoint sp = null;
                gPoint ep = null;
                int ci = 0;
                List<DuctLineEntity> lde = new List<DuctLineEntity>();

                var AllSignalDuct = sfo.GroupBy(x => new { x.SignalType }).Select(o => new SegmentInfoEntity()
                {
                    SignalType = o.Key.SignalType,
                    ParentRouteID = o.Count()
                });


                //Sorting order by Signal keywords

                List<string> lstsignal = new List<string>();
                foreach (var s in AllSignalDuct)
                {
                    if (s.SignalType.ToLower().Contains("signal"))
                    {
                        lstsignal.Add(s.SignalType);
                    }
                }
                List<SegmentInfoEntity> tempDucts = AllSignalDuct.ToList();
                foreach (var ls in lstsignal)
                {
                    tempDucts.Remove(tempDucts.Where(x => x.SignalType == ls).FirstOrDefault());
                }
                List<SegmentInfoEntity> lstAllDuct = new List<SegmentInfoEntity>();
                foreach (var l in lstsignal)
                {
                    var dl = AllSignalDuct.Where(x => x.SignalType == l).FirstOrDefault();
                    lstAllDuct.Add(dl);
                }
                foreach (var l in tempDucts)
                {
                    if (!lstsignal.Contains(l.SignalType))
                        lstAllDuct.Add(l);
                }
                var DuctList = sfo.Where(x => x.SegmentName == sg.SegmentName);


                //Done Sorting


                // var DuctOrder = DuctList.OrderBy(x => x.SegmentName);

                foreach (var dlGroup in lstAllDuct) // power, signal, comm, etc . . . //AllSignalDuct.OrderByDescending(x => x.SignalType)
                {
                    // dindex ++
                    //ci++;
                    SegmentInfoEntity dl = null;
                    if (DuctList.Where(x => x.SignalType == dlGroup.SignalType).Any())
                    {
                        dl = DuctList.Where(x => x.SignalType == dlGroup.SignalType).FirstOrDefault();
                    }
                    DuctLineEntity ductLineEntity1 = new DuctLineEntity();  // Ducts 

                    ductLineEntity1.IsVerticle = sg.StartPoint.x == sg.EndPoint.x;

                    if (sp == null || ep == null)
                    {
                        sp = sg.StartPoint.Clone() as gPoint;
                        ep = sg.EndPoint.Clone() as gPoint;
                    }
                    if (!ductLineEntity1.IsVerticle) // Horizontal
                    {
                        sp.y += gapduct;
                        ep.y += gapduct;
                        sp.x -= gapduct;
                        ep.x -= gapduct;
                    }
                    else // Vertical
                    {
                        sp.x -= gapduct;
                        ep.x -= gapduct;
                        sp.y += gapduct;
                        ep.y += gapduct;
                    }
                    ductLineEntity1.sp = sp.Clone() as gPoint;
                    ductLineEntity1.ep = ep.Clone() as gPoint;
                    //  ductLineEntity1.colorIndex = ci; 
                    // ductLineEntity1.IsBolder = true;
                    ductLineEntity1.oline = parentLine;
                    ductLineEntity1.osp = sg.StartPoint.Clone() as gPoint;
                    ductLineEntity1.oep = sg.EndPoint.Clone() as gPoint;
                    ductLineEntity1.SignalType = dl == null ? null : dl.SignalType;
                    ductLineEntity1.SegmentName = dl == null ? null : dl.SegmentName;
                    ductLineEntity1.DuctTypeName = dl == null ? null : dl.A_OptimalDuctSize;
                    var c = dl == null ? 0 : routeInfo.LstInsRouteInfo.Where(f => f.LstSegment.Where(x => x.ToString().Split(',').Last() == dl.SegmentName && x.Split(',').First() == dl.SignalType).Any()).Count();
                    ductLineEntity1.Cables = c;  //
                    lde.Add(ductLineEntity1);
                }

                foreach (var l in lde)
                {
                    l.fp = sp.Clone() as gPoint; // limited final point 
                }
                routeInfo.lstDuctLines.AddRange(lde);
            }


            var sgrp = sfo.GroupBy(x => new { x.SignalType }).Select(y => new SegmentInfoEntity()
            {
                SignalType = y.Key.SignalType,
            });

            foreach (var dl in routeInfo.lstDuctLines)
            {
                var halfDis = 0.0;
                var Isverticle = false;
                if (dl.osp.x != dl.oep.x)
                {
                    Isverticle = false;
                    halfDis = dl.fp.y - dl.osp.y + gapduct;
                }
                else
                {
                    Isverticle = true;
                    halfDis = dl.fp.x - dl.osp.x - gapduct;
                }

                //  if (ConnectorAquare < halfDis)  ConnectorAquare = halfDis;
                // halfDis = (sgrp.Count() +1) * gapduct / 2;// Need to recheck it might disallocate // Take ColorIndex as param to Calculate distance
                if (!Isverticle)
                {
                    dl.sp.y -= halfDis / 2;
                    dl.ep.y -= halfDis / 2;
                    dl.sp.x += halfDis / 2;
                    dl.ep.x += halfDis / 2;
                }
                else
                {
                    dl.sp.x -= halfDis / 2;
                    dl.ep.x -= halfDis / 2;
                    dl.sp.y += halfDis / 2;
                    dl.ep.y += halfDis / 2;
                }
            }
            var lstKnowColors = KnowColors();

            List<(string, int)> dicLines = new List<(string, int)>();
            int cindex = 2;

            foreach (var sg in sgrp)
            {
                cindex++;
                dicLines.Add((sg.SignalType, cindex - 1));
            }
            routeInfo.dicLinesPros = dicLines;
            int v = 0;
            var MaxCables = routeInfo.lstDuctLines.Max(x => x.Cables);
            var sbl = new BL.SettingBL();
            var TileDuctlist = sbl.GetCableDuctList();
            foreach (var dl in routeInfo.lstDuctLines)
            {
                v++;
                var e = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                var pl = new vdPolyline(vdFC1.BaseControl.ActiveDocument);
                pl.VertexList.Add(dl.sp);
                pl.VertexList.Add(dl.ep);
                var cin = routeInfo.dicLinesPros.Where(x => x.Item1 == dl.SignalType).FirstOrDefault().Item2;
                pl.PenColor.FromSystemColor(lstKnowColors[cin]);
                dl.colorIndex = cin;
                if (MaxCables != 0)
                {
                    pl.PenWidth = (Convert.ToInt32(cbo_MaxLineWeight.Text) / MaxCables) * dl.Cables;
                }
                var d = pl.PenWidth;
                //pl.LineWeight = VdConstLineWeight.LW_200; // if pen is greater than 0, then LineWeight is ignored.
                pl.Update();
                var c = dl.Cables <= 1 ? dl.Cables.ToString() + " Cable" : dl.Cables.ToString() + " Cables";
                var size = "";
                if (!string.IsNullOrEmpty(dl.SignalType))
                {
                    var dz = TileDuctlist.Where(x => x.Title == dl.DuctTypeName && x.Type == dl.SignalType);
                    if (dz.Any() && dz.Count() == 1)
                    {
                        size = "(" + dz.FirstOrDefault().Title + "-" + dz.FirstOrDefault().Width + "x" + dz.FirstOrDefault().Height + ")";
                    }
                }
                pl.ToolTip = dl.SignalType + " " + size + Environment.NewLine + c;

                if (e.FindFromId(pl.Id) == null)
                {
                    routeInfo.CURRENT_MODE = eACTION_MODE.NONE;
                    e.AddItem(pl);
                    dl.ductId = pl.Id;
                }
            }

            RefreshCADSpace();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdo_defaultDuct.Checked)
                {
                    ExecuteDuct(true);
                    BindDecimalplace();
                    //ShowDuctWithCables();
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

            }
        }
        private void ShowbackGroundColor()
        {
            var lstColor = KnowColors();
            foreach (DataGridViewRow r in dgv_SegAnalysis.Rows)
            {
                var c = routeInfo.lstDuctLines.Where(x => x.SignalType == r.Cells["A_SegSignalType"].Value.ToString());
                if (c.Any())
                {
                    r.Cells["A_SegSignalType"].Style.BackColor = lstColor[c.FirstOrDefault().colorIndex];
                }
            }
            return;  // breaked and mented remaining codes for Connector Useless statement by Clietns and Jeaeun 
            //var dtAnalysis = dgv_SegAnalysis.DataSource as DataTable;
            //if (dtAnalysis == null || dtAnalysis.Rows.Count == 0) return;
            //List<SegmentInfoEntity> sfo = new List<SegmentInfoEntity>();
            //foreach (DataRow dr in dtAnalysis.Rows)
            //{
            //    SegmentInfoEntity segmentInfoEntity = new SegmentInfoEntity();
            //    segmentInfoEntity.SignalType = dr["A_SegSignalType"].ToString();
            //    segmentInfoEntity.A_TotalArea = Convert.ToDouble(dr["TotalCable"].ToString());
            //    //segmentInfoEntity.A_OptimalDuctSize = dr["A_OptimalSize"].ToString();
            //    segmentInfoEntity.SegmentName = dr["Segment"].ToString();

            //    sfo.Add(segmentInfoEntity);
            //}

            //var Segs = sfo.GroupBy(x => new { x.SegmentName }).Select(y => new SegmentInfoEntity()
            //{
            //    SegmentName = y.Key.SegmentName,
            //    Length = Convert.ToDouble(y.Count())
            //    // SegmentName = y.Key.SegmentName,
            //});
            //var total = Segs.Max(x => x.Length);
            //var maxSegName = Segs.Where(c => c.Length == total).FirstOrDefault().SegmentName;
            //var sum = sfo.Where(x => x.SegmentName == maxSegName).Sum(x => x.A_TotalArea);
            //var gapduct = 40;
            //var gapcable = gapduct / 2;
            var squarepoly = ConnectorAquare * 1.5; //(total * gapduct) + (sum * gapcable);
            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            List<gPoint> lstReservedPoints = new List<gPoint>();
            foreach (var l in routeInfo.LstSegmentInfo)
            {
                if (l.CableList.Count == 0)
                {
                    continue;
                }

                var f = et.FindFromId(l.ParentRouteID);
                if (f != null && f is vdLine v)
                {
                    //var tp = new gPoint();
                    var c1 = routeInfo.LstTBBOXes.Where(t => t.polyline.BoundingBox.PointInBox(l.StartPoint));
                    if (!c1.Any() && !lstReservedPoints.Contains(l.StartPoint))
                    {
                        var p1 = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == l.StartPoint || x.EndPoint == l.StartPoint);
                        if (p1.Count() > 1)
                        {


                            lstReservedPoints.Add(l.StartPoint);

                            if (routeInfo.DGV_LstInstrument.Where(x => x.Instrument.centerPoint.Distance2D(l.StartPoint) <= unit).Any())
                            {
                                continue;
                            }

                            var connector = new vdPolyline(vdFC1.BaseControl.ActiveDocument);
                            var t1 = new gPoint(l.StartPoint.x - squarepoly / 2, l.StartPoint.y - squarepoly / 2);
                            var t2 = new gPoint(l.StartPoint.x + squarepoly / 2, l.StartPoint.y - squarepoly / 2);
                            var t3 = new gPoint(l.StartPoint.x + squarepoly / 2, l.StartPoint.y + squarepoly / 2);
                            var t4 = new gPoint(l.StartPoint.x - squarepoly / 2, l.StartPoint.y + squarepoly / 2);
                            connector.VertexList.Add(t1);
                            connector.VertexList.Add(t2);
                            connector.VertexList.Add(t3);
                            connector.VertexList.Add(t4);
                            connector.Layer = routeInfo.MainBoundary.Layer;
                            connector.PenColor.FromSystemColor(Color.Cyan);
                            connector.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);

                            et.AddItem(connector);
                            routeInfo.lstConnector.Add(connector);
                        }

                    }
                    var c2 = routeInfo.LstTBBOXes.Where(t => t.polyline.BoundingBox.PointInBox(l.EndPoint));
                    if (!c2.Any() && !lstReservedPoints.Contains(l.EndPoint))
                    {
                        var p1 = routeInfo.LstSegmentInfo.Where(x => x.StartPoint == l.EndPoint || x.EndPoint == l.EndPoint);
                        if (p1.Count() > 1)
                        {
                            lstReservedPoints.Add(l.EndPoint);
                            if (routeInfo.DGV_LstInstrument.Where(x => x.Instrument.centerPoint.Distance2D(l.EndPoint) <= unit).Any())
                            {
                                continue;
                            }

                            var connector = new vdPolyline(vdFC1.BaseControl.ActiveDocument);
                            var t1 = new gPoint(l.EndPoint.x - squarepoly / 2, l.EndPoint.y - squarepoly / 2);
                            var t2 = new gPoint(l.EndPoint.x + squarepoly / 2, l.EndPoint.y - squarepoly / 2);
                            var t3 = new gPoint(l.EndPoint.x + squarepoly / 2, l.EndPoint.y + squarepoly / 2);
                            var t4 = new gPoint(l.EndPoint.x - squarepoly / 2, l.EndPoint.y + squarepoly / 2);
                            connector.VertexList.Add(t1);
                            connector.VertexList.Add(t2);
                            connector.VertexList.Add(t3);
                            connector.VertexList.Add(t4);
                            connector.Layer = routeInfo.MainBoundary.Layer;
                            connector.PenColor.FromSystemColor(Color.Cyan);
                            connector.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);

                            et.AddItem(connector);
                        }

                    }

                }
            }
            RefreshCADSpace();
        }
        private void rdo_CableDuct_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdo_CableDuct.Checked)
                {
                    ExecuteDuct(false);
                    ShowbackGroundColor();
                    BindDecimalplace();
                    //ShowDuctWithCables();
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

            }
        }
        private void ExecuteDuct(bool IsDefault)
        {

            var et = vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            foreach (var dl in routeInfo.lstDuctLines)
            {
                var f = et.FindFromId(Convert.ToInt32(dl.ductId));
                if (f != null)
                {
                    if (IsDefault)
                    {
                        f.visibility = vdFigure.VisibilityEnum.Invisible;
                    }
                    else
                    {
                        f.visibility = vdFigure.VisibilityEnum.Visible;
                    }
                }
            }

            foreach (var dl in routeInfo.lstConnector)
            {
                if (dl != null)
                {
                    if (IsDefault)
                    {
                        dl.visibility = vdFigure.VisibilityEnum.Invisible;
                    }
                    else
                    {
                        dl.visibility = vdFigure.VisibilityEnum.Visible;
                    }
                }
            }
            foreach (var l in routeInfo.LstSegmentInfo)
            {
                var f = et.FindFromId(l.ParentRouteID);
                if (f != null)
                {
                    if (IsDefault)
                    {
                        f.visibility = vdFigure.VisibilityEnum.Visible;
                    }
                    else
                    {
                        f.visibility = vdFigure.VisibilityEnum.Invisible;
                    }
                }

            }
            foreach (var f in routeInfo.LstAllRoute())
            {
                if (IsDefault)
                {
                    f.visibility = vdFigure.VisibilityEnum.Visible;
                }
                else
                {
                    f.visibility = vdFigure.VisibilityEnum.Invisible;
                }
            }
            RefreshCADSpace();
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            tabControl3.SelectedIndex = 3;
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            if (routeInfo == null || (t != null && t.IsAlive)) return;
            progressBar1.Visible = true;
            if (routeInfo == null || GetMainBoundary() == null)
            {
                MessageBox.Show("외각 라인 설정이 필요합니다. 외각라인 추가를 해주시거나, 외각 라인이 있는 캐드 파일을 재입력해주세요. 외각 라인이 Boundary 레이어 내에 폴리라인으로 있을 경우 자동으로 인식됩니다.");
                return;
            }

            if (thread != null) thread.Abort();
            thread = new Thread(delegate ()
            {
                try
                {

                    progressBar1.Value = 5;
                    btn_Visualize.Enabled = false;
                    rdo_defaultDuct.Checked = false;
                    rdo_CableDuct.Checked = true;
                    progressBar1.Value = 10;
                    ClearConnector();
                    progressBar1.Value = 50;
                    ShowDuctWithCables();
                    progressBar1.Value = 70;

                    ShowbackGroundColor();
                    progressBar1.Value = 90;
                    progressBar1.Value = 100;

                    MessageBox.Show("분석 결과 시각화가 성공적으로 완료되었습니다!");
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);
                    MessageBox.Show("분석 결과 시각화가 정상적으로 처리되지 않았습니다!" + Environment.NewLine + ex.Message + ex.StackTrace);
                }
                progressBar1.Value = 0;
                progressBar1.Visible = false;
                btn_Visualize.Enabled = true;
                BindDecimalplace();
            });
            thread.Start();
            this.Cursor = Cursors.Arrow;

        }

        private void tableLayoutPanel43_Paint(object sender, PaintEventArgs e)
        {

        }
        private void BindResultTables()
        {

            //  dgv_1.Rows.Clear();
            //foreach (var l in routeInfo.lstResult1)
            //{
            //    dgv_1.Rows.Add(new object[] {

            //    l.No,
            //    l.AssignedDuct,
            //    l.TotalLength,
            //    l.Quantity
            //    });
            //}

            //  dgv_2.Rows.Clear();
            //foreach (var l in routeInfo.lstResult2)
            //{
            //    dgv_2.Rows.Add(new object[] {

            //    l.No,
            //    l.AssignedDuct,
            //    l.DuctType,
            //    l.TotalLength,
            //    l.Quantity
            //    });
            //}

            //  dgv_3.Rows.Clear();
            //foreach (var l in routeInfo.lstResult3)
            //{
            //    dgv_3.Rows.Add(new object[] {

            //    l.No,
            //    l.CableType, 
            //    l.TotalLength,
            //    l.Quantity
            //    });
            //}
            // chk_SignalSegment.Checked =   routeInfo.DefaultAnalysisCheck == 1;  // wanted by client on slides of google sheets
            rdo_defaultDuct.Checked = routeInfo.DefaultAnalysisCheck == 1; // Pending PTK 
            rdo_CableDuct.Checked = routeInfo.DefaultAnalysisCheck != 1;// Pending PTK 


        }

        private void progressBar1_VisibleChanged(object sender, EventArgs e)
        {
            button5.Enabled = !progressBar1.Visible;
        }


        private void dgv_SegAnalysis_Sorted(object sender, EventArgs e)
        {
            ShowbackGroundColor();
        }

        private void dgv_InsAnalysis_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_SegAnalysis_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel40_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel20_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CheckMinimalInput()
        {
            var input = txtConnectable.Text.Trim();
            var res = int.TryParse(input, out int val);
            if (!res)
            {
                MessageBox.Show("숫자만 입력 가능합니다. 다시 입력해주세요.");
                txtConnectable.Focus();
                return;
            }
            if (val == 0 || val > 100) //(val < 300 || val > 3000)
            {
                MessageBox.Show("0에서 100 사이의 숫자만 입력 가능합니다.");
                // MessageBox.Show("300에서 3,000 사이의 숫자만 입력 가능합니다.");
                txtConnectable.Focus();
                return;
            }

            unit = val * 1000;
        }

        private void txtConnectable_Leave(object sender, EventArgs e)
        {
            CheckMinimalInput();
        }

        private void txtConnectable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                CheckMinimalInput();
            }
        }


        private void button6_Click_4(object sender, EventArgs e)
        {
            //vdFC1.BaseControl.ActiveDocument.CommandAction.Zoom("w", new gPoint(185876.34, 181222.41), new gPoint(189476.34, 175222.41));
            //RefreshCADSpace();
            //if (routeInfo != null && vdFramedControl.BaseControl.ActiveDocument != null)
            //{

            //    DuplicateItem duplicateItem = new DuplicateItem(routeInfo, vdFramedControl, true);
            //    duplicateItem.Owner = this;
            //   // PlaceLowerRight(duplicateItem);
            //    duplicateItem.Show(); 
            //    CheckDuplicate(); 
            //}

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void xtextbox_TextChanged(object sender, EventArgs e)
        {
            var l = vdFC1.BaseControl.ActiveDocument.ActiveLayOut;
            var id = routeInfo.LstallInstruments;
            if (id.Count > 0)
            {
                try
                {
                    var i = routeInfo.LstTBBOXes[0].polyline.BoundingBox;
                    // l.AddCurentZoomToHistory();
                    // l.ZoomWindow(i.UpperLeft, i.LowerRight);
                    // l.ZoomWindow( );
                    //  l.ZoomScale(278317,206267,1);
                    vdFC1.BaseControl.ActiveDocument.CommandAction.Zoom(null, null, null);
                    l.ZoomWindow(i.UpperLeft, i.LowerRight);

                    // vdFC1.BaseControl.ActiveDocument.ActiveLayer.Update();//RefreshCADSpace();
                }
                catch
                {

                }
            }
        }


        private void txtConnectable_TextChanged(object sender, EventArgs e)
        {
            //var txt = txtConnectable.Text;
            //var t = int.TryParse(txt, out int result);

            //if (!t)
            //{
            //    MessageBox.Show("Invalid input. Please try value between 0-100m.");
            //    txtConnectable.Focus();
            //    return;
            //}
            //if (result == 0 || result > 100 )
            //{
            //    MessageBox.Show("Invalid input. Please try value between 0-100m.");
            //    txtConnectable.Focus(); 
            //    return;
            //}

        }

        private void cbo_MaxLineWeight_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            PS PS = new PS();
            PS.Owner = this;
            PS.ShowDialog();
            string r = "";
            SystemBind(ref r);
            SystemBind_Layer(ref r);

            TypeBind(ref r);
            TypeBind_Layer(ref r);


        }
        static DataGridView dtMainTable = new DataGridView();
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            BindSearchedInstruments();
            txtT1.Focus();

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            BindSearchedInstruments();
            txtT2.Focus();

        }
        private void BindSearchedInstruments()
        {
            if (routeInfo == null || string.IsNullOrEmpty(vdFramedControl.BaseControl.ActiveDocument.FileName)) return;
            var t1 = txtT1.Text.Trim();
            var t2 = txtT2.Text.Trim();
            List<InstrumentInfoEntity> dtS = new List<InstrumentInfoEntity>();
            if (string.IsNullOrEmpty(t1) && string.IsNullOrEmpty(t2))
            {
                BindGrid(true);
            }
            else if (string.IsNullOrEmpty(t1) && !string.IsNullOrEmpty(t2))
            {
                foreach (var gr in routeInfo.DGV_LstInstrument)
                {
                    if (gr.T2.Contains(t2))
                    {
                        dtS.Add(gr);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(t1) && string.IsNullOrEmpty(t2))
            {
                foreach (var gr in routeInfo.DGV_LstInstrument)
                {
                    if (gr.T1.Contains(t1))
                    {
                        dtS.Add(gr);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(t1) && !string.IsNullOrEmpty(t2))
            {
                foreach (var gr in routeInfo.DGV_LstInstrument)
                {
                    if (gr.T2.Contains(t2) && gr.T1.Contains(t1))
                    {
                        dtS.Add(gr);
                    }
                }
            }
            try
            {
                CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[instDGV.DataSource];
                currencyManager1.SuspendBinding();


                //foreach (var gr in dtS)
                //{
                foreach (DataGridViewRow r in instDGV.Rows)
                {
                    if (dtS.Count > 0)
                    {
                        var res = dtS.Where(x => x.T1 == r.Cells["colT1"].Value.ToString() && x.T2 == r.Cells["colT2"].Value.ToString());
                        if (res.Any())
                        {
                            r.Visible = true;
                        }
                        else
                        {
                            r.Visible = false;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(t1) || !string.IsNullOrEmpty(t2))
                        {
                            r.Visible = false;
                        }
                        else
                        {
                            r.Visible = true;
                        }
                    }
                }
                    
                //}
                //instDGV.Update();
                //foreach (var r in lstR)
                //{
                //    r.Visible = false;
                //}


                currencyManager1.ResumeBinding();
            }
            catch (Exception ex)
            {
                var exc = ex;
            }
            //DataGridView dtS = new DataGridView();
            //foreach (DataGridViewRow gr in dtMainTable.Rows)
            //{

            //}
            //instDGV = dtS;
            //instDGV.Update(); 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[instDGV.DataSource];
                currencyManager1.SuspendBinding();


                List<DataGridViewRow> lstR = new List<DataGridViewRow>();
                foreach (DataGridViewRow gr in instDGV.Rows)
                {
                    lstR.Add(gr);
                }
                instDGV.Update();
                foreach (var r in lstR)
                {
                    r.Visible = false;
                }
                currencyManager1.ResumeBinding();
            }
            catch (Exception ex)
            {
                var exc = ex;
            }
        }
        private void VisibleAll()
        {
            //txtT1.Text = "";
            //txtT2.Text = "";
            //foreach (DataGridViewRow gr in instDGV.Rows)
            //{
            //    gr.Visible = true;
            //}
        }
        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
            //try
            //{
            //    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[instDGV.DataSource];
            //    currencyManager1.SuspendBinding();


            //    List<DataGridViewRow> lstR = new List<DataGridViewRow>();
            //    foreach (DataGridViewRow gr in instDGV.Rows)
            //    {
            //        lstR.Add(gr);
            //    }
            //    // instDGV.Update();
            //    foreach (var r in lstR)
            //    {
            //     //   r.Visible = checkBox1.Checked;
            //    }
            //    currencyManager1.ResumeBinding();
            //}
            //catch (Exception ex)
            //{
            //    var exc = ex;
            //}
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            string Msg = "그려진 경로를 모두 제거하시겠습니까?";
            var resul = MessageBox.Show(Msg, "", MessageBoxButtons.YesNo);
            if (resul == DialogResult.Yes)
            {
                var allLine = routeInfo.LstAllRoute();
                foreach (var l in allLine)
                {
                    routeInfo.SyncronyzeRelatedUI(new List<int> { l.Id }, vdFramedControl, true);
                }
            } 
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

            this.vdFC1.BaseControl.ActiveDocument.EntitySelectMode = PickEntityMode.DrawOrder;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            this.vdFC1.BaseControl.ActiveDocument.EntitySelectMode = PickEntityMode.Auto;

        }

        private void button22_Click(object sender, EventArgs e)
        {
            this.vdFC1.BaseControl.ActiveDocument.EntitySelectMode = PickEntityMode.Closest;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            this.vdFC1.BaseControl.ActiveDocument.EntitySelectMode = PickEntityMode.EyeNearest;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            var d = vdFC1.BaseControl.ActiveDocument.GetGripSelection();
            if (d.Count > 0)
            {
                MessageBox.Show(d[0].Id.ToString());
            }
            var f = vdFC1.BaseControl.ActiveDocument.Selections;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            var d = vdFC1.BaseControl.ActiveDocument.GetGripSelection();
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdChangeOrder(d[0] as vdFigure, false );
            RefreshCADSpace();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            var d = vdFC1.BaseControl.ActiveDocument.GetGripSelection();
            vdFC1.BaseControl.ActiveDocument.CommandAction.CmdChangeOrder(d[0] as vdFigure, true);
            RefreshCADSpace();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type");
            dt.Columns.Add("system");
            dt.Columns.Add("destination");

            foreach (DataGridViewRow dr in instDGV.Rows)
            {
                var d = dr.Cells["colSystem"].Value;
                dt.Rows.Add(
                    dr.Cells["colType"].Value,
                    dr.Cells["colSystem"].Value,
                    dr.Cells["colTo"].Value 
                    ); ;
            }
        }
    }
}