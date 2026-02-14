using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class TrainingQuizPage : ContentPage
{
    private readonly TrainingQuizViewModel _vm;
    public TrainingQuizPage(TrainingQuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadQuizCommand.ExecuteAsync(null);
    }
}