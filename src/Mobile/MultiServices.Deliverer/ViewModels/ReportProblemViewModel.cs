using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

[QueryProperty(nameof(DeliveryId), "deliveryId")]
public partial class ReportProblemViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _deliveryId = string.Empty;
    [ObservableProperty] private string _selectedReason = string.Empty;
    [ObservableProperty] private string _description = string.Empty;

    public ObservableCollection<ProblemReason> Reasons { get; } = new()
    {
        new("incorrect_address", "Adresse incorrecte", "📍"),
        new("client_absent", "Client absent", "🚪"),
        new("damaged_product", "Produit endommagé", "📦"),
        new("restaurant_closed", "Restaurant fermé", "🔒"),
        new("incorrect_order", "Commande incorrecte", "❌"),
        new("security_issue", "Problème de sécurité", "⚠️"),
        new("other", "Autre", "💬")
    };

    public ReportProblemViewModel(ApiService api)
    {
        _api = api;
        Title = "Signaler un problème";
    }

    [RelayCommand]
    private void SelectReason(ProblemReason reason)
    {
        SelectedReason = reason.Code;
        foreach (var r in Reasons) r.IsSelected = r.Code == reason.Code;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrEmpty(SelectedReason))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un motif", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _api.PostAsync<ApiResponse<object>>(
                string.Format(AppConstants.ReportProblemEndpoint, DeliveryId),
                new { Reason = SelectedReason, Description });
            await Shell.Current.DisplayAlert("Envoyé", "Le problème a été signalé. Notre équipe va le traiter.", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}
