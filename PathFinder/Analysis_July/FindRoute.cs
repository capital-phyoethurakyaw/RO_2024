using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Geometry;

namespace RouteOptimizer.Analysis
{
    internal class FindRoute
    {
        public static List<Point> findWay1(int[,] matrix, Point insPoint, Point endPoint)
        {
            List<Point> gs = new List<Point>();
            if (endPoint.X < insPoint.X)
            {
                for (int i = endPoint.X; i <= insPoint.X; i++)
                {
                    if (matrix[i, endPoint.Y] != int.MaxValue)
                    {
                       
                        gs.Add(new Point(i * 2, endPoint.Y * 2));
                    }
                    else return null;
                }
                if (endPoint.Y < insPoint.Y)
                {
                    for (int i = endPoint.Y+1; i <= insPoint.Y; i++)
                    {
                      
                         if (matrix[insPoint.X, i] != int.MaxValue) gs.Add(new Point(insPoint.X * 2, i * 2));
                           else return null;
                    }

                }
                else {
                    for (int i = endPoint.Y-1; i >= insPoint.Y; i--)
                    {
                         if (matrix[insPoint.X, i] != int.MaxValue) gs.Add(new Point(insPoint.X * 2, i * 2));
                         else return null;
                    }
                }
                
            }else {

             
                for (int i = endPoint.X; i >= insPoint.X; i--)
                {
                    if (matrix[i, endPoint.Y] != int.MaxValue) gs.Add(new Point(i * 2, endPoint.Y * 2));
                     else return null;
                }
               
                if (endPoint.Y < insPoint.Y)
                {
                    for (int i = endPoint.Y +1; i <= insPoint.Y; i++)
                    {
                        if (matrix[insPoint.X , i] != int.MaxValue) gs.Add(new Point(insPoint.X * 2, i * 2));
                        else return null;
                    }

                }
              
                else
                {
                    for (int i = endPoint.Y-1 ; i >= insPoint.Y; i--)
                    {
                        if (matrix[insPoint.X, i] != int.MaxValue) gs.Add(new Point(insPoint.X * 2, i * 2));
                        else return null;
                    }
                }
                 
            }
 
            return gs;
        }
        public static List<Point> findWay2(int[,] matrix, Point insPoint, Point endPoint)
        {

            List<Point> gs = new List<Point>();
            if (endPoint.Y < insPoint.Y)
            {
                for (int i = endPoint.Y ; i <= insPoint.Y; i++)
                {
                    if (matrix[endPoint.X, i] != int.MaxValue)
                    {

                        gs.Add(new Point(endPoint.X * 2, i * 2));
                    }
                    else return null;
                }

                if (endPoint.X < insPoint.X)
                {
                    for (int i = endPoint.X+1  ; i <= insPoint.X; i++)
                    {
                        if (matrix[ i, insPoint.Y] != int.MaxValue) gs.Add(new Point( i * 2, insPoint.Y * 2));
                        else return null;
                    }

                }
                else
                {
                    for (int i = endPoint.X-1 ; i >= insPoint.X; i--)
                    {
                        if (matrix[i,insPoint.Y ] != int.MaxValue) gs.Add(new Point(i * 2, insPoint.Y * 2));
                        else return null;
                    }
                }

             
            }

           
            else
            {
                for (int i = endPoint.Y; i >= insPoint.Y; i--)
                {
                    try
                    {
                        if (matrix[endPoint.Y, i] != int.MaxValue)
                            gs.Add(new Point(endPoint.Y * 2, i * 2));
                        else
                            return null;
                    }
                    catch
                    {
                        return null;
                    }
                }


                if (endPoint.X < insPoint.X)
                {
                    for (int i = endPoint.X+1 ; i <= insPoint.X; i++)
                    {
                        if (matrix[i, insPoint.Y] != int.MaxValue) gs.Add(new Point(i * 2,insPoint.Y * 2));
                        else return null;
                    }

                }

                else
                {
                    for (int i = endPoint.X-1 ; i >= insPoint.X; i--)
                    {
                        if (matrix[ i, insPoint.Y] != int.MaxValue) gs.Add(new Point( i * 2,insPoint.X * 2));
                        else return null;
                    }
                }
            }
            return gs;
        }

        public static SubRoute findRoute(int[,] matrix, SubRoute route) {

            SubRoute addRoute = (SubRoute)route.Clone();
            addRoute.points = new List<Point>();
            addRoute.gps = new gPoints();
            foreach (Point p in route.points) {
                addRoute.points.Add(p);
            }

            
            foreach (gPoint p in route.gps)
            {
                addRoute.gps.Add(p);
            }

         

            for (int i = 0; i < addRoute.gps.Count - 2; i++)//addRoute.gps.Count - 2//
            {
                gPoint gp0 = addRoute.gps[i];
                gPoint gp1 = addRoute.gps[i + 1];
                gPoint gp2 = route .gps[i + 2];

                 
              if (gp0.x == gp1.x)
              {
                   MessageBox.Show("gp0.x == gp1.x");
                   
                    double xx = -1;
                    double yy = -1;
                    xx = gp2.x;


                   



                    if(gp0.y != gp1.y) yy = gp0.y;
                    else if (gp2.y != gp1.y) yy = gp2.y;
                    int x = (int)xx;  ///y row
                    int minY = -1;
                    int maxY = -1;
                    if (gp0.y > gp2.y)
                    {
                        maxY = (int)gp0.y;
                        minY = (int)gp2.y;
                    }
                    else
                    {
                        maxY = (int)gp2.y;
                        minY = (int)gp0.y;
                    }


                    if (gp1.x == gp2.x)
                    {
                        if ((gp1.y < gp0.y && gp1.y < gp2.y) || gp1.y > gp0.y && gp1.y > gp2.y)
                        {
                            gp1.y = gp0.y;
                        }

                        continue;
                    }

                    



                    bool isWay = true;
                    for (int j = minY; j < maxY; j++)
                    {
                       
                        if (matrix[x, j]==int.MaxValue)
                        {
                            isWay = false;
                            break;
                        }
                    }



                    if (!isWay)
                    {
                     //MessageBox.Show("error no way");
                        continue;
                    }

                    int y = (int)yy;  ///x col
                    int minX = -1;
                    int maxX = -1;
                    if (gp0.x > gp2.x)
                    {
                        maxX = (int)gp0.x;
                        minX = (int)gp2.x;
                    }
                    else
                    {
                        maxX = (int)gp2.x;
                        minX = (int)gp0.x;
                    }

                
                    for (int j = minX; j < maxX; j++)
                    {
                        if (matrix[j, y] == int.MaxValue)
                        {
                            isWay = false;
                            break;
                        }
                    }


                    if (!isWay)
                    {
                       //  MessageBox.Show("error no way");
                        continue;
                    }
                    else {

                        gp1.y = yy;
                        gp1.x = xx;
                      //  addRoute.gps.RemoveInLinePoints();
                    }
                }



                   else if (gp0.y == gp1.y)
                  {
            

                    double xx = -1;
                    double yy = -1;
                    yy = gp2.y;

                    if (gp0.x != gp1.x)
                    {
                        xx = gp0.x;
                    }
                    else if (gp2.x != gp1.x)
                    {
                        xx = gp2.x;
                    }



                    if (gp1.y == gp2.y)
                    {
                        if ((gp1.x < gp0.x && gp1.x < gp2.x) || gp1.x > gp0.x && gp1.x > gp2.x)
                        {
                            gp1.x = gp0.x;
                        }
                       // addRoute.gps.RemoveInLinePoints();
                        continue;
                    }
                   


                    int x = (int)xx;  ///y row
                    int minY = -1;
                    int maxY = -1;

                    if (gp0.y > gp2.y)
                    {
                        maxY = (int)gp0.y;
                        minY = (int)gp2.y;
                    }
                    else
                    {
                        maxY = (int)gp2.y;
                        minY = (int)gp0.y;
                    }
                    /* 월래 빼졌음
                    if (gp0.y > yy)
                    {
                        minY = (int)yy;
                        maxY = (int)gp0.y;
                    }
                    else
                    {
                        minY = (int)gp0.y;
                        maxY = (int)yy;  
                    }
                    */
                    bool isWay = true;
                     
                   
                    for (int j = minY; j < maxY; j++)
                    {
                        if (matrix[x, j] == int.MaxValue)
                        {
                            isWay = false;
                            break;
                        }
                    }
                    
                    if (!isWay)
                {
                   MessageBox.Show("error no way");
                    continue;
                }

                    int y =  (int)yy;  ///x col
                    int minX = -1;
                    int maxX = -1;
                    /* 월래 빼졌음
                    if (gp2.x > xx)
                    {
                        minX = (int)xx;
                        maxX = (int)gp2.x;
                    }
                    else
                    {
                        minX = (int)gp2.x;
                        maxX = (int)xx;
                    }
                    */


                     
                   if (gp0.x > gp2.x)
                   {
                       maxX = (int)gp0.x;
                       minX = (int)gp2.x;
                   }
                   else
                   {
                       maxX = (int)gp2.x;
                       minX = (int)gp0.x;
                   }



               for (int j = minX; j < maxX; j++)
                   {
                       if (matrix[j, y] == int.MaxValue)
                       {
                           isWay = false;
                           break;
                       }
                   }
                     
                    if (isWay)
                    {
                       gp1.x = xx;
                       gp1.y =yy;

                
                    }
                    else
                    {
                    //gp1.x = gp0.x;
                    ///  gp1.y = gp2.y;
                     //MessageBox.Show("This is no way2");
                      continue;
                  
                    }
                  




                }
                else {// MessageBox.Show("This is no way3");
                    }

            }
            //check addroute route가 같은지 체크 같으면 retrun null 




          


            return addRoute;
        }
    }
}
