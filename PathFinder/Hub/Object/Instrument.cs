using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdPrimaries;
using RouteOptimizer.Analysis1;
using System.Drawing;

namespace RouteOptimizer
{
    public class Instrument : IComparable, ICloneable
    {
        public int index = 0;
        public int gridIndex = 0;
        public gPoint gridPoint= new gPoint();
        public vdCircle circle= null;
        public double distance=0.0;
        public double distanceFromDestination=0.0;
        public string t1= "";
        public string t2="";
        public vdInsert OwnerInsert =null;  
        public gPoint centerPoint { get; set; } = new gPoint();
        public vdCircle instrumentBoundary = new vdCircle();
        public int guid = 0;
        public bool IsTBBox= false;
        public bool IsReserved= false;
        public bool IsHighlight= false;
        public Dictionary<string, double> Distance2Destination { get; set; }

        //
        public gPoint PossessPoint = new gPoint();
        public TBBOXDestination PosssessDestination = new TBBOXDestination();
        public vdLine PosssessRoute= new vdLine();

        public MatrixPoint mp; 
        public List<MatrixPoint> mps = new List<MatrixPoint>(); 
        public List<SubRoute> routes = new List<SubRoute>();

        public List<Point> point = new List<Point>();
        public TBBOXDestination destination = null;
        //public List<TBBOXDestination> LstLinkDestination { get; set; } = new List<TBBOXDestination>();


        public Instrument(vdCircle circle, string t1 = null, string t2 = null, bool IsTBBox = false, vdInsert vdInsert=null)
        {
            this.circle = circle;
            this.centerPoint = circle.Center;
            this.guid = circle.Id; 
            this.t1 = t1;
            this.t2 = t2;
            this.IsTBBox = IsTBBox;
            OwnerInsert = vdInsert;
        }
        public Instrument(vdCircle circle, int index)
        {
            this.circle = circle;
            this.centerPoint = circle.Center;
            this.index = index;
        }
        public Instrument()
        {

        }

        public int CompareTo(object obj)
        {
            return distanceFromDestination.CompareTo(obj);
        }

        public object Clone()
        {
            return this.Clone();
        }
    }
}