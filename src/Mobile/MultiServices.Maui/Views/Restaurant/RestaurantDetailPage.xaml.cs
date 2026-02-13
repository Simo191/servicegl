using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantDetailPage : ContentPage
{
    public RestaurantDetailPage(RestaurantDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
