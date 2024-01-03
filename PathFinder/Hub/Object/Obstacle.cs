using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;

namespace RouteOptimizer
{
    public class Obstacle
    {
        public vdFigure vdObstacle =null;//public vdPolyline vdObstacle= new vdPolyline();
        public double distance = 0.0;
        public double distanceFromDestination = 0.0;
        public int guid = 0;
        public gPoint centerPoint = new gPoint(); 
        public Obstacle(vdFigure figure)
        {
            this.vdObstacle = figure;
            this.centerPoint = figure.BoundingBox.MidPoint;
            this.guid = figure.Id;
        }
        public Obstacle()
        {

        }
    }
}