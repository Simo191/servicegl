using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.Services.Location;
using MultiServices.Deliverer.Services.Storage;
using MultiServices.Deliverer.ViewModels;
using MultiServices.Deliverer.ViewModels.Onboarding;
using MultiServices.Deliverer.Views;
using MultiServices.Deliverer.Views.Onboarding;

namespace MultiServices.Deliverer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Services ──
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddHttpClient<ApiService>(c =>
        {
            c.BaseAddress = new Uri(Helpers.AppConstants.ApiBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        // ── Onboarding ViewModels ──
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<VehicleSelectionViewModel>();
        builder.Services.AddTransient<DocumentUploadViewModel>();
        builder.Services.AddTransient<TrainingViewModel>();
        builder.Services.AddTransient<TrainingQuizViewModel>();
        builder.Services.AddTransient<PendingVerificationViewModel>();

        // ── Main ViewModels ──
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ActiveDeliveryViewModel>();
        builder.Services.AddTransient<DeliveryHistoryViewModel>();
        builder.Services.AddTransient<DeliveryDetailViewModel>();
        builder.Services.AddTransient<ReportProblemViewModel>();
        builder.Services.AddTransient<EarningsViewModel>();
        builder.Services.AddTransient<EarningDetailViewModel>();
        builder.Services.AddTransient<PayoutViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<EditProfileViewModel>();
        builder.Services.AddTransient<EditVehicleViewModel>();
        builder.Services.AddTransient<EmergencyContactViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<SupportViewModel>();
        builder.Services.AddTransient<SOSViewModel>();

        // ── Onboarding Pages ──
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<VehicleSelectionPage>();
        builder.Services.AddTransient<DocumentUploadPage>();
        builder.Services.AddTransient<TrainingPage>();
        builder.Services.AddTransient<TrainingQuizPage>();
        builder.Services.AddTransient<PendingVerificationPage>();

        // ── Main Pages ──
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ActiveDeliveryPage>();
        builder.Services.AddTransient<DeliveryHistoryPage>();
        builder.Services.AddTransient<DeliveryDetailPage>();
        builder.Services.AddTransient<ReportProblemPage>();
        builder.Services.AddTransient<EarningsPage>();
        builder.Services.AddTransient<EarningDetailPage>();
        builder.Services.AddTransient<PayoutPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<EditProfilePage>();
        builder.Services.AddTransient<EditVehiclePage>();
        builder.Services.AddTransient<EmergencyContactPage>();
        builder.Services.AddTransient<StatsPage>();
        builder.Services.AddTransient<SupportPage>();
        builder.Services.AddTransient<SOSPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
