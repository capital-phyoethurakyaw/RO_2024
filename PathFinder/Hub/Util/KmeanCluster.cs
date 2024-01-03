using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;

namespace RouteOptimizer.Util
{
    public class KmeanCluster
    {
        // (List<Instrument>, List<PointF>, List<PointData>)
        internal static void GetCentriod(ref List<Instrument> instruments, ref List<PointF> Centroids, ref List<PointData> Points)
        {
            if (Points.Count > 0) return;
            Points.Clear();
            foreach (var lst in instruments)
                Points.Add(new PointData(new System.Drawing.Point(Convert.ToInt32(lst.centerPoint.x), Convert.ToInt32(lst.centerPoint.y)), 0));
            Centroids.Clear();

            int num_clusters = 1;
            if (Points.Count < num_clusters) return;

            Centroids = new List<PointF>();
            Points.Randomize();
            for (int i = 0; i < num_clusters; i++)
                Centroids.Add(Points[i].Location);
            foreach (PointData point_data in Points)
                point_data.ClusterNum = 0;

            num_clusters = Centroids.Count;
            PointF[] new_centers = new PointF[num_clusters];
            int[] num_points = new int[num_clusters];
            foreach (PointData point in Points)
            {
                double best_dist =
                    Distance(point.Location, Centroids[0]);
                int best_cluster = 0;
                for (int i = 1; i < num_clusters; i++)
                {
                    double test_dist =
                        Distance(point.Location, Centroids[i]);
                    if (test_dist < best_dist)
                    {
                        best_dist = test_dist;
                        best_cluster = i;
                    }
                }
                point.ClusterNum = best_cluster;
                new_centers[best_cluster].X += point.Location.X;
                new_centers[best_cluster].Y += point.Location.Y;
                num_points[best_cluster]++;
            }

            List<PointF> new_centroids = new List<PointF>();
            for (int i = 0; i < num_clusters; i++)
            {
                new_centroids.Add(new PointF(
                    new_centers[i].X / num_points[i],
                    new_centers[i].Y / num_points[i]));
            }

            Centroids = new_centroids;
        }
        static double Distance(PointF point1, PointF point2)
        {
            float dx = point1.X - point2.X;
            float dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}