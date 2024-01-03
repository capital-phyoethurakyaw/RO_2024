using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer
{
    internal class PointData
    {
        public PointF Location;
        public int ClusterNum;
        public PointData(PointF location, int cluster_num)
        {
            Location = location;
            ClusterNum = cluster_num;
        }
        public PointData(float x, float y, int set)
            : this(new PointF(x, y), set)
        {
        }
    }
}