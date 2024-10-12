using NEA4;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NEA4
{
    public abstract class pocket
    {
        protected List<int> ballno = new List<int>();
        protected const double pocketradius=12.5;
        protected Vector pocketcentre;
        protected ball pocketball;
        protected borderco left;
        protected borderco right;

        public virtual void pocketlogic(Hitball ball)
        {
            
            if (checkballinhole(ball))
            {
                ballno.Add(ball.ballno);
                ball.thisball.pottedf();
            }
            else
            {
                checkpocketborder(ball);
            }
           
           

        }
        
        
        public void checkpocketborder(Hitball ball)
        {
            left.collisions(ball);right.collisions(ball);
        }
        public virtual bool checkballinhole(Hitball ball)
        {
             Vector vector = ball.thisball.pos - pocketball.pos;
             if(vector.Length <= pocketradius)
             {
                 return true;
             }
             else
             {
                 return false;
             }
        }



        public struct sectioncoordinates
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public sectioncoordinates sectionc = new sectioncoordinates();

    }
    public class middle_pocket : pocket
    {
        
        protected const double cushionheight = 13.4;
        protected const double cushionopening = 40;
        protected const double cushionaccepting = 25;
        protected double angleofcushion = 0.6871;
        //public double posx=cushion
        public middle_pocket() //the y corrdinate since all middle pockets have same x coordintate
        {
            //this.yp = yp;
           
            //Engine.pocket[sectionc.X, sectionc.Y] = this;
        }


    }
    public class top_middle : middle_pocket
    {
        public top_middle()
        {
            sectionc.X = 1; sectionc.Y = 0;
            this.pocketcentre.X = MainWindow.cwidth / 2; this.pocketcentre.Y = -pocketradius + 1;
            pocketball = new fake(pocketcentre, pocketradius);
            //pocketball.makehitball();
            Vector start = new Vector(MainWindow.cwidth / 2 - (cushionopening / 2), 0);
            Vector end = new Vector(MainWindow.cwidth / 2 - (cushionaccepting / 2), -cushionheight);
            left = new borderco(0.2, start, end);
            start.X = MainWindow.cwidth / 2 + (cushionopening / 2); start.Y = 0;
            end.X= MainWindow.cwidth /2+ (cushionaccepting / 2); end.Y = -cushionheight;
            right =new borderco(0.2, start, end);



        }
    }
    public class bottom_middle : middle_pocket
    {
        public bottom_middle()
        {
            sectionc.X = 1; sectionc.Y = 0;
            this.pocketcentre.X = MainWindow.cwidth / 2; this.pocketcentre.Y = MainWindow.cheight+pocketradius - 1;
            pocketball = new fake(pocketcentre, pocketradius);
            //pocketball.makehitball();
            Vector start = new Vector(MainWindow.cwidth / 2 - (cushionopening / 2), MainWindow.cheight);
            Vector end = new Vector(MainWindow.cwidth / 2 - (cushionaccepting / 2),MainWindow.cheight +cushionheight);
            left = new borderco(0.2, start, end);
            start.X = MainWindow.cwidth / 2 + (cushionopening / 2); start.Y = MainWindow.cheight;
            end.X = MainWindow.cwidth / 2 + (cushionaccepting / 2); end.Y = MainWindow.cheight+cushionheight;
            right = new borderco(0.2, start, end);

        }
    }

    
    public class top_left : pocket
    { 
        public top_left()//visual top left not coordinate wise
        {
            sectionc.X = 0; sectionc.Y=0;
            this.pocketcentre.X = -6;this.pocketcentre.Y = -6;
            pocketball = new fake(pocketcentre, pocketradius);
            
            Vector start = new Vector(0, 17);
            Vector end = new Vector(-13.4, 4);
            left = new borderco(0.2, start, end);
            
            start.X =17; start.Y = 0;
            end.X = 4; end.Y = -13.4;
            right = new borderco(0.2, start, end);
            
        }
    }
    public class top_right : pocket 
    {
        public top_right()
        {
            sectionc.X = 2; sectionc.Y=0;
            this.pocketcentre.X = MainWindow.cwidth + 6;this.pocketcentre.Y = -6;
            pocketball = new fake(pocketcentre, pocketradius);

            Vector start = new Vector(MainWindow.cwidth, 17);
            Vector end = new Vector(MainWindow.cwidth+13.4, 4);
            left = new borderco(0.2, start, end);

            start.X = MainWindow.cwidth -17; start.Y = 0;
            end.X = MainWindow.cwidth- 4; end.Y = -13.4;
            right = new borderco(0.2, start, end);
        }
    }
    public class bottom_right : pocket
    {
        public bottom_right() 
        {
            sectionc.X = 2; sectionc.Y = 1;
            this.pocketcentre.X = MainWindow.cwidth + 6; this.pocketcentre.Y = MainWindow.cheight+6;
            pocketball = new fake(pocketcentre, pocketradius);

            Vector start = new Vector(MainWindow.cwidth, MainWindow.cheight- 17);
            Vector end = new Vector(MainWindow.cwidth + 13.4, MainWindow.cheight- 4);
            left = new borderco(0.2, start, end);

            start.X = MainWindow.cwidth - 17; start.Y = MainWindow.cheight;
            end.X = MainWindow.cwidth - 4; end.Y = MainWindow.cheight + 13.4;
            right = new borderco(0.2, start, end);

        }
    }
    public class bottom_left : pocket
    {
        public bottom_left()
        {
            sectionc.X = 0; sectionc.Y = 1;
            this.pocketcentre.X = -6; this.pocketcentre.Y =MainWindow.cheight +6;
            pocketball = new fake(pocketcentre, pocketradius);

            Vector start = new Vector(0, MainWindow.cheight-17);
            Vector end = new Vector(-13.4, MainWindow.cheight- 4);
            left = new borderco(0.2, start, end);

            start.X = 17; start.Y = MainWindow.cheight;
            end.X = 4; end.Y = MainWindow.cheight + 13.4;
            right = new borderco(0.2, start, end);

        }
    }




}
