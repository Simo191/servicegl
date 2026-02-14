namespace MultiServices.Store.Views.Orders;
public partial class PickingPage : ContentPage { public PickingPage(ViewModels.PickingViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.PickingViewModel)?.LoadCommand.Execute(null); } }