using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.Analysis1
{
    public class Node
    {
        public Point p;
        public gPoint gp;

        public  List<Node> nodes;
        public  List<Connector> connectors ;


       public Node(gPoint gp) {
            nodes = new List<Node>();
            connectors = new List<Connector>();
            this.gp = gp;

        }
    }
}
