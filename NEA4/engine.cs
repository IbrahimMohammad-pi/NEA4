using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Net.Sockets;
using NEA4;
using System.Windows.Controls;
using System.Windows.Media.Media3D.Converters;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Media;


namespace NEA4
{
    public class Engine
    {
        public bool moving;
        public static ball[] balls = new ball[22];
        public static white white;
        public static bool[] ismoving = new bool[22];
    
        private pocket[,] pockets = new pocket[3, 2];
        private borderco[]borders = new borderco[6];

        public double cwidth = 800;
        public double cheight = 400;
        public double R;
        protected const double cushionopening = 40;

        private TextBlock playerindicator = new TextBlock();
        private TextBlock player1score = new TextBlock();
        private TextBlock player2score = new TextBlock();
        private TextBlock foulindicator = new TextBlock();

        public struct ballpair
        {
            public int x;

            public int y;

            public double t;
            
        }
        private bool gamewon = false;
        private int whichplayerwon;
        private bool player = true;
        public bool wasredpotted;
        public bool wascolourpotted;
        public bool wascueballpotted;
        public bool hascollisionoccured = false;
        public int pottedreds = 0;
        public List<ball> pottedballs_inpreviousframe = new List<ball>();
        private int[] playerscore = new int[2];
        private bool musthitred = true;
        private bool musthitcolour = false;
        private bool musthitlowestcolour = false;
        private int lowestcolour = 0;
        public ball collisionball;
        private List<ball> resetcolour = new List<ball>();

        public Engine()
        {
            player=true;
            ball.takeengine(this);
            R = 6;
            MainWindow.tablepos.X = 100; MainWindow.tablepos.Y=100;
            moving = false;
            createballs();
            createpockets();
            createborders();
            createtextboxes();

        }

        private borderco testborder;

        private void createborders()
        {
            Vector start = new Vector(0, 0);
            Vector end = new Vector(0, 0);

            start.X = 17; start.Y=0;
            end.X = (cwidth / 2) - (cushionopening / 2);end.Y = 0;
            borders[0] = new borderco(0.2, start, end);

            start.X =0; start.Y =17;
            end.X = 0; end.Y =cheight-17 ;
            borders[1] = new borderco(0.2, start, end);

            start.X= 17; start.Y=cheight;
            end.X = (cwidth / 2) - (cushionopening / 2); end.Y = cheight;
            borders[2] = new borderco(0.2, start, end);

            start.X = cwidth / 2 + (cushionopening / 2); start.Y = 0;
            end.X = cwidth - 17; end.Y = 0;
            borders[3] = new borderco(0.2, start, end);

            start.X=cwidth; start.Y=17;
            end.X = cwidth; end.Y =cheight -17;
            borders[4] = new borderco(0.2, start, end);

            start.X = cwidth / 2 + (cushionopening / 2); start.Y = cheight;
            end.X = cwidth - 17; end.Y = cheight;
            borders[5] = new borderco(0.2, start, end);

            start.X = 300; start.Y = 100;
            end.X = 500; end.Y = 200;
           // testborder = new borderco(20, start, end);





        }
        private void createballs()
        {
            Vector[] points = { new Vector(0, 0), new Vector(3 * (cwidth / 4) + R, cheight / 2), new Vector(cwidth / 5, (cheight / 3) * 2), new Vector(cwidth / 5, cheight / 3), new Vector(cwidth / 5, cheight / 2), new Vector(cwidth / 2, cheight / 2), new Vector(3 * (cwidth / 4), cheight / 2), new Vector(cwidth - cwidth / 12 -8, cheight / 2) };

            moving = false;
            Vector whitevector = new Vector(120, 230);
            white = new white(whitevector, -4, 0, 6.0);

            double yin = cheight / 2;//dont know why i made this but i need it.

            red[] reds = new red[15];
            colour[] colours = new colour[6];

            for (int i = 1; i < 16; i++)
            {
                int n = trianglec(i);
                Vector startingpos = new Vector(607.61 + trianglec(i) * 11 - 6, yin - n * 6 + (i - triangle(n - 1)) * 12.5 - 6);//made this to find the positon of the balls in a triangle

                reds[i - 1] = new red(startingpos, 1, i, 6.0);//this is to make the balls in a triangle
            }
            for (int i = 2; i < 8; i++)
            {
                colours[i - 2] = new colour(points[i], i, i + 14, 6.0);//this is to make the coloured balls
            }
        }

        private void createpockets()
        {
            pockets[1, 0] = new top_middle();
            pockets[1, 1] = new bottom_middle();
            pockets[0, 0] = new top_left();
            pockets[2, 0] = new top_right();
            pockets[2, 1] = new bottom_right();
            pockets[0, 1] = new bottom_left();

        }
        public string message = "Foul from last round awarded to current player = ";

        private void createtextboxes()
        {
            MainWindow.canvas.Children.Add(playerindicator);
            MainWindow.addcanvaspos(405, 25, playerindicator);
            playerindicator.Background = new SolidColorBrush(Colors.Orange);
            playerindicator.FontSize = 20;
            playerindicator.Text = "player1";
            Canvas.SetZIndex(playerindicator, 100);

            MainWindow.canvas.Children.Add(player1score);
            MainWindow.addcanvaspos(380, 25, player1score);
            player1score.Background = new SolidColorBrush(Colors.Orange);
            player1score.FontSize = 20;
            player1score.Text = "0";
            Canvas.SetZIndex(player1score, 100);

            MainWindow.canvas.Children.Add(player2score);
            MainWindow.addcanvaspos(480, 25, player2score);
            player2score.Background = new SolidColorBrush(Colors.DarkTurquoise);
            player2score.FontSize = 20;
            player2score.Foreground = new SolidColorBrush(Colors.White);
            player2score.Text = "0";
            Canvas.SetZIndex(player2score, 100);

            MainWindow.canvas.Children.Add(foulindicator);
            MainWindow.addcanvaspos(520, 25, foulindicator);
            foulindicator.Background = new SolidColorBrush(Colors.BlueViolet);
            foulindicator.FontSize = 20;
            foulindicator.Foreground=new SolidColorBrush(Colors.White);
            foulindicator.Text = message + 0; ;
          
            Canvas.SetZIndex(foulindicator, 100);
        }


        private List<ballpair> collisions = new List<ballpair>();
        public static int idealsimrunsperframe = 5;
        private Stopwatch enginestopwatch = new Stopwatch();

        public void UpdateGame(double ttime)
        {
            if (!gamewon)//checks if game is won
            {
                int simrunsperframe = idealsimrunsperframe;
                double time = 0;
                if (ttime < 10) simrunsperframe = (idealsimrunsperframe + 1) / 2;//for faster monitors to avoid too many sims
                enginestopwatch.Start();//for diagnostics
                time = ttime / simrunsperframe;

                for (int k = 0; k < simrunsperframe; k++)//for loop to iterate function set times. 
                {

                    collisions.Clear();
                    int movingballscounter = 0;//counter to actually check if any balls are moving. 
                    for (int i = 0; i < balls.Length; i++)
                    {

                        if (ismoving[i])//only need to applyfriction to ball that are moving. 
                        {
                            Hitball.time = time / 1000.0;

                            Hitball.applyfriction(balls[i].hit);
                            if (ismoving[i])//if they are still moving after friction their positions are updated based on their new velocities. 
                            {
                                movingballscounter++;

                                balls[i].hit.updatev(time / 1000.0);

                                if (balls[i].hit.pos.X <= cwidth / 2)//to check which quadrant ball is in. 
                                {
                                    if (balls[i].hit.pos.Y <= cheight / 2)
                                    {
                                        //quadrant 1
                                        borders[0].collisions(balls[i].hit);
                                        borders[1].collisions(balls[i].hit);
                                    }
                                    else
                                    {
                                        //quadrant 2
                                        borders[2].collisions(balls[i].hit);
                                        borders[1].collisions(balls[i].hit);
                                    }
                                }
                                else
                                {
                                    if (balls[i].hit.pos.Y <= cheight / 2)
                                    {
                                        //quadrant 3 
                                        borders[3].collisions(balls[i].hit);
                                        borders[4].collisions(balls[i].hit);
                                    }
                                    else
                                    {
                                        //quadrant 4
                                        borders[5].collisions(balls[i].hit);
                                        borders[4].collisions(balls[i].hit);
                                    }
                                }
                                //checks which section of table ball is in so the correct pocket logic is called for logic. 
                                pockets[balls[i].hit.sectionc.X, balls[i].hit.sectionc.Y].pocketlogic(balls[i].hit);
                               // testborder.collisions(balls[i].hit);

                            }


                        }
                    }
                    if (movingballscounter == 0)
                    {
                        //no balls are moving any further and game logic can be ran. 
                        moving = false;
                        gamelogic();
                        break;
                    }
                    else
                    {
                        moving = true;//frame is still moving so calculations are done. 
                        int counternew = -1;
                        for (int i = 0; i < balls.Length; i++)
                        {

                            for (int j = i + 1; j < balls.Length; j++)//really clever two for loops to cycle through every ball pair without revisitting a ball pair. 
                            {
                                if ((!balls[i].potted) && (!balls[i].potted))// makes sure the balls aren't potted. 
                                {
                                    counternew++;
                                    (double determinant, double t1) = Hitball.intersectionpoint(balls[i].hit, balls[j].hit);
                                    if (determinant >= 0 && t1 <= 0 && Math.Abs(t1) <= (3 * time) / 1000.0)
                                    {
                                        collisions.Add(new ballpair() { x = i, y = j, t = t1 });
                                    }
                                }
                            }
                        }
                        collisions = collisions.OrderByDescending(pair => Math.Abs(pair.t)).ToList();//collisions organised in order of which occurs firstl 
                        foreach (ballpair hit in collisions)
                        {
                            if (Hitball.checkhit(balls[hit.x].hit, balls[hit.y].hit, hit.t))//checks the collision and ammends the positions of the balls if need be.
                            {
                                Hitball.after(balls[hit.x].hit, balls[hit.y].hit);//does the collision logic. 
                            }
                        }
                    }




                }
                updatecanvas();//updates canvas position of ball 
                double elapsedenginetime = enginestopwatch.ElapsedMilliseconds;//for diagnostics. 
                enginestopwatch.Reset();
            }
        }
        private void updatecanvas()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].updatecanvasp();
            }
        }
      
        private void gamelogic()
        {
            int k;
            int d;
            int foul=0;
            if (player) k = 0; //first sets k to a value to acess the correct index position of the playerscore array. 
            else k = 1;
            d = k ^ 1;//uses an xor to make a 1 into a 0 and 0 to a 1. 'd' is used to acess the non-playing player's score. 
            if (!hascollisionoccured)//checks if the cue ball collided with anything. 
            {
                if (wascueballpotted)//then checks if the cue ball that collided was potted. This is to determine whether to reset the cue ball or not.
                {
                    white.reset();
                    foul = findmaxfoul();
                }
                else//if it wasn't potted it's foul is simply set to 4.
                {
                    foul = 4;
                    player = !player;//the player bool is flipped to indicate it's the other player's turn. 
                }

            }
            else if (wascueballpotted)//if cue ball did collide with something but it was potted this logic is called.  
            {
                white.reset();
                foul = Math.Max(findmaxfoul(), colourfoul(collisionball));//The maximum foul the player offended is calculated by seeing if the value of this hit ball is more or the highest potted ball. At the same time this function flips the player.
                if (musthitcolour)//if a colour was to be hit these booleans need to be updated.
                {
                    red_balls_remaining_logic();
                }

            }
            else if (musthitred)//if cue ball wasn't potted we check if the ball the cue ball collided with was in fact red. 
            {
                if (collisionball.type != 1) foul = Math.Max(findmaxfoul(), colourfoul(collisionball)); //The max foul is then calculated
                else if (wascolourpotted) foul = findmaxfoul();
                else if (wasredpotted)
                {
                    foreach (ball potted in pottedballs_inpreviousframe)//this for when mutliple reds are potted.
                    {
                        if (potted.type == 1)
                        {
                            playerscore[k]++;
                        }
                    }
                    musthitred = false;
                    musthitcolour = true;//now a colour ball must be struck .
                }
                else player = !player;//no foul but nothing potted. Player flip. 
            }
            else if (musthitcolour)//similar to red ball. 
            {
                if (collisionball.type != 2)//makes sure ball cue ball collided with was a coloured ball. 
                {
                    foul = Math.Max(findmaxfoul(), colourfoul(collisionball));
                }
                else if (pottedballs_inpreviousframe.Count > 1)//makes sure only one coloured ball is potted at a time. Anymore is a foul 
                {
                    foul = findmaxfoul();
                }
                else if (wascolourpotted)
                {
                    if (pottedballs_inpreviousframe[0].pointvalue == collisionball.pointvalue)//checks potted ball is same as collision ball
                    {
                        playerscore[k] += pottedballs_inpreviousframe[0].pointvalue;
                        pottedballs_inpreviousframe[0].reset();//resets the ball
                        resetcolour.Add(pottedballs_inpreviousframe[0]);//adds ball to resetcolour to make sure it isn't overlapping with anything 
                    }
                    else
                    {
                        foul = Math.Max(findmaxfoul(), colourfoul(collisionball));
                    }
                }
                else
                {
                    if (wasredpotted) foul = Math.Max(findmaxfoul(), colourfoul(collisionball));
                    else player = !player;
                }
                red_balls_remaining_logic();

            }
            else if (musthitlowestcolour)//for when alll the reds are potted. 
            {
                if (collisionball.pointvalue != lowestcolour) foul = Math.Max(findmaxfoul(), colourfoul(collisionball));// the collision ball must be the same as the potted ball to avoid foul 
                else if (pottedballs_inpreviousframe.Count > 1) foul = findmaxfoul();//make sure only one ball potted at a time
                else if (wascolourpotted)
                {
                    if (pottedballs_inpreviousframe[0].pointvalue != lowestcolour) foul = findmaxfoul();//checks if pottedball is the correct ball. If it's not it is a foul 
                    else
                    {
                        playerscore[k] += pottedballs_inpreviousframe[0].pointvalue;//the correct ball has been potted without foul and the score is added to player.
                        lowestcolour++;//lowest colour is incremented by 1. 
                        if (lowestcolour == 8)//if lowestcolour is 8 it means the black ball has been potted and thus the game is concluded. 
                        {
                            gamewon = true;//updategame won't run anymore till reset. 
                            if (playerscore[k] > playerscore[d])//highest score wins .
                            {
                                whichplayerwon = k;
                                MessageBox.Show("Player " + k + 1 + " has won");
                            }
                            else if (playerscore[k] < playerscore[d])
                            {
                                whichplayerwon = d;
                                MessageBox.Show("Player " + d + 1 + " has won");
                            }
                            else
                            {
                                MessageBox.Show("It's a draw.");

                            }



                        }
                    }

                }
                else player = !player;// if nothing is potted player is just flipped. 


            }
            else
            {
                MessageBox.Show("Error"); //should't reach here and never has. 
            }
            foreach(ball reset in resetcolour)
            {
                Hitball.moveballawayfromstatic(reset.hit);//makes sure the balls are overlapping some other ball. 
            }
            //booleans set to defaults. 
            wascolourpotted = false;
            wascueballpotted = false;
            wasredpotted=false;
            pottedballs_inpreviousframe.Clear();
            hascollisionoccured = false;
            playerscore[d] += foul;
            collisionball = null;
            if (player)
            {
                playerindicator.Background = new SolidColorBrush(Colors.Orange);
                playerindicator.Text = "player 1";
            }
            else
            { 
                playerindicator.Background = new SolidColorBrush(Colors.DarkTurquoise);
                playerindicator.Text = "player 2";

            }
            player1score.Text = playerscore[0].ToString();
            player2score.Text = playerscore[1].ToString();
            foulindicator.Text=message+foul.ToString();
            foul = 0;//score set back to 0 although unesseary, 



        }
        //this basically checks for whether the value of a ball is less than 4. If it is the foul worthy for it is set to at least 4. If it's equal or greater than 4 than the foul from potting it is equal to the value of the ball.
        private int colourfoul(ball potted)
        {
            if (potted.pointvalue < 4) return 4;
            else return potted.pointvalue;
        }
        //this function finds the highest value ball that was potted as a foul.
        //It resets all the colour balls that were potted from fouls 
        //it returns a minmum value of 4 and a max of 7.
        //the function also flips the player bool to indicate it's the other player's turn.
        private int findmaxfoul()
        {
            int foul = 4;
            if(musthitlowestcolour)
            {
                foul = lowestcolour;
                if (foul < 4) foul = 4;
            }
            foreach (ball potted in pottedballs_inpreviousframe)
            {
              
                if (potted.type == 2)
                {
                    potted.reset();
                    resetcolour.Add(potted);
                    foul = Math.Max(foul, colourfoul(potted));
                }
            }
            player = !player;
            return foul;
        }
        public void red_balls_remaining_logic()
        {
            if (pottedreds == 15)
            {
                musthitlowestcolour = true;
                musthitred = false;
                musthitcolour = false;
                lowestcolour = 2;
            }
            else
            {
                musthitlowestcolour = false;
                musthitred = true;
                musthitcolour = false;
            }
        }

        //adds the game balls when instantiated to Engine.balls[]
        public static void addball(ball sphere, int whichball)
        {
            balls[whichball] = sphere;

        }
        public void reset()
        {
            gamewon = false;
            for (int i = 0; i < balls.Length; i++)
            {
                ismoving[i] = false;//sets all the balls to not moving so they aren't interacted with update game collisions logic. 
                balls[i].reset();
            }
            playerscore[0] = 0; playerscore[1] = 0; player = true;
            player1score.Text = playerscore[0].ToString();
            player2score.Text = playerscore[1].ToString();
            foulindicator.Text = "0";
        }
        public void zero()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].hit.stophitball();
                ismoving[i] = false;
            }
        }
        //purpose of trianglec is to find which a row a certain ball would be within a triangle.
        //this is useful for creating the triangle of red balls rather than having to calculate the location of each ball manually I use this function. 
        private int trianglec(int i)
        {
            int n = 0;
            int num = 0;
            bool found;
            do
            {
                found = false;
                n++;
                num = triangle(n);
                if (i <= num) found = true;
                else found = false;
            } while (!found);//checks if the argument value being looked for is less than a triangle number.. 
            return n;
        }
        private int triangle(int n)//purpose of this is to give the different triangle numbers. 
        {
            int num = (n * (n + 1)) / 2;
            return num;
        }
       

    }
}