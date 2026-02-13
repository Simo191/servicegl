using MultiServices.Maui.ViewModels.Profile;
namespace MultiServices.Maui.Views.Profile;
public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;
    public ProfilePage(ProfileViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadProfileCommand.ExecuteAsync(null); }
}
