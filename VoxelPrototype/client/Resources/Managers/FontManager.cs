using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VoxelPrototype.client.Render.Text;

namespace VoxelPrototype.client.Resources.Managers
{
    internal class FontManager : IReloadableResourceManager
    {

        private  Dictionary<ResourceID, Font> Fonts = new Dictionary<ResourceID, Font>();
        public void Clean()
        {
            foreach (var font in Fonts.Values)
            {
                font.Clean();
            }
            Fonts.Clear();
        }
        public Font GetFont(ResourceID resourceID)
        {
            if (Fonts.TryGetValue(resourceID, out Font font))
            {
                return font;
            }
            throw new Exception("Can't find font");
        }
        public void Reload(ResourceManager Manager)
        {
            Clean();
            var fonts = Manager.ListResources("font", path => path.EndsWith(".ttf"));
            foreach (var font in fonts)
            {
                Fonts.Add(font.Key, new Font(font.Value.GetPath()));
                font.Value.Close();
            }
        }
    }
}
