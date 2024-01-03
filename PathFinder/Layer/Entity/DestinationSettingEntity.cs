using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Entity
{
   public class DestinationSettingEntity
    {
        //public string Title { get; set; }
        //public List<TBBOXDestination> lstTBBox { get; set; } 
        //public string CableType { get; set; }
        //public string AutoCheck { get; set; }
        //public string EachCheck { get; set; } 
        //public TBBOXDestination OwnDestination { get; set; } = null;
        public TBBOXDestination TBBOXDestination { get; set; }

        public RouteOptimizer.Object.RouteInfo RouteInfo { get; set; }
     
    }
}
