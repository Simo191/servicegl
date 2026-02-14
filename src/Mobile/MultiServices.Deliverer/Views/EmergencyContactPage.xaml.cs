using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class EmergencyContactPage : ContentPage
{
    private readonly EmergencyContactViewModel _vm;
    public EmergencyContactPage(EmergencyContactViewModel vm)
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
