using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class EarningDetailPage : ContentPage
{
    private readonly EarningDetailViewModel _vm;
    public EarningDetailPage(EarningDetailViewModel vm)
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
