namespace MultiServices.Store.Views.Catalog;
public partial class ProductFormPage : ContentPage { public ProductFormPage(ViewModels.ProductFormViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.ProductFormViewModel)?.LoadCommand.Execute(null); } }