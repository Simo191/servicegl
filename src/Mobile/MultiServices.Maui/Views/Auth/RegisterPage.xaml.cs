using MultiServices.Maui.ViewModels.Auth;
namespace MultiServices.Maui.Views.Auth;
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
