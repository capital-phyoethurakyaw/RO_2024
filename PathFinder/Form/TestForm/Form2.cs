using RouteOptimizer.Analysis1; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using static RenderFormats.PrimitiveRender3d;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskBand;

namespace RouteOptimizer
{
    public partial class Form2 : System.Windows.Forms.Form
    {
        List<Instrument> instruments = new List<Instrument>();
        List<vdPolyline> obstacles = new List<vdPolyline>();
        List<vdCircle> instrument = new List<vdCircle>();

        Route selectedRoute = new Route(null);


        Thread t;
        gPoints gridPoints = new gPoints();
        vdPolyline boundary;
        vdPolyline offsetBoundary;
        TBBOXDestination destination;
        double sx = 0;
        double sy = 0;

        List<Route> routes = new List<Route>();
        List<vdLine> routeLines = new List<vdLine>();
        bool isRun = true;

        int[,] mMatrix = null;
        MatrixPoint[,] mps = null;

        public Form2()
        {
            InitializeComponent();
            this.vdFramedControl1.BaseControl.DrawAfter += BaseControl_DrawAfter;
        }

        private void BaseControl_DrawAfter(object sender, VectorDraw.Render.vdRender render)
        {
            Graphics gr = this.vdFramedControl1.BaseControl.ActiveDocument.GlobalRenderProperties.GraphicsContext.MemoryGraphics; 
            drawRoute(render);
        }

        public void drawRoute()
        {
            vdLayer layer = vdFramedControl1.BaseControl.ActiveDocument.Layers.FindName("Route");
            if (layer == null)
            {
                layer = new vdLayer(this.vdFramedControl1.BaseControl.ActiveDocument, "Route");
                vdFramedControl1.BaseControl.ActiveDocument.Layers.Add(layer);

            }

            foreach (vdLine l in this.routeLines)
            {
                this.vdFramedControl1.BaseControl.ActiveDocument.Model.Entities.RemoveItem(l);
            }

            this.routeLines.Clear();



            foreach (Connector connector in this.selectedRoute.connectors)
            {

                connector.line.PenColor = new vdColor(Color.Yellow);
                this.vdFramedControl1.BaseControl.ActiveDocument.Model.Entities.AddItem(connector.line);
                this.routeLines.Add(connector.line);
                connector.line.Layer = layer;
            }

            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
             
        }


        public void drawRoute(VectorDraw.Render.vdRender render)
        {
            foreach (Connector connector1 in this.selectedRoute.connectors)
            {
                foreach (Instrument ins in connector1.instruments)
                {
                    gPoint gp = connector1.line.getClosestPointTo(ins.centerPoint); 
                    render.DrawLine(this.vdFramedControl1.BaseControl.ActiveDocument, ins.centerPoint, gp);

                }
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        { 
            t = new Thread(makeGrid); t.Start(); 
        }



        public void setMatrix()
        {
            Box box = this.offsetBoundary.BoundingBox;

            sx = box.Left;
            sy = box.Bottom;

            int wGap = 1000;// 1414;  //1.41421   1414//3000
            int hGap = wGap;
            int wc = (int)box.Width / wGap + 1;
            int hc = (int)box.Height / hGap + 1;
            int pCount = wc * hc;  
            mMatrix = new int[hc, wc]; 
            mps = new MatrixPoint[hc, wc];//
            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < wc; j++)
                {
                    gPoint p = new gPoint(sx + j * wGap, sy + i * hGap);
                    MatrixPoint mp = new MatrixPoint(p, i, j);
                    mps[i, j] = mp;

                    bool isIn = contains(this.boundary.VertexList, p);
                    // 바운더리 안에 있는지 없는지가 true / false로 나타남.
                    bool isIn2 = false;
                    foreach (vdPolyline obstacle in this.obstacles)
                    {
                        isIn2 = contains(obstacle.VertexList, p);
                        if (isIn2) break;
                    }
                    // obstacle 안에 노드가 있는지 없는지 true / false로 나타남.
                    mMatrix[i, j] = Graph.regularDistance;

                    // 갈 수 있으면 true, 못 가면 false.

                    if (!isIn || isIn2)
                    {
                        mMatrix[i, j] = int.MaxValue;
                    }
                }
            }
        }

        public void makeGrid()
        { 
            //setMatrix(); 
            //int[,] matrix = (int[,])this.mMatrix.Clone();
            //int minDistance = int.MaxValue;
            //int count = 0;

            //foreach (MatrixPoint mp in mps)
            //{ 
            //    var dis = (int)this.destination.centerPoint.Distance2D(mp.gp);
            //    if (minDistance > dis)
            //    {
            //        minDistance = dis;
            //        minDistance = dis;
            //        destination.mp = mp;
            //        destination.gridPoint = mp.gp;
            //        destination.gridIndex = count;
            //    }
            //    count++;
            //}
             
            //foreach (Instrument ins in this.instruments)
            //{
            //    minDistance = int.MaxValue;
            //    count = 0;
            //    foreach (MatrixPoint mp in mps)
            //    {
            //        int dis = (int)ins.centerPoint.Distance2D(mp.gp);
            //        if (minDistance > dis)
            //        {
            //            minDistance = dis;
            //            ins.distance = dis;
            //            ins.gridIndex = count;
            //            ins.mp = mp;
            //        }
            //        if (dis < 1000)
            //        {
            //            ins.mps.Add(mp);
            //        }
            //        count++;
            //    }
            //}


            //this.instruments = this.instruments.OrderBy(x => x.distanceFromDestination).ToList();


            ////주변으로 이동하도록 주변의 거리 매트릭스의 값을 낮춤
            //foreach (Instrument ins in this.instruments)
            //{
            //    Point sp1 = new Point(ins.mp.x, ins.mp.y);
            //    for (int o = -1; o < 1; o++)
            //    {
            //        for (int p = -1; p < 1; p++)
            //        {
            //            if (((ins.mp.x + o > 0) && (ins.mp.x + o) < matrix.GetLength(0))
            //               && ((ins.mp.y + p > 0) && (ins.mp.y + p) < matrix.GetLength(1)))
            //            {
            //                if (matrix[ins.mp.x + o, ins.mp.y + p] != int.MaxValue) matrix[ins.mp.x + o, ins.mp.y + p] = 4;
            //            }
            //        }
            //    }
            //}
            //////

            //int[,] matrix2 = (int[,])matrix.Clone();


            //DisForm disform = new DisForm();


            //double cou = -1;

            //isRun = true;
            //Random r = new Random();
            //while (cou < 100000 && isRun == true)
            //{
            //    cou++;
            //    Console.WriteLine("count " + cou);
            //    //Random으로 순서를 변경
            //    List<Instrument> instruments2 = new List<Instrument>();
            //    int[] array = getArray(instruments.Count);
            //    string ar = "";
            //    for (int g = 0; g < array.Length; g++)
            //    {
            //        instruments2.Add(this.instruments[array[g]]);
            //        ar += array[g] + ",";
            //    }


            //    Instrument ins0 = instruments2[0]; //3.4 7, 9 , 10* 11*
            //    int v = r.Next(ins0.mps.Count - 1);
            //    MatrixPoint mp = ins0.mps[v];
            //    while (matrix[mp.x, mp.y] == int.MaxValue)
            //    {
            //        v = r.Next(ins0.mps.Count - 1);
            //        mp = ins0.mps[v];
            //    }
            //    /////////////
            //    Point sp0 = new Point(mp.x, mp.y);

            //    ins0.mp = mp;
            //    Point ep0 = new Point(destination.mp.x, destination.mp.y);
            //    //Point sp0 = new Point(ins0.mp.x, ins0.mp.y);

            //    List<SubRoute> mainRoutes = new List<SubRoute>();

            //    List<Point> gs1 = FindRoute.findWay1(matrix, sp0, ep0);
            //    if (gs1 != null)
            //    {
            //        SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);

            //        route0.setPoints(gs1, mps);
            //        mainRoutes.Add(route0);
            //    }

            //    List<Point> gs2 = FindRoute.findWay2(matrix, sp0, ep0);
            //    if (gs2 != null)
            //    {
            //        SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);

            //        route0.setPoints(gs2, mps);
            //        mainRoutes.Add(route0);
            //    }
            //    if (mainRoutes.Count == 0)
            //    {
            //        SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);

            //        List<Point> pList = null;
            //        try
            //        {
            //            pList = disform.setInit(matrix, sp0, ep0);
            //        }
            //        catch (Exception ex)
            //        {
            //            break;
            //        }
            //        route0.setPoints(pList, mps);
            //        mainRoutes.Add(route0);
            //    }

            //    foreach (SubRoute mainRoute in mainRoutes)
            //    {
            //        Route route = new Route(null);
            //        route.id = ar;// permutation.ToString();
            //        routes.Add(route);
            //        route.subRoutes.Add(mainRoute);
            //        route.name = mainRoute.name;
            //        matrix = (int[,])matrix2.Clone(); //거리 매트릭스 초기화

            //        //갔던 길을 갈 수 있도록 거리값을 2로 줄임
            //        foreach (Point p in mainRoute.points)
            //        {
            //            matrix[p.X, p.Y] = 2;
            //        }

            //        for (int i = 1; i < instruments2.Count; i++)// instruments2.Count
            //        {
            //            Instrument ins = instruments2[i]; //3.4 7, 9 , 10* 11*
            //            int v1 = r.Next(ins.mps.Count - 1);
            //            MatrixPoint mp1 = ins.mps[v1];
            //            while (matrix[mp1.x, mp1.y] == int.MaxValue)
            //            {
            //                v1 = r.Next(ins.mps.Count - 1);
            //                mp1 = ins.mps[v1];
            //            }
            //            /////////////
            //            Point sp = new Point(mp1.x, mp1.y);
            //            SubRoute subRoute = new SubRoute(ins, destination, mp1);

            //            List<Point> ps = null;
            //            try
            //            {
            //                ps = disform.setInit(matrix, sp, ep0);
            //            }
            //            catch (Exception)
            //            {

            //                break;
            //            }
            //            ps.Reverse();

            //            List<Point> newPs = new List<Point>();
            //            foreach (Point p in ps)
            //            {
            //                if (matrix[p.X / 2, p.Y / 2] == 2)
            //                {
            //                    newPs.Add(p);
            //                    break;
            //                }
            //                else
            //                {
            //                    newPs.Add(p);
            //                    matrix[p.X / 2, p.Y / 2] = 2;
            //                }
            //            }
            //            subRoute.setPoints(newPs, mps);
            //            route.subRoutes.Add(subRoute);
            //        }



            //        AnaylisRoute.analysisRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);
            //        int cCount = 0;

            //        while (cCount != route.connectors.Count)
            //        {
            //            cCount = route.connectors.Count;
            //            AnaylisRoute.removeSubRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);

            //        }
            //        AnaylisRoute.joinLines(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);

            //    }
            //}

            //this.routes = this.routes.OrderBy(x => x.bend).ToList();
        
            //foreach (Route route in this.routes)
            //{
            //    object[] ob = { route, route.getLength(), route.getBendCount() - 1, route.id };
            //    this.dataGridView1.Rows.Add(ob);
            //}

            //this.vdFramedControl1.BaseControl.Redraw();
        }




        public static bool contains(Vertexes _pts, gPoint pt)
        {
            bool isIn = false;

            int NumberOfPoints = _pts.Count;
            if (true)
            {
                int i, j = 0;
                for (i = 0, j = NumberOfPoints - 1; i < NumberOfPoints; j = i++)
                {
                    if (
                    (
                    ((_pts[i].y <= pt.y) && (pt.y <= _pts[j].y)) || ((_pts[j].y <= pt.y) && (pt.y <= _pts[i].y))
                    ) &&
                    (pt.x <= (_pts[j].x - _pts[i].x) * (pt.y - _pts[i].y) / (_pts[j].y - _pts[i].y) + _pts[i].x)
                    )
                    {
                        isIn = !isIn;
                    }
                }
            }
            return isIn;
        }
        public static bool contains2(Vertexes _pts, gPoint pt)
        {
            bool isIn = false;

            int NumberOfPoints = _pts.Count;
            if (true)
            {
                int i, j = 0;
                for (i = 0, j = NumberOfPoints - 1; i < NumberOfPoints; j = i++)
                {
                    if (
                    (
                    ((_pts[i].y < pt.y) && (pt.y < _pts[j].y)) || ((_pts[j].y < pt.y) && (pt.y < _pts[i].y))
                    ) &&
                    (pt.x < (_pts[j].x - _pts[i].x) * (pt.y - _pts[i].y) / (_pts[j].y - _pts[i].y) + _pts[i].x)
                    )
                    {
                        isIn = !isIn;
                    }
                }
            }
            return isIn;
        }

        public static void setColorPath(vdPolyline polyline, Color color, VdConstLineWeight vdConstLineWeight)
        {
            polyline.PenColor = new VectorDraw.Professional.vdObjects.vdColor(color);
            polyline.LineWeight = vdConstLineWeight;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;


            Route r = (Route)this.dataGridView1[0, e.RowIndex].Value;


            this.selectedRoute = r;


            drawRoute();




            vdFramedControl1.BaseControl.ActiveDocument.Update();
            vdFramedControl1.BaseControl.Redraw();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //   t.Abort();

            isRun = false;

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            int[] aa = getArray(10);


            for (int i = 0; i < aa.Length; i++)
            {
                // Console.Write (aa[i]+",");

            }
            //   Console.WriteLine("--------");
        }

        public int[] getArray(int N)
        {
            Random random = new Random();

            bool[] selected = Enumerable.Repeat<bool>(false, N).ToArray<bool>();
            int selectedCnt = 0;
            int[] aa = new int[N];
            while (selectedCnt < N)
            {
                int a = random.Next(0, N); // 1에서 N-1까지
                if (selected[a] == true)
                {
                    continue;
                }

                // Console.WriteLine(a);
                aa[selectedCnt] = a;
                selected[a] = true;
                selectedCnt++;
            }
            return aa;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            vdSelection sel = this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0];
            foreach (vdFigure f in sel)
            {
                f.PenColor = new vdColor(Color.Red);
                foreach (Connector connector1 in this.selectedRoute.connectors)
                {
                    if (connector1.line == f)
                    {
                        f.PenColor = new vdColor(Color.Red);
                        f.Update();
                        if (!this.selectedRoute.disconnectedConnectors.Contains(connector1)) this.selectedRoute.disconnectedConnectors.Add(connector1);
                    }
                }
            }

            foreach (Connector connector1 in this.selectedRoute.disconnectedConnectors)
            {
                foreach (Instrument instrument in connector1.instruments)
                    if (!this.selectedRoute.disconnectedInstruments.Contains(instrument)) this.selectedRoute.disconnectedInstruments.Add(instrument);
            }

            this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            foreach (Connector connector1 in this.selectedRoute.disconnectedConnectors)
            { 
                connector1.line.Update();
            }
            this.selectedRoute.disconnectedConnectors.Clear();
            this.selectedRoute.disconnectedInstruments.Clear();
            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
            this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
        }

        private void reRunButton_Click(object sender, EventArgs e)
        {
            ////this.routes.Clear();
            ////this.dataGridView1.Rows.Clear();


            ////int[,] newMatrix = (int[,])this.mMatrix.Clone();
            ////AnaylisRoute.getNewMatrix(newMatrix, this.selectedRoute, this.mps);
             

            ////DisForm disform = new DisForm();


            ////Random r = new Random();
            ////Point ep0 = new Point(destination.mp.x, destination.mp.y);
            ////List<Instrument> instruments2 = new List<Instrument>();
            ////int[] array = getArray(this.selectedRoute.disconnectedInstruments.Count);
            ////for (int g = 0; g < array.Length; g++) instruments2.Add(this.selectedRoute.disconnectedInstruments[array[g]]);
            ////this.selectedRoute.subRoutes.Clear();
            ////int count = 0;
            ////while (count++ < 1000)
            ////{
            ////    int[,] matrix = (int[,])newMatrix.Clone();
            ////    Route route = new Route(null);
            ////    this.routes.Add(route);
            ////    foreach (Connector connector in selectedRoute.connectors)
            ////    {
            ////        if (selectedRoute.disconnectedConnectors.Contains(connector)) continue;
            ////        route.lines.Add((vdLine)connector.line.Clone(this.vdFramedControl1.BaseControl.ActiveDocument));
            ////    }

            ////    for (int i = 0; i < instruments2.Count; i++) //instruments2.Count
            ////    {
            ////        Instrument ins = instruments2[i];
            ////        int v1 = r.Next(ins.mps.Count - 1);
            ////        MatrixPoint mp1 = ins.mps[v1];
            ////        while (matrix[mp1.x, mp1.y] == int.MaxValue)
            ////        {
            ////            v1 = r.Next(ins.mps.Count - 1);
            ////            mp1 = ins.mps[v1];
            ////        }
            ////        /////////////
            ////        Point sp = new Point(mp1.x, mp1.y);
            ////        SubRoute subRoute = new SubRoute(ins, destination, mp1);
            ////        route.subRoutes.Add(subRoute);
            ////        List<Point> ps = null;
            ////        try
            ////        {
            ////            ps = disform.setInit(matrix, sp, ep0);
            ////        }
            ////        catch (Exception)
            ////        {
            ////            break;
            ////        }
            ////        ps.Reverse();
            ////        List<Point> newPs = new List<Point>();
            ////        foreach (Point p in ps)
            ////        {
            ////            if (matrix[p.X / 2, p.Y / 2] == 2)
            ////            {
            ////                newPs.Add(p);
            ////                break;
            ////            }
            ////            else
            ////            {
            ////                newPs.Add(p);
            ////                matrix[p.X / 2, p.Y / 2] = 2;
            ////            }
            ////        }
            ////        subRoute.setPoints(newPs, mps);
            ////        /*
            ////        vdPolyline poly = new vdPolyline(this.vdFramedControl1.BaseControl.ActiveDocument, subRoute.polyGps);
            ////        vdFramedControl1.BaseControl.ActiveDocument.Model.Entities.AddItem(poly);
            ////        poly.SetUnRegisterDocument(vdFramedControl1.BaseControl.ActiveDocument);
            ////        poly.setDocumentDefaults();
            ////        poly.PenColor = new vdColor(Color.Yellow);
            ////        route.subRoutes.Add(subRoute);
            ////        Console.WriteLine(subRoute.polyGps.Length() + " .. " + subRoute.gps.Length() + "  .. " + poly.Length());
            ////        */
            ////    }

            ////    AnaylisRoute.analysisRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);
            ////    ////
            ////    int cCount = 0;
            ////    while (cCount != route.connectors.Count)
            ////    {
            ////        cCount = route.connectors.Count;
            ////        AnaylisRoute.removeSubRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);

            ////    }
            ////    AnaylisRoute.joinLines(this.vdFramedControl1.BaseControl.ActiveDocument, route, this.instruments);


            ////}

            ////this.routes = this.routes.OrderBy(x => x.bend).ToList();
            ////int c = 0;

            ////this.routes.Insert(0, this.selectedRoute);


            ////foreach (Route route in this.routes)
            ////{
            ////    object[] ob = { route, route.getLength(), route.getBendCount() - 1, route.id };
            ////    this.dataGridView1.Rows.Add(ob);
            ////}

            ////this.vdFramedControl1.BaseControl.Redraw();

            ////vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false; 
            bool success = vdFramedControl1.BaseControl.ActiveDocument.Open(@"C:\Project\Docs\DAP_1_0531\DAP_1\DAP_1\test.dwg");
            if (!success) return;
            this.dataGridView1.Rows.Clear();
            instruments.Clear();

            foreach (vdFigure f in vdFramedControl1.BaseControl.ActiveDocument.Model.Entities)
            {

                if (f.Layer.Name == "Instrument")
                {
                    vdCircle circle = (vdCircle)f;
                    this.instrument.Add(circle);

                    Instrument instrument = new Instrument(circle, this.instruments.Count);
                    this.instruments.Add(instrument);
                }
                if (f.Layer.Name == "Obstacle")
                {
                    vdPolyline poly = (vdPolyline)f;
                    this.obstacles.Add(poly);

                }
                if (f.Layer.Name == "Boundary")
                {
                    vdPolyline poly = (vdPolyline)f;
                    this.boundary = poly;
                    vdCurves c = poly.getOffsetCurve(1000); ;
                    this.offsetBoundary = new vdPolyline(c.Document, c[0].GetGripPoints());
                }
                if (f.Layer.Name == "Destination")
                {

                    vdPolyline poly = (vdPolyline)f;
                    this.destination = new TBBOXDestination(poly);
                }
            }

            vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
