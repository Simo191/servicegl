using MultiServices.Deliverer.ViewModels;

namespace MultiServices.Deliverer.Views;

public partial class EditProfilePage : ContentPage
{
    private readonly EditProfileViewModel _vm;
    public EditProfilePage(EditProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}