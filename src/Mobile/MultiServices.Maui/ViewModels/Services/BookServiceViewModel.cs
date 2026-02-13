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

    public BookServiceViewModel(ApiService api)
    {
        _api = api;
        Title = "Réserver un service";
    }

    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        var result = await _api.GetAsync<List<AddressDto>>("/profile/addresses");
        if (result.Success && result.Data != null)
        {
            Addresses = new ObservableCollection<AddressDto>(result.Data);
            SelectedAddress = Addresses.FirstOrDefault(a => a.IsDefault) ?? Addresses.FirstOrDefault();
        }
    }

    [RelayCommand]
    private async Task LoadSlotsAsync()
    {
        var queryParams = new Dictionary<string, string>
        {
            ["date"] = SelectedDate.ToString("yyyy-MM-dd")
        };
        var result = await _api.GetAsync<List<AvailableSlotDto>>($"/services/providers/{ProviderId}/slots", queryParams);
        if (result.Success && result.Data != null)
            AvailableSlots = new ObservableCollection<AvailableSlotDto>(result.Data);
    }

    [RelayCommand]
    private async Task AddPhotoAsync()
    {
        var result = await MediaPicker.Default.CapturePhotoAsync();
        if (result != null)
            ProblemPhotos.Add(result.FullPath);
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        var result = await MediaPicker.Default.PickPhotoAsync();
        if (result != null)
            ProblemPhotos.Add(result.FullPath);
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
            await Shell.Current.DisplayAlert("Erreur", "Veuillez compléter tous les champs", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new
            {
                serviceOfferingId = Guid.Parse(ServiceId),
                problemDescription = ProblemDescription,
                addressId = SelectedAddress.Id,
                scheduledDate = SelectedDate,
                scheduledStartTime = SelectedSlot.StartTime,
                paymentTiming = PaymentTiming
            };

            var result = await _api.PostAsync<InterventionDto>("/services/interventions", request);
            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Succès", "Votre réservation a été confirmée!", "OK");
                await Shell.Current.GoToAsync("../..");
            }
        });
    }
}
