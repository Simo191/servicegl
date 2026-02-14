using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class PendingVerificationPage : ContentPage
{
    public PendingVerificationPage(PendingVerificationViewModel vm) { InitializeComponent(); BindingContext = vm; }
}