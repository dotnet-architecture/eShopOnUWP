using eShop.UWP.ViewModels.Login;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.TemplateSelectors
{
    public class LoginMethodTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SignInWithHelloTemplate { get; set; }
        public DataTemplate SignInWithPasswordTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(item is SignInWithHelloViewModel)
            {
                return SignInWithHelloTemplate;
            }
            else
            {
                return SignInWithPasswordTemplate;
            }
        }
    }
}
