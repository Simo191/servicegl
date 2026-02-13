using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class ShoppingListsPage : ContentPage
{
    private readonly ShoppingListsViewModel _vm;
    public ShoppingListsPage(ShoppingListsViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadListsCommand.ExecuteAsync(null); }
}
