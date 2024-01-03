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
    public class Routes : IComparable
    {
        public string routeID = ""; 
        public List<Route> routes = new List<Route>();

        // FOR THE RUTES OF MULTIPLE DESTINATION
        // RESULT
        public List<vdLine> lines = new List<vdLine>();

        public List<Node> nodes = new List<Node>();
        public List<Connector> connectors = new List<Connector>();
        public List<Connector> selectedConnectors = new List<Connector>();
        public List<Instrument> selectedInstruments = new List<Instrument>();

        //public List<Connector> connectors = new List<Connector>();
        //public List<Connector> selectedConnectors = new List<Connector>();
        //public List<Instrument> selectedInstruments = new List<Instrument>();

        public double length = 0;
        public int bend = 0;


        public int getBendCount()
        {
            //bend = 0;
            if (bend != 0) return bend;
            if (nodes == null)
                nodes = new List<Node>();
            foreach (Node n in nodes)
            {
                if (n.nodes.Count > 1) bend++;
            }
            return bend;
        }


        public double getLength()
        {
            this.length = 0;

            if (connectors == null)
                connectors = new List<Connector>();
            foreach (Connector c in this.connectors)
            {
                this.length += c.line.Length(); //c.getLength();
            }
            return this.length;
        }
        public double MaxLengthSegment()
        {
            if (connectors == null)
                connectors = new List<Connector>();
            try
            {
                if (connectors.Count > 0)
                return connectors.Where(c => c.line != null).Max(o => o.line.Length());
            }
            catch(Exception ex)
            {
                
            }
            return 0;
        }


        public Routes()
        {
        }


        public int CompareTo(object obj)
        {
            return length.CompareTo(obj);
        }


        public bool getJoin()
        {
            foreach (Connector c in this.connectors)
            {
                List<TBBOXDestination> destinations1 = (List<TBBOXDestination>)c.line.XProperties["Destinations"];

                if (destinations1.Count > 1) return true;
            }

            return false;
        }


        public override string ToString()
        {
            return "";
        }

    }
}
