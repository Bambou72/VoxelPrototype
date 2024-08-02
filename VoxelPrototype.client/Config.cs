


namespace VoxelPrototype.client
{
    /*
    public class Config
    {

        private TomlTable Data;
        public Config()
        {
            CreateBaseConfig();
            if (Path.Exists("config.toml"))
            {
                var temp = Toml.ToModel(File.ReadAllText("config.toml"));
                foreach (string keyset in temp.Keys)
                {
                    if (temp.ContainsKey(keyset))
                    {
                        Data[keyset] = temp[keyset];
                    }
                }
            }

        }
        private void CreateBaseConfig()
        {
            Data = new();
            Data["width"] = 900;
            Data["height"] = 600;
            Data["mode"] = "windowed";
        }
        public void SaveProperties()
        {
            File.WriteAllText("config.toml", Toml.FromModel(Data));
        }
        public object GetProperty(string name)
        {
            return Data[name];
        }
        public void SetProperty(string Name, object Value)
        {
            Data[Name] = Value;
        }
    }*/
}
