// Upload to codeproject.com
// By Ibraheem AlKilanny
// d3_ib@hotmail.com - http://sites.google.com/site/ibraheemalkilany/
// all rights reserved 2011

using RouteOptimizer.Analysis1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VectorDraw.Geometry;

namespace RouteOptimizer.Analysis1
{
    public partial class DisForm : System.Windows.Forms.Form
    {
        int algorithmIndex =0;
        private Graph graph;
        private Graph.Algorithms algorithm;
        public DisForm()
        {
            InitializeComponent();
            this.graph = new Graph(this.pictureBoxDraw.Width, this.pictureBoxDraw.Height,
                new Size(this.pictureBoxDraw.Width / Vertex.Size, this.pictureBoxDraw.Height / Vertex.Size));
            this.graph.PathFound += new EventHandler(graph_PathFound);
        }

        public List<Point> setInit(int[,] matrix, Point startPoint, Point endPoint, int algorithmIndex)
        {
            Vertex.Size = 2;
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            this.pictureBoxDraw.Width = width * Vertex.Size;
            this.pictureBoxDraw.Height = height * Vertex.Size;

            this.graph = new Graph(width * 10, height * 10, new Size(width, height));
            this.graph.PathFound += new EventHandler(graph_PathFound);
            this.graph.Start = startPoint;
            this.graph.End = endPoint;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point loc = new Point(i * Vertex.Size, j * Vertex.Size);
                    Point po = new Point(i, j);
                    graph.verteces[po.Y, po.X] = new Vertex(loc, po, matrix[i, j]);
                }
            }
            this.algorithmIndex = algorithmIndex;

            this.graph.PathFinding((Graph.Algorithms)algorithmIndex);//this.comboBox1.SelectedIndex

            List<Point> gps = new List<Point>();
            foreach (Vertex v in this.graph.foundPath)
            {
                Point gp = new Point(v.Location.X, v.Location.Y);
                gps.Add(gp);
            }
            return gps;
        }




        public gPoints getPath(int[,] matrix, Point startPoint, Point endPoint)
        {


            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point loc = new Point(i * Vertex.Size, j * Vertex.Size);
                    Point po = new Point(i, j);
                    if (matrix[i, j] == int.MaxValue) graph.verteces[po.Y, po.X].Walkable = false;
                    else graph.verteces[po.Y, po.X].Walkable = true;
                    graph.verteces[po.Y, po.X].Walkable2 = matrix[i, j];

                }
            }
            this.graph.Reset(false);
            this.algorithm = (Graph.Algorithms)algorithmIndex;//this.comboBox1.SelectedIndex;



            this.graph.PathFinding(this.algorithm);

            int co = 0;
            gPoints gps = new gPoints(); ;
            foreach (Vertex v in this.graph.foundPath)
            {
                gPoint gp = new gPoint(v.Location.X, v.Location.Y);
                gps.Add(gp);
                co++;
            }

            return gps;
        }



        private void graph_PathFound(object sender, EventArgs e)
        {

        }





    }
}
