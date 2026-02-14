using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
public partial class PromotionsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<PromotionDto> _promotions = new();
    [ObservableProperty] private ObservableCollection<PromotionDto> _activePromos = new();
    [ObservableProperty] private ObservableCollection<PromotionDto> _expiredPromos = new();
    public PromotionsViewModel(ApiService api) { _api = api; Title = "Promotions"; }

    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<List<PromotionDto>>("/restaurant/promotions");
        if (r.Success && r.Data != null) { Promotions = new(r.Data); ActivePromos = new(r.Data.Where(p => p.IsActive)); ExpiredPromos = new(r.Data.Where(p => !p.IsActive)); }
    }); }

    [RelayCommand] private async Task CreatePromotionAsync()
    {
        string type = await Shell.Current.DisplayActionSheet("Type de promotion", "Annuler", null, "Pourcentage (%)", "Montant fixe (MAD)", "Livraison gratuite", "X acheté Y offert", "Happy Hour");
        if (type == null || type == "Annuler") return;
        string title = await Shell.Current.DisplayPromptAsync("Titre", "Nom de la promotion:");
        if (title == null) return;
        string val = "0";
        if (type != "Livraison gratuite") { val = await Shell.Current.DisplayPromptAsync("Valeur", "Valeur de la réduction:", keyboard: Keyboard.Numeric) ?? "0"; }
        string discountType = type switch { "Pourcentage (%)" => "Percentage", "Montant fixe (MAD)" => "FixedAmount", "Livraison gratuite" => "FreeDelivery", "X acheté Y offert" => "BuyXGetY", "Happy Hour" => "HappyHour", _ => "Percentage" };
        await _api.PostAsync<PromotionDto>("/restaurant/promotions", new { title, discountType, discountValue = decimal.Parse(val), startDate = DateTime.Now, endDate = DateTime.Now.AddDays(30) });
        await LoadAsync();
    }

    [RelayCommand] private async Task TogglePromotionAsync(PromotionDto p) { await _api.PutAsync<object>($"/restaurant/promotions/{p.Id}", new { isActive = !p.IsActive }); await LoadAsync(); }
    [RelayCommand] private async Task DeletePromotionAsync(PromotionDto p) { if (await Shell.Current.DisplayAlert("Confirmer", $"Supprimer '{p.Title}'?", "Oui", "Non")) { await _api.DeleteAsync<object>($"/restaurant/promotions/{p.Id}"); await LoadAsync(); } }
}