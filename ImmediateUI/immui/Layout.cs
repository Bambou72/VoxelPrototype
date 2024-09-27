using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ImmediateUI.immui
{
    struct Layout
    {
        Vector2i Cursor;
        Rect Area;
        int CurrentRowHeight = 0;
        public Layout(Rect area) : this()
        {
            Area = area;
        }
        public Rect Next(float Width, float Height)
        {
            var Style = Immui.GetCurrentStyle();
            if (Width <= 1)
            {
                Width = Area.W * Width;
            }
            if (Height <= 1)
            {
                Height = Area.H * Height;
            }
            if (Cursor.X + Width <= Area.W)
            {
                Rect Fin = new(Area.X + Cursor.X + Style.Padding.X, Area.Y + Cursor.Y + Style.Padding.X, (int)Width - Style.Padding.X - Style.Padding.Z, (int)Height - Style.Padding.Y - Style.Padding.W);
                Cursor.X += (int)Width;
                CurrentRowHeight =Math.Max(CurrentRowHeight, (int)Height);
                return Fin;
            }
            else
            {
                Cursor.Y += CurrentRowHeight;
                CurrentRowHeight = 0;
                Cursor.X = 0;
                Rect Fin = new(Area.X + Cursor.X + Style.Padding.X, Area.Y + Cursor.Y + Style.Padding.X, (int)Width - Style.Padding.X - Style.Padding.Z, (int)Height - Style.Padding.Y - Style.Padding.W);
                Cursor.X += (int)Width;
                return Fin;
            }
        }
        public void NextRow(float RowHeight)
        {
            if (RowHeight <= 1)
            {
                RowHeight = Area.H * RowHeight;
            }
            var Style = Immui.GetCurrentStyle();
            Cursor.X = 0;
            Cursor.Y += CurrentRowHeight;
            CurrentRowHeight = (int)RowHeight;
        }
        public Rect NextCollumn(float  ColumnWidth)
        {
            var Style = Immui.GetCurrentStyle();
            if(ColumnWidth <=1)
            {
                ColumnWidth = Area.W * ColumnWidth;
            }
            if (Cursor.X +  ColumnWidth <= Area.W)
            {
                Rect Fin = new(Area.X + Cursor.X + Style.Padding.X, Area.Y + Cursor.Y + Style.Padding.X, (int)ColumnWidth - Style.Padding.X - Style.Padding.Z, CurrentRowHeight - Style.Padding.Y - Style.Padding.W);
                Cursor.X += (int)ColumnWidth;
                return Fin;
            
            }else
            {
                return new(-1, -1, -1, -1);
            }
        }
    }
}