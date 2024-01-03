
namespace RouteOptimizer.Util
{
    using RouteOptimizer.Object;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using VectorDraw.Geometry;
    using VectorDraw.Professional.vdCollections;
    using VectorDraw.Professional.vdFigures;

    public class Nearestpoint
    {
        gPoints gPoints = new gPoints();
        public int Unit = 1000;
        public void GetNearestpoint(TBBOXDestination des, RouteInfo routeInfo, int gapgrid)
        {
            Unit = gapgrid;
            // closestpoint = null;
            double gap = 1000;
            vdCurves c = routeInfo.MainBoundary.getOffsetCurve(gap);
            if (c.Count==0)
            {
                while (c.Count  == 0)
                {
                    gap += 100;
                    c = routeInfo.MainBoundary.getOffsetCurve(gap);
                }
            }
            routeInfo.OffSetBoundary = new vdPolyline(c.Document, c[0].GetGripPoints());


            if (routeInfo.OffSetBoundary == null)
            {
                MessageBox.Show("Cant Detect boundary or no exist that kind of boundary layer." + Environment.NewLine + "Please make sure boundary layer exist.");
                return;
            }
            Box box = routeInfo.OffSetBoundary.BoundingBox;

            double sx = box.Left;
            double sy = box.Bottom;
            int wGap = Unit * int.Parse(routeInfo.xgap);
            int hGap = Unit * int.Parse(routeInfo.ygap);
            //int wGap = (200 * 2) * int.Parse(routeInfo.ygap); 
            //int hGap = (200 * 2) * int.Parse(routeInfo.ygap); 
            int wc = (int)box.Width / (wGap) + 1;
            int hc = (int)box.Height / (hGap) + 1;

            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < wc; j++)
                {
                    int index = i * wc + j;
                    gPoint p = new gPoint(sx + j * wGap, sy + i * hGap);
                    gPoints.Add(p);
                }
            }
            double minDistance = double.MaxValue;
            int count = 0;
            foreach (gPoint gridPoint in this.gPoints)
            {
                double dis = des.centerPoint.Distance2D(gridPoint);
                if (minDistance > dis)
                {
                    minDistance = dis;
                    des.gridPoint = gridPoint;
                    des.gridIndex = count;
                }
                count++;
            }

        }
    }
}

