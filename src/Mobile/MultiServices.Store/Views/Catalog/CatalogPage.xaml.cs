namespace MultiServices.Store.Views.Catalog;
public partial class CatalogPage : ContentPage { public CatalogPage(ViewModels.CatalogViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.CatalogViewModel)?.LoadCommand.Execute(null); } }