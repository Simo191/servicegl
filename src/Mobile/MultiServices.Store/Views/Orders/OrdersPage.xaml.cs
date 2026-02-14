namespace MultiServices.Store.Views.Orders;
public partial class OrdersPage : ContentPage { public OrdersPage(ViewModels.OrdersViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.OrdersViewModel)?.LoadCommand.Execute(null); } }