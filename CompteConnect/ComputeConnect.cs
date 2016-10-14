using System;
using System.Drawing;
using System.Linq;

namespace CompteConnect
{
    public class ComputeConnect
    {
        public static uint[][] InitData(uint[] data, int row, int column)
        {
            var result = new uint[row][];
            for (var i = 0; i < row; i++)
            {
                result[i] = new uint[column];
                if (i == 0 || i == row - 1)
                {
                    for (var j = 0; j < column; j++)
                    {
                        result[i][j] = 0;
                    }
                    continue;
                }

                for (int j = 0; j < column; j++)
                {
                    if (j == 0 || j == column - 1)
                    {
                        result[i][j] = 0;
                        continue;
                    }
                    result[i][j] = data[(i - 1)*(column - 2) + j - 1];
                }
            }
            return result;
        }

        public uint[][] Datas { private set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="datas">连连看相关 uint 数据，默认4边边界已设置为0</param>
        public ComputeConnect(uint[][] datas)
        {
            Datas = datas;
        }


        /// <summary>
        /// 计算两点连接路径
        /// </summary>
        /// <param name="startP">开始点</param>
        /// <param name="endP">终止点</param>
        /// <returns>路径:包括开始点、中间点、终止点（2-4个），若不能连接为null</returns>
        public Point[] ComputePath(Point startP, Point endP)
        {
            if (startP == endP
                || Datas[startP.X][startP.Y] == 0
                || Datas[startP.X][startP.Y] != Datas[endP.X][endP.Y])
            {
                return null;
            }
            if (CanDrawLine(startP, endP))
            {
                return new[] { startP, endP };
            }

            int xStart, xEnd, yStart, yEnd;
            GetReachableIndex(startP, endP, out xStart, out xEnd, out yStart, out yEnd);

            var result = new[] { startP, endP };

            return ComputePath(startP.Y, xStart, xEnd, ref result, (y, x) => new Point(x, y))
                || ComputePath(startP.X, yStart, yEnd, ref result, (x, y) => new Point(x, y))
                ? result.Distinct().ToArray()
                : null;
        }

        private bool ComputePath(
            int same, int start, int end,
            ref Point[] result,
            Func<int, int, Point> getPointFunc)
        {
            for (var i = start; i < end; i++)
            {
                var tempS = getPointFunc(same, i);
                var tempE = getPointFunc(same, i);
                if (!CanDrawLine(tempS, tempE))
                {
                    continue;
                }
                result = new[] { result[0], tempS, tempE, result[1] };
                return true;
            }
            return false;
        }

        private void GetReachableIndex(
            Point startP, Point endP,
            out int xStart, out int xEnd,
            out int yStart, out int yEnd)
        {
            int sXs, sXe, sYs, sYe;
            int eXs, eXe, eYs, eYe;

            GetReachableIndex(startP, out sXs, out sXe, out sYs, out sYe);
            GetReachableIndex(endP, out eXs, out eXe, out eYs, out eYe);

            xStart = Math.Max(sXs, eXs);
            xEnd = Math.Min(sXe, eXe);

            yStart = Math.Max(sYs, eYs);
            yEnd = Math.Min(sYe, eYe);
        }

        private void GetReachableIndex(
            Point p,
            out int xStart, out int xEnd,
            out int yStart, out int yEnd)
        {
            GetReachableIndex(p.X, p.Y, Datas[0].Length, out yStart, out yEnd, (x, y) => Datas[x][y]);
            GetReachableIndex(p.Y, p.X, Datas.Length, out xStart, out xEnd, (y, x) => Datas[x][y]);
        }

        private void GetReachableIndex(
            int same, int dif, int length,
            out int start, out int end,
            Func<int, int, uint> getDataFunc)
        {
            var temp = dif;
            while (temp - 1 > 0 && getDataFunc(same, temp - 1) == 0)
            {
                temp--;
            }
            start = temp;

            temp = dif;
            while (temp + 1 < length && getDataFunc(same, temp - 1) == 0)
            {
                temp++;
            }
            end = temp;
        }

        private bool CanDrawLine(Point p1, Point p2)
        {
            return (p1.X == p2.X && CanDrawLine(p1.X, p1.Y, p2.Y, (x, y) => Datas[x][y]))
                || (p1.Y == p2.Y && CanDrawLine(p1.Y, p1.X, p2.X, (y, x) => Datas[x][y]));
        }

        private bool CanDrawLine(int same, int dif1, int dif2, Func<int, int, uint> func)
        {
            var start = Math.Min(dif1, dif2);
            var end = Math.Max(dif1, dif2);
            for (var i = start + 1; i < end; i++)
            {
                if (func(same, i) > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
