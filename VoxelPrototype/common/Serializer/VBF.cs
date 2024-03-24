//
//COPYRIGHTS Florian Pfeiffer
//
using System.Text;
namespace VBF
{
    public interface IVBFSerializableBinary<T> where T : class
    {
        public byte[] Serialize();
        public T Deserialize(byte[] data);
    }
    public interface IVBFSerializable<T> where T : class
    {
        public VBFCompound Serialize();
        public T Deserialize(VBFCompound compound);
    }
    public class VBFTag
    {
        public enum DataType : byte
        {
            Byte = 0x05,
            Bool = 0x07,
            Int = 0x01,
            Long = 0x09,
            Float = 0x10,
            Double = 0x11,
            String = 0x02,
            ByteArray = 0x06,
            IntArray = 0x08,
            List = 0x04,
            Compound = 0x03,
            End = 0x00,
        }
        public DataType Type { get; set; }
    }
    public class VBFByteArray : VBFTag
    {
        public byte[] Value { get; set; }
        public VBFByteArray(byte[] value)
        {
            Type = DataType.ByteArray;
            Value = value;
        }
    }
    public class VBFIntArray : VBFTag
    {
        public int[] Value { get; set; }
        public VBFIntArray(int[] value)
        {
            Type = DataType.IntArray;
            Value = value;
        }
    }
    public class VBFInt : VBFTag
    {
        public int Value { get; set; }
        public VBFInt(int value)
        {
            Type = DataType.Int;
            Value = value;
        }
    }
    public class VBFLong : VBFTag
    {
        public long Value { get; set; }
        public VBFLong(long value)
        {
            Type = DataType.Long;
            Value = value;
        }
    }
    public class VBFFloat : VBFTag
    {
        public float Value { get; set; }
        public VBFFloat(float value)
        {
            Type = DataType.Float;
            Value = value;
        }
    }
    public class VBFDouble : VBFTag
    {
        public double Value { get; set; }
        public VBFDouble(double value)
        {
            Type = DataType.Double;
            Value = value;
        }
    }
    public class VBFBool : VBFTag
    {
        public bool Value { get; set; }
        public VBFBool(bool value)
        {
            Type = DataType.Bool;
            Value = value;
        }
    }
    public class VBFByte : VBFTag
    {
        public byte Value { get; set; }
        public VBFByte(byte value)
        {
            Type = DataType.Byte;
            Value = value;
        }
    }
    public class VBFString : VBFTag
    {
        public string Value { get; set; }
        public VBFString(string value)
        {
            Type = DataType.String;
            Value = value;
        }
    }
    public class VBFCompound : VBFTag
    {
        internal Dictionary<string, VBFTag> Tags { get; set; } = new Dictionary<string, VBFTag>();
        public VBFCompound()
        {
            Type = DataType.Compound;
        }
        public VBFTag this[string Name]
        {
            get
            {
                return Tags[Name];
            }
            set
            {
                Tags[Name] = value;
            }
        }
        public T Get<T>(string Name) where T :VBFTag
        {
            return (T)Tags[Name];
        }
        public void Add(string Name,VBFTag tag)
        {
            Tags.Add(Name, tag);
        }
        public void AddBool(string Name, bool Value)
        {
            Add(Name, new VBFBool(Value));
        }
        public void AddInt(string Name , int Value)
        {
            Add(Name, new VBFInt(Value));
        }
        public void AddLong(string Name, long Value)
        {
            Add(Name, new VBFLong(Value));
        }
        public void AddByte(string Name , byte Value)
        {
            Add(Name, new VBFByte(Value));
        }
        public void AddFloat(string Name , float Value)
        {
            Add(Name, new VBFFloat(Value));
        }
        public void AddDouble(string Name, double Value)
        {
            Add(Name, new VBFDouble(Value));
        }
        public void AddIntArray(string Name, int[] Value)
        {
            Add(Name, new VBFIntArray(Value));
        }
        public void AddString(string Name, string Value)
        {
            Add(Name, new VBFString(Value));
        }
        public VBFString GetString(string Name)
        {
            return Get<VBFString>(Name);
        }
        public VBFIntArray GetIntArray(string Name)
        {
            return Get<VBFIntArray>(Name);
        }
        public VBFBool GetBool(string Name)
        {
            return Get<VBFBool>(Name);
        }
        public VBFInt GetInt(string Name)
        {
            return Get<VBFInt>(Name);
        }
        public VBFLong GetLong(string Name)
        {
            return Get<VBFLong>(Name);
        }
        public VBFByte GetByte(string Name)
        {
            return Get<VBFByte>(Name);
        }
        public VBFFloat GetFloat(string Name)
        {
            return Get<VBFFloat>(Name);
        }
        public VBFDouble GetDouble(string Name)
        {
            return Get<VBFDouble>(Name);
        }
    }
    public class VBFList : VBFTag
    {
        public DataType ListType { get; set; }
        public List<VBFTag> Tags { get; set; } = new List<VBFTag>();
        public VBFList()
        {
            Type = DataType.List;
        }
    }
    public class VBFSerializer
    {
        public static byte[] Serialize(VBFTag tag)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                SerializeTag(writer, tag);
                return stream.ToArray();
            }
        }
        private static void SerializeTag(BinaryWriter writer, VBFTag tag)
        {
            writer.Write((byte)tag.Type);
            switch (tag.Type)
            {
                case VBFTag.DataType.Int:
                    VBFInt integerTag = (VBFInt)tag;
                    writer.Write(integerTag.Value);
                    break;
                case VBFTag.DataType.Long:
                    VBFLong longTag = (VBFLong)tag;
                    writer.Write(longTag.Value);
                    break;
                case VBFTag.DataType.Float:
                    VBFFloat floatTag = (VBFFloat)tag;
                    writer.Write(floatTag.Value);
                    break;
                case VBFTag.DataType.Double:
                    VBFDouble doubleTag = (VBFDouble)tag;
                    writer.Write(doubleTag.Value);
                    break;
                case VBFTag.DataType.Bool:
                    VBFBool boolTag = (VBFBool)tag;
                    writer.Write(boolTag.Value);
                    break;
                case VBFTag.DataType.Byte:
                    VBFByte byteTag = (VBFByte)tag;
                    writer.Write(byteTag.Value);
                    break;
                case VBFTag.DataType.String:
                    VBFString stringTag = (VBFString)tag;
                    var stringBytes = String2Bytes(stringTag.Value);
                    writer.Write(stringBytes.Length);
                    writer.Write(stringBytes);
                    break;
                case VBFTag.DataType.Compound:
                    SerializeCompound((VBFCompound)tag, writer);
                    break;
                case VBFTag.DataType.List:
                    SerializeList((VBFList)tag, writer);
                    break;
                case VBFTag.DataType.ByteArray:
                    SerializeByteArray((VBFByteArray)tag, writer);
                    break;
                case VBFTag.DataType.IntArray:
                    SerializeIntArray((VBFIntArray)tag,writer);
                    break;
                default:
                    throw new Exception("Unknown VBF tag type.");
            }
        }
        private static void SerializeByteArray(VBFByteArray Tag, BinaryWriter writer)
        {
            writer.Write(Tag.Value.Length); // Array length
            writer.Write(Tag.Value); // Array data
        }
        private static void SerializeIntArray(VBFIntArray Tag, BinaryWriter writer)
        {
            byte[] byteArray = new byte[Tag.Value.Length * sizeof(int)];
            Buffer.BlockCopy(Tag.Value, 0, byteArray, 0, byteArray.Length);
            writer.Write(byteArray.Length); // Array length
            writer.Write(byteArray); // Array data
        }
        private static void SerializeCompound(VBFCompound Tag,BinaryWriter writer)
        {
            foreach (var kvp in Tag.Tags)
            {
                SerializeTag(writer, new VBFString(kvp.Key));
                SerializeTag(writer, kvp.Value);
            }
            writer.Write((byte)VBFTag.DataType.End); // End of compound
        }
        private static void SerializeList(VBFList Tag, BinaryWriter writer)
        {
            writer.Write((byte)Tag.ListType); // List type
            writer.Write(Tag.Tags.Count); // List length
            foreach (var innerTag in Tag.Tags)
            {
                SerializeTag(writer, innerTag);
            }
        }
        private static byte[] String2Bytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        private static string Bytes2String(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static VBFTag Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return DeserializeTag(reader);
            }
        }
        private static VBFTag DeserializeTag(BinaryReader reader)
        {
            VBFTag.DataType tagType = (VBFTag.DataType)reader.ReadByte();
            switch (tagType)
            {
                case VBFTag.DataType.Int:
                    return new VBFInt(reader.ReadInt32());
                case VBFTag.DataType.Long:
                    return new VBFLong(reader.ReadInt64());
                case VBFTag.DataType.Float:
                    return new VBFFloat(reader.ReadSingle());
                case VBFTag.DataType.Double:
                    return new VBFDouble(reader.ReadDouble());
                case VBFTag.DataType.Bool:
                    return new VBFBool(reader.ReadBoolean());
                case VBFTag.DataType.Byte:
                    return new VBFInt(reader.ReadByte());
                case VBFTag.DataType.String:
                    int strLength = reader.ReadInt32();
                    return new VBFString(Bytes2String(reader.ReadBytes(strLength)));
                case VBFTag.DataType.Compound:
                    return DeserializeCompound(reader);
                case VBFTag.DataType.List:
                    return DeserializeList(reader);
                case VBFTag.DataType.ByteArray:
                    return DeserializeByteArray(reader);
                case VBFTag.DataType.IntArray:
                    return DeserializeIntArray(reader);
                case VBFTag.DataType.End:
                    return null; // End of data
                default:
                    throw new Exception("Unknown VBF tag type.");
            }
        }
        private static VBFByteArray DeserializeByteArray(BinaryReader reader)
        {
            int arrayLength = reader.ReadInt32(); // Array length
            byte[] byteArray = reader.ReadBytes(arrayLength); // Array data
            return new VBFByteArray(byteArray);
        }
        private static VBFIntArray DeserializeIntArray(BinaryReader reader)
        {
            int intarrayLength = reader.ReadInt32(); // Array length
            byte[] intbyteArray = reader.ReadBytes(intarrayLength); // Array data
            int[] intArray = new int[intbyteArray.Length / sizeof(int)];
            Buffer.BlockCopy(intbyteArray, 0, intArray, 0, intbyteArray.Length);
            return new VBFIntArray(intArray);
        }
        private static VBFCompound DeserializeCompound(BinaryReader reader)
        {
            VBFCompound compoundTag = new VBFCompound();
            while (true)
            {
                VBFTag nestedTag = DeserializeTag(reader);
                if (nestedTag == null || nestedTag.Type == VBFTag.DataType.End || reader.BaseStream.Position >= reader.BaseStream.Length)
                {
                    break; // End of data or malformed data or End of compound or End of stream
                }
                if (nestedTag is VBFString keyTag)
                {
                    compoundTag.Tags[keyTag.Value] = DeserializeTag(reader);
                }
                else
                {
                    throw new Exception("Unexpected tag in VBF compound.");
                }
            }
            return compoundTag;
        }
        public static VBFList DeserializeList(BinaryReader reader)
        {
            VBFList listTag = new VBFList();
            listTag.ListType = (VBFTag.DataType)reader.ReadByte(); // List type
            int listLength = reader.ReadInt32(); // List length
            for (int i = 0; i < listLength; i++)
            {
                VBFTag innerTag = DeserializeTag(reader);
                listTag.Tags.Add(innerTag);
            }
            return listTag;
        }
    }
}
