namespace MultiServices.Store.Views.Auth;
public partial class LoginPage : ContentPage { public LoginPage(ViewModels.LoginViewModel vm) { InitializeComponent(); BindingContext = vm; } }