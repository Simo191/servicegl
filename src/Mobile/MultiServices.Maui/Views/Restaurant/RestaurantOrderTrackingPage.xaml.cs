using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantOrderTrackingPage : ContentPage
{
    public RestaurantOrderTrackingPage(RestaurantOrderTrackingViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
