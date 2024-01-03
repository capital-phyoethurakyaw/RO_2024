using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace RouteOptimizer
{
   public static class DebugLog
    {
        public static void WriteLog(string input)
        {
            try
            {
                var fName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                Settingpath(fName);
                File.AppendAllText(@"C:\RouteOptimizer\Log\" + fName, Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + Environment.NewLine + input);
            }
            catch (Exception ex)
            {

            }
            //try
            //{
            //    var fName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            //    try
            //    {
            //        if (!Directory.Exists(@"C:\RouteOptimizer\Log"))
            //            Directory.CreateDirectory(@"C:\RouteOptimizer\Log");
            //    }
            //    catch(Exception ex)
            //    {
            //        SecurityManager.Permission p = new SecurityManager.Permission();
            //        p.AddRightsByPS(@"C:\RouteOptimizer\Log");
            //        if (!Directory.Exists(@"C:\RouteOptimizer\Log"))
            //            Directory.CreateDirectory(@"C:\RouteOptimizer\Log");
            //    }
            //    //var dt = DateTime.Now.ToLongDateString().Replace("")
            //    if (!File.Exists(@"C:\RouteOptimizer\Log\" + fName))
            //    {
            //        File.Create(@"C:\RouteOptimizer\Log\" + fName); 
            //    }
            //    File.AppendAllText(@"C:\RouteOptimizer\Log\" + fName, Environment.NewLine + input);
            //}
            //catch(Exception ex)
            //{

            //}
        }

        private static void Settingpath(string fName)
        {
            try
            {
                //var fName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!Directory.Exists(@"C:\RouteOptimizer\Log"))
                //    Directory.CreateDirectory(@"C:\RouteOptimizer\Log");

                try
                {
                    if (!Directory.Exists(@"C:\RouteOptimizer\Log"))
                        Directory.CreateDirectory(@"C:\RouteOptimizer\Log");
                }
                catch (Exception ex)
                {
                    SecurityManager.Permission p = new SecurityManager.Permission();
                    p.AddRightsByPS(@"C:\RouteOptimizer\Log");
                    if (!Directory.Exists(@"C:\RouteOptimizer\Log"))
                        Directory.CreateDirectory(@"C:\RouteOptimizer\Log");
                }
                //var dt = DateTime.Now.ToLongDateString().Replace("")
                if (!File.Exists(@"C:\RouteOptimizer\Log\" + fName))
                {
                    File.Create(@"C:\RouteOptimizer\Log\" + fName);
                }
            }
            catch
            {

            }
        }
        public static void WriteLog(Exception input)
        {
            try
            {
                var fName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                Settingpath(fName);
                File.AppendAllText(@"C:\RouteOptimizer\Log\" + fName, Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + Environment.NewLine + input.Message + Environment.NewLine + input.StackTrace );
            }
            catch (Exception ex)
            {

            }

        }
    }
}
