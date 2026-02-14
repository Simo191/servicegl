using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class VehicleSelectionPage : ContentPage
{
    public VehicleSelectionPage(VehicleSelectionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
