using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Entity
{
    public class CableScheduleEntity
    {
        public class CableSchedule
        {
            public string TagNo { get; set; }
            public string System { get; set; }
            public string CableSpecification { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string SignalType { get; set; }
            public string CableLength_m { get; set; }
            public string Remark { get; set; }

        }
        public class DetectedInstrumentList
        {
            public string No { get; set; }
            public string T1 { get; set; }
            public string T2 { get; set; }
            public string Type { get; set; }
            public string System { get; set; }
            public string To { get; set; }
        }
        public class CableScheduleExport
        {
            public string AssigedDuct { get; set; }
            public string SegmentName { get; set; }
            public string TotalCable { get; set; }
            public string Length { get; set; }
            //public string OptimalDuctSize { get; set; }

            public string TotalArea { get; set; }
            public string OptimalSize { get; set; }
            public string OptimalRatio { get; set; }
            public string UserDefinedSize { get; set; }
            public string UserDefinedRato { get; set; }

            public string Signal { get; set; }

            public string TagName { get; set; }
            public string To { get; set; }
            public string Cable { get; set; }

        }
        public class InstrumentCableScheduleExport
        {
            public string AssignedDuct { get; set; }
            public string Signal { get; set; }
            public string TagName { get; set; }
            public string To { get; set; }
            public string Cable { get; set; }
            public string TotalLength { get; set; }
            public string TotalSegement { get; set; }
            public string SegmentName { get; set; }
            public string Length { get; set; }

        }

        public class DuctSchedule
        {
           public string No { get; set; }
           public string AssignedDuct { get; set; }
           public string Length { get; set; }
           public string Quantity { get; set; }

        }
        public class DuctTypeSchedule
        {
            public string No { get; set; }
            public string AssignedDuct { get; set; }
            public string Type { get; set; }
            public string Length { get; set; }
            public string Quantity { get; set; }

        }
        public class DuctCableSchedule
        {
            public string No { get; set; }
            public string Type { get; set; }
            public string Length { get; set; }
            public string Quantity { get; set; }

        }
    }
}