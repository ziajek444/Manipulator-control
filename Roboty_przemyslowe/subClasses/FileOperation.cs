using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Roboty_przemyslowe
{
    /// <summary>
    /// SubClass to finding and incuding files ->.txt
    /// SubClass for Orders.class, ...
    /// Mainly convert .txt to string[]
    /// 
    /// Returning string[] and amount of lines
    /// </summary>
    public class FileOperation
    {
        protected string path;
        protected String[] lines;
        protected int number_of_lines;
        protected bool egsist;

        /// <summary>
        /// This method is finding file.txt by string path and save every lines of file in "lines" (string[])
        /// Also converting files'lines to paragraphs type of vectors string
        /// </summary>
        /// <param name="path">this is the path of ur file</param>
        public void FindFile(string path)
        {
            //create private variable for base method
            int i = 0;
            //save path to protected path. U can work on ur path.
            this.path = path;
            egsist = false;

            if (File.Exists(path))
            {
                StreamReader sr = File.OpenText(path);
                string str_s;
                while ((str_s=sr.ReadLine()) != null && str_s != "") i++;
                this.number_of_lines = i;

                lines = new String[number_of_lines];
                sr.Close();
                sr = File.OpenText(path);
                for (i = 0; i < number_of_lines; i++) lines[i] = sr.ReadLine();
                sr.Close();
                egsist = true;

            }
            else { MessageBox.Show("There is not a file " + path.ToString(), "File not found"); egsist = false; }


        }

        /// <summary>
        /// Returned Strings vector from file (every lines wthout blank lines)
        /// </summary>
        /// <returns></returns>
        public String[] GetText()
        {
            return lines;
        }

        /// <summary>
        /// returned amount not blank paragraphs of file.
        /// </summary>
        /// <returns></returns>
        public int GetAmountOfLines()
        {
            return number_of_lines;
        }

        public bool IsEgsisting()
        {
            return egsist;
        }

    }

}
