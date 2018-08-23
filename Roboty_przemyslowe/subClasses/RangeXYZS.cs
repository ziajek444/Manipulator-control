using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboty_przemyslowe
{
    /// <summary>
    /// There u will set nad get vale of ranges x, y, z. These ranges are maximum of robots'arm moving. 
    /// Recommended is useed SetRange(?) or other setters after construstor;
    /// </summary>
    class RangeXYZS
    {
        private double x_min, x_max, y_min, y_max, z_min, z_max, speed;

        /// <summary>
        /// Konstructor
        /// </summary>
        public RangeXYZS()
        {
            SetDefaultRange();
        }

        /// <summary>
        /// Can be use only in constructors
        /// </summary>
        private void SetDefaultRange()
        {
            x_min = y_min = z_min = -1000;
            x_max = y_max = z_max = 1000;
            speed = 100;
        }

        /// <summary>
        /// Setting fields to workspace
        /// </summary>
        /// <param name="_xmin">minimum range x</param>
        /// <param name="_xmax">maximum range x</param>
        /// <param name="_ymin">minimum range y</param>
        /// <param name="_ymax">maximum range y</param>
        /// <param name="_zmin">minimum range z</param>
        /// <param name="_zmax">maximum range z</param>
        /// <param name="_speed">maximum speed</param>
        public void SetRange(double _xmin, double _xmax, double _ymin, double _ymax, double _zmin, double _zmax, double _speed)
        {
            this.x_min = _xmin; this.x_max = _xmax;
            this.y_min = _ymin; this.y_max = _ymax;
            this.z_min = _zmin; this.z_max = _zmax;
            this.speed = _speed;
        }
        
        public void SetRange(double _xmin, double _xmax, double _ymin, double _ymax, double _zmin, double _zmax)
        {
            this.x_min = _xmin; this.x_max = _xmax;
            this.y_min = _ymin; this.y_max = _ymax;
            this.z_min = _zmin; this.z_max = _zmax;
        }

        public void SetRange(double _xmin, double _xmax, double _ymin, double _ymax)
        {
            this.x_min = _xmin; this.x_max = _xmax;
            this.y_min = _ymin; this.y_max = _ymax;
        }

        public double XMAX
        {
            get { return x_max; }
            set { x_max = value; }
        }
        public double XMIN
        {
            get { return x_min; }
            set { x_min = value; }
        }
        public double YMAX
        {
            get { return y_max; }
            set { y_max = value; }
        }
        public double YMIN
        {
            get { return y_min; }
            set { y_min = value; }
        }
        public double ZMAX
        {
            get { return z_max; }
            set { z_max = value; }
        }
        public double ZMIN
        {
            get { return z_min; }
            set { z_min = value; }
        }
        public double SPEED
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>
        /// return x,y,z without speed
        /// </summary>
        /// <returns>ranges in double[6]</returns>
        public double[] GetAllRanges()
        {
            double[] d_help = { x_min, x_max, y_min, y_max, z_min, z_max };
            return d_help;
        }

        /// <summary>
        /// return x,y,z and speed
        /// </summary>
        /// <returns>ranges in double[7]</returns>
        public double[] GetAllRangesAndSpeed()
        {
            double[] d_help = { x_min, x_max, y_min, y_max, z_min, z_max, speed };
            return d_help;
        }

        /// <summary>
        /// return only speed
        /// </summary>
        /// <returns>ranges in double</returns>
        public double GetSpeed()
        {
            return speed;
        }
    }
}
