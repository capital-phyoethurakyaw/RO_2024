namespace RouteOptimizer.Object
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VectorDraw.Geometry;
    using VectorDraw.Professional.vdFigures;



    public class Destination
    {
       public vdPolyline boundary;
        public gPoint center;


        public gPoint gridPoint;
        public int gridIndex = -1;
        public Destination(vdPolyline boundary)
        {
            this.center = boundary.BoundingBox.MidPoint;
            this.boundary = boundary;

        }
    }
}
