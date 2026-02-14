using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class TrainingPage : ContentPage
{
    public TrainingPage(TrainingViewModel vm) { InitializeComponent(); BindingContext = vm; }
}