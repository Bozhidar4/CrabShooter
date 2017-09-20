using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrabShooter.Properties;
using System.Drawing;

namespace CrabShooter
{
    class Crab : ImageBase
    {
        private Rectangle crabHotSpot = new Rectangle();
        public Crab() : base(Resources.Crab2)
        {
            crabHotSpot.X = Left -1;
            crabHotSpot.Y = Top - 1;
            crabHotSpot.Width = 100;
            crabHotSpot.Height = 76;
        }

        public void Update(int X, int Y)
        {
            Left = X;
            Top = Y;
            crabHotSpot.X = Left + 20;
            crabHotSpot.Y = Top - 1;
        }

        public bool Hit(int X, int Y)
        {
            Rectangle c = new Rectangle(X, Y, 1, 1); //Create a cursor rectangle - quick way to check for hit.

            if (crabHotSpot.Contains(c))
            {
                return true;
            }

            return false;
        }
    }
}
