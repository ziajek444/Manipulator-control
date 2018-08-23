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

namespace Roboty_przemyslowe
{
    /// <summary>
    /// Interaction logic for RangesWindow.xaml
    /// </summary>
    public partial class RangesWindow : Window
    {
        SolidColorBrush SCBW;

        public RangesWindow()
        {
            InitializeComponent();
            SCBW = new SolidColorBrush(Colors.White);
            xMin_TextBox.GotFocus += TextBox_GotFocus;
            yMin_TextBox.GotFocus += TextBox_GotFocus;
            zMin_TextBox.GotFocus += TextBox_GotFocus;
            xMax_TextBox.GotFocus += TextBox_GotFocus;
            yMax_TextBox.GotFocus += TextBox_GotFocus;
            zMax_TextBox.GotFocus += TextBox_GotFocus;
            Speed_TextBox.GotFocus += TextBox_GotFocus; 
        }

        void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(xMin_TextBox) && xMin_TextBox.Text == "x min")
            {
                    xMin_TextBox.Text = "";
                    xMin_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(yMin_TextBox) && yMin_TextBox.Text == "y min")
            {
                yMin_TextBox.Text = "";
                yMin_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(zMin_TextBox) && zMin_TextBox.Text == "z min")
            {
                zMin_TextBox.Text = "";
                zMin_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(xMax_TextBox) && xMax_TextBox.Text == "x max")
            {
                xMax_TextBox.Text = "";
                xMax_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(yMax_TextBox) && yMax_TextBox.Text == "y max")
            {
                yMax_TextBox.Text = "";
                yMax_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(zMax_TextBox) && zMax_TextBox.Text == "z max")
            {
                zMax_TextBox.Text = "";
                zMax_TextBox.Foreground = SCBW;
            }
            if (sender.Equals(Speed_TextBox) && Speed_TextBox.Text == "max speed")
            {
                Speed_TextBox.Text = "";
                Speed_TextBox.Foreground = SCBW;
            }
        }

    }
}
