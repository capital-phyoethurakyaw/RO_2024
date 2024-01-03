using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer.Analysis
{
    public class SubRoute : ICloneable
    {
        public string name = "";

        //public Point sp { get; set; }
        //public Point ep { get; set; }
        public Instrument instrument;
        public TBBOXDestination destination; 
        public List<Point> points = new List<Point>();
        public List<MatrixPoint> matrixPoint = new List<MatrixPoint>(); 
        public gPoints gps = new gPoints(); 
        public gPoints polyGps = new gPoints(); 
        public vdPolyline poly;
        public MatrixPoint startMP;
        public SubRoute(Instrument instrument, TBBOXDestination destination, MatrixPoint startMP)
        {

            this.instrument = instrument;
            this.destination = destination;
            this.points = instrument.point;
            this.startMP = startMP;
            //  this.points = instrument.
        }
        public SubRoute()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void setPoints(List<Point> ps, MatrixPoint[,] mps)
        {
            this.points = new List<Point>();
            matrixPoint = new List<MatrixPoint>();
            gps = new gPoints();
            for (int i = 0; i < ps.Count; i++)
            {
                Point p1 = ps[i];
                MatrixPoint m1 = mps[(int)p1.X / 2, (int)p1.Y / 2];
                p1.X = p1.X / 2;
                p1.Y = p1.Y / 2;
                gps.Add(new gPoint(p1.X, p1.Y));
                matrixPoint.Add(m1);
                this.points.Add(p1);
            }
            gps.RemoveInLinePoints();
            polyGps = new gPoints();
            foreach (MatrixPoint mp in this.matrixPoint)
            {
                polyGps.Add(mp.gp);
            }
            polyGps.RemoveInLinePoints();
        }


        public void setPoints2(MatrixPoint[,] mps)
        {

            gps.RemoveInLinePoints();

            this.points.Clear();
            matrixPoint = new List<MatrixPoint>();

            for (int i = 0; i < gps.Count; i++)
            {
                gPoint p1 = gps[i];
                MatrixPoint m1 = mps[(int)p1.x, (int)p1.y];
                matrixPoint.Add(m1);
            }
            polyGps = new gPoints();
            foreach (MatrixPoint mp in this.matrixPoint)
            {
                polyGps.Add(mp.gp);
            }
            poly = new vdPolyline(null, gps);

            polyGps.RemoveInLinePoints();
        } 
        public void setPoints(MatrixPoint[,] mps)
        {
            this.points.Clear();
            matrixPoint = new List<MatrixPoint>();

            for (int i = 0; i < gps.Count; i++)
            {
                gPoint p1 = gps[i];
                MatrixPoint m1 = mps[(int)p1.x, (int)p1.y];
                matrixPoint.Add(m1);
            }

            for (int i = 0; i < gps.Count - 1; i++)
            {
                gPoint p1 = gps[i];
                gPoint p2 = gps[i + 1];

                if (p1.x <= p2.x)
                {
                    if (p1.y <= p2.y)
                    {
                        for (double m = p1.x; m <= p2.x; m++)
                        {
                            for (double n = p1.y; n <= p2.y; n++)
                            {
                                this.points.Add(new Point((int)m, (int)n));
                            }
                        }
                    }
                }
                if (p1.x <= p2.x)
                {
                    if (p1.y >= p2.y)
                    {
                        for (double m = p1.x; m <= p2.x; m++)
                        {
                            for (double n = p2.y; n <= p1.y; n++)
                            {
                                this.points.Add(new Point((int)m, (int)n));
                            }
                        }
                    }
                }

                if (p1.x >= p2.x)
                {
                    if (p1.y <= p2.y)
                    {
                        for (double m = p2.x; m <= p1.x; m++)
                        {
                            for (double n = p1.y; n <= p2.y; n++)
                            {
                                this.points.Add(new Point((int)m, (int)n));
                            }
                        }
                    }
                }
                if (p1.x >= p2.x)
                {
                    if (p1.y >= p2.y)
                    {
                        for (double m = p2.x; m <= p1.x; m++)
                        {
                            for (double n = p2.y; n <= p1.y; n++)
                            {
                                this.points.Add(new Point((int)m, (int)n));
                            }
                        }
                    }
                }
            }

            gps.RemoveInLinePoints(); 
            polyGps = new gPoints();
            foreach (MatrixPoint mp in this.matrixPoint)
            {
                polyGps.Add(mp.gp);
            }
            polyGps.RemoveInLinePoints(); 
        } 
    }
}
