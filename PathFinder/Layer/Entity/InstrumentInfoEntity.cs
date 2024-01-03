using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace RouteOptimizer.Entity
{
    public class InstrumentInfoEntity
    {

        public Instrument Instrument { get; set; }
        public int GUID { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string Type { get; set; }  // Class3
        //public string Type1 { get; set; }  // Class2
        public string System { get; set; }
        public int To { get; set; }
        public string Classification_1 { get; set; }
        public string Classification_2 { get; set; }
        public string Classification_3 { get; set; }
        public string Cable { get; set; }
        public byte CHK_AutoRoute { get; set; }
        public byte CHK_EachRoute { get; set; }
        public List<InstCableEntity> LstInstCableEntity { get; set; }
        //public int Iv { get; set; }
        public string LayerName { get; set; }
        //public string TemDestiantionID { get; set; }


    }
}
