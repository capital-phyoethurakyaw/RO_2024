
using RouteOptimizer.Analysis1;
using RouteOptimizer.Entity;
using RouteOptimizer.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdPrimaries;

namespace RouteOptimizer.XMLBuffer
{
    public class ID_Exchanger
    {
        public ID_Exchanger()
        {

        }
        //RouteOptimizer.Object.RouteInfo ID_Exchange = new ID_Exchanger();
        public string TAGNAME = "TAG No.";
        public string SYSTEMABBRE = "SystemReservedLayer_";
        public string SEGMENTABBRE = "SG_";
        public string IOABBRE = "";//"IO-";
        public string TBABBRE = "";//"TB-";
        public string xgap = "";
        public string ygap = "";
        public int MaxPoly = 50;
        public int BoxWidth = 1000;
        public int BoxHeight = 1000;
        public string IOcolor = "Red";
        public string TBColor = "Cyan";
        public string MCCcolor = "DarkBlue";
        public int DefaultDuctCheck { get; set; } = 1;
        public int DefaultAnalysisCheck { get; set; } = 1;
        public string DetectedObstacles { get; set; } = "";
        public int SelectedDestination { get; set; } = 0;
        public int SelectedMainBoundary { get; set; } = 0;
        public int SelectedGUID = 0;
        public string SystemFile = null;
        public int PreviousVdFigure = 0;

        public List<DictonaryTuple_ID_Exchanger> AllRouteCollections = new List<DictonaryTuple_ID_Exchanger>() { };
        public List<DictonaryTuple_ID_Exchanger> AllNodeInEachDuct = new List<DictonaryTuple_ID_Exchanger> { };
        public List<DictonaryTuple_ID_Exchanger> AllInstrument_Leader = new List<DictonaryTuple_ID_Exchanger>();

        public string MainBoundary = "";
        public List<string> LstTextName_Cricle_Cache = new List<string>() { };

        public List<string> LstExternalRoute = new List<string>() { };
        public List<string> LstAllRouteDrawn = new List<string>();

        public List<string> LstLayerSelectedInstruments = new List<string>();
        public List<string> LstLayerSelectedObstacles = new List<string>();
        public List<DictonaryTuple_ID_Exchanger> LstLayerAllColumn = new List<DictonaryTuple_ID_Exchanger>();

        public List<string> LstObstacles = new List<string>();
        public List<TB_ID_Exchanger> LstTBBOXes = new List<TB_ID_Exchanger>();
        public List<InsInfo_ID_Exchanger> DGV_LstInstrument = new List<InsInfo_ID_Exchanger>() { };

        public List<Segment_ID_Exchanger> LstSegmentInfo = new List<Segment_ID_Exchanger>();
        public List<Segment_ID_Exchanger> LstSelectedSegmentInfo = new List<Segment_ID_Exchanger>();
        public List<RouteInfo_ID_Exchanger> LstInsRouteInfo = new List<RouteInfo_ID_Exchanger>();
        public BasicSettingEntity basicSettingEntity = new BasicSettingEntity();

        public List<Connector_ID_Exchanger> LstGuidedRoute = new List<Connector_ID_Exchanger>() { };
        public List<Connector_ID_Exchanger> LstAutoRoute = new List<Connector_ID_Exchanger>() { };
        public List<Connector_ID_Exchanger> LstAutoGuidedRoute = new List<Connector_ID_Exchanger>() { };

        //public List<string> LstGuidedRoute = new List<string>() { };
        //public List<string> LstAutoRoute = new List<string>() { };
        //public List<string> LstAutoGuidedRoute = new List<string>() { };

        public List<Route_ID_ExChanger> CaseRoutes = new List<Route_ID_ExChanger>() { };
        public List<Ana_DuctLine_Exchanger> lstDuctLines = new List<Ana_DuctLine_Exchanger>();
        public List<AnalysisResultEntity> lstResult1 = new List<AnalysisResultEntity>();
        public List<AnalysisResultEntity> lstResult2 = new List<AnalysisResultEntity>();
        public List<AnalysisResultEntity> lstResult3 = new List<AnalysisResultEntity>();


        public List<(string, int)> dicLinesPros = new List<(string, int)>();
        public List<string> lstConnector = new List<string>();

        public int DEFAULT_OPTION = 0;
        //public vdControls.vdFramedControl vdFramedControl1 = null;
        public RouteOptimizer.Object.RouteInfo GetRouteInfo(vdControls.vdFramedControl vdFramedControl)
        {    
            var et = vdFramedControl.BaseControl.ActiveDocument.ActionLayout.Entities;
                //var poly = new vdPolyline();
                var routeInfo = new RouteOptimizer.Object.RouteInfo();
            try
            {
                //vdFramedControl1 = vdFramedControl;
         
                routeInfo.dicLinesPros = dicLinesPros;
                routeInfo.SYSTEMABBRE = SYSTEMABBRE;
                routeInfo.SEGMENTABBRE = SEGMENTABBRE;
                routeInfo.IOABBRE = IOABBRE.Replace("IO-", "");
                routeInfo.TBABBRE = TBABBRE.Replace("TB-", "");
                routeInfo.xgap = xgap;
                routeInfo.ygap = ygap;
                routeInfo.MaxPoly = MaxPoly;
                routeInfo.DetectedObstacles = DetectedObstacles;
                routeInfo.SelectedDestination = SelectedDestination;
                routeInfo.SelectedMainBoundary = SelectedMainBoundary;
                routeInfo.SelectedGUID = SelectedGUID;
                routeInfo.SystemFile = SystemFile;
                routeInfo.PreviousVdFigure = PreviousVdFigure;
                routeInfo.BoxWidth = BoxWidth;
                routeInfo.BoxHeight = BoxHeight;
                routeInfo.IOcolor = IOcolor;
                routeInfo.MCCcolor = MCCcolor;
                routeInfo.TBColor = TBColor;
                routeInfo.DefaultAnalysisCheck = DefaultAnalysisCheck;
                routeInfo.DefaultDuctCheck = DefaultDuctCheck;
                Dictionary<vdLine, gPoints> resAllRouteCollection = new Dictionary<vdLine, gPoints>() { };
                foreach (var arc in AllRouteCollections)
                {
                    var f = GetFigureById(arc.polyID, vdFramedControl);
                    if (f != null && f is vdLine vp)
                        resAllRouteCollection.Add(vp, arc.gpointsID);
                }
                routeInfo.AllRouteCollections = resAllRouteCollection;

                Dictionary<vdLine, List<gPoint>> resAllNodeInEachDuct = new Dictionary<vdLine, List<gPoint>>() { };
                foreach (var arc in AllNodeInEachDuct)
                {
                    var f = GetFigureById(arc.polyID, vdFramedControl);
                    if (f != null && f is vdLine vp) resAllNodeInEachDuct.Add(vp, arc.LstgPointID);
                }
                routeInfo.AllNodeInEachDuct = resAllNodeInEachDuct;

                List<Tuple<vdInsert, vdLeader>> resAllInstrument_Leader = new List<Tuple<vdInsert, vdLeader>>() { };
                foreach (var arc in AllInstrument_Leader)
                {
                    try { 
                    var f = GetFigureById(arc.insertID, vdFramedControl);
                    var g = GetFigureById(arc.leaderID, vdFramedControl);
                    vdInsert fvd = null;
                    vdLeader fld = null;
                    if (f != null && f is vdInsert fi)
                        fvd = fi;
                    if (g != null && f is vdLeader fl)
                        fld = fl;
                    //if (f != null && f is vdInsert vp && g != null && g is vdLeader vl)
                    //{
                    resAllInstrument_Leader.Add(Tuple.Create<vdInsert, vdLeader>(fvd, fld));
                        //}
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.AllInstrument_Leader = resAllInstrument_Leader;
                var lstCols = new List<DictonaryTuple_ID_Exchanger>();
                foreach (var l in LstLayerAllColumn)
                {
                    try { 
                    var die = new DictonaryTuple_ID_Exchanger();
                    die.LayerName = l.LayerName;
                    die.System = l.System;
                    die.Type = l.Type;
                    var val = l.To;
                    if (!string.IsNullOrEmpty(val) && val != "-1")
                        die.To = GetFigureById(l.To, vdFramedControl).Id.ToString();
                    lstCols.Add(die);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }

                routeInfo.LstLayerAllColumn = lstCols;


                var Mainpoly = GetFigureById(MainBoundary, vdFramedControl);
                if (Mainpoly != null && Mainpoly is vdPolyline vp1)
                {
                    routeInfo.MainBoundary = vp1;
                }

                var lcc = new List<vdFigure>();
                foreach (var c in LstTextName_Cricle_Cache)
                {
                    var Cache = GetFigureById(c, vdFramedControl);
                    if (Cache != null)
                    {
                        lcc.Add(Cache);
                    }
                }
                routeInfo.LstTextName_Cricle_Cache = lcc;

                var ler = new List<vdLine>();
                foreach (var c in LstExternalRoute)
                {
                    var Cache = GetFigureById(c, vdFramedControl);
                    if (Cache != null && Cache is vdLine vp)
                    {
                        ler.Add(vp);
                    }
                }
                routeInfo.LstExternalRoute = ler;

                var lad = new List<vdLine>();
                foreach (var c in LstAllRouteDrawn)
                {
                    var Cache = GetFigureById(c, vdFramedControl);
                    if (Cache != null && Cache is vdLine vp)
                    {
                        lad.Add(vp);
                    }
                }
                routeInfo.LstAllRouteDrawn = lad;

                var lstObs = new List<Obstacle>();
                foreach (var l in LstObstacles)
                {
                    var Cache = GetFigureById(l, vdFramedControl);
                    if (Cache != null)
                    {
                        lstObs.Add(new Obstacle(Cache));
                    }
                }
                routeInfo.LstObstacles = lstObs;

                routeInfo.LstallInstruments = routeInfo.GetAllInstrument(vdFramedControl, routeInfo);

                foreach (var l in routeInfo.LstLayer)
                {
                    foreach (var r in LstLayerSelectedInstruments)
                    {
                        if (r == l.name)
                            routeInfo.LstLayer.Where(v => v.name == l.name).FirstOrDefault().SelectedInInstrument = true;
                    }
                    foreach (var r in LstLayerSelectedObstacles)
                    {
                        if (r == l.name)
                            routeInfo.LstLayer.Where(v => v.name == l.name).FirstOrDefault().SelectedInObstacle = true;
                    }
                }

                var tbboxes = new List<TBBOXDestination>();
                foreach (var tb in LstTBBOXes)
                {
                    try
                    {
                        var IsIO = (Object.RouteInfo.eDestinationType)tb.IsIO;
                        var tbb = new TBBOXDestination(GetFigureById(tb.vdpolyline, vdFramedControl) as vdPolyline, IsIO);
                        var txt = GetFigureById(tb.vdText, vdFramedControl);
                        (txt as vdText).TextString = (txt as vdText).TextString.Replace("TB-", "").Replace("IO-", "");
                        tbb.NameId = txt.Id;
                        tbb.Name = (txt as vdText).TextString.ToString();
                        List<vdLine> vpl = new List<vdLine>();
                        foreach (var r in tb.MainRoute)
                        {
                            vpl.Add(GetFigureById(r, vdFramedControl) as vdLine);
                        }
                        tbb.MainRouteCollection = vpl;
                        tbb.IsIO = IsIO;


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
                                SignalType = mCC.SignalType
                            };
                            mce.Add(me);
                        }
                        tbb.LstmCCEntities = mce;

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
                        tbb.LstmCCEntitiesHeader = mceHeader;
                        //tbb.mps = tb.mps;
                        //tbb.mMatrix = tb.mMatrix;
                        //tbb.mp = tb.mp;
                        //List<Instrument> lstins = new List<Instrument>();
                        //foreach (var r in tb.lstInstrument)
                        //{
                        //    lstins.Add(routeInfo.LstallInstruments.Where(x => x.OwnerInsert.URL == r).FirstOrDefault());
                        //}
                        //tbb.lstInstrument = lstins;

                        tbb.EachCheck = tb.EachCheck;
                        tbb.AutoCheck = tb.AutoCheck;
                        tbb.CableType = tb.CableType;
                        tbboxes.Add(tbb);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.LstTBBOXes = tbboxes;

                foreach (var tb in routeInfo.LstTBBOXes)
                {
                    var t = LstTBBOXes.Where(x => x.vdpolyline == tb.polyline.URL && x.vdText == et.FindFromId(tb.NameId).URL);
                    if (t.Any())
                    {
                        if (!string.IsNullOrEmpty(t.FirstOrDefault().OwnDestination))
                        {
                            tb.OwnDestination = routeInfo.LstTBBOXes.Where(x => x.polyline.URL == t.FirstOrDefault().OwnDestination).FirstOrDefault();
                        }
                    }
                }
                var di = new List<InstrumentInfoEntity>();
                foreach (var dgvi in DGV_LstInstrument)
                {
                    try { 
                    InstrumentInfoEntity instrumentInfoEntity = new InstrumentInfoEntity();
                    instrumentInfoEntity.Instrument = routeInfo.LstallInstruments.Where(x => x.OwnerInsert.URL == dgvi.Instrument).FirstOrDefault();
                    instrumentInfoEntity.GUID = instrumentInfoEntity.Instrument.guid;
                    instrumentInfoEntity.T1 = dgvi.T1;
                    instrumentInfoEntity.T2 = dgvi.T2;
                    instrumentInfoEntity.Type = dgvi.Type;
                    instrumentInfoEntity.System = dgvi.System;
                    if (dgvi.To != null)
                        instrumentInfoEntity.To = GetFigureById(dgvi.To, vdFramedControl).Id;
                    instrumentInfoEntity.Classification_1 = dgvi.Classification_1;
                    instrumentInfoEntity.Classification_2 = dgvi.Classification_2;
                    instrumentInfoEntity.Classification_3 = dgvi.Classification_3;
                    instrumentInfoEntity.Cable = dgvi.Cable;
                    instrumentInfoEntity.CHK_AutoRoute = dgvi.CHK_AutoRoute;
                    instrumentInfoEntity.CHK_EachRoute = dgvi.CHK_EachRoute;

                    List<InstCableEntity> lins = new List<InstCableEntity>();
                    lins = dgvi.LstInstCableEntity;
                    foreach (var l in lins)
                    {
                        if (!string.IsNullOrEmpty(l.To) && l.To != "-1")
                        {
                            var fg = GetFigureById(l.To, vdFramedControl);
                            l.To = fg == null ? "" : fg.Id.ToString();
                        }
                    }
                    instrumentInfoEntity.LstInstCableEntity = lins;
                    instrumentInfoEntity.LayerName = dgvi.LayerName;

                    di.Add(instrumentInfoEntity);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.DGV_LstInstrument = di;

                var lsi = new List<SegmentInfoEntity>() { };
                foreach (var l in LstSegmentInfo)
                {
                    try { 
                    var Se = new SegmentInfoEntity();
                    Se.SegmentName = l.SegmentName;
                    Se.StartPoint = l.StartPoint;
                    Se.EndPoint = l.EndPoint;
                    Se.Length = l.Length;
                    Se.ParentRouteID = l.ParentRouteID == null ? 0 : GetFigureById(l.ParentRouteID, vdFramedControl).Id;
                    Se.CableList = l.CableList;
                    Se.ParentDestination = routeInfo.LstTBBOXes.Where(x => x.polyline.URL == l.ParentDestination).FirstOrDefault();
                    //Se.OptimalDuctSize = l.OptimalDuctSize;
                    Se.A_OptimalDuctSize = l.A_OptimalDuctSize;
                    Se.A_ActualOptimalResult = l.A_ActualOptimalResult;
                    Se.A_OptimalRatio = l.A_OptimalRatio;
                    Se.A_TotalArea = l.A_TotalArea;
                    Se.A_UserDefinedRato = l.A_UserDefinedRato;
                    Se.A_UserDefinedSize = l.A_UserDefinedSize;
                    Se.SignalType = l.SignalType;


                    lsi.Add(Se);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.LstSegmentInfo = lsi;

                var lssi = new List<SegmentInfoEntity>() { };
                foreach (var l in LstSelectedSegmentInfo)
                {
                    try { 
                    var Se = new SegmentInfoEntity();
                    Se.SegmentName = l.SegmentName;

                    Se.StartPoint = l.StartPoint;
                    Se.EndPoint = l.EndPoint;
                    Se.Length = l.Length;
                    Se.ParentRouteID = l.ParentRouteID == null ? 0 : GetFigureById(l.ParentRouteID, vdFramedControl).Id;
                    Se.CableList = l.CableList;
                    Se.ParentDestination = routeInfo.LstTBBOXes.Where(x => x.polyline.URL == l.ParentDestination).FirstOrDefault();
                    //Se.OptimalDuctSize = l.OptimalDuctSize;
                    Se.A_OptimalDuctSize = l.A_OptimalDuctSize;
                    Se.A_ActualOptimalResult = l.A_ActualOptimalResult;
                    Se.A_OptimalRatio = l.A_OptimalRatio;
                    Se.A_TotalArea = l.A_TotalArea;
                    Se.A_UserDefinedRato = l.A_UserDefinedRato;
                    Se.A_UserDefinedSize = l.A_UserDefinedSize;
                    Se.SignalType = l.SignalType;
                    lssi.Add(Se);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.LstSelectedSegmentInfo = lssi;

                var rie = new List<InstrumentRouteInfoEntity>();
                foreach (var l in LstInsRouteInfo)
                {
                    try
                    {
                        var re = new InstrumentRouteInfoEntity();
                        var ins = routeInfo.LstallInstruments.Where(x => x.OwnerInsert.URL == l.Instrument);
                        if (ins.Any())
                        {
                            re.Instrument = ins.FirstOrDefault();
                        }
                        else
                        {
                            var tb = routeInfo.LstTBBOXes.Where(x => x.polyline.URL == l.Instrument).FirstOrDefault();
                            var ie = new Instrument();
                            ie.t1 = tb.Name;
                            ie.t2 = null;
                            re.Instrument = ie;
                        }
                        re.CableType = l.CableType;
                        re.LstSegment = l.LstSegment;
                        re.Length = l.Length;
                        re.To = l.To;
                        re.SignalType = l.SignalType;
                        re.SystemType = l.SystemType;
                        rie.Add(re);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }

                routeInfo.LstInsRouteInfo = rie;



                var lgr = new List<vdLine>() { };
                foreach (var c in LstGuidedRoute)
                {
                    var Cache = GetFigureById(c.connectorId, vdFramedControl);
                    if (Cache != null && Cache is vdLine vp)
                    {
                        routeInfo.SyncronyzeRelatedUI(new List<int> { vp.Id }, vdFramedControl);
                    }
                    lgr.Add(new vdLine(vdFramedControl.BaseControl.ActiveDocument, new gPoint(c.sp.X, c.sp.Y), new gPoint(c.ep.X, c.ep.Y)));
                }
                routeInfo.LstGuidedRoute = lgr;

                var lar = new List<vdLine>() { };
                foreach (var c in LstAutoRoute)
                {
                    var Cache = GetFigureById(c.connectorId, vdFramedControl);
                    if (Cache != null && Cache is vdLine vp)
                    {
                        routeInfo.SyncronyzeRelatedUI(new List<int> { vp.Id }, vdFramedControl);
                    }
                    lar.Add(new vdLine(vdFramedControl.BaseControl.ActiveDocument, new gPoint(c.sp.X, c.sp.Y), new gPoint(c.ep.X, c.ep.Y)));
                }
                routeInfo.LstAutoRoute = lar;

                var lag = new List<vdLine>() { };
                foreach (var c in LstAutoGuidedRoute)
                {
                    var Cache = GetFigureById(c.connectorId, vdFramedControl);
                    if (Cache != null && Cache is vdLine vp)
                    {
                        routeInfo.SyncronyzeRelatedUI(new List<int> { vp.Id }, vdFramedControl);
                    }
                    lag.Add(new vdLine(vdFramedControl.BaseControl.ActiveDocument, new gPoint(c.sp.X, c.sp.Y), new gPoint(c.ep.X, c.ep.Y)));
                }
                routeInfo.LstAutoGuidedRoute = lag;

                var connector = new List<vdPolyline>() { };
                foreach (var c in lstConnector)
                {
                    var Cache = GetFigureById(c, vdFramedControl);
                    if (Cache != null && Cache is vdPolyline vp)
                    {
                        connector.Add(vp);
                    }
                    // connector.Add(new vdLine(vdFramedControl.BaseControl.ActiveDocument, new gPoint(c.sp.X, c.sp.Y), new gPoint(c.ep.X, c.ep.Y)));
                }
                routeInfo.lstConnector = connector;


                routeInfo.DEFAULT_OPTION = (RouteInfo.eSCOPE_MODE)DEFAULT_OPTION; // 0 udr,1 ar,2 agr



                var riex = new List<Routes>();
                foreach (var cr in CaseRoutes)
                {
                    try { 
                    var rts = new Routes();
                    rts.routeID = cr.routeID;
                    var lcie = new List<Connector>();
                    foreach (var ciex in cr.connectors)
                    {
                        var cie = new Connector();
                        cie.connectorID = ciex.connectorId;
                        cie.line = new vdLine(vdFramedControl.BaseControl.ActiveDocument, new gPoint(ciex.sp.X, ciex.sp.Y), new gPoint(ciex.ep.X, ciex.ep.Y));
                        // cie.line = GetFigureById(ciex.line, vdFramedControl) as vdLine;
                        lcie.Add(cie);
                    }
                    rts.bend = cr.bend;
                    rts.connectors = lcie;
                    riex.Add(rts);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.CaseRoutes = riex;


                var ade = new List<DuctLineEntity>();
                foreach (var c in lstDuctLines)
                {
                    try
                    {
                        DuctLineEntity ad = new DuctLineEntity();
                        ad.SegmentName = c.SegmentName;
                        ad.SignalType = c.SignalType;
                        //ad.ductId = GetFigureById(c.ductId, vdFramedControl);// c.ductId;
                        var dl1 = GetFigureById(c.ductId, vdFramedControl);
                        if (dl1 != null)
                            ad.ductId = dl1.Id;
                        ad.sp = new gPoint(c.sp.X, c.sp.Y);
                        ad.ep = new gPoint(c.ep.X, c.ep.Y);
                        ad.colorIndex = c.colorIndex;
                        var dl2 = GetFigureById(c.oline, vdFramedControl);
                        if (dl2 != null)
                            ad.oline = dl2 as vdLine;
                        ad.osp = new gPoint(c.osp.X, c.osp.Y);
                        ad.oep = new gPoint(c.oep.X, c.oep.Y);
                        ad.fp = new gPoint(c.fp.X, c.fp.Y);
                        ad.IsVerticle = c.IsVerticle == 1;
                        ad.IsBolder = c.IsBolder == 1;
                        ad.Cables = c.Cables;
                        ad.DuctTypeName = c.DuctTypeName;
                        ade.Add(ad);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);
                    }
                }
                routeInfo.lstDuctLines = ade;

                routeInfo.lstResult1 = lstResult1;
                routeInfo.lstResult2 = lstResult2;
                routeInfo.lstResult3 = lstResult3; 
                routeInfo.BasicInfo = basicSettingEntity;

            }
            catch(Exception ex)
            {
                DebugLog.WriteLog(ex);
            }
            return routeInfo;

        }

        public vdFigure GetFigureById(string Id, vdControls.vdFramedControl vdFramedControl1 = null)
        {
            if (string.IsNullOrEmpty(Id)) return null;
            var et = vdFramedControl1.BaseControl.ActiveDocument.ActionLayout.Entities;
            foreach (vdFigure e in et.GetNotDeletedItems())
            {
                if (e.URL == Id)
                    return e;
            }

            return null;
        }


    }
}
