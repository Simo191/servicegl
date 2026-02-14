using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class PayoutPage : ContentPage
{
    private readonly PayoutViewModel _vm;
    public PayoutPage(PayoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}