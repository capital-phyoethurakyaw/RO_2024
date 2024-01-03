using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Entity
{
    public class DestinationPathEntity
    {
        public List<List<vdLine>> LstPath { get; set; }  
        public IEnumerable<Instrument> selectedIns { get; set; }
        public DestinationPathEntity()
        {

        }
    }
}
