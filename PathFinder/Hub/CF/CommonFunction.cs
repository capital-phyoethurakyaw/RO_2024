using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vdControls;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Actions;
using RouteOptimizer;
using CsvHelper;
using RouteOptimizer.Entity;
using RouteOptimizer.PInfoForms;
using static RouteOptimizer.Entity.CableScheduleEntity;
using RouteOptimizer.Object;
using System.Data;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace RouteOptimizer.CF
{
    public class CommonFunction
    {
        MainP1 MainP1;
        public CommonFunction()
        {

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
        public DataTable ListToDataTable<T>(List<T> items, string GroupColumnNames = null)
        {
            if (items.Count == 0)
                return new DataTable();
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) dataTable.Columns.Add(prop.Name);
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            if (!string.IsNullOrEmpty(GroupColumnNames))  // later, proceed to be contained for split column name
            {
                var dt = dataTable.AsEnumerable()
             .GroupBy(r => new { Col1 = r[GroupColumnNames], Col2 = r[GroupColumnNames] })
             .Select(g => g.OrderBy(r => r["GUID"]).First())
             .CopyToDataTable();
                return dt;
            }
            return dataTable;
        }

        public void DistanceCalculation()
        {

        }
        public bool Chek_1C_2A_1L(vdEntities ve)
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
        // public List<int> 
        public vdCircle GetCenterInstrument(vdInsert blk, RouteInfo routeInfo)
        {
            var ent = blk.Explode();
            vdCircle cir = new vdCircle();
            vdLeader lder = null;
            string t1 = "";
            string t2 = "";

            if (ent != null && (Check_1C_2A(ent) || Chek_1C_2A_1L(ent)))
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
                    gPoint gpoint = lder.VertexList[0];
                    cir = new vdCircle(cir.Document, gpoint, cir.Radius);
                }

                var res = routeInfo.AllInstrument_Leader.Where(c => c.Item1.Id == blk.Id);
                if (res.Any())
                {

                    var p1 = res.FirstOrDefault().Item2.VertexList[0];
                    var p2 = res.FirstOrDefault().Item2.VertexList[1];
                    if (cir.Center.Distance2D(p1) < cir.Center.Distance2D(p2))
                        cir = new vdCircle(cir.Document, p2, cir.Radius);
                    else
                        cir = new vdCircle(cir.Document, p1, cir.Radius);
                }

                return cir;
            }

            return null;
        }
        public bool Check_1C_2A(vdEntities ve)
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
        RouteInfo ri = null;
        public List<Instrument> GetAllInstrument(vdControls.vdFramedControl vdFramedControl, RouteInfo routeInfo, DataGridView instListbox = null, ListBox ObstListbox = null)
        {
            ri = routeInfo;
            List<Instrument> allins = new List<Instrument>();

            var lstlyr = GetAllLayer(vdFramedControl, routeInfo, instListbox, ObstListbox);

            foreach (var layer in lstlyr)
            {
                foreach (var ins in layer.listInstrument)
                    allins.Add(ins);
            }
            return allins;
        }
        public List<Layer> GetAllLayer(vdControls.vdFramedControl vdFramedControl, RouteInfo routeInfo, DataGridView instListbox = null, ListBox ObstListbox = null)
        {
            List<Layer> layer = new List<Layer>();
            foreach (vdLayer l in vdFramedControl.BaseControl.ActiveDocument.Layers.GetNotDeletedItems())
            { 
                layer.Add(new Layer(l, vdFramedControl.ScrollableControl, routeInfo));
            }
            routeInfo.LstLayer = layer;
            if (instListbox != null)
            {
                foreach (var l in routeInfo.LstLayer)
                {
                    foreach (DataGridViewRow r in instListbox.Rows)
                    {
                        Layer c = r.Cells["dgv_Layer"].Value as Layer;
                        if (c.name == l.name)
                            routeInfo.LstLayer.Where(v => v.name == l.name).FirstOrDefault().SelectedInInstrument = true;
                    }
                    foreach (Layer c in ObstListbox.Items)
                    {
                        if (c.name == l.name)
                            routeInfo.LstLayer.Where(v => v.name == l.name).FirstOrDefault().SelectedInObstacle = true;
                    }
                }
            }
         
            return routeInfo.LstLayer;
        }
        static List<string> SearchList(List<string> inputList, string searchTerm)
        {
            return inputList.Where(item => item.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        }
    }
}