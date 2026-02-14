namespace MultiServices.Store.Views.Promotions;
public partial class PromotionsPage : ContentPage { public PromotionsPage(ViewModels.PromotionsViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.PromotionsViewModel)?.LoadCommand.Execute(null); } }