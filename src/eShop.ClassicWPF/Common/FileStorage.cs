using System;
using System.IO;
using System.Threading.Tasks;

namespace eShop.WPF
{
    public class FileStorage
    {
        public FileStorage(string folder)
        {
            Folder = folder;
            Directory.CreateDirectory(Folder);
        }

        public string Folder { get; }

        public bool FileExsits(string fileName)
        {
            string path = Path.Combine(Folder, fileName);
            return File.Exists(path);
        }

        public byte[] ReadBytes(string fileName)
        {
            string path = Path.Combine(Folder, fileName);
            return File.ReadAllBytes(path);
        }

        public string WriteBytes(string fileName, byte[] bytes)
        {
            string path = Path.Combine(Folder, fileName);
            File.WriteAllBytes(path, bytes);
            return path;
        }
    }
}
