namespace MultiServices.Store.Views.Orders;
public partial class OrderDetailPage : ContentPage { public OrderDetailPage(ViewModels.OrderDetailViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.OrderDetailViewModel)?.LoadCommand.Execute(null); } }