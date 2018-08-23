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
    /// Interaction logic for Set_Workspace_Window.xaml
    /// </summary>
    public partial class Set_Workspace_Window : Window
    {
        public delegate void Event_delegate();
        public Event_delegate Workspace_set;

        public int Xmin, Xmax, Ymin, Ymax, Zmin, Zmax;
        public Set_Workspace_Window()
        {
            InitializeComponent();
            Xmin_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            Ymin_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            Zmin_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            Xmax_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            Ymax_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            Zmax_TB.KeyDown += new KeyEventHandler(TextBoxes_KeyDown);
            W_Set_Button.Click += new RoutedEventHandler(W_Set_Button_Click);

            Xmin_TB.Text = "";
            Ymin_TB.Text = "";
            Zmin_TB.Text = "";

            Xmax_TB.Text = "";
            Ymax_TB.Text = "";
            Zmax_TB.Text = "";
        }

        private void W_Set_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Xmin_TB.Text) < Convert.ToInt32(Xmax_TB.Text) &&
                    Convert.ToInt32(Ymin_TB.Text) < Convert.ToInt32(Ymax_TB.Text) &&
                    Convert.ToInt32(Zmin_TB.Text) < Convert.ToInt32(Zmax_TB.Text))
                {
                    Xmin = Convert.ToInt32(Xmin_TB.Text);
                    Xmax = Convert.ToInt32(Xmax_TB.Text);
                    Ymin = Convert.ToInt32(Ymin_TB.Text);
                    Ymax = Convert.ToInt32(Ymax_TB.Text);
                    Zmin = Convert.ToInt32(Zmin_TB.Text);
                    Zmax = Convert.ToInt32(Zmax_TB.Text);

                    if (Workspace_set != null)
                    {
                        Workspace_set();
                    }
                } 
                else
                {
                    MessageBox.Show("Wrong workspace size");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong workspace size");
            }
        }

        private void TextBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.OemMinus))
            {
                if (e.Key == Key.OemMinus && sender.Equals(Xmin_TB))
                    if (Xmin_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
                if (e.Key == Key.OemMinus && sender.Equals(Xmax_TB))
                    if (Xmax_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
                if (e.Key == Key.OemMinus && sender.Equals(Ymin_TB))
                    if (Ymin_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
                if (e.Key == Key.OemMinus && sender.Equals(Ymax_TB))
                    if (Ymax_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
                if (e.Key == Key.OemMinus && sender.Equals(Zmin_TB))
                    if (Zmin_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
                if (e.Key == Key.OemMinus && sender.Equals(Zmax_TB))
                    if (Zmax_TB.Text.Contains("-"))
                    {
                        Cancel_Button.Focus();
                    }
            }
            else
            {
                Cancel_Button.Focus();
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            SW_Window.Close();
        }

    }
}
