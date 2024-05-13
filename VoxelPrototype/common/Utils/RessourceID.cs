using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPrototype.common.Utils
{
    internal static class RessourceID
    {
        internal static string? GetRessourceName(string ID)
        {
            string[] parts = ID.Split(new char[] { '@', '/' });
            return parts[parts.Length-1];
        }
        internal static string? GetRessourceLocalType(string ID)
        {
            string[] parts = ID.Split(new char[] { '@', '/' });
            return  parts.Length == 4  ? parts[2] : null;
        }
        internal static string GetRessourceNamespace(string ID)
        {
            string[] parts = ID.Split(new char[] { '@', '/' });
            return parts[0];
        }
        internal static string GetRessourceGlobalType(string ID)
        {
            string[] parts = ID.Split(new char[] { '@', '/' });
            return parts[1];
        }
    }
}
