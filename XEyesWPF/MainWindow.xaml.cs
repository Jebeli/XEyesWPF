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
using System.Windows.Threading;
using System.IO;
using System.IO.IsolatedStorage;

namespace XEyesWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point leftMiddle;
        private Point rightMiddle;
        private Eyes eyes;
        private Jiggler jiggler;
        private System.Timers.Timer timer;
        private Point mousePosition;

        public MainWindow()
        {
            this.InitializeComponent();
            this.eyes = new Eyes();
            this.jiggler = new Jiggler(this);
            this.leftMiddle = new Point(10 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            this.rightMiddle = new Point(110 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            this.mainCanvas.DataContext = this.eyes;
            this.eyes.LeftCenter = new Point(10 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            this.eyes.RightCenter = new Point(110 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            this.timer = new System.Timers.Timer();
            this.timer.Elapsed += timer_Elapsed;
            this.timer.Interval = 1;
            this.timer.Start();
            this.LoadSettings();
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.SaveSettings();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                Point mousePosition = new Point();
                if (this.CanUseCapture)
                {
                    Mouse.Capture(this);
                    mousePosition = Mouse.GetPosition(this);
                    Mouse.Capture(null);
                }
                else
                {
                    mousePosition = Mouse.GetPosition(this);
                }
                this.HandleMouse(mousePosition);
            }));

        }

        private Point LeftCenter
        {
            get
            {
                double eyeWidth = this.Width / 2 - 10;
                double eyeHeight = this.Height / 2 - 10;
                double x = this.Width / 2 - eyeWidth / 2;
                double y = this.Height / 2 - eyeHeight / 2;
                return new Point(x, y);
            }
        }


        public void DoWiggle()
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {

                    Point screenCoordinate = this.PointToScreen(this.LeftCenter);
                    int x = (int)screenCoordinate.X;
                    int y = (int)screenCoordinate.Y;
                    //Point screenCoordinate = this.PointToScreen(this.LeftCenter);

                    NativeMethods.Click(x, y);
                    //this.didWiggle = true;
                    this.jiggler.LastActivity = DateTime.Now;
                    //Console.WriteLine("Wiggled");
                    //this.Invalidate();
                }
            }));

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.Close();
        }

        private void menuPrefs_Click(object sender, RoutedEventArgs e)
        {
            PrefsWindow prefs = new PrefsWindow();
            prefs.Eyes = new Eyes(this.eyes);
            prefs.EnableJiggle = this.jiggler.EnableJiggle;
            prefs.ZenJiggle = this.jiggler.ZenJiggle;
            prefs.Wiggle = this.jiggler.Wiggle;
            this.timer.Stop();
            prefs.Left = this.Left - 2;
            prefs.Top = this.Top - 2;
            bool? ok = prefs.ShowDialog();
            if (ok != null)
            {
                if (ok == true)
                {
                    Eyes newEyes = prefs.Eyes;
                    if (newEyes != null)
                    {
                        this.eyes.BackColor = newEyes.BackColor;
                        this.eyes.EyeColor = newEyes.EyeColor;
                        this.eyes.ForeColor = newEyes.ForeColor;
                    }
                    this.jiggler.ZenJiggle = prefs.ZenJiggle;
                    this.jiggler.EnableJiggle = prefs.EnableJiggle;
                    this.jiggler.Wiggle = prefs.Wiggle;
                }
            }
            this.timer.Start();
        }

        

        private void windowMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool CanUseCapture
        {
            get
            {
                if (this.IsMouseOver)
                {
                    return false;
                }
                if (this.contextMenu.IsMouseOver)
                {
                    return false;
                }
                return true;
            }
        }

        private static Point MoveEye(Point center, Point mouse)
        {
            double x = 0;
            double y = 0;
            double ex = center.X;
            double ey = center.Y;
            double dx = mouse.X - ex;
            double dy = mouse.Y - ey;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist < 1)
            {
                dist = 1;
            }
            x = (25 * dx / dist);
            y = (25 * dy / dist);
            return new Point(x + ex, y + ey);
        }

        private static double MaxDist(Point p, Point p2)
        {
            double dx = Math.Abs(p.X - p2.X);
            double dy = Math.Abs(p.Y - p2.Y);
            return Math.Max(dx, dy);
        }

        private void HandleMouse(Point mousePosition)
        {
            double dist = MaxDist(mousePosition, this.mousePosition);
            bool mouseMoved = dist > 2;
            bool keyPressed = Keyboard.Modifiers != ModifierKeys.None || Keyboard.IsKeyDown(Key.A);

            bool mouseClicked = Mouse.LeftButton == MouseButtonState.Pressed ||
                Mouse.RightButton == MouseButtonState.Pressed ||
                Mouse.MiddleButton == MouseButtonState.Pressed ||
                Mouse.XButton1 == MouseButtonState.Pressed ||
                Mouse.XButton2 == MouseButtonState.Pressed;

            //bool mouseClicked Control.MouseButtons != MouseButtons.None;
            //bool keyPressed = Control.ModifierKeys != Keys.None;
            
            if (mouseMoved || keyPressed || mouseClicked)
            {
                this.jiggler.LastActivity = DateTime.Now;
            }
            if (mouseMoved)
            {
                this.mousePosition = mousePosition;
                Point p1 = this.leftMiddle;
                this.eyes.LeftCenter = MoveEye(p1, mousePosition);

                Point p2 = this.rightMiddle;
                this.eyes.RightCenter = MoveEye(p2, mousePosition);
                //Console.WriteLine(dist);
            }
        }

        private static int GetArgb(Color color)
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            int result = a << 24 | r << 16 | g << 8 | b;
            return result;
        }

        private static Color FromArgb(int argb)
        {
            byte a = 0;
            byte r = 0;
            byte g = 0;
            byte b = 0;
            a = (byte)((argb >> 24) & 0xFF);
            r = (byte)((argb >> 16) & 0xFF);
            g = (byte)((argb >> 8) & 0xFF);
            b = (byte)((argb) & 0xFF);
            Color c = Color.FromArgb(a, r, g, b);
            return c;
        }

        private void LoadSettings()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain();
            if (isoStore != null)
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Settings", FileMode.OpenOrCreate, isoStore))
                {
                    if (stream.Length >= 8)
                    {
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            double x = br.ReadDouble();
                            double y = br.ReadDouble();
                            this.Left = x;
                            this.Top = y;
                            int c1 = GetArgb(this.eyes.EyeColor);
                            int c2 = GetArgb(this.eyes.ForeColor);
                            int c3 = GetArgb(this.eyes.BackColor);
                            bool eJ = this.jiggler.EnableJiggle;
                            bool zJ = this.jiggler.ZenJiggle;
                            bool w = this.jiggler.Wiggle;

                            if (stream.Length >= (8 + (3 * 4)))
                            {
                                c1 = br.ReadInt32();
                                c2 = br.ReadInt32();
                                c3 = br.ReadInt32();
                                if (stream.Length > (8 + (3 * 4)))
                                {
                                    eJ = br.ReadBoolean();
                                    zJ = br.ReadBoolean();
                                    if (stream.Position < stream.Length)
                                    {
                                        w = br.ReadBoolean();
                                    }
                                }

                            }
                            this.eyes.EyeColor = FromArgb(c1);
                            this.eyes.ForeColor = FromArgb(c2);
                            this.eyes.BackColor = FromArgb(c3);
                            this.jiggler.ZenJiggle = zJ;
                            this.jiggler.EnableJiggle = eJ;
                            this.jiggler.Wiggle = w;
                        }
                    }
                }
            }
        }

        private void SaveSettings()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain();
            if (isoStore != null)
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Settings", FileMode.Create, isoStore))
                {
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        bw.Write(this.Left);
                        bw.Write(this.Top);
                        int c1 = GetArgb(this.eyes.EyeColor);
                        int c2 = GetArgb(this.eyes.ForeColor);
                        int c3 = GetArgb(this.eyes.BackColor);
                        bw.Write(c1);
                        bw.Write(c2);
                        bw.Write(c3);
                        bw.Write(this.jiggler.EnableJiggle);
                        bw.Write(this.jiggler.ZenJiggle);
                        bw.Write(this.jiggler.Wiggle);
                    }
                }
            }
        }

        private void mainCanvas_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            this.jiggler.CanWiggle = false;
        }

        private void mainCanvas_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            this.jiggler.CanWiggle = true;
        }

    }
}
