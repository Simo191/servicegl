using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

[QueryProperty(nameof(DeliveryId), "id")]
public partial class DeliveryDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _deliveryId = string.Empty;
    [ObservableProperty] private DeliveryHistoryItem? _delivery;
    [ObservableProperty] private EarningDetail? _earningDetail;

    public DeliveryDetailViewModel(ApiService api)
    {
        _api = api;
        Title = "Détail livraison";
    }

    partial void OnDeliveryIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value)) _ = LoadDetailAsync();
    }

    [RelayCommand]
    private async Task LoadDetailAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<DeliveryHistoryItem>>(
                string.Format(AppConstants.DeliveryDetailEndpoint, DeliveryId));
            Delivery = result?.Data;
        });
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");
}