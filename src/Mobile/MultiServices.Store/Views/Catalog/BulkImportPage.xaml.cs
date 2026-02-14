namespace MultiServices.Store.Views.Catalog;
public partial class BulkImportPage : ContentPage { public BulkImportPage(ViewModels.ProductFormViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.ProductFormViewModel)?.LoadCommand.Execute(null); } }