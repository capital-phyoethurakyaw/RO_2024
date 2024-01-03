using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using static System.Windows.Forms.LinkLabel;
using RouteOptimizer.Object;
using RouteOptimizer.Entity;

namespace RouteOptimizer.Analysis
{
    public class AnaylisRoute
    {
       // public static int Unit = 3000;
        public static int UnitDestinationNearest = 1000;
        public static void removeLines(List<vdLine> lines)
        {

            for (int i = 0; i < lines.Count - 1; i++)
            {
                vdLine l1 = lines[i];
                for (int j = i + 1; j < lines.Count; j++)
                {
                    vdLine l2 = lines[j];

                    if ((l1.StartPoint.x == l2.StartPoint.x && l1.StartPoint.y == l2.StartPoint.y && l1.EndPoint.x == l2.EndPoint.x && l1.EndPoint.y == l2.EndPoint.y) ||
                        (l1.StartPoint.x == l2.EndPoint.x && l1.StartPoint.y == l2.EndPoint.y && l1.EndPoint.x == l2.StartPoint.x && l1.EndPoint.y == l2.StartPoint.y))
                    {
                        lines.Remove(l2);




                        j--;
                    }
                }
            }


            for (int i = 0; i < lines.Count - 1; i++)
            {
                vdLine l1 = lines[i];
                for (int j = i + 1; j < lines.Count; j++)
                {
                    vdLine l2 = lines[j];
                    gPoints intersectedP = new gPoints();
                    l1.IntersectWith(l2, VdConstInters.VdIntOnBothOperands, intersectedP);
                    if (intersectedP.Count > 1)
                    {
                        if (l1.Length() > l2.Length()) lines.Remove(l2);
                        else lines.Remove(l1);
                        i = 0;
                        break;
                    }
                    /*
                    intersectedP = new gPoints();
                    l1.IntersectWith(l2, VdConstInters.VdIntOnBothOperands, intersectedP);

                    if (intersectedP.Count > 1)
                    {
                        if (l1.Length() > l2.Length()) lines.Remove(l2);
                        else lines.Remove(l2);
             
                        i = 0;
                        break;
                    }*/
                }
            }
        }

        public static void joinSameParallelLine(List<vdLine> lines)
        {
            List<vdLine> removeLines = new List<vdLine>();
            List<vdLine> addLines = new List<vdLine>();

            gPoints gps = new gPoints();
            foreach (vdLine line in lines)
            {

                gps.Add(line.StartPoint);
                gps.Add(line.EndPoint);
            }
            gps.RemoveEqualPoints(0);

            foreach (gPoint p in gps)
            {
                List<vdLine> ls = new List<vdLine>();
                foreach (vdLine line in lines)
                {
                    if (p == line.StartPoint || p == line.EndPoint)
                    {
                        ls.Add(line);
                    }
                }
                if (ls.Count == 2)
                {
                    vdLine l1 = ls[0];
                    vdLine l2 = ls[1];
                    if ((l1.StartPoint.x == l2.StartPoint.x && l1.EndPoint.x == l2.EndPoint.x && l1.StartPoint.x == l1.EndPoint.x)
                        || (l1.StartPoint.y == l2.StartPoint.y && l1.EndPoint.y == l2.EndPoint.y && l1.StartPoint.y == l1.EndPoint.y))
                    {
                        List<TBBOXDestination> destinations1 = (List<TBBOXDestination>)l1.XProperties["Destinations"];
                        List<TBBOXDestination> destinations2 = (List<TBBOXDestination>)l2.XProperties["Destinations"];

                        List<TBBOXDestination> newDestinations = new List<TBBOXDestination>();

                        removeLines.Add(l1);
                        removeLines.Add(l2);

                        if (l1.StartPoint == l2.StartPoint && l1.EndPoint != l2.EndPoint)
                        {
                            vdLine line = new vdLine(l1.Document, l1.EndPoint, l2.EndPoint);
                            addLines.Add(line);

                            newDestinations.AddRange(destinations1);
                            newDestinations.AddRange(destinations2);
                            List<TBBOXDestination> newDestinations2 = newDestinations.Distinct<TBBOXDestination>().ToList();
                            line.XProperties.Add("Destinations", newDestinations2); 
                        }
                        else if (l1.EndPoint == l2.EndPoint && l2.StartPoint != l2.StartPoint)
                        {
                            vdLine line = new vdLine(l1.Document, l1.StartPoint, l2.StartPoint);
                            addLines.Add(line);
                            newDestinations.AddRange(destinations1);
                            newDestinations.AddRange(destinations2);
                            List<TBBOXDestination> newDestinations2 = newDestinations.Distinct<TBBOXDestination>().ToList();
                            line.XProperties.Add("Destinations", newDestinations2);
                        }
                        else if (l1.StartPoint == l2.EndPoint && l1.EndPoint != l2.StartPoint)
                        {

                            vdLine line = new vdLine(l1.Document, l1.EndPoint, l2.StartPoint);
                            addLines.Add(line);
                            newDestinations.AddRange(destinations1);
                            newDestinations.AddRange(destinations2);
                            List<TBBOXDestination> newDestinations2 = newDestinations.Distinct<TBBOXDestination>().ToList();
                            line.XProperties.Add("Destinations", newDestinations2);
                        }
                        else if (l1.EndPoint == l2.StartPoint && l1.StartPoint != l2.EndPoint)
                        {
                            vdLine line = new vdLine(l1.Document, l1.StartPoint, l2.EndPoint);
                            addLines.Add(line);
                            newDestinations.AddRange(destinations1);
                            newDestinations.AddRange(destinations2);
                            List<TBBOXDestination> newDestinations2 = newDestinations.Distinct<TBBOXDestination>().ToList();
                            line.XProperties.Add("Destinations", newDestinations2);
                        }
                    }
                }
            }

            if (addLines.Count > 0)
            {

                foreach (vdLine line in removeLines)
                {
                    lines.Remove(line);
                }
                lines.AddRange(addLines);
            }

        }


        public static void breakLine(List<vdLine> lines)
        {
            gPoints gps = new gPoints();
            for (int i = 0; i < lines.Count - 1; i++)
            {
                gps.Add(lines[i].StartPoint);
                gps.Add(lines[i].EndPoint);
            }
            List<vdLine> newlines = new List<vdLine>();
            foreach (gPoint p in gps)
            {
                foreach (vdLine line in lines)
                {
                    bool isOn = line.PointOnCurve(p, false);
                    if (isOn)
                    {
                        if (line.StartPoint != p && line.EndPoint != p)
                        {
                            vdFigure f = new vdLine();
                            line.Break(p, p, out f);
                            vdLine l = (vdLine)f;

                            List<TBBOXDestination> lineDestinations = (List<TBBOXDestination>)line.XProperties["Destinations"];
                            List<TBBOXDestination> newDestinations = new List<TBBOXDestination>();
                            newDestinations.AddRange(lineDestinations);
                            List<TBBOXDestination> newDestinations2 = newDestinations.Distinct<TBBOXDestination>().ToList();
                            l.XProperties.Add("Destinations", newDestinations2); 
                            newlines.Add(l);
                        }
                    }
                }
            }
            lines.AddRange(newlines);
        }
        public static void removeSubRoute(vdDocument doc, Route route, List<Instrument> instruments, TBBOXDestination destination, int Unit = 3000)
        {

            List<Connector> endConnectors = new List<Connector>();
            List<Instrument> noneInstruments = new List<Instrument>();
            foreach (Instrument instrument in instruments)
            {
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in route.connectors)
                {
                    if ((connector.sNode.connectors.Count == 1 || connector.eNode.connectors.Count == 1)) continue;

                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (dist > Unit) continue;
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minConnector = connector;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
                else noneInstruments.Add(instrument);
            }


            // 앞에 조건에 연결된 connetor가 없는 instrumnet를 대상으로 최단 거리의 connecot와 연결
            // connector가 많은 쪽으로 밀기 ************************

            route.connectors = route.connectors.OrderBy(x => x.instruments.Count).ToList<Connector>();
            route.connectors.Reverse();
            foreach (Instrument instrument in noneInstruments)
            {
                Connector minConnector = null;
                foreach (Connector connector in route.connectors)
                {
                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (dist < Unit)
                    {
                        minConnector = connector;
                        break;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }

            /*
                foreach (Instrument instrument in noneInstruments)
                {
                    Connector minConnector = null;
                    double minDist = double.MaxValue;
                    foreach (Connector connector in route.connectors)
                    {
                        gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                        double dist = gp.Distance2D(instrument.centerPoint);
                        if (minDist > dist)
                        {
                            minDist = dist;
                            minConnector = connector;
                        }
                    }
                    if (minConnector != null) minConnector.instruments.Add(instrument);

                }
            */


            //    끝단이고 연결된 instrumnet가 없는 경우   목적지 끝단의 경우를 생각해야함
            for (int i = 0; i < route.connectors.Count; i++)
            {
                Connector c = route.connectors[i];

                if (c.sNode.gp == destination.mp.gp || c.eNode.gp == destination.mp.gp) continue;

                if (c.sNode.connectors.Count == 1 || c.eNode.connectors.Count == 1)
                {
                    if (c.instruments.Count == 0)
                    {
                        route.connectors.Remove(c);

                        route.delectedConnector.Add(c);
                        i--;
                    }
                }
            }


            foreach (Connector connector1 in route.delectedConnector)
            {
                connector1.sNode.connectors.Remove(connector1);
                connector1.eNode.connectors.Remove(connector1);
            }
            for (int i = 0; i < route.nodes.Count; i++)
            {
                if (route.nodes[i].connectors.Count == 0)
                {
                    route.nodes.Remove(route.nodes[i]);
                    i--;
                }
            }

            foreach (Connector connector1 in route.connectors) connector1.instruments.Clear();

            //남아 있는 connector에서 최단 거리의 connector와 연결

            foreach (Instrument instrument in instruments)
            {
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in route.connectors)
                {
                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minConnector = connector;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }
        } 
        public static void removeSubRoute(vdDocument doc, Route route, List<Instrument> instruments, int Unit = 3000)
        {

            List<Connector> endConnectors = new List<Connector>();
            List<Instrument> noneInstruments = new List<Instrument>();
            foreach (Instrument instrument in instruments)
            {
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in route.connectors)
                {
                    if ((connector.sNode.connectors.Count == 1 || connector.eNode.connectors.Count == 1)) continue;

                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    var gap = Unit;
                    if (instrument.t2 == "Destination")
                    {
                        gap = UnitDestinationNearest;
                    }
                    if (dist > gap) continue;
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minConnector = connector;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
                else noneInstruments.Add(instrument);
            }


            // 앞에 조건에 연결된 connetor가 없는 instrumnet를 대상으로 최단 거리의 connecot와 연결
            // connector가 많은 쪽으로 밀기 ************************

            route.connectors = route.connectors.OrderBy(x => x.instruments.Count).ToList<Connector>();
            route.connectors.Reverse();
            foreach (Instrument instrument in noneInstruments)
            {
                Connector minConnector = null;
                foreach (Connector connector in route.connectors)
                {
                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (dist < Unit)
                    {
                        minConnector = connector;
                        break;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }

            /*
                foreach (Instrument instrument in noneInstruments)
                {
                    Connector minConnector = null;
                    double minDist = double.MaxValue;
                    foreach (Connector connector in route.connectors)
                    {
                        gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                        double dist = gp.Distance2D(instrument.centerPoint);
                        if (minDist > dist)
                        {
                            minDist = dist;
                            minConnector = connector;
                        }
                    }
                    if (minConnector != null) minConnector.instruments.Add(instrument);

                }
            */

            //끝단이고 연결된 instrumnet가 없는 경우
            for (int i = 0; i < route.connectors.Count; i++)
            {
                Connector c = route.connectors[i];

                if (c.sNode.connectors.Count == 1 || c.eNode.connectors.Count == 1)
                {
                    if (c.instruments.Count == 0)
                    {
                        route.connectors.Remove(c);

                        route.delectedConnector.Add(c);
                        i--;
                    }
                }
            }


            foreach (Connector connector1 in route.delectedConnector)
            {
                connector1.sNode.connectors.Remove(connector1);
                connector1.eNode.connectors.Remove(connector1);
            }
            for (int i = 0; i < route.nodes.Count; i++)
            {
                if (route.nodes[i].connectors.Count == 0)
                {
                    route.nodes.Remove(route.nodes[i]);
                    i--;
                }
            }

            foreach (Connector connector1 in route.connectors) connector1.instruments.Clear();

            //남아 있는 connector에서 최단 거리의 connector와 연결

            foreach (Instrument instrument in instruments)
            {
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in route.connectors)
                {
                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minConnector = connector;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }
        }
        public static void joinLines(vdDocument doc, Route route, List<Instrument> instruments)
        {
            List<vdLine> lines = new List<vdLine>();
            for (int i = 0; i < route.connectors.Count; i++)
            {
                Connector connector = (Connector)route.connectors[i];
                lines.Add(connector.line);
            }

            route.connectors.Clear();
            route.nodes.Clear();

            ///////
            removeLines(lines);

            gPoints gsa11 = new gPoints();
            gPoints gsa22 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa11.Add(l.StartPoint);
                gsa11.Add(l.EndPoint);
                gsa22.Add(l.StartPoint);
                gsa22.Add(l.EndPoint);
            }
            gsa11.RemoveEqualPoints();

            foreach (gPoint p1 in gsa11)
            {
                int co = 0;
                foreach (gPoint p2 in gsa22)
                {
                    if (p1.x == p2.x && p1.y == p2.y) co++;

                }

                if (co == 2)
                {
                    vdLine sl = null;
                    vdLine el = null;
                    foreach (vdLine l in lines)
                    {
                        if (l.StartPoint.x == p1.x && l.StartPoint.y == p1.y) el = l;
                        if (l.EndPoint.x == p1.x && l.EndPoint.y == p1.y) sl = l;
                    }
                    if (sl != null && el != null)
                    {
                        if (sl.StartPoint.x - el.EndPoint.x == 0 || sl.StartPoint.y - el.EndPoint.y == 0)
                        {
                            vdLine newLine = new vdLine(doc, new gPoint(sl.StartPoint.x, sl.StartPoint.y), new gPoint(el.EndPoint.x, el.EndPoint.y));
                            int index1 = lines.FindIndex(0, l => l.Equals(sl));
                            int index2 = lines.FindIndex(0, l => l.Equals(el));
                            lines.Insert(index1, newLine);
                            lines.Remove(sl);
                            lines.Remove(el);
                        }
                    }
                }
            }
            /////////////////////////////





            /////////////////////////////////
            gPoints gsa1 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa1.Add(l.StartPoint);
                gsa1.Add(l.EndPoint);
            }
            gsa1.RemoveEqualPoints();

            foreach (gPoint p1 in gsa1)
            {

                Node n = new Node(p1);
                route.nodes.Add(n);
            }

            //   MessageBox.Show(" ----     " + lines.Count);
            route.connectors.Clear();

            foreach (vdLine l in lines)
            {
                Node sNode = getNode(route.nodes, l.StartPoint);
                Node eNode = getNode(route.nodes, l.EndPoint);
                sNode.nodes.Add(eNode);
                eNode.nodes.Add(sNode);
                Connector connector = new Connector(sNode, eNode, doc, route.destination);
                sNode.connectors.Add(connector);
                eNode.connectors.Add(connector);
                route.connectors.Add(connector);
            }

            foreach (Instrument instrument in instruments)
            {
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in route.connectors)
                {
                    gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                    double dist = gp.Distance2D(instrument.centerPoint);
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minConnector = connector;
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }

        }
        public static bool joinLines2(vdDocument doc, Routes routes, List<Instrument> instruments, int maxDistance, RouteInfo routeInfo)
        {
            List<vdLine> lines = routes.lines;

            routes.connectors.Clear();
            routes.nodes.Clear();

            removeLines(lines);



            /////////////////////////////////
            gPoints gsa1 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa1.Add(l.StartPoint);
                gsa1.Add(l.EndPoint);
            }
            gsa1.RemoveEqualPoints();

            foreach (gPoint p1 in gsa1)
            {

                Node n = new Node(p1);
                routes.nodes.Add(n);
            }

            //   MessageBox.Show(" ----     " + lines.Count);
            routes.connectors.Clear();

            foreach (vdLine l in lines)
            {
                Node sNode = getNode(routes.nodes, l.StartPoint);
                Node eNode = getNode(routes.nodes, l.EndPoint);
                sNode.nodes.Add(eNode);
                eNode.nodes.Add(sNode);

                List<TBBOXDestination> destinations = (List<TBBOXDestination>)l.XProperties["Destinations"];
                Connector connector = new Connector(sNode, eNode, doc, destinations);
                sNode.connectors.Add(connector);
                eNode.connectors.Add(connector);
                routes.connectors.Add(connector);
            }

            foreach (InstrumentInfoEntity iie in routeInfo.DGV_LstInstrument)// foreach (Instrument instrument in instruments)
            {
                var instrument = iie.Instrument;
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in routes.connectors)
                {
                    List<TBBOXDestination> destinations = (List<TBBOXDestination>)connector.line.XProperties["Destinations"];
                    if (destinations.Contains(routeInfo.FindIO_TBBox(iie.To)))
                    {
                        gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                        double dist = gp.Distance2D(instrument.centerPoint);
                        if (minDist > dist && dist <= maxDistance)
                        {
                            minDist = dist;
                            minConnector = connector;
                        }
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
                else return false;
            }
            return true;
        }

        // for detecting the number of edge and the bending.

        public static void joinLines2(vdDocument doc, Routes routes, RouteInfo routeInfo ) //List<Instrument> instruments
        {
            List<vdLine> lines = routes.lines;

            routes.connectors.Clear();
            routes.nodes.Clear();

            removeLines(lines);



            /////////////////////////////////
            gPoints gsa1 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa1.Add(l.StartPoint);
                gsa1.Add(l.EndPoint);
            }
            gsa1.RemoveEqualPoints();

            foreach (gPoint p1 in gsa1)
            {

                Node n = new Node(p1);
                routes.nodes.Add(n);
            }

            //   MessageBox.Show(" ----     " + lines.Count);
            routes.connectors.Clear();

            foreach (vdLine l in lines)
            {
                Node sNode = getNode(routes.nodes, l.StartPoint);
                Node eNode = getNode(routes.nodes, l.EndPoint);
                sNode.nodes.Add(eNode);
                eNode.nodes.Add(sNode);

                List<TBBOXDestination> destinations = (List<TBBOXDestination>)l.XProperties["Destinations"];
                Connector connector = new Connector(sNode, eNode, doc, destinations);
                sNode.connectors.Add(connector);
                eNode.connectors.Add(connector);
                routes.connectors.Add(connector);
            }

            foreach (InstrumentInfoEntity iie in routeInfo.DGV_LstInstrument )
            {
                var instrument = iie.Instrument;
                Connector minConnector = null;
                double minDist = double.MaxValue;
                foreach (Connector connector in routes.connectors)
                {
                    List<TBBOXDestination> destinations = (List<TBBOXDestination>)connector.line.XProperties["Destinations"];
                    if (destinations.Contains(routeInfo.FindIO_TBBox(iie.To)))
                    {
                        gPoint gp = connector.line.getClosestPointTo(instrument.centerPoint);
                        double dist = gp.Distance2D(instrument.centerPoint);
                        if (minDist > dist)
                        {
                            minDist = dist;
                            minConnector = connector;
                        }
                    }
                }
                if (minConnector != null) minConnector.instruments.Add(instrument);
            }
        }



        public static void analysisRoute(vdDocument doc, Route route, List<Instrument> instruments)
        {
            List<vdLine> lines = route.lines;
            for (int i = 0; i < route.subRoutes.Count; i++)
            {
                SubRoute sr1 = (SubRoute)route.subRoutes[i];
                vdPolyline poly = new vdPolyline(doc);
                poly.VertexList.AddRange(sr1.polyGps);
                vdEntities entities = poly.Explode();
                foreach (vdLine en in entities)
                {
                    lines.Add(en);
                }
            }

            gPoints gsa1 = analysisRoute(doc, lines);

            foreach (gPoint p1 in gsa1)
            {
                Node n = new Node(p1);
                route.nodes.Add(n);
            }
            route.connectors.Clear();
            foreach (vdLine l in lines)
            {
                Node sNode = getNode(route.nodes, l.StartPoint);
                Node eNode = getNode(route.nodes, l.EndPoint);
                sNode.nodes.Add(eNode);
                eNode.nodes.Add(sNode);
                Connector connector = new Connector(sNode, eNode, doc, route.destination);
                sNode.connectors.Add(connector);
                eNode.connectors.Add(connector);
                route.connectors.Add(connector);
            }
        }




        public static gPoints analysisRoute(vdDocument doc, List<vdLine> lines)
        {


            for (int i = 0; i < lines.Count - 1; i++)
            {

                vdLine l1 = (vdLine)lines[i];
                int t1 = -1;
                if (l1.StartPoint.x - l1.EndPoint.x == 0) t1 = 0;
                if (l1.StartPoint.y - l1.EndPoint.y == 0) t1 = 1;

                for (int j = i + 1; j < lines.Count; j++)
                {
                    vdLine l2 = (vdLine)lines[j];
                    gPoints gPoints = new gPoints();
                    int t2 = -1;
                    if (l2.StartPoint.x - l2.EndPoint.x == 0) t2 = 0;
                    if (l2.StartPoint.y - l2.EndPoint.y == 0) t2 = 1;

                    if (t1 == t2)
                    {

                        continue;
                    }
                    bool isIntersected = l1.IntersectWith(l2, VdConstInters.VdIntOnBothOperands, gPoints);
                    if (isIntersected)
                    {
                        gPoint p = gPoints[0];
                        if (!(p.x == l1.StartPoint.x && p.y == l1.StartPoint.y) && !(p.x == l1.EndPoint.x && p.y == l1.EndPoint.y))
                        {
                            vdLine sl = new vdLine(doc, l1.StartPoint, p);
                            vdLine el = new vdLine(doc, p, l1.EndPoint);

                            lines.Add(sl);
                            lines.Add(el);
                            lines.Remove(l1);
                            i--;
                            break;
                        }
                        else if (!(p.x == l2.StartPoint.x && p.y == l2.StartPoint.y) && !(p.x == l2.EndPoint.x && p.y == l2.EndPoint.y))
                        {
                            vdLine sl = new vdLine(doc, l2.StartPoint, p);
                            vdLine el = new vdLine(doc, p, l2.EndPoint);
                            lines.Add(sl);
                            lines.Add(el);
                            lines.Remove(l2);
                            j--;
                        }
                    }
                }
            }

            gPoints gsa11 = new gPoints();
            gPoints gsa22 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa11.Add(l.StartPoint);
                gsa11.Add(l.EndPoint);
                gsa22.Add(l.StartPoint);
                gsa22.Add(l.EndPoint);
            }
            gsa11.RemoveEqualPoints();

            foreach (gPoint p1 in gsa11)
            {
                int co = 0;
                foreach (gPoint p2 in gsa22)
                {
                    if (p1.x == p2.x && p1.y == p2.y) co++;

                }
                //      Console.WriteLine(p1.x + "  " + p1.y + "  " + co);

                if (co == 2)
                {
                    vdLine sl = null;
                    vdLine el = null;
                    foreach (vdLine l in lines)
                    {

                        if (l.StartPoint.x == p1.x && l.StartPoint.y == p1.y)
                        {
                            el = l;
                        }
                        if (l.EndPoint.x == p1.x && l.EndPoint.y == p1.y)
                        {
                            sl = l;
                        }
                    }
                    if (sl != null && el != null)
                    {
                        if (sl.StartPoint.x - el.EndPoint.x == 0 || sl.StartPoint.y - el.EndPoint.y == 0)
                        {


                            vdLine newLine = new vdLine(doc, new gPoint(sl.StartPoint.x, sl.StartPoint.y), new gPoint(el.EndPoint.x, el.EndPoint.y));

                            int index1 = lines.FindIndex(0, l => l.Equals(sl));
                            int index2 = lines.FindIndex(0, l => l.Equals(el));
                            lines.Insert(index1, newLine);
                            lines.Remove(sl);
                            lines.Remove(el);


                        }
                    }
                }
            }

            ///////////////////////
            gPoints gsa1 = new gPoints();
            foreach (vdLine l in lines)
            {
                gsa1.Add(l.StartPoint);
                gsa1.Add(l.EndPoint);
            }
            gsa1.RemoveEqualPoints();



            return gsa1;

        }


        public static Node getNode(List<Node> nodes, gPoint gp)
        {
            foreach (Node n in nodes)
            {
                if (n.gp.x == gp.x && n.gp.y == gp.y) return n;
            }
            return null;
        }
        public static int[,] getNewMatrix(int[,] newMatrix, List<vdLine> lines, MatrixPoint[,] mps)
        {
            foreach (vdLine line in lines)
            {
                MatrixPoint smp = getMatrixPoint(line.StartPoint.x, line.StartPoint.y, mps);
                MatrixPoint emp = getMatrixPoint(line.EndPoint.x, line.EndPoint.y, mps);


                int minX = 0; ;
                int minY = 0;
                int maxX = 0;
                int maxY = 0;

                if (smp.y == emp.y)
                {
                    if (smp.x > emp.x)
                    {
                        minX = emp.x;
                        maxX = smp.x;
                    }
                    else
                    {
                        minX = smp.x;
                        maxX = emp.x;
                    }
                    for (int i = minX; i <= maxX; i++) newMatrix[i, smp.y] = 3;
                }
                if (smp.x == emp.x)
                {
                    if (smp.y > emp.y)
                    {
                        minY = emp.y;
                        maxY = smp.y;
                    }
                    else
                    {
                        minY = smp.y;
                        maxY = emp.y;
                    }
                    for (int i = minY; i <= maxY; i++) newMatrix[smp.x, i] = 3;
                }
            }
            return newMatrix;
        }

        public static int[,] getNewMatrix(int[,] newMatrix, Routes selectedRoutes, MatrixPoint[,] mps)
        {
            foreach (Connector connector in selectedRoutes.connectors)
            {
                if (selectedRoutes.selectedConnectors.Contains(connector)) continue;

                Console.WriteLine(connector.sNode.gp.x + "  11  " + connector.sNode.gp.y);
                Console.WriteLine(connector.eNode.gp.x + "  11  " + connector.eNode.gp.y);
                MatrixPoint smp = getMatrixPoint(connector.sNode.gp.x, connector.sNode.gp.y, mps);
                MatrixPoint emp = getMatrixPoint(connector.eNode.gp.x, connector.eNode.gp.y, mps);


                int minX = 0; ;
                int minY = 0;
                int maxX = 0;
                int maxY = 0;

                if (smp.y == emp.y)
                {
                    if (smp.x > emp.x)
                    {
                        minX = emp.x;
                        maxX = smp.x;
                    }
                    else
                    {
                        minX = smp.x;
                        maxX = emp.x;
                    }
                    for (int i = minX; i <= maxX; i++) newMatrix[i, smp.y] = 2;
                }
                if (smp.x == emp.x)
                {
                    if (smp.y > emp.y)
                    {
                        minY = emp.y;
                        maxY = smp.y;
                    }
                    else
                    {
                        minY = smp.y;
                        maxY = emp.y;
                    }
                    for (int i = minY; i <= maxY; i++) newMatrix[smp.x, i] = 2;
                }
            }
            return newMatrix;
        }


        public static int[,] getNewMatrix(int[,] newMatrix, Route selectedRoute, MatrixPoint[,] mps)
        {
            foreach (Connector connector in selectedRoute.connectors)
            {
                if (selectedRoute.disconnectedConnectors.Contains(connector)) continue;

                Console.WriteLine(connector.sNode.gp.x + "  11  " + connector.sNode.gp.y);
                Console.WriteLine(connector.eNode.gp.x + "  11  " + connector.eNode.gp.y);
                MatrixPoint smp = getMatrixPoint(connector.sNode.gp.x, connector.sNode.gp.y, mps);
                MatrixPoint emp = getMatrixPoint(connector.eNode.gp.x, connector.eNode.gp.y, mps);
                Console.WriteLine(smp.x + "  22  " + smp.y);
                Console.WriteLine(emp.x + "  22  " + emp.y);

                int minX = 0; ;
                int minY = 0;
                int maxX = 0;
                int maxY = 0;

                if (smp.y == emp.y)
                {
                    if (smp.x > emp.x)
                    {
                        minX = emp.x;
                        maxX = smp.x;
                    }
                    else
                    {
                        minX = smp.x;
                        maxX = emp.x;
                    }
                    for (int i = minX; i <= maxX; i++) newMatrix[i, smp.y] = 2;
                }
                if (smp.x == emp.x)
                {
                    if (smp.y > emp.y)
                    {
                        minY = emp.y;
                        maxY = smp.y;
                    }
                    else
                    {
                        minY = smp.y;
                        maxY = emp.y;
                    }
                    for (int i = minY; i <= maxY; i++) newMatrix[smp.x, i] = 2;
                }
            }
            return newMatrix;
        }

        public static List<vdLine> getSelectedLine(Routes selectedRoutes)
        {
            List<vdLine> lines = new List<vdLine>();


            foreach (Connector connector in selectedRoutes.connectors)
            {
                if (selectedRoutes.selectedConnectors.Contains(connector))
                {

                    lines.Add(connector.line);
                }
            }
            return lines;
        }
        public static MatrixPoint getMatrixPoint(double x, double y, MatrixPoint[,] mps)
        {
            for (int i = 0; i < mps.GetLength(0); i++)
            {
                for (int j = 0; j < mps.GetLength(1); j++)
                {
                    if (mps[i, j].gp.x == x && mps[i, j].gp.y == y)
                    {

                        return mps[i, j];
                    }

                }
            }
            return null;
        }
    }
}
