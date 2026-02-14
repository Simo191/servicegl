namespace MultiServices.Maui.Views.Checkout;
public partial class CheckoutAuthPage : ContentPage
{
    public CheckoutAuthPage(ViewModels.Checkout.CheckoutAuthViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
