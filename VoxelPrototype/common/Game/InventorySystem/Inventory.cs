namespace VoxelPrototype.common.Game.InventorySystem
{
    internal class Inventory
    {
        internal List<ItemStack> data = new List<ItemStack>();
        public int GetItemIndex(string Name)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].item == Name)
                {
                    return i;
                }
            }
            return -1;
        }
        public bool ContainsItem(string Name)
        {
            for (int t = 0; t < data.Count; t++)
            {
                if (data[t].item == Name)
                {
                    return true;
                }
            }
            return false;
        }
        public int NumberOfItems(string Name)
        {
            int Output = 0;
            for (int t = 0; t < data.Count; t++)
            {
                if (data[t].item == Name)
                {
                    Output += data[t].Number;
                }
            }
            return Output;
        }
        internal bool AddItem(ItemStack stack)
        {
            foreach (var item in data)
            {
                if (item != null)
                {
                    if (item.item == stack.item)
                    {                        
                        item.Number += stack.Number;
                        return true;
                    }
                }
            }
            data.Add(stack);
            return true;
        }
        internal bool RemoveItem(ItemStack stack)
        {
            for (int i = data.Count; i >= 0; i--)
            {
                var item = data[i];
                if (item != null)
                {
                    if (item.item == stack.item)
                    {
                        if (stack.Number < item.Number)
                        {
                            item.Number -= stack.Number;
                            return true;
                        }
                        else if (stack.Number == item.Number)
                        {
                            data.RemoveAt(i);
                        }
                    }
                }
            }
            return false;
        }
    }
}
