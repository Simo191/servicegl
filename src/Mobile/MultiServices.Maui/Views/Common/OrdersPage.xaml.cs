using MultiServices.Maui.ViewModels.Common;
namespace MultiServices.Maui.Views.Common;
public partial class OrdersPage : ContentPage
{
    private readonly OrdersViewModel _vm;
    public OrdersPage(OrdersViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadOrdersCommand.ExecuteAsync(null); }
}
