using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;


namespace Roboty_przemyslowe
{

    public class User_Colors
    {
        public string Command { get; set; }
        public SolidColorBrush SCB { get; set; }
        public User_Colors(String Command, SolidColorBrush SCB)
        {
            this.Command = Command;
            this.SCB = SCB;

        }

    }

        [Serializable()]
     class HUser_Colors
    {
        public string Command { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public HUser_Colors(string Command, byte R, byte G, byte B)
        {
            this.Command = Command;
            this.R = R;
            this.G = G;
            this.B = B;
        }
    }

   public class CommandColors
    {
        List<string> Logicallist = new List<String>();
        List<string> ManipulatorMovelist = new List<String>();
        List<string> JointMovelist = new List<String>();
        List<string> Looplist = new List<String>();
        List<string> Textlist = new List<String>();
        List<string> Otherlist = new List<String>();
       
        List<User_Colors> UC = new List<User_Colors>();
        List<HUser_Colors> HUC = new List<HUser_Colors>();

        SolidColorBrush Logicallistcolor = new SolidColorBrush(Colors.Blue);
        SolidColorBrush ManipulatorMovelistcolor = new SolidColorBrush(Colors.Green);
        SolidColorBrush JointMovelistcolor = new SolidColorBrush(Colors.Red);
        SolidColorBrush Looplistcolor = new SolidColorBrush(Colors.Purple);
        SolidColorBrush Textlistcolor = new SolidColorBrush(Colors.Pink);
        SolidColorBrush Otherlistcolor = new SolidColorBrush(Colors.Orange);

        public CommandColors()
        {
            FillList();
            try
            {
                
                LoadUserListsColors();
                HUC = LoadUserCommandsColors();

                foreach (HUser_Colors a in HUC)
                {
                    CleanColorsAfterLoad(a.Command);
                    UC.Add(new User_Colors(a.Command,new SolidColorBrush(Color.FromRgb(a.R,a.G,a.B))));
                }
            }
            catch (Exception) { }
        }
        
        public void SetLogicalColor(Color logicalcolor)
        {
            Logicallistcolor.Color = logicalcolor;
            Properties.Settings.Default.CLogical = Logicallistcolor.Color;
            Properties.Settings.Default.Save();
        }

        public SolidColorBrush GetLogicalColor()
        {
            return Logicallistcolor;
        }

        public void SetManipulatorColor(Color manipulatorcolor)
        {
            ManipulatorMovelistcolor.Color = manipulatorcolor;
            Properties.Settings.Default.CManipulator = ManipulatorMovelistcolor.Color;
            Properties.Settings.Default.Save();
        }

        public SolidColorBrush GetManipulatorColor()
        {
            return ManipulatorMovelistcolor;
        }

        public void SetJointColor(Color jointcolor)
        {
            JointMovelistcolor.Color = jointcolor;
            Properties.Settings.Default.CJoint = JointMovelistcolor.Color;
            Properties.Settings.Default.Save();

        }

        public SolidColorBrush GetJointColor()
        {
            return JointMovelistcolor;
        }

        public void SetLoopsColor(Color loopcolor)
        {
            Looplistcolor.Color = loopcolor;
            Properties.Settings.Default.CLoop = Looplistcolor.Color;
            Properties.Settings.Default.Save();
        }

        public SolidColorBrush GetLoopColor()
        {
            return Looplistcolor;
        }

        public void SetTextColor(Color textcolor)
        {
            Textlistcolor.Color = textcolor;
            Properties.Settings.Default.CText = Textlistcolor.Color;
            Properties.Settings.Default.Save();
        }

        public SolidColorBrush GetTextColor()
        {
            return Textlistcolor;
        }

        public void SetOtherColor(Color differentcolor)
        {
            Otherlistcolor.Color = differentcolor;
            Properties.Settings.Default.COther = Otherlistcolor.Color;
            Properties.Settings.Default.Save();
        }

        public SolidColorBrush GetDifferentColor()
        {
            return Otherlistcolor;
        }

        public void SetUserColor(String command, Color color)
        {
            if (Logicallist.Contains(command))
                Logicallist.Remove(command);
            else if (ManipulatorMovelist.Contains(command))
                ManipulatorMovelist.Remove(command);
            else if (JointMovelist.Contains(command))
                JointMovelist.Remove(command);
            else if (Looplist.Contains(command))
                Looplist.Remove(command);
            else if (Textlist.Contains(command))
                Textlist.Remove(command);
            else if (Otherlist.Contains(command))
                Otherlist.Remove(command);
            else if (UC.Count != 0)
            {
                int i = 0;
                foreach (User_Colors a in UC )
                {
                    if (a.Command == command)
                        break;
                    else
                        i++;
                }
                UC.Remove(UC[i]);

                i = 0;
                foreach (HUser_Colors a in HUC)
                {
                    if (a.Command == command)
                        break;
                    else
                        i++;
                }
                HUC.Remove(HUC[i]);
            }

            UC.Add(new User_Colors(command, new SolidColorBrush(color)));
            HUC.Add(new HUser_Colors(command, color.R, color.G, color.B));
            SaveUserCommandsColors(HUC);
        }

        public SolidColorBrush GetColor(string variable_from_Patrick)
        {
            if (Logicallist.Contains(variable_from_Patrick))
                return Logicallistcolor;
            else if (ManipulatorMovelist.Contains(variable_from_Patrick))
                return ManipulatorMovelistcolor;
            else if (JointMovelist.Contains(variable_from_Patrick))
                return JointMovelistcolor;
            else if (Looplist.Contains(variable_from_Patrick))
                return Looplistcolor;
            else if (Textlist.Contains(variable_from_Patrick))
                return Textlistcolor;
            else if (Otherlist.Contains(variable_from_Patrick))
                return Otherlistcolor;
            else
                if (UC.Count != 0)
            {
                foreach (User_Colors a in UC)
                {
                    if (a.Command == variable_from_Patrick)
                        return a.SCB;
                }
            }
            return new SolidColorBrush();
        }

        public void SetDefaultColors()
        {
            UC.Clear();
            HUC.Clear();
            FillList();

            Properties.Settings.Default.HUserColors = null;
            Properties.Settings.Default.CLogical = Colors.Blue;
            Properties.Settings.Default.CManipulator = Colors.Green;
            Properties.Settings.Default.CJoint = Colors.Red;
            Properties.Settings.Default.CLoop = Colors.Purple;
            Properties.Settings.Default.CText = Colors.Pink;
            Properties.Settings.Default.COther = Colors.Orange;
            Properties.Settings.Default.Save();

            Logicallistcolor = new SolidColorBrush(Colors.Blue);
            ManipulatorMovelistcolor = new SolidColorBrush(Colors.Green);
            JointMovelistcolor = new SolidColorBrush(Colors.Red);
            Looplistcolor = new SolidColorBrush(Colors.Purple);
            Textlistcolor = new SolidColorBrush(Colors.Pink);
            Otherlistcolor = new SolidColorBrush(Colors.Orange);
        }

        public void LoadUserListsColors()
        {
            Color temp = new Color();
            temp = Properties.Settings.Default.CLogical;
            if (!(0 == temp.R  && 0 == temp.G  && 0 == temp.B))
            {
                Logicallistcolor.Color = Properties.Settings.Default.CLogical;
                ManipulatorMovelistcolor.Color = Properties.Settings.Default.CManipulator;
                JointMovelistcolor.Color = Properties.Settings.Default.CJoint;
                Looplistcolor.Color = Properties.Settings.Default.CLoop;
                Textlistcolor.Color = Properties.Settings.Default.CText;
                Otherlistcolor.Color = Properties.Settings.Default.COther;
            }
        }
        
        void FillList()
        {

            using (var reader = new StreamReader("Files/Logicallist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Logicallist.Add(line);
                }
            }

            using (var reader = new StreamReader("Files/ManipulatorMovelist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ManipulatorMovelist.Add(line);
                }
            }

            using (var reader = new StreamReader("Files/JointMovelist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    JointMovelist.Add(line);
                }
            }

            using (var reader = new StreamReader("Files/Looplist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Looplist.Add(line);
                }
            }

            using (var reader = new StreamReader("Files/Textlist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Textlist.Add(line);
                }
            }

            using (var reader = new StreamReader("Files/Differentlist.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Otherlist.Add(line);
                }
            }
        }

        private void CleanColorsAfterLoad(string command)
        {
            if (Logicallist.Contains(command))
                Logicallist.Remove(command);
            else if (ManipulatorMovelist.Contains(command))
                ManipulatorMovelist.Remove(command);
            else if (JointMovelist.Contains(command))
                JointMovelist.Remove(command);
            else if (Looplist.Contains(command))
                Looplist.Remove(command);
            else if (Textlist.Contains(command))
                Textlist.Remove(command);
            else if (Otherlist.Contains(command))
                Otherlist.Remove(command);
        }

        void SaveUserCommandsColors(List<HUser_Colors> Colores)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, Colores);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.HUserColors = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }

        }

        List<HUser_Colors> LoadUserCommandsColors()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.HUserColors)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<HUser_Colors>)bf.Deserialize(ms);
            }
        }

    }
}
