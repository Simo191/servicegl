namespace MultiServices.Store.Views.Settings;
public partial class SettingsPage : ContentPage { public SettingsPage(ViewModels.SettingsViewModel vm) { InitializeComponent(); BindingContext = vm; }
    protected override void OnAppearing() { base.OnAppearing(); (BindingContext as ViewModels.SettingsViewModel)?.LoadCommand.Execute(null); } }