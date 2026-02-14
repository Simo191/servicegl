using MultiServices.Store.Views.Orders; using MultiServices.Store.Views.Catalog; using MultiServices.Store.Views.Promotions; using MultiServices.Store.Views.Settings;
namespace MultiServices.Store;
public partial class AppShell : Shell { public AppShell() { InitializeComponent();
    Routing.RegisterRoute("orderdetail", typeof(OrderDetailPage)); Routing.RegisterRoute("picking", typeof(PickingPage));
    Routing.RegisterRoute("productform", typeof(ProductFormPage)); Routing.RegisterRoute("bulkimport", typeof(BulkImportPage));
    Routing.RegisterRoute("promotions", typeof(PromotionsPage)); Routing.RegisterRoute("settings", typeof(SettingsPage));
} }