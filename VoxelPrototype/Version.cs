
namespace VoxelPrototype
{
    public enum VersionType
    {
        Release,
        ReleaseCandidate,
        Dev
    }
    public class Version : IEquatable<Version>
    {
        public Version( VersionType Type, int major, int minor, int patch)
        {
            this.Type = Type;
            Major = major;
            Minor = minor;
            Patch = patch;
        }
        VersionType Type;
        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 1;
        public int Patch { get; set; } = 0;


        public bool Equals(Version? other)
        {
            if(Major != other.Major || Minor != other.Minor|| Patch != other.Patch)
            {
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            string suf = "";
            if(Type == VersionType.ReleaseCandidate)
            {
                suf = "rc";

            }else if(Type == VersionType.Dev)
            {
                suf = "d";
            }

            return Major+ "." + Minor+"."+suf+Patch;
        }
    }
}
