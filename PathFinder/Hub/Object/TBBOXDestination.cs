namespace RouteOptimizer
{
    using RouteOptimizer.Analysis1;
    using RouteOptimizer.Entity;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VectorDraw.Geometry;
    using VectorDraw.Professional.vdFigures;
    using VectorDraw.Professional.vdPrimaries;
    using static RouteOptimizer.Object.RouteInfo;

    public class TBBOXDestination  
    {
        public Color color = Color.White;
        public List<Route> routes = new List<Route>();//new
        public Route selectedRoute ;//new
        public List<vdLine> routeLines = new List<vdLine>(); 
        public int[,] mMatrix = null; 
        public MatrixPoint[,] mps = null; 
        public MatrixPoint mp=null; 
        public List<Instrument> lstInstrument = new List<Instrument>();//new

        public int id { get; set; } = 0;
        public int guid = 0;
        //public int gridIndex = 0;
        //public gPoint gridPoint= new gPoint();
        public gPoint gridPoint;
        public int gridIndex = -1;
        public vdPolyline polyline=null;
        
        public gPoint centerPoint= new gPoint();
        public vdPolyline tbBoxBoundary =null;
        public string Name { get; set; } = "";
        public int NameId { get; set; } = 0;
        //public bool IsIO { get; set; } =false;//
        public eDestinationType IsIO { get; set; } = eDestinationType.TBBox;
        public bool UsedEndPoint = false;
        public bool UsedInstrument = false;
        public vdCircle Circle = new vdCircle();
        public eROUTE_MODE RouteState { get; set; } = eROUTE_MODE.NOROUTE;
        

        public List<vdLine> MainRouteCollection { get; set; } = new List<vdLine>() { };

        #region OutputCableProperties
        //public string LstCableEntities { get; set; }= new s
        public string CableType { get; set; } = "";//Old Singular Logic
        public string AutoCheck { get; set; } = "0";
        public string EachCheck { get; set; } = "0";
        public TBBOXDestination OwnDestination { get; set; } = null;
        #endregion
        #region MCCCableProperties
        public List<MCCEntity> LstmCCEntities { get; set; } = new List<MCCEntity>();  //Used for Both TB and MCC
        public List<MCCEntity> LstmCCEntitiesHeader { get; set; } = new List<MCCEntity>();  
                                                                                     
                                                                                      

        #endregion



        public TBBOXDestination()
        {
           
        }
        public TBBOXDestination(vdPolyline polyline, eDestinationType IsIO = eDestinationType.TBBox)
        {
            this.polyline = polyline;
            if (polyline.BoundingBox.Height > polyline.BoundingBox.Width)
                this.Circle = new vdCircle(polyline.Document, polyline.BoundingBox.MidPoint, polyline.BoundingBox.Width / 2);
            else
                this.Circle = new vdCircle(polyline.Document, polyline.BoundingBox.MidPoint, polyline.BoundingBox.Height / 2);

            this.guid = polyline.Id;
            this.centerPoint = polyline.BoundingBox.MidPoint;
            this.IsIO = IsIO;
        }
        public TBBOXDestination(vdPolyline polyline, gPoint  centerpt )
        { 
           // this.guid = polyline.Id;
            this.centerPoint = centerpt; 
        }
        public void SetName(string TBBoxName, int tbid)
        {
            
            this.Name = TBBoxName;
            if (tbid != 0)
            this.NameId = tbid;
        }
        public void SetOwnDestination(TBBOXDestination OwnDes)
        {
            this.OwnDestination = OwnDes;
        }
      
        public override string ToString()
        { 
            return Name;
        }
    }
}
