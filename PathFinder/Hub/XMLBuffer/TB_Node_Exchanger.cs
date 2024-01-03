using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.XMLBuffer
{
    public class TB_Node_Exchanger : ICloneable
    {

        public TB_Node_Exchanger()
        {

        }
        public object Clone()
        {
          return this.MemberwiseClone();
        }
    }
}
