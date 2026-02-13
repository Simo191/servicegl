using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class InterventionTrackingPage : ContentPage
{
    public InterventionTrackingPage(InterventionTrackingViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
