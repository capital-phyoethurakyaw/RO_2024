using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdPrimaries;

namespace RouteOptimizer
{
    public class IO_Room
    {
        public int guid=0;
        public gPoint gridPoint;
        public vdPolyline polyline;
        public int gridIndex = -1;
        public gPoint centerPoint;
        public vdPolyline ioRoomBoundary = new vdPolyline();
        public string Name { get; set; }
        public int NameId { get; set; }
        public IO_Room(vdPolyline polyline, gPoint centerpt = null)
        {
            if (centerpt != null) 
                this.centerPoint = centerpt;  
            else
                this.centerPoint = polyline.BoundingBox.MidPoint;
            this.polyline = polyline;
            this.guid = polyline.Id;
          
        }
        public IO_Room()
        {

        }
        public void SetName(string IOName, int tbid)
        {
            this.Name = IOName;
            this.NameId = tbid;
        }
    }
}