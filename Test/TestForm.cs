using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CompteConnect;

namespace Test
{
    public partial class TestForm : Form
    {
        private const int COUNT = 80;
        private const int ROW = 10 + 2;
        private const int COLUMN = 8 + 2;
        private const float W_EDGE = 50.0f;
        private const float H_EDGE = 20.0f;
        private readonly ComputeConnect computeConnect;
        private readonly RectangleF[][] dataRectf;
        private float cellW;
        private float cellH;
        private Position lastClicked;
        private PointF[] paths;

        public TestForm()
        {
            dataRectf = new RectangleF[ROW][];
            for (int i = 0; i < ROW; i++)
            {
                dataRectf[i] = new RectangleF[COLUMN];
            }

            InitializeComponent();

            var tempArr = new uint[COUNT];
            for (uint i = 0; i < COUNT; i++)
            {
                tempArr[i] = i / 4 + 1;
            }

            var random = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < COUNT - 1; i++)
            {
                //tempArr.ExChange(i, random.Next(i, COUNT));
            }

            computeConnect = new ComputeConnect(ComputeConnect.InitData(tempArr, ROW, COLUMN));

            cellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            cellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
        }

        private void ReInitCellRectfs()
        {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COLUMN; j++)
                {
                    dataRectf[i][j] = new RectangleF(
                        W_EDGE + cellW * j, H_EDGE + cellH * i, cellW, cellH);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            cellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            cellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
            Refresh();
        }

        private void basePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < ROW + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE, H_EDGE + cellH * i,
                    basePanel.Width - W_EDGE, H_EDGE + cellH * i);
            }
            for (int i = 0; i < COLUMN + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE + cellW * i, H_EDGE,
                    W_EDGE + cellW * i, basePanel.Height - H_EDGE);
            }

            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COLUMN; j++)
                {
                    if (computeConnect.Datas[i][j] == 0)
                    {
                        continue;
                    }
                    e.Graphics.DrawString(
                        computeConnect.Datas[i][j].ToString(),
                        Font,
                        new SolidBrush(Color.Red),
                        dataRectf[i][j],
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                }
            }

            if (lastClicked != null)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.FromArgb(50, Color.Blue)),
                    dataRectf[lastClicked.Row][lastClicked.Column]);
                return;
            }

            if (paths != null)
            {
                e.Graphics.DrawLines(new Pen(Color.Black, 2), paths);
            }
        }

        private void basePanel_MouseClick(object sender, MouseEventArgs e)
        {
            var curClicked = new Position(
                (int)((e.Y - H_EDGE) / cellH),
                (int)((e.X - W_EDGE) / cellW));
            if (lastClicked == null)
            {
                lastClicked = curClicked;
                Refresh();
                return;
            }
            
            var result = computeConnect.ComputePath(lastClicked, curClicked);
            lastClicked = null;
            paths = result == null ? null : result.Select(s => new PointF(
                W_EDGE + s.Column*cellW + cellW/2,
                H_EDGE + s.Row*cellH + cellH/2)).ToArray();
            Refresh();
        }
    }

    public static class Extensions
    {
        public static void ExChange(this uint[] arr, int index1, int index2)
        {
            arr[index1] += arr[index2];
            arr[index2] = arr[index1] - arr[index2];
            arr[index1] = arr[index1] - arr[index2];
        }
    }
}
