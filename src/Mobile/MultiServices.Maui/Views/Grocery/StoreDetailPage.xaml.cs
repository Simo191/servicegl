using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class StoreDetailPage : ContentPage
{
    public StoreDetailPage(StoreDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
