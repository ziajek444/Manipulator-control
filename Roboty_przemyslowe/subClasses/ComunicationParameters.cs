using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Windows;

namespace Roboty_przemyslowe
{
    

    //The class to contain Comunication's Parameters. 
    // For each fild there is one method to set value and one method to return value.

    /// <summary>
    /// Contain full list of parameters to comunicate with Robot.
    /// </summary>
    public class ComunicationParameters
    {
        private String com_name;
        private Int32 baudrate;
        private Int16 data_bits;
        private Parity parity_bits;
        private StopBits stop_bits;
        private SerialPort port;
        private Boolean can_set_new_parameter = false;
        
        public ComunicationParameters(bool file_is_good)
        {
            if (!file_is_good) SetDefaultParameters();
            else can_set_new_parameter = true;
        }

        //setting default parametrs, is using when valid parameters are out of range => Variable "file_is_good" is false
        private void SetDefaultParameters()
        {
            parity_bits = Parity.Even;
            stop_bits = StopBits.Two;
            com_name = "COM1";
            baudrate = 9600;
            data_bits = 8;
        }


        //methods to setting basic parameters
        public void setPort()
        {
            try
            {
                this.port = new SerialPort(this.com_name, this.baudrate, this.parity_bits, this.data_bits, this.stop_bits);
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void SetComName(string new_name)
        {
            //Expected value for "Com Name"

            if (new_name.Length > 2 && new_name.Length < 7)
            {
                try
                {
                    this.com_name = new_name;
                }
                catch (Exception ex) { MessageBox.Show("I couldnt save ur Name to contener Name \n"+ex.Message, "Check ur Parameters File"); }

            }
            else MessageBox.Show("The Name have too little or too much chars", "u have to check ur param. file or fix SetComName (method)");

        }

        private void SetBaudrate(string new_baudrate)
        {
            Int32 i_var;
            try
            {
                i_var = Convert.ToInt32(new_baudrate);
                this.baudrate = i_var;
            }
            catch { MessageBox.Show("U can't convert data to Int16\n Baudrate value is default (9600)", "Error in SetBaudrate()"); this.baudrate = 9600; }

        }

        private void SetParity(string new_parity)
        {


             switch (new_parity)
            {
                case "EVEN":
                    {
                        this.parity_bits = Parity.Even;
                        break;
                    }
                case "MARK":
                    {
                        this.parity_bits = Parity.Mark;
                        break;
                    }
                case "ODD":
                    {
                        this.parity_bits = Parity.Odd;
                        break;
                    }
                case "SPACE":
                    {
                        this.parity_bits = Parity.Space;
                        break;
                    }
                case "NONE":
                    {
                        this.parity_bits = Parity.None;
                        break;
                    }
                default:
                    {
                        MessageBox.Show("There is problem in ComunicationParameters class,\n probably u schould extender ur class of new Cases\nParity value is default (EVEN)", "Doesn't expected Name");
                        break;
                    }
            }
        }

        private void SetDataBits(string new_data_b)
        {
            Int16 i_var;
            try
            {
                i_var = Convert.ToInt16(new_data_b);
                this.data_bits = i_var;
            }
            catch { MessageBox.Show("U can't convert data to Int16\n Data Bits value is default (8)", "Error in SetDataBits()"); this.data_bits = 8; }

        }

        private void SetStopBits(string new_stop_b)
        {
            switch (new_stop_b)
            {
                case "ONE":
                    {
                        this.stop_bits = StopBits.One;
                        break;
                    }
                case "TWO":
                    {
                        this.stop_bits = StopBits.Two;
                        break;
                    }
                case "ONEPOINTFIVE":
                    {
                        this.stop_bits = StopBits.OnePointFive;
                        break;
                    }
                case "OPF"://One Point Five
                    {
                        this.stop_bits = StopBits.OnePointFive;
                        break;
                    }
                case "NONE":
                    {
                        MessageBox.Show("Stop Bits in ur file are set on 'NONE' so is setting default value (Two bits)", "Default Values i set in Stop bits");
                        this.stop_bits = StopBits.Two;
                        break;
                    }
                default: { MessageBox.Show("U can't convert data to Int16\n Stop Bits value is default (TWO)", "Error in SetStopBits()"); this.stop_bits = StopBits.Two; break; }
            }
        }


        //Main Method. Recomended to use after constructor. 
        public void SetParameters(string[] file_txt)
        {
            if (can_set_new_parameter)
            {
                //Expect Com Name, Exp. "COM1"
                string str_seter = file_txt[1].Substring(10, file_txt[1].Length - 10 - 1);
                SetComName(str_seter);
                //Expect Baudrate Exp. 96900
                str_seter = file_txt[2].Substring(10, file_txt[2].Length - 10 - 1);
                SetBaudrate(str_seter);
                //Expect Parity Exp. ODD
                str_seter = file_txt[3].Substring(8, file_txt[3].Length - 8 - 1);
                SetParity(str_seter);
                //Expect Data Bits Exp. 8
                str_seter = file_txt[4].Substring(11, file_txt[4].Length - 11 - 1);
                SetDataBits(str_seter);
                //Expect Stop Bits Exp. TWO
                str_seter = file_txt[5].Substring(11, file_txt[5].Length - 11 - 1);
                SetStopBits(str_seter);

            }
            else
            {
                SetDefaultParameters();
                MessageBox.Show("Parameters of Comunication can't be set, and are Default", "Message from ComunicationParameters class");
            }
            
            setPort();
        }


        //methods to return values
        public Parity GetParityBits()
        {
            return parity_bits;
        }

        public StopBits GetStopBits()
        {
            return stop_bits;
        }

        public String GetComName()
        {
            return com_name;
        }

        public Int32 GetBaudrate()
        {
            return baudrate;
        }

        public Int16 GetDataBits()
        {
            return data_bits;
        }

        public SerialPort GetPort()
        {
            return port;
        }

       

    }
}
