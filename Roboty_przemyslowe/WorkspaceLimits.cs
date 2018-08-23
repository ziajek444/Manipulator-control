using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboty_przemyslowe
{
    class WorkspaceLimits
    {
        int _X_lower_limit;
        public int X_lower_limit
        {
            get { return _X_lower_limit; }
            set { _X_lower_limit = value; }
        }
        int _X_upper_limit;
        public int X_upper_limit
        {
            get { return _X_upper_limit; }
            set { _X_upper_limit = value; }
        }

        int _Y_lower_limit;
        public int Y_lower_limit
        {
            get { return _Y_lower_limit; }
            set { _Y_lower_limit = value; }
        }
        int _Y_upper_limit;
        public int Y_upper_limit
        {
            get { return _Y_upper_limit; }
            set { _Y_upper_limit = value; }
        }

        int _Z_lower_limit;
        public int Z_lower_limit
        {
            get { return _Z_lower_limit; }
            set { _Z_lower_limit = value; }
        }
        int _Z_upper_limit;
        public int Z_upper_limit
        {
            get { return _Z_upper_limit; }
            set { _Z_upper_limit = value; }
        }

        public WorkspaceLimits(int x_low, int x_up, int y_low, int y_up, int z_low, int z_up )
        {
            X_lower_limit = x_low;
            X_upper_limit = x_up;
            Y_lower_limit = y_low;
            Y_upper_limit = y_up;
            Z_lower_limit = z_low;
            Z_upper_limit = z_up;
        }

    }
}
