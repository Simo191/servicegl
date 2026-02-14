using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class SupportPage : ContentPage
{
    private readonly SupportViewModel _vm;
    public SupportPage(SupportViewModel vm)
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
