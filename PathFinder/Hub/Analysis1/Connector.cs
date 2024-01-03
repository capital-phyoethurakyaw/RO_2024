//using DAP_1.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;

namespace RouteOptimizer.Analysis1
{
    public class Connector : IComparable 
    {
     public   Node sNode;
        public Node eNode; 
        public string connectorID = "";
        public vdLine line;

       public List<Instrument> instruments= new List<Instrument>();    
        

        public Connector(Node sNode, Node eNode, vdDocument doc, TBBOXDestination destination) { 
             this.sNode= sNode;
            this.eNode= eNode;
            line= new vdLine(doc, sNode.gp, eNode.gp);
            line.PenColor  = new vdColor(destination.color);
            List<TBBOXDestination> destinations= new List<TBBOXDestination>();
            destinations.Add(destination);
            line.XProperties.Add("Destinations", destinations);
        }
        public Connector()
        {

        }
        public Connector(Node sNode, Node eNode, vdDocument doc, List<TBBOXDestination> destinations)
        {
            this.sNode = sNode;
            this.eNode = eNode;
            line = new vdLine(doc, sNode.gp, eNode.gp);
            if (destinations != null) {
                int r =0 , g = 0   ,   b= 0;
                foreach (TBBOXDestination dest in destinations)
                {
                    r += dest.color.R;
                    g += dest.color.G;
                    b += dest.color.B;
                }
                r = r/destinations.Count;
                g = g / destinations.Count;
                b = b / destinations.Count;

                line.PenColor = new vdColor(Color.FromArgb( r,g,b) );
            }
            line.XProperties.Add("Destinations", destinations);
        }


        public int CompareTo(object other)
        {
            return instruments.Count.CompareTo(other);
        }

        public double getLength() {



            return sNode.gp.Distance2D(eNode.gp); ;
        }

    }
}
