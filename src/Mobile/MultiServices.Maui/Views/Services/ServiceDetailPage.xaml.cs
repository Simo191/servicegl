using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class ServiceDetailPage : ContentPage
{
    public ServiceDetailPage(ServiceDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
