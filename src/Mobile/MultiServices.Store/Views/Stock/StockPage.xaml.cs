namespace MultiServices.Store.Views.Stock;
public partial class StockPage : ContentPage { public StockPage(ViewModels.StockViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.StockViewModel)?.LoadCommand.Execute(null); } }