using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel;

namespace eShop.UWP
{
    public class ImagePicker
    {
        public string FilePath { get; private set; }
        public string ContentType { get; private set; }

        public async Task<byte[]> GetImageAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            return await LoadImageAsync(file);
        }

        private async Task<byte[]> LoadImageAsync(StorageFile file)
        {
            if (file == null)
            {
                return null;
            }

            ContentType = file.ContentType;

            var appInstalledFolder = Package.Current.InstalledLocation;
            var assets = await appInstalledFolder.GetFolderAsync("Assets");

            var targetFile = await assets.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
            await file.CopyAndReplaceAsync(targetFile);
            FilePath = targetFile.Path;

            using (var randomStream = await file.OpenReadAsync())
            {
                using (var stream = randomStream.AsStream())
                {
                    byte[] buffer = new byte[randomStream.Size];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}
