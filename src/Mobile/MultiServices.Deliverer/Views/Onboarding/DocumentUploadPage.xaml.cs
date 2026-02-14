using MultiServices.Deliverer.ViewModels.Onboarding;

namespace MultiServices.Deliverer.Views.Onboarding;

public partial class DocumentUploadPage : ContentPage
{
    public DocumentUploadPage(DocumentUploadViewModel vm) { InitializeComponent(); BindingContext = vm; }
}