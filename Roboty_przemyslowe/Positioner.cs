using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboty_przemyslowe
{
    class Positioner
    {
        double ax, ay;
        int[] temp = new int[2];
        int xlow, ylow;
        public void set_a(int Xlow, int Xup, int Ylow, int Yup)
        {
            ax = (Xup - Xlow) / 260.00;
            ay = (Yup - Ylow) / 260.00;
            xlow = Xlow;
            ylow = Ylow;
        }

        public int[] get_position_by_cursor(int posx, int posy)
        {
            temp[0] = (int)(ax * posx + xlow);
            temp[1] = (int)(ay * (-posy +260) + ylow);
            
            return temp;
        }

        public int[] get_position_by_scale(int x, int y)
        {
            temp[0] = (int)((x - xlow) / ax);
            temp[1] = (int)((((y - ylow) / ay)-260)*-1);
            
            return temp;
        }

    }
}
