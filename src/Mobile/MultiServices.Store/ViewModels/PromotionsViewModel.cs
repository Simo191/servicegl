using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
public partial class PromotionsViewModel : BaseViewModel {
    private readonly ApiService _api; [ObservableProperty] private ObservableCollection<StorePromotionDto> _promotions = new();
    public PromotionsViewModel(ApiService api) { _api = api; Title = "Promotions"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<List<StorePromotionDto>>("/grocery/store/promotions");
        if (r.Success && r.Data != null) Promotions = new(r.Data);
    }); }
    [RelayCommand] private async Task CreateAsync() {
        string type = await Shell.Current.DisplayActionSheet("Type promotion", "Annuler", null, "Pourcentage (%)", "Montant fixe (MAD)", "Livraison gratuite", "X acheté Y offert");
        if (type == null || type == "Annuler") return;
        string title = await Shell.Current.DisplayPromptAsync("Titre", "Nom de la promo:"); if (title == null) return;
        await _api.PostAsync<StorePromotionDto>("/grocery/store/promotions", new { title, discountType = type, startDate = DateTime.Now, endDate = DateTime.Now.AddDays(7) });
        await LoadAsync();
    }
    [RelayCommand] private async Task ToggleAsync(StorePromotionDto p) { await _api.PutAsync<object>($"/grocery/store/promotions/{p.Id}", new { isActive = !p.IsActive }); await LoadAsync(); }
    [RelayCommand] private async Task DeleteAsync(StorePromotionDto p) { if (await Shell.Current.DisplayAlert("Confirmer", "Supprimer?", "Oui", "Non")) { await _api.DeleteAsync<object>($"/grocery/store/promotions/{p.Id}"); await LoadAsync(); } }
}