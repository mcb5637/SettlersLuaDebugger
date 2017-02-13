using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public partial class JoyStick : UserControl
    {
        PointF JoyStickPos;
        PointF JoyStickPosOrig;
        float JoyStickRadius = 20;
        public enum Action { Forward = 3, Backward = 2, Rotate = 1, Leftward = -4, Rightward = -3, Upward = -2, Downward = -1, None = 0 };
        public Action CurrentAction = Action.None;
        Action MouseOverAction = Action.None;
        Pen connectionPen = new Pen(Color.FromArgb(150, 150, 150, 150), 5);
        public float Pitch { protected set; get; }
        public float Yaw { protected set; get; }
        public float Speed
        {
            get
            {
                switch (CurrentAction)
                {
                    case Action.Forward:
                        return 1.0f;
                    case Action.Backward:
                        return -1.0f;
                    default:
                        return 0.00f;
                }
            }
        }
        const int arrowWidth = 30;
        const int arrowHeight = 100;
        public JoyStick()
        {
            InitializeComponent();
            JoyStickPos = new PointF(pbJoyStick.Width / 2, pbJoyStick.Height / 2);
            JoyStickPosOrig = JoyStickPos;
            connectionPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        private void pbJoyStick_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            foreach (Action action in new Action[] { Action.Leftward, Action.Rightward, Action.Upward, Action.Downward })
            {
                PointF[] polygon = GetDirectionArrow(action);
                if (CurrentAction == action)
                    g.FillPolygon(new SolidBrush(Color.FromArgb(150, 150, 150, 150)), polygon);
                else if (MouseOverAction == action)
                    g.FillPolygon(new SolidBrush(Color.FromArgb(200, 200, 200, 200)), polygon);
                g.DrawPolygon(Pens.Black, polygon);
            }

            if (CurrentAction > 0)
            {
                g.DrawEllipse(Pens.Red, JoyStickPosOrig.X - JoyStickRadius - 5, JoyStickPosOrig.Y - JoyStickRadius - 5, 2 * JoyStickRadius + 10, 2 * JoyStickRadius + 10);

                g.DrawLine(connectionPen, JoyStickPos, JoyStickPosOrig);
                g.FillEllipse(Brushes.Black, JoyStickPos.X - JoyStickRadius, JoyStickPos.Y - JoyStickRadius, 2 * JoyStickRadius, 2 * JoyStickRadius);
            }
        }

        private PointF[] GetDirectionArrow(Action direction)
        {

            int pbw = pbJoyStick.Width;
            int pbh = pbJoyStick.Height;
            int ah2 = arrowHeight / 2;
            int pbw2 = pbJoyStick.Width / 2;
            int pbh2 = pbJoyStick.Height / 2;

            switch (direction)
            {
                case Action.Leftward:
                    return new PointF[] { new Point(arrowWidth + 5, pbh2 - ah2), new Point(5, pbh2), new Point(arrowWidth + 5, pbh2 + ah2) };
                case Action.Rightward:
                    return new PointF[] { new Point(pbw - arrowWidth - 5, pbh2 - ah2), new Point(pbw - 5, pbh2), new Point(pbw - arrowWidth - 5, pbh2 + ah2) };
                case Action.Upward:
                    return new PointF[] { new Point(pbw2 - ah2, arrowWidth + 5), new Point(pbw2, 5), new Point(pbw2 + ah2, arrowWidth + 5) };
                case Action.Downward:
                    return new PointF[] { new Point(pbw2 - ah2, pbh - arrowWidth - 5), new Point(pbw2, pbh - 5), new Point(pbw2 + ah2, pbh - arrowWidth - 5) };
                default:
                    throw new NotImplementedException();
            }
        }

        private void pbJoyStick_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (MouseOverAction == Action.None)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        CurrentAction = Action.Forward;
                        break;
                    case MouseButtons.Right:
                        CurrentAction = Action.Backward;
                        break;
                    case MouseButtons.Middle:
                        CurrentAction = Action.Rotate;
                        break;
                }
                JoyStickPos = new PointF(e.X, e.Y);
                JoyStickPosOrig = JoyStickPos;
            }
            else
                CurrentAction = MouseOverAction;
            pbJoyStick.Invalidate();
        }

        private void pbJoyStick_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            CurrentAction = Action.None;
            this.Pitch = 0;
            this.Yaw = 0;
            pbJoyStick.Invalidate();
        }

        private void pbJoyStick_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (CurrentAction <= Action.None)
            {
                Action oldMouseOverAction = MouseOverAction;
                foreach (Action action in new Action[] { Action.Leftward, Action.Rightward, Action.Upward, Action.Downward })
                {
                    if (IsPointInPolygon(GetDirectionArrow(action), new PointF(e.X, e.Y)))
                    {
                        MouseOverAction = action;
                        break;
                    }
                    else
                        MouseOverAction = Action.None;
                }
                if (oldMouseOverAction != MouseOverAction)
                    pbJoyStick.Invalidate();
            }
            if (CurrentAction > Action.None)
            {
                JoyStickPos = new PointF(e.X, e.Y);
                Recalculate();
                pbJoyStick.Invalidate();
            }
        }

        void Recalculate()
        {
            this.Pitch = Math.Min(Math.Max((JoyStickPosOrig.Y - JoyStickPos.Y) / (8 * JoyStickRadius), -1), 1);
            this.Yaw = Math.Min(Math.Max((JoyStickPos.X - JoyStickPosOrig.X) / (8 * JoyStickRadius), -1), 1);
        }

       private bool IsPointInPolygon(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        private void pbJoyStick_Resize(object sender, EventArgs e)
        {
            pbJoyStick.Invalidate();
        }
    }
}
