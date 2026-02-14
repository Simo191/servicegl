namespace MultiServices.Restaurant.Views.Dashboard;
public partial class DashboardPage : ContentPage
{
    public DashboardPage(ViewModels.DashboardViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); if (BindingContext is ViewModels.DashboardViewModel vm) vm.LoadDataCommand.Execute(null); }
}