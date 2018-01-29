/**
 * Copyright 2018 Jean Pascal Bellot
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 **/

namespace XEyesWPF
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using System.IO;
    using System.IO.IsolatedStorage;
    using Microsoft.Win32;

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
            InitializeComponent();
            eyes = new Eyes();
            jiggler = new Jiggler(this);
            leftMiddle = new Point(10 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            rightMiddle = new Point(110 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            mainCanvas.DataContext = eyes;
            eyes.LeftCenter = new Point(10 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            eyes.RightCenter = new Point(110 + 100 / 2 - 40 / 2, 10 + 100 / 2 - 40 / 2);
            timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1;
            timer.Start();
            LoadSettings();
            SystemEvents.SessionEnded += SystemEvents_SessionEnded;
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            SaveSettings();   
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            SaveSettings();
            SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                Point mousePosition = new Point();
                if (CanUseCapture)
                {
                    Mouse.Capture(this);
                    mousePosition = Mouse.GetPosition(this);
                    Mouse.Capture(null);
                }
                else
                {
                    mousePosition = Mouse.GetPosition(this);
                }
                HandleMouse(mousePosition);
            }));

        }

        private Point LeftCenter
        {
            get
            {
                double eyeWidth = Width / 2 - 10;
                double eyeHeight = Height / 2 - 10;
                double x = Width / 2 - eyeWidth / 2;
                double y = Height / 2 - eyeHeight / 2;
                return new Point(x, y);
            }
        }


        public void DoWiggle()
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {

                    Point screenCoordinate = PointToScreen(LeftCenter);
                    int x = (int)screenCoordinate.X;
                    int y = (int)screenCoordinate.Y;
                    //Point screenCoordinate = this.PointToScreen(this.LeftCenter);

                    NativeMethods.Click(x, y);
                    //this.didWiggle = true;
                    jiggler.LastActivity = DateTime.Now;
                    //Console.WriteLine("Wiggled");
                    //this.Invalidate();
                }
            }));

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Close();
        }

        private void menuPrefs_Click(object sender, RoutedEventArgs e)
        {
            PrefsWindow prefs = new PrefsWindow();
            prefs.Eyes = new Eyes(eyes);
            prefs.EnableJiggle = jiggler.EnableJiggle;
            prefs.ZenJiggle = jiggler.ZenJiggle;
            prefs.Wiggle = jiggler.Wiggle;
            timer.Stop();
            prefs.Left = Left - 2;
            prefs.Top = Top - 2;
            var currentScreen = ScreenHandler.GetCurrentScreen(this);
            if (prefs.Left + prefs.Width > currentScreen.Bounds.X + currentScreen.Bounds.Width)
            {
                prefs.Left = currentScreen.Bounds.X + currentScreen.Bounds.Width - prefs.Width;
            }
            if (prefs.Top + prefs.Height > currentScreen.Bounds.Y + currentScreen.Bounds.Height)
            {
                prefs.Top = currentScreen.Bounds.Y + currentScreen.Bounds.Height - prefs.Height;
            }
            bool? ok = prefs.ShowDialog();
            if (ok != null)
            {
                if (ok == true)
                {
                    Eyes newEyes = prefs.Eyes;
                    if (newEyes != null)
                    {
                        eyes.BackColor = newEyes.BackColor;
                        eyes.EyeColor = newEyes.EyeColor;
                        eyes.ForeColor = newEyes.ForeColor;
                    }
                    jiggler.ZenJiggle = prefs.ZenJiggle;
                    jiggler.EnableJiggle = prefs.EnableJiggle;
                    jiggler.Wiggle = prefs.Wiggle;
                }
            }
            timer.Start();
        }



        private void windowMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private bool CanUseCapture
        {
            get
            {
                if (IsMouseOver)
                {
                    return false;
                }
                if (contextMenu.IsMouseOver)
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
                jiggler.LastActivity = DateTime.Now;
            }
            if (mouseMoved)
            {
                this.mousePosition = mousePosition;
                Point p1 = leftMiddle;
                eyes.LeftCenter = MoveEye(p1, mousePosition);

                Point p2 = rightMiddle;
                eyes.RightCenter = MoveEye(p2, mousePosition);
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
                            Left = x;
                            Top = y;
                            int c1 = GetArgb(eyes.EyeColor);
                            int c2 = GetArgb(eyes.ForeColor);
                            int c3 = GetArgb(eyes.BackColor);
                            bool eJ = jiggler.EnableJiggle;
                            bool zJ = jiggler.ZenJiggle;
                            bool w = jiggler.Wiggle;

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
                            eyes.EyeColor = FromArgb(c1);
                            eyes.ForeColor = FromArgb(c2);
                            eyes.BackColor = FromArgb(c3);
                            jiggler.ZenJiggle = zJ;
                            jiggler.EnableJiggle = eJ;
                            jiggler.Wiggle = w;
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
                        bw.Write(Left);
                        bw.Write(Top);
                        int c1 = GetArgb(eyes.EyeColor);
                        int c2 = GetArgb(eyes.ForeColor);
                        int c3 = GetArgb(eyes.BackColor);
                        bw.Write(c1);
                        bw.Write(c2);
                        bw.Write(c3);
                        bw.Write(jiggler.EnableJiggle);
                        bw.Write(jiggler.ZenJiggle);
                        bw.Write(jiggler.Wiggle);
                    }
                }
            }
        }

        private void mainCanvas_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            jiggler.CanWiggle = false;
        }

        private void mainCanvas_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            jiggler.CanWiggle = true;
        }

    }
}
