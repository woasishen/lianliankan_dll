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
        private readonly ComputeConnect _computeConnect;
        private readonly RectangleF[][] _dataRectf;
        private float _cellW;
        private float _cellH;
        private Point _oldClicked;
        private Point _curClicked;
        private PointF[] paths;

        public TestForm()
        {
            _dataRectf = new RectangleF[ROW][];
            for (int i = 0; i < ROW; i++)
            {
                _dataRectf[i] = new RectangleF[COLUMN];
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

            _computeConnect = new ComputeConnect(ComputeConnect.InitData(tempArr, ROW, COLUMN));

            _cellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            _cellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
        }

        private void ReInitCellRectfs()
        {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COLUMN; j++)
                {
                    _dataRectf[i][j] = new RectangleF(
                        W_EDGE + _cellW * j, H_EDGE + _cellH * i, _cellW, _cellH);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _cellW = (basePanel.Width - W_EDGE * 2) / COLUMN;
            _cellH = (basePanel.Height - H_EDGE * 2) / ROW;
            ReInitCellRectfs();
            Refresh();
        }

        private void basePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < ROW + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE, H_EDGE + _cellH * i,
                    basePanel.Width - W_EDGE, H_EDGE + _cellH * i);
            }
            for (int i = 0; i < COLUMN + 1; i++)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.Chartreuse),
                    W_EDGE + _cellW * i, H_EDGE,
                    W_EDGE + _cellW * i, basePanel.Height - H_EDGE);
            }

            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COLUMN; j++)
                {
                    if (_computeConnect.Datas[i][j] == 0)
                    {
                        continue;
                    }
                    e.Graphics.DrawString(
                        _computeConnect.Datas[i][j].ToString(),
                        Font,
                        new SolidBrush(Color.Red),
                        _dataRectf[i][j],
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                }
            }

            if (_oldClicked.IsEmpty)
            {
                if (!_curClicked.IsEmpty)
                {
                    e.Graphics.FillRectangle(
                        new SolidBrush(Color.FromArgb(50, Color.Blue)),
                        _dataRectf[_curClicked.X][_curClicked.Y]);
                }
                return;
            }

            if (paths != null)
            {
                e.Graphics.DrawLines(new Pen(Color.Black, 2), paths);
            }
        }

        private void basePanel_MouseClick(object sender, MouseEventArgs e)
        {
            _oldClicked = _curClicked;
            _curClicked = new Point(
                (int)((e.Y - H_EDGE) / _cellH),
                (int)((e.X - W_EDGE) / _cellW));
            var result = _computeConnect.ComputePath(_oldClicked, _curClicked);

            paths =result == null ? null : result.Select(s => new PointF(
                W_EDGE + s.Y*_cellW + _cellW/2,
                H_EDGE + s.X*_cellH + _cellH/2)).ToArray();
            Refresh();
        }

        private void CheckAndInvalidate()
        {
            if (!_curClicked.IsEmpty)
            {
                Invalidate(Rectangle.Round(_dataRectf[_curClicked.X][_curClicked.Y]), true);
            }
            if (!_oldClicked.IsEmpty)
            {
                Invalidate(Rectangle.Round(_dataRectf[_oldClicked.X][_oldClicked.Y]), true);
            }
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
