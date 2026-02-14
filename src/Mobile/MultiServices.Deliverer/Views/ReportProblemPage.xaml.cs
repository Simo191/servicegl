using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class ReportProblemPage : ContentPage
{
    public ReportProblemPage(ReportProblemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
