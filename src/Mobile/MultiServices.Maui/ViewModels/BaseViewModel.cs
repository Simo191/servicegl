using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiServices.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _isEmpty;

    protected async Task ExecuteAsync(Func<Task> operation, string? errorMsg = null)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = null;
            await operation();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = errorMsg ?? ex.Message;
            await Shell.Current.DisplayAlert("Erreur", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
