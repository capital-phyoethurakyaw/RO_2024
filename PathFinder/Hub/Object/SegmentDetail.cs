using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Object
{
    public class SegmentDetail
    {
        public vdLine LstGuidedRoute = new vdLine() { };
        public vdText SegmentName = new vdText() { };
        public SegmentDetail()
        {

        }
    }
}
