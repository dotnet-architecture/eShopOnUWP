using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace eShop.UWP
{
    static public class ImagePicker
    {
        static public async Task<ImagePickerResult> OpenAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return new ImagePickerResult
                {
                    FileName = file.Name,
                    ContentType = file.ContentType,
                    ImageUri = await GetImageUriAsync(file),
                    ImageBytes = await GetImageBytesAsync(file)
                };
            }
            return null;
        }

        static private async Task<byte[]> GetImageBytesAsync(StorageFile file)
        {
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

        static private async Task<string> GetImageUriAsync(StorageFile file)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            string tempFileName = $"{DateTime.UtcNow.Ticks}{file.FileType}";
            var destinationFile = await file.CopyAsync(folder, tempFileName);
            return $"ms-appdata:///local/images/{destinationFile.Name}";
        }
    }

    public class ImagePickerResult
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ImageUri { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}
