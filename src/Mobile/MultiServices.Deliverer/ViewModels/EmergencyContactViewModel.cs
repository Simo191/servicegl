using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class EmergencyContactViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly IAuthService _auth;

    [ObservableProperty] private string _contactName = string.Empty;
    [ObservableProperty] private string _contactPhone = string.Empty;
    [ObservableProperty] private string _relationship = string.Empty;

    public EmergencyContactViewModel(ApiService api, IAuthService auth)
    {
        _api = api;
        _auth = auth;
        Title = "Contact d'urgence";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profile = await _auth.GetDelivererProfileAsync();
            if (profile?.EmergencyContact != null)
            {
                ContactName = profile.EmergencyContact.Name ?? string.Empty;
                ContactPhone = profile.EmergencyContact.Phone ?? string.Empty;
                Relationship = profile.EmergencyContact.Relationship ?? string.Empty;
            }
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(ContactName) || string.IsNullOrWhiteSpace(ContactPhone))
        {
            await Shell.Current.DisplayAlert("Erreur", "Nom et téléphone requis", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _api.PutAsync<ApiResponse<object>>(AppConstants.EmergencyContactEndpoint,
                new { Name = ContactName, Phone = ContactPhone, Relationship });
            await Shell.Current.DisplayAlert("Succès", "Contact d'urgence mis à jour", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}
