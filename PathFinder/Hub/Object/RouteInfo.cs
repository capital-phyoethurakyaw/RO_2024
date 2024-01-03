using RouteOptimizer.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using RouteOptimizer;
using static RouteOptimizer.OPERATION_MODE;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Professional.vdCollections;
using System.Windows.Forms;
using VectorDraw.Professional.Constants;
using RouteOptimizer.XMLBuffer;
using RouteOptimizer.Analysis1;
using System.Windows;
//using System.Drawing;

namespace RouteOptimizer.Object
{
    public class RouteInfo : CF.CommonFunction,ICloneable
    {
        #region Members
        public string TAGNAME = "TAG No.";
        public string SYSTEMABBRE = "SystemReservedLayer_";
        public string SEGMENTABBRE = "SG_";
        public string IOABBRE = "";//"IO-";
        public string TBABBRE = "";//"TB-";
        public string xgap = "";
        public string ygap = "";
        public int MaxPoly = 50;
        public int MaxFrequency { get; set; } =400;
        public int BoxWidth = 1000;
        public int BoxHeight = 1000;

        public string IOcolor = "Red";
        public string TBColor = "Cyan";
        public string MCCcolor = "DarkBlue";

        public int DefaultDuctCheck { get; set; } =1;
        public int DefaultAnalysisCheck { get; set; }= 1;

        public bool DoneAnalysis { get; set; } =false;
        public string DetectedObstacles { get; set; } = "";
        public int SelectedDestination { get; set; } = 0;
        public int SelectedMainBoundary { get; set; } = 0;
        public int SelectedGUID = 0;
        public string SystemFile = null;
        public int PreviousVdFigure = 0;

        //XML generic error  
        public Dictionary<vdLine, gPoints> AllRouteCollections = new Dictionary<vdLine, gPoints>() { };
        public Dictionary<vdLine, List<gPoint>> AllNodeInEachDuct = new Dictionary<vdLine, List<gPoint>>() { };
        public List<Tuple<vdInsert, vdLeader>> AllInstrument_Leader = new List<Tuple<vdInsert, vdLeader>>();
        //XML generic error 
        
        public vdPolyline OffSetBoundary = null;// SK
        public vdPolyline MainBoundary = null;

        public List<int> LstEraseSelectedObjectID = null; //SK
        public List<vdFigure> LstTextName_Cricle_Cache = new List<vdFigure>() { };
        public List<DictonaryTuple_ID_Exchanger> LstLayerAllColumn = new List<DictonaryTuple_ID_Exchanger>();


        //public List<vdLine> LstGuidedRoute = new List<vdLine>() { };// new
        //public List<vdLine> LstAutoRoute = new List<vdLine>() { };//new
        //public List<vdLine> LstAutoGuidedRoute = new List<vdLine>() { };//new

        public List<vdLine> LstGuidedRoute = new List<vdLine>() { };// new
        public List<vdLine> LstAutoRoute = new List<vdLine>() { };//new
        public List<vdLine> LstAutoGuidedRoute = new List<vdLine>() { };//new


        public List<vdText> LstGuidedRoute_Label = new List<vdText>() { };// new
        public List<vdText> LstAutoRoute_Label = new List<vdText>() { };//new
        public List<vdText> LstAutoGuidedRoute_Label = new List<vdText>() { };//new
        public  eSCOPE_MODE DEFAULT_OPTION { get; set; } =  eSCOPE_MODE.USERDEFINED_ROUTE;//new   0=> udr, 1=>ar, 2=>agr 

        //public List<SegmentDetail> LstGuidedRoute = new List<SegmentDetail>() { };// new
        //public List<SegmentDetail> LstAutoRoute = new List<SegmentDetail>() { };//new
        //public List<SegmentDetail> LstAutoGuidedRoute = new List<SegmentDetail>() { };//new 

        public List<vdLine> LstExternalRoute = new List<vdLine>() { };
        public List<vdLine> LstAllRouteDrawn = new List<vdLine>();
        public List<vdFigure> LstHighlightedRoute = new List<vdFigure>(); // SK
        public List<vdFigure> LstHighlightedDestination = new List<vdFigure>();//SK

        //public List<IGroup> LstUserBoundary = new List<IGroup>(); 

        public List<Obstacle> LstObstacles = new List<Obstacle>();

        //Get normal input by getting commonfunction.cs
        public List<Layer> LstLayer = new List<Layer>();  // Not put URL   SK
        public List<Instrument> LstallInstruments = new List<Instrument>(); // Not Put URL  SK

        //Get poly.Id, TxtId, MainRouteCollection, GridIndex   
        public List<TBBOXDestination> LstTBBOXes = new List<TBBOXDestination>(); 
        //Get all from selected instrument
        public List<InstrumentInfoEntity> DGV_LstInstrument = new List<InstrumentInfoEntity>();
   
        public List<SegmentInfoEntity> LstSegmentInfo = new List<SegmentInfoEntity>() { };
        public List<SegmentInfoEntity> LstSelectedSegmentInfo = new List<SegmentInfoEntity>() { };
        public List<InstrumentRouteInfoEntity> LstInsRouteInfo = new List<InstrumentRouteInfoEntity>() { };
      
        public BasicSettingEntity BasicInfo = new BasicSettingEntity();

        public List<Routes> CaseRoutes { get; set; }
        public List<Routes> CaseRoutes_WithRoute { get; set; }
         

        public eACTION_MODE CURRENT_MODE { get; set; } = eACTION_MODE.INSTRUMENT; //SK

     

        public List<DuctLineEntity> lstDuctLines = new List<DuctLineEntity>();
        public List<AnalysisResultEntity> lstResult1 = new List<AnalysisResultEntity>();
        public List<AnalysisResultEntity> lstResult2 = new List<AnalysisResultEntity>();
        public List<AnalysisResultEntity> lstResult3 = new List<AnalysisResultEntity>();
        //public Dictionary<string, Color> lstDuctColors = new Dictionary<string, Color>();
       public List<(string, int)> dicLinesPros = new List<(string, int)>();
       public List<vdPolyline> lstConnector = new List<vdPolyline>();

        #endregion

        #region EnumerableType
        public enum eSCOPE_MODE : short
        { 
            USERDEFINED_ROUTE,
            AUTO_ROUTE,
            AUTO_GUIDED_ROUTE,
            IDLE,
        }
        public eVDFigure CURRENT_FIGURE { get; set; } = eVDFigure.POLYLINE;
        public enum eROUTE_MODE : short
        {
            NOROUTE,
            ONEROUTE,
            MULTIPLE,
            NONE
        }
        public enum eACTION_MODE : short
        {
            INSTRUMENT,
            OBSTACLE,
            DESTINSTION,
            BOUNDARY,
            MAINROUTE,
            ERASE,
            LINE,
            MOVE,
            UNDO,
            REDO,
            NONE
        }
        public enum eVDFigure : short
        {
            POLYLINE,
            RECTANGLE,
            CIRCLE,
            IRREGULAR,
            NONE
        }
        public enum eDestinationType : short
        {
            TBBox,
            IORoom,
            MCC
        }
        #endregion
        #region ID_Changer
        public XMLBuffer.ID_Exchanger GetID_Changer(vdControls.vdFramedControl vdFramedControl, RouteInfo routeInfo, DataGridView instListbox = null, ListBox ObstListbox = null)
        {
            try
            {
                var e = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                var ID_changer = new XMLBuffer.ID_Exchanger();
                ID_changer.dicLinesPros = dicLinesPros;
                ID_changer.SYSTEMABBRE = SYSTEMABBRE;
                ID_changer.SEGMENTABBRE = SEGMENTABBRE;
                ID_changer.IOABBRE = IOABBRE;
                ID_changer.TBABBRE = TBABBRE;
                ID_changer.xgap = xgap;
                ID_changer.ygap = ygap;
                ID_changer.MaxPoly = MaxPoly;
                ID_changer.DetectedObstacles = DetectedObstacles;
                ID_changer.SelectedDestination = SelectedDestination;
                ID_changer.SelectedMainBoundary = SelectedMainBoundary;
                ID_changer.SelectedGUID = SelectedGUID;
                ID_changer.SystemFile = SystemFile;
                ID_changer.PreviousVdFigure = PreviousVdFigure;
                ID_changer.BoxWidth = BoxWidth;
                ID_changer.BoxHeight = BoxHeight;

                ID_changer.IOcolor = IOcolor;
                ID_changer.MCCcolor = MCCcolor;
                ID_changer.TBColor = TBColor;


                List<DictonaryTuple_ID_Exchanger> resAllRouteCollection = new List<DictonaryTuple_ID_Exchanger>() { };

                foreach (var vf in AllRouteCollections)
                {
                    var die = new DictonaryTuple_ID_Exchanger();
                    die.polyID = GenerateEmbeddedURLId(vf.Key);
                    die.gpointsID = vf.Value;
                    resAllRouteCollection.Add(die);
                }
                ID_changer.AllRouteCollections = resAllRouteCollection;

                List<DictonaryTuple_ID_Exchanger> resAllNodeInEachDuct = new List<DictonaryTuple_ID_Exchanger>();
                foreach (var vf in AllNodeInEachDuct)
                {
                    var die = new DictonaryTuple_ID_Exchanger();
                    die.polyID = GenerateEmbeddedURLId(vf.Key);
                    die.LstgPointID = vf.Value;
                    resAllNodeInEachDuct.Add(die);
                }
                ID_changer.AllNodeInEachDuct = resAllNodeInEachDuct;


                List<DictonaryTuple_ID_Exchanger> resAllInstrument_Leader = new List<DictonaryTuple_ID_Exchanger> { };
                foreach (var arc in AllInstrument_Leader)
                {
                    var die = new DictonaryTuple_ID_Exchanger();
                    die.insertID = GenerateEmbeddedURLId(arc.Item1);
                    die.leaderID = GenerateEmbeddedURLId(arc.Item2);
                    resAllInstrument_Leader.Add(die);
                }
                ID_changer.AllInstrument_Leader = resAllInstrument_Leader;

                ID_changer.MainBoundary = GenerateEmbeddedURLId(MainBoundary);

                var lcc = new List<string>();
                foreach (var c in LstTextName_Cricle_Cache)
                {
                    lcc.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.LstTextName_Cricle_Cache = lcc;

                var ler = new List<string>();
                foreach (var c in LstExternalRoute)
                {
                    ler.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.LstExternalRoute = ler;


                var connector = new List<string>();
                foreach (var c in lstConnector)
                {
                    connector.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.lstConnector = connector;

                var lad = new List<string>();
                foreach (var c in LstAllRouteDrawn)
                {
                    lad.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.LstAllRouteDrawn = lad;


                var lo = new List<string>();
                foreach (var l in LstObstacles)
                {
                    lo.Add(GenerateEmbeddedURLId(l.vdObstacle));
                }
                ID_changer.LstObstacles = lo;

                LstallInstruments = GetAllInstrument(vdFramedControl, this, instListbox, ObstListbox);

                var lstselectedIns = new List<string>();

                foreach (var g in LstLayer)
                {
                    if (g.SelectedInInstrument)
                    {
                        lstselectedIns.Add(g.name);

                    }
                }
                ID_changer.LstLayerSelectedInstruments = lstselectedIns;


                var lstlayerAllCols = new List<DictonaryTuple_ID_Exchanger>();
                foreach (DataGridViewRow dr in instListbox.Rows)
                {
                    var die = new DictonaryTuple_ID_Exchanger();
                    die.LayerName = dr.Cells["dgv_Layer"].EditedFormattedValue.ToString();
                    die.System = dr.Cells["dgv_System"].EditedFormattedValue.ToString();
                    die.Type = dr.Cells["dgv_Type_Ins"].EditedFormattedValue.ToString();
                    var val = dr.Cells["Cable1"].Value == null ? "" : dr.Cells["Cable1"].Value.ToString();
                    if (!string.IsNullOrEmpty(val) && val != "-1")
                        die.To = GenerateEmbeddedURLId(e.FindFromId(Convert.ToInt32(val)));
                    lstlayerAllCols.Add(die);

                }
                ID_changer.LstLayerAllColumn = lstlayerAllCols;

                var lstselectedObs = new List<string>();
                foreach (var g in LstLayer)
                {
                    if (g.SelectedInObstacle)
                        lstselectedObs.Add((g).name);
                }
                ID_changer.LstLayerSelectedObstacles = lstselectedObs;


                var ri = new List<TB_ID_Exchanger>();

                foreach (var tb in LstTBBOXes)
                {
                    var tie = new TB_ID_Exchanger();
                    tie.IsIO = (short)tb.IsIO;
                    tie.vdText = GenerateEmbeddedURLId(e.FindFromId(tb.NameId));
                    tie.vdpolyline = GenerateEmbeddedURLId(tb.polyline);
                    var lstRoute = new List<string>();
                    foreach (var r in tb.MainRouteCollection)
                    {
                        lstRoute.Add(GenerateEmbeddedURLId(r));
                    }
                    var mce = new List<MCCEntity>();//
                    foreach (var mCC in tb.LstmCCEntities)
                    {
                        var me = new MCCEntity()
                        {
                            Title = mCC.Title,
                            Description = mCC.Description,
                            CableSpecifications = mCC.CableSpecifications,
                            Status = mCC.Status,
                            TagNo = mCC.TagNo,
                            Signal = mCC.Signal,
                            SignalType = mCC.SignalType,
                            EquipmentName = mCC.EquipmentName
                        };
                        mce.Add(me);
                    }
                    tie.LstmCCEntities = mce;

                    var mceHeader = new List<MCCEntity>();//
                    foreach (var mCC in tb.LstmCCEntitiesHeader)
                    {
                        var me = new MCCEntity()
                        {
                            Title = mCC.Title,
                            Description = mCC.Description,
                            CableSpecifications = mCC.CableSpecifications,
                            Status = mCC.Status,
                            TagNo = mCC.TagNo,
                            Signal = mCC.Signal,
                            SignalType = mCC.SignalType,
                            EquipmentName = mCC.EquipmentName
                        };
                        mceHeader.Add(me);
                    }
                    tie.LstmCCEntitiesHeader = mceHeader;

                    tie.MainRoute = lstRoute;
                    if (tb.OwnDestination != null)
                        tie.OwnDestination = GenerateEmbeddedURLId(tb.OwnDestination.polyline);

                    ////var rl = new List<string>();
                    ////foreach (var r in tb.routeLines)
                    ////{
                    ////    rl.Add(GenerateEmbeddedURLId(r));
                    ////} 
                    ////tie.routeLines = rl; 
                    ////tie.mMatrix = tb.mMatrix;
                    ////tie.mps = tb.mps;
                    ////tie.mp = tb.mp;
                    ////var lstins = new List<string>();
                    ////foreach (var r in tb.lstInstrument)
                    ////{  
                    ////    lstins.Add( GenerateEmbeddedURLId(r.OwnerInsert)); 
                    ////}
                    ////tie.lstInstrument = lstins; 
                    tie.CableType = tb.CableType;
                    tie.AutoCheck = tb.AutoCheck;
                    tie.EachCheck = tb.EachCheck;
                    ri.Add(tie);
                }
                ID_changer.LstTBBOXes = ri;

                var lstdgv = new List<InsInfo_ID_Exchanger>() { };
                foreach (var dgvi in DGV_LstInstrument)
                {
                    InsInfo_ID_Exchanger instrumentInfoEntity = new InsInfo_ID_Exchanger();
                    instrumentInfoEntity.Instrument = GenerateEmbeddedURLId(dgvi.Instrument.OwnerInsert);
                    instrumentInfoEntity.GUID = dgvi.Instrument.guid;
                    instrumentInfoEntity.T1 = dgvi.T1;
                    instrumentInfoEntity.T2 = dgvi.T2;
                    instrumentInfoEntity.Type = dgvi.Type;
                    instrumentInfoEntity.System = dgvi.System;
                    instrumentInfoEntity.To = GenerateEmbeddedURLId(e.FindFromId(dgvi.To));
                    instrumentInfoEntity.Classification_1 = dgvi.Classification_1;
                    instrumentInfoEntity.Classification_2 = dgvi.Classification_2;
                    instrumentInfoEntity.Classification_3 = dgvi.Classification_3;
                    instrumentInfoEntity.Cable = dgvi.Cable;
                    instrumentInfoEntity.CHK_AutoRoute = dgvi.CHK_AutoRoute;
                    instrumentInfoEntity.CHK_EachRoute = dgvi.CHK_EachRoute;
                    var lce = new InstCableEntity();

                    // instrumentInfoEntity.LstInstCableEntity = dgvi.LstInstCableEntity;
                    List<InstCableEntity> lins = new List<InstCableEntity>();
                    lins = dgvi.LstInstCableEntity;
                    foreach (var l in lins)
                    {

                        if (!string.IsNullOrEmpty(l.To))
                        {
                            try
                            {
                                if (l.To.Contains(".wyzrs")) continue;
                                l.To = GenerateEmbeddedURLId(e.FindFromId(Convert.ToInt32(l.To)));
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.Message;
                            }
                        }

                    }
                    instrumentInfoEntity.LstInstCableEntity = lins;
                    instrumentInfoEntity.LayerName = dgvi.LayerName;
                    lstdgv.Add(instrumentInfoEntity);
                }
                ID_changer.DGV_LstInstrument = lstdgv;

                var lsi = new List<Segment_ID_Exchanger>();
                foreach (var l in LstSegmentInfo)
                {
                    var Se = new Segment_ID_Exchanger();
                    Se.SegmentName = l.SegmentName;
                    Se.StartPoint = l.StartPoint;
                    Se.EndPoint = l.EndPoint;
                    Se.Length = l.Length;
                    Se.ParentRouteID = GenerateEmbeddedURLId(e.FindFromId(l.ParentRouteID)); //l.ParentRouteID;
                    Se.CableList = l.CableList;
                    if (l.ParentDestination != null)
                        Se.ParentDestination = GenerateEmbeddedURLId(l.ParentDestination.polyline);// routeInfo.LstTBBOXes.Where(x => x.polyline.URL == l.ParentDestination).FirstOrDefault();
                    Se.SignalType = l.SignalType;
                    Se.A_OptimalDuctSize = l.A_OptimalDuctSize;
                    Se.A_ActualOptimalResult = l.A_ActualOptimalResult;
                    Se.A_OptimalRatio = l.A_OptimalRatio;
                    Se.A_TotalArea = l.A_TotalArea;
                    Se.A_UserDefinedRato = l.A_UserDefinedRato;
                    Se.A_UserDefinedSize = l.A_UserDefinedSize;

                    lsi.Add(Se);
                }
                ID_changer.LstSegmentInfo = lsi;

                var lsi2 = new List<Segment_ID_Exchanger>();
                foreach (var l in LstSelectedSegmentInfo)
                {
                    var Se = new Segment_ID_Exchanger();
                    Se.SegmentName = l.SegmentName;
                    Se.StartPoint = l.StartPoint;
                    Se.EndPoint = l.EndPoint;
                    Se.Length = l.Length;
                    Se.ParentRouteID = GenerateEmbeddedURLId(e.FindFromId(l.ParentRouteID)); //l.ParentRouteID;
                    Se.CableList = l.CableList;
                    if (l.ParentDestination != null)
                        Se.ParentDestination = GenerateEmbeddedURLId(l.ParentDestination.polyline);// routeInfo.LstTBBOXes.Where(x => x.polyline.URL == l.ParentDestination).FirstOrDefault();
                    Se.A_OptimalDuctSize = l.A_OptimalDuctSize;
                    Se.A_ActualOptimalResult = l.A_ActualOptimalResult;
                    Se.A_OptimalRatio = l.A_OptimalRatio;
                    Se.A_TotalArea = l.A_TotalArea;
                    Se.A_UserDefinedRato = l.A_UserDefinedRato;
                    Se.A_UserDefinedSize = l.A_UserDefinedSize;

                    Se.SignalType = l.SignalType;
                    lsi2.Add(Se);
                }
                ID_changer.LstSelectedSegmentInfo = lsi2;

                var lri = new List<RouteInfo_ID_Exchanger>();
                foreach (var l in LstInsRouteInfo)
                {
                    var re = new RouteInfo_ID_Exchanger();
                    if (l.Instrument.OwnerInsert != null)
                        re.Instrument = GenerateEmbeddedURLId(l.Instrument.OwnerInsert); /// May be instrument or Destination
                    else
                        re.Instrument = GenerateEmbeddedURLId(LstTBBOXes.Where(x => x.Name == l.Instrument.t1).FirstOrDefault().polyline);
                    re.CableType = l.CableType;
                    re.LstSegment = l.LstSegment;
                    re.Length = l.Length;
                    re.To = l.To;
                    re.SignalType = l.SignalType;
                    re.SystemType = l.SystemType;
                    lri.Add(re);
                }
                ID_changer.LstInsRouteInfo = lri;
                // new



                var lgr = new List<Connector_ID_Exchanger>();
        
                foreach (var c in LstGuidedRoute)
                {
                    Connector_ID_Exchanger connector_ID_Exchanger = new Connector_ID_Exchanger();
                    connector_ID_Exchanger.connectorId = GenerateEmbeddedURLId(c);
                    connector_ID_Exchanger.sp = new Point(c.StartPoint.x, c.StartPoint.y);
                    connector_ID_Exchanger.ep = new Point(c.EndPoint.x, c.EndPoint.y);
                    lgr.Add(connector_ID_Exchanger);
                }
                ID_changer.LstGuidedRoute = lgr;



                var lar = new List<Connector_ID_Exchanger>();
                foreach (var c in LstAutoRoute)
                {
                    Connector_ID_Exchanger connector_ID_Exchanger = new Connector_ID_Exchanger();
                    connector_ID_Exchanger.connectorId = GenerateEmbeddedURLId(c);
                    connector_ID_Exchanger.sp = new Point(c.StartPoint.x, c.StartPoint.y);
                    connector_ID_Exchanger.ep = new Point(c.EndPoint.x, c.EndPoint.y);
                    lar.Add(connector_ID_Exchanger);
                    //lar.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.LstAutoRoute = lar;

 

                var lag = new List<Connector_ID_Exchanger>();
                foreach (var c in LstAutoGuidedRoute)
                {
                    Connector_ID_Exchanger connector_ID_Exchanger = new Connector_ID_Exchanger();
                    connector_ID_Exchanger.connectorId = GenerateEmbeddedURLId(c);
                    connector_ID_Exchanger.sp = new Point(c.StartPoint.x, c.StartPoint.y);
                    connector_ID_Exchanger.ep = new Point(c.EndPoint.x, c.EndPoint.y);
                    lag.Add(connector_ID_Exchanger);
                    // lag.Add(GenerateEmbeddedURLId(c));
                }
                ID_changer.LstAutoGuidedRoute = lag;




                var rie = new List<Route_ID_ExChanger>();
                if (CaseRoutes == null) CaseRoutes = new List<Routes>();
                foreach (var cr in CaseRoutes)
                {
                    var rts = new Route_ID_ExChanger();
                    rts.routeID = cr.routeID;
                    var lcie = new List<Connector_ID_Exchanger>();
                    foreach (var ciex in cr.connectors)
                    {
                        var cie = new Connector_ID_Exchanger();
                        cie.connectorId = ciex.connectorID;
                       // cie.line= GenerateEmbeddedURLId(ciex.line);
                        cie.sp = new Point(ciex.line.StartPoint.x, ciex.line.StartPoint.y); 
                        cie.ep = new Point(ciex.line.EndPoint.x, ciex.line.EndPoint.y);
                        lcie.Add(cie);
                    }
                    rts.bend = cr.bend;
                    rts.connectors = lcie;
                    rie.Add(rts); 

                }
                ID_changer.CaseRoutes = rie;



                var ade = new List<Ana_DuctLine_Exchanger>();
                foreach (var c in lstDuctLines)
                {
                    Ana_DuctLine_Exchanger ad = new Ana_DuctLine_Exchanger(); 
                    ad.SegmentName =  c.SegmentName;
                    ad.SignalType =  c.SignalType;
                    var et = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities;
                    if (et.FindFromId(c.ductId) != null)
                    ad.ductId = GenerateEmbeddedURLId(et.FindFromId(c.ductId)); //c.ductId; 
                    ad.sp = new Point(c.sp.x,c.sp.y);
                    ad.ep = new Point(c.ep.x,c.ep.y);
                    ad.colorIndex = c.colorIndex;
                    ad.oline= GenerateEmbeddedURLId(c.oline);
                    ad.osp = new Point(c.osp.x, c.osp.y);
                    ad.oep = new Point(c.oep.x, c.oep.y);
                    ad.fp = new Point(c.fp.x, c.fp.y);
                    ad.IsVerticle = c.IsVerticle ? 1 : 0;
                    ad.IsBolder = c.IsBolder ? 1 : 0;
                    ad.Cables = c.Cables;
                    ad.DuctTypeName = c.DuctTypeName;
                    ade.Add(ad); 
                }
                ID_changer.lstDuctLines = ade;


                ID_changer.lstResult1 = lstResult1;
                ID_changer.lstResult2 = lstResult2;
                ID_changer.lstResult3 = lstResult3;


                ID_changer.DefaultAnalysisCheck = DefaultAnalysisCheck;
                ID_changer.DefaultDuctCheck = DefaultDuctCheck;
                ID_changer.DEFAULT_OPTION = (int)DEFAULT_OPTION;

                ID_changer.basicSettingEntity = BasicInfo;
                return ID_changer;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex); 
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            
        }
        public string GenerateEmbeddedURLId(vdFigure vdFigure )
        {
            if (vdFigure == null) return null;
            try
            {
                Guid guid = Guid.NewGuid();
                var urlID = guid.ToString().Replace("-", "") + ".wyzrs";
                if (vdFigure.URL == null || vdFigure.URL == "")
                {
                    vdFigure.URL = urlID;
                    vdFigure.Update();
                }
                else
                    urlID = vdFigure.URL;
                return urlID;
            }
            catch
            {
                return null;
            }
      
        }
        #endregion
        #region HelperMethods
        public Layer FinLayer(string Name)
        {
            if (LstLayer.Count > 0)
            {
                return LstLayer.Where(n => n.name == Name).FirstOrDefault();
            }
            return null;
        }
        public void UpdateLayerItem(Layer layer, Instrument instrument, EOperationMode eOperationMode)
        {
            switch (eOperationMode)
            {
                case EOperationMode.INSERT:
                    {
                        var lyr = FinLayer(layer.name);
                        lyr.listInstrument.Add(instrument);
                        break;
                    }
                case EOperationMode.DELETE:
                    {
                        var lyr = FinLayer(layer.name);
                        lyr.listInstrument.Remove(instrument);
                        break;
                    }
            }
        }
        //public bool CheckIORoomPersisted()
        //{
        //    return LstTBBOXes.Where(x => x.IsIO == true).Any();

        //}
        public TBBOXDestination FindTBBox(int Id)
        {
            foreach (var box in LstTBBOXes)
                if (box.guid == Id)
                    return box;
            return null;
        }
        public TBBOXDestination FindIO_TBBox(int Id)
        {
            foreach (var box in LstTBBOXes)
                if (box.guid == Id)
                    return box;
            return null;
        }
        public InstrumentInfoEntity FindDGV_Instrument(int Id)
        {
            foreach (var box in DGV_LstInstrument)
                if (box.GUID == Id)
                    return box;
            return null;
        }
        public void ExecuteBoundaryDM(vdPolyline vdPolyline, Array vdEntitie, ref bool IsLegal)
        {
            vdEntities vdEntities = new vdEntities();
            foreach (vdFigure e in vdEntitie)
                vdEntities.AddItem(e);

            if (CheckLegalDrawn(vdPolyline, vdEntities))
            {
                //IGroup userGroup = new IGroup(vdPolyline, LstallInstruments);
                //LstUserBoundary.Add(userGroup);
            }
            else
                IsLegal = false;
        }
        public void ExecuteDestinationDM_IO(vdPolyline vdPolyline)
        {
            //foreach (var lst in LstTBBOXes)
            //    if (lst.polyline.Id == vdPolyline.Id) return;
            ////if (FindIORoom(vdPolyline.Id) == null || FindIO_TBBox(vdPolyline.Id) == null)
            ////{
            //TBBOXDestination tBBOX = new TBBOXDestination(vdPolyline, true);
            //tBBOX.SetName(IOABBRE + "Room", 0);
            //LstTBBOXes.Add(tBBOX);
            ////}
        }
        public void ExecuteDestinationDM(vdPolyline vdPolyline, Array vdE, ref bool IsLegal)
        {
            vdEntities vdEntities = new vdEntities();
            foreach (vdFigure e in vdE)
                vdEntities.AddItem(e);
            if (CheckLegalDrawnDestination(vdPolyline, vdEntities))
            {
                TBBOXDestination tBBOX = new TBBOXDestination(vdPolyline);
                LstTBBOXes.Add(tBBOX);
            }
            else
                IsLegal = false;
        }
        private bool CheckLegalDrawnDestination(vdPolyline vdPolyline, vdEntities vdEntities)
        {
            return true;// ptk added for Jeaeun request due to much element on Xref

            if (!CheckPingPong(vdPolyline, vdEntities))
                return false;
            foreach (var ins in LstallInstruments)
                if (vdPolyline.BoundingBox.PointInBox(ins.centerPoint)) return false;
            return true;
        }
        private bool CheckPingPong(vdPolyline vdPolyline, vdEntities vdEntities, bool IsObstacle = false, bool IsBoundary = false)
        {
            foreach (vdFigure f in vdEntities)
            {
                if (f != null && f.Deleted)  continue;

                if (IsBoundary)
                {
                    if (f.Id != vdPolyline.Id)
                    {
                        gPoints gPoint = new gPoints();
                        if (f.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
                        {
                            return false;
                        }

                        foreach (gPoint p1 in f.BoundingBox.GetPoints())
                        {
                            if (!vdPolyline.BoundingBox.PointInBox(p1) && !(f is vdDimension))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (MainBoundary != null)
                    {
                        if (f.Id != MainBoundary.Id)
                        {
                            bool IsMain = false;
                            foreach (var m in LstTBBOXes)
                            {
                                if (m.MainRouteCollection.Count > 0)
                                {
                                    if (m.MainRouteCollection.Where(c => c.Id == f.Id).Any()) //if (f.Id == m.MainRoute.Id)
                                    {
                                        IsMain = true;
                                        break;
                                    }
                                }
                            }
                            if (!IsMain)
                            {
                                if (f.Id != vdPolyline.Id)
                                {
                                    gPoints gPoint = new gPoints();
                                    if (!IsObstacle)
                                        if (f.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
                                        {
                                            return false;
                                        }
                                    foreach (gPoint p1 in f.BoundingBox.GetPoints())
                                    {
                                        if (vdPolyline.BoundingBox.PointInBox(p1))
                                        {
                                            return false;
                                        }
                                    }

                                    //foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
                                    //{
                                    //    if (f.BoundingBox.PointInBox(p1))
                                    //        return false;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            gPoints gPoint = new gPoints();
                            if (MainBoundary.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
                            {
                                return false;
                            }
                            foreach (gPoint p1 in MainBoundary.BoundingBox.GetPoints())
                            {
                                if (vdPolyline.BoundingBox.PointInBox(p1))
                                {
                                    return false;
                                }
                            }

                            foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
                            {
                                if (!MainBoundary.BoundingBox.PointInBox(p1))
                                    return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void DoCheckOverlapping(vdControls.vdFramedControl vdFramedControl, bool IsBoundary = false)
        {
            if (PreviousVdFigure > 0)
            {
                var vdf = vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities.FindFromId(PreviousVdFigure);
                var poly = new vdPolyline();
                if (vdf is vdCircle vdCircle)
                {
                    poly = vdCircle.AsPolyline();
                }
                if (vdf is vdRect rec)
                {
                    poly = rec.AsPolyline();
                }
                if (vdf is vdPolyline p)
                {
                    poly = p;
                }
                vdEntities vdEntities = new vdEntities();
                var ent = vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities.GetNotDeletedItems();
                foreach (vdFigure i in ent) vdEntities.AddItem(i);

                if (!CheckPingPong(poly, vdEntities, true, IsBoundary))
                {
                    vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities.RemoveItem(vdf);
                    vdFramedControl.BaseControl.ActiveDocument.Update();
                    vdFramedControl.BaseControl.ActiveDocument.Redraw(true);
                }
                else
                {
                    if (IsBoundary)
                    {
                        MainBoundary = vdf as vdPolyline;
                    }
                }

            }
        }
        private bool CheckPingPong_(vdPolyline vdPolyline)
        {
            //foreach (var lbg in LstObstacles)
            //{
            //    gPoints gPoint = new gPoints();
            //    if (lbg.vdObstacle.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
            //    {
            //        return false;
            //    }
            //    foreach (gPoint p1 in lbg.vdObstacle.BoundingBox.GetPoints())
            //    {
            //        if (vdPolyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //    foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
            //    {
            //        if (lbg.vdObstacle.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //}
            //foreach (var lbg in LstUserBoundary)
            //{
            //    gPoints gPoint = new gPoints();
            //    if (lbg.Boundary.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
            //    {
            //        return false;
            //    }
            //    foreach (gPoint p1 in lbg.Boundary.BoundingBox.GetPoints())
            //    {
            //        if (vdPolyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //    foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
            //    {
            //        if (lbg.Boundary.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //}
            //foreach (var lbg in LstTBBOXes)
            //{
            //    gPoints gPoint = new gPoints();
            //    if (lbg.polyline.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
            //    {
            //        return false;
            //    }
            //    foreach (gPoint p1 in lbg.polyline.BoundingBox.GetPoints())
            //    {
            //        if (vdPolyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //    foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
            //    {
            //        if (lbg.polyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //}

            //List<Destination> Lstdes = new List<Destination>();
            //List<IORoom> Lstdes = new List<IORoom>();
            ////if (MainDestination != null)
            ////    Lstdes.Add(MainDestination);
            //foreach (var lbg in Lstdes)
            //{
            //    gPoints gPoint = new gPoints();
            //    if (lbg.polyline.IntersectWith(vdPolyline, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gPoint))
            //    {
            //        return false;
            //    }
            //    foreach (gPoint p1 in lbg.polyline.BoundingBox.GetPoints())
            //    {
            //        if (vdPolyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //    foreach (gPoint p1 in vdPolyline.BoundingBox.GetPoints())
            //    {
            //        if (!lbg.polyline.BoundingBox.PointInBox(p1))
            //        {
            //            return false;
            //        }
            //    }
            //}

            return true;
        }
        public List<Instrument> DetectedInstrumenToList()
        {
            List<Instrument> instruments = new List<Instrument>();
            foreach (var row in DGV_LstInstrument)
                instruments.Add(row.Instrument);
            return instruments;
        }
        private bool CheckLegalDrawn(vdPolyline vdPolyline, vdEntities vdEntities)
        {
            return CheckPingPong(vdPolyline, vdEntities);
        }
        public DataTable ListToDataTable<T>(List<T> items, string GroupColumnNames = null, string IndexColumns = null)
        {
            if (items.Count == 0)
                return new DataTable();
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) dataTable.Columns.Add(prop.Name);
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            if (IndexColumns != null)
            {
                int k = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    k++;
                    dr[IndexColumns] = k.ToString();
                }
            }
            if (!string.IsNullOrEmpty(GroupColumnNames))  // later, proceed to be contained for split column name
            {
                var dt = dataTable.AsEnumerable()
             .GroupBy(r => new { Col1 = r[GroupColumnNames], Col2 = r[GroupColumnNames] })
             .Select(g => g.OrderBy(r => r["GUID"]).First())
             .CopyToDataTable();
                return dt;
            }

            return dataTable;
        }
        public List<InstrumentInfoEntity> ListToListGroup(List<InstrumentInfoEntity> lst = null, string Keys = null)
        {
            var result = (from g in lst
                          group g by g.To into gp
                          select gp).ToList().FirstOrDefault().ToList();
            return result;
        }
        public void SyncronyzeRelatedUI(List<int> lst, vdControls.vdFramedControl vdFramedControl, bool ForcetoRemove=false)
        {

            LstEraseSelectedObjectID = lst;
            var des = new TBBOXDestination();
            var e = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            foreach (var id in LstEraseSelectedObjectID)
            {
                try
                {
                    var tbbox = FindIO_TBBox(id);//remove desitnation
                    var txtDestination = LstTBBOXes.Where(x => x.NameId == id);
                    if (txtDestination.Any() || tbbox != null) // From Text Selection
                    {
                        if (tbbox == null)
                            tbbox = txtDestination.FirstOrDefault();
                        if (tbbox == null) continue;
                        e.RemoveItem(e.FindFromId(tbbox.polyline.Id));
                    }
                    if (tbbox != null)  // From polyline Selection
                    {

                        e.RemoveItem(e.FindFromId(tbbox.NameId)); //Bounded Destination Name (Text)
                        foreach (var l in tbbox.MainRouteCollection)
                        {
                            e.RemoveItem(e.FindFromId(l.Id));  // Bounded Sub-Routes (MainRoute) 
                        }
                        LstTBBOXes.Remove(tbbox);
                    }
                    if (LstExternalRoute.Where(c => c.Id == id).Any())
                    {
                        LstExternalRoute.Remove(LstExternalRoute.Where(c => c.Id == id).FirstOrDefault());
                    }

                    if (LstObstacles.Where(c => c.guid == id).Any())
                    {
                        LstObstacles.Remove(LstObstacles.Where(c => c.guid == id).FirstOrDefault());
                    }
                    if (LstallInstruments.Where(c => c.OwnerInsert.Id == id).Any())
                    {
                        LstallInstruments.Remove(LstallInstruments.Where(c => c.OwnerInsert.Id == id).FirstOrDefault());
                    }
                    if (AllInstrument_Leader.Where(c => c.Item2 != null && c.Item2.Id == id).Any())
                    {
                        AllInstrument_Leader.Remove(AllInstrument_Leader.Where(c => c.Item2 != null && c.Item2.Id == id).FirstOrDefault());
                    }
                    if (AllInstrument_Leader.Where(c => c.Item1 != null && c.Item1.Id == id).Any())
                    {
                        AllInstrument_Leader.Remove(AllInstrument_Leader.Where(c => c.Item1 != null && c.Item1.Id == id).FirstOrDefault());
                    }
                    try
                    {
                        var tc = LstTBBOXes.Where(c => c.MainRouteCollection.Count > 0 && c.MainRouteCollection.Where(x => x.Id == id).Any());
                        if (tc.Any())// remove route
                        {
                            var mc = tc.FirstOrDefault().MainRouteCollection;
                            mc.Remove(mc.Where(c => c.Id == id).FirstOrDefault());
                        }
                    }
                    catch
                    {

                    }
                    if (MainBoundary != null)
                    {
                        if (MainBoundary.Id == id)
                        {
                            MainBoundary = null;
                        }
                    }
                    if (lstConnector.Where(c => c.Id == id).Any())
                    {
                        lstConnector.Remove(lstConnector.Where(c => c.Id == id).FirstOrDefault());
                    }
                    if (lstDuctLines.Count >0)
                    {
                        if (!ForcetoRemove) // It is for Ductline only
                        {
                            var vf = e.FindFromId(id);
                            if (vf != null && vf is vdLine vl)
                            {
                                var resp = lstDuctLines.Where(x => x.sp == vl.StartPoint && x.ep == vl.EndPoint);
                                if (resp.Any())
                                {
                                    vl.visibility = vdFigure.VisibilityEnum.Invisible;
                                    continue;
                                }
                            }
                        }
                    }
                    var fg = e.FindFromId(id);
                    e.RemoveItem(fg); 
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLog(ex);
                }
            }
            RefreshCADSpace(vdFramedControl);

        }
        private void RefreshCADSpace(vdControls.vdFramedControl vdFC1)
        {
            vdFC1.BaseControl.Redraw();
            vdFC1.BaseControl.Update();
            vdFC1.Update();
            vdFC1.BaseControl.ActiveDocument.Redraw(true);
            //vdFC1.BaseControl.ActiveDocument.ActiveLayOut.Refresh();
            vdFC1.BaseControl.ActiveDocument.ActiveLayer.Update();
        }
        public void UpdateInstrument(vdControls.vdFramedControl vdFramedControl, DataGridView dgv1)
        {
            var lstRow = new List<DataGridViewRow>();
            var lstins = new List<Instrument>();
            foreach (var eo in LstEraseSelectedObjectID)
            {
                foreach (DataGridViewRow dr in dgv1.Rows)
                {
                    var cur = Convert.ToInt32(dr.Cells["GUID"].Value); // Something was wrong with this GUID need to check this from Circle ID from Circle
                    var T1 = (dr.Cells["colT1"].Value);
                    var T2 = (dr.Cells["colT2"].Value);
                    // var curins = dr.Cells["Iv"].Value.ToString();
                    //var res = vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities.FindFromId(cur);
                    foreach (Layer l in LstLayer)//vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities)
                    {
                        foreach (var ins in l.listInstrument)
                        {
                            if (ins.OwnerInsert.Id == eo && ins.t1 == T1.ToString() && ins.t2 == T2.ToString())
                            {
                                lstRow.Add(dr);
                                lstins.Add(ins);
                                break;
                            }
                        }
                    }
                }
            }


            foreach (var dr in lstRow)
                dgv1.Rows.Remove(dr);
            foreach (var ins in lstins)
            {
                foreach (Layer l in LstLayer)
                {
                    l.listInstrument.Remove(ins);
                }
            }
            dgv1.Update();
        }

        public void UpdateInstrumentDistance(vdControls.vdFramedControl vdFramedControl) // Update Instrument Destination
        {
            Dictionary<string, double> dis;//= new Dictionary<string, double>();
            var lstIns = GetAllInstrument(vdFramedControl, this);
            LstallInstruments = lstIns;
        }
        public void HighLightOverLength(vdControls.vdFramedControl vdFramedControl, double MaxOverlappedDistance, Tuple<int, int> tp)
        {
            UpdateInstrumentDistance(vdFramedControl);
            var LstIns = new List<Instrument>();
            var lstDes_Group = ListToDataTable(DGV_LstInstrument, "To");
            List<Instrument> selectedInstruments = new List<Instrument>();
            foreach (DataRow l in lstDes_Group.Rows)
            {
                var GUID = Convert.ToInt32(l["To"]);
                var selectedTB = FindIO_TBBox(GUID);
                if (selectedTB == null) continue;
                var name = selectedTB.Name; //(ioDes != null && ioDes.Name != null) ? ioDes.Name : selectedTB.Name;
                foreach (var lr in DGV_LstInstrument)
                {
                    var RouteCollections = selectedTB.MainRouteCollection;
                    var selectedIns = LstallInstruments.Where(x => x.t1 == lr.Instrument.t1 && x.t2 == lr.Instrument.t2);

                    if (RouteCollections.Count > 0)
                    {
                        foreach (var p in RouteCollections)
                        {
                            var nrst = 0.0;


                            var ele = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(selectedIns.FirstOrDefault().OwnerInsert.Id);

                            int count = (int)(p.Length() / tp.Item1);
                            List<vdPolyline> polylines = new List<vdPolyline>();
                            gPoints gps = p.Divide(count);
                            foreach (gPoint gp in gps)
                            {
                                var nearestdis = lr.Instrument.centerPoint.Distance2D(gp);
                                if (nearestdis < nrst || nrst == 0)
                                {
                                    nrst = nearestdis;
                                }
                            }
                            if (nrst > MaxOverlappedDistance && !selectedIns.FirstOrDefault().IsReserved)
                            {
                                ele.HighLight = true;
                            }
                            else
                            {
                                ele.HighLight = false;
                                selectedIns.FirstOrDefault().IsReserved = true;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(l["To"]) == lr.To)
                        {
                            if (selectedIns.Any())
                            {
                                var InsDic = selectedIns.FirstOrDefault().Distance2Destination.Where(x => x.Key == name);

                                if (InsDic.Any())
                                {

                                    if (InsDic.FirstOrDefault().Value <= MaxOverlappedDistance)
                                    {
                                        selectedIns.FirstOrDefault().IsReserved = true;
                                    }
                                    else
                                    {
                                        if (!selectedIns.FirstOrDefault().IsReserved)
                                        {
                                            var ele = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(selectedIns.FirstOrDefault().OwnerInsert.Id);
                                            ele.HighLight = true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                vdFramedControl.BaseControl.Redraw();
            }
            int i = 0;
            foreach (var lr in DGV_LstInstrument)
            {
                var highlight = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(lr.Instrument.OwnerInsert.Id).HighLight;
                if (highlight)
                {
                    i++;
                }
            }
            if (i > 0)
            {
                ////var msg = "There are " + i.ToString() + " instruments that are not within the range of specified gap." +
                ////    Environment.NewLine + "Please take a check for those remaining instruments.";
                ////MessageBox.Show(msg, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public List<vdLine> LstAllRoute()
        {
            LstAllRouteDrawn = new List<vdLine>();
            foreach (var l in LstTBBOXes) // for MainRoute
            {
                foreach (var m in l.MainRouteCollection)
                {
                    var vdline = new vdLine();
                    vdline = m;
                    LstAllRouteDrawn.Add(vdline);
                }
            }
            foreach (var l in LstExternalRoute)
            {
                var vdline = new vdLine();
                vdline = l;
                LstAllRouteDrawn.Add(vdline);
            }
            return LstAllRouteDrawn;
        }
        public List<int> DetectInstrumentInsideRange(vdControls.vdFramedControl vdFramedControl, double MaxOverlappedDistance, Tuple<int, int> tp, double Unigrid)
        { 
            UpdateInstrumentDistance(vdFramedControl);
            var LstIns = new List<Instrument>();
            AllRouteCollections = new Dictionary<vdLine, gPoints>();

            LstAllRoute(); 
            List<Instrument> selectedInstruments = new List<Instrument>();
            foreach (var l in LstTBBOXes) // for MainRoute
            {
                //var GUID = Convert.ToInt32(l["To"]);
                var selectedTB = l; //FindIO_TBBox(GUID);
                if (selectedTB == null) continue;
                //var name = selectedTB.Name; //(ioDes != null && ioDes.Name != null) ? ioDes.Name : selectedTB.Name;
                if (DGV_LstInstrument == null) continue;
                foreach (var lr in DGV_LstInstrument)
                {
                    var RouteCollections = selectedTB.MainRouteCollection;
                    var selectedIns = LstallInstruments.Where(x => x.t1 == lr.Instrument.t1 && x.t2 == lr.Instrument.t2);
                    if (selectedIns.FirstOrDefault() == null) 
                        continue;
                    if (RouteCollections.Count > 0)
                    {
                        foreach (var p in RouteCollections)
                        {
                           // if (p.Length() == Unigrid) continue;
                            var nrst = 0.0;
                            vdFigure ele = null;
                            try
                            {
                                ele = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(selectedIns.FirstOrDefault().OwnerInsert.Id);
                            }
                            catch
                            {
                                continue;
                            }
                            if (ele != null && ele.Deleted) continue;
                            int count = (int)(p.Length() / tp.Item1);
                            List<vdPolyline> polylines = new List<vdPolyline>();
                            gPoints gps = p.Divide(count);
                            gPoint resp = new gPoint();
                            if (gps == null) continue;
                            foreach (gPoint gp in gps)
                            {
                                var nearestdis = lr.Instrument.centerPoint.Distance2D(gp);
                                if (nearestdis < nrst || nrst == 0)
                                {
                                    resp = gp;
                                    nrst = nearestdis;
                                }
                            }
                            if (nrst > MaxOverlappedDistance && !selectedIns.FirstOrDefault().IsReserved)
                            {
                                ele.HighLight = true;
                            }
                            else
                            {
                                if (!selectedIns.FirstOrDefault().IsReserved)
                                {
                                    selectedIns.FirstOrDefault().PosssessRoute = p;
                                    selectedIns.FirstOrDefault().PosssessDestination = selectedTB;
                                    selectedIns.FirstOrDefault().PossessPoint = resp;
                                    var rDic = AllRouteCollections.Where(x => x.Key == p);
                                    if (rDic.Any())
                                    {
                                        var v = rDic.FirstOrDefault().Value;
                                        bool Persist = false;
                                        foreach (var cr in v)
                                        {
                                            if (cr == resp)
                                            {
                                                Persist = true;
                                                break;
                                            }
                                        }
                                        if (!Persist)
                                            rDic.FirstOrDefault().Value.Add(resp);
                                    }
                                    else
                                    {
                                        var ps = rDic.FirstOrDefault().Value;
                                        if (ps == null)
                                            ps = new gPoints();
                                        ps.Add(resp);
                                        AllRouteCollections.Add(p, ps);
                                    }
                                }
                                ele.HighLight = false;
                                selectedIns.FirstOrDefault().IsReserved = true;
                            }
                        }
                    }
                    else
                    {
                        ////if (Convert.ToInt32(l["To"]) == lr.To)
                        ////{
                        if (selectedIns.Any())
                        {
                            // var InsDic = selectedIns.FirstOrDefault().Distance2Destination.Where(x => x.Key == name);
                            var InsDic = selectedIns.FirstOrDefault().centerPoint.Distance2D(selectedTB.centerPoint);

                            if (InsDic != 0)
                            {
                                if (InsDic <= MaxOverlappedDistance)
                                {
                                    selectedIns.FirstOrDefault().IsReserved = true;
                                }
                                else
                                {
                                    if (!selectedIns.FirstOrDefault().IsReserved)
                                    {
                                        var ele = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(selectedIns.FirstOrDefault().OwnerInsert.Id);
                                        if (ele != null)
                                        {
                                            if (ele.Deleted) continue;
                                            ele.HighLight = true;
                                        }
                                    }

                                }
                            }
                        }
                        ////}
                    }
                }
                vdFramedControl.BaseControl.Redraw();
            }

            //For External Route
            {
                TBBOXDestination selectedTB = null; //
                                                    //if (selectedTB == null) ;
                if (DGV_LstInstrument != null)
                {
                    foreach (var lr in DGV_LstInstrument)
                    {
                        var RouteCollections = LstExternalRoute;
                        var selectedIns = LstallInstruments.Where(x => x.t1 == lr.Instrument.t1 && x.t2 == lr.Instrument.t2);
                        if (selectedIns.FirstOrDefault() == null) continue;
                        if (RouteCollections.Count > 0)
                        {
                            foreach (var p in RouteCollections)
                            {
                                // if (p.Length() == Unigrid) continue;
                                var nrst = 0.0;
                                vdFigure ele = null;
                                try
                                {
                                    ele = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(selectedIns.FirstOrDefault().OwnerInsert.Id);
                                }
                                catch
                                {
                                    continue;
                                }
                                if (ele != null && ele.Deleted) continue;
                                int count = (int)(p.Length() / tp.Item1);
                                List<vdPolyline> polylines = new List<vdPolyline>();
                                gPoints gps = p.Divide(count);
                                gPoint resp = new gPoint();
                                if (gps == null) continue;
                                foreach (gPoint gp in gps)
                                {
                                    var nearestdis = lr.Instrument.centerPoint.Distance2D(gp);

                                    if (nearestdis < nrst || nrst == 0)
                                    {
                                        resp = gp;
                                        nrst = nearestdis;
                                    }
                                }
                                if (nrst > MaxOverlappedDistance && !selectedIns.FirstOrDefault().IsReserved)
                                {
                                    ele.HighLight = true;
                                }
                                else
                                {
                                    if (!selectedIns.FirstOrDefault().IsReserved)
                                    {
                                        selectedIns.FirstOrDefault().PosssessRoute = p;
                                        selectedIns.FirstOrDefault().PosssessDestination = selectedTB;
                                        selectedIns.FirstOrDefault().PossessPoint = resp;
                                        var rDic = AllRouteCollections.Where(x => x.Key == p);
                                        if (rDic.Any())
                                        {
                                            var v = rDic.FirstOrDefault().Value;
                                            bool Persist = false;
                                            foreach (var cr in v)
                                            {
                                                if (cr == resp)
                                                {
                                                    Persist = true;
                                                    break;
                                                }
                                            }
                                            if (!Persist)
                                                rDic.FirstOrDefault().Value.Add(resp);
                                        }
                                        else
                                        {
                                            var ps = rDic.FirstOrDefault().Value;
                                            if (ps == null)
                                                ps = new gPoints();
                                            ps.Add(resp);
                                            AllRouteCollections.Add(p, ps);
                                        }
                                    }
                                    ele.HighLight = false;
                                    selectedIns.FirstOrDefault().IsReserved = true;
                                }
                            }
                        }
                    }
                }
                vdFramedControl.BaseControl.Redraw();
            }

            int i = 0;
            List<int> lstIds = new List<int>();
            if (DGV_LstInstrument != null)
            {
                foreach (var lr in DGV_LstInstrument)
                {
                    var highlight = vdFramedControl.BaseControl.ActiveDocument.ActiveLayOut.Entities.FindFromId(lr.Instrument.OwnerInsert.Id);//.HighLight;
                    if (highlight != null)
                    {
                        if (highlight.Deleted) continue;
                        if (highlight.HighLight)
                        {
                            lstIds.Add(lr.Instrument.OwnerInsert.Id);
                            i++;
                        }
                    }
                }
            }
            return lstIds;
        }

        public List<Obstacle> GetSelectedObstacles(vdControls.vdFramedControl vdFramedControl, ListBox listBox)
        {
            List<Obstacle> obstacles = new List<Obstacle>(); 
            if (listBox.Items.Count == 0) return obstacles;
            foreach (vdFigure f in vdFramedControl.BaseControl.ActiveDocument.Model.Entities.GetNotDeletedItems())
            {
                foreach (var l in listBox.Items)
                {
                    if (f.Layer.Name == (l as Layer).name.ToString())
                    {
                        obstacles.Add(new Obstacle(f));
                    }
                }
            }
            return obstacles;
        }

        public object Clone()
        {
            var ri = (RouteInfo)MemberwiseClone(); 
            return ri;
        }

        public bool CheckDuplicatedInstrument(vdControls.vdFramedControl vdFramedControl)
        {
            var lst = GetAllInstrument(vdFramedControl, this);
            var dtduplicate = new DataTable();
            dtduplicate.Columns.Add("T1_T2");
            foreach (var r in lst)
            {
                dtduplicate.Rows.Add(new object[] { r.t1 + "_" + r.t2 });
            }
            var query = (from row in dtduplicate.AsEnumerable()
                         group row by new { t11 = row.Field<string>("T1_T2") } into dupli
                         where dupli.Count() > 1
                         select new { t11 = dupli.Key.t11 });
            if (query.Any())
            {
                MessageBox.Show("There are some instruments having same attributes in CAD window. if proceed as it is the analysis might execute wrong calculation." + Environment.NewLine + "Please check and prepare for those TagNos.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        public int CheckDuplicatedInstrumentNoMessage(vdControls.vdFramedControl vdFramedControl)
        {
            var lstIns =  GetAllInstrument(vdFramedControl, this);

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
            int c = 0;
            if (query.Any())
            {
                c = query.Count();
            }
            return c;
        }


        public void SetvaluesToNullCablesFromMultiCheck()
        {
            try
            {
                var sbl = new RouteOptimizer.BL.SettingBL();
                var lstInscable = sbl.GetInsCable();
                // var lstSignal = sbl.GetSignalType(); 
                var LstInsType = sbl.GetInstrumetnType();
                var sigDuct = sbl.GetSignalType();
                foreach (var inst in DGV_LstInstrument)
                {
                    var selectedIns = LstallInstruments.Where(x => x.t1 == inst.Instrument.t1 && x.t2 == inst.Instrument.t2); //x => x.t1 == inst.Instrument.t1 && x.t2 == inst.Instrument.t2

                    if (!selectedIns.Any()) continue;

                    var cableIns = inst.LstInstCableEntity;
                    if (cableIns == null && inst.To != -1 && !string.IsNullOrEmpty(inst.To.ToString()))
                    {
                        List<InstCableEntity> newvalue = new List<InstCableEntity>();

                        inst.LstInstCableEntity = newvalue;
                        inst.LstInstCableEntity = lstInscable.Where(a => a.InstType == inst.Type).ToList<InstCableEntity>();
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
                            var sig = sigDuct.Where(x => x.Title == lsc.System).FirstOrDefault();
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
                        //SetInsCable(null,  lr);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex.Message);
            }
        }
        #endregion
    }
}
