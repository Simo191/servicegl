using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Services;

[QueryProperty(nameof(ProviderId), "id")]
public partial class ServiceDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _providerId = string.Empty;
    [ObservableProperty] private ServiceProviderDto? _provider;
    [ObservableProperty] private ObservableCollection<ServiceOfferingDto> _services = new();
    [ObservableProperty] private ObservableCollection<PortfolioItemDto> _portfolio = new();
    [ObservableProperty] private ObservableCollection<ReviewDto> _reviews = new();
    [ObservableProperty] private ServiceOfferingDto? _selectedService;
    [ObservableProperty] private string _activeTab = "services";
    [ObservableProperty] private bool _isFavorite;

    public ServiceDetailViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnProviderIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadProviderCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadProviderAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ServiceProviderDto>($"/services/providers/{ProviderId}");
            if (result.Success && result.Data != null)
            {
                Provider = result.Data;
                Title = result.Data.CompanyName;
                Services = new ObservableCollection<ServiceOfferingDto>(result.Data.Services);
                Portfolio = new ObservableCollection<PortfolioItemDto>(result.Data.Portfolio);
                Reviews = new ObservableCollection<ReviewDto>(result.Data.Reviews);
            }
        });
    }

    [RelayCommand]
    private async Task BookServiceAsync(ServiceOfferingDto service)
    {
        SelectedService = service;
        await Shell.Current.GoToAsync($"bookservice?providerId={ProviderId}&serviceId={service.Id}");
    }

    [RelayCommand]
    private void SetTab(string tab) => ActiveTab = tab;

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        IsFavorite = !IsFavorite;
        if (IsFavorite)
            await _api.PostAsync<object>($"/favorites/services/{ProviderId}");
        else
            await _api.DeleteAsync<object>($"/favorites/services/{ProviderId}");
    }
}
