using ImmediateUI.immui.math;
using OpenTK.Mathematics;
namespace ImmediateUI.immui.layout
{
    struct Composer
    {
        Vector2i Cursor;
        Rect Area;
        int CurrentRowHeight = 0;
        public Composer(Rect area) : this()
        {
            Area = area;
        }
        public Rect Next(float Width, float Height,Vector4i Padding = default)
        {
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
                Rect Fin = new(Area.X + Cursor.X + Padding.X, Area.Y + Cursor.Y + Padding.X, (int)Width - Padding.X - Padding.Z, (int)Height - Padding.Y - Padding.W);
                Cursor.X += (int)Width;
                CurrentRowHeight = Math.Max(CurrentRowHeight, (int)Height);
                return Fin;
            }
            else
            {
                Cursor.Y += CurrentRowHeight;
                CurrentRowHeight = 0;
                Cursor.X = 0;
                Rect Fin = new(Area.X + Cursor.X + Padding.X, Area.Y + Cursor.Y + Padding.X, (int)Width - Padding.X - Padding.Z, (int)Height - Padding.Y - Padding.W);
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
            Cursor.X = 0;
            Cursor.Y += CurrentRowHeight;
            CurrentRowHeight = (int)RowHeight;
        }
        public Rect NextCollumn(float ColumnWidth, Vector4i Padding = default)
        {
            if (ColumnWidth <= 1)
            {
                ColumnWidth = Area.W * ColumnWidth;
            }
            if (Cursor.X + ColumnWidth <= Area.W)
            {
                Rect Fin = new(Area.X + Cursor.X +Padding.X, Area.Y + Cursor.Y + Padding.X, (int)ColumnWidth - Padding.X - Padding.Z, CurrentRowHeight - Padding.Y - Padding.W);
                Cursor.X += (int)ColumnWidth;
                return Fin;

            }
            else
            {
                return new(-1, -1, -1, -1);
            }
        }
    }
}