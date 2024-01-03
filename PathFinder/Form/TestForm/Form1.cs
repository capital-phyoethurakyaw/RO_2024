using RouteOptimizer.Analysis1; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    public partial class Form1 : System.Windows.Forms.Form
    {
        Color[] colors = new Color[] { Color.Magenta, Color.Yellow, Color.AliceBlue, Color.Bisque };
        
        List<Instrument> instruments  = new List<Instrument>();
        List<vdPolyline> obstacles = new List<vdPolyline>();
        List<vdCircle> instrument = new List<vdCircle>();


        Route selectedRoute = new Route(null);
        Routes selectedRoutes = new Routes();


        Thread t;
        gPoints gridPoints = new gPoints();
        vdPolyline boundary;
        vdPolyline offsetBoundary;
      //  Destination destination;

        List<TBBOXDestination> destinations = new List<TBBOXDestination>();


        double sx = 0;
        double sy = 0;

        List<Routes> caseRoutes = new List<Routes>();
        List<vdLine> routeLines = new List<vdLine>();
        bool isRun = true;

        int[,] mMatrix = null;
        MatrixPoint[,] mps = null;
        
        public List<Connector> disconnectedConnectors = new List<Connector>();


        public Form1()
        {
            InitializeComponent();
           // this.vdFramedControl1.BaseControl.DrawAfter += BaseControl_DrawAfter;
        }

        private void BaseControl_DrawAfter(object sender, VectorDraw.Render.vdRender render)
        {
            Graphics gr = this.vdFramedControl1.BaseControl.ActiveDocument.GlobalRenderProperties.GraphicsContext.MemoryGraphics;
            
            
            
            //  Rectangle rc = new Rectangle(new Point(0, 0), this.vdFramedControl1.BaseControl.ActiveDocument.GlobalRenderProperties.GraphicsContext.MemoryBitmap.Size);
            //  rc.Inflate(1, 1);
            // Font font = new Font("Verdana", 20);
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



            /*
           foreach(Route r in this.selectedRoutes.routes) { 
              
                foreach (Connector connector in r.connectors)
                {
                    connector.line.PenColor = new vdColor(r.destination.color);
                    this.vdFramedControl1.BaseControl.ActiveDocument.Model.Entities.AddItem(connector.line);
                    this.routeLines.Add(connector.line);
                    connector.line.Layer = layer;
                 }
                    this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
            }
          */
            foreach (Connector connector in selectedRoutes.connectors)
            {

                this.vdFramedControl1.BaseControl.ActiveDocument.Model.Entities.AddItem(connector.line);
                this.routeLines.Add(connector.line);
                connector.line.Layer = layer;
            }
            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);



            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
            /*
            if (info.route != null) info.route.Draw(render);
            List<vdPolyline> polylines = writeTriangle(info.route);
            foreach (vdPolyline polyline in polylines) polyline.Draw(render);


            vdCircle circle11 = new vdCircle(this.vectorDrawBaseControl1.ActiveDocument);
            circle11.Center = first.getStartPoint();
            circle11.Radius = 800;
            circle11.HatchProperties = new VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid);
            circle11.PenColor.Red = 0; circle11.PenColor.Green = 0; circle11.PenColor.Blue = 255;
            circle11.Draw(render);
            */
        }


        public void drawRoute(VectorDraw.Render.vdRender render)
        {
            int c = 0;
            foreach (Connector connector1 in this.selectedRoutes.connectors) {
                foreach (Instrument ins in connector1.instruments)
                {
                   gPoint gp = connector1.line.getClosestPointTo(ins.centerPoint);
                    render.DrawLine(null, ins.centerPoint, gp);
                }
           }

            if (mMatrix != null)
            {
                for (int i = 0; i < mMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < mMatrix.GetLength(1); j++)
                    {
                        if (mMatrix[i, j] < 1000)
                        {
                            /*
                            vdCircle c1 = new vdCircle(this.vdFramedControl1.BaseControl.ActiveDocument, mps[i, j].gp, 200);
                            vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(c1);
                            c1.SetUnRegisterDocument(vdFramedControl1.BaseControl.ActiveDocument);
                            c1.setDocumentDefaults();
                            c1.PenColor = new vdColor(Color.Yellow);
                            */
                            render.EdgeColor = Color.Yellow;
                            render.Update();
                         //   render.DrawArc(null, mps[i, j].gp, 0, Math.PI * 2, 200);
                        }
                        else
                        {
                            /*
                            vdCircle c1 = new vdCircle(this.vdFramedControl1.BaseControl.ActiveDocument, mps[i, j].gp, 200);
                            vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities.AddItem(c1);
                            c1.SetUnRegisterDocument(vdFramedControl1.BaseControl.ActiveDocument);
                            c1.setDocumentDefaults();
                            c1.PenColor = new vdColor(Color.Blue);
                            */
                            render.PenStyle.color = Color.Blue;
                            render.EdgeColor = Color.Blue;
                          //  render.DrawArc(null, mps[i, j].gp, 0, Math.PI * 2, 100);
                        }
                    }
                }
            }


        }

         private void toolStripButton1_Click(object sender, EventArgs e)
        {
            int[,] matrix = (int[,])this.mMatrix.Clone();
            t = new Thread(()=> makeGrid(matrix,null));
            t.Start();
        }

        private void reRunButton_Click(object sender, EventArgs e)
        {
            this.caseRoutes.Clear();
            this.dataGridView1.Rows.Clear();
            int[,] newMatrix = (int[,])this.mMatrix.Clone();
            List<vdLine> lines = AnaylisRoute.getSelectedLine(this.selectedRoutes);
            AnaylisRoute.getNewMatrix(newMatrix, lines, this.mps);
            t = new Thread(() => makeGrid(newMatrix, lines));
            t.Start();
        }

        public void setMatrix() {
            Box box = this.offsetBoundary.BoundingBox;

            sx = box.Left;
            sy = box.Bottom;

            int wGap = 3000;// 1414;  //1.41421   1414//3000
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

        public void makeGrid(int[,] matrix, List<vdLine> selectedLines)
        { 
            int[,] matrix2 = (int[,])matrix.Clone();


            DisForm disform = new DisForm();

 
            double cou = -1;

            isRun = true;
            Random r = new Random();
            while (cou <100 && isRun == true)//100000
            {
                cou++;
                Routes routes = new Routes(); 
              

                bool isSucceed = false;
                foreach (TBBOXDestination destination in this.destinations)
                {

                    //Random으로 순서를 변경
                    List<Instrument> instruments2 = new List<Instrument>();
                    int[] array = getArray(destination.lstInstrument.Count);
                    string ar = "";

                
                    for (int g = 0; g < array.Length; g++)
                    {
                        instruments2.Add(destination.lstInstrument[array[g]]);
                        ar += array[g] + ",";
                    }

                    Instrument ins0 = instruments2[0]; //3.4 7, 9 , 10* 11*
                    int v = r.Next(ins0.mps.Count - 1);
                    MatrixPoint mp = ins0.mps[v];
                    while (matrix[mp.x, mp.y] == int.MaxValue)
                    {
                        v = r.Next(ins0.mps.Count - 1);
                        mp = ins0.mps[v];
                    }
 
                
                    Point sp0 = new Point(mp.x, mp.y);

                    ins0.mp = mp;
                    Point ep0 = new Point(destination.mp.x, destination.mp.y);
                    //Point sp0 = new Point(ins0.mp.x, ins0.mp.y);

                    List<SubRoute> mainRoutes = new List<SubRoute>();
                 
                    int type =  r.Next(2);
                    /////////////////////////////////
                    // ㄱ shape route 
                    if (type == 0)
                    {
                        List<Point> gs1 = FindRoute.findWay1(matrix, sp0, ep0);
                        if (gs1 != null)
                        {
                            SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                            route0.setPoints(gs1, mps);
                            mainRoutes.Add(route0);
                        }
                    }
                    else
                    {
                        // L shape route 
                        List<Point> gs2 = FindRoute.findWay2(matrix, sp0, ep0);
                        if (gs2 != null)
                        {
                            SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                            route0.setPoints(gs2, mps);
                            mainRoutes.Add(route0);
                        }
                    }
                    ////////////////////////////////////////////
               
                    if (mainRoutes.Count == 0)
                    {
                        SubRoute route0 = new SubRoute(ins0, destination, ins0.mp);
                        List<Point> pList = null;
                        try
                        {
                            pList = disform.setInit(matrix, sp0, ep0,0);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                        route0.setPoints(pList, mps);
                        mainRoutes.Add(route0);
                        /*
                        // 꺽임 최소화
                          SubRoute route1 = FindRoute.findRoute(matrix, route0);
                          if (route1 != null)
                          {
                              route1.name = "route1";
                              route1.setPoints2(    mps);

                              mainRoutes.Add(route1);
                          }
                          else {
                              MessageBox.Show("null");
                              Console.WriteLine("null");
                          }
                         route0.gps.Reverse();
                         */
                    }

                    foreach (SubRoute mainRoute in mainRoutes)
                    {
                        Route route = new Route(destination);
                        route.id = ar;// permutation.ToString();
                        routes.routes.Add(route);
                        route.subRoutes.Add(mainRoute);
                        route.name = mainRoute.name;
                        matrix = (int[,])matrix2.Clone(); //거리 매트릭스 초기화

                        //갔던 길을 갈 수 있도록 거리값을 2로 줄임
                        foreach (Point p in mainRoute.points)
                        {
                            matrix[p.X, p.Y] = 2;
                        }

                        for (int i = 1; i < instruments2.Count; i++)// instruments2.Count
                        {
                            Instrument ins = instruments2[i]; //3.4 7, 9 , 10* 11*
                            int v1 = r.Next(ins.mps.Count - 1);
                            MatrixPoint mp1 = ins.mps[v1];
                            while (matrix[mp1.x, mp1.y] == int.MaxValue)
                            {
                                v1 = r.Next(ins.mps.Count - 1);
                                mp1 = ins.mps[v1];
                            }
                            /////////////
                            Point sp = new Point(mp1.x, mp1.y);
                            SubRoute subRoute = new SubRoute(ins, destination, mp1);

                            List<Point> ps = null;
                            try
                            {
                                ps = disform.setInit(matrix, sp, ep0, 0);
                            }
                            catch (Exception)
                            {

                                break;
                            }
                            ps.Reverse();

                            List<Point> newPs = new List<Point>();
                            foreach (Point p in ps)
                            {
                                if (matrix[p.X / 2, p.Y / 2] == 2)
                                {
                                    newPs.Add(p);
                                    break;
                                }
                                else
                                {
                                    newPs.Add(p);
                                    matrix[p.X / 2, p.Y / 2] = 2;
                                }
                            }
                            subRoute.setPoints(newPs, mps);
                            route.subRoutes.Add(subRoute);
                        }
                        AnaylisRoute.analysisRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, destination.lstInstrument);
                        int cCount = 0;

                        while (cCount != route.connectors.Count)
                        {
                            cCount = route.connectors.Count;
                            //0711 modified
                            AnaylisRoute.removeSubRoute(this.vdFramedControl1.BaseControl.ActiveDocument, route, destination.lstInstrument, destination,  3000);
                        }
                        AnaylisRoute.joinLines(this.vdFramedControl1.BaseControl.ActiveDocument, route, destination.lstInstrument);
                    }
                }


                /// join routes 
                List<vdLine> lines = new List<vdLine>();
                List<Instrument> instruments = new List<Instrument>();
                foreach (Route route in routes.routes)
                {
                    instruments.AddRange(route.destination.lstInstrument);
                    routes.connectors.AddRange(route.connectors);
                    routes.nodes.AddRange(route.nodes);
                }
                for (int i = 0; i < routes.connectors.Count; i++)
                {
                    Connector connector = (Connector)routes.connectors[i];
                    lines.Add(connector.line);
                }

                AnaylisRoute.removeLines(lines);
                int linesCount = 0;

                while (linesCount != lines.Count)
                {
                    AnaylisRoute.breakLine(lines);
                    linesCount = lines.Count;
                }
                AnaylisRoute.removeLines(lines);
                AnaylisRoute.joinSameParallelLine(lines);
                routes.lines = lines;
                isSucceed = AnaylisRoute.joinLines2(this.vdFramedControl1.BaseControl.ActiveDocument, routes, instruments,3000);
                ///////////////////////
               
                //restart(pin)
                bool isInAll = true;
                if (selectedLines != null) {
                    foreach (vdLine selectedLine in selectedLines) {
                        bool isIn = false;
                        foreach (vdLine line in routes.lines) {
                            if ((selectedLine.StartPoint == line.StartPoint && selectedLine.EndPoint == line.EndPoint) ||
                                    (selectedLine.EndPoint == line.StartPoint && selectedLine.StartPoint == line.EndPoint)) {
                                isIn = true;
                                break;   
                            }                       
                        }
                        if (!isIn)
                        {
                            isInAll = false;
                            break;
                        }
                    }
                }
                if (isSucceed && isInAll) 
                    this.caseRoutes.Add(routes);
            }
             this.caseRoutes = this.caseRoutes.OrderBy(x => x.getBendCount()).ToList();
            //remove error
            for (int i = 0; i < this.caseRoutes.Count; i++) { 
            Routes routes = this.caseRoutes[i];
                if (routes.getLength() % 10 != 0) {
                    this.caseRoutes.Remove(routes);
                    i--;
                }
            }
            foreach (Routes routes in this.caseRoutes)
            {
                object[] ob = { routes, routes.getJoin(), routes.getLength(), routes.getBendCount() - 1, routes.getLength() };
                this.dataGridView1.Rows.Add(ob);
            }
            this.vdFramedControl1.BaseControl.Redraw();
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


            Routes r = (Routes) this.dataGridView1[0, e.RowIndex].Value;
           

            this.selectedRoutes = r;


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


            for (int i = 0; i < aa.Length; i++) {
               // Console.Write (aa[i]+",");

            }
         //   Console.WriteLine("--------");
        }

        public int[] getArray(int N) {
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

     

        private void Reset_Click(object sender, EventArgs e)
        {
            foreach (Connector connector1 in this.selectedRoute.disconnectedConnectors)
            {
                connector1.line.PenColor = new vdColor(Color.White);
                connector1.line.Update();            
            }
        
            this.selectedRoute.disconnectedConnectors.Clear();
            this.selectedRoute.disconnectedInstruments.Clear();
            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
            this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
        }




    

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            string fname;
            /*
            string DocPath;
            
                object ret = vdFramedControl1.BaseControl.ActiveDocument.GetOpenFileNameDlg(0, "", 0);
                if (ret == null) return;

                DocPath = ret as string;
                fname = (string)ret;
            */

            bool success = vdFramedControl1.BaseControl.ActiveDocument.Open(@"C:\Project\Docs\May\DAP_1_0707\DAP_1_0711\DAP_1_0711\test_d2.dwg");//Prepare_OutofRange  test_d2
            if (!success) return;
            this.dataGridView1.Rows.Clear();
            this.instruments.Clear();

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
                    TBBOXDestination destination = new TBBOXDestination(poly);
                  //  destination.id = destinations.Count;
                    destination.color = this.colors[destinations.Count];
                    destinations.Add(destination);
                }
            }

            setMatrix();

            //int[,] matrix = (int[,])this.mMatrix.Clone();
            int minDistance = int.MaxValue;


            foreach (TBBOXDestination destination in this.destinations)
            {
                int count = 0;
                foreach (MatrixPoint mp in mps)
                {

                    int dis =  (int)destination.centerPoint.Distance2D(mp.gp);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        minDistance = dis;
                        destination.mp = mp;
                        destination.gridPoint = mp.gp;
                        destination.gridIndex = count;
                    }
                    count++;
                }
            } 
            foreach (Instrument ins in this.instruments)
            {
                minDistance = int.MaxValue;
                int count = 0;
                foreach (MatrixPoint mp in mps)
                {
                    int dis = (int)ins.centerPoint.Distance2D(mp.gp);
                    if (minDistance > dis)
                    {
                        minDistance = dis;
                        ins.distance = dis;
                        ins.gridIndex = count;
                        ins.mp = mp;
                    }
                    if (dis < 3000)
                    {
                        ins.mps.Add(mp);
                    }
                    count++;
                }
            } 
            foreach (Instrument ins in this.instruments)
            {
                double minDis = double.MaxValue;
                TBBOXDestination minDestination = null;
                foreach (TBBOXDestination destination in this.destinations)
                {
                    double dis = ins.mp.gp.Distance2D(destination.mp.gp);
                    if (dis < minDis) {
                        minDis = dis;
                        minDestination = destination;
                    }
                }
                ins.destination = minDestination;
            }
            foreach (TBBOXDestination destination in this.destinations)
            {
                foreach (Instrument ins in this.instruments)
                {
                    if (destination == ins.destination)
                    {
                        destination.lstInstrument.Add(ins);
                    }
                }

            }

        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            vdLine l1 = new vdLine(new gPoint(0, 0), new gPoint(150, 0));
            vdLine l2 = new vdLine(new gPoint(150, 0), new gPoint(200, 0));

            List<vdLine> lines = new List<vdLine>();
            lines.Add(l1);
            lines.Add(l2);
                vdFigure f = new vdLine();
            l1.Break(new gPoint(100, 0), new gPoint(100, 0), out f);
            vdLine l3 = (vdLine)f;

           Console.WriteLine(l1.Length()+"  "+l3.Length());  
        }

        private void select_Click(object sender, EventArgs e)
        {
            vdSelection sel = this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0];
            foreach (vdFigure f in sel)
            {
                f.PenColor = new vdColor(Color.Red);
                foreach (Connector connector1 in this.selectedRoutes.connectors)
                {
                    if (connector1.line == f)
                    {
                        f.PenColor = new vdColor(Color.Red);
                        f.Update();
                        if (!this.selectedRoutes.selectedConnectors.Contains(connector1)) this.selectedRoutes.selectedConnectors.Add(connector1);
                    }
                }
            }

            foreach (Connector connector1 in this.selectedRoutes.selectedConnectors)
            {
                foreach (Instrument instrument in connector1.instruments)
                    if (!this.selectedRoutes.selectedInstruments.Contains(instrument)) this.selectedRoutes.selectedInstruments.Add(instrument);
            }

            this.vdFramedControl1.BaseControl.ActiveDocument.Selections[0].RemoveAll();
            this.vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          //  vdFramedControl1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, true);
        }
    }
}
