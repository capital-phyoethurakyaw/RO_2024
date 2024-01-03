namespace RouteOptimizer.Util
{
    using RouteOptimizer;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Drawing;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Runtime.Remoting.Activation;
    using System.Text;
    using System.Threading.Tasks;
    using vdControls;
    using VectorDraw.Geometry;
    using VectorDraw.Professional.Control;
    using VectorDraw.Professional.vdCollections;
    using VectorDraw.Professional.vdFigures;
    using VectorDraw.Professional.vdObjects;
    using VectorDraw.Professional.vdPrimaries;

    internal class ImportLayer
    {
        public static vdLayer GetVdLayer(string name, vdDocument doc)
        {
            foreach (vdLayer layer in doc.Layers)
            {
                if (layer.Name == "instrument")
                {
                    return layer;
                }
                else if (layer.Name == "Obstacle")
                {
                    gPoint lrpoint = new gPoint();
                    lrpoint.z = 0;
                    vdCircle cir = new vdCircle();
                    vdPolyline poly = cir.AsPolyline();



                }
            }
            return null;
        }
    }
}
