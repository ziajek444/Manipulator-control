using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Roboty_przemyslowe
{
    public enum _MoveType { MS, MO, DS, MRA };

    class PointEllipses
    {
        Ellipse _Ellipse;
        public Ellipse Ellipse
        {
            get { return _Ellipse; }
        }
        int _ID;
        public int ID
        {
            get {return _ID;}
            set { _ID = value; }
        }
        
        public _MoveType MoveType;
        public int X, Y, Z, A, B, C,absX,absY,absZ;
        public bool G;
        public string List_Text;

        public PointEllipses(int ID, Thickness Thick,_MoveType MoveType, int X, int Y, int Z, int A, int B, int C, bool G,int absX,int absY,int absZ)
        {
            _Ellipse = new Ellipse();
            _Ellipse.Fill = new SolidColorBrush(Colors.Red);
            _Ellipse.Width = 5;
            _Ellipse.Height = 5;
            _Ellipse.VerticalAlignment = VerticalAlignment.Top;
            _Ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            _Ellipse.Margin = Thick;
            _Ellipse.IsHitTestVisible = false;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A;
            this.B = B;
            this.C = C;
            this.G = G;
            this.absX = absX;
            this.absY = absY;
            this.absZ = absZ;
            this.ID = ID;
            this.MoveType = MoveType;
            List_Text = ID.ToString() + "\t"+MoveType.ToString() +"\t"+ X.ToString() + "\t" + Y.ToString() + "\t" +Z.ToString();
        }

        public string List_Text_update()
        {
            List_Text = ID.ToString() + "\t" + MoveType.ToString() + "\t" + X.ToString() + "\t" + Y.ToString() + "\t" + Z.ToString();
            return List_Text;
        }

    }
}
