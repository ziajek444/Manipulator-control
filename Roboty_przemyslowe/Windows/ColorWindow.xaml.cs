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
using System.IO;

namespace Roboty_przemyslowe
{
    /// <summary>
    /// Interaction logic for ColorWindow.xaml
    /// </summary>
    public class UserColorMake
    {
        public byte R;
        public byte G;
        public byte B;
        public string Command;

        public UserColorMake(string Command, byte R, byte G, byte B)
        {
            this.Command = Command;
            this.R = R;
            this.G = G;
            this.B = B;
        }
    }

    class ItemIndex
    {
        public string text;
        public int index;


        public ItemIndex(string text, int index)
        {
            this.text = text;
            this.index = index;
        }
    }

    public partial class ColorWindow : Window
    {
        public delegate void EventDelegate();
        public EventDelegate ColorsSet;

        List<string> Commandlist = new List<string>();
        List<UserColorMake> SingleCommandColorList = new List<UserColorMake>();
        List<UserColorMake> CommandTypeColorList = new List<UserColorMake>();
        List<UserColorMake> ChosenCommandList = new List<UserColorMake>();
        List<UserColorMake> ChosenCommandTypeList = new List<UserColorMake>();
        List<ItemIndex> Correct_Indexes = new List<ItemIndex>();
        List<ItemIndex> Correct_Type_Indexes = new List<ItemIndex>();

        SolidColorBrush TestGrid1Brush = new SolidColorBrush();
        SolidColorBrush TestGrid2Brush = new SolidColorBrush();

        CommandColors ActualColors;

        byte TG1B_Actual_R = 0;
        byte TG1B_Actual_G = 0;
        byte TG1B_Actual_B = 0;

        byte TG2B_Actual_R = 0;
        byte TG2B_Actual_G = 0;
        byte TG2B_Actual_B = 0;

        int Slider1Changing = 0;
        int Slider2Changing = 0;

        public ColorWindow(CommandColors ActualColors)
        {
            InitializeComponent();

            ColorWindowWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            SliderR.ValueChanged += Sliders1_Value_Changed;
            SliderG.ValueChanged += Sliders1_Value_Changed;
            SliderB.ValueChanged += Sliders1_Value_Changed;

            SliderR1.ValueChanged += Sliders2_Value_Changed;
            SliderG1.ValueChanged += Sliders2_Value_Changed;
            SliderB1.ValueChanged += Sliders2_Value_Changed;

            SetButton.Click += SetButton_Click;

            Chosencommandtype_Listbox.SelectionChanged += Chosencommandtype_Listbox_SelectionChanged;

            using (var reader = new StreamReader("Files/All_Command.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Commandlist.Add(line);
                }
            }

            foreach (string command in Commandlist)
            {
                SolidColorBrush b = new SolidColorBrush();
                b = ActualColors.GetColor(command);
                SingleCommandColorList.Add(new UserColorMake(command, b.Color.R, b.Color.G, b.Color.B));
            }

            int i = 0;

            foreach (UserColorMake c in SingleCommandColorList)
            {

                Commandlist_Listbox.Items.Add(c.Command.ToString() + "\t" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString());
                Correct_Indexes.Add(new ItemIndex(c.Command.ToString() + "\t" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString(), i++));
            }

          

            SolidColorBrush a = ActualColors.GetLogicalColor();
            CommandTypeColorList.Add(new UserColorMake("Logical", a.Color.R, a.Color.G, a.Color.B));

            a = ActualColors.GetManipulatorColor();
            CommandTypeColorList.Add(new UserColorMake("Manipulator", a.Color.R, a.Color.G, a.Color.B));

            a = ActualColors.GetJointColor();
            CommandTypeColorList.Add(new UserColorMake("Joint", a.Color.R, a.Color.G, a.Color.B));

            a = ActualColors.GetLoopColor();
            CommandTypeColorList.Add(new UserColorMake("Loops", a.Color.R, a.Color.G, a.Color.B));

            a = ActualColors.GetTextColor();
            CommandTypeColorList.Add(new UserColorMake("Text", a.Color.R, a.Color.G, a.Color.B));

            a = ActualColors.GetDifferentColor();
            CommandTypeColorList.Add(new UserColorMake("Other", a.Color.R, a.Color.G, a.Color.B));

            int index = 0;

            foreach (UserColorMake c in CommandTypeColorList)
            {
                Commandtypelist_Listbox.Items.Add(c.Command.ToString() + "\t" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString());
                Correct_Type_Indexes.Add(new ItemIndex(c.Command.ToString() + "\t" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString(), index++));
            }
        }

        #region Color Returns

        public List<UserColorMake> GetCommandColorChanges()
        {
            return ChosenCommandList;
        }

        public List<UserColorMake> GetCommandTypeColorChanges()
        {
            return ChosenCommandTypeList;
        }

        #endregion

        #region Window Functions

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != ColorsSet)
            {
                ColorsSet();
            }

        }

        private void Sliders2_Value_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Slider2Changing > 0)
            { Slider2Changing--; }
            else
            {
                if (Chosencommandtype_Listbox.SelectedIndex >= 0)
                {
                    if (sender.Equals(SliderR1))
                    {
                        TG2B_Actual_R = (byte)SliderR1.Value;
                        TestGrid2Brush.Color = Color.FromRgb(TG2B_Actual_R, TG2B_Actual_G, TG2B_Actual_B);
                    }

                    if (sender.Equals(SliderG1))
                    {
                        TG2B_Actual_G = (byte)SliderG1.Value;
                        TestGrid2Brush.Color = Color.FromRgb(TG2B_Actual_R, TG2B_Actual_G, TG2B_Actual_B);
                    }

                    if (sender.Equals(SliderB1))
                    {
                        TG2B_Actual_B = (byte)SliderB1.Value;
                        TestGrid2Brush.Color = Color.FromRgb(TG2B_Actual_R, TG2B_Actual_G, TG2B_Actual_B);
                    }

                    ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].R = TG2B_Actual_R;
                    ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].G = TG2B_Actual_G;
                    ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].B = TG2B_Actual_B;

                    int tempindex = Chosencommandtype_Listbox.SelectedIndex;
                    List<string> templist = new List<string>();
                    foreach (string k in Chosencommandtype_Listbox.Items)
                    {
                        templist.Add(k);
                    }
                    Chosencommandtype_Listbox.Items.Clear();
                    int i = 0;
                    foreach (string k in templist)
                    {
                        if (i == tempindex)
                        {
                            Chosencommandtype_Listbox.Items.Add(ChosenCommandTypeList[tempindex].Command.ToString() + "\t" + ChosenCommandTypeList[tempindex].R.ToString() + "," + ChosenCommandTypeList[tempindex].G.ToString() + "," + ChosenCommandTypeList[tempindex].B.ToString());
                        }
                        else
                        {
                            Chosencommandtype_Listbox.Items.Add(k);
                        }
                        i++;
                    }

                    Chosencommandtype_Listbox.SelectedIndex = tempindex;

                    TestGrid2.Background = TestGrid2Brush;
                }
            }
        }
        private void Sliders1_Value_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Slider1Changing > 0)
            { Slider1Changing--; }
            else
            {
                if (Chosencommand_Listbox.SelectedIndex >= 0)
                {
                    if (sender.Equals(SliderR))
                    {
                        TG1B_Actual_R = (byte)SliderR.Value;
                        TestGrid1Brush.Color = Color.FromRgb(TG1B_Actual_R, TG1B_Actual_G, TG1B_Actual_B);
                    }

                    if (sender.Equals(SliderG))
                    {
                        TG1B_Actual_G = (byte)SliderG.Value;
                        TestGrid1Brush.Color = Color.FromRgb(TG1B_Actual_R, TG1B_Actual_G, TG1B_Actual_B);
                    }

                    if (sender.Equals(SliderB))
                    {
                        TG1B_Actual_B = (byte)SliderB.Value;
                        TestGrid1Brush.Color = Color.FromRgb(TG1B_Actual_R, TG1B_Actual_G, TG1B_Actual_B);
                    }

                    ChosenCommandList[Chosencommand_Listbox.SelectedIndex].R = TG1B_Actual_R;
                    ChosenCommandList[Chosencommand_Listbox.SelectedIndex].G = TG1B_Actual_G;
                    ChosenCommandList[Chosencommand_Listbox.SelectedIndex].B = TG1B_Actual_B;

                    int tempindex = Chosencommand_Listbox.SelectedIndex;
                    List<string> templist = new List<string>();
                    foreach (string k in Chosencommand_Listbox.Items)
                    {
                        templist.Add(k);
                    }
                    Chosencommand_Listbox.Items.Clear();
                    int i = 0;
                    foreach (string k in templist)
                    {
                        if (i == tempindex)
                        {
                            Chosencommand_Listbox.Items.Add(ChosenCommandList[tempindex].Command.ToString() + "\t" + ChosenCommandList[tempindex].R.ToString() + "," + ChosenCommandList[tempindex].G.ToString() + "," + ChosenCommandList[tempindex].B.ToString());
                        }
                        else
                        {
                            Chosencommand_Listbox.Items.Add(k);
                        }
                        i++;
                    }

                    Chosencommand_Listbox.SelectedIndex = tempindex;

                    TestGrid1.Background = TestGrid1Brush;
                }
            }
        }

        private void Chosencommand_Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Chosencommand_Listbox.SelectedIndex != -1)
            {
                Slider1Changing = 3;

                SliderR.Value = (int)ChosenCommandList[Chosencommand_Listbox.SelectedIndex].R;
                TG1B_Actual_R = ChosenCommandList[Chosencommand_Listbox.SelectedIndex].R;

                SliderG.Value = (int)ChosenCommandList[Chosencommand_Listbox.SelectedIndex].G;
                TG1B_Actual_G = ChosenCommandList[Chosencommand_Listbox.SelectedIndex].G;

                SliderB.Value = (int)ChosenCommandList[Chosencommand_Listbox.SelectedIndex].B;
                TG1B_Actual_B = ChosenCommandList[Chosencommand_Listbox.SelectedIndex].B;

                TestGrid1Brush = new SolidColorBrush(Color.FromRgb(TG1B_Actual_R, TG1B_Actual_G, TG1B_Actual_B));

                TestGrid1.Background = TestGrid1Brush;

            }
        }
        private void Chosencommandtype_Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Chosencommandtype_Listbox.SelectedIndex != -1)
            {
                Slider2Changing = 3;

                SliderR1.Value = (int)ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].R;
                TG2B_Actual_R = ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].R;

                SliderG1.Value = (int)ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].G;
                TG2B_Actual_G = ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].G;

                SliderB1.Value = (int)ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].B;
                TG2B_Actual_B = ChosenCommandTypeList[Chosencommandtype_Listbox.SelectedIndex].B;

                TestGrid2Brush = new SolidColorBrush(Color.FromRgb(TG2B_Actual_R, TG2B_Actual_G, TG2B_Actual_B));

                TestGrid2.Background = TestGrid2Brush;


            }
        }

        private void Choosetype_button_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;

            try
            {
                foreach (ItemIndex x in Correct_Type_Indexes)
                {
                    if (Commandtypelist_Listbox.SelectedItem.ToString() == x.text)
                    {
                        index = x.index;
                    }
                }
                Chosencommandtype_Listbox.Items.Add(Commandtypelist_Listbox.SelectedItem);
                Commandtypelist_Listbox.Items.Remove(Commandtypelist_Listbox.SelectedItem);
                ChosenCommandTypeList.Add(new UserColorMake(CommandTypeColorList[index].Command, CommandTypeColorList[index].R, CommandTypeColorList[index].G, CommandTypeColorList[index].B));
            }
            catch (Exception)
            { }


        }
        private void Choose_button_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            try
            {
                foreach (ItemIndex x in Correct_Indexes)
                {
                    if (Commandlist_Listbox.SelectedItem.ToString() == x.text)
                    {
                        index = x.index;
                    }
                }
                Chosencommand_Listbox.Items.Add(Commandlist_Listbox.SelectedItem);
                Commandlist_Listbox.Items.Remove(Commandlist_Listbox.SelectedItem);
                ChosenCommandList.Add(new UserColorMake(SingleCommandColorList[index].Command, SingleCommandColorList[index].R, SingleCommandColorList[index].G, SingleCommandColorList[index].B));
            }

            catch (Exception)
            { }
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            ColorWindowWindow.Close();
        }
        #endregion
    }
}
