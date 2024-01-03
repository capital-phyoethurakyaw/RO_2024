using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Analysis
{
    public class Route : IComparable
    {
        public TBBOXDestination destination;
        public List<Route> routes = new List<Route>();

        public List<Connector> delectedConnector = new List<Connector>();

        public List<SubRoute> subRoutes = new List<SubRoute>();

        public List<vdLine> lines = new List<vdLine>();

        public double length = 0;
        public int bend = 0;
        public string name = "";
        public string id = "";

        public List<Node> nodes = new List<Node>();
        public List<Connector> connectors = new List<Connector>();

        public List<Connector> disconnectedConnectors = new List<Connector>();
        public List<Instrument> disconnectedInstruments = new List<Instrument>();


        public int getBendCount()
        {
            bend = 0;
            foreach (Node n in nodes)
            {
                if (n.nodes.Count > 1) bend++;
            }
            return bend;
        }


        public double getLength()
        {
            this.length = 0;
            foreach (Connector c in this.connectors)
            {
                this.length += c.getLength();
            }


            return this.length;
        }

        public double MaxLengthSegment()
        {
            return connectors.Max(o => o.line.Length());
        }

        public Route(TBBOXDestination destination)
        { 

            this.destination = destination;
        }


        public int CompareTo(object obj)
        {
            return length.CompareTo(obj);
        }

        public override string ToString()
        {
            return this.name;
        }

    }
}
