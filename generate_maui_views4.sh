#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# GROCERY VIEWS
# ============================================================
cat > "$BASE/Views/Grocery/GroceryStoresPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Grocery"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Grocery.GroceryStoresPage"
             x:DataType="vm:GroceryStoresViewModel"
             Title="Courses">
    <Grid RowDefinitions="Auto,Auto,*">
        <SearchBar Placeholder="Rechercher un magasin..." Text="{Binding SearchQuery}"
                   SearchCommand="{Binding LoadStoresCommand}" Style="{StaticResource SearchBarStyle}" Margin="16,8" />
        <!-- Brand Filters -->
        <ScrollView Grid.Row="1" Orientation="Horizontal" Padding="16,0" Margin="0,0,0,8">
            <HorizontalStackLayout Spacing="8">
                <Frame BackgroundColor="#D1FAE5" CornerRadius="14" Padding="12,8" HasShadow="False" BorderColor="Transparent">
                    <Label Text="Marjane" FontSize="13" FontAttributes="Bold" TextColor="#065F46">
                        <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SelectBrandCommand}" CommandParameter="Marjane" /></Label.GestureRecognizers>
                    </Label>
                </Frame>
                <Frame BackgroundColor="#DBEAFE" CornerRadius="14" Padding="12,8" HasShadow="False" BorderColor="Transparent">
                    <Label Text="Carrefour" FontSize="13" FontAttributes="Bold" TextColor="#1E40AF">
                        <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SelectBrandCommand}" CommandParameter="Carrefour" /></Label.GestureRecognizers>
                    </Label>
                </Frame>
                <Frame BackgroundColor="#FEF3C7" CornerRadius="14" Padding="12,8" HasShadow="False" BorderColor="Transparent">
                    <Label Text="Aswak Assalam" FontSize="13" FontAttributes="Bold" TextColor="#92400E">
                        <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SelectBrandCommand}" CommandParameter="Aswak Assalam" /></Label.GestureRecognizers>
                    </Label>
                </Frame>
                <Frame BackgroundColor="#FCE7F3" CornerRadius="14" Padding="12,8" HasShadow="False" BorderColor="Transparent">
                    <Label Text="Acima" FontSize="13" FontAttributes="Bold" TextColor="#9D174D">
                        <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SelectBrandCommand}" CommandParameter="Acima" /></Label.GestureRecognizers>
                    </Label>
                </Frame>
                <Frame BackgroundColor="#EDE9FE" CornerRadius="14" Padding="12,8" HasShadow="False" BorderColor="Transparent">
                    <Label Text="Label'Vie" FontSize="13" FontAttributes="Bold" TextColor="#5B21B6">
                        <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SelectBrandCommand}" CommandParameter="Label'Vie" /></Label.GestureRecognizers>
                    </Label>
                </Frame>
            </HorizontalStackLayout>
        </ScrollView>
        <!-- Stores List -->
        <RefreshView Grid.Row="2" Command="{Binding LoadStoresCommand}" IsRefreshing="{Binding IsRefreshing}">
            <CollectionView ItemsSource="{Binding Stores}">
                <CollectionView.EmptyView>
                    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
                        <Label Text="🛒" FontSize="48" HorizontalOptions="Center" />
                        <Label Text="Aucun magasin trouvé" Style="{StaticResource Title3}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:GroceryStoreListDto">
                        <Frame Margin="16,6" CornerRadius="16" Padding="16" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="50,*" ColumnSpacing="12">
                                <Frame BackgroundColor="#D1FAE5" CornerRadius="25" HeightRequest="50" WidthRequest="50" HasShadow="False" Padding="0">
                                    <Label Text="🛒" FontSize="22" HorizontalOptions="Center" VerticalOptions="Center" />
                                </Frame>
                                <VerticalStackLayout Grid.Column="1" Spacing="4">
                                    <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="15" />
                                    <Label Text="{Binding Brand}" FontSize="12" TextColor="{StaticResource Success}" />
                                    <HorizontalStackLayout Spacing="12">
                                        <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                        <Label Text="{Binding DeliveryFee, StringFormat='Livraison: {0:N2} DH'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                        <Frame IsVisible="{Binding HasFreeDelivery}" BackgroundColor="#D1FAE5" CornerRadius="6" Padding="6,1" HasShadow="False">
                                            <Label Text="LIVR. GRATUITE" FontSize="9" FontAttributes="Bold" TextColor="#065F46" />
                                        </Frame>
                                    </HorizontalStackLayout>
                                </VerticalStackLayout>
                            </Grid>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:GroceryStoresViewModel}}, Path=SelectStoreCommand}"
                                                      CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </RefreshView>
    </Grid>
</ContentPage>
EOF
cat > "$BASE/Views/Grocery/GroceryStoresPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class GroceryStoresPage : ContentPage
{
    private readonly GroceryStoresViewModel _vm;
    public GroceryStoresPage(GroceryStoresViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadStoresCommand.ExecuteAsync(null); }
}
EOF

cat > "$BASE/Views/Grocery/StoreDetailPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Grocery"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Grocery.StoreDetailPage"
             x:DataType="vm:StoreDetailViewModel"
             Title="{Binding Title}">
    <Grid RowDefinitions="Auto,Auto,*,Auto">
        <!-- Search & Filters -->
        <Grid Padding="16,8" ColumnDefinitions="*,Auto" ColumnSpacing="8">
            <SearchBar Placeholder="Rechercher un produit..." Text="{Binding SearchQuery}"
                       SearchCommand="{Binding LoadProductsCommand}" Style="{StaticResource SearchBarStyle}" />
            <HorizontalStackLayout Grid.Column="1" Spacing="4" VerticalOptions="Center">
                <Frame BackgroundColor="{Binding FilterBio, Converter={StaticResource BoolToFilterBg}}" CornerRadius="10" Padding="8,4" HasShadow="False">
                    <Label Text="Bio" FontSize="11" />
                </Frame>
                <Frame BackgroundColor="{Binding FilterHalal, Converter={StaticResource BoolToFilterBg}}" CornerRadius="10" Padding="8,4" HasShadow="False">
                    <Label Text="Halal" FontSize="11" />
                </Frame>
            </HorizontalStackLayout>
        </Grid>

        <!-- Departments -->
        <ScrollView Grid.Row="1" Orientation="Horizontal" Padding="16,0">
            <HorizontalStackLayout Spacing="8" BindableLayout.ItemsSource="{Binding Departments}">
                <BindableLayout.ItemTemplate>
                    <DataTemplate x:DataType="models:GroceryDepartmentDto">
                        <Frame CornerRadius="12" Padding="12,8" HasShadow="False" BackgroundColor="#F3F4F6" BorderColor="Transparent">
                            <VerticalStackLayout Spacing="2" HorizontalOptions="Center">
                                <Label Text="{Binding Name}" FontSize="12" FontAttributes="Bold" HorizontalOptions="Center" />
                                <Label Text="{Binding ProductCount, StringFormat='{0} produits'}" FontSize="10" TextColor="{StaticResource TextMuted}" HorizontalOptions="Center" />
                            </VerticalStackLayout>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:StoreDetailViewModel}}, Path=SelectDepartmentCommand}"
                                                      CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>
                        </Frame>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </HorizontalStackLayout>
        </ScrollView>

        <!-- Products Grid -->
        <CollectionView Grid.Row="2" ItemsSource="{Binding Products}" Margin="16,8">
            <CollectionView.ItemsLayout>
                <GridItemsLayout Orientation="Vertical" Span="2" HorizontalItemSpacing="10" VerticalItemSpacing="10" />
            </CollectionView.ItemsLayout>
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:GroceryProductDto">
                    <Frame CornerRadius="12" Padding="0" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                        <VerticalStackLayout>
                            <Grid HeightRequest="100">
                                <BoxView BackgroundColor="#F9FAFB" />
                                <Label Text="🛍️" FontSize="32" HorizontalOptions="Center" VerticalOptions="Center" />
                                <Frame IsVisible="{Binding IsOnPromotion}" BackgroundColor="{StaticResource Danger}" CornerRadius="0" Padding="6,2"
                                       HasShadow="False" HorizontalOptions="Start" VerticalOptions="Start">
                                    <Label Text="{Binding PromotionLabel}" FontSize="9" TextColor="White" FontAttributes="Bold" />
                                </Frame>
                                <HorizontalStackLayout HorizontalOptions="End" VerticalOptions="Start" Margin="4">
                                    <Frame IsVisible="{Binding IsBio}" BackgroundColor="#D1FAE5" CornerRadius="4" Padding="4,1" HasShadow="False">
                                        <Label Text="BIO" FontSize="8" TextColor="#065F46" FontAttributes="Bold" />
                                    </Frame>
                                </HorizontalStackLayout>
                            </Grid>
                            <VerticalStackLayout Padding="8" Spacing="2">
                                <Label Text="{Binding Name}" FontSize="13" FontAttributes="Bold" MaxLines="2" LineBreakMode="TailTruncation" />
                                <Label Text="{Binding Brand}" FontSize="11" TextColor="{StaticResource TextMuted}" />
                                <Grid ColumnDefinitions="*,Auto">
                                    <VerticalStackLayout>
                                        <Label Text="{Binding Price, StringFormat='{0:N2} DH'}" FontSize="14" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                                        <Label Text="{Binding PricePerUnit, StringFormat='{0:N2} DH/kg'}" FontSize="10" TextColor="{StaticResource TextMuted}" />
                                    </VerticalStackLayout>
                                    <Button Grid.Column="1" Text="+" BackgroundColor="{StaticResource Success}" TextColor="White"
                                            CornerRadius="16" HeightRequest="32" WidthRequest="32" FontSize="16" Padding="0"
                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:StoreDetailViewModel}}, Path=AddToCartCommand}"
                                            CommandParameter="{Binding}" />
                                </Grid>
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- Cart Bar -->
        <Frame Grid.Row="3" BackgroundColor="{StaticResource Success}" CornerRadius="0" Padding="16,12" HasShadow="True"
               IsVisible="{Binding Cart.ItemCount, Converter={StaticResource NullToBool}}">
            <Grid ColumnDefinitions="Auto,*,Auto">
                <Frame BackgroundColor="White" CornerRadius="12" Padding="8,4" HasShadow="False">
                    <Label Text="{Binding Cart.ItemCount}" FontAttributes="Bold" TextColor="{StaticResource Success}" />
                </Frame>
                <Label Grid.Column="1" Text="Voir le panier" TextColor="White" FontAttributes="Bold" FontSize="16"
                       VerticalOptions="Center" HorizontalOptions="Center">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding ProceedToCheckoutCommand}" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Grid.Column="2" Text="{Binding Cart.Total, StringFormat='{0:N2} DH'}" TextColor="White" FontAttributes="Bold" FontSize="16" />
            </Grid>
        </Frame>
    </Grid>
</ContentPage>
EOF
cat > "$BASE/Views/Grocery/StoreDetailPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class StoreDetailPage : ContentPage
{
    public StoreDetailPage(StoreDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

cat > "$BASE/Views/Grocery/ShoppingListsPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Grocery"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Grocery.ShoppingListsPage"
             x:DataType="vm:ShoppingListsViewModel"
             Title="Listes de courses">
    <Grid RowDefinitions="*,Auto">
        <RefreshView Command="{Binding LoadListsCommand}" IsRefreshing="{Binding IsRefreshing}">
            <CollectionView ItemsSource="{Binding Lists}">
                <CollectionView.EmptyView>
                    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
                        <Label Text="📋" FontSize="48" HorizontalOptions="Center" />
                        <Label Text="Aucune liste" Style="{StaticResource Title3}" HorizontalOptions="Center" />
                        <Label Text="Créez votre première liste de courses" Style="{StaticResource BodyText}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:ShoppingListDto">
                        <Frame Margin="16,6" CornerRadius="14" Padding="16" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="*,Auto">
                                <VerticalStackLayout Spacing="4">
                                    <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="16" />
                                    <Label Text="{Binding ItemCount, StringFormat='{0} articles'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                                    <HorizontalStackLayout Spacing="8">
                                        <Label Text="{Binding CheckedCount, StringFormat='{0} cochés'}" FontSize="12" TextColor="{StaticResource Success}" />
                                        <Frame IsVisible="{Binding IsRecurring}" BackgroundColor="#EEF2FF" CornerRadius="6" Padding="6,1" HasShadow="False">
                                            <Label Text="🔄 Récurrente" FontSize="10" TextColor="{StaticResource Primary}" />
                                        </Frame>
                                    </HorizontalStackLayout>
                                </VerticalStackLayout>
                                <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Spacing="8">
                                    <Button Text="🛒" BackgroundColor="{StaticResource Success}" TextColor="White" CornerRadius="10" HeightRequest="36" WidthRequest="36"
                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ShoppingListsViewModel}}, Path=ConvertToCartCommand}" CommandParameter="{Binding}" />
                                </VerticalStackLayout>
                            </Grid>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ShoppingListsViewModel}}, Path=SelectListCommand}" CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </RefreshView>
        <Button Grid.Row="1" Text="+ Nouvelle liste" Style="{StaticResource PrimaryButton}" Margin="16,8"
                Command="{Binding CreateListCommand}" />
    </Grid>
</ContentPage>
EOF
cat > "$BASE/Views/Grocery/ShoppingListsPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Grocery;
namespace MultiServices.Maui.Views.Grocery;
public partial class ShoppingListsPage : ContentPage
{
    private readonly ShoppingListsViewModel _vm;
    public ShoppingListsPage(ShoppingListsViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadListsCommand.ExecuteAsync(null); }
}
EOF

cat > "$BASE/Views/Grocery/GroceryCheckoutPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.Views.Grocery.GroceryCheckoutPage"
             Title="Valider les courses">
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="12">
                    <Label Text="📍 Adresse de livraison" Style="{StaticResource Title3}" />
                    <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="12" HasShadow="False">
                        <Label Text="🏠 Maison - 123 Rue Mohammed V, Casablanca" FontSize="14" />
                    </Frame>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="12">
                    <Label Text="🕐 Créneau de livraison" Style="{StaticResource Title3}" />
                    <HorizontalStackLayout Spacing="8">
                        <Frame BackgroundColor="{StaticResource PrimaryLight}" CornerRadius="10" Padding="12,8" HasShadow="False"><Label Text="Maintenant" FontSize="13" /></Frame>
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="10" Padding="12,8" HasShadow="False"><Label Text="14h-16h" FontSize="13" /></Frame>
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="10" Padding="12,8" HasShadow="False"><Label Text="16h-18h" FontSize="13" /></Frame>
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="10">
                    <Label Text="⚙️ Options" Style="{StaticResource Title3}" />
                    <HorizontalStackLayout Spacing="8">
                        <Switch OnColor="{StaticResource Primary}" />
                        <Label Text="Autoriser les remplacements" VerticalOptions="Center" FontSize="14" />
                    </HorizontalStackLayout>
                    <HorizontalStackLayout Spacing="8">
                        <Switch OnColor="{StaticResource Primary}" />
                        <Label Text="Laisser devant la porte" VerticalOptions="Center" FontSize="14" />
                    </HorizontalStackLayout>
                    <Label Text="Type de sacs" FontSize="14" FontAttributes="Bold" Margin="0,4,0,0" />
                    <HorizontalStackLayout Spacing="8">
                        <Frame BackgroundColor="{StaticResource PrimaryLight}" CornerRadius="10" Padding="12,8" HasShadow="False"><Label Text="Plastique" FontSize="13" /></Frame>
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="10" Padding="12,8" HasShadow="False"><Label Text="Réutilisables (+3 DH)" FontSize="13" /></Frame>
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="12">
                    <Label Text="💳 Paiement" Style="{StaticResource Title3}" />
                    <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="12" HasShadow="False"><Label Text="Carte •••• 4242" FontSize="14" /></Frame>
                    <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="12" HasShadow="False"><Label Text="💵 Espèces" FontSize="14" /></Frame>
                </VerticalStackLayout>
            </Frame>
            <Button Text="Confirmer les courses" Style="{StaticResource PrimaryButton}" Margin="0,8" BackgroundColor="{StaticResource Success}" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Grocery/GroceryCheckoutPage.xaml.cs" << 'EOF'
namespace MultiServices.Maui.Views.Grocery;
public partial class GroceryCheckoutPage : ContentPage { public GroceryCheckoutPage() { InitializeComponent(); } }
EOF

echo "✅ Grocery views created"

# ============================================================
# COMMON VIEWS (Orders, Notifications)
# ============================================================
cat > "$BASE/Views/Common/OrdersPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Common"
             x:Class="MultiServices.Maui.Views.Common.OrdersPage"
             x:DataType="vm:OrdersViewModel"
             Title="Mes commandes">
    <Grid RowDefinitions="Auto,*">
        <HorizontalStackLayout Spacing="8" Padding="16,12" HorizontalOptions="Center">
            <Frame BackgroundColor="{StaticResource Primary}" CornerRadius="18" Padding="16,8" HasShadow="False">
                <Label Text="En cours" TextColor="White" FontSize="14" FontAttributes="Bold">
                    <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SetTabCommand}" CommandParameter="active" /></Label.GestureRecognizers>
                </Label>
            </Frame>
            <Frame BackgroundColor="#F3F4F6" CornerRadius="18" Padding="16,8" HasShadow="False">
                <Label Text="Terminées" FontSize="14" TextColor="{StaticResource TextSecondary}">
                    <Label.GestureRecognizers><TapGestureRecognizer Command="{Binding SetTabCommand}" CommandParameter="completed" /></Label.GestureRecognizers>
                </Label>
            </Frame>
        </HorizontalStackLayout>
        <RefreshView Grid.Row="1" Command="{Binding LoadOrdersCommand}" IsRefreshing="{Binding IsRefreshing}">
            <CollectionView ItemsSource="{Binding Orders}">
                <CollectionView.EmptyView>
                    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
                        <Label Text="📦" FontSize="48" HorizontalOptions="Center" />
                        <Label Text="Aucune commande" Style="{StaticResource Title3}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="vm:OrderSummary">
                        <Frame Margin="16,6" CornerRadius="14" Padding="16" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="12">
                                <Frame BackgroundColor="#F3F4F6" CornerRadius="22" HeightRequest="44" WidthRequest="44" HasShadow="False" Padding="0">
                                    <Label Text="{Binding TypeIcon}" FontSize="20" HorizontalOptions="Center" VerticalOptions="Center" />
                                </Frame>
                                <VerticalStackLayout Grid.Column="1" Spacing="2">
                                    <Label Text="{Binding ProviderName}" FontAttributes="Bold" FontSize="15" />
                                    <Label Text="{Binding OrderNumber}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                    <Frame BackgroundColor="{Binding Status, Converter={StaticResource StatusToColor}}" CornerRadius="6" Padding="8,2" HasShadow="False" HorizontalOptions="Start">
                                        <Label Text="{Binding Status, Converter={StaticResource StatusToText}}" TextColor="White" FontSize="10" FontAttributes="Bold" />
                                    </Frame>
                                </VerticalStackLayout>
                                <VerticalStackLayout Grid.Column="2" VerticalOptions="Center" HorizontalOptions="End">
                                    <Label Text="{Binding TotalAmount, StringFormat='{0:N2} DH'}" FontAttributes="Bold" FontSize="14" TextColor="{StaticResource Primary}" />
                                    <Label Text="{Binding CreatedAt, StringFormat='{0:dd/MM HH:mm}'}" FontSize="11" TextColor="{StaticResource TextMuted}" />
                                </VerticalStackLayout>
                            </Grid>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:OrdersViewModel}}, Path=ViewOrderCommand}" CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </RefreshView>
    </Grid>
</ContentPage>
EOF
cat > "$BASE/Views/Common/OrdersPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Common;
namespace MultiServices.Maui.Views.Common;
public partial class OrdersPage : ContentPage
{
    private readonly OrdersViewModel _vm;
    public OrdersPage(OrdersViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadOrdersCommand.ExecuteAsync(null); }
}
EOF

cat > "$BASE/Views/Common/NotificationsPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.Views.Common.NotificationsPage"
             Title="Notifications">
    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
        <Label Text="🔔" FontSize="48" HorizontalOptions="Center" />
        <Label Text="Aucune notification" Style="{StaticResource Title3}" HorizontalOptions="Center" />
        <Label Text="Vos notifications apparaîtront ici" Style="{StaticResource BodyText}" HorizontalOptions="Center" />
    </VerticalStackLayout>
</ContentPage>
EOF
cat > "$BASE/Views/Common/NotificationsPage.xaml.cs" << 'EOF'
namespace MultiServices.Maui.Views.Common;
public partial class NotificationsPage : ContentPage { public NotificationsPage() { InitializeComponent(); } }
EOF

echo "✅ Common views created"

# ============================================================
# PROFILE VIEWS
# ============================================================
cat > "$BASE/Views/Profile/ProfilePage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Profile"
             x:Class="MultiServices.Maui.Views.Profile.ProfilePage"
             x:DataType="vm:ProfileViewModel"
             Title="Profil">
    <ScrollView>
        <VerticalStackLayout Spacing="16" Padding="16">
            <!-- Profile Header -->
            <Frame Style="{StaticResource CardFrame}">
                <HorizontalStackLayout Spacing="16">
                    <Frame BackgroundColor="{StaticResource PrimaryLight}" CornerRadius="35" HeightRequest="70" WidthRequest="70" HasShadow="False" Padding="0">
                        <Label Text="{Binding User.FirstName, StringFormat='{0:0}'}" FontSize="28" FontAttributes="Bold" TextColor="{StaticResource Primary}"
                               HorizontalOptions="Center" VerticalOptions="Center" />
                    </Frame>
                    <VerticalStackLayout VerticalOptions="Center" Spacing="4">
                        <Label Text="{Binding User.FullName}" Style="{StaticResource Title2}" />
                        <Label Text="{Binding User.Email}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                        <HorizontalStackLayout Spacing="4">
                            <Frame BackgroundColor="#FEF3C7" CornerRadius="8" Padding="8,2" HasShadow="False">
                                <Label Text="{Binding User.LoyaltyTier}" FontSize="11" FontAttributes="Bold" TextColor="#92400E" />
                            </Frame>
                            <Label Text="{Binding User.LoyaltyPoints, StringFormat='{0} pts'}" FontSize="12" TextColor="{StaticResource TextMuted}" VerticalOptions="Center" />
                        </HorizontalStackLayout>
                    </VerticalStackLayout>
                </HorizontalStackLayout>
            </Frame>

            <!-- Wallet -->
            <Frame BackgroundColor="{StaticResource Primary}" CornerRadius="16" Padding="20" HasShadow="True">
                <Grid ColumnDefinitions="*,Auto">
                    <VerticalStackLayout Spacing="4">
                        <Label Text="Mon portefeuille" TextColor="White" FontSize="13" Opacity="0.8" />
                        <Label Text="{Binding Wallet.Balance, StringFormat='{0:N2} DH'}" TextColor="White" FontSize="28" FontAttributes="Bold" />
                    </VerticalStackLayout>
                    <Button Grid.Column="1" Text="+ Recharger" BackgroundColor="White" TextColor="{StaticResource Primary}"
                            CornerRadius="12" FontSize="13" FontAttributes="Bold" HeightRequest="40" VerticalOptions="Center"
                            Command="{Binding ViewWalletCommand}" />
                </Grid>
            </Frame>

            <!-- Menu Items -->
            <Frame Style="{StaticResource CardFrame}" Padding="0">
                <VerticalStackLayout>
                    <!-- Edit Profile -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="👤" FontSize="20" />
                        <Label Grid.Column="1" Text="Modifier le profil" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding EditProfileCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" Margin="16,0" />
                    <!-- Addresses -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="📍" FontSize="20" />
                        <Label Grid.Column="1" Text="Mes adresses" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding ManageAddressesCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" Margin="16,0" />
                    <!-- Orders -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="📦" FontSize="20" />
                        <Label Grid.Column="1" Text="Mes commandes" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ProfileViewModel}}, Path=ViewFavoritesCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" Margin="16,0" />
                    <!-- Favorites -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="❤️" FontSize="20" />
                        <Label Grid.Column="1" Text="Mes favoris" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding ViewFavoritesCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" Margin="16,0" />
                    <!-- Loyalty -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="🏆" FontSize="20" />
                        <Label Grid.Column="1" Text="Programme fidélité" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding ViewLoyaltyCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" Margin="16,0" />
                    <!-- Support -->
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="16,14" ColumnSpacing="12">
                        <Label Text="💬" FontSize="20" />
                        <Label Grid.Column="1" Text="Support" FontSize="15" VerticalOptions="Center" />
                        <Label Grid.Column="2" Text="›" FontSize="20" TextColor="{StaticResource TextMuted}" />
                        <Grid.GestureRecognizers><TapGestureRecognizer Command="{Binding ContactSupportCommand}" /></Grid.GestureRecognizers>
                    </Grid>
                </VerticalStackLayout>
            </Frame>

            <!-- Logout / Delete -->
            <Button Text="Se déconnecter" Style="{StaticResource SecondaryButton}" Command="{Binding LogoutCommand}" />
            <Button Text="Supprimer mon compte" BackgroundColor="Transparent" TextColor="{StaticResource Danger}" 
                    FontSize="14" Command="{Binding DeleteAccountCommand}" Margin="0,0,0,20" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Profile/ProfilePage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Profile;
namespace MultiServices.Maui.Views.Profile;
public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;
    public ProfilePage(ProfileViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadProfileCommand.ExecuteAsync(null); }
}
EOF

# Simplified stub pages
for page in EditProfilePage AddressesPage SupportPage; do
cat > "$BASE/Views/Profile/${page}.xaml" << EOF
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.Views.Profile.${page}"
             Title="${page/Page/}">
    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
        <Label Text="🔧" FontSize="48" HorizontalOptions="Center" />
        <Label Text="Page en construction" FontSize="18" FontAttributes="Bold" HorizontalOptions="Center" />
        <Label Text="Cette fonctionnalité sera bientôt disponible" FontSize="14" TextColor="#6B7280" HorizontalOptions="Center" HorizontalTextAlignment="Center" />
    </VerticalStackLayout>
</ContentPage>
EOF
cat > "$BASE/Views/Profile/${page}.xaml.cs" << EOF
namespace MultiServices.Maui.Views.Profile;
public partial class ${page} : ContentPage { public ${page}() { InitializeComponent(); } }
EOF
done

echo "✅ Profile views created"
echo ""
echo "========================================="
echo "✅ ALL MAUI VIEWS generated successfully"
echo "========================================="
