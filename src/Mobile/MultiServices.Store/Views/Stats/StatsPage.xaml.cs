namespace MultiServices.Store.Views.Stats;
public partial class StatsPage : ContentPage { public StatsPage(ViewModels.StatsViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.StatsViewModel)?.LoadCommand.Execute(null); } }