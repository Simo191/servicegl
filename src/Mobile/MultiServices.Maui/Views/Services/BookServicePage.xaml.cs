using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class BookServicePage : ContentPage
{
    public BookServicePage(BookServiceViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
