using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class SupportViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<FaqItem> _faqItems = new();
    [ObservableProperty] private ObservableCollection<SupportTicket> _tickets = new();
    [ObservableProperty] private bool _showNewTicket;
    [ObservableProperty] private string _ticketSubject = string.Empty;
    [ObservableProperty] private string _ticketDescription = string.Empty;
    [ObservableProperty] private string _ticketCategory = string.Empty;
    [ObservableProperty] private int _selectedTab; // 0 = FAQ, 1 = Tickets

    public List<string> Categories { get; } = new()
    {
        "Technique", "Paiement", "Livraison", "Compte", "Autre"
    };

    public SupportViewModel(ApiService api)
    {
        _api = api;
        Title = "Support";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var faqResult = await _api.GetAsync<ApiResponse<List<FaqItem>>>(AppConstants.FaqEndpoint);
            if (faqResult?.Data != null)
                FaqItems = new ObservableCollection<FaqItem>(faqResult.Data);

            var ticketResult = await _api.GetAsync<ApiResponse<List<SupportTicket>>>(AppConstants.SupportTicketsEndpoint);
            if (ticketResult?.Data != null)
                Tickets = new ObservableCollection<SupportTicket>(ticketResult.Data);
        });
    }

    [RelayCommand]
    private void ToggleNewTicket() => ShowNewTicket = !ShowNewTicket;

    [RelayCommand]
    private async Task SubmitTicketAsync()
    {
        if (string.IsNullOrWhiteSpace(TicketSubject) || string.IsNullOrWhiteSpace(TicketDescription))
        {
            await Shell.Current.DisplayAlert("Erreur", "Sujet et description requis", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<ApiResponse<SupportTicket>>(AppConstants.SupportTicketsEndpoint,
                new { Subject = TicketSubject, Description = TicketDescription, Category = TicketCategory });

            if (result?.Data != null)
            {
                Tickets.Insert(0, result.Data);
                TicketSubject = string.Empty;
                TicketDescription = string.Empty;
                ShowNewTicket = false;
                await Shell.Current.DisplayAlert("Envoyé", "Votre ticket a été créé", "OK");
            }
        });
    }

    [RelayCommand]
    private async Task CallSupportAsync()
    {
        try { await Launcher.OpenAsync(new Uri($"tel:{AppConstants.SupportPhone}")); }
        catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'ouvrir le téléphone", "OK"); }
    }
}
