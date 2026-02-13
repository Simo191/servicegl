#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# VIEWMODELS
# ============================================================

cat > "$BASE/ViewModels/BaseViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiServices.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _isEmpty;

    protected async Task ExecuteAsync(Func<Task> operation, string? errorMsg = null)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = null;
            await operation();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = errorMsg ?? ex.Message;
            await Shell.Current.DisplayAlert("Erreur", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
EOF

# --- AUTH VIEWMODELS ---
cat > "$BASE/ViewModels/Auth/LoginViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _showPassword;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "Connexion";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var (success, error) = await _authService.LoginAsync(Email, Password);
            if (success)
                await Shell.Current.GoToAsync("//main/home");
            else
                await Shell.Current.DisplayAlert("Erreur", error ?? "Identifiants incorrects", "OK");
        });
    }

    [RelayCommand]
    private async Task SocialLoginAsync(string provider)
    {
        await Shell.Current.DisplayAlert("Info", $"Connexion {provider} - Bientôt disponible", "OK");
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await Shell.Current.GoToAsync("forgotpassword");
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("register");
    }

    [RelayCommand]
    private void TogglePassword() => ShowPassword = !ShowPassword;
}
EOF

cat > "$BASE/ViewModels/Auth/RegisterViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _acceptTerms;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "Inscription";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs obligatoires", "OK");
            return;
        }
        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }
        if (!AcceptTerms)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez accepter les conditions d'utilisation", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var (success, error) = await _authService.RegisterAsync(FirstName, LastName, Email, Password, Phone);
            if (success)
                await Shell.Current.GoToAsync("//main/home");
            else
                await Shell.Current.DisplayAlert("Erreur", error, "OK");
        });
    }

    [RelayCommand]
    private async Task GoToLoginAsync() => await Shell.Current.GoToAsync("..");
}
EOF

# --- RESTAURANT VIEWMODELS ---
cat > "$BASE/ViewModels/Restaurant/RestaurantsViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Restaurant;

public partial class RestaurantsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<RestaurantListDto> _restaurants = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedCuisine;
    [ObservableProperty] private string? _selectedPriceRange;
    [ObservableProperty] private double? _minRating;
    [ObservableProperty] private bool _showFilters;
    [ObservableProperty] private ObservableCollection<string> _cuisineTypes = new(Helpers.AppConstants.CuisineTypes.All);

    public RestaurantsViewModel(ApiService api)
    {
        _api = api;
        Title = "Restaurants";
    }

    [RelayCommand]
    private async Task LoadRestaurantsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["cuisineType"] = SelectedCuisine ?? "",
                ["priceRange"] = SelectedPriceRange ?? "",
                ["minRating"] = MinRating?.ToString() ?? "",
                ["page"] = "1",
                ["pageSize"] = "20"
            };

            var result = await _api.GetAsync<PaginatedResult<RestaurantListDto>>("/restaurants", queryParams);
            if (result.Success && result.Data != null)
            {
                Restaurants = new ObservableCollection<RestaurantListDto>(result.Data.Items);
                IsEmpty = !Restaurants.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadRestaurantsAsync();
    }

    [RelayCommand]
    private void ToggleFilters() => ShowFilters = !ShowFilters;

    [RelayCommand]
    private async Task SelectRestaurantAsync(RestaurantListDto restaurant)
    {
        await Shell.Current.GoToAsync($"restaurantdetail?id={restaurant.Id}");
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SelectedCuisine = null;
        SelectedPriceRange = null;
        MinRating = null;
    }
}
EOF

cat > "$BASE/ViewModels/Restaurant/RestaurantDetailViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Restaurant;

[QueryProperty(nameof(RestaurantId), "id")]
public partial class RestaurantDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _restaurantId = string.Empty;
    [ObservableProperty] private RestaurantDto? _restaurant;
    [ObservableProperty] private ObservableCollection<MenuCategoryDto> _menuCategories = new();
    [ObservableProperty] private MenuCategoryDto? _selectedCategory;
    [ObservableProperty] private RestaurantCart _cart = new();
    [ObservableProperty] private bool _showCart;
    [ObservableProperty] private bool _isFavorite;

    public RestaurantDetailViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnRestaurantIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadRestaurantCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadRestaurantAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<RestaurantDto>($"/restaurants/{RestaurantId}");
            if (result.Success && result.Data != null)
            {
                Restaurant = result.Data;
                Title = result.Data.Name;
                MenuCategories = new ObservableCollection<MenuCategoryDto>(result.Data.MenuCategories);
                if (MenuCategories.Any())
                    SelectedCategory = MenuCategories.First();
                
                Cart.RestaurantId = result.Data.Id;
                Cart.RestaurantName = result.Data.Name;
                Cart.RestaurantLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                Cart.MinimumOrder = result.Data.MinimumOrder;
            }
        });
    }

    [RelayCommand]
    private void AddToCart(MenuItemDto item)
    {
        var existingItem = Cart.Items.FirstOrDefault(i => i.MenuItemId == item.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            Cart.Items.Add(new RestaurantCartItem
            {
                MenuItemId = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Quantity = 1
            });
        }
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void RemoveFromCart(RestaurantCartItem item)
    {
        if (item.Quantity > 1)
            item.Quantity--;
        else
            Cart.Items.Remove(item);
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void ToggleCart() => ShowCart = !ShowCart;

    [RelayCommand]
    private async Task ProceedToCheckoutAsync()
    {
        if (!Cart.MeetsMinimum)
        {
            await Shell.Current.DisplayAlert("Minimum non atteint",
                $"Le minimum de commande est {Cart.MinimumOrder:N2} DH", "OK");
            return;
        }
        await Shell.Current.GoToAsync("checkout");
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        IsFavorite = !IsFavorite;
        if (IsFavorite)
            await _api.PostAsync<object>($"/favorites/restaurants/{RestaurantId}");
        else
            await _api.DeleteAsync<object>($"/favorites/restaurants/{RestaurantId}");
    }
}
EOF

cat > "$BASE/ViewModels/Restaurant/RestaurantOrderTrackingViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Restaurant;

[QueryProperty(nameof(OrderId), "id")]
public partial class RestaurantOrderTrackingViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _orderId = string.Empty;
    [ObservableProperty] private RestaurantOrderDto? _order;
    [ObservableProperty] private ObservableCollection<OrderStatusHistoryDto> _statusHistory = new();
    [ObservableProperty] private double _delivererLat;
    [ObservableProperty] private double _delivererLng;
    [ObservableProperty] private bool _showMap;

    public RestaurantOrderTrackingViewModel(ApiService api)
    {
        _api = api;
        Title = "Suivi commande";
    }

    partial void OnOrderIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadOrderCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadOrderAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<RestaurantOrderDto>($"/restaurant-orders/{OrderId}");
            if (result.Success && result.Data != null)
            {
                Order = result.Data;
                StatusHistory = new ObservableCollection<OrderStatusHistoryDto>(result.Data.StatusHistory);
                if (result.Data.DelivererLatitude.HasValue)
                {
                    DelivererLat = result.Data.DelivererLatitude.Value;
                    DelivererLng = result.Data.DelivererLongitude!.Value;
                    ShowMap = true;
                }
            }
        });
    }

    [RelayCommand]
    private async Task CallDelivererAsync()
    {
        if (Order?.DelivererPhone != null)
        {
            try { PhoneDialer.Default.Open(Order.DelivererPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }

    [RelayCommand]
    private async Task RefreshTrackingAsync() => await LoadOrderAsync();
}
EOF

# --- SERVICE VIEWMODELS ---
cat > "$BASE/ViewModels/Services/ServicesViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Helpers;

namespace MultiServices.Maui.ViewModels.Services;

public partial class ServicesViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<ServiceProviderListDto> _providers = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedCategory;
    [ObservableProperty] private bool _showFilters;
    [ObservableProperty] private ObservableCollection<CategoryItem> _categories = new();

    public ServicesViewModel(ApiService api)
    {
        _api = api;
        Title = "Services";
        foreach (var cat in AppConstants.ServiceCategories.All)
            Categories.Add(new CategoryItem { Key = cat.Key, Label = cat.Label, Icon = cat.Icon });
    }

    [RelayCommand]
    private async Task LoadProvidersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["category"] = SelectedCategory ?? "",
                ["page"] = "1",
                ["pageSize"] = "20"
            };

            var result = await _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers", queryParams);
            if (result.Success && result.Data != null)
            {
                Providers = new ObservableCollection<ServiceProviderListDto>(result.Data.Items);
                IsEmpty = !Providers.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(string category)
    {
        SelectedCategory = category == SelectedCategory ? null : category;
        await LoadProvidersAsync();
    }

    [RelayCommand]
    private async Task SelectProviderAsync(ServiceProviderListDto provider)
    {
        await Shell.Current.GoToAsync($"servicedetail?id={provider.Id}");
    }
}

public class CategoryItem
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
EOF

cat > "$BASE/ViewModels/Services/ServiceDetailViewModel.cs" << 'EOF'
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
EOF

cat > "$BASE/ViewModels/Services/BookServiceViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Services;

[QueryProperty(nameof(ProviderId), "providerId")]
[QueryProperty(nameof(ServiceId), "serviceId")]
public partial class BookServiceViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _providerId = string.Empty;
    [ObservableProperty] private string _serviceId = string.Empty;
    [ObservableProperty] private string _problemDescription = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _problemPhotos = new();
    [ObservableProperty] private AddressDto? _selectedAddress;
    [ObservableProperty] private ObservableCollection<AddressDto> _addresses = new();
    [ObservableProperty] private ObservableCollection<AvailableSlotDto> _availableSlots = new();
    [ObservableProperty] private AvailableSlotDto? _selectedSlot;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today.AddDays(1);
    [ObservableProperty] private string _paymentTiming = "after";
    [ObservableProperty] private int _currentStep = 1;

    public BookServiceViewModel(ApiService api)
    {
        _api = api;
        Title = "Réserver un service";
    }

    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        var result = await _api.GetAsync<List<AddressDto>>("/profile/addresses");
        if (result.Success && result.Data != null)
        {
            Addresses = new ObservableCollection<AddressDto>(result.Data);
            SelectedAddress = Addresses.FirstOrDefault(a => a.IsDefault) ?? Addresses.FirstOrDefault();
        }
    }

    [RelayCommand]
    private async Task LoadSlotsAsync()
    {
        var queryParams = new Dictionary<string, string>
        {
            ["date"] = SelectedDate.ToString("yyyy-MM-dd")
        };
        var result = await _api.GetAsync<List<AvailableSlotDto>>($"/services/providers/{ProviderId}/slots", queryParams);
        if (result.Success && result.Data != null)
            AvailableSlots = new ObservableCollection<AvailableSlotDto>(result.Data);
    }

    [RelayCommand]
    private async Task AddPhotoAsync()
    {
        var result = await MediaPicker.Default.CapturePhotoAsync();
        if (result != null)
            ProblemPhotos.Add(result.FullPath);
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        var result = await MediaPicker.Default.PickPhotoAsync();
        if (result != null)
            ProblemPhotos.Add(result.FullPath);
    }

    [RelayCommand]
    private void NextStep()
    {
        if (CurrentStep < 4) CurrentStep++;
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentStep > 1) CurrentStep--;
    }

    [RelayCommand]
    private async Task ConfirmBookingAsync()
    {
        if (SelectedAddress == null || SelectedSlot == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez compléter tous les champs", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new
            {
                serviceOfferingId = Guid.Parse(ServiceId),
                problemDescription = ProblemDescription,
                addressId = SelectedAddress.Id,
                scheduledDate = SelectedDate,
                scheduledStartTime = SelectedSlot.StartTime,
                paymentTiming = PaymentTiming
            };

            var result = await _api.PostAsync<InterventionDto>("/services/interventions", request);
            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Succès", "Votre réservation a été confirmée!", "OK");
                await Shell.Current.GoToAsync("../..");
            }
        });
    }
}
EOF

cat > "$BASE/ViewModels/Services/InterventionTrackingViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Services;

[QueryProperty(nameof(InterventionId), "id")]
public partial class InterventionTrackingViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _interventionId = string.Empty;
    [ObservableProperty] private InterventionDto? _intervention;
    [ObservableProperty] private ObservableCollection<InterventionStatusHistoryDto> _statusHistory = new();

    public InterventionTrackingViewModel(ApiService api)
    {
        _api = api;
        Title = "Suivi intervention";
    }

    partial void OnInterventionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadInterventionCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadInterventionAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<InterventionDto>($"/services/interventions/{InterventionId}");
            if (result.Success && result.Data != null)
            {
                Intervention = result.Data;
                StatusHistory = new ObservableCollection<InterventionStatusHistoryDto>(result.Data.StatusHistory);
            }
        });
    }

    [RelayCommand]
    private async Task CallIntervenantAsync()
    {
        if (Intervention?.IntervenantPhone != null)
        {
            try { PhoneDialer.Default.Open(Intervention.IntervenantPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }
}
EOF

# --- GROCERY VIEWMODELS ---
cat > "$BASE/ViewModels/Grocery/GroceryStoresViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

public partial class GroceryStoresViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<GroceryStoreListDto> _stores = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedBrand;

    public string[] Brands => Helpers.AppConstants.GroceryBrands.All;

    public GroceryStoresViewModel(ApiService api)
    {
        _api = api;
        Title = "Courses";
    }

    [RelayCommand]
    private async Task LoadStoresAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["brand"] = SelectedBrand ?? ""
            };
            var result = await _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores", queryParams);
            if (result.Success && result.Data != null)
            {
                Stores = new ObservableCollection<GroceryStoreListDto>(result.Data.Items);
                IsEmpty = !Stores.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SelectBrandAsync(string brand)
    {
        SelectedBrand = brand == SelectedBrand ? null : brand;
        await LoadStoresAsync();
    }

    [RelayCommand]
    private async Task SelectStoreAsync(GroceryStoreListDto store)
    {
        await Shell.Current.GoToAsync($"storedetail?id={store.Id}");
    }
}
EOF

cat > "$BASE/ViewModels/Grocery/StoreDetailViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

[QueryProperty(nameof(StoreId), "id")]
public partial class StoreDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _storeId = string.Empty;
    [ObservableProperty] private GroceryStoreDto? _store;
    [ObservableProperty] private ObservableCollection<GroceryDepartmentDto> _departments = new();
    [ObservableProperty] private GroceryDepartmentDto? _selectedDepartment;
    [ObservableProperty] private ObservableCollection<GroceryProductDto> _products = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private GroceryCart _cart = new();
    [ObservableProperty] private bool _showCart;
    [ObservableProperty] private bool _filterBio;
    [ObservableProperty] private bool _filterHalal;
    [ObservableProperty] private bool _filterPromo;

    public StoreDetailViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnStoreIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadStoreCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadStoreAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<GroceryStoreDto>($"/grocery/stores/{StoreId}");
            if (result.Success && result.Data != null)
            {
                Store = result.Data;
                Title = result.Data.Name;
                Departments = new ObservableCollection<GroceryDepartmentDto>(result.Data.Departments);
                Cart.StoreId = result.Data.Id;
                Cart.StoreName = result.Data.Name;
                Cart.StoreLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                Cart.MinimumOrder = result.Data.MinimumOrder;
            }
        });
    }

    [RelayCommand]
    private async Task SelectDepartmentAsync(GroceryDepartmentDto dept)
    {
        SelectedDepartment = dept;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["departmentId"] = SelectedDepartment?.Id.ToString() ?? "",
                ["isBio"] = FilterBio ? "true" : "",
                ["isHalal"] = FilterHalal ? "true" : "",
                ["isOnPromotion"] = FilterPromo ? "true" : ""
            };
            var result = await _api.GetAsync<PaginatedResult<GroceryProductDto>>($"/grocery/stores/{StoreId}/products", queryParams);
            if (result.Success && result.Data != null)
                Products = new ObservableCollection<GroceryProductDto>(result.Data.Items);
        });
    }

    [RelayCommand]
    private void AddToCart(GroceryProductDto product)
    {
        var existing = Cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
            existing.Quantity++;
        else
            Cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.IsOnPromotion ? product.PromotionPrice ?? product.Price : product.Price,
                Quantity = 1
            });
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void RemoveFromCart(CartItem item)
    {
        if (item.Quantity > 1) item.Quantity--;
        else Cart.Items.Remove(item);
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void ToggleCart() => ShowCart = !ShowCart;

    [RelayCommand]
    private async Task ProceedToCheckoutAsync()
    {
        await Shell.Current.GoToAsync("grocerycheckout");
    }
}
EOF

cat > "$BASE/ViewModels/Grocery/ShoppingListsViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

public partial class ShoppingListsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<ShoppingListDto> _lists = new();
    [ObservableProperty] private string _newListName = string.Empty;
    [ObservableProperty] private bool _showCreateForm;

    public ShoppingListsViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes listes";
    }

    [RelayCommand]
    private async Task LoadListsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<List<ShoppingListDto>>("/grocery/shopping-lists");
            if (result.Success && result.Data != null)
            {
                Lists = new ObservableCollection<ShoppingListDto>(result.Data);
                IsEmpty = !Lists.Any();
            }
        });
    }

    [RelayCommand]
    private async Task CreateListAsync()
    {
        if (string.IsNullOrWhiteSpace(NewListName)) return;
        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<ShoppingListDto>("/grocery/shopping-lists", new { name = NewListName });
            if (result.Success && result.Data != null)
            {
                Lists.Add(result.Data);
                NewListName = string.Empty;
                ShowCreateForm = false;
            }
        });
    }

    [RelayCommand]
    private async Task SelectListAsync(ShoppingListDto list)
    {
        await Shell.Current.GoToAsync($"shoppinglistdetail?id={list.Id}");
    }

    [RelayCommand]
    private async Task DeleteListAsync(ShoppingListDto list)
    {
        var confirm = await Shell.Current.DisplayAlert("Supprimer", $"Supprimer \"{list.Name}\" ?", "Oui", "Non");
        if (confirm)
        {
            await _api.DeleteAsync<object>($"/grocery/shopping-lists/{list.Id}");
            Lists.Remove(list);
        }
    }

    [RelayCommand]
    private async Task ConvertToCartAsync(ShoppingListDto list)
    {
        await Shell.Current.GoToAsync($"storedetail?id=select&listId={list.Id}");
    }
}
EOF

# --- COMMON VIEWMODELS ---
cat > "$BASE/ViewModels/Common/HomeViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Common;

public partial class HomeViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;

    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private ObservableCollection<RestaurantListDto> _popularRestaurants = new();
    [ObservableProperty] private ObservableCollection<ServiceProviderListDto> _topProviders = new();
    [ObservableProperty] private ObservableCollection<GroceryStoreListDto> _nearbyStores = new();
    [ObservableProperty] private int _activeOrdersCount;

    public HomeViewModel(ApiService api, AuthService authService)
    {
        _api = api;
        _authService = authService;
        Title = "MultiServices";
        SetGreeting();
    }

    private void SetGreeting()
    {
        var hour = DateTime.Now.Hour;
        var name = _authService.CurrentUser?.FirstName ?? "";
        Greeting = hour switch
        {
            < 12 => $"Bonjour {name} 👋",
            < 18 => $"Bon après-midi {name} 👋",
            _ => $"Bonsoir {name} 👋"
        };
    }

    [RelayCommand]
    private async Task LoadHomeDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var restaurantsTask = _api.GetAsync<PaginatedResult<RestaurantListDto>>("/restaurants",
                new Dictionary<string, string> { ["pageSize"] = "5", ["sortBy"] = "popularity" });
            var providersTask = _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers",
                new Dictionary<string, string> { ["pageSize"] = "5", ["sortBy"] = "rating" });
            var storesTask = _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores",
                new Dictionary<string, string> { ["pageSize"] = "5" });

            await Task.WhenAll(restaurantsTask, providersTask, storesTask);

            var restaurants = await restaurantsTask;
            if (restaurants.Success && restaurants.Data != null)
                PopularRestaurants = new ObservableCollection<RestaurantListDto>(restaurants.Data.Items);

            var providers = await providersTask;
            if (providers.Success && providers.Data != null)
                TopProviders = new ObservableCollection<ServiceProviderListDto>(providers.Data.Items);

            var stores = await storesTask;
            if (stores.Success && stores.Data != null)
                NearbyStores = new ObservableCollection<GroceryStoreListDto>(stores.Data.Items);
        });
    }

    [RelayCommand]
    private async Task GoToRestaurantsAsync() => await Shell.Current.GoToAsync("//main/restaurants");

    [RelayCommand]
    private async Task GoToServicesAsync() => await Shell.Current.GoToAsync("//main/services");

    [RelayCommand]
    private async Task GoToGroceryAsync() => await Shell.Current.GoToAsync("//main/grocery");

    [RelayCommand]
    private async Task GoToOrdersAsync() => await Shell.Current.GoToAsync("//main/orders");

    [RelayCommand]
    private async Task GoToNotificationsAsync() => await Shell.Current.GoToAsync("notifications");
}
EOF

cat > "$BASE/ViewModels/Common/OrdersViewModel.cs" << 'EOF'
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Common;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<OrderSummary> _orders = new();
    [ObservableProperty] private string _activeTab = "active";

    public OrdersViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes commandes";
    }

    [RelayCommand]
    private async Task LoadOrdersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var status = ActiveTab == "active" ? "active" : "completed";
            var result = await _api.GetAsync<List<OrderSummary>>("/orders", new Dictionary<string, string> { ["status"] = status });
            if (result.Success && result.Data != null)
            {
                Orders = new ObservableCollection<OrderSummary>(result.Data);
                IsEmpty = !Orders.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SetTabAsync(string tab)
    {
        ActiveTab = tab;
        await LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task ViewOrderAsync(OrderSummary order)
    {
        var route = order.Type switch
        {
            "restaurant" => $"restaurantordertracking?id={order.Id}",
            "service" => $"interventiontracking?id={order.Id}",
            "grocery" => $"groceryordertracking?id={order.Id}",
            _ => ""
        };
        if (!string.IsNullOrEmpty(route))
            await Shell.Current.GoToAsync(route);
    }
}

public class OrderSummary
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TypeIcon => Type switch
    {
        "restaurant" => "🍔",
        "service" => "🛠️",
        "grocery" => "🛒",
        _ => "📦"
    };
    public Color TypeColor => Type switch
    {
        "restaurant" => Color.FromArgb("#F59E0B"),
        "service" => Color.FromArgb("#3B82F6"),
        "grocery" => Color.FromArgb("#10B981"),
        _ => Color.FromArgb("#6B7280")
    };
}
EOF

cat > "$BASE/ViewModels/Profile/ProfileViewModel.cs" << 'EOF'
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;

    [ObservableProperty] private UserDto? _user;
    [ObservableProperty] private ObservableCollection<AddressDto> _addresses = new();
    [ObservableProperty] private WalletDto? _wallet;

    public ProfileViewModel(ApiService api, AuthService authService)
    {
        _api = api;
        _authService = authService;
        Title = "Mon profil";
        User = _authService.CurrentUser;
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profileTask = _api.GetAsync<UserDto>("/profile");
            var addressesTask = _api.GetAsync<List<AddressDto>>("/profile/addresses");
            var walletTask = _api.GetAsync<WalletDto>("/profile/wallet");

            await Task.WhenAll(profileTask, addressesTask, walletTask);

            var profile = await profileTask;
            if (profile.Success && profile.Data != null)
                User = profile.Data;

            var addresses = await addressesTask;
            if (addresses.Success && addresses.Data != null)
                Addresses = new ObservableCollection<AddressDto>(addresses.Data);

            var wallet = await walletTask;
            if (wallet.Success && wallet.Data != null)
                Wallet = wallet.Data;
        });
    }

    [RelayCommand]
    private async Task EditProfileAsync() => await Shell.Current.GoToAsync("editprofile");

    [RelayCommand]
    private async Task ManageAddressesAsync() => await Shell.Current.GoToAsync("addresses");

    [RelayCommand]
    private async Task ViewWalletAsync() => await Shell.Current.GoToAsync("wallet");

    [RelayCommand]
    private async Task ViewFavoritesAsync() => await Shell.Current.GoToAsync("favorites");

    [RelayCommand]
    private async Task ViewLoyaltyAsync() => await Shell.Current.GoToAsync("loyalty");

    [RelayCommand]
    private async Task ViewNotificationsSettingsAsync() => await Shell.Current.GoToAsync("notificationsettings");

    [RelayCommand]
    private async Task ContactSupportAsync() => await Shell.Current.GoToAsync("support");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vous déconnecter ?", "Oui", "Non");
        if (confirm)
            await _authService.LogoutAsync();
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Supprimer le compte",
            "Cette action est irréversible. Voulez-vous continuer ?", "Supprimer", "Annuler");
        if (confirm)
        {
            await _api.DeleteAsync<object>("/profile");
            await _authService.LogoutAsync();
        }
    }
}
EOF

echo "✅ All ViewModels created"
