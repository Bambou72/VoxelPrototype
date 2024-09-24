using ImmediateUI.immui.math;
using OpenTK.Mathematics;

namespace ImmediateUI.immui
{
    struct Layout
    {
        public int CollNum, RowNum;
        public Rect Zone;
        public Vector2i CurrentCursor = new Vector2i(0,0);
        public Layout(int collNum, int rowNum, Rect zone)
        {
            CollNum = collNum;
            RowNum = rowNum;
            Zone = zone;
        }
        public Rect GetNext()
        {
            var Style = Immui.GetCurrentStyle();
            Rect Rect = new(Zone.X + Style.Padding.X + CurrentCursor.X * (Zone.W / CollNum), Zone.Y + Style.Padding.Y + CurrentCursor.Y * (Zone.H / RowNum), Zone.W / CollNum - 2 * Style.Padding.X, Zone.H / RowNum - 2 * Style.Padding.X);
            
            
            if (CurrentCursor.X+ 1  < CollNum)
            {
                CurrentCursor.X++;
            }
            else
            {
                if (CurrentCursor.Y < RowNum -1)
                {
                    CurrentCursor.X = 0;
                    CurrentCursor.Y++;
                }
            }
            return Rect;
        }
    }
}