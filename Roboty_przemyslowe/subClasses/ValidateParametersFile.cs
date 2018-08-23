using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Roboty_przemyslowe
{

    /// <summary>
    /// Is using in Orders, and starts comunicate
    /// </summary>
    abstract class ValidateBase
    {
        protected int number_of_tests;
        protected bool validate;
        protected string[] new_file;
        
        public ValidateBase()
        {
            number_of_tests = 0;
        }

        /// <summary>
        /// Method is saving file's lines to Vector of strings, also it do a tests;
        /// </summary>
        /// <param name="file"></param>
        virtual public void SetFile(string[] file)
        {
            new_file = file;
            validate = Tests();
        }

        abstract public bool Tests();

        public bool GetValidate()
        {
            return validate;
        }

        protected void NumberOfTestsAdd()
        {
            number_of_tests++;
        }

    }

    /// <summary>
    /// Class is finding potencial errors in Parameters file
    /// </summary>
    class ValidateParametersFile : ValidateBase
    {

        override public bool Tests()
        {
            int PassedTests = 0;
            number_of_tests = 0;
            try
            {
                PassedTests = FileTest01() + FileTest02() + FileTest03() + FileTest04() + FileTest05() + FileTest06() + FileTest07() + FileTest08() + FileTest09() + FileTest10() + FileTest11() + FileTest12() + FileTest13();
            }
            catch
            {
                return false;
            }
            if (PassedTests == number_of_tests)
            {
                number_of_tests = 0;
                return true;
            }
            else return false;
        }
        
        private char[] help_chars;
        private char help_char;
        private string help_string;
        private int help_int;

        private int FileTest01()
        {
            NumberOfTestsAdd();
            if (new_file[0].Equals("Start#") && new_file[6].Equals("Stop#")) return 1;
            else return 0;
        }

        private int FileTest02()
        {
            NumberOfTestsAdd();
           
            help_chars = new_file[1].ToCharArray();
            help_string="";
            for (int i = 0; i < 10; i++) help_string += help_chars[i];
            if (help_string.Equals("Com Name: ")) return 1;
            else return 0;
           
        }

        private int FileTest03()
        {
            NumberOfTestsAdd();

            help_chars  = new_file[1].ToCharArray();
            help_string = "";
            for (int i = 10; i < 13; i++) help_string += help_chars[i];
            if (help_string.Equals("COM")) return 1;
            else return 0;
        }

        private int FileTest04()
        {
            NumberOfTestsAdd();

            if ((help_char = new_file[2].ToCharArray()[new_file[2].ToCharArray().Length - 1]) == '#' 
                && (help_char = new_file[1].ToCharArray()[14]) == '#' 
                && (help_char = new_file[3].ToCharArray()[new_file[3].ToCharArray().Length - 1]) == '#'
                && (help_char = new_file[4].ToCharArray()[new_file[4].ToCharArray().Length - 1]) == '#'
                && (help_char = new_file[5].ToCharArray()[new_file[5].ToCharArray().Length - 1]) == '#') return 1;
            else return 0;
            
        }

        private int FileTest05()
        {
            NumberOfTestsAdd();

            help_chars = new_file[2].ToCharArray();
            help_string = "";
            for (int i = 0; i < 10; i++) help_string += help_chars[i];
            if (help_string.Equals("Baudrate: ")) return 1;
            else return 0;

        }

        private int FileTest06()
        {
            NumberOfTestsAdd();

            help_chars = new_file[3].ToCharArray();
            help_string = "";
            for (int i = 0; i < 8; i++) help_string += help_chars[i];
            if (help_string.Equals("Parity: ")) return 1;
            else return 0;

        }

        private int FileTest07()
        {
            NumberOfTestsAdd();

            help_chars = new_file[4].ToCharArray();
            help_string = "";
            for (int i = 0; i < 11; i++) help_string += help_chars[i];
            if (help_string.Equals("Data Bits: ")) return 1;
            else return 0;

        }

        private int FileTest08()
        {
            NumberOfTestsAdd();

            help_chars = new_file[5].ToCharArray();
            help_string = "";
            for (int i = 0; i < 11; i++) help_string += help_chars[i];
            if (help_string.Equals("Stop Bits: ")) return 1;
            else return 0;

        }

        private int FileTest09()
        {
            NumberOfTestsAdd();

            help_string = new_file[6];
            if (help_string.Equals("Stop#")) return 1;
            else return 0;

        }

        private int FileTest10()
        {
            NumberOfTestsAdd();

            help_chars = new_file[2].ToCharArray();
            help_string = "";
            for (int i = 10; i <= help_chars.Length-2; i++) help_string += help_chars[i];
            try
            {
                help_int = Convert.ToInt32(help_string);
            }
            catch (Exception ex) { MessageBox.Show("Błąd konvertowania w teście 10", ex.Message); }

            if (help_int>0 && help_int<=1000000) return 1;
            else return 0;

        }

        private int FileTest11()
        {
            NumberOfTestsAdd();

            help_chars = new_file[3].ToCharArray();
            help_string = "";
            for (int i = 8; i <= help_chars.Length - 2; i++) help_string += help_chars[i];
            if (help_string.Equals("EVEN") || help_string.Equals("ODD") || help_string.Equals("SPACE")) return 1;
            else return 0;

        }

        private int FileTest12()
        {
            NumberOfTestsAdd();

            help_char = new_file[4].ToCharArray()[new_file[4].ToCharArray().Length - 2];
            help_string = help_char.ToString();
            try
            {
                help_int = Convert.ToInt32(help_string);
            }
            catch (Exception ex) { MessageBox.Show("Błąd konvertowania w teście 12",ex.Message); }

            if (help_int > 5 && help_int <= 9) return 1;
            else return 0;

        }

        private int FileTest13()
        {
            NumberOfTestsAdd();

            help_chars = new_file[5].ToCharArray();
            help_string = "";
            for (int i = 11; i <= help_chars.Length - 2; i++) help_string += help_chars[i];
            if (help_string.Equals("ONE") 
                || help_string.Equals("TWO") 
                || help_string.Equals("NONE")
                || help_string.Equals("OPF")
                || help_string.Equals("ONEPOINTFIVE")) return 1;
            else return 0;

        }

    }

    /// <summary>
    /// Class is finding potencial errors in Orders File
    /// </summary>
    class ValidateOrdersFile : ValidateBase
    {
        private int amount_lines;
        

        override public bool Tests()
        {
            int PassedTests = 0;
            number_of_tests = 0;
            PassedTests = FileTest01();//linijka testów
            if (PassedTests == number_of_tests)
            {
                number_of_tests = 0;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// prepare file to test, and test.
        /// </summary>
        /// <param name="file"></param>
        override public void SetFile(string[] file)
        {
            //przygotowanie stringów do testów
            //Code below is cutting blank paragraphs from included strings
            amount_lines = file.Length;
            int blank_paragraph = 0;
            for (int i = 0; i < file.Length; i++) if (file[i] == "") amount_lines--;
            string[] s = new string[amount_lines];
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] != "") s[i - blank_paragraph] = file[i];
                else blank_paragraph++;
            }
            //for (int i = 0; i < amount_lines; i++) MessageBox.Show(s[i]);

            //bazowa umiejętność metody SetFile()
            base.SetFile(s);
        }

        /// <summary>
        /// Testing file, every lines should have more than 3 chars and less than 13 chars. exp. 'MO 3' 
        /// </summary>
        /// <returns></returns>
        private int FileTest01()
        {
            NumberOfTestsAdd();

            for(int i=0;i<amount_lines;i++)
            {
                if (new_file[i].Length > 20 || new_file[i].Length < 4) return 0;
            }
            return 1;
            

        }

    }


}
