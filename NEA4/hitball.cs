using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Security.RightsManagement;
using System.Windows.Media.Media3D;
using System;
using System.Diagnostics.Eventing.Reader;
using NEA4;
using System.Runtime.InteropServices;

namespace NEA4
{
    public class Hitball
    {
        public ball thisball;

        public Vector pos;//must be public. 
        public Vector velocity;//must be public due to logic in border class. 
        private Vector acceleration;

        public int ballno;//asts as a refernce to acess other balls

    
        private const double a = 50;
        public static double bounceratio = 0.85;
        public double radius;
        public static double time = 0;//shared across all hitballs. 

     

        public struct sectioncoordinates
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        public sectioncoordinates sectionc = new sectioncoordinates() { X = 0, Y = 0 };


        public Hitball(Vector pos, ball ball)
        {
            thisball = ball;
            this.pos = pos;
            this.ballno = thisball.ballno;
            this.radius = thisball.radius;
            this.velocity = new Vector(0, 0);
        }
        public Hitball(Vector pos, double radius)//constructor for fakeballs
        {
            this.radius = radius;
            this.pos = pos;
            this.ballno = 100;

        }
        public void stophitball()
        {
            this.velocity = new Vector(0, 0);
            this.acceleration = new Vector(0, 0);
        }
        public static bool islengthbetween(Hitball ball1, Hitball ball2)
        {
            Vector difference = (ball1.pos) - (ball2.pos);
            if (difference.Length < ball1.radius + ball2.radius)
            {
               
                return true;
            }
            else return false;
        }
        public static bool checkhit(Hitball ball1, Hitball ball2, double t1)//function makes sure balls are colliding then moves them to the correct point of intersection.
        {
            if (t1 < 0 && Math.Abs(t1) < time)
            {
                Vector tempb1 = new Vector();
                Vector tempb2 = new Vector();
                var vfinal1 = ball1.velocity * t1;
                var vfinal2 = ball2.velocity * t1;
                tempb1 = ball1.pos + vfinal1;
                tempb2 = ball2.pos + vfinal2;
                Vector diff = tempb1 - tempb2;
                double length = diff.Length;
                if (length <= ball1.radius + ball2.radius + 2)
                {
                    ball1.pos += ball1.velocity * t1;
                    ball2.pos += ball2.velocity * t1;
                    if (!ball.engine.hascollisionoccured&&(ball1.ballno==0||ball2.ballno==0))
                    {
                        ball.engine.hascollisionoccured= true;
                        if (ball1.ballno != 0) ball.engine.collisionball = ball1.thisball;
                        else ball.engine.collisionball = ball2.thisball;
                    }
                    return true;
                }
                else return true;

            }
            else
            {
                if( islengthbetween(ball1, ball2))
                {
                    if (!ball.engine.hascollisionoccured && (ball1.ballno == 0 || ball2.ballno == 0))
                    {
                        ball.engine.hascollisionoccured = true;
                        if (ball1.ballno != 0) ball.engine.collisionball = ball1.thisball;
                        else ball.engine.collisionball = ball2.thisball;
                    }
                    return true;
                }
                return false;
            }


        }

        public static (double, double) intersectionpoint(Hitball ball1, Hitball ball2)
        {

            Vector p = ball1.pos - ball2.pos;
            Vector pΔ = ball1.velocity - ball2.velocity;

            double a = pΔ.X * pΔ.X + pΔ.Y * pΔ.Y;
            double b = 2 * (p.X * pΔ.X + p.Y * pΔ.Y);
            double c = p.X * p.X + p.Y * p.Y - Math.Pow((ball1.radius + ball2.radius), 2);

            double discriminant = b * b - 4 * a * c;

            // Check for small values of a
            if (Math.Abs(a) < 1e-8)
            {
                return (-1, 0);
            }

            if (discriminant >= 0)
            {
                double sqrtDiscriminant = Math.Sqrt(discriminant);
                double t1 = (-b - sqrtDiscriminant) / (2 * a);

                return (discriminant, t1);
            }
            else
            {
                return (-1, 0);
            }

        }
        public void edgedamp()
        {
            this.velocity *= bounceratio;
        }
   
        
        public static void after(Hitball circle1, Hitball circle2)
        {

            Vector n = circle1.pos - circle2.pos;

            n.Normalize();

            double a1 = circle1.velocity * n;
            double a2 = circle2.velocity * n;


            double impulse =  (a1 - a2);


            Vector v1 = circle1.velocity - impulse * n;


            Vector v2 = circle2.velocity + impulse * n;

            circle1.velocity = v1;
            circle2.velocity = v2;
            int balln1 = circle1.ballno;
            int balln2 = circle2.ballno;
            ismoving(circle1);
            ismoving(circle2);

        }
        public static void ismoving(Hitball sphere)
        {
            try { Engine.ismoving[sphere.ballno] = true; }
            catch { }

        }
        public void applyvelocity(Vector velocity)
        {
            this.velocity = velocity;
            ismoving(this);
        }

        public Vector normalized = new Vector(0, 0);
        public void updatev(double elapsedtime)
        {
            
            int x, y;
            Vector tempqq1 = this.velocity;
            Vector tempqq2 = this.pos;
            int tempcx = sectionc.X;int tempcy = sectionc.Y;
            try
            {
                if (elapsedtime > 0.0001)
                {
                    this.pos += (this.velocity * elapsedtime);
                    if (pos.X == double.NaN || pos.Y == double.NaN)
                    {
                        this.pos = tempqq2;
                        this.velocity = tempqq1;
                    }
                    else
                    {
                        var v = this.pos;

                        x = (int)Math.Max(0, Math.Min(Math.Floor(v.X / (MainWindow.cwidth / 3)), 2.0));
                        y = (int)Math.Max(0, Math.Min(Math.Floor(v.Y / (MainWindow.cheight / 2)), 1.0));
                        sectionc.X = x; sectionc.Y = y;
                    }

                }
            }
            catch 
            { 
            double time5=elapsedtime;
            }
           
        }
        public static void applyfriction(Hitball sphere)
        {
            Vector dsv = sphere.velocity;
            Vector normalized = sphere.velocity;

            normalized.Normalize();
            sphere.acceleration = -normalized * a;
            dsv += (sphere.acceleration * time);
            if (dsv.Length < 0.2)
            {
                sphere.velocity.X=0;sphere.velocity.Y=0;
                Engine.ismoving[sphere.ballno] = false;

            }
            else sphere.velocity += (sphere.acceleration * time);
        }
        public static void moveballawayfromstatic(Hitball ball)
        {
            int counter;
            int ballno4= ball.ballno;
            do
            {
                counter = 0;
                for (int i = 0; i < Engine.balls.Length; i++)
                {
                    if (i != ballno4)
                    {
                        Vector length = ball.pos - Engine.balls[i].hit.pos;
                        if (length.Length < ball.radius + Engine.balls[i].radius)
                        {
                            double overlap = length.Length - ball.radius - Engine.balls[i].hit.radius - 0.3;
                            ball.pos -= overlap * (length / length.Length);
                            counter++;
                        }
                    }
                }
            } while (counter != 0);//to make sure ball is not overlapping anything
        }
        public static double metretopixel(double metre)
        {
            double pixels = 224.1 * metre;
            return pixels;
        }
        public static double pixeltometre(double pixel)
        {
            double metres = pixel / 224.1;
            return metres;
        }


    }
}