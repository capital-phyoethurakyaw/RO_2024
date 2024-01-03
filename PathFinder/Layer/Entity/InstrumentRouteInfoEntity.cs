using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.Entity
{
   public class InstrumentRouteInfoEntity
    {
        public string SignalType { get; set; }
        public string SystemType { get; set; }

        public Instrument Instrument { get; set; }
        public string CableType { get; set; }
        //public string SegmentName { get; set; }
        //public gPoint StartPoint { get; set; }
        //public gPoint EndPoint { get; set; }
        public List<string> LstSegment { get; set; }
        public double Length { get; set; }
        public string To { get; set; }
        //public TBBOXDestination IsTB { get; set; }
    }
}
