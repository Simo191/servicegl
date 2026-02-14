using MultiServices.Maui.ViewModels.Common;
namespace MultiServices.Maui.Views.Common;
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadDataCommand.ExecuteAsync(null);
    }
}
