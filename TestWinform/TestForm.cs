using System;
using System.Drawing;
using System.Windows.Forms;
using CompteConnect;

namespace TestWinform
{
    public partial class TestForm : Form
    {
        public const float W_EDGE = 50.0f;
        public const float H_EDGE = 20.0f;

        private const int COUNT = 80;
        private const int ROW = 10 + 2;
        private const int COLUMN = 8 + 2;
        private readonly ComputeConnect computeConnect;
        private readonly RectangleF[][] dataRectf;
        public float CellW;
        public float CellH;
        private Position lastClicked;
        private readonly Path paths;

        public TestForm()
        {
            paths = new Path(this);
            dataRectf = new RectangleF[ROW][];
            for (int i = 0; i < ROW; i++)
            {
                dataRectf[i] = new RectangleF[COLUMN];
            }

            InitializeComponent();

            var tempArr = new uint[COUNT];
            for (uint i = 0; i < COUNT; i++)
            {
                tempArr[i] = i / 40 + 1;
            }

            var random = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < COUNT - 1; i++)
            {
                tempArr.ExChange(i, random.Next(i, COUNT));
            }

            computeConnect = new ComputeConnect(ComputeConnect.InitData(tempArr, ROW, COLUMN));

            CellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            CellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
            timer.Tick += (sender, args) => ClearPath();
            timer.Interval = 1000;
            timer.Start();
        }

        private void ReInitCellRectfs()
        {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COLUMN; j++)
                {
                    dataRectf[i][j] = new RectangleF(
                        W_EDGE + CellW * j, H_EDGE + CellH * i, CellW, CellH);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            CellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
            Refresh();
        }

        private void basePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < ROW + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE, H_EDGE + CellH * i,
                    basePanel.Width - W_EDGE, H_EDGE + CellH * i);
            }
            for (int i = 0; i < COLUMN + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE + CellW * i, H_EDGE,
                    W_EDGE + CellW * i, basePanel.Height - H_EDGE);
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

            if (paths.PointF != null)
            {
                e.Graphics.DrawLines(new Pen(Color.Black, 2), paths.PointF);
            }
        }

        private void basePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (paths.Position != null)
            {
                ClearPath();
                timer.Enabled = false;
                timer.Enabled = true;
                return;
            }

            var curClicked = new Position(
                (int)((e.Y - H_EDGE) / CellH),
                (int)((e.X - W_EDGE) / CellW));
            if (lastClicked == null)
            {
                lastClicked = curClicked;
                Refresh();
                return;
            }

            paths.Position = computeConnect.ComputePath(lastClicked, curClicked);
            lastClicked = null;
            Refresh();
        }

        private void ClearPath()
        {
            if (paths.Position != null)
            {
                var start = paths.Position[0];
                var end = paths.Position[paths.Position.Length - 1];
                computeConnect.Datas[start.Row][start.Column] = 0;
                computeConnect.Datas[end.Row][end.Column] = 0;
                paths.Position = null;
                Refresh();
            }
        }
    }

    public static class Extensions
    {
        public static void ExChange(this uint[] arr, int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }
            arr[index1] += arr[index2];
            arr[index2] = arr[index1] - arr[index2];
            arr[index1] = arr[index1] - arr[index2];
        }
    }
}
