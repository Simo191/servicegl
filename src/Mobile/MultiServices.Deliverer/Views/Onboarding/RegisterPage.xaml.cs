using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm) { InitializeComponent(); BindingContext = vm; }
}