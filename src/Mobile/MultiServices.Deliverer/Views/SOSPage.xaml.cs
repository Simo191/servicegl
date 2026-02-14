using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class SOSPage : ContentPage
{
    public SOSPage(SOSViewModel vm) { InitializeComponent(); BindingContext = vm; }
}