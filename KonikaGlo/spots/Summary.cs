using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace KonikaGlo.spots
{
    class Summary
    {
        public static Dictionary<String, float> calculate(String spotName, SortedDictionary<int, Dictionary<String, float>> result) {

            Dictionary<String, float> output = new Dictionary<String, float>();
            String name = spotName.Substring(0, spotName.IndexOf(" "));

            if (name == "13")
            {
                output.Add("center_lv", result[7]["Lv"]);
                output.Add("center_x", result[7]["x"]);
                output.Add("center_y", result[7]["y"]);
                output.Add("center_u", result[7]["u"]);
                output.Add("center_v", result[7]["v"]);
                output.Add("center_tcp", result[7]["tcp"]);
            }
            if (name == "135")
            {
                output.Add("center_lv_135pt", result[68]["Lv"]);
                output.Add("center_x_135pt", result[68]["x"]);
                output.Add("center_y_135pt", result[68]["y"]);
                output.Add("center_u_135pt", result[68]["u"]);
                output.Add("center_v_135pt", result[68]["v"]);
                output.Add("center_tcp_135pt", result[68]["tcp"]);
            }

            float minLv = -1;
            float maxLv = -1;
            float minx = -1;
            float maxx = -1;
            float miny = -1;
            float maxy = -1;
            float minu = -1;
            float maxu = -1;
            float minv = -1;
            float maxv = -1;
            float mintcp = -1;
            float maxtcp = -1;
            float sum = 0;
            float sumX = 0;
            float sumZ = 0;
            float sumpur = -1;
            float sumdw = -1;

            Dictionary<int, float> u = new Dictionary<int, float>();
            Dictionary<int, float> v = new Dictionary<int, float>();
            Dictionary<int, float> u84 = new Dictionary<int, float>();
            Dictionary<int, float> v84 = new Dictionary<int, float>();
            Dictionary<int, float> u126 = new Dictionary<int, float>();
            Dictionary<int, float> v126 = new Dictionary<int, float>();
            
            foreach (KeyValuePair<int, Dictionary<String, float>> pair in result)
            {
                if (minLv == -1 || minLv > pair.Value["Lv"])
                {
                    minLv = pair.Value["Lv"];
                }
                if (maxLv == -1 || maxLv < pair.Value["Lv"])
                {
                    maxLv = pair.Value["Lv"];
                }
                if (minx == -1 || minx > pair.Value["x"])
                {
                    minx = pair.Value["x"];
                }
                if (maxx == -1 || maxx < pair.Value["x"])
                {
                    maxx = pair.Value["x"];
                }
                if (miny == -1 || miny > pair.Value["y"])
                {
                    miny = pair.Value["y"];
                }
                if (maxy == -1 || maxy < pair.Value["y"])
                {
                    maxy = pair.Value["y"];
                }

                if (minu == -1 || minu > pair.Value["u"])
                {
                    minu = pair.Value["u"];
                }
                if (maxu == -1 || maxu < pair.Value["u"])
                {
                    maxu = pair.Value["u"];
                }
                if (minv == -1 || minv > pair.Value["v"])
                {
                    minv = pair.Value["v"];
                }
                if (maxv == -1 || maxv < pair.Value["v"])
                {
                    maxv = pair.Value["v"];
                }

                if (mintcp == -1 || mintcp > pair.Value["tcp"])
                {
                    mintcp = pair.Value["tcp"];
                }
                if (maxtcp == -1 || maxtcp < pair.Value["tcp"])
                {
                    maxtcp = pair.Value["tcp"];
                }
                sum += pair.Value["Lv"];
                sumX += pair.Value["X"];
                sumZ += pair.Value["Z"];

                sumpur += pair.Value["pur"];
                sumdw += pair.Value["dw"];

                u.Add(pair.Key, pair.Value["u"]);
                v.Add(pair.Key, pair.Value["v"]);

                int[] s = {1, 10, 19, 28, 37, 46, 55, 64, 73, 82, 91, 100, 109, 118, 127, 9, 18, 27, 36, 45, 54, 63, 72, 81, 90, 99, 108, 117, 126, 135}; 
                if (pair.Key > 18 && pair.Key <= 126 && !s.Contains(pair.Key)) {
                    u84.Add(pair.Key, pair.Value["u"]);
                    v84.Add(pair.Key, pair.Value["v"]);
                }
                if (pair.Key > 9) {
                    u126.Add(pair.Key, pair.Value["u"]);
                    v126.Add(pair.Key, pair.Value["v"]);
                }

            }


            if (name == "13")
            {
               double avg = sum / 13;
               double avgX = sumX / 13;
               double avgZ = sumZ / 13;
               double center = result[7]["Lv"];
               double centerX = result[7]["X"];
               double centerZ = result[7]["Z"];

               double maxAvg = -1;
               double max7 = -1;
               double maxAvgdE = -1;
               double max7dE = -1;

               foreach (KeyValuePair<int, Dictionary<String, float>> pair in result)
               {
                   double lv = pair.Value["Lv"];
                   double X = pair.Value["X"];
                   double Z = pair.Value["Z"];
                   if (lv > 0 && avg > 0 && center > 0)
                   {
                       double ysvL = 116 * System.Math.Pow((lv / avg), (1.0 / 3.0)) - 16;
                       double ysv7 = 116 * System.Math.Pow((lv / center), (1.0 / 3.0)) - 16;

                       if (System.Math.Abs(ysvL - 100) > maxAvg)
                           maxAvg = System.Math.Abs(ysvL - 100);
                       if (System.Math.Abs(ysv7 - 100) > max7)
                           max7 = System.Math.Abs(ysv7 - 100);

                       double ysvL1 = System.Math.Pow((lv / avg), (1.0 / 3.0));
                       double ysv71 = System.Math.Pow((lv / center), (1.0 / 3.0));

                       double xsvL = System.Math.Pow((X / avgX), (1.0 / 3.0));
                       double xsv7 = System.Math.Pow((X / centerX), (1.0 / 3.0));
                        
                       double zsvL = System.Math.Pow((Z / avgZ), (1.0 / 3.0));
                       double zsv7 = System.Math.Pow((Z / centerZ), (1.0 / 3.0));

                       double aL = 500 * (xsvL - ysvL1);
                       double a7 = 500 * (xsv7 - ysv71);

                       double bL = 200 * (ysvL1 - zsvL);
                       double b7 = 200 * (ysv71 - zsv7);

                       double dEavg = System.Math.Pow((
                           System.Math.Pow((ysvL - 100), 2.0) +
                           System.Math.Pow(aL, 2.0) +
                           System.Math.Pow(bL, 2.0)
                           ), (1.0 / 2.0));

                       double dE7 = System.Math.Pow((
                        System.Math.Pow((ysv7 - 100), 2.0) +
                        System.Math.Pow(a7, 2.0) +
                        System.Math.Pow(b7, 2.0)
                        ), (1.0 / 2.0));

                       if (dEavg > maxAvgdE)
                           maxAvgdE = dEavg;
                       if (dE7 > max7dE)
                           max7dE = dE7;
                   }
               }

               output.Add("dLstar7", (float)max7);
               output.Add("dLstarAvg", (float)maxAvg);
               output.Add("dE7", (float)max7dE);
               output.Add("dEAvg", (float)maxAvgdE);
            }

            String nme = name;
            if (name == "805" || name == "807")
            {
                nme = "80";
            }

            if (maxLv > 0)
                output.Add("unif_" + name + "pt", 100 * minLv / maxLv);
            if (minLv > 0)
                output.Add("min_" + name + "pt", minLv);
            if (sum > 0)
                output.Add("avg_" + name + "pt", sum / (float)Convert.ToDouble(nme));
            if (sumpur > 0)
                output.Add("pur_" + name + "pt", sumpur / (float)Convert.ToDouble(nme));
            if (sumdw > 0)
                output.Add("dw_" + name + "pt", sumdw / (float)Convert.ToDouble(nme));

            output.Add("ciex_" + name + "pt", maxx - minx);
            output.Add("ciey_" + name + "pt", maxy - miny);
            output.Add("u_" + name + "pt", maxu - minu);
            output.Add("v_" + name + "pt", maxv - minv);
            output.Add("tcp_" + name + "pt", maxtcp > 0 ? (float)(100 * mintcp / maxtcp) : 0);

            // Calculate arbitrary points
            output.Add("color_shift_arbitrary_" + name + "pt", arbitrary(u, v, Convert.ToInt32(nme)));
            // Calculate adjacent points
            short cols = Definitions.instance("").Measures[spotName][0]._cond.col;
            output.Add("color_shift_adjacent_" + name + "pt", adjacent(u, v, cols, Convert.ToInt32(nme)));

            if (name == "135")
            {
                output.Add("color_shift_arbitrary_126pt", arbitrary(u126, v126, Convert.ToInt32(nme)));
                output.Add("color_shift_adjacent_126pt", adjacent(u126, v126, 9, Convert.ToInt32(nme)));
                output.Add("color_shift_arbitrary_84pt", arbitrary(u84, v84, Convert.ToInt32(nme)));
                output.Add("color_shift_adjacent_84pt", adjacent(u84, v84, 9, Convert.ToInt32(nme)));
            }

            return output;
        }

        private static float arbitrary(Dictionary<int, float> u, Dictionary<int, float> v, Int32 size)
        {
            float res = 0;
            for (int i = 1; i <= size; i++)
            {
                for (int j = i + 1; j <= size; j++)
                {
                    float d = (float)Math.Sqrt(power(u, i, j) + power(v, i, j));
                    if (d > res)
                        res = d;
                }
            }

            return res;
        }

        private static float adjacent(Dictionary<int, float> u, Dictionary<int, float> v, short cols, Int32 size)
        {
            float res = 0;
            for (int i = 1; i <= size; i++)
            {
                if (i + 1 <= size &&  i % cols != 0)
                {
                    float d = (float) Math.Sqrt(power(u, i, i + 1) + power(v, i, i + 1));
                    if (d > res)
                        res = d;
                }
                if (i + cols <= size)
                {
                    float d = (float) Math.Sqrt(power(u, i, i + cols) + power(v, i, i + cols));
                    if (d > res)
                        res = d;
                }
            }
            return res;
        }

        private static float power(Dictionary<int, float> u, int p1, int p2)
        {
            if (u.ContainsKey(p1) && u.ContainsKey(p2))
            {
                return (float)Math.Pow(u[p1] - u[p2], 2);
            }
            else
            {
                return 0;
            }
        }
    }
}
