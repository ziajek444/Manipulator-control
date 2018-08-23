using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Roboty_przemyslowe
{
    abstract class FindError
    {
        /// <summary>
        /// Help Fields 
        protected Orders orders;
        protected const string LAST_WORD = "ED";
        /// </summary>

        /// <summary>
        /// Valid Fields
        protected dynamic error_type;
        protected bool valued;
        protected  static List<double[]> PD_list = new List<double[]>();

        /// </summary>

        protected Filtre filtre = new Filtre();
        

        public FindError()
        {
            valued = false;
            
        }
        abstract public void CheckCode(string _paragraph);
        abstract public void CheckCode(string[] _paragraphs);
        abstract public string GetError_str();
        virtual public bool IsError()
        {
            return valued;
        }
        virtual public dynamic GetError()
        {
            return error_type;
        }

       
    }

    /// <summary>
    /// Looking for errors, mistakes and all bugs at the every line.
    /// Recomended to set ranges before run CheckCode method
    /// </summary>
    class Syntax : FindError
    {
        /// <summary>
        /// Wyświetla komunikaty o poprawności kodu
        private string MessageFromCheckingCode(ErrorTypeParagraph ETP, int _line_number, string language)
        {
            if (language == "PL")
            {
                if (ETP == ErrorTypeParagraph.Nothing) return "line: " + _line_number.ToString() + " W porządku.";
                else if (ETP == ErrorTypeParagraph.ThereIsNotAnyOrder) return "line: " + _line_number.ToString() + ", W tej lini nie ma żadnej znanej komendy.";
                else if (ETP == ErrorTypeParagraph.SpaceAfterComma) return "line: " + _line_number.ToString() + ", Program nie filtruje poprawnie tej lini,\n(Brak spacji po przecinku).";
                else if (ETP == ErrorTypeParagraph.NotNumber) return "line: " + _line_number.ToString() + ", W tej lini nie ma indeksu (numeru lini).";
                else if (ETP == ErrorTypeParagraph.ExistedOrder) return "line: " + _line_number.ToString() + ", W tym miejscu powinna pojawić się jakaś komenda.";
                else if (ETP == ErrorTypeParagraph.Syntax) return "line: " + _line_number.ToString() + ", Nie poprawna ilość argumentów dla tej komendy.";
                else if (ETP == ErrorTypeParagraph.RangeBorder) return "line: " + _line_number.ToString() + ", Twój program pozwala robotowi wyjść poza dopuszczalny zakres, \nspróbuj rozszerzyć zakresy albo zmienić kod.";
                else return "Nie mam pojecia co sie stało Hyba mam wirus i zaraz umre, BIOS wymaga zatankowania !! ";
            }
            else
            {
                if (ETP == ErrorTypeParagraph.Nothing) return "linia: " + _line_number.ToString() + ", Program is Good";
                else if (ETP == ErrorTypeParagraph.ThereIsNotAnyOrder) return "linia: " + _line_number.ToString() + ", There is no any known command in this line";
                else if (ETP == ErrorTypeParagraph.SpaceAfterComma) return "linia: " + _line_number.ToString() + ", The program does not properly filter this line,\n(No spaces after the decimal point).";
                else if (ETP == ErrorTypeParagraph.NotNumber) return "linia: " + _line_number.ToString() + " There is no index in this line (line's number)";
                else if (ETP == ErrorTypeParagraph.ExistedOrder) return "linia: " + _line_number.ToString() + ", There schould be any command.";
                else if (ETP == ErrorTypeParagraph.Syntax) return "linia: " + _line_number.ToString() + ", Not the correct number of arguments for this command";
                else if (ETP == ErrorTypeParagraph.RangeBorder) return "linia: " + _line_number.ToString() + ", Your program allows the robot to go beyond the permissible range, \njust try to extend the ranges or change the code";
                else return "Oh Boy, i don't know what happened, Fatal Error !! No connection with Processor... Try Again...";
            }
        }


        /// <summary>
        /// Zmienne testowe
        /// </summary>
        FileOperation FO2 = new FileOperation();
        /// <summary>
        /// Zmienne testowe
        /// </summary>
        ValidateOrdersFile VOF = new ValidateOrdersFile();
        /// <summary>
        /// Zmienne testowe
        /// </summary>
        Orders ORD = new Orders();

        private string paragraph;
        private int index;
        private int amount_parameters;
        protected int line_number;
        private double[] PDs = new double[4];
        private bool pd_exisisting = false;
        private bool mo_exisisting = false;
        private bool ds_exisisting = false;
        private double[] actual_rage;//x, y, z;
        /// <summary>
        /// 0:Xmin 1:Xmax 2:Ymin 3:Ymax 4:Zmin 5:Zmax
        /// </summary>
        private double[] ranges;//x_min, x_max, y_min, y_max, z_min, z_max;
        private bool first_set_ranges_done;
        private string actual_command;//including actual command from this paragraph
        private string _LANG;
 
        public string LANG
        {
        get
        {
            return _LANG;
        }
        set
        {
            _LANG = value;
        }
    }
        
        
        public Syntax()
        {
            actual_rage = new double[3];
            first_set_ranges_done = false;
            LANG = "ENG";

            FO2.FindFile("orders.txt");
            VOF.SetFile(FO2.GetText());
            if (VOF.GetValidate())
            {
                ORD.SetOrders(FO2.GetText(), FO2.GetAmountOfLines());

            }
            else
            {
                MessageBox.Show("Jest problem z plikiem orders.txt, Zamykam program, bez tych danych program nie jest w stanie pracować", "Klasa Syntax");
                try
                {
                    Environment.Exit(1);
                }
                catch
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                throw new Exception();          
            }

            error_type = new ErrorTypeParagraph();
            error_type = ErrorTypeParagraph.Nothing;
        }
        public override void CheckCode(string _paragraph)
        {
            this.paragraph = _paragraph;
            this.paragraph = filtre.SetBox(paragraph);

            if (first_set_ranges_done)
            {
                if (!IndexControl()) error_type = ErrorTypeParagraph.NotNumber;
                else if (!CommandExist()) error_type = ErrorTypeParagraph.ThereIsNotAnyOrder;
                else if (!SpaceAfterComma()) error_type = ErrorTypeParagraph.SpaceAfterComma;
                else if (!FirstArgument()) error_type = ErrorTypeParagraph.ExistedOrder;
                else if (!PerfectAmountAgruments()) error_type = ErrorTypeParagraph.Syntax;
                else if (!ExpectedValueOfArgument()) error_type = ErrorTypeParagraph.WhereIsArgument;
                else if (!ForbiddenRange()) error_type = ErrorTypeParagraph.RangeBorder;
                else error_type = ErrorTypeParagraph.Nothing;
            }
                else { MessageBox.Show("There is not Set Ranges", "Ranges missed"); error_type = ErrorTypeParagraph.RangeBorder; }

        }
        public override void CheckCode(string[] _paragraphs)
        {
            throw new NotImplementedException();
        }
        public override string GetError_str()
        {

            return MessageFromCheckingCode(error_type, line_number, LANG);
        }

        /// <summary>
        /// Checking if after all commas ther is a space ' ' 
        /// </summary>
        /// <returns></returns>
        private bool SpaceAfterComma()
        {
            for (int i = 0; i < paragraph.Length; i++)
            {

                if (paragraph[i] == ',')
                {
                    i++;
                    if (paragraph[i] == ' ') continue;
                    else return false;
                }

            }
            return true;
        }
        /// <summary>
        /// Pierwszy wyraz to liczba (numer lini)
        /// </summary>
        /// <returns></returns>
        private bool IndexControl()
        {
            int i_help;
            string s_help = "";

            for (int i = 0; i < paragraph.Length; i++)
            {
                if (paragraph[i] != ' ') s_help += paragraph[i];
                else break;
            }

            try
            {
                i_help = Convert.ToInt32(s_help.ToString());
            }
            catch {
                return false;
            }
            line_number = i_help;
                return true;
        }
        /// <summary>
        /// Drugi wyraz to istniejąca komenda
        /// </summary>
        /// <returns></returns>
        private bool CommandExist()
        {
            
            string s_help = "";
            bool yes = false;
            int word = 1;

            for (int i = 0; i < paragraph.Length; i++)
            {
                if (paragraph[i] != ' ' && word == 1) continue;
                else if (paragraph[i] == ' ' && word == 1) word++;
                else if (paragraph[i] != ' ' && word == 2) s_help += paragraph[i];
                else break;
            }

            for (int i = 0; i < ORD.GetOrders().Length; i++)
            {
                if (s_help.Equals(ORD.GetOrders()[i].Command))
                {
                    this.index = i;//zmienna potrzeban do doboru argumentow do polecenia
                    yes = true;
                    break;
                }
                else yes = false;
            }

            //catch position command
            if (yes) { 
                //catch PD
                if (s_help.Equals("PD")) { pd_exisisting = true; PDs = new double[4]; } 
                //catch Move
                if (s_help.Equals("MO")) { mo_exisisting = true;  }
                //catch D move
                if (s_help.Equals("DS")) { ds_exisisting = true;  }
            
            }
            actual_command = s_help;
                return yes;
        }
        /// <summary>
        /// Po komendzie od razu pojawia sie liczba
        /// </summary>
        /// <returns></returns>
        private bool FirstArgument()
        {
            int word = 1;
            string s_help = "";
            int i_help;

            //if The command dosen't have to have any arguments, and upper loop will prove there isn't any commas, then return true.
            if (0 == ORD.GetOrders()[this.index].Args[0]) return true;

            for (int i = 0; i < paragraph.Length; i++)
            {
                if (paragraph[i] != ' ' && word == 1) continue;
                else if (paragraph[i] == ' ' && word == 1) word++;
                else if (paragraph[i] != ' ' && word == 2) continue;
                else if (paragraph[i] == ' ' && word == 2) word++;
                else if (paragraph[i] != ' ' && word == 3) s_help += paragraph[i];
                else break;
            }

            
            //usuwanie przecinka po rzekomym znaku
            if (s_help.Length > 0)
            {
                if (s_help[s_help.Length - 1] == ',' && 3 == word) s_help = s_help.Substring(0, s_help.Length - 1);
            }
            else return false;//MessageBox.Show("Not enough characters", "FirstArgumentd method");
            

            try
            {
                i_help = Convert.ToInt32(s_help.ToString());
            }
            catch
            {
                return false;
            }

            
            return true;

        }
        /// <summary>
        /// Istnieje tyle przecinków ile jest mozliwych argumentów -1
        /// </summary>
        /// <returns></returns>
        private bool PerfectAmountAgruments()
        {
            int i_help = 1; 

            for (int i = 0; i < paragraph.Length; i++)
            {
               
                if (paragraph[i] == ',') i_help++;//pierwsze przypisanie amount_parameters
                
            }

            amount_parameters = i_help;

            for (int j = 0; j < ORD.GetOrders()[this.index].Amount; j++)
            {
                if (ORD.GetOrders()[this.index].Args[j] == amount_parameters) return true;
                else continue;
            }

            //if The command dosen't have to have any arguments, and upper loop will prove there isn't any commas, then return true.
            if (0 == ORD.GetOrders()[this.index].Args[0]) return true;

            return false;
        }
        /// <summary>
        /// Po każdym przecinku występuje wartość liczbowa bądź jedna litera
        /// </summary>
        /// <returns>Tu powwin byc to co w returnach</returns>
        private bool ExpectedValueOfArgument()
        {
            string[] value = new string[amount_parameters];
            int i_help = 0;
            //najpierw wpisuje do stringa value[] wartości parametrów i przecinek np. "123," "400,"
            //sprawdzanie ostatniej vartosci czy jest o, c lub liczbą
            //sprawdzanie wartosci od konca czy są przecinkiem, jeżeli tak to wszystkei znaki przechwytuję do spacji
            //sprawdzam te przechwytane znaki czy są liczbami
            for (int i = paragraph.Length-1; i > 0 ; i--)//\n tez nie przypisuje
            {
                if (paragraph[i] != ' ' && null != paragraph[i] && '\n' != paragraph[i]) value[i_help] += paragraph[i];
                else i_help++;

                if (i_help == amount_parameters) break;
            }//for

            //obracanie wartości, ponieważ została wczytana od tyłu
            char[] c_ar_help= new char[1];
            for (int i = 0; i < amount_parameters; i++)
            
            {
                try
                {
                    c_ar_help = value[i].ToCharArray();
                    Array.Reverse(c_ar_help);
                }
                catch (Exception r) { MessageBox.Show(r.Message+" Spacja na koncu?, linia: "+line_number); }
                
                value[i] = "";
                for (int j = 0; j < c_ar_help.Length; j++) value[i] += c_ar_help[j];
            }

            double d_help = 0;
            string s_help = "";

            for (int i = 0; i < amount_parameters; i++)
            {
                
                //usuniecie przecinka z końca 
                if (0 != i)
                {
                    value[i] = value[i].Substring(0, value[i].Length - 1);
                }
                //MessageBox.Show(value[i]);
                
                //3 mozliwosci 1. liczba, 2. litera, 3. 'text';
                //1
                try
                {
                    //Need to use Replace because string dosent see dots sa cammas;
                    d_help = Convert.ToDouble(Double.Parse(value[i].Replace('.',',')));
                    

                }
                catch (FormatException one_format)
                {
                    //MessageBox.Show("Error type: " + one_format.Message);
                    s_help = value[i];
                    //d_help = Convert.ToDouble("432.98");
                }
                catch (Exception any)
                {
                    MessageBox.Show("Other error: " + any.Message);
                }

                //Save PD
                if (pd_exisisting && i == amount_parameters - 1) { PDs[0] = d_help; PD_list.Add(PDs); pd_exisisting = false; }
                if (pd_exisisting && i == amount_parameters - 2) { PDs[1] = d_help; }//x
                if (pd_exisisting && i == amount_parameters - 3) { PDs[2] = d_help; }//y
                if (pd_exisisting && i == amount_parameters - 4) { PDs[3] = d_help; }//z
                //check MO by PSs' values
                if (mo_exisisting && i == amount_parameters - 1)
                {
                    foreach (double[] pd in PD_list) { if (pd[0] == d_help) { for (int r = 0; 3 > r; r++) { actual_rage[r] = pd[r + 1]; } mo_exisisting = false; break; } } 
                }
                //Move by DS
                if (ds_exisisting && i == amount_parameters - 1) { actual_rage[0] += d_help; ds_exisisting = false; }
                if(ds_exisisting && i == amount_parameters - 2) { actual_rage[1] += d_help; }
                if(ds_exisisting && i == amount_parameters - 3) { actual_rage[2] += d_help; }
                
            }
            
                return true;
        }
        /// <summary>
        /// Wyjście poza dopuszczalny zakres (x,y,z)
        /// </summary>
        /// <returns></returns>
        private bool ForbiddenRange()
        {
            if (actual_rage[0] < ranges[0] || actual_rage[0] > ranges[1]) return false;
            if (actual_rage[1] < ranges[2] || actual_rage[1] > ranges[3]) return false;
            if (actual_rage[2] < ranges[4] || actual_rage[2] > ranges[5]) return false;
            else
            return true;
        }

        public void SetRanges(double[] _ranges)
        {
            this.ranges = _ranges;
            first_set_ranges_done = true;
        }
        public string GetCommandFromLine()
        {
            return actual_command;
        }
        
    }

    class Code : FindError
    {
        private string[] paragraphs;
        private int bad_line1;
        private List<string> l_command;
        private double speed, sp_actual_speed, ovr_actual_speed;
        private bool first_set_ranges_done;
        private char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private bool speed_help = true;
        private int[] mo_moves;
        private string _LANG;
        public string LANG
        {
            get
            {
                return _LANG;
            }
            set
            {
                _LANG = value;
            }
        }

        /// <summary>
        /// Wyświetla komunikaty o poprawności kodu
        private string MessageFromCheckingCode(ErrorTypeCode ETC, string language)
        {
            if (language == "PL")
            {
                if (ETC == ErrorTypeCode.Nothing) return "Program jest w porządku.";
                else if (ETC == ErrorTypeCode.WrongNumbers) return "w lini  " + bad_line1 + " jest niepoprawny index, prawdopodobnie numery lini nie są po kolei.";
                else if (ETC == ErrorTypeCode.TooMuchOrders) return "w lini " + bad_line1 + " jest więcej niż jedno polecenie.";
                else if (ETC == ErrorTypeCode.WhereIsOpenLoop) return "Pętla zamykana jest częściej niż otwierana.";
                else if (ETC == ErrorTypeCode.WhereIsCloseLoop) return "Wszystkie otwarte pętle muszą być zamknięte.";
                else if (ETC == ErrorTypeCode.TooFurious) return "Prędkość robota musi być ustawiona. \nJest to wymagane ze względu na bezpieczeństwo pracy z robotem. \nUżyj komendy 'OVR' lub 'SP'. ";
                else if (ETC == ErrorTypeCode.TooFast) return "Twój program przekracza założoną prędkość. \nPowinieneś zmienić zakłądaną prędkość maksymalną lub zmniejszyć prędkość robota. \nKomendy odpowiedzialne za prędkość to 'OVR' i 'SP'";
                //else if (ETC == ErrorTypeCode.FinishHim) return "Program nie zawiera ED";
                else if (ETC == ErrorTypeCode.iDontNoThisPosition) return "Próbujesz przenieść się do niezadeklarowanej pozycji. Sprawdź zadeklarowane pozycje";
                else return "Bomba atomowa próbowała utopić dzikiego Bila ale skończyło się jej siano...";
            }
            else
            {
                if (ETC == ErrorTypeCode.Nothing) return "Program is correct.";
                if (ETC == ErrorTypeCode.WrongNumbers) return "In line  " + bad_line1 + " there is incorrect index, probably line's numbers aren't in sequence.";
                else if (ETC == ErrorTypeCode.TooMuchOrders) return "In line  " + bad_line1 + " there is more than one command.";
                else if (ETC == ErrorTypeCode.WhereIsOpenLoop) return "Open loop is missing.";
                else if (ETC == ErrorTypeCode.WhereIsCloseLoop) return "Close loop is missing.";
                else if (ETC == ErrorTypeCode.TooFurious) return "Speed of robot must be set. \nThis is required for the safety of work with the robot. \nUse 'OVR' or 'SP' command. ";
                else if (ETC == ErrorTypeCode.TooFast) return "Your program exceeds the assumed speed. \nYou should change the speed limit or decrease the speed of the robot. \nSpeed ​​commands are 'OVR' and 'SP'.";
                //else if (ETC == ErrorTypeCode.FinishHim) return "Program nie zawiera ED";
                else if (ETC == ErrorTypeCode.iDontNoThisPosition) return "You are trying to move to an undeclared position. Check the declared positions";
                else return "Oh Boy, i don't know what happened, Fatal Error !! No connection with Processor... Try Again...";
            }
        }

        public Code()
        {            
            error_type = new ErrorTypeCode();
            error_type = ErrorTypeCode.Nothing;
            l_command = new List<string>();
            first_set_ranges_done = false;
            LANG = "ENG";
        }
        public override void CheckCode(string _paragraph)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Before this method, is recomended to set speed by SetSpeed(double speed)
        /// </summary>
        /// <param name="_paragraphs"></param>
        public override void CheckCode(string[] _paragraphs)
        {
            this.paragraphs = _paragraphs;
            this.paragraphs = filtre.SetBox(paragraphs);
            //filling l_command
            mo_moves = new int[paragraphs.Length];
            FillCommandList();
            

            if (speed <= 0)
            {
                MessageBox.Show("U didn't set speed u can't work on 0% or less speed. Meaby try use SetSpeed(double) before CheckCode. P.S. Using brain is helpfull, trust me am C# program.");
                Environment.Exit(1);
            }

            if (first_set_ranges_done == false)
            {
                MessageBox.Show("There is not set speed before CheckCode. Method to set is called SetRangesAndSpeed. Program will be terminate, Marcin plz check ur code and use brain next time. Va fail Gwynbleid", "FindError class");
                Environment.Exit(1);
            }
                if (!RisingIndexes()) error_type = ErrorTypeCode.WrongNumbers;
                else if (!OnlyOneOrder()) error_type = ErrorTypeCode.TooMuchOrders;
                else if (!TooMuchOpenFoops()) error_type = ErrorTypeCode.WhereIsCloseLoop;//brakuje zamknięć pętli
                else if (!TooMuchCloseFoops()) error_type = ErrorTypeCode.WhereIsOpenLoop;//brakuje otwarć pętli
                else if (!SetSpeed()) error_type = ErrorTypeCode.TooFurious;
                else if (!TooFast()) error_type = ErrorTypeCode.TooFast;
                else if (!NoExistDimension()) error_type = ErrorTypeCode.iDontNoThisPosition;
                //else if (!FinishProgram()) error_type = ErrorTypeCode.FinishHim;
                else error_type = ErrorTypeCode.Nothing;
            
            
        }
        public override string GetError_str()
        {
            return MessageFromCheckingCode(error_type, LANG);
        
        }

        /// <summary>
        /// Numery lini rosną w kolejnych liniach
        /// </summary>
        /// <returns></returns>
        private bool RisingIndexes()
        {
            string s_help="";
            int[] i_help = new int[paragraphs.Length];
            
            //Type every index to ints' vector
            for (int i = 0; paragraphs.Length > i; i++)//line
            {
                for (int j = 0; paragraphs[i].Length > j; j++)//character, (index)
                {
                    if (' ' != paragraphs[i][j]) { s_help += paragraphs[i][j]; }
                    else break;
                }

                i_help[i] = Convert.ToInt32(s_help.ToString());
                s_help = "";
            }

            int line_help = 1;
            //Check indexes
            for (int j = 0; paragraphs.Length-1 > j; j++)//character, (index)
            {
                if (i_help[j + 1] > i_help[j]) line_help++;
                else { bad_line1 = line_help;  return false; }
            }

                return true;
        }
        /// <summary>
        ///  Więcej niż jedna komenda w jednej lini (niedozwolone)
        /// </summary>
        /// <returns></returns>
        private bool OnlyOneOrder()
        {
            return true;
        }
        /// <summary>
        /// Za dużo otwarć pętli
        /// </summary>
        /// <returns></returns>
        private bool TooMuchOpenFoops()
        {
            int i_help, i_help2;
            i_help = i_help2 = 0;
            string[] ss_help = paragraphs;
            ss_help.ToString().ToUpper();

            for (int i = 0; ss_help.Length > i; i++)
            {
                if (ss_help[i].Contains("RC")) i_help++;
            }
            for (int i = 0; ss_help.Length > i; i++)
            {
                if (ss_help[i].Contains("NX")) i_help2++;
            }

            if (i_help > i_help2) return false;
            else return true;
            
        }
        /// <summary>
        /// Za dużo zamknięć pętli
        /// </summary>
        /// <returns></returns>
        private bool TooMuchCloseFoops()
        {
            int i_help, i_help2;
            i_help = i_help2 = 0;
            string[] ss_help = paragraphs;
            ss_help.ToString().ToUpper();

            for (int i = 0; ss_help.Length > i; i++)
            {
                if (ss_help[i].Contains("RC")) i_help++;
            }
            for (int i = 0; ss_help.Length > i; i++)
            {
                if (ss_help[i].Contains("NX")) i_help2++;
            }

            if (i_help < i_help2) return false;
            else return true;
        }
        /// <summary>
        /// Brak Setera prędkości ruchu robota
        /// </summary>
        /// <returns></returns>
        private bool SetSpeed()
        {
            if (l_command.Contains("OVR") || l_command.Contains("SP")) return true;
            else return false;

            
        }
        /// <summary>
        /// Przekroczenie dopuszczalnej prędkości
        /// </summary>
        /// <returns></returns>
        private bool TooFast()
        {

            return speed_help;
        }
        /// <summary>
        /// Ruch do nie zadeklarowanej pozycji
        /// </summary>
        /// <returns></returns>
        private bool NoExistDimension()
        {
            //compare mo_moves[] with PD_list = new List<double[]>();
            double[] pd_list = new double[PD_list.Count];
            for(int i=0; pd_list.Length>i;i++)
            {
                pd_list[i] = PD_list[i][0];
            }

            for (int i = 0; mo_moves.Length > i;i++ )
            {
                if (pd_list.Contains(mo_moves[i]) || 0 == mo_moves[i]) continue;
                else return false;
            }

            return true;
        }
        /// <summary>
        /// Brak zakończenia Programu
        /// </summary>
        /// <returns></returns>
        private bool FinishProgram()
        {
            if (l_command.Contains("ED")) return true;
            else return false;
           
        }

        ///Trzeba zrobic spis liste wszystkich komend, wyedy bedzie latwiej
        private void FillCommandList()
        {

                string s_help = "";
                //String S_help;
                int word = 1;
                string s_help2 = "";
                int mo_index = 0;
                
                for (int j = 0; j < paragraphs.Length; j++)
                {
                    for (int i = 0; i < paragraphs[j].Length; i++)
                    {
                        if (paragraphs[j][i] != ' ' && word == 1) continue;
                        else if (paragraphs[j][i] == ' ' && word == 1) word++;
                        else if (paragraphs[j][i] != ' ' && word == 2) { s_help += paragraphs[j][i];
                        //if "SP" or "OVR" == s_speed, we have to save this value to debug program
                            if (s_help.Equals("SP"))
                            {
                                for (int index = i + 1; paragraphs[j].Length > index; index++)
                                {
                                    if ('\n' == paragraphs[j][index]) break;
                                    else if (' ' != paragraphs[j][index] && numbers.Contains(paragraphs[j][index])) s_help2 += paragraphs[j][index];
                                        
                                }
                                sp_actual_speed = Convert.ToDouble(s_help2);
                                s_help2 = "";
                                if ((sp_actual_speed * 10) > speed) speed_help = false;
                            }
                            else if (s_help.Equals("OVR"))
                            {
                                for (int index = i + 1; paragraphs[j].Length > index; index++)
                                {
                                    if ('\n' == paragraphs[j][index]) break;
                                    else if (' ' != paragraphs[j][index] && numbers.Contains(paragraphs[j][index])) s_help2 += paragraphs[j][index];

                                }
                                ovr_actual_speed = Convert.ToDouble(s_help2);
                                s_help2 = "";
                                if (ovr_actual_speed > speed) speed_help = false;
                            }
                            //Slightly situation for Move commands, Firstly MO
                            if (s_help.Equals("MO"))
                            {
                                
                                for (int index = i + 1; paragraphs[j].Length > index; index++)
                                {
                                    if ('\n' == paragraphs[j][index]) break;
                                    else if (' ' != paragraphs[j][index] && numbers.Contains(paragraphs[j][index])) s_help2 += paragraphs[j][index];

                                }
                                mo_moves[mo_index] = Convert.ToInt32(s_help2);
                                s_help2 = "";
                                mo_index++;
                            }
                            else if (s_help.Equals("MS"))
                            {
                                for (int index = i + 1; paragraphs[j].Length > index; index++)
                                {
                                    if ('\n' == paragraphs[j][index]) break;
                                    else if (' ' != paragraphs[j][index] && numbers.Contains(paragraphs[j][index])) s_help2 += paragraphs[j][index];
                                }

                                mo_moves[mo_index] = Convert.ToInt32(s_help2);
                                s_help2 = "";
                                mo_index++;

                            }


                        }
                        else break;
                    }
                    l_command.Add(s_help);

                    s_help = "";
                    word = 1;

                }
            
        }

        /// <summary>
        /// Schould be set before CheckCode()
        /// </summary>
        /// <param name="speed">speed in percents 0-100</param>
        public void SetSpeed(double speed)
        {
            this.speed = speed;
            first_set_ranges_done = true;
        }
        

    }
    
   
}
