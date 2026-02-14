namespace MultiServices.Store.Views.Dashboard;
public partial class DashboardPage : ContentPage { public DashboardPage(ViewModels.DashboardViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.DashboardViewModel)?.LoadDataCommand.Execute(null); } }