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
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public class Eyes : INotifyPropertyChanged
    {
        private Point leftCenter;
        private Point rightCenter;        
        private Color eyeColor = Colors.Black;
        private Color backColor = Colors.White;
        private Color foreColor = Colors.Black;
        private Brush eyeBrush;
        private Brush backBrush;
        private Brush foreBrush;

        public Eyes()
        {
            leftCenter = new Point();
            rightCenter = new Point();
            eyeColor = Colors.Black;
            backColor = Colors.White;
            eyeBrush = new SolidColorBrush(eyeColor);
            backBrush = new SolidColorBrush(backColor);
            foreBrush = new SolidColorBrush(foreColor);
        }

        public Eyes(Eyes other)
        {
            leftCenter = other.leftCenter;
            rightCenter = other.rightCenter;
            eyeColor = other.eyeColor;
            backColor = other.backColor;
            eyeBrush = new SolidColorBrush(eyeColor);
            backBrush = new SolidColorBrush(backColor);
            foreBrush = new SolidColorBrush(foreColor);
        }

        public Point LeftCenter
        {
            get { return leftCenter; }
            set
            {
                leftCenter = value;
                OnPropertyChanged("LeftCenter");
            }
        }

        public Point RightCenter
        {
            get { return rightCenter; }
            set
            {
                rightCenter = value;
                OnPropertyChanged("RightCenter");
            }
        }

        public Brush EyeBrush
        {
            get { return eyeBrush; }
        }

        public Color EyeColor
        {
            get { return eyeColor; }
            set
            {
                eyeColor = value;
                eyeBrush = new SolidColorBrush(eyeColor);
                OnPropertyChanged("EyeBrush");
            }
        }

        public Brush BackBrush
        {
            get { return backBrush; }
        }

        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                backBrush = new SolidColorBrush(backColor);
                OnPropertyChanged("BackBrush");
            }
        }

        
        public Brush ForeBrush
        {
            get { return foreBrush; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                foreBrush = new SolidColorBrush(foreColor);
                OnPropertyChanged("ForeBrush");
            }
        }

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
