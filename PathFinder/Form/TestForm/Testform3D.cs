using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Render;

namespace RouteOptimizer.Form.TestForm
{
    public partial class Testform3D : System.Windows.Forms.Form
    {
        public Testform3D()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities.Add();
          //  vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("VISW");
        }

        private void button2_Click(object sender, EventArgs e)
        {
          var r=  vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities;
            vdPolyline p = new vdPolyline(vdFramedControl1.BaseControl.ActiveDocument);
            var p0 = new gPoint(10000, 10000, 10000); 
            var p1 = new gPoint(10000, 10000, 20000);
            var p2 = new gPoint(10000, 0, 20000);
            var p3 = new gPoint(10000, 0, 40000);
            var p4 = new gPoint(0, 40000, 0);
            p.VertexList.Add(p0);
            p.VertexList.Add(p1);
            p.VertexList.Add(p2);
            p.VertexList.Add(p3);
            p.VertexList.Add(p4);
            r.Add(p);
            r.SetUnRegisterDocument(vdFramedControl1.BaseControl.ActiveDocument);
            p.Update();
            p.visibility = VectorDraw.Professional.vdPrimaries.vdFigure.VisibilityEnum.Visible; 
           // vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(null, null, null, null);
            RefreshUpdate();
        }
        private void RefreshUpdate()
        { 
            vdFramedControl1.BaseControl.ActiveDocument.Update();
            vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
            //vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomExtents();
            //vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("VISW");
            vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("SHADEON");
            RefreshUpdate();
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(10000, 10000, 20000), 100.0, 10, 10);
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(10000, 10000, 10000), 100.0, 10, 10);
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(10000, 0, 20000), 100.0, 10, 10);
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(10000, 0, 40000), 100.0, 10, 10);
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(0, 40000, 0), 100.0, 10, 10);

            //var figLast = vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities;

            //foreach (vdFigure f in figLast)
            //{
            //    if (!(f is vdPolyface)) continue;
            //    f.PenColor = new vdColor(Color.Red);
            //    f.Update();
            //}
            //vdSphere  
            // vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("VISE");  
            // vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("SHADEON"); 
            //  RefreshUpdate();
        }

        private void View3D(string port)
        {
            vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D(port);
        }
        //private void View3D()
        //{
        //    vdFramedControl1.BaseControl.ActiveDocument.CommandAction.View3D("VISE");
        //}
        private void GenerateGrid(double width, double wGap, double breadth, double bGap, double height, double hGap)  //x-y-z
        {
            var doc = vdFramedControl1.BaseControl.ActiveDocument;
            var cmd = vdFramedControl1.BaseControl.ActiveDocument.CommandAction;
            //vdFramedControl1.BaseControl.ActiveDocument.CommandAction.CmdSphere(new gPoint(10000, 10000, 20000), 100.0, 10, 10);
            for (int z = 0; height > hGap * z; z++)
            {
               // if (z == 0) continue;
                double yg = 0;
                for (int y = 0; breadth > bGap * y; y++)
                {
                 //   if (y == 0) continue;
                    double xg = 0;
                    for (int x = 0; width > wGap * x; x++)
                    { 
                        cmd.CmdSphere(new gPoint(wGap*x,bGap*y,hGap*z), 100.00, 10, 10);
                    } 
                }
            }
            RefreshUpdate();
            vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomExtents();
            vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomAll();
        }
        private void Gnerate3DGridPoint()
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
            RefreshUpdate();
            vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomExtents();
            vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomAll();
        }
        private void button5_Click(object sender, EventArgs e)
        {
           vdFramedControl1.BaseControl.ActiveDocument.New();
            var doc = vdFramedControl1.BaseControl.ActiveDocument;
            vdPolyface pf = new vdPolyface(doc);
            pf.CreateBox(new gPoint(0, 0, 0), 5, 5, 5, 0);//create a box (5x5x5) size
            doc.Model.Entities.AddItem(pf);
            createAlignToViewEntities(); //fill the collections with the alignet to view texts
            doc.RenderMode = vdRender.Mode.Shade;
            doc.ZoomExtents();
            doc.Redraw(false);
        }
        private void Doc_OnDrawAfter(object sender, vdRender render)
        {
            IvdOpenGLRender glrender = render.OpenGLRender;
            if (glrender != null) glrender.Flush(-1);//finishing draw all existing primitives in the buffer in case of 3d rendering views
            bool oldenabledepth = render.EnableDepthBuffer(false);//disable depth buffer comparison
            bool oldenabledepthwrite = render.EnableDepthBufferWrite(false);//disable write to depth buffer
            if (drawafterentities != null)
            foreach (vdFigure item in drawafterentities)
            {
                item.Draw(render);
            }
            //restore depth buffer values
            render.EnableDepthBuffer(oldenabledepth);
            render.EnableDepthBufferWrite(oldenabledepthwrite);
        }
        void createAlignToViewEntities()
        {
            var doc = vdFramedControl1.BaseControl.ActiveDocument;
            if (drawafterentities.Count > 0) return;
            drawafterentities.SetUnRegisterDocument(doc);
            //create a vdText foreach vertex of a box (5x5x5)
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(0, 0, 0), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(5, 0, 0), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(5, 5, 0), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(0, 5, 0), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });

            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(0, 0, 5), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(5, 0, 5), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(5, 5, 5), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
            drawafterentities.AddItem(new vdText(doc, "VectorDraw", new gPoint(0, 5, 5), 1.0, VdConstHorJust.VdTextHorCenter, VdConstVerJust.VdTextVerCen, doc.TextStyles.Standard) { AlignToView = true, AlignToViewSize = 10, PenColor = new vdColor(Color.Red) });
        }
        public vdEntities drawafterentities;
        private void vdFramedControl1_Load(object sender, EventArgs e)
        {
            vdFramedControl1.BaseControl.ActiveDocument.OnDrawAfter += Doc_OnDrawAfter;
              drawafterentities = new vdEntities(); // this the the collection of texts that will be rendered "on - top" of the model entities
        }

        private void button6_Click(object sender, EventArgs e)
        {
            GenerateGrid(100000,10000,100000,10000,100000,10000);

        }
    }
}
