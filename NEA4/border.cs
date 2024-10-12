
using NEA4;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace NEA4
{
    public class borderco
    {
        private static readonly Canvas canvas = MainWindow.canvas;
        private Vector s;
        private Vector e;
        private double radius;
        private Vector starttoball;
        private Vector starttoend;
        private fake[] fakeballs = new fake[2];
        private double howlong = 0;



        public borderco(double radius, Vector s, Vector e)
        {
            this.radius = radius;
            this.s = s;
            this.e = e;
            starttoend = e - s;
            this.howlong = starttoend.Length;
            starttoend.Normalize();
            drawline();
        }
        private void drawline()
        {
            fakeballs[0] = new fake(s, this.radius); //.makehitball();
            fakeballs[1] = new fake(e, this.radius);// end.makehitball();

            Vector n = new Vector();
            n.X = -(e.Y - s.Y);
            n.Y = (e.X - s.X);
            n.Normalize();
            n = n * radius;
            Line line1 = new Line(); thinline(line1, s + n, e + n);
            Line line2 = new Line(); thinline(line2, s - n, e - n);
        }
        public static void thinline(Line line, Vector s, Vector e)//made this public static just in case i wanted to recycle this in other classes.
        {
            Vector start = MainWindow.updategenpos(s.X, s.Y);
            Vector end = MainWindow.updategenpos(e.X, e.Y);
            line.X1 = start.X; line.Y1 = start.Y; line.X2 = end.X; line.Y2 = end.Y;
            line.StrokeThickness = 2;
            line.Stroke = new SolidColorBrush(Colors.Purple);
            canvas.Children.Add(line);
            Canvas.SetZIndex(line, 100);
           
        }
        public void collisions(Hitball ball)//is public because this function is called from other classes.
        {
            starttoball = ball.pos - s;
            double t = starttoball * starttoend;//projects position of ball onto line
            t = Math.Max(Math.Min(howlong, t), 0);//makes the value of t stay between legnth of border and 0;
            Vector projectedposition = s + starttoend * t;//finds projected position of argument ball on line. 
            Vector length = ball.pos - projectedposition;
            double size = length.Length;

            if (length.Length <= radius + ball.radius)
            {
                Hitball dummyball = new Hitball(projectedposition, radius);//creates a fake ball using a unique constructor only used by dummyballs. 

                dummyball.velocity = -ball.velocity;
                double overlap = size - ball.radius - dummyball.radius-0.3;
                ball.pos -= overlap * ((ball.pos - dummyball.pos) / size);//moves ball out of overlap.
                Hitball.after(ball, dummyball);
                ball.edgedamp();//slows ball down.
                
            }

        }

    }
}
