namespace MultiServices.Maui.Views.Common;
public partial class AddressEntryPage : ContentPage
{
    public AddressEntryPage(ViewModels.Common.AddressEntryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
