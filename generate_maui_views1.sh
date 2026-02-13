#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# RESOURCES - STYLES & COLORS
# ============================================================
cat > "$BASE/Resources/Styles/Colors.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Color x:Key="Primary">#6366F1</Color>
    <Color x:Key="PrimaryDark">#4F46E5</Color>
    <Color x:Key="PrimaryLight">#A5B4FC</Color>
    <Color x:Key="Secondary">#8B5CF6</Color>
    <Color x:Key="Accent">#EC4899</Color>
    <Color x:Key="Success">#10B981</Color>
    <Color x:Key="Warning">#F59E0B</Color>
    <Color x:Key="Danger">#EF4444</Color>
    <Color x:Key="Info">#3B82F6</Color>
    <Color x:Key="RestaurantColor">#F59E0B</Color>
    <Color x:Key="ServiceColor">#3B82F6</Color>
    <Color x:Key="GroceryColor">#10B981</Color>
    <Color x:Key="TextPrimary">#1F2937</Color>
    <Color x:Key="TextSecondary">#6B7280</Color>
    <Color x:Key="TextMuted">#9CA3AF</Color>
    <Color x:Key="Background">#F9FAFB</Color>
    <Color x:Key="Surface">#FFFFFF</Color>
    <Color x:Key="Border">#E5E7EB</Color>
    <Color x:Key="CardShadow">#1A000000</Color>
</ResourceDictionary>
EOF

cat > "$BASE/Resources/Styles/Styles.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <Style TargetType="Page" ApplyToDerivedTypes="True">
        <Setter Property="BackgroundColor" Value="{StaticResource Background}" />
    </Style>

    <Style x:Key="Title1" TargetType="Label">
        <Setter Property="FontSize" Value="28" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
    </Style>

    <Style x:Key="Title2" TargetType="Label">
        <Setter Property="FontSize" Value="22" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
    </Style>

    <Style x:Key="Title3" TargetType="Label">
        <Setter Property="FontSize" Value="18" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
    </Style>

    <Style x:Key="BodyText" TargetType="Label">
        <Setter Property="FontSize" Value="15" />
        <Setter Property="TextColor" Value="{StaticResource TextSecondary}" />
    </Style>

    <Style x:Key="Caption" TargetType="Label">
        <Setter Property="FontSize" Value="12" />
        <Setter Property="TextColor" Value="{StaticResource TextMuted}" />
    </Style>

    <Style x:Key="PrimaryButton" TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="FontSize" Value="16" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HeightRequest" Value="50" />
        <Setter Property="Padding" Value="20,0" />
    </Style>

    <Style x:Key="SecondaryButton" TargetType="Button">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{StaticResource Primary}" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="FontSize" Value="15" />
        <Setter Property="BorderColor" Value="{StaticResource Primary}" />
        <Setter Property="BorderWidth" Value="2" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HeightRequest" Value="50" />
    </Style>

    <Style x:Key="DangerButton" TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource Danger}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HeightRequest" Value="50" />
    </Style>

    <Style x:Key="CardFrame" TargetType="Frame">
        <Setter Property="BackgroundColor" Value="{StaticResource Surface}" />
        <Setter Property="CornerRadius" Value="16" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="HasShadow" Value="True" />
        <Setter Property="BorderColor" Value="Transparent" />
    </Style>

    <Style x:Key="EntryStyle" TargetType="Entry">
        <Setter Property="FontSize" Value="15" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
        <Setter Property="PlaceholderColor" Value="{StaticResource TextMuted}" />
        <Setter Property="BackgroundColor" Value="{StaticResource Surface}" />
        <Setter Property="HeightRequest" Value="50" />
    </Style>

    <Style x:Key="SearchBarStyle" TargetType="SearchBar">
        <Setter Property="FontSize" Value="15" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
        <Setter Property="PlaceholderColor" Value="{StaticResource TextMuted}" />
        <Setter Property="BackgroundColor" Value="{StaticResource Surface}" />
        <Setter Property="CancelButtonColor" Value="{StaticResource Primary}" />
    </Style>

    <Style x:Key="BadgeLabel" TargetType="Label">
        <Setter Property="FontSize" Value="11" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="Padding" Value="8,2" />
        <Setter Property="HorizontalOptions" Value="Start" />
    </Style>
</ResourceDictionary>
EOF

# ============================================================
# APP SHELL
# ============================================================
cat > "$BASE/AppShell.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:auth="clr-namespace:MultiServices.Maui.Views.Auth"
       xmlns:common="clr-namespace:MultiServices.Maui.Views.Common"
       xmlns:restaurant="clr-namespace:MultiServices.Maui.Views.Restaurant"
       xmlns:services="clr-namespace:MultiServices.Maui.Views.Services"
       xmlns:grocery="clr-namespace:MultiServices.Maui.Views.Grocery"
       xmlns:profile="clr-namespace:MultiServices.Maui.Views.Profile"
       x:Class="MultiServices.Maui.AppShell"
       Shell.FlyoutBehavior="Disabled"
       Shell.TabBarBackgroundColor="White"
       Shell.TabBarForegroundColor="{StaticResource Primary}"
       Shell.TabBarUnselectedColor="{StaticResource TextMuted}"
       Shell.TabBarTitleColor="{StaticResource Primary}">

    <!-- Auth -->
    <ShellContent Route="login"
                  ContentTemplate="{DataTemplate auth:LoginPage}"
                  Shell.TabBarIsVisible="False"
                  Shell.NavBarIsVisible="False" />

    <ShellContent Route="register"
                  ContentTemplate="{DataTemplate auth:RegisterPage}"
                  Shell.TabBarIsVisible="False"
                  Shell.NavBarIsVisible="False" />

    <!-- Main TabBar -->
    <TabBar Route="main">
        <Tab Title="Accueil" Icon="home_icon.png">
            <ShellContent Route="home"
                          ContentTemplate="{DataTemplate common:HomePage}" />
        </Tab>
        <Tab Title="Restos" Icon="restaurant_icon.png">
            <ShellContent Route="restaurants"
                          ContentTemplate="{DataTemplate restaurant:RestaurantsPage}" />
        </Tab>
        <Tab Title="Services" Icon="service_icon.png">
            <ShellContent Route="services"
                          ContentTemplate="{DataTemplate services:ServicesPage}" />
        </Tab>
        <Tab Title="Courses" Icon="grocery_icon.png">
            <ShellContent Route="grocery"
                          ContentTemplate="{DataTemplate grocery:GroceryStoresPage}" />
        </Tab>
        <Tab Title="Profil" Icon="profile_icon.png">
            <ShellContent Route="profile"
                          ContentTemplate="{DataTemplate profile:ProfilePage}" />
        </Tab>
    </TabBar>
</Shell>
EOF

cat > "$BASE/AppShell.xaml.cs" << 'EOF'
using MultiServices.Maui.Views.Auth;
using MultiServices.Maui.Views.Common;
using MultiServices.Maui.Views.Restaurant;
using MultiServices.Maui.Views.Services;
using MultiServices.Maui.Views.Grocery;
using MultiServices.Maui.Views.Profile;

namespace MultiServices.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Auth routes
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));

        // Restaurant routes
        Routing.RegisterRoute("restaurantdetail", typeof(RestaurantDetailPage));
        Routing.RegisterRoute("restaurantordertracking", typeof(RestaurantOrderTrackingPage));
        Routing.RegisterRoute("checkout", typeof(CheckoutPage));

        // Service routes
        Routing.RegisterRoute("servicedetail", typeof(ServiceDetailPage));
        Routing.RegisterRoute("bookservice", typeof(BookServicePage));
        Routing.RegisterRoute("interventiontracking", typeof(InterventionTrackingPage));

        // Grocery routes
        Routing.RegisterRoute("storedetail", typeof(StoreDetailPage));
        Routing.RegisterRoute("shoppinglists", typeof(ShoppingListsPage));
        Routing.RegisterRoute("grocerycheckout", typeof(GroceryCheckoutPage));

        // Common routes
        Routing.RegisterRoute("notifications", typeof(NotificationsPage));
        Routing.RegisterRoute("orders", typeof(OrdersPage));

        // Profile routes
        Routing.RegisterRoute("editprofile", typeof(EditProfilePage));
        Routing.RegisterRoute("addresses", typeof(AddressesPage));
        Routing.RegisterRoute("support", typeof(SupportPage));
    }
}
EOF

# ============================================================
# APP.XAML
# ============================================================
cat > "$BASE/App.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
EOF

cat > "$BASE/App.xaml.cs" << 'EOF'
namespace MultiServices.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
EOF

# ============================================================
# MAUI PROGRAM
# ============================================================
cat > "$BASE/MauiProgram.cs" << 'EOF'
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
EOF

# ============================================================
# PLATFORMS
# ============================================================
cat > "$BASE/Platforms/Android/AndroidManifest.xml" << 'EOF'
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true"
                 android:usesCleartextTraffic="true">
    </application>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    <uses-permission android:name="android.permission.USE_BIOMETRIC" />
    <uses-permission android:name="android.permission.CALL_PHONE" />
    <uses-permission android:name="android.permission.VIBRATE" />
    <uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
    <uses-permission android:name="android.permission.SCHEDULE_EXACT_ALARM" />
    <uses-feature android:name="android.hardware.location" android:required="false" />
    <uses-feature android:name="android.hardware.location.gps" android:required="false" />
    <uses-feature android:name="android.hardware.camera" android:required="false" />
    <queries>
        <intent>
            <action android:name="android.intent.action.VIEW" />
            <data android:scheme="https" />
        </intent>
    </queries>
</manifest>
EOF

cat > "$BASE/Platforms/Android/MainApplication.cs" << 'EOF'
using Android.App;
using Android.Runtime;

namespace MultiServices.Maui;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership) { }
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
EOF

cat > "$BASE/Platforms/Android/MainActivity.cs" << 'EOF'
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MultiServices.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
    ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
EOF

cat > "$BASE/Platforms/iOS/AppDelegate.cs" << 'EOF'
using Foundation;

namespace MultiServices.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
EOF

cat > "$BASE/Platforms/iOS/Program.cs" << 'EOF'
using UIKit;

namespace MultiServices.Maui;

public class Program
{
    static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
EOF

cat > "$BASE/Platforms/iOS/Info.plist" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>LSRequiresIPhoneOS</key><true/>
    <key>UIDeviceFamily</key><array><integer>1</integer><integer>2</integer></array>
    <key>UIRequiredDeviceCapabilities</key><array><string>arm64</string></array>
    <key>UISupportedInterfaceOrientations</key>
    <array><string>UIInterfaceOrientationPortrait</string></array>
    <key>NSLocationWhenInUseUsageDescription</key>
    <string>MultiServices a besoin de votre position pour trouver les restaurants et services proches de vous.</string>
    <key>NSCameraUsageDescription</key>
    <string>MultiServices a besoin de l'appareil photo pour scanner les codes-barres et prendre des photos.</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>MultiServices a besoin d'accéder à vos photos pour les joindre aux demandes de service.</string>
    <key>NSFaceIDUsageDescription</key>
    <string>MultiServices utilise Face ID pour une connexion sécurisée.</string>
</dict>
</plist>
EOF

echo "✅ App Shell, Resources, Platforms created"
