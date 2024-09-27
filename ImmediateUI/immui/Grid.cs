using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmediateUI.immui
{
    public struct Grid
    {
        public int CollNum, RowNum;
        public Rect Area;
        public Vector2i CurrentCursor = new Vector2i(0, 0);
        public Grid(int collNum, int rowNum, Rect area)
        {
            CollNum = collNum;
            RowNum = rowNum;
            Area = area;
        }
        public Rect GetNext()
        {
            var Style = Immui.GetCurrentStyle();
            Rect Rect = new(Area.X + Style.Padding.X + CurrentCursor.X * (Area.W / CollNum), Area.Y + Style.Padding.Y + CurrentCursor.Y * (Area.H / RowNum), Area.W / CollNum - 2 * Style.Padding.X, Area.H / RowNum - 2 * Style.Padding.X);


            if (CurrentCursor.X + 1 < CollNum)
            {
                CurrentCursor.X++;
            }
            else
            {
                if (CurrentCursor.Y < RowNum - 1)
                {
                    CurrentCursor.X = 0;
                    CurrentCursor.Y++;
                }
            }
            return Rect;
        }
    }
}
