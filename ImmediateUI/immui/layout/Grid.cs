using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmediateUI.immui.layout
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
        public Rect GetNext(Vector4i Padding = default)
        {
            Rect Rect = new(Area.X + Padding.X + CurrentCursor.X * (Area.W / CollNum), Area.Y + Padding.Y + CurrentCursor.Y * (Area.H / RowNum), Area.W / CollNum - Padding.X -Padding.Z, Area.H / RowNum - Padding.Y - Padding.W) ;


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
