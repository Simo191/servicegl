using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiServices.Deliverer.ViewModels.Base;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _hasError;

    protected async Task ExecuteAsync(Func<Task> operation, string? errorMessage = null)
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
            ErrorMessage = errorMessage ?? ex.Message;
            HasError = true;
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}