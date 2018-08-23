using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Roboty_przemyslowe
{
    class Command_Search
    {
        private string _Text_Line;
        public string Text_Line
        {
            get { return _Text_Line; }
            set { _Text_Line = value; }
        }

        private string _Command;
        public string Command
        {
            get { return _Command; }
            set { _Command = value; }
        }
        
        SolidColorBrush CommandColor;
              
        public Command_Search(string Text_Line, string Command, SolidColorBrush CommandColor)
        {
            this.Text_Line = Text_Line;
            this.Command = Command;
            this.CommandColor = CommandColor; 
        }

        public Paragraph get_Command_Line()
        {
            if (Command != "")
            {
                int command_index = Text_Line.IndexOf(Command);
                Run Part_1 = new Run(Text_Line.Substring(0, command_index));
                Run Part_2 = new Run();
                try
                {
                    if (Text_Line.Substring(command_index + Command.Length, 1) == " ")
                    {
                        Part_2 = new Run(Text_Line.Substring(command_index, Command.Length));
                    }
                    else
                    {
                        Part_2 = new Run(Text_Line.Substring(command_index, Command.Length));
                    }
                }
                catch (Exception)
                {
                    Part_2 = new Run(Text_Line.Substring(command_index, Command.Length));
                }
                Part_2.Foreground = CommandColor;
                Bold bold = new Bold(Part_2);
                Run Part_3 = new Run(Text_Line.Substring(command_index + Command.Length, Text_Line.Length - (command_index + Command.Length)));
                Paragraph a = new Paragraph();
                a.Inlines.Add(Part_1);
                a.Inlines.Add(bold);
                a.Inlines.Add(Part_3);
                return a;

            }
            else
            {
                Run Part_1 = new Run(Text_Line);
                Paragraph a = new Paragraph();
                a.Inlines.Add(Part_1);
                return a;
            }
            
        }


    }
}
