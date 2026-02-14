using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class DeliveryHistoryPage : ContentPage
{
    private readonly DeliveryHistoryViewModel _vm;
    public DeliveryHistoryPage(DeliveryHistoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadHistoryCommand.ExecuteAsync(null);
    }
}