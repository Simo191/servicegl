using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm) { InitializeComponent(); BindingContext = vm; }
}