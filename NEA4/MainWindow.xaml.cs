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
using System.Windows.Threading;
using System.Net.Sockets;


namespace NEA4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public static Canvas canvas = new Canvas();

        public static double cwidth = 800;
        public static double cheight = 400;
        private Stopwatch stopwatch = new Stopwatch();
        private Engine engine = new Engine();
        public static Vector tablepos = new Vector(0, 0);
        private Button resetb = new Button();
        private Button zerob = new Button();
        private Button infoo = new Button();
        //not parametrised becuase i only have one picture on entire program.
        private BitmapImage tablebitmap = new BitmapImage(new Uri(@"C:\IBurygrammar\NEA project\tabletopview.png"));

        private Image tabletop = new Image();

        public MainWindow()
        {

            InitializeComponent();
           
            MouseDown += MainWindow_MouseDown;
            MouseUp += MainWindow_MouseUp;
            MouseMove += MainWindow_MouseMove;

            Viewbox v = new Viewbox();
            v.Stretch = Stretch.Uniform;
            v.StretchDirection = StretchDirection.Both;

            GameWindow.Content = v;
            v.Child = canvas;
            canvas.Background = new SolidColorBrush(Colors.White);

            canvas.Height = 600; canvas.Width = 1000;
            addcanvaspos(0, 0, canvas);


            createtable((int)cwidth, (int)cheight);

            resetb.Height = 30; resetb.Width = 60; resetb.Content = "Reset"; resetb.Click += (sender, e) => { reset(); };
            addcanvaspos(0, 25, resetb);
            canvas.Children.Add(resetb);

            zerob.Height = 30; zerob.Width = 60; zerob.Content = "Zero"; zerob.Click += (sender, e) => { engine.zero(); };
            addcanvaspos(100, 25, zerob);
            canvas.Children.Add(zerob);

          

            infoo.Height = 30; infoo.Width = 60; infoo.Content = "Help"; infoo.Click +=  OpenWindow1Button_Click;
            addcanvaspos(200, 25, infoo);
            canvas.Children.Add(infoo);

        }
        private void OpenWindow1Button_Click(object sender, RoutedEventArgs e) 
        {
            Window1 newWindow = new Window1();
            newWindow.Show(); 
        }

   

        private void reset()
        {
            engine.reset();
        
        }


        private void createtable(int width, int height)//this is to create the table and lines on it.
        {
            double imageratio = 37.5 / 32.8;//these values exist for personal calculations. 
            double cushionratio = 2.35 / 37.5;
            double heighttotable = 1.143125;
            double widthtotable = 1.28625;
           
            tablepos.X = 100; tablepos.Y = 100;
          

            tabletop.Source = tablebitmap;
            tabletop.Width = 914.5; tabletop.Height = 514.4;
            addcanvaspos(42.6, 42.6, tabletop);
            Canvas.SetZIndex(tabletop, 1);
            canvas.Children.Add(tabletop);


            Line baulk = new Line();
            baulk.Stroke = new SolidColorBrush(Colors.Red);
            baulk.StrokeThickness = 1;
            baulk.X1 = tablex(0);
            baulk.Y1 = tabley(0);
            baulk.X2 = tablex(0);
            baulk.Y2 = tabley(height);
            canvas.Children.Add(baulk);
            Canvas.SetZIndex(baulk, 7);


            Line baulk2 = new Line();
            baulk2.Stroke = new SolidColorBrush(Colors.Red);
            baulk2.StrokeThickness = 1;
            baulk2.X1 = tablex(width);
            baulk2.Y1 = tabley(0);
            baulk2.X2 = tablex(width);
            baulk2.Y2 = tabley(height);
            canvas.Children.Add(baulk2);
            Canvas.SetZIndex(baulk2, 7);

            Line baulk3 = new Line();
            baulk3.Stroke = new SolidColorBrush(Colors.Red);
            baulk3.StrokeThickness = 1;
            baulk3.X1 = tablex(0);
            baulk3.Y1 = tabley(0);
            baulk3.X2 = tablex(width);
            baulk3.Y2 = tabley(0);
            canvas.Children.Add(baulk3);
            Canvas.SetZIndex(baulk3, 7);

            Line baulk4 = new Line();
            baulk4.Stroke = new SolidColorBrush(Colors.Red);
            baulk4.StrokeThickness = 1;
            baulk4.X1 = tablex(0);
            baulk4.Y1 = tabley(height);
            baulk4.X2 = tablex(width);
            baulk4.Y2 = tabley(height);
            canvas.Children.Add(baulk4);
            Canvas.SetZIndex(baulk4, 7);

        }
        public static Vector updategenpos(double X, double Y)//this to add position to the {table position}
        {
            Vector newp = new Vector();
            newp.X = tablepos.X + X;
            newp.Y = tablepos.Y + Y;  
            return newp;
        }

        public static void addcanvaspos(double X, double Y, UIElement obj)//this is just to add the position to the canvas
        {
            Canvas.SetLeft(obj, X);
            Canvas.SetTop(obj, Y);
        }
        public static double tablex(double X)
        {
            X = tablepos.X + X; return X;
        }
        public static double tabley(double Y)
        {
            Y = tablepos.Y + Y; return Y;
        }
        private bool mouseisdown = false;
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseisdown)
            {
                Vector mouse = new Vector(e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                Engine.white.gencue(mouse);
            }
        }



        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {

            double x = e.GetPosition(canvas).X;
            double y = e.GetPosition(canvas).Y;

            if ((x >= Engine.white.pos.X - 6) && (y >= Engine.white.pos.Y - 6) && (x <= Engine.white.pos.X + 6) && (y <= Engine.white.pos.Y + 6) && !engine.moving)
            {
                mouseisdown = true;
                Engine.white.cue.X1 = Engine.white.pos.X; Engine.white.cue.Y1 = Engine.white.pos.Y; Engine.white.cue.X2 = Engine.white.pos.X; Engine.white.cue.Y2 = Engine.white.pos.Y;
                Engine.white.cue.Visibility = Visibility.Visible;
                Engine.white.guide.X1 = Engine.white.pos.X; Engine.white.guide.Y1 = Engine.white.pos.Y; Engine.white.guide.X2 = Engine.white.pos.X; Engine.white.guide.Y2 = Engine.white.pos.Y;
                Engine.white.guide.Visibility = Visibility.Visible;
            }
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (mouseisdown)
            {
                mouseisdown = false;

                startsim();
            }
        }


        private void startsim()
        {
         
            if (!engine.moving)
            {
                Engine.white.applycueballspeed();
                Engine.white.cue.Visibility = Visibility.Hidden;
                Engine.white.guide.Visibility = Visibility.Hidden;
                engine.moving= true;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
                stopwatch.Reset();
            }

        }
       
        private void CompositionTarget_Rendering(object sender, EventArgs e)//function is called just before the monitor is about to update. 
        {
            if (!engine.moving) { stopsim(); }
            else
            {
                double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                Hitball.time = elapsedMilliseconds / 1000.0;
                engine.UpdateGame(elapsedMilliseconds);
                stopwatch.Restart();
            }
        }
        private void stopsim()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            stopwatch.Stop();
            stopwatch.Reset();
        }

    }


}