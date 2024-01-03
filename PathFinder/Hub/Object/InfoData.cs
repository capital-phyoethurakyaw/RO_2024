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
namespace RouteOptimizer.Object
{
    public class InfoData
    {
        public string IOABBRE = "IO-";
        public string TBABBRE = "TB-";
        public string xgap = "";
        public string ygap = "";
        public string SystemFile = null;
        public vdPolyline OffSetBoundary = null;
        public int MaxPoly = 50;
        public List<vdPolyline> HighlightedRoute = new List<vdPolyline>();
        public List<vdPolyline> HighlightedDestination = new List<vdPolyline>();
        public List<IGroup> LstUserBoundary = new List<IGroup>();
        public List<Instrument> allInstruments = new List<Instrument>();

        public vdPolyline MainBoundary = new vdPolyline();
        public List<TBBOXDestination> LstTBBOXes = new List<TBBOXDestination>();
        //public IORoom MainDestination = new IORoom();
        public List<Obstacle> LstObstacles = new List<Obstacle>();
        public List<Layer> LstLayer = new List<Layer>();
        public List<InstrumentInfoEntity> DGV_LstInstrument = new List<InstrumentInfoEntity>();
        public int SelectedGUID = 0;
        public InstrumentInfoEntity SelectedInstrumentInfoEntity = null;
        public eACTION_MODE CURRENT_MODE { get; set; } = eACTION_MODE.INSTRUMENT;
        public eVDFigure CURRENT_FIGURE { get; set; } = eVDFigure.POLYLINE;
        public enum eACTION_MODE : short
        {
            INSTRUMENT,
            OBSTACLE,
            DESTINSTION,
            BOUNDARY,
            MAINROUTE
        }
        public enum eVDFigure : short
        {
            POLYLINE,
            RECTANGLE,
            CIRCLE,
            IRREGULAR
        }
        public enum eDestinationType : short
        {
            TBBox,
            IORoom,
        }
    }
}
