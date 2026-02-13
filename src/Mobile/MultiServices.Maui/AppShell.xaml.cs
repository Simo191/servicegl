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
