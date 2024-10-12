using System;
using System.Collections.Generic;
using System.Linq;
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

using System.Diagnostics;
using System.Threading;
using System.Security.RightsManagement;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices.WindowsRuntime;
using NEA4;

namespace NEA4
{

    public abstract class ball
    {
        public static Engine engine;
        public Vector pos = new Vector(0, 0);
        public Vector startingpos = new Vector(0, 0);

        public int ballno;
        public int uniqueid;
        public int pointvalue;
        public static int R = 6;//radiius of ball
        public double cuetableratio = 0.6 / 3.57;


        public static double mass = 0.142;
        public static double drawtospeed = 1500 / 150;
        public static readonly Canvas canvas = MainWindow.canvas;
        protected Ellipse image = new Ellipse();
        public Hitball hit;
        public double radius;
        private static int totalamountballs = -1;
        public bool potted = false;

        public int type;


        public ball(Vector newpos, double radius)//ball deals with UI so has updated position.hitball deals with ingame
        {
            totalamountballs++;
            this.ballno = totalamountballs;
            startingpos = newpos;
            pos = newpos;

            this.radius = radius;
            updatep(pos);


            Canvas.SetZIndex(image, 3);
            image.Height = 2 * radius;
            image.Width = 2 * radius;
            canvas.Children.Add(image);

        }
        public ball()
        {

        }
        public void standardball(int newpointvalue)
        {
            radius = R;
            this.pointvalue = newpointvalue;
            hit = new Hitball(startingpos, this);
            Engine.addball(this, ballno);

        }
        public void applyshade(Color color)
        {
            image.Fill = new SolidColorBrush(color);
            image.Stroke = new SolidColorBrush(color);
        }
        public void updatep(Vector v)
        {

            pos = MainWindow.updategenpos(v.X, v.Y);
            MainWindow.addcanvaspos(pos.X - this.radius, pos.Y - this.radius, image);
            //hit.pos = v;

        }
        public void updatecanvasp()
        {
            pos = MainWindow.updategenpos(hit.pos.X, hit.pos.Y);
            MainWindow.addcanvaspos(pos.X - this.radius, pos.Y - this.radius, image);
        }
        public void updatecanvaspex(Vector v)//updates visual position and object position
        {

            hit.pos = v;
            updatecanvasp();

        }
        public static void takeengine(Engine eng)
        {
            engine = eng;
        }
        public void reset()
        {
            image.Visibility = Visibility.Visible;
            hit.pos = startingpos;
            updatecanvasp();
            hit.stophitball();
            Engine.ismoving[ballno] = false;
            potted = false;
        }
        public void makehitball()
        {
            Vector temp = new Vector();
            temp = pos - MainWindow.tablepos;
            hit = new Hitball(temp, this);
        }
        public virtual void pottedf()
        {
            engine.pottedballs_inpreviousframe.Add(this);
            this.potted=true;
            image.Visibility = Visibility.Collapsed;
            hit.pos= new Vector(double.NaN, double.NaN);
            hit.stophitball();
            Engine.ismoving[ballno] = false;

        }
        //make a scoring system for balls. Disappear the balls with a function. Scoring potentially in engine. Make the rules of what you can hit.
        //make a 2 user game:
        //make a bool for player system
        //make two total systems
        //should make it so i can choose one player or two player. bool so two player or alone. make a button so they can choose


    }
    public class fake : ball
    {
        public fake(Vector newpos, double radius) : base(newpos, radius)
        {
            applyshade(Colors.Purple);
            Canvas.SetZIndex(image, 100);
        }


    }


    public class red : ball
    {

        public red(Vector newpos, int newpointvalue, int ballno, double radius) : base(newpos, radius)
        {
            standardball(newpointvalue);
            applyshade(Colors.Red);
            //newpointvalue=newpointvalue
            type = 1;
        }
        public red()
        {

        }
        public override void pottedf()
        {
            base.pottedf();
            engine.pottedreds++;
            engine.wasredpotted = true;
        }

    }
    public class colour : ball
    {
        public colour(Vector newpos, int newpointvalue, int ballno, double radius) : base(newpos, radius)
        {
            standardball(newpointvalue);
            if (newpointvalue == 2) applyshade(Colors.Yellow);
            else if (newpointvalue == 3) applyshade(Colors.DarkGreen);
            else if (newpointvalue == 4) applyshade(Colors.SaddleBrown);
            else if (newpointvalue == 5) applyshade(Colors.Blue);
            else if (newpointvalue == 6) applyshade(Colors.HotPink);
            else if (newpointvalue == 7) applyshade(Colors.Black);
            type = 2;
        }
        public colour()
        {

        }
        public override void pottedf()
        {
            base.pottedf();
            engine.wascolourpotted = true;
        }
    }
    public class white : ball
    {
        public Line cue = new Line();
        public Line guide = new Line();
        private double maxdrawback;
        public white(Vector newpos, int newpointvalue, int ballno, double radius) : base(newpos, radius)
        {
            standardball(newpointvalue);
            maxdrawback = 800 * cuetableratio;
            applyshade(Colors.White);

            cue.X1 = pos.X; cue.Y1 = pos.Y; cue.X2 = pos.X; cue.Y2 = pos.Y;
            cue.StrokeThickness = 2;
            cue.Stroke = new SolidColorBrush(Colors.Black);
            Canvas.SetZIndex(cue, 3);
            canvas.Children.Add(cue);

            guide.X1 = pos.X; guide.Y1 = pos.Y; guide.X2 = pos.X; guide.Y2 = pos.Y;
            guide.StrokeThickness = 1;
            guide.Stroke = new SolidColorBrush(Colors.Red);
            Canvas.SetZIndex(guide, 3);
            canvas.Children.Add(guide);
            type=0;
        }
        public white()
        {

        }
        private Vector difference = new Vector();
        private Vector Normaliseddifference = new Vector();
        public void gencue(Vector mouse)
        {
            difference = pos - mouse;
            Normaliseddifference = difference;
            Normaliseddifference.Normalize();

            if (difference.Length > maxdrawback)
            {
                difference = Normaliseddifference * maxdrawback;
                mouse = pos - difference;

                cue.X2 = mouse.X;
                cue.Y2 = mouse.Y;
            }
            else
            {
                cue.X2 = mouse.X;
                cue.Y2 = mouse.Y;
            }
            Normaliseddifference *= engine.cwidth * 0.8;
            guide.X1 = pos.X; guide.Y1 = pos.Y; guide.X2 = pos.X + Normaliseddifference.X; guide.Y2 = pos.Y + Normaliseddifference.Y;

        }
   
        public void applycueballspeed()
        {

            this.hit.applyvelocity(drawtospeed * (difference));
            Engine.ismoving[this.ballno] = true;
        }

        public override void pottedf()
        {
            base.pottedf();
            engine.wascueballpotted=true;
        }

    }

}
