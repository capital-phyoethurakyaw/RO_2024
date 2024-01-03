using RouteOptimizer.Analysis1;
using RouteOptimizer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RouteOptimizer.Object.RouteInfo;

namespace RouteOptimizer.XMLBuffer
{
    public class TB_ID_Exchanger
    {
        //public string vdText = "";
        //public string vdpolyline = "";
        //public bool IsIO = false;
        //public string OwnDestination = "";
        //public List<string> MainRoute = new List<string>() { };


        //public string CableType { get; set; }
        //public string AutoCheck { get; set; }
        //public string EachCheck { get; set; }
        //public TB_ID_Exchanger()
        //{

        //}
        public string vdText = "";
        public string vdpolyline = "";
        //public bool IsIO = false;
        public int IsIO { get; set; }=0;

        // public eDestinationType IsIO { get; set; }= eDestinationType.TBBox;
        public string OwnDestination = "";
        public List<string> MainRoute = new List<string>() { };

        public List<string> routeLines = new List<string>();//new
        //public int[,] mMatrix { get; set; }//new
        //public MatrixPoint[,] mps = null;//new
        //public MatrixPoint mp = null;//new
        public List<string> lstInstrument = new List<string>() { };


        public string CableType { get; set; }
        public string AutoCheck { get; set; }
        public string EachCheck { get; set; }
        public List<MCCEntity> LstmCCEntities { get; set; }
        public List<MCCEntity> LstmCCEntitiesHeader { get; set; } = new List<MCCEntity>();

        public TB_ID_Exchanger()
        {

        }

    }
}
