using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class ActiveDeliveryPage : ContentPage
{
    public ActiveDeliveryPage(ActiveDeliveryViewModel vm) { InitializeComponent(); BindingContext = vm; }
}