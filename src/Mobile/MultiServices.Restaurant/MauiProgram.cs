using CommunityToolkit.Maui; using Microsoft.Extensions.Logging;
using MultiServices.Restaurant.Services.Api; using MultiServices.Restaurant.Services.Auth; using MultiServices.Restaurant.Services.Storage;
using MultiServices.Restaurant.ViewModels; using MultiServices.Restaurant.Views.Auth; using MultiServices.Restaurant.Views.Dashboard;
using MultiServices.Restaurant.Views.Orders; using MultiServices.Restaurant.Views.Menu; using MultiServices.Restaurant.Views.Promotions;
using MultiServices.Restaurant.Views.Stats; using MultiServices.Restaurant.Views.Reviews; using MultiServices.Restaurant.Views.Settings;
namespace MultiServices.Restaurant;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit().ConfigureFonts(f => { f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); });
        // Services
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddHttpClient<ApiService>(c => { c.BaseAddress = new Uri(Helpers.AppConstants.ApiBaseUrl); c.Timeout = TimeSpan.FromSeconds(30); });
        // ViewModels
        builder.Services.AddTransient<LoginViewModel>(); builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<OrdersKanbanViewModel>(); builder.Services.AddTransient<OrderDetailViewModel>();
        builder.Services.AddTransient<MenuManagementViewModel>(); builder.Services.AddTransient<MenuItemFormViewModel>();
        builder.Services.AddTransient<PromotionsViewModel>(); builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<ReviewsViewModel>(); builder.Services.AddTransient<SettingsViewModel>();
        // Pages
        builder.Services.AddTransient<LoginPage>(); builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<OrdersKanbanPage>(); builder.Services.AddTransient<OrderDetailPage>();
        builder.Services.AddTransient<MenuManagementPage>(); builder.Services.AddTransient<MenuItemFormPage>();
        builder.Services.AddTransient<CategoryFormPage>(); builder.Services.AddTransient<PromotionsPage>();
        builder.Services.AddTransient<StatsPage>(); builder.Services.AddTransient<ReviewsPage>();
        builder.Services.AddTransient<SettingsPage>(); builder.Services.AddTransient<StaffPage>();
        builder.Services.AddTransient<OpeningHoursPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}