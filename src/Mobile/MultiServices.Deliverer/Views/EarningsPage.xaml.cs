using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class EarningsPage : ContentPage
{
    private readonly EarningsViewModel _vm;
    public EarningsPage(EarningsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadEarningsCommand.ExecuteAsync(null);
    }
}