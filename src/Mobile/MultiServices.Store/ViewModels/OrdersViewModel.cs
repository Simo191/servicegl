using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
public partial class OrdersViewModel : BaseViewModel {
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<GroceryOrderDto> _orders = new(); [ObservableProperty] private string _filter = "Received";
    public OrdersViewModel(ApiService api) { _api = api; Title = "Commandes"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var qp = new Dictionary<string, string> { ["pageSize"] = "50" }; if (!string.IsNullOrEmpty(Filter)) qp["status"] = Filter;
        var r = await _api.GetAsync<PaginatedResult<GroceryOrderDto>>("/grocery/store/orders", qp);
        if (r.Success && r.Data != null) Orders = new(r.Data.Items);
    }); }
    [RelayCommand] private async Task FilterAsync(string s) { Filter = s; await LoadAsync(); }
    [RelayCommand] private async Task ViewOrderAsync(GroceryOrderDto o) => await Shell.Current.GoToAsync($"orderdetail?id={o.Id}");
    [RelayCommand] private async Task StartPickingAsync(GroceryOrderDto o) { await _api.PostAsync<object>($"/grocery/store/orders/{o.Id}/status", new { status = "Preparing" }); await Shell.Current.GoToAsync($"picking?id={o.Id}"); }
    [RelayCommand] private async Task AssignPreparerAsync(GroceryOrderDto o) { string name = await Shell.Current.DisplayPromptAsync("Préparateur", "Nom du préparateur:"); if (name != null) { await _api.PostAsync<object>($"/grocery/store/orders/{o.Id}/assign-preparer", new { preparerName = name }); await LoadAsync(); } }
    [RelayCommand] private async Task MarkReadyAsync(GroceryOrderDto o) { await _api.PostAsync<object>($"/grocery/store/orders/{o.Id}/ready"); await LoadAsync(); }
    [RelayCommand] private async Task PrintPickingListAsync(GroceryOrderDto o) { await Shell.Current.DisplayAlert("Impression", "Bon de préparation envoyé à l'imprimante", "OK"); }
}