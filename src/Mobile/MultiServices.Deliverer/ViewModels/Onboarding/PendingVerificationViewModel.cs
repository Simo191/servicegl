using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class PendingVerificationViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _statusMessage = "Vos documents sont en cours de vérification...";
    [ObservableProperty] private string _statusDetail = "Ce processus prend généralement entre 24 et 48 heures. Nous vous enverrons une notification dès que votre compte sera vérifié.";
    [ObservableProperty] private bool _isChecking;

    public PendingVerificationViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Vérification en cours";
    }

    [RelayCommand]
    private async Task CheckStatusAsync()
    {
        IsChecking = true;
        try
        {
            var profile = await _authService.GetDelivererProfileAsync();
            if (profile?.KycStatus == "Verified")
            {
                await Shell.Current.DisplayAlert("Félicitations ! 🎉", "Votre compte est vérifié. Bienvenue !", "Commencer");
                await Shell.Current.GoToAsync("//main/dashboard");
            }
            else if (profile?.KycStatus == "Rejected")
            {
                await Shell.Current.DisplayAlert("Refusé",
                    $"Vos documents ont été refusés. Raison: {profile.KycRejectionReason ?? "Non spécifiée"}. Veuillez les soumettre à nouveau.",
                    "Corriger");
                await Shell.Current.GoToAsync("//documentUpload");
            }
            else
            {
                StatusMessage = "Toujours en attente de vérification...";
                StatusDetail = "Nous travaillons aussi vite que possible. Merci de votre patience !";
            }
        }
        catch { }
        finally { IsChecking = false; }
    }

    [RelayCommand]
    private async Task ContactSupportAsync()
    {
        await Shell.Current.DisplayAlert("Support", "Envoyez un email à support@multiservices.ma ou appelez le 0800-123-456", "OK");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
    }
}