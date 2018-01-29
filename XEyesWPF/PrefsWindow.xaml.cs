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
using System.Windows.Shapes;

namespace XEyesWPF
{
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
            this.InitializeComponent();
            this.labelHelp.Text = HELPTEXT;
        }

        public Eyes Eyes
        {
            get { return this.eyes; }
            set
            {
                this.eyes = value;
                if (this.eyes != null)
                {
                    this.colorPickerEye.SelectedColor = this.eyes.EyeColor;
                    this.colorPickerBack.SelectedColor = this.eyes.BackColor;
                    this.colorPickerFore.SelectedColor = this.eyes.ForeColor;
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
                if (this.cbJiggle.IsChecked.HasValue)
                {
                    return this.cbJiggle.IsChecked.Value;
                }
                return false;
            }
            set { this.cbJiggle.IsChecked = value; }
        }

        public bool ZenJiggle
        {
            get
            {
                if (this.cbZenJiggle.IsChecked.HasValue)
                {
                    return this.cbZenJiggle.IsChecked.Value;
                }
                return false;
            }
            set { this.cbZenJiggle.IsChecked = value; }
        }

        public bool Wiggle
        {
            get
            {
                if (this.cbWiggle.IsChecked.HasValue)
                {
                    return this.cbWiggle.IsChecked.Value;
                }
                return false;
            }
            set { this.cbWiggle.IsChecked = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.eyes = null;
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.eyes = new Eyes();
            this.DialogResult = true;
            this.Close();
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.eyes != null)
            {
                Color c = Colors.Black;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                this.eyes.EyeColor = c;
            }
        }

        private void ColorPicker_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.eyes != null)
            {
                Color c = Colors.Transparent;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                this.eyes.BackColor = c;
            }
        }

        private void colorPickerFore_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.eyes != null)
            {
                Color c = Colors.Black;
                if (e.NewValue.HasValue)
                {
                    c = e.NewValue.Value;
                }
                this.eyes.ForeColor = c;
            }

        }

    }
}
