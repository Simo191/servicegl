using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class DeliveryDetailPage : ContentPage
{
    public DeliveryDetailPage(DeliveryDetailViewModel vm) { InitializeComponent(); BindingContext = vm; }
}