using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Entity
{
   public class BasicSettingEntity
    {
        public string MaxLengthInstrument { get; set; } 
        public string DuctOptimizedRatio { get; set; }
        public string HTolerence { get; set; }
        public string YTolerence { get; set; }
        public string Connectable { get; set; }
        public string PanelBay { get; set; }
        public string FrontPart { get; set; }
        public string RearClearence { get; set; }
        public string WallFront { get; set; }
        public string SideFront { get; set; }

    }
}
