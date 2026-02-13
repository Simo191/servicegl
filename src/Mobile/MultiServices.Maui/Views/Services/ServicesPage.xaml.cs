using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class ServicesPage : ContentPage
{
    private readonly ServicesViewModel _vm;
    public ServicesPage(ServicesViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadProvidersCommand.ExecuteAsync(null); }
}
