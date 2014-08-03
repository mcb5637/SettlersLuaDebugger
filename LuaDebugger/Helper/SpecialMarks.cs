using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    public class BreakMark : Bookmark
    {
        public BreakMark(IDocument d, TextLocation l) : base(d, l) { }

        public override void Draw(IconBarMargin margin, Graphics g, Point p)
        {
            BookmarkManager bm = margin.TextArea.Document.BookmarkManager;
            Bookmark next = bm.GetNextMark(this.LineNumber,
                (Bookmark bk) => { return bk.LineNumber == this.LineNumber && this != bk; });

            if (next == null)
                margin.DrawBreakpoint(g, p.Y, IsEnabled, true);
            else
                MyDrawBreakpoint(g, p.Y, IsEnabled, margin); //arrow active, draw opaque breakpoint
        }
        protected void MyDrawBreakpoint(Graphics g, int y, bool isEnabled, IconBarMargin margin)
        {
            int diameter = Math.Min(margin.Size.Width - 2, margin.TextArea.TextView.FontHeight);
            Rectangle rect = new Rectangle(1,
                                           y + (margin.TextArea.TextView.FontHeight - diameter) / 2,
                                           diameter,
                                           diameter);


            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (PathGradientBrush pthGrBrush = new PathGradientBrush(path))
                {
                    pthGrBrush.CenterPoint = new PointF(rect.Left + rect.Width / 3, rect.Top + rect.Height / 3);
                    pthGrBrush.CenterColor = Color.FromArgb(100, Color.MistyRose);
                    pthGrBrush.SurroundColors = new Color[] { Color.FromArgb(100, Color.Firebrick) };


                    if (isEnabled)
                    {
                        g.FillEllipse(pthGrBrush, rect);
                    }
                    else
                    {
                        g.FillEllipse(SystemBrushes.Control, rect);
                        using (Pen pen = new Pen(pthGrBrush))
                        {
                            g.DrawEllipse(pen, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                        }
                    }
                }
            }
        }
    }

    public class ArrowMark : Bookmark
    {
        public ArrowMark(IDocument d, TextLocation l) : base(d, l) { }

        public override void Draw(IconBarMargin margin, Graphics g, Point p)
        {
            if (this.IsEnabled)
                margin.DrawArrow(g, p.Y);
        }

        public override bool CanToggle
        {
            get { return false; }
        }

        public int NormalLineNr
        {
            get { return this.LineNumber + 1; }
            set { this.Location = new TextLocation(0, value - 1); }
        }

    }
}
