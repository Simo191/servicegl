using CommunityToolkit.Mvvm.ComponentModel;
namespace MultiServices.Store.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _isEmpty;
    protected async Task ExecuteAsync(Func<Task> op, string? err = null)
    { if (IsBusy) return; try { IsBusy = true; HasError = false; ErrorMessage = null; await op(); } catch (Exception ex) { HasError = true; ErrorMessage = err ?? ex.Message; await Shell.Current.DisplayAlert("Erreur", ErrorMessage, "OK"); } finally { IsBusy = false; IsRefreshing = false; } }
}