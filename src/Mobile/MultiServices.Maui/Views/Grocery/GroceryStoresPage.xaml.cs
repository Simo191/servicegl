using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class GroceryStoresPage : ContentPage
{
    private readonly GroceryStoresViewModel _vm;
    public GroceryStoresPage(GroceryStoresViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadStoresCommand.ExecuteAsync(null); }
}
