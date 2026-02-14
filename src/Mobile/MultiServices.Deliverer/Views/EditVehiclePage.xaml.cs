using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class EditVehiclePage : ContentPage
{
    private readonly EditVehicleViewModel _vm;
    public EditVehiclePage(EditVehicleViewModel vm)
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
