using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.Analysis
{
    public class Node
    {
        public string nodeID = "";
        public gPoint gp { get; set; }
        public List<Node> nodes { get; set; }
        public List<Connector> connectors { get; set; }

        public Node()
        { 
        }
        public Node(gPoint gp)
        {
            nodes = new List<Node>();
            connectors = new List<Connector>();
            this.gp = gp;

        }
    }
}
