using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Object
{
    public class IGroup
    {
        public int GUID=0;
        public List<Instrument> Inscribedinstruments = new List<Instrument>();
        public vdPolyline Boundary = new vdPolyline();
        public IGroup(vdPolyline vdPolyline, List<Instrument> lstIns)
        {
            this.GUID = vdPolyline.Id ;
            this.Boundary = vdPolyline;
            foreach (var ins in lstIns)
            {
                if (vdPolyline.BoundingBox.PointInBox(ins.centerPoint))
                {
                    Inscribedinstruments.Add(ins);
                }
            }
        }
        public IGroup( )
        {
         
        }
    }
}
