using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MultiServices.Maui.Services;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;
using MultiServices.Maui.Services.Location;
using MultiServices.Maui.Services.Notification;
using MultiServices.Maui.Services.Storage;
using MultiServices.Maui.ViewModels.Auth;
using MultiServices.Maui.ViewModels.Common;
using MultiServices.Maui.ViewModels.Grocery;
using MultiServices.Maui.ViewModels.Profile;
using MultiServices.Maui.ViewModels.Restaurant;
using MultiServices.Maui.ViewModels.Services;
using MultiServices.Maui.Views.Auth;
using MultiServices.Maui.Views.Common;
using MultiServices.Maui.Views.Grocery;
using MultiServices.Maui.Views.Profile;
using MultiServices.Maui.Views.Restaurant;
using MultiServices.Maui.Views.Services;

namespace MultiServices.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
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

        // Services
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<LocationService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AddressService>();
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri(Helpers.AppConstants.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<OrdersViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<RestaurantsViewModel>();
        builder.Services.AddTransient<RestaurantDetailViewModel>();
        builder.Services.AddTransient<RestaurantOrderTrackingViewModel>();
        builder.Services.AddTransient<ServicesViewModel>();
        builder.Services.AddTransient<ServiceDetailViewModel>();
        builder.Services.AddTransient<BookServiceViewModel>();
        builder.Services.AddTransient<InterventionTrackingViewModel>();
        builder.Services.AddTransient<GroceryStoresViewModel>();
        builder.Services.AddTransient<StoreDetailViewModel>();
        builder.Services.AddTransient<ShoppingListsViewModel>();
        builder.Services.AddTransient<AddressEntryViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ForgotPasswordPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<OrdersPage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<EditProfilePage>();
        builder.Services.AddTransient<AddressesPage>();
        builder.Services.AddTransient<SupportPage>();
        builder.Services.AddTransient<RestaurantsPage>();
        builder.Services.AddTransient<RestaurantDetailPage>();
        builder.Services.AddTransient<RestaurantOrderTrackingPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<ServiceDetailPage>();
        builder.Services.AddTransient<BookServicePage>();
        builder.Services.AddTransient<InterventionTrackingPage>();
        builder.Services.AddTransient<GroceryStoresPage>();
        builder.Services.AddTransient<StoreDetailPage>();
        builder.Services.AddTransient<ShoppingListsPage>();
        builder.Services.AddTransient<GroceryCheckoutPage>();
        builder.Services.AddTransient<AddressEntryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            throw;
        }
    }
}
