using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Services;

[QueryProperty(nameof(InterventionId), "id")]
public partial class InterventionTrackingViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _interventionId = string.Empty;
    [ObservableProperty] private InterventionDto? _intervention;
    [ObservableProperty] private ObservableCollection<InterventionStatusHistoryDto> _statusHistory = new();

    public InterventionTrackingViewModel(ApiService api)
    {
        _api = api;
        Title = "Suivi intervention";
    }

    partial void OnInterventionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadInterventionCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadInterventionAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<InterventionDto>($"/services/interventions/{InterventionId}");
            if (result.Success && result.Data != null)
            {
                Intervention = result.Data;
                StatusHistory = new ObservableCollection<InterventionStatusHistoryDto>(result.Data.StatusHistory);
            }
        });
    }

    [RelayCommand]
    private async Task CallIntervenantAsync()
    {
        if (Intervention?.IntervenantPhone != null)
        {
            try { PhoneDialer.Default.Open(Intervention.IntervenantPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }
}
