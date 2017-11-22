using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;

namespace eShop.UWP
{
    public class FileStorage
    {
        public FileStorage(StorageFolder folder)
        {
            Folder = folder;
        }

        public StorageFolder Folder { get; }

        public async Task<bool> FileExsits(string fileName)
        {
            var file = await Folder.TryGetItemAsync(fileName);
            return file != null;
        }

        public async Task<byte[]> ReadBytes(string fileName)
        {
            var storageFile = await Folder.GetFileAsync(fileName);
            using (var randomStream = await storageFile.OpenReadAsync())
            {
                using (var stream = new BinaryReader(randomStream.AsStreamForRead()))
                {
                    return stream.ReadBytes((int)randomStream.Size);
                }
            }
        }

        public async Task<string> WriteBytes(string fileName, byte[] bytes)
        {
            var storageFile = await Folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            using (var randomStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (var stream = new BinaryWriter(randomStream.AsStreamForWrite()))
                {
                    stream.Write(bytes);
                }
            }
            return Path.Combine(storageFile.Path, fileName);
        }
    }
}
