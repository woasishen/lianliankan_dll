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
        public Position[] ComputePath(Position startP, Position endP)
        {
            if (startP == endP
                || Datas[startP.Row][startP.Column] == 0
                || Datas[startP.Row][startP.Column] != Datas[endP.Row][endP.Column])
            {
                return null;
            }
            if (CanDrawLine(startP, endP))
            {
                return new[] { startP, endP };
            }

            int rStart, rEnd, cStart, cEnd;
            GetReachableIndex(startP, endP, out rStart, out rEnd, out cStart, out cEnd);

            var result = new[] { startP, endP };

            return ComputePath(startP.Column, endP.Column, rStart, rEnd, ref result, (y, x) => new Position(x, y))
                || ComputePath(startP.Row, endP.Row, cStart, cEnd, ref result, (x, y) => new Position(x, y))
                ? result.Distinct().ToArray()
                : null;
        }

        private bool ComputePath(
            int same1, int same2,
            int start, int end,
            ref Position[] result,
            Func<int, int, Position> getPositionFunc)
        {
            for (var i = start; i <= end; i++)
            {
                var tempS = getPositionFunc(same1, i);
                var tempE = getPositionFunc(same2, i);
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
            Position startP, Position endP,
            out int xStart, out int xEnd,
            out int yStart, out int yEnd)
        {
            int sRs, sRe, sCs, sCe;
            int eRs, eRe, eCs, eCe;

            GetReachableIndex(startP, out sRs, out sRe, out sCs, out sCe);
            GetReachableIndex(endP, out eRs, out eRe, out eCs, out eCe);

            xStart = Math.Max(sRs, eRs);
            xEnd = Math.Min(sRe, eRe);

            yStart = Math.Max(sCs, eCs);
            yEnd = Math.Min(sCe, eCe);
        }

        private void GetReachableIndex(
            Position p,
            out int xStart, out int xEnd,
            out int yStart, out int yEnd)
        {
            GetReachableIndex(p.Row, p.Column, Datas[0].Length, out yStart, out yEnd, (x, y) => Datas[x][y]);
            GetReachableIndex(p.Column, p.Row, Datas.Length, out xStart, out xEnd, (y, x) => Datas[x][y]);
        }

        private void GetReachableIndex(
            int same, int dif, int length,
            out int start, out int end,
            Func<int, int, uint> getDataFunc)
        {
            var temp = dif;
            while (temp - 1 >= 0 && getDataFunc(same, temp - 1) == 0)
            {
                temp--;
            }
            start = temp;

            temp = dif;
            while (temp + 1 < length && getDataFunc(same, temp + 1) == 0)
            {
                temp++;
            }
            end = temp;
        }

        private bool CanDrawLine(Position p1, Position p2)
        {
            return (p1.Row == p2.Row && CanDrawLine(p1.Row, p1.Column, p2.Column, (x, y) => Datas[x][y]))
                || (p1.Column == p2.Column && CanDrawLine(p1.Column, p1.Row, p2.Row, (y, x) => Datas[x][y]));
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
