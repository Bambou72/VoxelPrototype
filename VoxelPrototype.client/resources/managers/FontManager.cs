using OpenTK.Mathematics;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.resources.managers;
using VoxelPrototype.client.utils;
using VoxelPrototype.client.utils.StbImage;
using VoxelPrototype.utils;
namespace VoxelPrototype.client.Resources.Managers
{
    internal class FontManager : IReloadableResourceManager
    {

        private Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        public void Clean()
        {
            foreach (var font in Fonts.Values)
            {
                font.Clean();
            }
            Fonts.Clear();
        }
        
        public Font GetFont(string resourceLocation)
        {
            if (Fonts.TryGetValue(resourceLocation, out Font font))
            {
                return font;
            }
            throw new Exception("Can't find font");
        }
        public void Reload(ResourcesManager Manager)
        {
            Clean();
            List<TempFont> TempFonts = new();
            var fonts = Manager.ListAllResources("font", path => path.EndsWith(".json"));
            foreach (var font in fonts)
            {
                if (font.Value.Count > 0)
                {
                    TempFont TMPFont = new();
                    TMPFont.Location = font.Key;
                    foreach (var font2 in font.Value)
                    {
                        font2.Open();
                        FontJson JSFont = JsonSerializer.Deserialize(new StreamReader(font2.GetStream()).ReadToEnd(),FontJsonJsonSerializerContext.Default.FontJson);
                        foreach (var sup in JSFont.suppliers)
                        {
                            TMPFont.FontSuppliers.Add(sup);
                            TempFonts.Add(TMPFont);
                        }
                    }

                }
            }
            //
            //Finalize Font data
            //
            foreach (var font in TempFonts)
            {
                Font Font = new();
                foreach (Supplier supplier in font.FontSuppliers)
                {
                    switch (supplier.type)
                    {
                        case "bitmap":
                            LoadSupplierBitmap(supplier, Font);
                            break;
                        default:
                            throw new Exception("You are using a supplier that doesn't exist");
                    }
                }
                Font.GenerateAtlas();
                Font.CleanBitmapAftonGeneration();
                Font.FontTexture.SetData(Font.TextureData, Font.TextureSize, Font.TextureSize);
                if (!Directory.Exists("temp/debug/atlas/fonts"))
                {
                    Directory.CreateDirectory("temp/debug/atlas/fonts");
                }
                ImageSaver.SaveAsPNG("temp/debug/atlas/fonts/" + Path.GetFileNameWithoutExtension(font.Location) + ".png", 
                    new ImageResult()
                    {
                        Data = Font.TextureData,
                        Width = Font.TextureSize,
                        Height= Font.TextureSize,
                        Comp = ColorComponents.RedGreenBlueAlpha,
                        SourceComp = ColorComponents.RedGreenBlueAlpha
                    });
                Font.TextureData = null;
                Fonts[font.Location] = Font;
            }
        }
        public void LoadSupplierBitmap(Supplier Sup, Font Font)
        {
            int WidthCount = Sup.data[0].Length;
            int HeightCount = Sup.data.Length;
            StoreTexture Text = (StoreTexture)Client.TheClient.TextureManager.GetTexture(Sup.file);
            int CellWidth = Text.ImageResult.Width / WidthCount;
            int CellHeight = Text.ImageResult.Height / HeightCount;
            for (int y = 0; y < HeightCount; y++)
            {
                for (int x = 0; x < WidthCount; x++)
                {
                
                    int GlobalX = x * CellWidth;
                    int GlobalY = y * CellHeight;
                    //Char Width
                    bool[] RowEmptys = new bool[CellWidth];
                    for (int i = 0; i < CellWidth; i++)
                    {
                        for(int j = 0; j <CellHeight; j++)
                        {
                            if (Text.ImageResult.Data[((GlobalY + j) * Text.ImageResult.Width + (GlobalX + i)) * 4 + 3] != 0)
                            {
                                RowEmptys[i] = true;
                                break;
                            }
                        }
                    }
                    int CharWidth = 0;
                    for(int n = CellWidth-1; n >= 0; n--)
                    {
                        if (RowEmptys[n])
                        {
                            CharWidth = n + 1;
                            break;
                        }
                    }
                    //TransferData 
                    Character character = new Character();
                    character.Value = Sup.data[y][x];
                    character.Width =  (int)MathF.Ceiling((float)CharWidth / (CellHeight / Sup.height));
                    character.Height = Sup.height;
                    character.Ascender = Sup.ascender;
                    CharacterBitmap characterBitmap = new CharacterBitmap();
                    characterBitmap.Height = CellHeight;
                    characterBitmap.Width = CharWidth;
                    byte[] BitmapData = new byte[CellHeight * CharWidth * 4];
                    for (int k = 0; k < CellHeight; k++)
                    {
                        for (int l = 0; l < CharWidth; l++)
                        {
                            int pixelX = GlobalX + l;
                            int pixelY = GlobalY + k;
                            int index = (k* CharWidth + l) * 4;
                            BitmapData[index] = Text.ImageResult.Data[(pixelY * Text.ImageResult.Width + pixelX)*4];
                            BitmapData[index + 1] = Text.ImageResult.Data[(pixelY * Text.ImageResult.Width + pixelX) * 4+1];
                            BitmapData[index + 2] = Text.ImageResult.Data[(pixelY * Text.ImageResult.Width + pixelX) * 4+2];
                            BitmapData[index + 3] = Text.ImageResult.Data[(pixelY * Text.ImageResult.Width + pixelX) * 4+3];
                        }
                    }
                    characterBitmap.Bitmap  = BitmapData;
                    Font.Characters[character.Value] = character;
                    Font.CharactersBitmap[character.Value] = characterBitmap;
                }
            }
        }

    }
    //
    //Temp Font 
    //
    internal class TempFont
    {
        internal string Location;
        internal List<Supplier> FontSuppliers = new List<Supplier>();
    }


    //
    //Font
    //
    public class Font
    {
        internal Dictionary<char, Character> Characters = new();
        internal Dictionary<char, CharacterBitmap> CharactersBitmap = new();
        internal Texture FontTexture;
        internal byte[] TextureData;
        internal int TextureSize = 128;
        public Font()
        {
            FontTexture = new(new Vector2i(TextureSize),false);
            TextureData = new byte[TextureSize*TextureSize*4];
        }
        public void GenerateAtlas(int Size = 128)
        {
            int XPointer =0 ,YPointer = 0;
            int LineHeight = 0;

            foreach (var Char in Characters.Values)
            {
                CharacterBitmap CharBit = CharactersBitmap[Char.Value];
                LineHeight = Math.Max(LineHeight, CharBit.Height);
                if (XPointer + CharBit.Width > TextureSize)
                {
                    XPointer = 0;
                    YPointer += LineHeight;
                    LineHeight = 0;
                }
                if (YPointer + CharBit.Height > TextureSize)
                {
                    TextureSize *= 2;
                    TextureData = new byte[TextureSize * TextureSize * 4];
                    GenerateAtlas(TextureSize);
                }
                Char.UV0 = new Vector2((float)XPointer , (float)YPointer) / (float)TextureSize ;
                Char.UV1 = new Vector2(XPointer + CharBit.Width , YPointer + CharBit.Height ) / (float)TextureSize;
                for (int y = 0; y < CharBit.Height; y++)
                {
                    for (int x = 0; x < CharBit.Width; x++)
                    {
                        int pixelX = XPointer + x;
                        int pixelY = YPointer + y;
                        int index = (pixelY * TextureSize + pixelX) * 4;
                        TextureData[index] = CharBit.Bitmap[(y * CharBit.Width + x)*4];
                        TextureData[index + 1] = CharBit.Bitmap[(y * CharBit.Width + x) * 4+1];
                        TextureData[index + 2] = CharBit.Bitmap[(y * CharBit.Width + x) * 4+2];
                        TextureData[index + 3] = CharBit.Bitmap[(y * CharBit.Width + x) * 4+3];
                    }
                }
                XPointer += CharBit.Width;
            }
            
        }
        static int IndexConvertion(int x, int y, int width)
        {
            return y * width + x;
        }
        internal Character? GetChar(char Char)
        {
            if(Characters.TryGetValue(Char,out Character Character))
            {
                return Character;
            }
            return null;
        }
       

        public void Clean()
        {
            FontTexture.Clean();
        }
        public void CleanBitmapAftonGeneration()
        {
            CharactersBitmap.Clear();
        }
    }
    public class CharacterBitmap
    {
        internal byte[] Bitmap;
        internal int Height;
        internal int Width;
    }
    public class Character
    {
        internal char Value;
        internal int Height { get; set; }
        internal int Ascender;
        internal int Width;
        internal Vector2 UV0 = Vector2.Zero;
        internal Vector2 UV1 = Vector2.Zero;
    }
    //
    //JSON
    //
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(FontJson))]
    [JsonSerializable(typeof(Supplier))]
    internal partial class FontJsonJsonSerializerContext : JsonSerializerContext
    {
    }
    public class FontJson
    {
        public Supplier[] suppliers { get; set; }
    }

    public class Supplier
    {
        public string type { get; set; }
        public string file { get; set; }
        public int height { get; set; } = 8;
        public int ascender { get; set; } = 7;
        public string[] data { get; set; }
    }
}
