using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantsPage : ContentPage
{
    private readonly RestaurantsViewModel _vm;
    public RestaurantsPage(RestaurantsViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadRestaurantsCommand.ExecuteAsync(null); }
}
