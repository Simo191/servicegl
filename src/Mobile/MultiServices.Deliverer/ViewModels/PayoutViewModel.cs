using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class PayoutViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private decimal _availableBalance;
    [ObservableProperty] private decimal _requestAmount;
    [ObservableProperty] private string _bankName = string.Empty;
    [ObservableProperty] private string _accountNumber = string.Empty;
    [ObservableProperty] private string _accountHolder = string.Empty;
    [ObservableProperty] private ObservableCollection<PayoutHistory> _payoutHistory = new();

    public PayoutViewModel(ApiService api)
    {
        _api = api;
        Title = "Demander un virement";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var summary = await _api.GetAsync<ApiResponse<EarningsSummary>>(AppConstants.EarningsSummaryEndpoint);
            if (summary?.Data != null) AvailableBalance = summary.Data.AvailableBalance;

            var history = await _api.GetAsync<ApiResponse<List<PayoutHistory>>>(AppConstants.PayoutHistoryEndpoint);
            if (history?.Data != null)
                PayoutHistory = new ObservableCollection<PayoutHistory>(history.Data);
        });
    }

    [RelayCommand]
    private async Task RequestPayoutAsync()
    {
        if (RequestAmount <= 0 || RequestAmount > AvailableBalance)
        {
            ErrorMessage = "Montant invalide"; HasError = true; return;
        }
        if (string.IsNullOrWhiteSpace(BankName) || string.IsNullOrWhiteSpace(AccountNumber))
        {
            ErrorMessage = "Remplissez les informations bancaires"; HasError = true; return;
        }

        await ExecuteAsync(async () =>
        {
            await _api.PostAsync<ApiResponse<object>>(AppConstants.PayoutRequestEndpoint,
                new PayoutRequest
                {
                    Amount = RequestAmount, BankName = BankName,
                    AccountNumber = AccountNumber, AccountHolder = AccountHolder
                });
            await Shell.Current.DisplayAlert("Succès", "Demande de virement envoyée", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}