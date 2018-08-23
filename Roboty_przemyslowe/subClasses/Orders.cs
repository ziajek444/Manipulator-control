using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace Roboty_przemyslowe
{
    

    /// <summary>
    /// SubClass for FindError
    /// Also Sending Split Command to FindError SuperClas
    /// </summary>
    class Orders
    {
        private string[] commands;
        private int amount;
        private Order[] o;

        /// <summary>
        /// making list of orders which will be used to corectly validate code
        /// </summary>
        public Orders()
        {
            
        }

        public void SetOrders(string[] _commands, int _amount)
        {
            this.commands = _commands;
            this.amount = _amount;
            o = new Order[amount];
            
            //Potrzebuje tablice poleceń []
            SetCommands();
            
            //potrzebuje tablice argumentow poleceń [id polecenia,ilośc argumentów] => Trzeba wypełnić "o[j].Args"
            SetArguments();

        }//SetOrders

        private void SetCommands()
        {
            //pierwsze słowo to komenda
            char c_help;
            for (int j = 0; j < amount; j++)
            {
                o[j] = new Order();
                o[j].CommandLenght = 0;
                //o[j].Index = j;
                int Long = commands[j].Length;
                for (int i = 0; i < Long; i++)
                {
                    c_help = commands[j][i];
                    if (c_help != ' ')
                    {
                        o[j].Command += c_help;//nadawanie nazwy polecenia 
                        o[j].CommandLenght++;//długość polecenia 
                    }
                    else break;
                }
                //liczba przecinków + 1 to jest liczba argumentów
                //o[j].Amount = (Long - o[j].CommandLenght) / 2;
                o[j].Amount = (CountComma(commands[j])+1); // liczba możliwości liczby argumentów

                o[j].Args = new int[o[j].Amount];//Stworzona pusta tablica dwuwymiarowa przechowywująca "o[j].Index" poleceń i "o[j].Amount" możliwości liczby argumentów
            }
        }//SetCommands

        private void SetArguments()
        {
            string str_help;
            int next = 0;
            string s_i_help = "";
            bool comma = false;

            for (int j = 0; j < amount; j++)
            {
                str_help = commands[j].Substring(o[j].CommandLenght+1);//tylko wartości po nazwie komendy + spacja np. z "MO 1,3,5,6#" wycinamy "1,3,5,6#"
                for (int i = 0; i < (o[j].Amount*10); i++)//
                {
                    if (str_help[i] == ',') comma = true;
                    
                    if (str_help[i] == '#')
                    {
                        try
                        {
                        o[j].Args[next] = Convert.ToInt32(s_i_help.ToString());
                        comma = false;
                        }
                        catch (Exception d) { MessageBox.Show(d.Message); comma = false; }

                        next = 0;
                        s_i_help = "";
                        break;
                    }
                    else
                    {
                        try
                        {
                            if (!comma)
                            {
                                s_i_help += str_help[i];
                            }
                            else
                            {
                                o[j].Args[next] = Convert.ToInt32(s_i_help.ToString());
                                comma = false;
                                s_i_help = "";
                                next++;
                                
                            }
                            
                        }
                        catch (Exception d) { MessageBox.Show(d.Message); comma = false; }
                        
                    }
                }
            }//for
        }//SetArguments

        /// <summary>
        /// Counter of comma in paragraph
        /// </summary>
        private int CountComma(string line_commands)
        {
            int amount_commas=0;
            for (int i = 0; i < line_commands.Length; i++)
            {
                if (',' == line_commands[i]) amount_commas++;
            }
                return amount_commas;
        }

        public Order[] GetOrders()
        {
            return o;//zwrot tabliy struktur, przechowywujących Komendy i mozliwość ilości argumentów
        }



    }//class


}//namespace
