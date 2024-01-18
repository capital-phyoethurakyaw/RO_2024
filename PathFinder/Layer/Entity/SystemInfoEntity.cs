using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Entity
{
    public class SystemInfoEntity
    {
        //public string Id { get; set; } 
        public string Title { get; set; }
        public string NW_W { get; set; }
        public string NW_D { get; set; }
        public string RIO_H { get; set; }
        public string RIO_Check { get; set; }
        public string PLC_Qty { get; set; }
        public string PLC_Sl { get; set; }
        public string PLC_Sl_Spare { get; set; }
        public string PLC_DI { get; set; }
        public string PLC_DO { get; set; }
        public string PLC_AI { get; set; }
        public string PLC_AI_C { get; set; }
        public string PLC_AO { get; set; }
        public string RTD { get; set; }
        public string IO_Ratio { get; set; }

        //public override string ToString()
        //{
        //    return Title;
        //}
    }
}
