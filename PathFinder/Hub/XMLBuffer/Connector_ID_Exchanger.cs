using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RouteOptimizer.XMLBuffer
{
   public class Connector_ID_Exchanger
    {
        public string connectorId = "";
        public string line = "";
        public Point sp { get; set; } =new Point();
        public Point ep =new Point(); 
        public Connector_ID_Exchanger()
        {

        }
    }
}
