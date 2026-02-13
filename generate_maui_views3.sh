#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# RESTAURANT VIEWS
# ============================================================
cat > "$BASE/Views/Restaurant/RestaurantsPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Restaurant"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Restaurant.RestaurantsPage"
             x:DataType="vm:RestaurantsViewModel"
             Title="Restaurants">
    <Grid RowDefinitions="Auto,*">
        <VerticalStackLayout Padding="16,8" Spacing="12" BackgroundColor="{StaticResource Surface}">
            <SearchBar Placeholder="Rechercher un restaurant..." Text="{Binding SearchQuery}"
                       SearchCommand="{Binding SearchCommand}" Style="{StaticResource SearchBarStyle}" />
            <ScrollView Orientation="Horizontal">
                <HorizontalStackLayout Spacing="8">
                    <CollectionView ItemsSource="{Binding CuisineTypes}" SelectionMode="Single"
                                    SelectionChangedCommand="{Binding Source={RelativeSource AncestorType={x:Type vm:RestaurantsViewModel}}, Path=SearchCommand}"
                                    HeightRequest="36">
                        <CollectionView.ItemsLayout>
                            <LinearItemsLayout Orientation="Horizontal" ItemSpacing="8" />
                        </CollectionView.ItemsLayout>
                        <CollectionView.ItemTemplate>
                            <DataTemplate x:DataType="x:String">
                                <Frame BackgroundColor="#F3F4F6" CornerRadius="18" Padding="14,6" HasShadow="False" BorderColor="Transparent">
                                    <Label Text="{Binding}" FontSize="13" TextColor="{StaticResource TextPrimary}" />
                                </Frame>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </HorizontalStackLayout>
            </ScrollView>
        </VerticalStackLayout>

        <RefreshView Grid.Row="1" Command="{Binding LoadRestaurantsCommand}" IsRefreshing="{Binding IsRefreshing}">
            <CollectionView ItemsSource="{Binding Restaurants}" SelectionMode="Single"
                            SelectionChangedCommand="{Binding SelectRestaurantCommand}"
                            SelectionChangedCommandParameter="{Binding SelectedItem, Source={RelativeSource Self}}">
                <CollectionView.EmptyView>
                    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
                        <Label Text="🍔" FontSize="48" HorizontalOptions="Center" />
                        <Label Text="Aucun restaurant trouvé" Style="{StaticResource Title3}" HorizontalOptions="Center" />
                        <Label Text="Essayez d'élargir vos critères" Style="{StaticResource BodyText}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:RestaurantListDto">
                        <Frame Margin="16,6" CornerRadius="16" Padding="0" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="100,*" HeightRequest="110">
                                <BoxView BackgroundColor="#FEF3C7" />
                                <Frame AbsoluteLayout.LayoutBounds="0.5,0.5,50,50" BackgroundColor="White" CornerRadius="25" Padding="0" HasShadow="False" HorizontalOptions="Center" VerticalOptions="Center">
                                    <Label Text="🍔" FontSize="24" HorizontalOptions="Center" VerticalOptions="Center" />
                                </Frame>
                                <VerticalStackLayout Grid.Column="1" Padding="12" Spacing="4" VerticalOptions="Center">
                                    <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="15" LineBreakMode="TailTruncation" />
                                    <Label Text="{Binding CuisineType}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                    <HorizontalStackLayout Spacing="12">
                                        <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                        <Label Text="{Binding EstimatedDeliveryMinutes, StringFormat='🕐 {0} min'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                        <Label Text="{Binding PriceRange}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                    </HorizontalStackLayout>
                                    <HorizontalStackLayout Spacing="8">
                                        <Label Text="{Binding DeliveryFee, StringFormat='Livraison: {0:N2} DH'}" FontSize="11" TextColor="{StaticResource TextMuted}" />
                                        <Frame IsVisible="{Binding HasActivePromotions}" BackgroundColor="#FEF3C7" CornerRadius="8" Padding="6,2" HasShadow="False">
                                            <Label Text="PROMO" FontSize="9" FontAttributes="Bold" TextColor="#92400E" />
                                        </Frame>
                                    </HorizontalStackLayout>
                                </VerticalStackLayout>
                            </Grid>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </RefreshView>
    </Grid>
</ContentPage>
EOF

cat > "$BASE/Views/Restaurant/RestaurantsPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantsPage : ContentPage
{
    private readonly RestaurantsViewModel _vm;
    public RestaurantsPage(RestaurantsViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadRestaurantsCommand.ExecuteAsync(null); }
}
EOF

cat > "$BASE/Views/Restaurant/RestaurantDetailPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Restaurant"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Restaurant.RestaurantDetailPage"
             x:DataType="vm:RestaurantDetailViewModel"
             Title="{Binding Title}">
    <Grid RowDefinitions="*,Auto">
        <ScrollView>
            <VerticalStackLayout Spacing="16">
                <!-- Hero -->
                <Grid HeightRequest="200">
                    <BoxView BackgroundColor="#FEF3C7" />
                    <VerticalStackLayout VerticalOptions="End" Padding="16" Spacing="4">
                        <Label Text="{Binding Restaurant.Name}" FontSize="24" FontAttributes="Bold" TextColor="{StaticResource TextPrimary}" />
                        <HorizontalStackLayout Spacing="12">
                            <Label Text="{Binding Restaurant.CuisineType}" FontSize="14" TextColor="{StaticResource TextSecondary}" />
                            <Label Text="{Binding Restaurant.Rating, StringFormat='★ {0:F1}'}" FontSize="14" TextColor="#F59E0B" FontAttributes="Bold" />
                            <Label Text="{Binding Restaurant.PriceRange}" FontSize="14" TextColor="{StaticResource TextSecondary}" />
                        </HorizontalStackLayout>
                        <HorizontalStackLayout Spacing="16">
                            <Label Text="{Binding Restaurant.EstimatedDeliveryMinutes, StringFormat='🕐 {0} min'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                            <Label Text="{Binding Restaurant.DeliveryFee, StringFormat='🛵 {0:N2} DH'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                            <Label Text="{Binding Restaurant.MinimumOrder, StringFormat='Min: {0:N2} DH'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                        </HorizontalStackLayout>
                    </VerticalStackLayout>
                    <Button Text="{Binding IsFavorite, Converter={StaticResource BoolToFav}}" BackgroundColor="White" TextColor="{StaticResource Danger}"
                            CornerRadius="20" HeightRequest="40" WidthRequest="40" HorizontalOptions="End" VerticalOptions="Start" Margin="16"
                            Command="{Binding ToggleFavoriteCommand}" />
                </Grid>

                <!-- Menu Categories -->
                <ScrollView Orientation="Horizontal" Padding="16,0">
                    <HorizontalStackLayout Spacing="8" BindableLayout.ItemsSource="{Binding MenuCategories}">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:MenuCategoryDto">
                                <Frame BackgroundColor="{StaticResource PrimaryLight}" CornerRadius="18" Padding="14,8" HasShadow="False" BorderColor="Transparent">
                                    <Label Text="{Binding Name}" FontSize="13" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                                </Frame>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </HorizontalStackLayout>
                </ScrollView>

                <!-- Menu Items -->
                <VerticalStackLayout Padding="16" Spacing="8" BindableLayout.ItemsSource="{Binding MenuCategories}">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate x:DataType="models:MenuCategoryDto">
                            <VerticalStackLayout Spacing="8">
                                <Label Text="{Binding Name}" Style="{StaticResource Title3}" Margin="0,8,0,4" />
                                <VerticalStackLayout Spacing="8" BindableLayout.ItemsSource="{Binding Items}">
                                    <BindableLayout.ItemTemplate>
                                        <DataTemplate x:DataType="models:MenuItemDto">
                                            <Frame CornerRadius="12" Padding="12" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                                                <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
                                                    <VerticalStackLayout Spacing="4">
                                                        <HorizontalStackLayout Spacing="6">
                                                            <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="15" />
                                                            <Frame IsVisible="{Binding IsPopular}" BackgroundColor="#FEF3C7" CornerRadius="6" Padding="6,1" HasShadow="False">
                                                                <Label Text="★ Populaire" FontSize="9" TextColor="#92400E" />
                                                            </Frame>
                                                        </HorizontalStackLayout>
                                                        <Label Text="{Binding Description}" FontSize="12" TextColor="{StaticResource TextMuted}" MaxLines="2" LineBreakMode="TailTruncation" />
                                                        <Label Text="{Binding BasePrice, StringFormat='{0:N2} DH'}" FontSize="15" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                                                    </VerticalStackLayout>
                                                    <Button Grid.Column="1" Text="+" BackgroundColor="{StaticResource Primary}" TextColor="White"
                                                            CornerRadius="20" HeightRequest="40" WidthRequest="40" FontSize="18" VerticalOptions="Center"
                                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:RestaurantDetailViewModel}}, Path=AddToCartCommand}"
                                                            CommandParameter="{Binding}" />
                                                </Grid>
                                            </Frame>
                                        </DataTemplate>
                                    </BindableLayout.ItemTemplate>
                                </VerticalStackLayout>
                            </VerticalStackLayout>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </VerticalStackLayout>
            </VerticalStackLayout>
        </ScrollView>

        <!-- Cart Bar -->
        <Frame Grid.Row="1" BackgroundColor="{StaticResource Primary}" CornerRadius="0" Padding="16,12" HasShadow="True"
               IsVisible="{Binding Cart.ItemCount, Converter={StaticResource NullToBool}}">
            <Grid ColumnDefinitions="Auto,*,Auto">
                <Frame BackgroundColor="White" CornerRadius="12" Padding="8,4" HasShadow="False">
                    <Label Text="{Binding Cart.ItemCount}" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                </Frame>
                <Label Grid.Column="1" Text="Voir le panier" TextColor="White" FontAttributes="Bold" FontSize="16"
                       VerticalOptions="Center" HorizontalOptions="Center">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding ProceedToCheckoutCommand}" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Grid.Column="2" Text="{Binding Cart.Total, StringFormat='{0:N2} DH'}" TextColor="White" FontAttributes="Bold" FontSize="16" VerticalOptions="Center" />
            </Grid>
        </Frame>
    </Grid>
</ContentPage>
EOF

cat > "$BASE/Views/Restaurant/RestaurantDetailPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantDetailPage : ContentPage
{
    public RestaurantDetailPage(RestaurantDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

# Checkout + Order Tracking (simplified)
cat > "$BASE/Views/Restaurant/CheckoutPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.Views.Restaurant.CheckoutPage"
             Title="Valider la commande">
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
                    <Label Text="💳 Mode de paiement" Style="{StaticResource Title3}" />
                    <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="12" HasShadow="False">
                        <Label Text="Carte bancaire •••• 4242" FontSize="14" />
                    </Frame>
                    <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="12" HasShadow="False">
                        <Label Text="💵 Espèces à la livraison" FontSize="14" />
                    </Frame>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="🎁 Code promo" Style="{StaticResource Title3}" />
                    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
                        <Entry Placeholder="Entrez votre code" Style="{StaticResource EntryStyle}" />
                        <Button Text="Appliquer" BackgroundColor="{StaticResource Primary}" TextColor="White" CornerRadius="10" />
                    </Grid>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="💰 Pourboire livreur" Style="{StaticResource Title3}" />
                    <HorizontalStackLayout Spacing="8" HorizontalOptions="Center">
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="16,8" HasShadow="False"><Label Text="5 DH" FontSize="14" /></Frame>
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="16,8" HasShadow="False"><Label Text="10 DH" FontSize="14" /></Frame>
                        <Frame BackgroundColor="#F3F4F6" CornerRadius="12" Padding="16,8" HasShadow="False"><Label Text="20 DH" FontSize="14" /></Frame>
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Frame>
            <Button Text="Confirmer la commande" Style="{StaticResource PrimaryButton}" Margin="0,8" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Restaurant/CheckoutPage.xaml.cs" << 'EOF'
namespace MultiServices.Maui.Views.Restaurant;
public partial class CheckoutPage : ContentPage { public CheckoutPage() { InitializeComponent(); } }
EOF

cat > "$BASE/Views/Restaurant/RestaurantOrderTrackingPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Restaurant"
             x:Class="MultiServices.Maui.Views.Restaurant.RestaurantOrderTrackingPage"
             x:DataType="vm:RestaurantOrderTrackingViewModel"
             Title="Suivi commande">
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="{Binding Order.OrderNumber, StringFormat='Commande #{0}'}" Style="{StaticResource Title2}" />
                    <Label Text="{Binding Order.RestaurantName}" FontSize="15" TextColor="{StaticResource TextSecondary}" />
                    <Frame BackgroundColor="{Binding Order.Status, Converter={StaticResource StatusToColor}}" CornerRadius="8" Padding="12,6" HasShadow="False" HorizontalOptions="Start">
                        <Label Text="{Binding Order.Status, Converter={StaticResource StatusToText}}" TextColor="White" FontAttributes="Bold" FontSize="14" />
                    </Frame>
                </VerticalStackLayout>
            </Frame>
            <!-- Status Timeline -->
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="4" BindableLayout.ItemsSource="{Binding StatusHistory}">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate x:DataType="models:OrderStatusHistoryDto" xmlns:models="clr-namespace:MultiServices.Maui.Models">
                            <HorizontalStackLayout Spacing="12" Padding="0,4">
                                <Frame BackgroundColor="{StaticResource Success}" CornerRadius="8" HeightRequest="16" WidthRequest="16" HasShadow="False" Padding="0" />
                                <VerticalStackLayout>
                                    <Label Text="{Binding Status}" FontAttributes="Bold" FontSize="14" />
                                    <Label Text="{Binding Timestamp, StringFormat='{0:HH:mm}'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                </VerticalStackLayout>
                            </HorizontalStackLayout>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </VerticalStackLayout>
            </Frame>
            <!-- Deliverer info -->
            <Frame Style="{StaticResource CardFrame}" IsVisible="{Binding Order.DelivererName, Converter={StaticResource NullToBool}}">
                <Grid ColumnDefinitions="Auto,*,Auto">
                    <Frame BackgroundColor="{StaticResource PrimaryLight}" CornerRadius="25" HeightRequest="50" WidthRequest="50" HasShadow="False" Padding="0">
                        <Label Text="🛵" FontSize="24" HorizontalOptions="Center" VerticalOptions="Center" />
                    </Frame>
                    <VerticalStackLayout Grid.Column="1" Padding="12,0" VerticalOptions="Center">
                        <Label Text="{Binding Order.DelivererName}" FontAttributes="Bold" FontSize="15" />
                        <Label Text="Votre livreur" FontSize="12" TextColor="{StaticResource TextMuted}" />
                    </VerticalStackLayout>
                    <Button Grid.Column="2" Text="📞" BackgroundColor="{StaticResource Success}" TextColor="White"
                            CornerRadius="20" HeightRequest="40" WidthRequest="40" Command="{Binding CallDelivererCommand}" VerticalOptions="Center" />
                </Grid>
            </Frame>
            <!-- Order Summary -->
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="Résumé" Style="{StaticResource Title3}" />
                    <Grid ColumnDefinitions="*,Auto"><Label Text="Sous-total" /><Label Grid.Column="1" Text="{Binding Order.SubTotal, StringFormat='{0:N2} DH'}" /></Grid>
                    <Grid ColumnDefinitions="*,Auto"><Label Text="Livraison" /><Label Grid.Column="1" Text="{Binding Order.DeliveryFee, StringFormat='{0:N2} DH'}" /></Grid>
                    <BoxView HeightRequest="1" BackgroundColor="{StaticResource Border}" />
                    <Grid ColumnDefinitions="*,Auto"><Label Text="Total" FontAttributes="Bold" /><Label Grid.Column="1" Text="{Binding Order.TotalAmount, StringFormat='{0:N2} DH'}" FontAttributes="Bold" TextColor="{StaticResource Primary}" /></Grid>
                </VerticalStackLayout>
            </Frame>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Restaurant/RestaurantOrderTrackingPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Restaurant;
namespace MultiServices.Maui.Views.Restaurant;
public partial class RestaurantOrderTrackingPage : ContentPage
{
    public RestaurantOrderTrackingPage(RestaurantOrderTrackingViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

echo "✅ Restaurant views created"

# ============================================================
# SERVICE VIEWS
# ============================================================
cat > "$BASE/Views/Services/ServicesPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Services"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Services.ServicesPage"
             x:DataType="vm:ServicesViewModel"
             Title="Services">
    <Grid RowDefinitions="Auto,Auto,*">
        <SearchBar Placeholder="Rechercher un prestataire..." Text="{Binding SearchQuery}"
                   SearchCommand="{Binding LoadProvidersCommand}" Style="{StaticResource SearchBarStyle}" Margin="16,8" />
        <!-- Category Cards -->
        <ScrollView Grid.Row="1" Orientation="Horizontal" Padding="16,0" Margin="0,0,0,8">
            <HorizontalStackLayout Spacing="10" BindableLayout.ItemsSource="{Binding Categories}">
                <BindableLayout.ItemTemplate>
                    <DataTemplate x:DataType="vm:CategoryItem">
                        <Frame CornerRadius="14" Padding="14,10" HasShadow="False" BackgroundColor="#EEF2FF" BorderColor="Transparent">
                            <HorizontalStackLayout Spacing="6">
                                <Label Text="{Binding Icon}" FontSize="18" />
                                <Label Text="{Binding Label}" FontSize="13" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                            </HorizontalStackLayout>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ServicesViewModel}}, Path=SelectCategoryCommand}"
                                                      CommandParameter="{Binding Key}" />
                            </Frame.GestureRecognizers>
                        </Frame>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </HorizontalStackLayout>
        </ScrollView>
        <!-- Providers List -->
        <RefreshView Grid.Row="2" Command="{Binding LoadProvidersCommand}" IsRefreshing="{Binding IsRefreshing}">
            <CollectionView ItemsSource="{Binding Providers}">
                <CollectionView.EmptyView>
                    <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="8" Padding="40">
                        <Label Text="🛠️" FontSize="48" HorizontalOptions="Center" />
                        <Label Text="Aucun prestataire trouvé" Style="{StaticResource Title3}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:ServiceProviderListDto">
                        <Frame Margin="16,6" CornerRadius="16" Padding="16" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="50,*" ColumnSpacing="12">
                                <Frame BackgroundColor="#DBEAFE" CornerRadius="25" HeightRequest="50" WidthRequest="50" HasShadow="False" Padding="0">
                                    <Label Text="🛠️" FontSize="22" HorizontalOptions="Center" VerticalOptions="Center" />
                                </Frame>
                                <VerticalStackLayout Grid.Column="1" Spacing="4">
                                    <Label Text="{Binding CompanyName}" FontAttributes="Bold" FontSize="15" />
                                    <Label Text="{Binding Category}" FontSize="12" TextColor="{StaticResource Info}" />
                                    <HorizontalStackLayout Spacing="12">
                                        <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                        <Label Text="{Binding YearsExperience, StringFormat='{0} ans'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                        <Label Text="{Binding StartingPrice, StringFormat='À partir de {0:N0} DH'}" FontSize="12" TextColor="{StaticResource Primary}" />
                                    </HorizontalStackLayout>
                                </VerticalStackLayout>
                            </Grid>
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ServicesViewModel}}, Path=SelectProviderCommand}"
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
cat > "$BASE/Views/Services/ServicesPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class ServicesPage : ContentPage
{
    private readonly ServicesViewModel _vm;
    public ServicesPage(ServicesViewModel viewModel) { InitializeComponent(); BindingContext = _vm = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadProvidersCommand.ExecuteAsync(null); }
}
EOF

# Service Detail + Booking + Tracking (simplified XAML)
cat > "$BASE/Views/Services/ServiceDetailPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Services"
             xmlns:models="clr-namespace:MultiServices.Maui.Models"
             x:Class="MultiServices.Maui.Views.Services.ServiceDetailPage"
             x:DataType="vm:ServiceDetailViewModel"
             Title="{Binding Title}">
    <ScrollView>
        <VerticalStackLayout Spacing="16" Padding="16">
            <!-- Provider Header -->
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Grid ColumnDefinitions="60,*,Auto">
                        <Frame BackgroundColor="#DBEAFE" CornerRadius="30" HeightRequest="60" WidthRequest="60" HasShadow="False" Padding="0">
                            <Label Text="🛠️" FontSize="28" HorizontalOptions="Center" VerticalOptions="Center" />
                        </Frame>
                        <VerticalStackLayout Grid.Column="1" Padding="12,0" VerticalOptions="Center">
                            <Label Text="{Binding Provider.CompanyName}" Style="{StaticResource Title2}" />
                            <Label Text="{Binding Provider.Category}" FontSize="14" TextColor="{StaticResource Info}" />
                        </VerticalStackLayout>
                        <Button Grid.Column="2" Text="♡" BackgroundColor="Transparent" TextColor="{StaticResource Danger}" FontSize="22"
                                Command="{Binding ToggleFavoriteCommand}" VerticalOptions="Start" />
                    </Grid>
                    <HorizontalStackLayout Spacing="16">
                        <Label Text="{Binding Provider.Rating, StringFormat='★ {0:F1}'}" FontSize="14" TextColor="#F59E0B" FontAttributes="Bold" />
                        <Label Text="{Binding Provider.TotalReviews, StringFormat='{0} avis'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                        <Label Text="{Binding Provider.YearsExperience, StringFormat='{0} ans exp.'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                        <Label Text="{Binding Provider.CompletedInterventions, StringFormat='{0} interventions'}" FontSize="13" TextColor="{StaticResource TextMuted}" />
                    </HorizontalStackLayout>
                    <Label Text="{Binding Provider.Description}" Style="{StaticResource BodyText}" />
                </VerticalStackLayout>
            </Frame>
            <!-- Services -->
            <Label Text="Services proposés" Style="{StaticResource Title3}" />
            <VerticalStackLayout Spacing="8" BindableLayout.ItemsSource="{Binding Services}">
                <BindableLayout.ItemTemplate>
                    <DataTemplate x:DataType="models:ServiceOfferingDto">
                        <Frame CornerRadius="12" Padding="14" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                            <Grid ColumnDefinitions="*,Auto">
                                <VerticalStackLayout Spacing="4">
                                    <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="15" />
                                    <Label Text="{Binding Description}" FontSize="12" TextColor="{StaticResource TextMuted}" MaxLines="2" />
                                    <HorizontalStackLayout Spacing="8">
                                        <Label Text="{Binding PricingType}" FontSize="12" TextColor="{StaticResource Info}" />
                                        <Label Text="{Binding FixedPrice, StringFormat='{0:N0} DH'}" FontSize="14" FontAttributes="Bold" TextColor="{StaticResource Primary}" />
                                    </HorizontalStackLayout>
                                </VerticalStackLayout>
                                <Button Grid.Column="1" Text="Réserver" BackgroundColor="{StaticResource Primary}" TextColor="White"
                                        CornerRadius="10" FontSize="13" Padding="12,6" VerticalOptions="Center"
                                        Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ServiceDetailViewModel}}, Path=BookServiceCommand}"
                                        CommandParameter="{Binding}" />
                            </Grid>
                        </Frame>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </VerticalStackLayout>
            <!-- Portfolio -->
            <Label Text="Portfolio" Style="{StaticResource Title3}" Margin="0,8,0,0" />
            <CollectionView ItemsSource="{Binding Portfolio}" HeightRequest="180">
                <CollectionView.ItemsLayout><LinearItemsLayout Orientation="Horizontal" ItemSpacing="12" /></CollectionView.ItemsLayout>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:PortfolioItemDto">
                        <Frame WidthRequest="220" CornerRadius="12" Padding="0" HasShadow="True" BorderColor="Transparent">
                            <VerticalStackLayout>
                                <Grid ColumnDefinitions="*,*" HeightRequest="100">
                                    <BoxView BackgroundColor="#FEE2E2" /><Label Text="Avant" FontSize="10" HorizontalOptions="Center" VerticalOptions="Center" />
                                    <BoxView Grid.Column="1" BackgroundColor="#D1FAE5" /><Label Grid.Column="1" Text="Après" FontSize="10" HorizontalOptions="Center" VerticalOptions="Center" />
                                </Grid>
                                <Label Text="{Binding Title}" Padding="8" FontSize="13" FontAttributes="Bold" />
                            </VerticalStackLayout>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
            <!-- Reviews -->
            <Label Text="Avis clients" Style="{StaticResource Title3}" Margin="0,8,0,0" />
            <VerticalStackLayout Spacing="8" BindableLayout.ItemsSource="{Binding Reviews}">
                <BindableLayout.ItemTemplate>
                    <DataTemplate x:DataType="models:ReviewDto">
                        <Frame CornerRadius="12" Padding="12" HasShadow="False" BackgroundColor="White" BorderColor="{StaticResource Border}">
                            <VerticalStackLayout Spacing="4">
                                <HorizontalStackLayout Spacing="8">
                                    <Label Text="{Binding UserName}" FontAttributes="Bold" FontSize="14" />
                                    <Label Text="{Binding Rating, StringFormat='★ {0}'}" TextColor="#F59E0B" FontSize="13" />
                                </HorizontalStackLayout>
                                <Label Text="{Binding Comment}" FontSize="13" TextColor="{StaticResource TextSecondary}" />
                                <Label Text="{Binding CreatedAt, StringFormat='{0:dd/MM/yyyy}'}" FontSize="11" TextColor="{StaticResource TextMuted}" />
                            </VerticalStackLayout>
                        </Frame>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </VerticalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Services/ServiceDetailPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class ServiceDetailPage : ContentPage
{
    public ServiceDetailPage(ServiceDetailViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

cat > "$BASE/Views/Services/BookServicePage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Services"
             x:Class="MultiServices.Maui.Views.Services.BookServicePage"
             x:DataType="vm:BookServiceViewModel"
             Title="Réserver un service">
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">
            <!-- Step Indicator -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                <Frame BackgroundColor="{StaticResource Primary}" CornerRadius="14" HeightRequest="28" WidthRequest="28" HasShadow="False" Padding="0">
                    <Label Text="1" TextColor="White" FontSize="12" FontAttributes="Bold" HorizontalOptions="Center" VerticalOptions="Center" />
                </Frame>
                <BoxView WidthRequest="30" HeightRequest="2" BackgroundColor="{StaticResource Border}" VerticalOptions="Center" />
                <Frame BackgroundColor="#E5E7EB" CornerRadius="14" HeightRequest="28" WidthRequest="28" HasShadow="False" Padding="0">
                    <Label Text="2" TextColor="{StaticResource TextMuted}" FontSize="12" HorizontalOptions="Center" VerticalOptions="Center" />
                </Frame>
                <BoxView WidthRequest="30" HeightRequest="2" BackgroundColor="{StaticResource Border}" VerticalOptions="Center" />
                <Frame BackgroundColor="#E5E7EB" CornerRadius="14" HeightRequest="28" WidthRequest="28" HasShadow="False" Padding="0">
                    <Label Text="3" TextColor="{StaticResource TextMuted}" FontSize="12" HorizontalOptions="Center" VerticalOptions="Center" />
                </Frame>
            </HorizontalStackLayout>

            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="12">
                    <Label Text="📝 Décrivez votre problème" Style="{StaticResource Title3}" />
                    <Editor Placeholder="Décrivez le problème en détail..." Text="{Binding ProblemDescription}"
                            HeightRequest="120" BackgroundColor="#F9FAFB" />
                    <HorizontalStackLayout Spacing="8">
                        <Button Text="📷 Photo" Style="{StaticResource SecondaryButton}" Command="{Binding AddPhotoCommand}" HeightRequest="40" />
                        <Button Text="🖼️ Galerie" Style="{StaticResource SecondaryButton}" Command="{Binding PickPhotoCommand}" HeightRequest="40" />
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Frame>

            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="12">
                    <Label Text="📅 Date et heure" Style="{StaticResource Title3}" />
                    <DatePicker Date="{Binding SelectedDate}" MinimumDate="{x:Static sys:DateTime.Today}"
                                xmlns:sys="clr-namespace:System;assembly=netstandard" />
                    <Button Text="Voir les créneaux" Style="{StaticResource SecondaryButton}" Command="{Binding LoadSlotsCommand}" HeightRequest="40" />
                </VerticalStackLayout>
            </Frame>

            <Button Text="Confirmer la réservation" Style="{StaticResource PrimaryButton}" Command="{Binding ConfirmBookingCommand}" Margin="0,8" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Services/BookServicePage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class BookServicePage : ContentPage
{
    public BookServicePage(BookServiceViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

cat > "$BASE/Views/Services/InterventionTrackingPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Services"
             x:Class="MultiServices.Maui.Views.Services.InterventionTrackingPage"
             x:DataType="vm:InterventionTrackingViewModel"
             Title="Suivi intervention">
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="{Binding Intervention.InterventionNumber, StringFormat='Intervention #{0}'}" Style="{StaticResource Title2}" />
                    <Label Text="{Binding Intervention.ProviderName}" FontSize="15" TextColor="{StaticResource TextSecondary}" />
                    <Label Text="{Binding Intervention.ServiceName}" FontSize="14" TextColor="{StaticResource Info}" />
                    <Frame BackgroundColor="{Binding Intervention.Status, Converter={StaticResource StatusToColor}}" CornerRadius="8" Padding="12,6" HasShadow="False" HorizontalOptions="Start">
                        <Label Text="{Binding Intervention.Status, Converter={StaticResource StatusToText}}" TextColor="White" FontAttributes="Bold" />
                    </Frame>
                </VerticalStackLayout>
            </Frame>
            <Frame Style="{StaticResource CardFrame}" IsVisible="{Binding Intervention.IntervenantName, Converter={StaticResource NullToBool}}">
                <Grid ColumnDefinitions="50,*,Auto">
                    <Frame BackgroundColor="#DBEAFE" CornerRadius="25" HeightRequest="50" WidthRequest="50" HasShadow="False" Padding="0">
                        <Label Text="👷" FontSize="24" HorizontalOptions="Center" VerticalOptions="Center" />
                    </Frame>
                    <VerticalStackLayout Grid.Column="1" Padding="12,0" VerticalOptions="Center">
                        <Label Text="{Binding Intervention.IntervenantName}" FontAttributes="Bold" />
                        <Label Text="Votre intervenant" FontSize="12" TextColor="{StaticResource TextMuted}" />
                    </VerticalStackLayout>
                    <Button Grid.Column="2" Text="📞" BackgroundColor="{StaticResource Success}" TextColor="White"
                            CornerRadius="20" HeightRequest="40" WidthRequest="40" Command="{Binding CallIntervenantCommand}" />
                </Grid>
            </Frame>
            <Frame Style="{StaticResource CardFrame}">
                <VerticalStackLayout Spacing="8">
                    <Label Text="Détails" Style="{StaticResource Title3}" />
                    <Grid ColumnDefinitions="*,Auto"><Label Text="Date prévue" /><Label Grid.Column="1" Text="{Binding Intervention.ScheduledDate, StringFormat='{0:dd/MM/yyyy}'}" /></Grid>
                    <Grid ColumnDefinitions="*,Auto"><Label Text="Coût estimé" /><Label Grid.Column="1" Text="{Binding Intervention.EstimatedCost, StringFormat='{0:N2} DH'}" FontAttributes="Bold" TextColor="{StaticResource Primary}" /></Grid>
                </VerticalStackLayout>
            </Frame>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF
cat > "$BASE/Views/Services/InterventionTrackingPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Services;
namespace MultiServices.Maui.Views.Services;
public partial class InterventionTrackingPage : ContentPage
{
    public InterventionTrackingPage(InterventionTrackingViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

echo "✅ Service views created"
