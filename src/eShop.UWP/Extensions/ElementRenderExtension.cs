using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.UI.Xaml;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;

namespace eShop.UWP
{
    static public class ElementRenderExtension
    {
        static public async Task<IRandomAccessStream> RenderAsync(this UIElement element)
        {
            var renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(element);

            var buffer = await renderBitmap.GetPixelsAsync();
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)renderBitmap.PixelWidth, (uint)renderBitmap.PixelHeight, 96, 96, buffer.ToArray());
            await encoder.FlushAsync();
            return stream;
        }
    }
}
