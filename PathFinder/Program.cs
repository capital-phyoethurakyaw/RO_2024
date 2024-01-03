using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using RouteOptimizer.PInfoForms;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using RouteOptimizer.Form.TestForm;

namespace RouteOptimizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Test();
            Application.EnableVisualStyles(); 
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled; 
            Application.SetCompatibleTextRenderingDefault(false);  
            try
            { 
                     
                DebugLog.WriteLog("Program started ***************************************"); 
                Application.Run(new MainP1());
               // Application.Run(new MainP1());
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);
                MessageBox.Show(ex.Message);
                DebugLog.WriteLog("Program ended with errors***************************************");
            }
        }

        static void Test()
        {
      
            // var l1 = new List<int> { 1,2,3};
            // var l2 = new List<int> { 1, 2 };
            // var listA = new object[] { "1", "2", "3" };
            // var listB = new object[] { "a", "b", "c" };
            //var l3 = new List<int>() {};
            //   var list2 = new List<int>() { 4, 5, 6 };
            // List<Tuple<int, int>> pairs = list1.SelectMany(item1 => list2.Select(item2 => new Tuple<int, int>(item1, item2))).ToList();
            // BuildPossibleCombination(0, new List<int>());

        }
        //public static List<List<int>> productOptions()
        //{
        //    List<List<int>> vs = new List<List<int>>();
        //    var l1 = new List<int> { 1, 2, 3 };
        //    var l2 = new List<int> { 1, 2 };
        //    var l3 = new List<int> { 5 };
        //    vs.Add(l1);
        //    vs.Add(l2);
        //    vs.Add(l3);

        //    return vs;



        //}
        //public static void AllCombination(int level, List<int> output)
        //{
        //    if (level < productOptions().Count)
        //    {
        //        foreach (var value in productOptions()[level])
        //        {
        //            List<int> resultList = new List<int>();
        //            resultList.AddRange(output);
        //            resultList.Add(value);
        //            if (resultList.Count == productOptions().Count)
        //            {
        //                Console.WriteLine(string.Join(", ", resultList));
        //                Console.WriteLine("-------------------");
        //            }
        //            AllCombination(level + 1, resultList);
        //        }
        //    }
        //}
       

        /*
         * Sequence Setting->
	            SequenceListDatagridview의 순서 변경
	            SequenceListDatagridview의 클릭 방법(더블 클릭)

            RoomGroup Setting -> 
	            RoomListDataGridView의 클릭 방법(더블 클릭) 
	            Order 변경 안되는 문제 ->GridView의 행을 다시 삭제하고 추가

            Analysis 
	            Check 초기 상태 -> Flase
	            Check 변경 시 바로 적용 안되는 문제 해결
	            동선 에러 체크 - 에러 발생 하지 안음
		            N.S ->  4인실 19 -> 4인실 18 -> 4인실 8WAB03 -> 4인실 4 -> 4인실 7 -> N.S
        */
    } 
}
