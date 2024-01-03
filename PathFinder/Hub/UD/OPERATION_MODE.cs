using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer
{
   public class OPERATION_MODE
    {
        public enum EOperationMode : short
        {
            INSERT,
            DELETE,
            UPDATE,
            SHOW,
            PRINT,
            SAVEAS,
            IDLE

        }
        
    }
 
}
