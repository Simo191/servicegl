using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadDashboardCommand.ExecuteAsync(null);
    }
    private async void OnToggled(object sender, ToggledEventArgs e)
    {
        await _vm.ToggleOnlineCommand.ExecuteAsync(null);
    }
}