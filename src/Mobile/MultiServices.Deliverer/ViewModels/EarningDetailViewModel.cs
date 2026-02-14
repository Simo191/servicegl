using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

[QueryProperty(nameof(EarningId), "earningId")]
public partial class EarningDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _earningId = string.Empty;
    [ObservableProperty] private EarningDetail? _earning;

    public EarningDetailViewModel(ApiService api)
    {
        _api = api;
        Title = "Détail du gain";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<EarningDetail>>(
                $"{AppConstants.EarningsHistoryEndpoint}/{EarningId}");
            if (result?.Data != null)
                Earning = result.Data;
        });
    }
}
