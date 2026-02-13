using MultiServices.Maui.ViewModels.Auth;

namespace MultiServices.Maui.Views.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
