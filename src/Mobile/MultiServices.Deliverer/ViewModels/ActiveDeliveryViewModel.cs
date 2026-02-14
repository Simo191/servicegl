using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Location;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

[QueryProperty(nameof(DeliveryId), "id")]
public partial class ActiveDeliveryViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly ILocationService _locationService;

    [ObservableProperty] private string _deliveryId = string.Empty;
    [ObservableProperty] private ActiveDelivery? _delivery;
    [ObservableProperty] private bool _showPickupInfo;
    [ObservableProperty] private bool _showDeliveryInfo;
    [ObservableProperty] private string _nextAction = string.Empty;
    [ObservableProperty] private double _currentLat;
    [ObservableProperty] private double _currentLng;

    public ActiveDeliveryViewModel(ApiService api, ILocationService locationService)
    {
        _api = api;
        _locationService = locationService;
        Title = "Livraison en cours";
    }

    partial void OnDeliveryIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadDeliveryAsync();
    }

    [RelayCommand]
    private async Task LoadDeliveryAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<ActiveDelivery>>(
                string.Format(AppConstants.DeliveryDetailEndpoint, DeliveryId));
            if (result?.Data != null)
            {
                Delivery = result.Data;
                ShowPickupInfo = Delivery.IsAtPickupPhase;
                ShowDeliveryInfo = Delivery.IsAtDeliveryPhase;
                NextAction = Delivery.NextActionLabel;
            }
        });
    }

    [RelayCommand]
    private async Task AdvanceStatusAsync()
    {
        if (Delivery == null) return;

        var nextStatus = Delivery.Status switch
        {
            "Assigned" => "ArrivedAtPickup",
            "ArrivedAtPickup" => "PickedUp",
            "PickedUp" => "ArrivedAtCustomer",
            "ArrivedAtCustomer" => "Delivered",
            _ => null
        };

        if (nextStatus == null) return;

        if (nextStatus == "Delivered")
        {
            var takePhoto = await Shell.Current.DisplayAlert("Photo de livraison",
                "Prendre une photo comme preuve de livraison ?", "Oui", "Non");
            if (takePhoto)
                await TakeDeliveryPhotoAsync();
        }

        await ExecuteAsync(async () =>
        {
            await _api.PutAsync<ApiResponse<object>>(
                string.Format(AppConstants.DeliveryStatusEndpoint, DeliveryId),
                new StatusUpdateRequest { Status = nextStatus });

            if (nextStatus == "Delivered")
            {
                await Shell.Current.DisplayAlert("Bravo ! 🎉", "Livraison terminée avec succès", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            await LoadDeliveryAsync();
        });
    }

    [RelayCommand]
    private async Task NavigateToPickupAsync()
    {
        if (Delivery == null) return;
        await OpenNavigationAsync(Delivery.PickupLat, Delivery.PickupLng, Delivery.PickupName);
    }

    [RelayCommand]
    private async Task NavigateToClientAsync()
    {
        if (Delivery == null) return;
        await OpenNavigationAsync(Delivery.DeliveryLat, Delivery.DeliveryLng, Delivery.ClientName);
    }

    private async Task OpenNavigationAsync(double lat, double lng, string name)
    {
        try
        {
            var location = new Microsoft.Maui.Devices.Sensors.Location(lat, lng);
            var options = new MapLaunchOptions { Name = name, NavigationMode = NavigationMode.Driving };
            await Map.Default.OpenAsync(location, options);
        }
        catch
        {
            await Shell.Current.DisplayAlert("Erreur", "Impossible d'ouvrir la navigation", "OK");
        }
    }

    [RelayCommand]
    private async Task CallPickupAsync()
    {
        if (Delivery?.PickupPhone != null)
        {
            try { PhoneDialer.Open(Delivery.PickupPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }

    [RelayCommand]
    private async Task CallClientAsync()
    {
        if (Delivery?.ClientPhone != null)
        {
            try { PhoneDialer.Open(Delivery.ClientPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }

    [RelayCommand]
    private async Task ChatClientAsync()
    {
        if (Delivery?.ClientPhone != null)
        {
            try
            {
                await Sms.ComposeAsync(new SmsMessage("", new[] { Delivery.ClientPhone }));
            }
            catch { }
        }
    }

    [RelayCommand]
    private async Task TakeDeliveryPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo != null)
            {
                var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using var stream = await photo.OpenReadAsync();
                using var file = File.OpenWrite(localPath);
                await stream.CopyToAsync(file);
                file.Close();

                await _api.UploadAsync<ApiResponse<object>>(
                    string.Format(AppConstants.DeliveryPhotoEndpoint, DeliveryId), localPath, "photo");
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task ReportProblemAsync()
    {
        var problemType = await Shell.Current.DisplayActionSheet(
            "Type de problème", "Annuler", null,
            "Adresse incorrecte", "Client absent", "Produit endommagé", "Autre");

        if (problemType == null || problemType == "Annuler") return;

        var description = await Shell.Current.DisplayPromptAsync("Description", "Décrivez le problème:");
        if (string.IsNullOrEmpty(description)) return;

        await ExecuteAsync(async () =>
        {
            var typeCode = problemType switch
            {
                "Adresse incorrecte" => "WrongAddress",
                "Client absent" => "ClientAbsent",
                "Produit endommagé" => "DamagedProduct",
                _ => "Other"
            };

            await _api.PostAsync<ApiResponse<object>>(
                string.Format(AppConstants.ReportProblemEndpoint, DeliveryId),
                new ProblemReport { DeliveryId = DeliveryId, Type = typeCode, Description = description });

            await Shell.Current.DisplayAlert("Signalé", "Le problème a été signalé au support", "OK");
        });
    }

    [RelayCommand]
    private async Task SOSAsync()
    {
        await Shell.Current.GoToAsync("sos");
    }
}