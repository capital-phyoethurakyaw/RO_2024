using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.XMLBuffer
{
    public class Ana_DuctLine_Exchanger
    {
        public Ana_DuctLine_Exchanger()
        {

        }
        public string SegmentName { get; set; }
        public string SignalType { get; set; }
        public string ductId { get; set; }//ductline = "";
        public Point sp { get; set; }
        public Point ep { get; set; }
        public int colorIndex { get; set; } = 9; // 9 is none
        //public int orderId { get; set; }
        //public double lw { get; set; }
        public string oline { get; set; }
        public Point osp { get; set; }
        public Point oep { get; set; }
        public Point fp { get; set; }
        public int IsVerticle = 0;
        public int IsBolder = 0;
        public int Cables { get; set; } = 0;
        public string DuctTypeName { get; set; }
    }
}
