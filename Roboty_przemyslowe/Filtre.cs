using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;//Regex

namespace Roboty_przemyslowe
{
    class Filtre
    {

        public Filtre()
        { }

        private string[] box_temp;

        public string[] SetBox(string[] box)
        {
            this.box_temp = box;
            string pattern = "\\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            string[] bo = new string[2];

            for (int i = 0; i < box_temp.Length; i++)
            {
                if (box_temp[i] != null)
                {
                    box_temp[i] = regex.Replace(box_temp[i], replacement);
                    box_temp[i] = box_temp[i].Replace(" ,", ",");
                    box_temp[i] = box_temp[i].Replace(", ", ",");
                    box_temp[i] = box_temp[i].Replace(",", ", ");
                    box_temp[i] = box_temp[i].TrimEnd();
                    box_temp[i] = box_temp[i].TrimStart();
                }
                else
                {
                    box_temp[i] = "";
                }
            }

            return box_temp;
        }

        public string[] SetBoxCheck(string[] box) //dla patryka
        {
            this.box_temp = box;
            string pattern = "\\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            string[] bo = new string[2];

            for (int i = 0; i < box_temp.Length; i++)
            {
                if (box_temp[i] != null)
                {
                    box_temp[i] = regex.Replace(box_temp[i], replacement);
                    box_temp[i] = box_temp[i].Replace(" ,", ",");
                    box_temp[i] = box_temp[i].Replace(", ", ",");
                    box_temp[i] = box_temp[i].Replace(",", ", ");
                    //box_temp[i] = box_temp[i].TrimEnd();
                    box_temp[i] = box_temp[i].TrimStart();
                }
                else
                {
                    box_temp[i] = "";
                }
            }

            return box_temp;
        }

        public string[] SetCapitals(string[] _box)
        {
            string[] s_help = _box;
            for (int i = 0; i < s_help.Length; i++) s_help[i] = s_help[i].ToUpper();

            return s_help;
        }

        public string SetCapitals(string _box)
        {
            string s_help = _box;
            for (int i = 0; i < s_help.Length; i++) s_help = s_help.ToUpper();

            return s_help;
        }


        private string box_temp2;

        public string SetBox(string box)
        {
            this.box_temp2 = box;
            string pattern = "\\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            string[] bo = new string[2];

            if (box_temp2 != null)
            {
                box_temp2 = regex.Replace(box_temp2, replacement);
                box_temp2 = box_temp2.Replace(" ,", ",");
                box_temp2 = box_temp2.Replace(", ", ",");
                box_temp2 = box_temp2.Replace(",", ", ");
                box_temp2 = box_temp2.TrimEnd();
                box_temp2 = box_temp2.TrimStart();
            }
            else
            {
                box_temp2 = "";
            }
            

            return box_temp2;
        }

        public string SetBoxCheck(string box)
        {
            this.box_temp2 = box;
            string pattern = "\\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            string[] bo = new string[2];

            if (box_temp2 != null)
            {
                box_temp2 = regex.Replace(box_temp2, replacement);
                box_temp2 = box_temp2.Replace(" ,", ",");
                box_temp2 = box_temp2.Replace(", ", ",");
                box_temp2 = box_temp2.Replace(",", ", ");
                //box_temp2 = box_temp2.TrimEnd();
                box_temp2 = box_temp2.TrimStart();
            }
            else
            {
                box_temp2 = "";
            }


            return box_temp2;
        } //dla patryka


    }
}
