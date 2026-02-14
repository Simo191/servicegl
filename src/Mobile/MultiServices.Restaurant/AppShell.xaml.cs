using MultiServices.Restaurant.Views.Orders;
using MultiServices.Restaurant.Views.Menu;
using MultiServices.Restaurant.Views.Reviews;
using MultiServices.Restaurant.Views.Settings;
namespace MultiServices.Restaurant;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("orderdetail", typeof(OrderDetailPage));
        Routing.RegisterRoute("menuitemform", typeof(MenuItemFormPage));
        Routing.RegisterRoute("categoryform", typeof(CategoryFormPage));
        Routing.RegisterRoute("reviews", typeof(ReviewsPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("staffmanagement", typeof(StaffPage));
        Routing.RegisterRoute("openinghours", typeof(OpeningHoursPage));
    }
}