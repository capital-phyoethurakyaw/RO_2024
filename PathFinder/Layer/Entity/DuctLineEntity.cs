using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdObjects;
using System.Drawing;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Entity
{
    public class DuctLineEntity
    {
        public string SegmentName { get; set; }
        public string SignalType { get; set; }
        public int ductId { get; set; } 
        public gPoint sp { get; set; }
        public gPoint ep { get; set; }
        public int colorIndex { get; set; } = 9; // 9 is none
        //public int orderId { get; set; }
        //public double lw { get; set; }
        public vdLine oline { get; set; }
        public gPoint osp { get;  set; }
        public gPoint oep { get; set; }
        public gPoint fp { get; set; }
        public bool IsVerticle = false;
        public bool IsBolder = false;
        public int Cables { get; set; } = 0; 
        public string DuctTypeName { get; set; } 

    }
}
