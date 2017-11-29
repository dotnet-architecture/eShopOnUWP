using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Views
{
    public sealed partial class SignInWithPasswordView : UserControl
    {
        public SignInWithPasswordView()
        {
            InitializeComponent();
        }

        public void Focus()
        {
            userName.Focus(FocusState.Programmatic);
        }
    }
}
