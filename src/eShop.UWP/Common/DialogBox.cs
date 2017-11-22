using System;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace eShop.UWP
{
    static public class DialogBox
    {
        static public async Task<bool> ShowAsync(string title, string content, string ok = "Ok", string cancel = null)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = ok
            };
            if (cancel != null)
            {
                dialog.SecondaryButtonText = cancel;
            }
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        static public async Task ShowAsync(string title, Exception ex, string ok = "Ok")
        {
            await ShowAsync(title, ex.Message, ok);
        }

        static public async Task ShowAsync(Result result, string ok = "Ok")
        {
            await ShowAsync(result.Message, result.Description, ok);
        }
    }
}
