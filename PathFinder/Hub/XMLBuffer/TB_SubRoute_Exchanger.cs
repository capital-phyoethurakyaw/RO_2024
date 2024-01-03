using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.XMLBuffer
{
    public class TB_SubRoute_Exchanger : ICloneable
    {
        public string a = "";
        public TB_SubRoute_Exchanger()
        {

        }
        public object Clone()
        {
          return  this.MemberwiseClone();
        }
    }
}
