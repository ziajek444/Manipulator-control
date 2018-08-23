using Microsoft.Win32;
using System;
using System.IO;
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
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections;
using System.Threading;

namespace Roboty_przemyslowe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {

#region Variables

        Logs Logs = new Logs();
        MyMessageWindow MessageWindow;

        CheckEventCaster CECaster;
        List<Command_Search> CS;
        List<string> CommandList;
        Filtre Filtr;
        bool ProjectSaved;
        int LineNumber;

        List<PointEllipses> PointEllipse_List;
        Set_Workspace_Window S_W_Window;
        WorkspaceLimits W_Limits;
        Positioner Positions;
        _MoveType MoveType;
        double a_scale, full_scale, modify;
        bool Grid_state;
        int current_ID;

        DispatcherTimer AnimTimer;
        int Angle;

        List<ManipulatorPositions> ManipulatorPositionsList;
        Queue<String> IOChangeRequestQue = new Queue<string>();
        Ellipse[] In_Ellipses = new Ellipse[16];
        Ellipse[] Out_Ellipses = new Ellipse[16];
        SolidColorBrush IE_Off;
        SolidColorBrush IE_On;
        SolidColorBrush OE_Off;
        SolidColorBrush OE_On;
        bool Pos_Download, Pos_Upload, IORefreshing, IOReadLast;
        uint nowDownloading = 1200;
        string LastIOQueued;

        Connection_Window Connection_Window;
        RS232 Communication;

        ColorWindow Color_Window;
        CommandColors CommandColors;

        MenuItem FileMI;
        MenuItem Open;

        Queue<string> DataReceivedQueue;
        List<string> ConsoleCommandsSent;
        bool LastSent, LastReceived; 
        int command_ID;
        int RobotSpeed;

#endregion

        public MainWindow()
        {
            AnimTimer = new DispatcherTimer();
            AnimTimer.Tick += AnimTimer_Tick;
            AnimTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            AnimTimer.Start();

            Logs.LogAppend("");
            Logs.LogAppend(DateTime.Now.ToString("dd/MM/yyyy HH\\:mm tt") + "\t SESSION STARTED");


            InitializeComponent();

            #region Utilities utilities

            U_IO_Controlls_Add();
            U_RangeStart_TextBox.PreviewKeyDown += U_Range_TextBox_PreviewKeyDown;
            U_RangeFinish_TextBox.PreviewKeyDown += U_Range_TextBox_PreviewKeyDown;
            Angle = 0;
            ManipulatorPositionsList = new List<ManipulatorPositions>();
            Jojca.ItemsSource = ManipulatorPositionsList;
            
            #endregion

            #region Editor utilities

            Filtr = new Filtre();
            CS = new List<Command_Search>();
            CommandList = new List<string>();
            CECaster = new CheckEventCaster(3);
            CommandColors = new CommandColors();
            
            CECaster.CheckTick += CheckTime;
            CECaster.CheckTick2 += ConsoleReceivedUpdate;
            CECaster.StartTimer();

            Rich_TB.Document.Blocks.Clear();
            Rich_TB.CaretPosition = Rich_TB.CaretPosition.DocumentEnd;
            Rich_TB.PreviewKeyDown += Rich_TB_PreviewKeyDown;
            Rich_TB.PreviewMouseDown += Rich_TB_PreviewMouseDown;
            using (var reader = new StreamReader("Files/All_Command.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    CommandList.Add(line);
                }
            }
            LineNumber = 10;

            #endregion

            #region Point Select utilities

            PointEllipse_List = new List<PointEllipses>();
            
            Z_Slider.ValueChanged += Z_Slider_ValueChanged;
            AxisXY_Image.MouseDown += AxisXY_Image_MouseDown;

            Delete_button.Click += Delete_Button_Click;
            Set_W_button.Click += Set_Workspace_Button_Click;

            MO_Rbutton.Checked += Rbutton_Checked;
            MS_Rbutton.Checked += Rbutton_Checked;
            DS_Rbutton.Checked += Rbutton_Checked;
            MRA_Rbutton.Checked += Rbutton_Checked;

            W_Limits = new WorkspaceLimits(0, 400, 0, 400, 0, 400);
            Positions = new Positioner();

            Update_Workspace_Limits_Labels();

            Z_Slider.Maximum = W_Limits.Z_upper_limit;
            Z_Slider.Minimum = W_Limits.Z_lower_limit;
            Zvalue_label_scale_update();
            
            Positions.set_a(W_Limits.X_lower_limit, W_Limits.X_upper_limit,
                            W_Limits.Y_lower_limit, W_Limits.Y_upper_limit);

            MO_Rbutton.IsChecked = true;
            MoveType = _MoveType.MO;

            Grid_state = false;

            #endregion

            #region Menu items utilities

            FileMI = new MenuItem();
            FileMI.Header = "File";

            Open = new MenuItem();
            Open.Header = "Open";

            MenuItem Save = new MenuItem();
            Save.Header = "Save";

            MenuItem Close = new MenuItem();
            Close.Header = "Close";

            SolidColorBrush col = new SolidColorBrush();
            col.Color = System.Windows.Media.Color.FromRgb(1, 124, 205);
            Open.Background = col;
            Save.Background = col;
            Close.Background = col;

            FileMI.Items.Add(Open);
            FileMI.Items.Add(Save);
            FileMI.Items.Add(Close);

            Close.Click += Closenow;
            Save.Click += Savenow;
            Open.Click += Opennow;

            MenuItem Connection = new MenuItem();
            Connection.Header = "Connection";

            MenuItem ConnectionSettings = new MenuItem();
            ConnectionSettings.Header = "Settings";

            MenuItem ConnectAutomatically = new MenuItem();
            ConnectAutomatically.Header = "Connect Automatically";

            Connection.Items.Add(ConnectionSettings);
            Connection.Items.Add(ConnectAutomatically);

            ConnectionSettings.Click += ConnectionSettings_Click;
            ConnectAutomatically.Click += ConnectAutomatically_Click;

            MenuItem Project = new MenuItem();
            Project.Header = "Project";

            MenuItem CodeCheck = new MenuItem();
            CodeCheck.Header = "Check Code (F5)";

            MenuItem SetWorkspace = new MenuItem();
            SetWorkspace.Header = "Set Workspace and speed limits";

            Project.Items.Add(CodeCheck);
            Project.Items.Add(SetWorkspace);

            CodeCheck.Click += CodeCheck_Click;

            MenuItem Customise = new MenuItem();
            Customise.Header = "Customise";

            MenuItem SyntaxColors = new MenuItem();
            SyntaxColors.Header = "Syntax Colors";

            MenuItem WindowSkin = new MenuItem();
            WindowSkin.Header = "Window Skin";

            Customise.Items.Add(SyntaxColors);
            Customise.Items.Add(WindowSkin);

            SyntaxColors.Click += SyntaxColors_Click;
            
            Main_Menu.Items.Add(FileMI);
            Main_Menu.Items.Add(Connection);
            Main_Menu.Items.Add(Project);
            Main_Menu.Items.Add(Customise);

            ProjectSaved = true;

            #endregion

            #region Rest Window utilities
           
            My_Window.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
            Tab1.PreviewMouseDown += Tabcontrols_MouseDown;
            Tab2.PreviewMouseDown += Tabcontrols_MouseDown;
            Tab3.PreviewMouseDown += Tabcontrols_MouseDown;
            Tab4.PreviewMouseDown += Tabcontrols_MouseDown;
            My_Window.ResizeMode = ResizeMode.NoResize;

            #endregion

            #region Console utilities

            DataReceivedQueue = new Queue<string>();
            RobotSpeed = 12;
            LastSent = LastReceived = false;
            Console_richtextbox.Document.Blocks.Clear();

            Console_Xminus_button.Click += Console_DSbutton_Click;
            Console_Yminus_button.Click += Console_DSbutton_Click;
            Console_Zminus_button.Click += Console_DSbutton_Click;
            Console_Xplus_button.Click += Console_DSbutton_Click;
            Console_Yplus_button.Click += Console_DSbutton_Click;
            Console_Zplus_button.Click += Console_DSbutton_Click;
            Console_GridOpen_button.Click += Console_DSbutton_Click;
            Console_GridClose_button.Click += Console_DSbutton_Click;

            Console_J1minus_button.Click += Console_JbuttonMinus_Click;
            Console_J2minus_button.Click += Console_JbuttonMinus_Click;
            Console_J3minus_button.Click += Console_JbuttonMinus_Click;
            Console_J4minus_button.Click += Console_JbuttonMinus_Click;
            Console_J5minus_button.Click += Console_JbuttonMinus_Click;
            Console_J6minus_button.Click += Console_JbuttonMinus_Click;

            Console_J1plus_button.Click += Console_JbuttonPlus_Click;
            Console_J2plus_button.Click += Console_JbuttonPlus_Click;
            Console_J3plus_button.Click += Console_JbuttonPlus_Click;
            Console_J4plus_button.Click += Console_JbuttonPlus_Click;
            Console_J5plus_button.Click += Console_JbuttonPlus_Click;
            Console_J6plus_button.Click += Console_JbuttonPlus_Click;

            Console_Xstep_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_Ystep_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_Zstep_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;

            Console_J1step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_J2step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_J3step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_J4step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_J5step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;
            Console_J6step_tbox.PreviewKeyDown += Console_stepbox_PreviewKeyDown;

            Console_SPminus_button.Click += Console_SPbutton_Click;
            Console_SPplus_button.Click += Console_SPbutton_Click;

            ConsoleCommandsSent = new List<string>();
            command_ID = 0;
            
            #endregion
       
        }



#region Menu Items

        void CodeCheck_Click(object sender, RoutedEventArgs e)
        {
            Syntax SyntaxChecker = new Syntax();
            double[] a = new double[] { -100.0, 100.0, -100.0, 100.0, -100.0, 100.0 };
            List<String> CodeListing = new List<string>();
            Code CodeChecker = new Code();
            CodeChecker.SetSpeed(100.0);
            SyntaxChecker.SetRanges(a);
            Syntax_Error_RTB.Document.Blocks.Clear();
            int ErrorCount = 0;
            foreach (Paragraph Par in Rich_TB.Document.Blocks)
            {
                String CodeLine = new TextRange(Par.ContentStart, Par.ContentEnd).Text;
                CodeListing.Add(CodeLine);
                SyntaxChecker.CheckCode(CodeLine);
                String WhichError = SyntaxChecker.GetError().ToString();
                String ErrorInfo = SyntaxChecker.GetError_str();

                if (!(WhichError.Contains("Nothing")))
                {
                    Syntax_Error_RTB.Document.Blocks.Add(new Paragraph(new Run(ErrorInfo)));
                    ErrorCount++;
                }
            }
            String[] CodeListingArray = CodeListing.ToArray<String>();
            if (0 == ErrorCount)
            {
                CodeChecker.CheckCode(CodeListingArray);
                String ErrorInfo = CodeChecker.GetError_str();
                Syntax_Error_RTB.Document.Blocks.Add(new Paragraph(new Run(ErrorInfo)));
            }

            

        }

        void ConnectionSettings_Click(object sender, RoutedEventArgs e)
        {
            if (Connection_Window == null || !Connection_Window.IsLoaded)
            {
                Connection_Window = new Connection_Window();
                Connection_Window.Owner = Window.GetWindow(this);
                Connection_Window.Port_Open += Port_Open_Request;
                Connection_Window.Show();
            }
            
        }

        void ConnectionSettingsOpen()
        {
            if (Connection_Window == null || !Connection_Window.IsLoaded)
            {
                Connection_Window = new Connection_Window();
                Connection_Window.Owner = Window.GetWindow(this);
                Connection_Window.Port_Open += Port_Open_Request;
                Connection_Window.Show();
            }
        }

        private void Savenow(object sender, RoutedEventArgs e)
        {
            var save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Text file (*.txt)|*.txt";

            if (true == save_dialog.ShowDialog())
            {
                File.WriteAllText(save_dialog.FileName, new TextRange(Rich_TB.CaretPosition.DocumentStart, Rich_TB.CaretPosition.DocumentEnd).Text);

                int tempi = save_dialog.FileName.LastIndexOf('\\');
                string name= save_dialog.FileName.Substring(tempi + 1, save_dialog.FileName.Length - (tempi + 5));
                if (name.Length > 9)
                    Tab1_label.Content = name.Substring(0, 8) + "*";
                else
                    Tab1_label.Content = name;
                ProjectSaved = true;
            }
        }

        private bool Savenow()
        {
            var save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Text file (*.txt)|*.txt";

            if (true == save_dialog.ShowDialog())
            {
                File.WriteAllText(save_dialog.FileName, new TextRange(Rich_TB.CaretPosition.DocumentStart, Rich_TB.CaretPosition.DocumentEnd).Text);

                int tempi = save_dialog.FileName.LastIndexOf('\\');
                string name = save_dialog.FileName.Substring(tempi + 1, save_dialog.FileName.Length - (tempi + 5));
                if (name.Length > 9)
                    Tab1_label.Content = name.Substring(0, 8) + "*";
                else
                    Tab1_label.Content = name;
                ProjectSaved = true;
                return true;
            }
            return false;
        }

        private void Opennow(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.Title = "Open file...";
            if (true == OpenDialog.ShowDialog())
            {
                StreamReader SReader = new StreamReader(OpenDialog.FileName);

                Rich_TB.Document.Blocks.Clear();

                while (!SReader.EndOfStream)
                {
                    Rich_TB.Document.Blocks.Add(new Paragraph(new Run(SReader.ReadLine())));
                }

            }
        }

        private void Closenow(object sender, RoutedEventArgs e)
        {
            My_Window.Close();
        }

        void ConnectAutomatically_Click(object sender, RoutedEventArgs e)
        {
            String[] PNames = SerialPort.GetPortNames();
            if (PNames.Contains(Properties.Settings.Default.MPortName))
            {
                SerialPort Port = new SerialPort();
                Port.PortName = Properties.Settings.Default.MPortName;
                Port.BaudRate = Convert.ToInt32(Properties.Settings.Default.MBaudRate);
                if (Properties.Settings.Default.MParity == "Odd")
                {
                    Port.Parity = Parity.Odd;
                }
                else if (Properties.Settings.Default.MParity == "Even")
                {
                    Port.Parity = Parity.Even;
                }
                else
                {
                    Port.Parity = Parity.None;
                }
                Port.DataBits = Convert.ToInt32(Properties.Settings.Default.MDataBits);
                if (Properties.Settings.Default.MStopBits == "1")
                {
                    Port.StopBits = StopBits.One;
                }
                else
                    Port.StopBits = StopBits.Two;
                Port_Open_Request(Port);
            }
            else
            {
                MessageWindow = new MyMessageWindow("Lasly used port is not available.", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
                ConnectionSettingsOpen();
            }
            
        }

        void SyntaxColors_Click(object sender, RoutedEventArgs e)
        {

            Color_Window = new ColorWindow(CommandColors);
            Color_Window.Owner = this;
            Color_Window.Show();
            Color_Window.ColorsSet += ColorsSetEvent;
        }

        void ColorsSetEvent()
        {

            List<UserColorMake> TempSingleCommandColorMake = Color_Window.GetCommandColorChanges();
            List<UserColorMake> TempTypeCommandColorMake = Color_Window.GetCommandTypeColorChanges();

            CECaster.TimerReset();

            foreach (UserColorMake UCM in TempSingleCommandColorMake)
            {
                CommandColors.SetUserColor(UCM.Command, System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                CECaster.TimerReset();
            }

            foreach (UserColorMake UCM in TempTypeCommandColorMake)
            {
                if (UCM.Command == "Logical") CommandColors.SetLogicalColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                else
                    if (UCM.Command == "Manipulator") CommandColors.SetManipulatorColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                    else
                        if (UCM.Command == "Joint") CommandColors.SetJointColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                        else
                            if (UCM.Command == "Loops") CommandColors.SetLoopsColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                               else
                                if (UCM.Command == "Text") CommandColors.SetTextColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));
                                else
                                    if (UCM.Command == "Other") CommandColors.SetOtherColor(System.Windows.Media.Color.FromRgb(UCM.R, UCM.G, UCM.B));

                CECaster.TimerReset();
            }

            Color_Window.Close();

            Color_Window = new ColorWindow(CommandColors);
            Color_Window.Owner = this;
            Color_Window.Show();
            Color_Window.ColorsSet += ColorsSetEvent;
        }

#endregion

#region Point Select Lap

        void Rbutton_Checked(object sender, RoutedEventArgs e)
        {
            if (MO_Rbutton.IsChecked == true)
            {
                MoveType = _MoveType.MO;
            }
            else
                if (MS_Rbutton.IsChecked == true)
                {
                    MoveType = _MoveType.MS;
                }
                else
                    if (DS_Rbutton.IsChecked == true)
                    {
                        MoveType = _MoveType.DS;
                    }
                    else if (MRA_Rbutton.IsChecked == true)
                    {
                        MoveType = _MoveType.MRA;
                    }
        }

        private void Workspace_set()
        {
            W_Limits.X_lower_limit = S_W_Window.Xmin;
            W_Limits.X_upper_limit = S_W_Window.Xmax;
            W_Limits.Y_lower_limit = S_W_Window.Ymin;
            W_Limits.Y_upper_limit = S_W_Window.Ymax;
            W_Limits.Z_lower_limit = S_W_Window.Zmin;
            W_Limits.Z_upper_limit = S_W_Window.Zmax;
            Update_Workspace_Limits_Labels();
            Z_Slider.Maximum = W_Limits.Z_upper_limit;
            Z_Slider.Minimum = W_Limits.Z_lower_limit;
            Zvalue_label_scale_update();
            Zvalue_label.Margin = get_z_label_pos();
            Positions.set_a(W_Limits.X_lower_limit, W_Limits.X_upper_limit,
                            W_Limits.Y_lower_limit, W_Limits.Y_upper_limit);
            S_W_Window.Close();
        }

        private void Update_Workspace_Limits_Labels()
        {
            Xlow_label.Content = W_Limits.X_lower_limit.ToString();
            Xup_label.Content = W_Limits.X_upper_limit.ToString();
            Ylow_label.Content = W_Limits.Y_lower_limit.ToString();
            Yup_label.Content = W_Limits.Y_upper_limit.ToString();
            Zlow_label.Content = W_Limits.Z_lower_limit.ToString();
            Zup_label.Content = W_Limits.Z_upper_limit.ToString();

        }

        private int[] GetCursor()
        {
            int[] temp = new int[2];
            int[] temp1 = new int[3];

            System.Windows.Point a = new System.Windows.Point();
            int posx, posy;

            a = Mouse.GetPosition(AxisXY_Image);
            posx = (int)a.X;
            posy = (int)a.Y;


            if (posx >= 0 && posy >= 0 && posx <= 260 && posy <= 260)
            {
                temp = Positions.get_position_by_cursor(posx, posy);
                temp1[0] = temp[0];
                temp1[1] = temp[1];
                temp1[2] = 0;
                Point_label.Content = (temp[0].ToString() + ", " + temp[1].ToString());
            }
            else
            {
                Point_label.Content = "-, -";
                temp1[2] = -1;
            }

            return temp;
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (null != My_Listbox.SelectedItem)
            {
                bool Found = false;
                string Temp = My_Listbox.SelectedItem.ToString();

                PointEllipses PointToRemove = new PointEllipses(0, new Thickness(0, 0, 0, 0), 0, 0, 0, 0, 0, 0, 0, false, 0, 0, 0);

                foreach (PointEllipses l in PointEllipse_List)
                {
                    if (l.List_Text == Temp)
                    {
                        PointToRemove = l;
                        Found = true;
                    }
                }
                if (Found == true)
                {
                    Tab2_Grid.Children.Remove(PointToRemove.Ellipse);
                    PointEllipse_List.Remove(PointToRemove);
                    current_ID--;
                }
                My_Listbox.Items.Clear();
                for (int i = 0; i <= current_ID - 1; i++)
                {
                    PointEllipse_List[i].ID = i;
                    My_Listbox.Items.Add(PointEllipse_List[i].List_Text_update());
                }
            }

        }

        private void AxisXY_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point a = new System.Windows.Point();
            a = Mouse.GetPosition(Tab2_Grid);
            int[] temp = GetCursor();
            try
            {
                int A = Convert.ToInt32(A_TextB.Text);
                int B = Convert.ToInt32(B_TextB.Text);
                int C = Convert.ToInt32(L_TextB.Text);

                if (DS_Rbutton.IsChecked == true)
                {
                    if (current_ID != 0)
                    {
                        int lastposx = PointEllipse_List[current_ID - 1].absX;
                        int lastposy = PointEllipse_List[current_ID - 1].absY;
                        int lastposz = PointEllipse_List[current_ID - 1].absZ;

                        int X = 0, Y = 0, Z = 0;
                        
                        #region Getting Positions from Absolutes 

                        if (lastposx >= 0 && temp[0] >= 0)
                        {
                            X = temp[0] - lastposx;
                        }
                        if (lastposy > 0 && temp[1] > 0)
                        {
                            Y = temp[1] - lastposy;
                        }

                        if (lastposx >= 0 && temp[0] <= 0)
                        {
                            X = -(lastposx + Math.Abs(temp[0]));
                        }
                        if (lastposy >= 0 && temp[1] <= 0)
                        {
                            Y = -(lastposy + Math.Abs(temp[1]));
                        }

                        if (lastposx <= 0 && temp[0] >= 0)
                        {
                            X = Math.Abs(lastposx) + temp[0];
                        }
                        if (lastposy <= 0 && temp[1] >= 0)
                        {
                            Y = Math.Abs(lastposy) + temp[1];
                        }

                        if (lastposx <= 0 && temp[0] <= 0)
                        {
                            if (temp[0] < lastposx)
                                X = (Math.Abs(lastposx) - Math.Abs(temp[0]));
                            else
                                X = (Math.Abs(lastposx) + temp[0]);
                        }
                        if (lastposy <= 0 && temp[1] <= 0)
                        {
                            if (temp[1] < lastposy)
                                Y = (Math.Abs(lastposy) - Math.Abs(temp[1]));
                            else
                                Y = (Math.Abs(lastposy) + temp[1]);
                        }
                        if (lastposz > (int)Z_Slider.Value)
                        {
                            Z = -(lastposz - (int)Z_Slider.Value);
                        }
                        else
                        {
                            Z = (int)Z_Slider.Value - lastposz;
                        }
                        #endregion

                        PointEllipse_List.Add(new PointEllipses(current_ID++, new Thickness(a.X, a.Y, 0, 0), MoveType, X, Y, Z, A, B, C, Grid_state, temp[0], temp[1],lastposz + Z));

                        Tab2_Grid.Children.Add(PointEllipse_List[current_ID - 1].Ellipse);

                        My_Listbox.Items.Add(PointEllipse_List[current_ID - 1].List_Text);
                    }
                    else
                    {
                        MessageWindow = new MyMessageWindow("DS must be used after MO or MS", "command", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                        
                    }
                }
                else
                {
                    PointEllipse_List.Add(new PointEllipses(current_ID++, new Thickness(a.X, a.Y, 0, 0), MoveType, temp[0], temp[1], (int)Z_Slider.Value, A, B, C, Grid_state, temp[0], temp[1], (int)Z_Slider.Value));

                    Tab2_Grid.Children.Add(PointEllipse_List[current_ID - 1].Ellipse);

                    My_Listbox.Items.Add(PointEllipse_List[current_ID - 1].List_Text);
                }
            }
            catch (Exception)
            {
                MessageWindow = new MyMessageWindow("Wrong data input.", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }

        }

        private void Z_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Zvalue_label.Content = ((int)Z_Slider.Value).ToString();
            Zvalue_label.Margin = get_z_label_pos();
        }

        private Thickness get_z_label_pos()
        {
            return new Thickness(335, (int)(a_scale * (Z_Slider.Value + modify) + 271), 0, 0);
        }

        private void Zvalue_label_scale_update()
        {
            if (Z_Slider.Minimum < 0)
            {
                full_scale = Z_Slider.Maximum + Math.Abs(Z_Slider.Minimum);
                modify = Math.Abs(Z_Slider.Minimum);
            }
            else
            {
                full_scale = Z_Slider.Maximum - Z_Slider.Minimum;
                modify = -Z_Slider.Minimum;
            }
            a_scale = -244 / full_scale;
        }

        private void Set_Workspace_Button_Click(object sender, RoutedEventArgs e)
        {
            S_W_Window = new Set_Workspace_Window();
            S_W_Window.Workspace_set += Workspace_set;
            S_W_Window.Show();
        }

        private void DeleteAll_button_Click(object sender, RoutedEventArgs e)
        {
            foreach (PointEllipses a in PointEllipse_List)
            {
                Tab2_Grid.Children.Remove(a.Ellipse);
            }
            My_Listbox.Items.Clear();
            PointEllipse_List.Clear();
            current_ID = 0;
        }

        private void SetDefault_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Point_button_Click(object sender, RoutedEventArgs e)
        {
            int[] temp = new int[2];

            try
            {
                int X = Convert.ToInt32(X_TextB.Text);
                int Y = Convert.ToInt32(Y_TextB.Text);
                int Z = Convert.ToInt32(Z_TextB.Text);
                int A = Convert.ToInt32(A_TextB.Text);
                int B = Convert.ToInt32(B_TextB.Text);
                int C = Convert.ToInt32(L_TextB.Text);

                if (DS_Rbutton.IsChecked == true)
                {
                    int absX = PointEllipse_List[current_ID - 1].absX + X;
                    int absY = PointEllipse_List[current_ID - 1].absY + Y;
                    int absZ = PointEllipse_List[current_ID - 1].absZ + Z;

                    if (absX > W_Limits.X_upper_limit || absX < W_Limits.Y_lower_limit || absY > W_Limits.Y_upper_limit || absY < W_Limits.Y_lower_limit || absZ > W_Limits.Z_upper_limit || absZ < W_Limits.Z_lower_limit)
                    {
                        MessageWindow = new MyMessageWindow("Point exceeds the workspace.", "", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                    else
                    {
                        temp = Positions.get_position_by_scale(absX, absY);
                        PointEllipse_List.Add(new PointEllipses(current_ID++, new Thickness(temp[0] + 33, temp[1] + 33, 0, 0),
                                                                                                             MoveType, X, Y, Z, A, B, C, Grid_state, absX, absY,absZ));
                        Tab2_Grid.Children.Add(PointEllipse_List[current_ID - 1].Ellipse);

                        My_Listbox.Items.Add(PointEllipse_List[current_ID - 1].List_Text);
                    }
                }
                else
                {
                    if (X > W_Limits.X_upper_limit || X < W_Limits.Y_lower_limit || Y > W_Limits.Y_upper_limit || Y < W_Limits.Y_lower_limit || Z > W_Limits.Z_upper_limit || Z < W_Limits.Z_lower_limit)
                    {
                        MessageWindow = new MyMessageWindow("Point exceeds the workspace.", "", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                    else
                    {
                        temp = Positions.get_position_by_scale(X, Y);

                        PointEllipse_List.Add(new PointEllipses(current_ID++, new Thickness(temp[0] + 33, temp[1] + 33, 0, 0),
                                                                 MoveType, X, Y, Z, A, B, C, Grid_state, X, Y, Z));

                        Tab2_Grid.Children.Add(PointEllipse_List[current_ID - 1].Ellipse);

                        My_Listbox.Items.Add(PointEllipse_List[current_ID - 1].List_Text);
                    }
                }
            }
            catch (Exception)
            {
                MessageWindow = new MyMessageWindow("Wrong input data 2", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }


        }

        private void GOC_button_Click(object sender, RoutedEventArgs e)
        {
            if (GOC_button.Content == "G Open")
            {
                GOC_button.Content = "G Closed";
                Grid_state = true;
            }
            else
            {
                GOC_button.Content = "G Open";
                Grid_state = false;
            }
        }

        private void Generate_Code_button_Click(object sender, RoutedEventArgs e)
        {
            int line = 10;
            int PNumer = 50;
            List<string> CodeListing = new List<string>();

            foreach (PointEllipses a in PointEllipse_List)
            {
                if (a.MoveType != _MoveType.DS)
                {
                    CodeListing.Add(line.ToString() + " PD " + PNumer.ToString() + "," + a.X.ToString() + "," + a.Y.ToString() + "," + a.Z.ToString() + "," + a.A.ToString() + "," + a.B.ToString() + "," + a.C.ToString());
                    line += 10; PNumer++;
                }
            }
            PNumer = 50;
            foreach (PointEllipses a in PointEllipse_List)
            {
                if (a.MoveType == _MoveType.MO)
                {
                    if (a.G == false)
                        CodeListing.Add(line.ToString() + " MO " + (PNumer++).ToString() + "," + "O");
                    else
                        CodeListing.Add(line.ToString() + " MO " + (PNumer++).ToString() + "," + "C");
                    line += 10;

                }

                if (a.MoveType == _MoveType.MS)
                {
                    if (a.G == false)
                        CodeListing.Add(line.ToString() + " MS " + (PNumer++).ToString() + "," + "O");
                    else
                        CodeListing.Add(line.ToString() + " MS " + (PNumer++).ToString() + "," + "C");
                    line += 10;
                }
                if (a.MoveType == _MoveType.MRA)
                {
                    if (a.G == false)
                        CodeListing.Add(line.ToString() + " MRA " + (PNumer++).ToString() + "," + "O");
                    else
                        CodeListing.Add(line.ToString() + " MRA " + (PNumer++).ToString() + "," + "C");
                    line += 10;
                }
                if (a.MoveType == _MoveType.DS)
                {
                    CodeListing.Add(line.ToString() + " DS " + a.X.ToString() + "," + a.Y.ToString() + "," + a.Z.ToString());
                    line += 10;
                }

            }

            CodeGenerate_Window CG_Window = new CodeGenerate_Window(CodeListing);
            CG_Window.Show();
        }

        private void Zslider_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(ZsliderAdd_Button))
            {
                Z_Slider.Value++;
            }
            else
            {
                Z_Slider.Value--;
            }
        }

#endregion 

#region Communication

        void Port_Open_Request(SerialPort Port)
        {
            try
            {
                Communication.close();
            }
            catch
            {
            }
            Communication = new RS232();
            if (Communication.PortOpen(Port))
            {
                MessageWindow = new MyMessageWindow("Connected!", "", 0, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
                Communication.DataSent += CommunicationDataSent;
                Communication.DataReceived += CommunicationDataReceived;

                string date = DateTime.Now.ToString("dd/MM/yyyy HH\\:mm tt");
                string LogData=date + "\t Connection set:";
                Logs.LogAppend(LogData);
                Thread.Sleep(50);

                LogData = "Port name:\t" + Port.PortName.ToString();
                Logs.LogAppend(LogData);
                Thread.Sleep(50);

                LogData = "BaudRate:\t" + Port.BaudRate.ToString();
                Logs.LogAppend(LogData);
                Thread.Sleep(50);

                LogData = "Data bits:\t" + Port.DataBits.ToString();
                Logs.LogAppend(LogData);
                Thread.Sleep(50);

                LogData = "Parity:  \t" + Port.Parity.ToString();
                Logs.LogAppend(LogData);
                Thread.Sleep(50);

                LogData = "Stop bits:\t" + Port.StopBits.ToString();
                Logs.LogAppend(LogData);
                My_Window.Focus();
            }
            else
            {
                MessageWindow = new MyMessageWindow("Connection failed!", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }

            if (Connection_Window != null)
            {
                Connection_Window.Close();
            }

        }

        private void CommunicationDataReceived(string data)
        {
            DataReceivedQueue.Enqueue(data);
        }

        private void CommunicationDataSent(string data)
        {
           ConsoleSentUpdate(data);
        }

        private async void ControlsSend(string Data)
        {
            if (Pos_Download || Pos_Upload || IORefreshing)
            {
                MessageWindow = new MyMessageWindow("Upload in progress.", "Abort?", 0, 2);
                if (MessageWindow.ShowDialog()== true)
                {
                    Pos_Download = false;
                    Pos_Upload = false;
                    IORefreshing = false;
                    await Task.Delay(400);
                    U_StartDownload_Button.Content = "Download";
                    U_Upload_Button.Content = "Upload";
                    U_RefreshIO_Button.Content = "Start Refreshing";
                    nowDownloading = 1100;
                    Communication.send(Data);
                }
            }
            else
            {
                try
                {
                    Communication.send(Data);
                }
                catch (NullReferenceException)
                {
                    MessageWindow = new MyMessageWindow("Connect device before sending", "any data.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
            }
        }

        private async void Position_Download(uint StartPos, uint EndPos)
        {
            uint i = StartPos;
            while (Pos_Download == true && i <= EndPos)
            {
                try
                {
                    Communication.send("PR " + (i).ToString());
                }
                catch (NullReferenceException)
                {
                    U_StartDownload_Button.Content = "Download";
                    Pos_Download = false;
                    nowDownloading = 1100;

                    MessageWindow = new MyMessageWindow("Connect device before sending", "any data.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();

                    break;
                }
                nowDownloading = i;
                i++;
                await Task.Delay(500);
            }
            U_StartDownload_Button.Content = "Download";
            Pos_Download = false;
            nowDownloading = 1100;
        }

        private async void Position_Upload()
        {
            int i = 0;
            while (Pos_Upload == true && i < ManipulatorPositionsList.Count)
            {
                try
                {
                    Communication.send("PD " + ManipulatorPositionsList[i].ID.ToString() + ", " + ManipulatorPositionsList[i].X.ToString().Replace(",", ".") + ", " + ManipulatorPositionsList[i].Y.ToString().Replace(",", ".") + ", " + ManipulatorPositionsList[i].Z.ToString().Replace(",", ".")
                         + ", " + ManipulatorPositionsList[i].A.ToString().Replace(",", ".") + ", " + ManipulatorPositionsList[i].B.ToString().Replace(",", ".") + ", " + ManipulatorPositionsList[i].L1.ToString().Replace(",", ".") + ", 0.0, " + ManipulatorPositionsList[i].RL + ", " + ManipulatorPositionsList[i].AB + ", " + ManipulatorPositionsList[i].OC);
                    await Task.Delay(500);
                }
                catch (NullReferenceException)
                {
                    Pos_Upload = false;
                    U_Upload_Button.Content = "Upload";

                    MessageWindow = new MyMessageWindow("Connect device before sending", "any data.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();

                    break;
                }
                i++;
            }
            Pos_Upload = false;
            U_Upload_Button.Content = "Upload";
        }

        private async void IOInfoRefresh()
        {
            while (IORefreshing == true)
            {
                try
                {
                    Communication.send("ID");
                    await Task.Delay(500);
                    Communication.send("DR");
                    await Task.Delay(500);

                    if (IOChangeRequestQue.Count != 0)
                    {
                        for (int i = 0; i < IOChangeRequestQue.Count; i++)
                        {
                            String Data = IOChangeRequestQue.Dequeue();
                            Communication.send(Data);
                            await Task.Delay(500);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    IORefreshing = false;
                    U_RefreshIO_Button.Content = "Start Refreshing";

                    MessageWindow = new MyMessageWindow("Connect device before sending", "any data.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();

                    break;
                }
            }
            IORefreshing = false;
            U_RefreshIO_Button.Content = "Start Refreshing";
        }

#endregion

#region Main Editor Lap

        private void Rich_TB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            CECaster.TimerReset();
            if (e.Key == Key.Enter)
            {
                int Offset = Rich_TB.CaretPosition.GetOffsetToPosition(Rich_TB.CaretPosition.DocumentEnd);
                SingleLineCheck();
                InsertLineNumber();
                GetCaretBackAtPlace(Offset);
                e.Handled = true;
            }
        }

        private void Rich_TB_KeyDown(object sender, KeyEventArgs e)
        {
            CECaster.TimerReset();
            ProjectSaved = false;
        }

        private void Rich_TB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CECaster.TimerReset();
        }

        private void CheckTime()
        {
            GetCursor();
            if (Rich_TB.Selection.IsEmpty)
            {
                int Offset = Rich_TB.CaretPosition.GetOffsetToPosition(Rich_TB.CaretPosition.DocumentEnd);
                SingleLineCheck();
                GetCaretBackAtPlace(Offset);
            }
        }

        private void InsertLineNumber()
        {
            int NewLength = new TextRange(Rich_TB.CaretPosition.Paragraph.ContentStart, Rich_TB.CaretPosition.Paragraph.ContentEnd).Text.Length;
            Block CurrentBlock = Rich_TB.CaretPosition.Paragraph;
            string tempS = new TextRange(CurrentBlock.ContentStart, CurrentBlock.ContentEnd).Text;
            int LN = -100;

            for (int i = 1; i <= 100000000; i++)
            {
                try
                {
                    LN = Convert.ToInt32(tempS.Substring(0, i));
                }
                catch (Exception)
                {
                    break;
                }
            }

            if (LN == -100)
            {
                Rich_TB.Document.Blocks.InsertAfter(CurrentBlock, new Paragraph(new Run(LineNumber.ToString() + " ")));
                LineNumber += 10;
            }
            else
            {
                Rich_TB.Document.Blocks.InsertAfter(CurrentBlock, new Paragraph(new Run((LN = LN + 10).ToString() + " ")));
                LineNumber = LN + 10;
            }
        }

        private void SingleLineCheck()
        {
            try
            {
                Block CurrentBlock = Rich_TB.CaretPosition.Paragraph;
                string OriginalText = new TextRange(CurrentBlock.ContentStart, CurrentBlock.ContentEnd).Text;
                OriginalText = Filtr.SetCapitals(OriginalText);
                OriginalText = Filtr.SetBoxCheck(OriginalText);
                Command_Search CommandS = new Command_Search("", "", new SolidColorBrush());
                bool flag = false;
                for (int i = CommandList.Count - 1; i >= 0; i--)
                {
                    String command = CommandList[i];
                    if (OriginalText.Contains(" " + command + " ") && flag == false)
                    {
                        CommandS = new Command_Search(OriginalText, command, CommandColors.GetColor(command));
                        flag = true;
                    }
                    if (OriginalText.Contains(command + " ") && flag == false)
                    {
                        if (OriginalText.IndexOf(command) == 0)
                        {
                            CommandS = new Command_Search(OriginalText, command, CommandColors.GetColor(command));
                            flag = true;
                        }
                    }
                    if (OriginalText.Contains(" " + command) && flag == false)
                    {
                        CommandS = new Command_Search(OriginalText, command, CommandColors.GetColor(command));
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    CommandS = new Command_Search(OriginalText, "", new SolidColorBrush());
                }

                Rich_TB.CaretPosition.Paragraph.Inlines.Clear();


                Paragraph Ptemp = CommandS.get_Command_Line();
                Ptemp.Margin = new Thickness(0, 0, 0, 0);
                Rich_TB.CaretPosition.Paragraph.Margin = new Thickness(0, 0, 0, 0);

                List<System.Windows.Documents.Inline> Inlines = Ptemp.Inlines.ToList();
                foreach (System.Windows.Documents.Inline Inline in Inlines)
                {
                    Rich_TB.CaretPosition.Paragraph.Inlines.Add(Inline);
                }

            }
            catch (Exception) { }

        }

        private void GetCaretBackAtPlace(int PreviousOffset)
        {

            if (PreviousOffset == 2)
            {
                Rich_TB.CaretPosition = Rich_TB.CaretPosition.DocumentEnd;
            }
            else
            {
                Rich_TB.CaretPosition = Rich_TB.CaretPosition.DocumentStart;
                int MaxOffset = Rich_TB.CaretPosition.GetOffsetToPosition(Rich_TB.CaretPosition.DocumentEnd);
                int PossiblePos = MaxOffset - PreviousOffset;

                if (PossiblePos <= 0)
                {
                    Rich_TB.CaretPosition = Rich_TB.CaretPosition.DocumentStart;
                }
                else
                {

                    Rich_TB.CaretPosition = Rich_TB.CaretPosition.GetPositionAtOffset(PossiblePos);

                    while (Rich_TB.CaretPosition.GetOffsetToPosition(Rich_TB.CaretPosition.DocumentEnd) != PreviousOffset)
                    {
                        try
                        {
                            Rich_TB.CaretPosition = Rich_TB.CaretPosition.GetPositionAtOffset(++PossiblePos);
                        }
                        catch (Exception) { break; }
                    }
                }
            }
        }

#endregion

#region Console Lap
 
        private void Console_stepbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Delete))
            {
                Console_GridOpen_button.Focus();
            }
        }

        private void Console_textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (Communication.isopen())
                    {
                        if (Console_textbox.Text != "")
                        {
                            ControlsSend(Console_textbox.Text);
                            ConsoleCommandsSent.Add(Console_textbox.Text);
                            command_ID = ConsoleCommandsSent.Count;
                            Console_textbox.Text = String.Empty;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageWindow = new MyMessageWindow("Set and open connection before sending ", "data to robot.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }

            }
            if (e.Key == Key.Up)
            {
                if ((command_ID - 1) >= 1)
                {
                    command_ID--;
                    Console_textbox.Text = ConsoleCommandsSent[command_ID - 1];
                    Console_textbox.CaretIndex = Console_textbox.Text.Length;
                }
            }
            if (e.Key == Key.Down)
            {
                if ((command_ID + 1) <= ConsoleCommandsSent.Count)
                {
                    command_ID++;
                    Console_textbox.Text = ConsoleCommandsSent[command_ID - 1];
                    Console_textbox.CaretIndex = Console_textbox.Text.Length;
                }
            }
        }

        private void ConsoleReceivedUpdate()
        {
            if(null != DataReceivedQueue)
            if (DataReceivedQueue.Count != 0)
            {
                for (int i = 0; i < DataReceivedQueue.Count; i++)
                {
                    string receiveds = DataReceivedQueue.Dequeue();
                    
                    #region PointListUpdate
                    if (nowDownloading < 1000)
                    {
                        bool Updated = false;
                        for(int j=0; j< ManipulatorPositionsList.Count;j++)
                        {
                            if (ManipulatorPositionsList[j].ID == nowDownloading)
                            {
                                ManipulatorPositionsList[j] = new ManipulatorPositions(receiveds, nowDownloading);
                                Updated = true;
                                break;
                            }
                        }
                        if (Updated == false)
                        {
                            ManipulatorPositionsList.Add(new ManipulatorPositions(receiveds,nowDownloading));
                            
                            ManipulatorPositions[] MPtab = ManipulatorPositionsList.ToArray();
                            Array.Sort(MPtab, ManipulatorPositions.sortIDAscending());
                            ManipulatorPositionsList = MPtab.ToList();
                            Jojca.ItemsSource = ManipulatorPositionsList;
                        }

                        Jojca.Items.Refresh();
                    }
                    #endregion

                    #region IOUpdate

                    if (IORefreshing == true)
                    {
                        IOReadLast = true;
                        U_IOEllispesRefresh(receiveds);
                    }
                    else if (IOReadLast == true && receiveds.Contains("&H"))
                    {
                        IOReadLast = false;
                        U_IOEllispesRefresh(receiveds);
                    }
                    else
                    {
                        IOReadLast = false;
                    }
                    #endregion

                    if (LastReceived != true)
                    {
                        LastSent = false;
                        LastReceived = true;

                        Run Rtemp = new Run("Robot's response:");
                        Rtemp.Foreground = new SolidColorBrush(Colors.MediumVioletRed);
                        Paragraph Ptemp = new Paragraph(Rtemp);
                        Ptemp.Margin = new Thickness(0, 0, 0, 0);
                        Console_richtextbox.Document.Blocks.Add(Ptemp);
                        Logs.LogAppend(new TextRange(Ptemp.ContentStart,Ptemp.ContentEnd).Text);

                        string Stemp = DateTime.Now.ToString("MM/dd/yyyy HH\\:mm tt");
                        Rtemp = new Run(Stemp);
                        Rtemp.FontSize = 10;
                        Ptemp.Inlines.Add(Rtemp);
                        Rtemp = new Run("    " + receiveds);
                        Rtemp.Foreground = new SolidColorBrush(Colors.White);
                        Ptemp = new Paragraph(Rtemp);

                        Ptemp.Margin = new Thickness(20, 0, 0, 0);
                        Console_richtextbox.Document.Blocks.Add(Ptemp);
                        Logs.LogAppend("\t" + new TextRange(Ptemp.ContentStart, Ptemp.ContentEnd).Text);
                        Console_richtextbox.ScrollToEnd();

                    }
                    else
                    {
                        string Stemp = DateTime.Now.ToString("MM/dd/yyyy HH\\:mm tt");
                        Run Rtemp = new Run("    " + Stemp);
                        Rtemp.Foreground = new SolidColorBrush(Colors.White);
                        Paragraph Ptemp = new Paragraph(Rtemp);
                        Rtemp.FontSize = 10;
                        Rtemp = new Run(receiveds);                        
                        Ptemp.Inlines.Add(Rtemp);

                        Ptemp.Margin = new Thickness(20, 0, 0, 0);
                        Console_richtextbox.Document.Blocks.Add(Ptemp);
                        Logs.LogAppend("\t" + new TextRange(Ptemp.ContentStart, Ptemp.ContentEnd).Text);
                        Console_richtextbox.ScrollToEnd();
                    }
                }
            }
        }

        private void ConsoleSentUpdate(string data)
        {
            if (LastSent != true)
            {
                LastSent = true;
                LastReceived = false;

                Run Rtemp = new Run("Sent to Robot:");
                Rtemp.Foreground = new SolidColorBrush(Colors.Blue);
                Paragraph Ptemp = new Paragraph(Rtemp);
                Ptemp.Margin = new Thickness(0, 0, 0, 0);
                Console_richtextbox.Document.Blocks.Add(Ptemp);
                Logs.LogAppend(new TextRange(Ptemp.ContentStart, Ptemp.ContentEnd).Text);


                string Stemp = DateTime.Now.ToString("dd/MM/yyyy HH\\:mm tt");
                Rtemp = new Run(Stemp);
                Rtemp.Foreground = new SolidColorBrush(Colors.White);
                Rtemp.FontSize = 10;
                Ptemp = new Paragraph(Rtemp);
                Rtemp = new Run("    " + data);               
                Ptemp.Inlines.Add(Rtemp);
                Ptemp.Margin = new Thickness(20, 0, 0, 0);
                Console_richtextbox.Document.Blocks.Add(Ptemp);
                Logs.LogAppend("\t" + new TextRange(Ptemp.ContentStart, Ptemp.ContentEnd).Text);
                Console_richtextbox.ScrollToEnd();
            }
            else
            {
                string Stemp = DateTime.Now.ToString("dd/MM/yyyy HH\\:mm tt");
                Run Rtemp = new Run(Stemp);
                Rtemp.Foreground = new SolidColorBrush(Colors.White);
                Rtemp.FontSize = 10;
                Paragraph Ptemp = new Paragraph(Rtemp);
                Rtemp = new Run("    " + data);                
                Ptemp.Inlines.Add(Rtemp);
                Ptemp.Margin = new Thickness(20, 0, 0, 0);
                Console_richtextbox.Document.Blocks.Add(Ptemp);
                Logs.LogAppend("\t" + new TextRange(Ptemp.ContentStart, Ptemp.ContentEnd).Text);
                Console_richtextbox.ScrollToEnd();
            }
        }

        private void Console_DSbutton_Click(object sender, RoutedEventArgs e)
        {
            bool temp1 = false;
            try
            {
                int Xstep = Convert.ToInt32(Console_Xstep_tbox.Text);
                int Ystep = Convert.ToInt32(Console_Ystep_tbox.Text);
                int Zstep = Convert.ToInt32(Console_Zstep_tbox.Text);
                if (Xstep < 101 && Ystep < 101 && Zstep < 101)
                    temp1 = true;
                String Data = "";

                if (Communication.isopen() == true && temp1 == true)
                {
                    if (sender.Equals(Console_Xplus_button))
                    {
                        Data = ("DS " + Xstep.ToString() + ",0,0");
                    }
                    else
                        if (sender.Equals(Console_Xminus_button))
                        {
                            Data = ("DS " + (-Xstep).ToString() + ",0,0");
                        }
                        else
                            if (sender.Equals(Console_Yplus_button))
                            {
                                Data = ("DS 0," + Ystep.ToString() + ",0");
                            }
                            else
                                if (sender.Equals(Console_Yminus_button))
                                {
                                    Data = ("DS 0," + (-Ystep).ToString() + ",0");
                                }
                                else
                                    if (sender.Equals(Console_Zplus_button))
                                    {
                                        Data = ("DS 0,0," + Zstep.ToString());
                                    }
                                    else
                                        if (sender.Equals(Console_Zminus_button))
                                        {
                                            Data = ("DS 0,0," + (-Zstep).ToString());
                                        }
                                        else
                                            if (sender.Equals(Console_GridOpen_button))
                                            {

                                                Data = ("GO");
                                            }
                                            else
                                                if (sender.Equals(Console_GridClose_button))
                                                {
                                                    Data = ("GC");
                                                }

                    ControlsSend(Data);
                }
                else
                {
                    if (temp1 == true)
                    {
                        MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                    else
                    {
                        MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                }
            }
            catch
            {
                if (temp1 == true)
                {
                    MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
                else
                {
                    MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
            }
        }

        private void Console_JbuttonPlus_Click(object sender, RoutedEventArgs e)
        {
            bool temp1 = false;
            try
            {
                int j1step = Convert.ToInt32(Console_J1step_tbox.Text);
                int j2step = Convert.ToInt32(Console_J2step_tbox.Text);
                int j3step = Convert.ToInt32(Console_J3step_tbox.Text);
                int j4step = Convert.ToInt32(Console_J4step_tbox.Text);
                int j5step = Convert.ToInt32(Console_J5step_tbox.Text);
                int j6step = Convert.ToInt32(Console_J6step_tbox.Text);
                String Data="";

                if (j1step < 101 && j2step < 101 && j3step < 101 && j4step < 101 && j5step < 101 && j6step < 101)
                    temp1 = true;

                if (Communication.isopen() && temp1)
                {
                    if (sender.Equals(Console_J1plus_button))
                    {
                        Data = "DJ 1," + j1step.ToString();
                    }
                    if (sender.Equals(Console_J2plus_button))
                    {
                        Data = "DJ 2," + j2step.ToString();
                    }
                    if (sender.Equals(Console_J3plus_button))
                    {
                        Data = "DJ 3," + j3step.ToString();
                    }
                    if (sender.Equals(Console_J4plus_button))
                    {
                        Data = "DJ 4," + j4step.ToString();
                    }
                    if (sender.Equals(Console_J5plus_button))
                    {
                        Data = ("DJ 5," + j5step.ToString());
                    }
                    if (sender.Equals(Console_J6plus_button))
                    {
                        Data = "DJ 5," + j6step.ToString();
                    }

                    ControlsSend(Data);
                }
                else
                {
                    if (true == temp1)
                    {
                        MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                    else
                    {
                        MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                }
            }
            catch
            {
                if (temp1 == true)
                {
                    MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
                else
                {
                    MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
            }
        }

        private void Console_JbuttonMinus_Click(object sender, RoutedEventArgs e)
        {
            bool temp1 = false;
            try
            {
                int j1step = Convert.ToInt32(Console_J1step_tbox.Text);
                int j2step = Convert.ToInt32(Console_J2step_tbox.Text);
                int j3step = Convert.ToInt32(Console_J3step_tbox.Text);
                int j4step = Convert.ToInt32(Console_J4step_tbox.Text);
                int j5step = Convert.ToInt32(Console_J5step_tbox.Text);
                int j6step = Convert.ToInt32(Console_J6step_tbox.Text);

                if (j1step < 101 && j2step < 101 && j3step < 101 && j4step < 101 && j5step < 101 && j6step < 101)
                    temp1 = true;
                String Data = "";

                if (Communication.isopen() && temp1)
                {
                    if (sender.Equals(Console_J1minus_button))
                    {
                        Data = "DJ 1," + (-j1step).ToString();
                    }
                    if (sender.Equals(Console_J2minus_button))
                    {
                        Data = "DJ 2," + (-j2step).ToString();
                    }
                    if (sender.Equals(Console_J3minus_button))
                    {
                        Data = "DJ 3," + (-j3step).ToString();
                    }
                    if (sender.Equals(Console_J4minus_button))
                    {
                        Data = "DJ 4," + (-j4step).ToString();
                    }
                    if (sender.Equals(Console_J5minus_button))
                    {
                        Data = "DJ 5," + (-j5step).ToString();
                    }
                    if (sender.Equals(Console_J6minus_button))
                    {
                        Data = "DJ 5," + (-j6step).ToString();
                    }
                    ControlsSend(Data);
                }
                else
                {
                    if (true == temp1)
                    {
                        MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                    else
                    {
                        MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                        MessageWindow.Owner = this;
                        MessageWindow.ShowDialog();
                    }
                }
            }
            catch
            {
                if (temp1 == true)
                {
                    MessageWindow = new MyMessageWindow("Set and open connection before using", "this panel.", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
                else
                {
                    MessageWindow = new MyMessageWindow("Insert correct step values below 100", "", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
            }
        }

        private void Console_SPbutton_Click(object sender, RoutedEventArgs e)
        {
            String Data = "";
            if (sender.Equals(Console_SPminus_button))
            {
                if (RobotSpeed > 0)
                {
                    RobotSpeed--;
                    Data = "SP " + RobotSpeed.ToString();
                    CurrentSpeed_label.Content = "Current speed: " + RobotSpeed.ToString();
                    ControlsSend(Data);
                }
            }
            if (sender.Equals(Console_SPplus_button))
            {
                if (RobotSpeed < 30)
                {
                    RobotSpeed++;
                    Data = "SP " + RobotSpeed.ToString();
                    CurrentSpeed_label.Content = "Current speed: " + RobotSpeed.ToString();
                    ControlsSend(Data);
                }
            }
            
        }

#endregion

#region Min, Close Buttons

        private void LEL_Click(object sender, RoutedEventArgs e)
        {
            My_Window.WindowState = WindowState.Minimized;           
        }
     
        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            My_Window.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (false == ProjectSaved)
            {
                MessageWindow = new MyMessageWindow("Project changes haven't been saved.", "Save before closing?", 1, 3);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
                int MsbResult = MessageWindow.customDialogResult;
                switch (MsbResult)
                {
                    case 1:
                        {
                            if (false == Savenow())
                                e.Cancel = true;
                            break;
                        }
                    case 2:
                        {
                            e.Cancel = true;
                            break;
                        }
                    case 0:
                        {
                            if (S_W_Window != null)
                                S_W_Window.Close();
                            break;
                        }
                }
            }
        }

#endregion

#region Utilities Lap

        private void U_IO_Controlls_Add()
        {
            IE_Off = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 0, 0));
            IE_On = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 21, 21));
            OE_On = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 223, 0));
            OE_Off = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 100, 0));

            for (int i = 0; i < 16; i++)
            {
                Out_Ellipses[i] = new Ellipse();
                Out_Ellipses[i].Width = 25;
                Out_Ellipses[i].Height = 25;
                Out_Ellipses[i].VerticalAlignment = VerticalAlignment.Top;
                Out_Ellipses[i].HorizontalAlignment = HorizontalAlignment.Left;
                Out_Ellipses[i].Fill = OE_Off;
                Out_Ellipses[i].Margin = new Thickness(650, 30 + i * 30, 0, 0);
                Out_Ellipses[i].MouseDown += U_OutEllipses_MouseDown;
                Tab4_Grid.Children.Add(Out_Ellipses[i]);
            }

            for (int i = 0; i < 16; i++)
            {
                In_Ellipses[i] = new Ellipse();
                In_Ellipses[i].Width = 25;
                In_Ellipses[i].Height = 25;
                In_Ellipses[i].VerticalAlignment = VerticalAlignment.Top;
                In_Ellipses[i].HorizontalAlignment = HorizontalAlignment.Left;
                In_Ellipses[i].Fill = IE_Off;
                In_Ellipses[i].Margin = new Thickness(620, 30 + i * 30, 0, 0);
                Tab4_Grid.Children.Add(In_Ellipses[i]);
            }


        }

        private void U_Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Pos_Download != true && ManipulatorPositionsList.Count != 0 && Pos_Upload != true && IORefreshing != true)
            {
                U_Upload_Button.Content = "Stop Upload";
                Pos_Upload = true;
                Position_Upload();
            }
            else if (Pos_Upload == true)
            {
                U_Upload_Button.Content = "Upload";
                Pos_Upload = false;
            }
            else if (Pos_Download == true || IORefreshing == true)
            {
                MessageWindow = new MyMessageWindow("Other communication in progress.", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }
            else
            {
                MessageWindow = new MyMessageWindow("No points to send.", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }
        }

        private void U_Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            ManipulatorPositionsList.Clear();
            Jojca.Items.Refresh();
        }

        private void U_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }

        private void U_StartDownload_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Pos_Download != true && Pos_Upload != true && IORefreshing != true)
            {
                uint Start = 0, Finish = 0;
                try
                {
                    Start = Convert.ToUInt32(U_RangeStart_TextBox.Text);
                    Finish = Convert.ToUInt32(U_RangeFinish_TextBox.Text);
                }
                catch (Exception)
                {
                    
                }
                if (Finish >= Start && Finish < 1000 && Start < 1000)
                {
                    U_StartDownload_Button.Content = "Stop download";
                    Pos_Download = true;
                    Position_Download(Start, Finish);
                }
                else
                {
                    MessageWindow = new MyMessageWindow("Wrong positions range.", "", 1, 1);
                    MessageWindow.Owner = this;
                    MessageWindow.ShowDialog();
                }
            }
            else if (Pos_Download == true)
            {
                Pos_Download = false;
                U_StartDownload_Button.Content = "Download";
            }
            else
            {
                MessageWindow = new MyMessageWindow("Other communication in progress.", "", 1, 1);
                MessageWindow.Owner = this;
                MessageWindow.ShowDialog();
            }
        }

        private void U_AddPoint_Button_Click(object sender, RoutedEventArgs e)
        {
            int Count;
            ManipulatorPositions[] MPtab = ManipulatorPositionsList.ToArray();
            Array.Sort(MPtab, ManipulatorPositions.sortIDAscending());
            ManipulatorPositionsList = MPtab.ToList();

            if ((Count = ManipulatorPositionsList.Count) != 0)
            {
                ManipulatorPositionsList.Add(new ManipulatorPositions(ManipulatorPositionsList[Count - 1].ID+1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, "R", "A", "O"));
            }
            else
            {
                ManipulatorPositionsList.Add(new ManipulatorPositions(1, 0, 0, 0, 0, 0, 0, "R", "A", "O"));
            }

            Jojca.ItemsSource = ManipulatorPositionsList;
            Jojca.Items.Refresh();
            Jojca.SelectedItem = Jojca.Items[Count];
        }

        private void U_Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            int index = Jojca.SelectedIndex;

            if (index >= 0)
            {
                ManipulatorPositionsList.Remove(ManipulatorPositionsList[index]);
            }
            Jojca.Items.Refresh();
            Jojca.SelectedIndex = Jojca.Items.Count - 1;
        }

        private void U_RefreshIO_Button_Click(object sender, RoutedEventArgs e)
        {
            if (IORefreshing != true && Pos_Download != true && Pos_Upload != true)
            {
                U_RefreshIO_Button.Content = "Stop Refreshing";
                IORefreshing = true;
                IOInfoRefresh();
                
            }
            else if (IORefreshing == true)
            {
                IORefreshing = false;
                U_RefreshIO_Button.Content = "Start Refreshing";
            }
            else
            {
                MessageBox.Show("Other communication in progress");
            }
        }

        private void U_OutEllipses_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Communication != null && Communication.isopen() && IORefreshing == true)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (sender.Equals(Out_Ellipses[i]))
                    {
                        if (Out_Ellipses[i].Fill == OE_Off)
                        {
                            if (LastIOQueued != "OB +" + i.ToString())
                            {
                                IOChangeRequestQue.Enqueue(LastIOQueued = "OB +" + i.ToString());
                                Out_Ellipses[i].Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 200, 0));
                                Out_Ellipses[i].StrokeThickness = 3;
                            }
                        }
                        else
                        {
                            if (LastIOQueued != "OB -" + i.ToString())
                            {
                                IOChangeRequestQue.Enqueue(LastIOQueued = "OB -" + i.ToString());
                                Out_Ellipses[i].Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 100, 0));
                                Out_Ellipses[i].StrokeThickness = 3;
                            }
                        }
                    }
                }
            }
        }

        private void U_Range_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Delete))
            {
                U_StartDownload_Button.Focus();
            }
        }

        private bool[] hex2bin(string hex)
        {
            int decValue = Convert.ToInt32(hex, 16);
            bool[] binary = new bool[16];

            for (int i = 0; i <= 15; i++)
            {
                if (decValue % 2 == 1)
                {
                    binary[i] = true;
                }
                else
                    binary[i] = false;

                decValue = decValue / 2;
            }
            return binary;
        }

        private void U_IOEllispesRefresh(string data)
        {
            int index = data.LastIndexOf("H");
            bool[] Outputs = hex2bin(data.Substring(index + 1, 4));
            for (int i = 0; i < 16; i++)
            {
                if (Outputs[i] == true)
                {
                    Out_Ellipses[i].Fill = OE_On;
                }
                else
                {
                    Out_Ellipses[i].Fill = OE_Off;
                }
            }

            bool[] Inputs = hex2bin(data.Substring(3, 4));
            for (int i = 0; i < 16; i++)
            {
                if (Inputs[i] == true)
                {
                    In_Ellipses[i].Fill = IE_On;
                }
                else
                {
                    In_Ellipses[i].Fill = IE_Off;
                }
            }

        }

#endregion

#region Others

        private void Tabcontrols_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush white = new SolidColorBrush();
            SolidColorBrush black = new SolidColorBrush();
            white.Color = Colors.White;
            black.Color = Colors.Black;

            if (sender.Equals(Tab1))
            {
                Tab1_label.Foreground = white;
                Tab2_label.Foreground = Tab3_label.Foreground = Tab4_label.Foreground = black;

            }
            if (sender.Equals(Tab2))
            {
                Tab2_label.Foreground = white;
                Tab1_label.Foreground = Tab3_label.Foreground = Tab4_label.Foreground = black;
            }
            if (sender.Equals(Tab3))
            {
                Tab3_label.Foreground = white;
                Tab1_label.Foreground = Tab2_label.Foreground = Tab4_label.Foreground = black;
            }
            if (sender.Equals(Tab4))
            {
                Tab4_label.Foreground = white;
                Tab1_label.Foreground = Tab2_label.Foreground = Tab3_label.Foreground = black;
            }
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (Pos_Download || Pos_Upload || IORefreshing)
            {
                Angle += 10;
                if (Angle == 360)
                    Angle = 0;

                RotateTransform rt = new RotateTransform(Angle);
                Download_Image.RenderTransform = rt;
            }
        }
#endregion

        private void My_Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                CodeCheck_Click(new object() ,new RoutedEventArgs());
            }
        }

    }

    public class ManipulatorPositions : IComparable
    {
        private class sortIDAscendingHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                ManipulatorPositions MP1 = (ManipulatorPositions)a;
                ManipulatorPositions MP2 = (ManipulatorPositions)b;

                if (MP1.ID > MP2.ID)
                {
                    return 1;
                }
                else if (MP2.ID > MP1.ID)
                {
                    return -1;
                }
                else return 0;

            }
        }


        public uint ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double L1 { get; set; }
        public String RL { get; set; }
        public String AB { get; set; }
        public String OC { get; set; }
        public String SourceData { get; set; }

        public ManipulatorPositions()
        {

        }
        public ManipulatorPositions(uint ID, double X, double Y, double Z, double A, double B, double L, String RL, String AB, String OC)
        {
            this.ID = ID;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A;
            this.B = B;
            this.L1 = L;
            this.RL = RL;
            this.AB = AB;
            this.OC = OC;
        }
        public ManipulatorPositions(string data, uint ID)
        {
            this.ID = ID;
            String[] Source = data.Split(',');
            for (int i = 0; i < Source.Length; i++)
            {
                Source[i] = Source[i].Replace(".", ",");
            }
            try
            {
                this.X = Convert.ToDouble(Source[0]);
                this.Y = Convert.ToDouble(Source[1]);
                this.Z = Convert.ToDouble(Source[2]);
                this.A = Convert.ToDouble(Source[3]);
                this.B = Convert.ToDouble(Source[4]);
                this.L1 = Convert.ToDouble(Source[5]);
                this.RL = Source[7];
                this.AB = Source[8];
                this.OC = Source[9];
            }
            catch (Exception) { }
        }
        public bool ManipulatorPositionsFromString(string data, uint ID)
        {
            if (Validate(data) == true)
            {
                this.ID = ID;

                String[] Source = data.Split(',');
                for (int i = 0; i < Source.Length; i++)
                {
                    Source[i] = Source[i].Replace(".", ",");
                }
                this.X = Convert.ToDouble(Source[0]);
                this.Y = Convert.ToDouble(Source[1]);
                this.Z = Convert.ToDouble(Source[2]);
                this.A = Convert.ToDouble(Source[3]);
                this.B = Convert.ToDouble(Source[4]);
                this.L1 = Convert.ToDouble(Source[5]);
                this.RL = Source[7];
                this.AB = Source[8];
                this.OC = Source[9];

                return true;
            }
            return false;
        }
        public bool CheckIfEquals(string data)
        {
            if (SourceData!= null && SourceData == data)
                return true;

            return false;
        }
        static bool Validate(string data)
        {
           String[] Source = data.Split(',');
            if(Source.Length == 10)
                return true;
            else
                return false;
        }
        int IComparable.CompareTo(object obj)
        {
            ManipulatorPositions MP = (ManipulatorPositions)obj;
            return String.Compare(this.ID.ToString(), MP.ID.ToString());
        }
        public static IComparer sortIDAscending()
       {
            return (IComparer)new sortIDAscendingHelper();
       }
    }

    public class Logs
    {
        private System.Threading.Mutex mut = new System.Threading.Mutex();
        private StreamWriter LogSW;

        public async void LogAppend(string data)
        {
            await Task.Run(() => WriteLogData(data));
        }

        private void WriteLogData(string data)
        {
            mut.WaitOne();
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string date = DateTime.Now.ToString("yyyy/MM/dd");
            string FileTittle = "Logs\\" + date + "_Logs.txt";
            if (!File.Exists(FileTittle))
            {
                File.CreateText(FileTittle).Close();
            }

            LogSW = File.AppendText(FileTittle);
            
            LogSW.WriteLine(data);
            LogSW.Flush();
            LogSW.Close();
            mut.ReleaseMutex();
        }
    }

}