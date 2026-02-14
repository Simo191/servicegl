using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api; using MultiServices.Store.Services.Auth;
namespace MultiServices.Store.ViewModels;
public partial class SettingsViewModel : BaseViewModel {
    private readonly ApiService _api; private readonly AuthService _auth;
    [ObservableProperty] private string _name = string.Empty; [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _phone = string.Empty; [ObservableProperty] private string _deliveryFee = string.Empty;
    [ObservableProperty] private string _freeDeliveryMin = string.Empty; [ObservableProperty] private string _minOrder = string.Empty;
    [ObservableProperty] private string _prepTime = string.Empty; [ObservableProperty] private bool _syncCaisse;
    public SettingsViewModel(ApiService api, AuthService auth) { _api = api; _auth = auth; Title = "Paramètres"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<GroceryStoreDto>("/grocery/store/me");
        if (r.Success && r.Data != null) { var d = r.Data; Name = d.Name; Description = d.Description; Phone = d.Phone;
            DeliveryFee = d.DeliveryFee.ToString(); FreeDeliveryMin = d.FreeDeliveryMinimum.ToString(); MinOrder = d.MinOrderAmount.ToString(); PrepTime = d.AveragePreparationMinutes.ToString(); }
    }); }
    [RelayCommand] private async Task SaveAsync() { await _api.PutAsync<object>("/grocery/store/me", new {
        name = Name, description = Description, phone = Phone,
        deliveryFee = decimal.TryParse(DeliveryFee, out var df) ? df : 0, freeDeliveryMinimum = decimal.TryParse(FreeDeliveryMin, out var fd) ? fd : 0,
        minOrderAmount = decimal.TryParse(MinOrder, out var mo) ? mo : 0, averagePreparationMinutes = int.TryParse(PrepTime, out var pt) ? pt : 30
    }); await Shell.Current.DisplayAlert("OK", "Paramètres enregistrés", "OK"); }
    [RelayCommand] private async Task LogoutAsync() => await _auth.LogoutAsync();
}