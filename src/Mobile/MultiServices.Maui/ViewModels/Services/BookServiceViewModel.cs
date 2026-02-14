using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Services;

[QueryProperty(nameof(ProviderId), "providerId")]
[QueryProperty(nameof(ServiceId), "serviceId")]
public partial class BookServiceViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _providerId = string.Empty;
    [ObservableProperty] private string _serviceId = string.Empty;
    [ObservableProperty] private string _problemDescription = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _problemPhotos = new();
    [ObservableProperty] private AddressDto? _selectedAddress;
    [ObservableProperty] private ObservableCollection<AddressDto> _addresses = new();
    [ObservableProperty] private ObservableCollection<AvailableSlotDto> _availableSlots = new();
    [ObservableProperty] private AvailableSlotDto? _selectedSlot;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today.AddDays(1);
    [ObservableProperty] private string _paymentTiming = "after";
    [ObservableProperty] private int _currentStep = 1;

    /// <summary>Bindable MinimumDate for DatePicker (avoids sys:DateTime in XAML)</summary>
    public DateTime MinimumDate => DateTime.Today;

    public BookServiceViewModel(ApiService api)
    {
        _api = api;
        Title = "Réserver un service";
    }

    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        var result = await _api.GetAsync<List<AddressDto>>("/auth/profile/addresses");
        if (result.Success && result.Data != null)
        {
            Addresses = new ObservableCollection<AddressDto>(result.Data);
            SelectedAddress = Addresses.FirstOrDefault(a => a.IsDefault) ?? Addresses.FirstOrDefault();
        }
    }

    [RelayCommand]
    private async Task LoadSlotsAsync()
    {
        if (string.IsNullOrEmpty(ProviderId)) return;

        var queryParams = new Dictionary<string, string>
        {
            ["category"] = "",
            ["date"] = SelectedDate.ToString("yyyy-MM-dd"),
            ["city"] = SelectedAddress?.City ?? ""
        };

        var result = await _api.GetAsync<List<AvailableSlotDto>>("/services/providers/available", queryParams);
        if (result.Success && result.Data != null)
        {
            AvailableSlots = new ObservableCollection<AvailableSlotDto>(result.Data);
        }
    }

    [RelayCommand]
    private void NextStep()
    {
        if (CurrentStep < 4) CurrentStep++;
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentStep > 1) CurrentStep--;
    }

    [RelayCommand]
    private async Task ConfirmBookingAsync()
    {
        if (SelectedAddress == null || SelectedSlot == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<InterventionDto>("/services/bookings", new
            {
                serviceProviderId = Guid.Parse(ProviderId),
                serviceOfferingId = Guid.Parse(ServiceId),
                problemDescription = ProblemDescription,
                problemPhotos = ProblemPhotos.ToList(),
                scheduledDate = SelectedSlot.Date.ToString("yyyy-MM-dd"),
                scheduledStartTime = SelectedSlot.StartTime.ToString(@"hh\:mm"),
                scheduledEndTime = SelectedSlot.EndTime.ToString(@"hh\:mm"),
                addressId = SelectedAddress.Id,
                paymentTiming = PaymentTiming  // FIX: use property, not field (MVVMTK0034)
            });

            if (result.Success && result.Data != null)
            {
                await Shell.Current.DisplayAlert("Réservation confirmée",
                    $"N° {result.Data.InterventionNumber}", "OK");
                await Shell.Current.GoToAsync($"interventiontracking?id={result.Data.Id}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur",
                    result.Message ?? "Impossible de créer la réservation", "OK");
            }
        });
    }

    [RelayCommand]
    private async Task AddPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                using var stream = await photo.OpenReadAsync();
                var uploadResult = await _api.UploadAsync<string>("/files/upload", stream, photo.FileName);
                if (uploadResult.Success && uploadResult.Data != null)
                    ProblemPhotos.Add(uploadResult.Data);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                using var stream = await photo.OpenReadAsync();
                var uploadResult = await _api.UploadAsync<string>("/files/upload", stream, photo.FileName);
                if (uploadResult.Success && uploadResult.Data != null)
                    ProblemPhotos.Add(uploadResult.Data);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }
}
