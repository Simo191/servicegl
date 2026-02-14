namespace MultiServices.Restaurant.Views.Orders;
public partial class OrdersKanbanPage : ContentPage
{
    public OrdersKanbanPage(ViewModels.OrdersKanbanViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); if (BindingContext is ViewModels.OrdersKanbanViewModel vm) vm.LoadOrdersCommand.Execute(null); }
}