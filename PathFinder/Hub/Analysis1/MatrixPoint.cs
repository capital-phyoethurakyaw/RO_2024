using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.Analysis1
{
    public class MatrixPoint
    {
        public int x;
        public int y;
        public gPoint gp;
        public MatrixPoint(gPoint gp, int x, int y)
        {
            this.gp = gp;
            this.x = x;
            this.y = y;

        }


    }
}
