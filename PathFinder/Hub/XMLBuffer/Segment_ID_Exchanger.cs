using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.XMLBuffer
{
  public  class Segment_ID_Exchanger
    {
        public string SignalType { get; set; }
        public string SegmentName { get; set; }
        public gPoint StartPoint { get; set; }
        public gPoint EndPoint { get; set; }
        public double Length { get; set; }
        public string ParentRouteID { get; set; }
        public string ParentDestination { get; set; } = "";
        public List<string> CableList { get; set; }
        //public double OptimalDuctSize { get; set; }

        public double A_TotalArea { get; set; }
        public string A_OptimalDuctSize { get; set; }
        public double A_ActualOptimalResult { get; set; }
        public double A_OptimalRatio { get; set; }

        public string A_UserDefinedSize { get; set; }
        public double A_UserDefinedRato { get; set; }
    }
}
