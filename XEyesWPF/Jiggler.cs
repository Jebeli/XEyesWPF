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
    public class Jiggler
    {
        private System.Timers.Timer timer;
        private bool zig = true;
        private bool zenJiggle = true;
        private bool wiggle = true;
        private bool enableJiggle = false;
        private DateTime lastActivity;
        private bool canWiggle = true;
        private bool didWiggle = false;
        private MainWindow visual;

        public Jiggler(MainWindow v)
        {
            visual = v;
            timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;
            lastActivity = DateTime.Now;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (enableJiggle)
            {
                DoJiggle();
                DoWiggle();
            }
        }

        public bool ZenJiggle
        {
            get { return zenJiggle; }
            set { zenJiggle = value; }
        }

        public bool EnableJiggle
        {
            get { return enableJiggle; }
            set
            {
                enableJiggle = value;
                timer.Enabled = value;
            }
        }

        public bool Wiggle
        {
            get { return wiggle; }
            set { wiggle = value; }
        }

        public bool CanWiggle
        {
            get { return canWiggle; }
            set { canWiggle = value; }
        }

        private void DoJiggle()
        {
            if (zenJiggle)
            {
                NativeMethods.Jiggle(0, 0);
                timer.Interval = 1000;
            }
            else
            {
                if (zig)
                {
                    NativeMethods.Jiggle(4, 4);
                    timer.Interval = 50;
                }
                else
                {
                    NativeMethods.Jiggle(-4, -4);
                    timer.Interval = 1000;
                }
            }
            zig = !zig;
        }

        private TimeSpan ElapsedTimeSinceLastActivity
        {
            get { return DateTime.Now - lastActivity; }
        }

        public DateTime LastActivity
        {
            get { return lastActivity; }
            set { lastActivity = value; }
        }

        private bool NeedsWiggle
        {
            get
            {
                if (ElapsedTimeSinceLastActivity.TotalMilliseconds > 1000 * 60)
                {
                    return true;
                }
                return false;
            }
        }

        private void DoWiggle()
        {
            if (wiggle && canWiggle && NeedsWiggle)
            {
                visual.DoWiggle();
                didWiggle = true;
            }
            else
            {
                didWiggle = false;
            }

        }
    }
}
