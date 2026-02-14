using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
[QueryProperty(nameof(OrderId), "id")]
public partial class OrderDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private string _orderId = string.Empty;
    [ObservableProperty] private RestaurantOrderDto? _order;
    [ObservableProperty] private bool _hasInstructions;
    [ObservableProperty] private bool _canAccept; [ObservableProperty] private bool _canPrepare;
    [ObservableProperty] private bool _canReady; [ObservableProperty] private bool _canReject;
    public OrderDetailViewModel(ApiService api) { _api = api; }
    partial void OnOrderIdChanged(string v) { if (!string.IsNullOrEmpty(v)) LoadCommand.ExecuteAsync(null); }

    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<RestaurantOrderDto>($"/restaurant/orders/{OrderId}");
        if (r.Success && r.Data != null) { Order = r.Data; HasInstructions = !string.IsNullOrEmpty(r.Data.SpecialInstructions);
            CanAccept = r.Data.Status == "Pending"; CanPrepare = r.Data.Status == "Accepted"; CanReady = r.Data.Status == "Preparing"; CanReject = r.Data.Status == "Pending"; }
    }); }
    [RelayCommand] private async Task AcceptAsync() { string m = await Shell.Current.DisplayPromptAsync("Temps", "Minutes:", keyboard: Keyboard.Numeric, initialValue: "20"); if (m != null) { await _api.PostAsync<object>($"/restaurant/orders/{OrderId}/accept", new { estimatedMinutes = int.Parse(m) }); await LoadAsync(); } }
    [RelayCommand] private async Task RejectAsync() { string r = await Shell.Current.DisplayPromptAsync("Refus", "Motif:"); if (r != null) { await _api.PostAsync<object>($"/restaurant/orders/{OrderId}/reject", new { reason = r }); await Shell.Current.GoToAsync(".."); } }
    [RelayCommand] private async Task PrepareAsync() { await _api.PostAsync<object>($"/restaurant/orders/{OrderId}/status", new { status = "Preparing" }); await LoadAsync(); }
    [RelayCommand] private async Task ReadyAsync() { string note = await Shell.Current.DisplayPromptAsync("Note livreur", "Note (optionnel):", "Prêt!", "Annuler"); await _api.PostAsync<object>($"/restaurant/orders/{OrderId}/status", new { status = "Ready", note = note ?? "" }); await Shell.Current.GoToAsync(".."); }
    [RelayCommand] private async Task CallCustomerAsync() { if (Order?.CustomerPhone != null) try { PhoneDialer.Default.Open(Order.CustomerPhone); } catch { } }
    [RelayCommand] private async Task PrintAsync() { await Shell.Current.DisplayAlert("Impression", "Envoi vers imprimante...", "OK"); }
}