using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Util
{
    internal class Protocol
    {
        public readonly static char Delimeter_Rooms = '+';
        public readonly static string Instrument_Property_Name = "Instrument";
        public readonly static string Instrument_Layer_Name = "instrument";
        public readonly static string Obstacle_Layer_Name = "obstacle";
        public readonly static string IORoom_Layer_Name = "ioRoom";
    }
}