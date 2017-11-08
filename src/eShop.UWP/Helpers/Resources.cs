using System;
using System.IO;
using System.Reflection;

namespace eShop.UWP.Helpers
{
    static public class Resources
    {
        static public string LoadString(string name)
        {
            return LoadString("eShop.UWP.Assets", name);
        }

        static public string LoadString(string path, string name)
        {
            var assembly = typeof(Resources).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{path}.{name}"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}
