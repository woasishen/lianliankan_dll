using System.Drawing;
using System.Linq;
using CompteConnect;

namespace TestWinform
{
    public class Path
    {
        private TestForm baseForm;
        private Position[] position;

        public Path(TestForm form)
        {
            baseForm = form;
        }

        public PointF[] PointF { private set; get; }

        public Position[] Position
        {
            get { return position; }
            set
            {
                position = value;
                PointF = position == null ? null : position.Select(s => new PointF(
                TestForm.W_EDGE + s.Column * baseForm.CellW + baseForm.CellW / 2,
                TestForm.H_EDGE + s.Row * baseForm.CellH + baseForm.CellH / 2)).ToArray();
            }
        }
    }
}