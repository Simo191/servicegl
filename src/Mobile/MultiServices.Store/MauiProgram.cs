using CommunityToolkit.Maui; using Microsoft.Extensions.Logging;
using MultiServices.Store.Services.Api; using MultiServices.Store.Services.Auth; using MultiServices.Store.Services.Storage;
using MultiServices.Store.ViewModels; using MultiServices.Store.Views.Auth; using MultiServices.Store.Views.Dashboard;
using MultiServices.Store.Views.Orders; using MultiServices.Store.Views.Catalog; using MultiServices.Store.Views.Stock;
using MultiServices.Store.Views.Stats; using MultiServices.Store.Views.Promotions; using MultiServices.Store.Views.Settings;
namespace MultiServices.Store;
public static class MauiProgram { public static MauiApp CreateMauiApp() {
    var builder = MauiApp.CreateBuilder();
    builder.UseMauiApp<App>().UseMauiCommunityToolkit().ConfigureFonts(f => { f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });
    builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>(); builder.Services.AddSingleton<AuthService>();
    builder.Services.AddHttpClient<ApiService>(c => { c.BaseAddress = new Uri(Helpers.AppConstants.ApiBaseUrl); c.Timeout = TimeSpan.FromSeconds(30); });
    // VMs
    builder.Services.AddTransient<LoginViewModel>(); builder.Services.AddTransient<DashboardViewModel>(); builder.Services.AddTransient<OrdersViewModel>();
    builder.Services.AddTransient<OrderDetailViewModel>(); builder.Services.AddTransient<PickingViewModel>(); builder.Services.AddTransient<CatalogViewModel>();
    builder.Services.AddTransient<ProductFormViewModel>(); builder.Services.AddTransient<StockViewModel>(); builder.Services.AddTransient<StatsViewModel>();
    builder.Services.AddTransient<PromotionsViewModel>(); builder.Services.AddTransient<SettingsViewModel>();
    // Pages
    builder.Services.AddTransient<LoginPage>(); builder.Services.AddTransient<DashboardPage>(); builder.Services.AddTransient<OrdersPage>();
    builder.Services.AddTransient<OrderDetailPage>(); builder.Services.AddTransient<PickingPage>(); builder.Services.AddTransient<CatalogPage>();
    builder.Services.AddTransient<ProductFormPage>(); builder.Services.AddTransient<BulkImportPage>(); builder.Services.AddTransient<StockPage>();
    builder.Services.AddTransient<StatsPage>(); builder.Services.AddTransient<PromotionsPage>(); builder.Services.AddTransient<SettingsPage>();
#if DEBUG
    builder.Logging.AddDebug();
#endif
    return builder.Build();
} }