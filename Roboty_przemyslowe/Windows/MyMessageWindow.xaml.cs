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
    /// Interaction logic for MyMessageWindow.xaml
    /// </summary>
    public partial class MyMessageWindow : Window
    {
        Button Ok_button, Yes_Button, No_Button, Cancel_Button;
        BitmapImage EXC, OK;

        public int customDialogResult { get; private set; }

        public MyMessageWindow(string lab1,string lab2, int type ,int buttons)
        {
            InitializeComponent();
            Message_Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Mssage_Label1.Content = lab1;
            Mssage_Label2.Content = lab2;
            
            EXC = new BitmapImage();
            EXC.BeginInit();
            EXC.UriSource = new Uri("/Roboty_przemyslowe;component/graphic/excla.jpg", UriKind.Relative);
            EXC.EndInit();


            OK = new BitmapImage();
            OK.BeginInit();
            OK.UriSource = new Uri("/Roboty_przemyslowe;component/graphic/excla.jpg", UriKind.Relative);
            OK.EndInit();

            if (type == 0)
            {

                WindowType_Image.Source = OK;
            }
            else
            {

                WindowType_Image.Source = EXC;
            }

            if (buttons == 1)
            {
                Ok_button = new Button();
                Ok_button.Height = 35;
                Ok_button.Width = 65;
                Ok_button.Content = "OK";
                Ok_button.VerticalAlignment = VerticalAlignment.Top;
                Ok_button.HorizontalAlignment = HorizontalAlignment.Left;
                Ok_button.Margin = new Thickness(148, 67, 0, 0);
                Ok_button.Background = new SolidColorBrush(Color.FromRgb(36,36,36));
                Ok_button.Foreground = new SolidColorBrush(Colors.White);
                Content_Grid.Children.Add(Ok_button);
                Ok_button.Focus();
                Ok_button.Click += Ok_button_Click;

                Close_Button.Click += Ok_button_Click;
            }
            else
                if (buttons == 2)
                {
                    Yes_Button = new Button();
                    Yes_Button.Height = 35;
                    Yes_Button.Width = 65;
                    Yes_Button.Content = "Yes";
                    Yes_Button.VerticalAlignment = VerticalAlignment.Top;
                    Yes_Button.HorizontalAlignment = HorizontalAlignment.Left;
                    Yes_Button.Margin = new Thickness(110, 67, 0, 0);
                    Yes_Button.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                    Yes_Button.Foreground = new SolidColorBrush(Colors.White);
                    Content_Grid.Children.Add(Yes_Button);

                    Yes_Button.Click += Yes_Button_Click;

                    No_Button = new Button();
                    No_Button.Height = 35;
                    No_Button.Width = 65;
                    No_Button.Content = "No";
                    No_Button.VerticalAlignment = VerticalAlignment.Top;
                    No_Button.HorizontalAlignment = HorizontalAlignment.Left;
                    No_Button.Margin = new Thickness(190, 67, 0, 0);
                    No_Button.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                    No_Button.Foreground = new SolidColorBrush(Colors.White);
                    Content_Grid.Children.Add(No_Button);
                    No_Button.Focus();

                    No_Button.Click += No_Button_Click;
                    Close_Button.Click += No_Button_Click;
                } 
                else
                    {
                        Yes_Button = new Button();
                        Yes_Button.Height = 35;
                        Yes_Button.Width = 65;
                        Yes_Button.Content = "Yes";
                        Yes_Button.VerticalAlignment = VerticalAlignment.Top;
                        Yes_Button.HorizontalAlignment = HorizontalAlignment.Left;
                        Yes_Button.Margin = new Thickness(78, 67, 0, 0);
                        Yes_Button.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                        Yes_Button.Foreground = new SolidColorBrush(Colors.White);
                        Content_Grid.Children.Add(Yes_Button);

                        Yes_Button.Click += Yes_Button_Click;

                        No_Button = new Button();
                        No_Button.Height = 35;
                        No_Button.Width = 65;
                        No_Button.Content = "No";
                        No_Button.VerticalAlignment = VerticalAlignment.Top;
                        No_Button.HorizontalAlignment = HorizontalAlignment.Left;
                        No_Button.Margin = new Thickness(148, 67, 0, 0);
                        No_Button.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                        No_Button.Foreground = new SolidColorBrush(Colors.White);
                        Content_Grid.Children.Add(No_Button);

                        No_Button.Click += No_Button_Click;


                        Cancel_Button = new Button();
                        Cancel_Button.Height = 35;
                        Cancel_Button.Width = 65;
                        Cancel_Button.Content = "Cancel";
                        Cancel_Button.VerticalAlignment = VerticalAlignment.Top;
                        Cancel_Button.HorizontalAlignment = HorizontalAlignment.Left;
                        Cancel_Button.Margin = new Thickness(218, 67, 0, 0);
                        Cancel_Button.Background = new SolidColorBrush(Color.FromRgb(36, 36, 36));
                        Cancel_Button.Foreground = new SolidColorBrush(Colors.White);
                        Content_Grid.Children.Add(Cancel_Button);
                        Cancel_Button.Focus();

                        Cancel_Button.Click += Cancel_Button_Click;
                        Close_Button.Click += Cancel_Button_Click;

                    }

        }


        void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            customDialogResult = 2;
            Message_Window.Close();
        }

        void No_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            customDialogResult = 0;
            Message_Window.Close();
        }

        void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            customDialogResult = 1;
            Message_Window.Close();
        }

        void Ok_button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Message_Window.Close();
        }

    }
}
