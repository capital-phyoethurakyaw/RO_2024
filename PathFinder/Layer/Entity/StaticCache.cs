namespace RouteOptimizer.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.RightsManagement;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data;
    using System.IO;
    using System.Diagnostics;

    public class StaticCache
    {
        public static string path = System.IO.Directory.GetCurrentDirectory().ToString().Replace("bin\\Debug", "DS").Contains("DS") ? System.IO.Directory.GetCurrentDirectory().ToString().Replace("bin\\Debug", "DS")   :System.IO.Directory.GetCurrentDirectory().ToString().Replace("bin\\Debug", "DS") + "\\DS"
            ; //get the directory of DataSource folder
      
      
        public static void FileCopy(string source, string desti)
        {
            try
            {
                //MessageBox.Show(desti);
                File.Copy(source, desti, true);
            }
            catch (Exception ex)
            {
                try
                {
                    //desti = "'" + desti + "'";
                    //ProcessStartInfo psi = new ProcessStartInfo
                    //{
                    //    CreateNoWindow = true,
                    //    FileName = "powershell.exe",
                    //    Arguments = $"xcopy \"{source}\" \"{desti}\" /c /q",
                    //    Verb = "runas"
                    //};

                    //Process.Start(psi);
                }
                catch (Exception exx)
                {
                //    MessageBox.Show(exx.StackTrace);
                }
            }

        }
        public static string DataSourceAnalysisDuctSchedule = @"C:\RouteOptimizer\AnalysisDuct\";
        public static string DataSourceAnalysisDuctTypeSchedule = @"C:\RouteOptimizer\AnalysisDuctType\";
        public static string DataSourceAnalysisDuctCableSchedule = @"C:\RouteOptimizer\AnalysisDuctCable\";

        public static string DataSourceCableDuctSchedule = @"C:\RouteOptimizer\CableDuctSchedule\";
        public static string DataSourceInstrumentSchedule = @"C:\RouteOptimizer\InstrumentSchedule\";
        public static string DataSourceBasicInfo = path + "\\BasicInfo.csv";
        public static string DataSourcePSCombo = path + "\\PSCombo.csv";
        public static string DataSourcePSCombo1 = path + "\\EnumerationList1-ProjectInfo.csv";
        public static string DataSourcePSCombo2 = path + "\\EnumerationList2-ProjectInfo.csv";
        public static string DataSourcePSCombo3 = path + "\\EnumerationList3-ProjectInfo.csv";
        public static string DataSourceProjectInfo = path + "\\ProjectInfo.csv";
        public static string DataSourceInstrumentList = path + "\\InstrumentList.csv";
        public static string DataSourceCableList = path + "\\CableList.csv";
        public static string DataSourceCableDuctList = path + "\\CableDuctList.csv";
        public static string DataSourceCableDuctCableList = path + "\\CableDuctCableList.csv";
        public static string DataSourceCableDuctTypeList = path + "\\CableDuctTypeList.csv";
        public static string DataSourceMccPkgCombo = path + "\\MccPkgCombo.csv";
        public static string DataSourceMccPkgList = path + "\\MccPkgList.csv";
        public static string DataSourceSystemList = path + "\\SystemList.csv";
        public static string DataSourceSignalList = path + "\\SignalList.csv";
        public static string DataSourceIoroomList = path + "\\IoroomList.csv";
        public static string DataSourceMCCManager = path + "\\MCCManager.csv";
        public static string DataSourceSignal = path + "\\Signal.csv";
        public static string DataSourceInstCable = path + "\\InstCable.csv";//
        public static string GetCableDuctTypeList = path + "\\CableDuctTypeList.csv";//GetCableDuctTypeList

        public static DataTable DataSourceInstrumentType()   // it will be removed later
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Key");
            dt.Columns.Add("Name");
            dt.Columns.Add("Display");

            dt.Rows.Add(new object[] { "0_121", "Pressure Transmitter 121", "Pressure Transmitter 121" });//121
            dt.Rows.Add(new object[] { "1_132", "Temp. Sensor(RTD) 132", "Temp. Sensor(RTD) 132" });//132
            dt.Rows.Add(new object[] { "2_156", "Humidity & Temp. Sensor 156", "Humidity & Temp. Sensor 156" });//156  
            dt.Rows.Add(new object[] { "3_121", "Control Valve(Pneumatic) 121", "156 Control Valve(Pneumatic) 121" });//121 
            dt.Rows.Add(new object[] { "4_121", "Analyzer 121", "Analyzer 121" });//121

            return dt;
        }
    }

   
}
