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
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for PrefsWindow.xaml
    /// </summary>
    public partial class PrefsWindow : Window
    {
        private const string HELPTEXT = @"XEyes V2.0.0.0
Tick the 'Enable Jiggle' checkbox to begin jiggling the mouse; untick it to stop. The 'Zen Jiggle' checkbox enables a mode in which the pointer is jiggled 'virtually' - the system believes it to be moving and thus screen saver activation, etc., is prevented, but the pointer does not actually move.";

        private Eyes eyes;

        public PrefsWindow()
        {
            InitializeComponent();
            labelHelp.Text = HELPTEXT;
        }

        public Eyes Eyes
        {
            get { return eyes; }
            set
            {
                eyes = value;
                if (eyes != null)
                {
                    colorPickerEye.SelectedColor = eyes.EyeColor;
                    colorPickerBack.SelectedColor = eyes.BackColor;
                    colorPickerFore.SelectedColor = eyes.ForeColor;
                }
            }
        }

        //public Jiggler Jiggler
        //{
        //    get { return this.jiggler; }
        //    set
        //    {
        //        this.jiggler = value;
        //        if (this.jiggler != null)
        //        {
        //            this.cbJiggle.IsChecked = this.jiggler.EnableJiggle;
        //            this.cbZenJiggle.IsChecked = this.jiggler.ZenJiggle;
        //        }
        //    }
        //}

        public bool EnableJiggle
        {
            get
            {
                if (cbJiggle.IsChecked.HasValue)
                {
                    return cbJiggle.IsChecked.Value;
                }
                return false;
            }
            set { cbJiggle.IsChecked = value; }
        }

        public bool ZenJiggle
        {
            get
            {
                if (cbZenJiggle.IsChecked.HasValue)
                {
                    return cbZenJiggle.IsChecked.Value;
                }
                return false;
            }
            set { cbZenJiggle.IsChecked = value; }
        }

        public bool Wiggle
        {
            get
            {
                if (cbWiggle.IsChecked.HasValue)
                {
                    return cbWiggle.IsChecked.Value;
                }
                return false;
            }
            set { cbWiggle.IsChecked = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            eyes = null;
            DialogResult = false;
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            eyes = new Eyes();
            DialogResult = true;
            Close();
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (eyes != null)
            {
                Color c = Colors.Black;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                eyes.EyeColor = c;
            }
        }

        private void ColorPicker_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (eyes != null)
            {
                Color c = Colors.Transparent;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                eyes.BackColor = c;
            }
        }

        private void colorPickerFore_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (eyes != null)
            {
                Color c = Colors.Black;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                eyes.ForeColor = c;
            }

        }

    }
}
