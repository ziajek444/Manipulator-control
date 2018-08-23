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
    /// Interaction logic for CodeGenerate_Window.xaml
    /// </summary>
    public partial class CodeGenerate_Window : Window
    {
        public CodeGenerate_Window(List <string> PointList)
        {
            InitializeComponent();

            Code_RTbox.Document.Blocks.Clear();
            Code_RTbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            foreach (string a in PointList)
            {
                Code_RTbox.Document.Blocks.Add(new Paragraph(new Run(a)));
            }
        }
    }
}
