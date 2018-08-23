using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboty_przemyslowe
{

    enum ErrorTypeParagraph
    {
        Nothing, // Brak jakich kolwiek znanych błędów

        ThereIsNotAnyOrder, //W lini nie ma żadnego rozkazu
        NotNumber, //Pierwszy wyraz nie jest Liczbą
        ExistedOrder, // po numerze lini nie pojawia się żadna istniejąca komenda
        WhereIsArgument, // Po komendzie nie ma liczby, 
        Syntax, // Zla ilosc argumentow (przecinków)
        SpaceAfterComma, // Nie ma spacji po przecinku
        RangeBorder, //Wyjście poza dopuszczalny zakres (x,y,z)

        FatalError, // Zupełne zniszczenie już na poziomie programowania walidatora

        
    }

    enum ErrorTypeCode
    {
        Nothing, // Brak jakich kolwiek znanych błędów

        WrongNumbers, //Kolejne numery lini nie są coraz większe
        TooMuchOrders, //za dużo rozkazów (MO, GC, GO) w jednej lini
        WhereIsOpenLoop, // za duzo zamknięć pętli
        WhereIsCloseLoop, // za dużo otwarć pętli
        
        TooFurious, // Brak Setera prędkości ruchu robota
        TooFast, //Przekroczenie dopuszczalnej prędkości
        FinishHim, // Brak zakończenia Programu
        iDontNoThisPosition, // Ruch do niezadklerowanej pozycji

        FatalError, // Zupełne zniszczenie już na poziomie programowania walidatora

    }

    struct Order
    {
        public string Command;
        public int[] Args;

        //public int Index;//id komendy do której należą argmenty 
        public int Amount;//ilość możliwości ilości argumentów
        public int CommandLenght;//długość nazwy polecenia
    }

    

}
