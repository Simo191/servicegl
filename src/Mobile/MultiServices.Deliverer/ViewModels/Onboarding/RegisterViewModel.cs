using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private int _selectedVehicleIndex;

    public List<string> VehicleTypes { get; } = new() { "Vélo", "Scooter", "Moto", "Voiture", "Van" };
    public List<string> Cities { get; } = new() { "Casablanca", "Rabat", "Marrakech", "Fès", "Tanger", "Agadir", "Meknès", "Oujda", "Kénitra", "Tétouan" };

    private static readonly string[] VehicleCodes = { "Bicycle", "Scooter", "Motorcycle", "Car", "Van" };

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Inscription Livreur";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(City))
        {
            ErrorMessage = "Veuillez remplir tous les champs obligatoires";
            HasError = true;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Les mots de passe ne correspondent pas";
            HasError = true;
            return;
        }

        if (Password.Length < 8)
        {
            ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères";
            HasError = true;
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new DelivererRegistrationRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                PhoneNumber = PhoneNumber,
                City = City,
                VehicleType = VehicleCodes[Math.Clamp(SelectedVehicleIndex, 0, VehicleCodes.Length - 1)]
            };

            var success = await _authService.RegisterAsync(request);
            if (success)
                await Shell.Current.GoToAsync("//documentUpload");
            else
            {
                ErrorMessage = "Échec de l'inscription. Cet email existe peut-être déjà.";
                HasError = true;
            }
        }, "Erreur lors de l'inscription");
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}