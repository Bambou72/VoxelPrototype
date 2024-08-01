namespace VoxelPrototype.client.ui.elements
{
    internal class Spacing : Element
    {
        public Spacing(int Spacing)
        {
            ParentSizing = false;
            Size.Y = Spacing;
        }
    }
}
