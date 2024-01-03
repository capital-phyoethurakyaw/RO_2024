using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Entity
{
    public class InstCableEntity
    {
        public string InstType { get; set; }
        public string Type { get; set; }
        public string System { get; set; }
        public string Cable { get; set; }
        public string No { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Abb { get; set; }
        public string S_Check { get; set; } = "0";
        public string col_Signal { get; set; } 
        public string col_Catetegory { get; set; }


    }
}
