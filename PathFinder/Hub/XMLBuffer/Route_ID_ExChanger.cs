using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.XMLBuffer
{
    public class Route_ID_ExChanger
    {
        public string routeID = "";
        public int bend =0;
        public List<Connector_ID_Exchanger> connectors { get; set; }
        public Route_ID_ExChanger()
        {

        }
    }
}
