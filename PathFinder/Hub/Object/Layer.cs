using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdPrimaries;

namespace RouteOptimizer
{
    public class Layer
    {

        public int guid = 0;
        public string name = "";
        public List<Instrument> listInstrument = new List<Instrument>();
        public List<Obstacle> lstObstacle = new List<Obstacle>();
        public bool SelectedInInstrument = false;
        public bool SelectedInObstacle = false;
    
        public RouteOptimizer.Object.RouteInfo routeInfo = null;

        private vdLayer layer = new vdLayer();
        public Layer()
        {

        }
        private gPoint Centerpoint(vdScrollableControl.vdScrollableControl vdsc, Instrument vi)
        {
            gPoint reg = new gPoint();

            var e = vdsc.BaseControl.ActiveDocument.ActiveLayOut.Entities.GetNotDeletedItems();
            var c = vi.circle;
            try
            {
                foreach (vdFigure fg in e)
                {
                    if (fg is vdLeader ldr)
                    {
                        var gp = new gPoints();
                        var res = c.IntersectWith(ldr, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gp);
                        if (res)
                        {
                            if (ldr.PenColor.ColorIndex != 0) continue;
                            if (gp.Count > 1) continue;
                            if (gp.Count == 1)
                            {
                                if (ldr.VertexList[0].Distance2D(gp[0]) > ldr.VertexList[1].Distance2D(gp[0]))
                                {
                                    return ldr.VertexList[0];
                                }
                                else
                                {
                                    return ldr.VertexList[1];
                                }
                            }
                        }
                    }
                }
                foreach (vdFigure fg in e)
                {
                    if (fg is vdLine ldr)
                    {
                        var gp = new gPoints();
                        var res = c.IntersectWith(ldr, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, gp);
                        if (res)
                        {
                            if (ldr.PenColor.ColorIndex != 0) continue;
                            if (gp.Count > 1) continue;
                            if (gp.Count == 1)
                            {
                                if (ldr.StartPoint.Distance2D(gp[0]) > ldr.EndPoint.Distance2D(gp[0]))
                                {
                                    return ldr.StartPoint;
                                }
                                else
                                {
                                    return ldr.EndPoint;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return vi.centerPoint;
        }
        public Layer(VectorDraw.Professional.vdPrimaries.vdLayer vdlayer, vdScrollableControl.vdScrollableControl vdsc,  RouteOptimizer.Object.RouteInfo routeInfo=null) 
        {
            
            this.guid = vdlayer.Id;
            this.name = vdlayer.Name;
            this.layer = vdlayer;
            this.routeInfo = routeInfo;
            foreach (vdFigure f in vdsc.BaseControl.ActiveDocument.Model.Entities.GetNotDeletedItems())
            {
        
                var ent = f.Explode();
                vdCircle cir = new vdCircle();
                vdLeader lder = null;
                string t1 = "";
                string t2 = "";
                if (f.Layer.Name == vdlayer.Name)
                {
               
                    if (ent != null && (Check_1C_2A(ent) || Chek_1C_2A_1L(ent)) && f is vdInsert blk)
                    { 
                        foreach (var e in ent)
                        {
                            if (e is vdAttribDef va && va.TextString == "T1")
                                t1 = va.ValueString;
                            if (e is vdAttribDef vb && vb.TextString == "T2")
                                t2 = vb.ValueString;
                            if (e is vdCircle vc)
                                cir = vc;
                            if (e is vdLeader vl)
                                lder = vl;
                        }
                        if (lder != null)
                        {
                            var vt = lder.VertexList.Count;
                            gPoint gpoint = lder.VertexList[0];
                            cir = new vdCircle(cir.Document, gpoint, cir.Radius);
                        }

                        var ps = f.BoundingBox.GetPoints();
                        bool IsOutSide = false;
                        if (routeInfo != null)
                        {
                            if (routeInfo.MainBoundary != null)
                            {
                                var b = routeInfo.MainBoundary;
                                foreach (gPoint p in ps)
                                {
                                    if (!b.BoundingBox.PointInBox(p))
                                    {
                                        IsOutSide = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (IsOutSide)
                            continue;

                        if (routeInfo != null)
                        {
                            var res = routeInfo.AllInstrument_Leader.Where(c => c.Item1 == blk);
                            if (res.Any())
                            {
                                if (res.FirstOrDefault().Item2 != null)
                                {
                                    var p1 = res.FirstOrDefault().Item2.VertexList[0];
                                    var p2 = res.FirstOrDefault().Item2.VertexList[1];
                                    if (cir.Center.Distance2D(p1) < cir.Center.Distance2D(p2))
                                        cir = new vdCircle(cir.Document, p2, cir.Radius);
                                    else
                                        cir = new vdCircle(cir.Document, p1, cir.Radius);
                                }
                            }
                            else
                            {
                                var tpl = Tuple.Create<vdInsert, vdLeader>(blk, null);
                                routeInfo.AllInstrument_Leader.Add(tpl);
                            }
                        }
                        var addedInsrument = new Instrument(cir, t1, t2, false, blk);
                        addedInsrument.centerPoint = Centerpoint(vdsc, addedInsrument);
                        this.listInstrument.Add(addedInsrument);
                    }
                    if (routeInfo != null)
                    {
                        var lob = routeInfo.LstObstacles.Where(o => o.vdObstacle == f);
                        if (lob.Any())
                        {
                            lstObstacle.Add(lob.FirstOrDefault());
                        }
                    }
                }
           
            }
        }
        private bool Check_1C_2A(vdEntities ve)
        {
            vdAttribDef atb1 = null;
            vdAttribDef atb2 = null;
            //vdLeader lder = null;
            vdCircle cir = null;
            foreach (var lines in ve)
            {
                if (!(lines is vdCircle) && !(lines is vdAttribDef))
                    return false;
                if (lines is vdAttribDef l)
                {
                    if (atb1 == null)
                        atb1 = l;
                    else if (atb2 == null)
                        atb2 = l;
                    else
                        return false;
                }
                if (lines is vdCircle c)
                {
                    if (cir == null)
                        cir = c;
                    else
                        return false;
                }
            }
            if (atb1 != null && atb2 != null && cir != null)
            {
                return true;
            }
            return false;
        }
        private bool Chek_1C_2A_1L(vdEntities ve)
        {
            vdAttribDef atb1 = null;
            vdAttribDef atb2 = null;
            vdLeader lder = null;
            vdCircle cir = null;
            foreach (var lines in ve)
            {
                if (!(lines is vdCircle) && !(lines is vdAttribDef) && !(lines is vdLeader))
                    return false;
                if (lines is vdAttribDef l)
                {
                    if (atb1 == null)
                        atb1 = l;
                    else if (atb2 == null)
                        atb2 = l;
                    else
                        return false;

                }
                if (lines is vdLeader e)
                {
                    if (lder == null)
                        lder = e;
                    else
                        return false;
                }
                if (lines is vdCircle c)
                {
                    if (cir == null)
                        cir = c;
                    else
                        return false;
                }


            }
            if (atb1 != null && atb2 != null && cir != null && lder != null)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return this.name;
        }
    }

}