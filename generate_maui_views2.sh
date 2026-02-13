#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# AUTH VIEWS
# ============================================================
cat > "$BASE/Views/Auth/LoginPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Auth"
             x:Class="MultiServices.Maui.Views.Auth.LoginPage"
             x:DataType="vm:LoginViewModel"
             Shell.NavBarIsVisible="False"
             BackgroundColor="{StaticResource Background}">
    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="20" VerticalOptions="Center">
            <!-- Logo -->
            <VerticalStackLayout Spacing="8" HorizontalOptions="Center" Margin="0,40,0,20">
                <Label Text="🏠" FontSize="60" HorizontalOptions="Center" />
                <Label Text="MultiServices" Style="{StaticResource Title1}" HorizontalOptions="Center" />
                <Label Text="Restaurants • Services • Courses" Style="{StaticResource BodyText}" HorizontalOptions="Center" />
            </VerticalStackLayout>

            <!-- Form -->
            <Frame Style="{StaticResource CardFrame}" Padding="20">
                <VerticalStackLayout Spacing="16">
                    <Label Text="Connexion" Style="{StaticResource Title2}" />

                    <VerticalStackLayout Spacing="6">
                        <Label Text="Email" Style="{StaticResource Caption}" />
                        <Entry Placeholder="votre@email.com" Keyboard="Email"
                               Text="{Binding Email}" Style="{StaticResource EntryStyle}" />
                    </VerticalStackLayout>

                    <VerticalStackLayout Spacing="6">
                        <Label Text="Mot de passe" Style="{StaticResource Caption}" />
                        <Grid ColumnDefinitions="*,Auto">
                            <Entry Placeholder="••••••••" IsPassword="{Binding ShowPassword, Converter={StaticResource InverseBool}}"
                                   Text="{Binding Password}" Style="{StaticResource EntryStyle}" />
                            <Button Grid.Column="1" Text="👁" BackgroundColor="Transparent"
                                    TextColor="{StaticResource TextMuted}" Command="{Binding TogglePasswordCommand}" />
                        </Grid>
                    </VerticalStackLayout>

                    <Label Text="Mot de passe oublié ?" TextColor="{StaticResource Primary}"
                           FontSize="13" HorizontalOptions="End">
                        <Label.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding ForgotPasswordCommand}" />
                        </Label.GestureRecognizers>
                    </Label>

                    <Button Text="Se connecter" Style="{StaticResource PrimaryButton}"
                            Command="{Binding LoginCommand}" IsEnabled="{Binding IsBusy, Converter={StaticResource InverseBool}}" />

                    <ActivityIndicator IsVisible="{Binding IsBusy}" IsRunning="{Binding IsBusy}"
                                       Color="{StaticResource Primary}" HeightRequest="30" />
                </VerticalStackLayout>
            </Frame>

            <!-- Social Login -->
            <VerticalStackLayout Spacing="12">
                <Label Text="Ou continuer avec" HorizontalOptions="Center" Style="{StaticResource Caption}" />
                <HorizontalStackLayout Spacing="16" HorizontalOptions="Center">
                    <Frame BackgroundColor="White" CornerRadius="12" Padding="16,10" HasShadow="True" BorderColor="Transparent">
                        <Label Text="Google" FontSize="14" FontAttributes="Bold">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding SocialLoginCommand}" CommandParameter="Google" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Frame>
                    <Frame BackgroundColor="White" CornerRadius="12" Padding="16,10" HasShadow="True" BorderColor="Transparent">
                        <Label Text="Facebook" FontSize="14" FontAttributes="Bold" TextColor="#1877F2">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding SocialLoginCommand}" CommandParameter="Facebook" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Frame>
                    <Frame BackgroundColor="White" CornerRadius="12" Padding="16,10" HasShadow="True" BorderColor="Transparent">
                        <Label Text="Apple" FontSize="14" FontAttributes="Bold">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding SocialLoginCommand}" CommandParameter="Apple" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Frame>
                </HorizontalStackLayout>
            </VerticalStackLayout>

            <!-- Register Link -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="4" Margin="0,10,0,40">
                <Label Text="Pas encore de compte ?" Style="{StaticResource BodyText}" />
                <Label Text="S'inscrire" TextColor="{StaticResource Primary}" FontAttributes="Bold" FontSize="15">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding GoToRegisterCommand}" />
                    </Label.GestureRecognizers>
                </Label>
            </HorizontalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF

cat > "$BASE/Views/Auth/LoginPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Auth;

namespace MultiServices.Maui.Views.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
EOF

cat > "$BASE/Views/Auth/RegisterPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Auth"
             x:Class="MultiServices.Maui.Views.Auth.RegisterPage"
             x:DataType="vm:RegisterViewModel"
             Shell.NavBarIsVisible="False">
    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="16" VerticalOptions="Center">
            <Label Text="Créer un compte" Style="{StaticResource Title1}" Margin="0,40,0,10" />
            <Label Text="Rejoignez MultiServices" Style="{StaticResource BodyText}" />

            <Frame Style="{StaticResource CardFrame}" Padding="20">
                <VerticalStackLayout Spacing="14">
                    <Grid ColumnDefinitions="*,*" ColumnSpacing="12">
                        <VerticalStackLayout Spacing="4">
                            <Label Text="Prénom" Style="{StaticResource Caption}" />
                            <Entry Placeholder="Prénom" Text="{Binding FirstName}" Style="{StaticResource EntryStyle}" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Grid.Column="1" Spacing="4">
                            <Label Text="Nom" Style="{StaticResource Caption}" />
                            <Entry Placeholder="Nom" Text="{Binding LastName}" Style="{StaticResource EntryStyle}" />
                        </VerticalStackLayout>
                    </Grid>
                    <VerticalStackLayout Spacing="4">
                        <Label Text="Email" Style="{StaticResource Caption}" />
                        <Entry Placeholder="votre@email.com" Keyboard="Email" Text="{Binding Email}" Style="{StaticResource EntryStyle}" />
                    </VerticalStackLayout>
                    <VerticalStackLayout Spacing="4">
                        <Label Text="Téléphone" Style="{StaticResource Caption}" />
                        <Entry Placeholder="+212 6XX XXX XXX" Keyboard="Telephone" Text="{Binding Phone}" Style="{StaticResource EntryStyle}" />
                    </VerticalStackLayout>
                    <VerticalStackLayout Spacing="4">
                        <Label Text="Mot de passe" Style="{StaticResource Caption}" />
                        <Entry Placeholder="Min. 8 caractères" IsPassword="True" Text="{Binding Password}" Style="{StaticResource EntryStyle}" />
                    </VerticalStackLayout>
                    <VerticalStackLayout Spacing="4">
                        <Label Text="Confirmer le mot de passe" Style="{StaticResource Caption}" />
                        <Entry Placeholder="Confirmer" IsPassword="True" Text="{Binding ConfirmPassword}" Style="{StaticResource EntryStyle}" />
                    </VerticalStackLayout>
                    <HorizontalStackLayout Spacing="8">
                        <CheckBox IsChecked="{Binding AcceptTerms}" Color="{StaticResource Primary}" />
                        <Label Text="J'accepte les conditions d'utilisation" Style="{StaticResource BodyText}" VerticalOptions="Center" />
                    </HorizontalStackLayout>
                    <Button Text="S'inscrire" Style="{StaticResource PrimaryButton}" Command="{Binding RegisterCommand}"
                            IsEnabled="{Binding IsBusy, Converter={StaticResource InverseBool}}" />
                    <ActivityIndicator IsVisible="{Binding IsBusy}" IsRunning="{Binding IsBusy}" Color="{StaticResource Primary}" />
                </VerticalStackLayout>
            </Frame>

            <HorizontalStackLayout HorizontalOptions="Center" Spacing="4" Margin="0,0,0,40">
                <Label Text="Déjà un compte ?" Style="{StaticResource BodyText}" />
                <Label Text="Se connecter" TextColor="{StaticResource Primary}" FontAttributes="Bold">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding GoToLoginCommand}" />
                    </Label.GestureRecognizers>
                </Label>
            </HorizontalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF

cat > "$BASE/Views/Auth/RegisterPage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Auth;
namespace MultiServices.Maui.Views.Auth;
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
EOF

cat > "$BASE/Views/Auth/ForgotPasswordPage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MultiServices.Maui.Views.Auth.ForgotPasswordPage"
             Title="Mot de passe oublié">
    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="20" VerticalOptions="Center">
            <Label Text="🔒" FontSize="60" HorizontalOptions="Center" />
            <Label Text="Mot de passe oublié ?" Style="{StaticResource Title2}" HorizontalOptions="Center" />
            <Label Text="Entrez votre email pour recevoir un lien de réinitialisation" Style="{StaticResource BodyText}" HorizontalOptions="Center" HorizontalTextAlignment="Center" />
            <Frame Style="{StaticResource CardFrame}" Padding="20">
                <VerticalStackLayout Spacing="16">
                    <Entry x:Name="EmailEntry" Placeholder="votre@email.com" Keyboard="Email" Style="{StaticResource EntryStyle}" />
                    <Button Text="Envoyer le lien" Style="{StaticResource PrimaryButton}" Clicked="OnSendClicked" />
                </VerticalStackLayout>
            </Frame>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
EOF

cat > "$BASE/Views/Auth/ForgotPasswordPage.xaml.cs" << 'EOF'
namespace MultiServices.Maui.Views.Auth;
public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage() { InitializeComponent(); }
    private async void OnSendClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre email", "OK");
            return;
        }
        await DisplayAlert("Email envoyé", "Vérifiez votre boîte de réception pour réinitialiser votre mot de passe.", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
EOF

echo "✅ Auth views created"

# ============================================================
# HOME PAGE
# ============================================================
cat > "$BASE/Views/Common/HomePage.xaml" << 'EOF'
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MultiServices.Maui.ViewModels.Common"
             x:Class="MultiServices.Maui.Views.Common.HomePage"
             x:DataType="vm:HomeViewModel"
             Shell.NavBarIsVisible="False">
    <RefreshView Command="{Binding LoadHomeDataCommand}" IsRefreshing="{Binding IsRefreshing}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="16">
                <!-- Header -->
                <Grid ColumnDefinitions="*,Auto" Margin="0,16,0,0">
                    <VerticalStackLayout>
                        <Label Text="{Binding Greeting}" Style="{StaticResource Title2}" />
                        <Label Text="Que cherchez-vous aujourd'hui ?" Style="{StaticResource BodyText}" />
                    </VerticalStackLayout>
                    <Frame Grid.Column="1" BackgroundColor="{StaticResource Primary}" CornerRadius="20"
                           Padding="10" HeightRequest="40" WidthRequest="40" HasShadow="False">
                        <Label Text="🔔" FontSize="16" HorizontalOptions="Center" VerticalOptions="Center">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding GoToNotificationsCommand}" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Frame>
                </Grid>

                <!-- Quick Access Modules -->
                <Grid ColumnDefinitions="*,*,*" ColumnSpacing="12">
                    <!-- Restaurants -->
                    <Frame BackgroundColor="#FEF3C7" CornerRadius="16" Padding="16" HasShadow="False" BorderColor="Transparent">
                        <VerticalStackLayout Spacing="8" HorizontalOptions="Center">
                            <Label Text="🍔" FontSize="32" HorizontalOptions="Center" />
                            <Label Text="Restaurants" FontSize="13" FontAttributes="Bold" TextColor="#92400E" HorizontalOptions="Center" />
                        </VerticalStackLayout>
                        <Frame.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding GoToRestaurantsCommand}" />
                        </Frame.GestureRecognizers>
                    </Frame>
                    <!-- Services -->
                    <Frame Grid.Column="1" BackgroundColor="#DBEAFE" CornerRadius="16" Padding="16" HasShadow="False" BorderColor="Transparent">
                        <VerticalStackLayout Spacing="8" HorizontalOptions="Center">
                            <Label Text="🛠️" FontSize="32" HorizontalOptions="Center" />
                            <Label Text="Services" FontSize="13" FontAttributes="Bold" TextColor="#1E40AF" HorizontalOptions="Center" />
                        </VerticalStackLayout>
                        <Frame.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding GoToServicesCommand}" />
                        </Frame.GestureRecognizers>
                    </Frame>
                    <!-- Courses -->
                    <Frame Grid.Column="2" BackgroundColor="#D1FAE5" CornerRadius="16" Padding="16" HasShadow="False" BorderColor="Transparent">
                        <VerticalStackLayout Spacing="8" HorizontalOptions="Center">
                            <Label Text="🛒" FontSize="32" HorizontalOptions="Center" />
                            <Label Text="Courses" FontSize="13" FontAttributes="Bold" TextColor="#065F46" HorizontalOptions="Center" />
                        </VerticalStackLayout>
                        <Frame.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding GoToGroceryCommand}" />
                        </Frame.GestureRecognizers>
                    </Frame>
                </Grid>

                <!-- Active Orders -->
                <Frame Style="{StaticResource CardFrame}" IsVisible="{Binding ActiveOrdersCount, Converter={StaticResource NullToBool}}">
                    <HorizontalStackLayout Spacing="12">
                        <Frame BackgroundColor="{StaticResource Primary}" CornerRadius="20" Padding="8" HeightRequest="40" WidthRequest="40" HasShadow="False">
                            <Label Text="📦" FontSize="16" HorizontalOptions="Center" VerticalOptions="Center" />
                        </Frame>
                        <VerticalStackLayout VerticalOptions="Center">
                            <Label Text="Commandes en cours" FontAttributes="Bold" FontSize="14" TextColor="{StaticResource TextPrimary}" />
                            <Label Text="{Binding ActiveOrdersCount, StringFormat='{0} commande(s) active(s)'}" Style="{StaticResource Caption}" />
                        </VerticalStackLayout>
                        <Label Text="→" FontSize="20" TextColor="{StaticResource Primary}" VerticalOptions="Center" HorizontalOptions="EndAndExpand">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding GoToOrdersCommand}" />
                            </Label.GestureRecognizers>
                        </Label>
                    </HorizontalStackLayout>
                </Frame>

                <!-- Popular Restaurants -->
                <VerticalStackLayout Spacing="12">
                    <Grid ColumnDefinitions="*,Auto">
                        <Label Text="🍔 Restaurants populaires" Style="{StaticResource Title3}" />
                        <Label Grid.Column="1" Text="Voir tout →" TextColor="{StaticResource Primary}" FontSize="13" VerticalOptions="Center">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding GoToRestaurantsCommand}" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Grid>
                    <CollectionView ItemsSource="{Binding PopularRestaurants}" HeightRequest="180">
                        <CollectionView.ItemsLayout>
                            <LinearItemsLayout Orientation="Horizontal" ItemSpacing="12" />
                        </CollectionView.ItemsLayout>
                        <CollectionView.ItemTemplate>
                            <DataTemplate x:DataType="models:RestaurantListDto"
                                          xmlns:models="clr-namespace:MultiServices.Maui.Models">
                                <Frame WidthRequest="200" CornerRadius="16" Padding="0" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                                    <VerticalStackLayout>
                                        <BoxView HeightRequest="80" BackgroundColor="#FEF3C7" />
                                        <VerticalStackLayout Padding="12" Spacing="4">
                                            <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="14" LineBreakMode="TailTruncation" />
                                            <Label Text="{Binding CuisineType}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                            <HorizontalStackLayout Spacing="8">
                                                <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                                <Label Text="{Binding EstimatedDeliveryMinutes, StringFormat='{0} min'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                            </HorizontalStackLayout>
                                        </VerticalStackLayout>
                                    </VerticalStackLayout>
                                </Frame>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </VerticalStackLayout>

                <!-- Top Service Providers -->
                <VerticalStackLayout Spacing="12">
                    <Grid ColumnDefinitions="*,Auto">
                        <Label Text="🛠️ Services les mieux notés" Style="{StaticResource Title3}" />
                        <Label Grid.Column="1" Text="Voir tout →" TextColor="{StaticResource Primary}" FontSize="13" VerticalOptions="Center">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding GoToServicesCommand}" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Grid>
                    <CollectionView ItemsSource="{Binding TopProviders}" HeightRequest="120">
                        <CollectionView.ItemsLayout>
                            <LinearItemsLayout Orientation="Horizontal" ItemSpacing="12" />
                        </CollectionView.ItemsLayout>
                        <CollectionView.ItemTemplate>
                            <DataTemplate x:DataType="models:ServiceProviderListDto"
                                          xmlns:models="clr-namespace:MultiServices.Maui.Models">
                                <Frame WidthRequest="180" CornerRadius="16" Padding="12" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                                    <VerticalStackLayout Spacing="6">
                                        <Label Text="{Binding CompanyName}" FontAttributes="Bold" FontSize="14" LineBreakMode="TailTruncation" />
                                        <Label Text="{Binding Category}" FontSize="12" TextColor="{StaticResource Info}" />
                                        <HorizontalStackLayout Spacing="4">
                                            <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                            <Label Text="{Binding YearsExperience, StringFormat='{0} ans exp.'}" FontSize="12" TextColor="{StaticResource TextMuted}" />
                                        </HorizontalStackLayout>
                                    </VerticalStackLayout>
                                </Frame>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </VerticalStackLayout>

                <!-- Nearby Stores -->
                <VerticalStackLayout Spacing="12" Margin="0,0,0,20">
                    <Grid ColumnDefinitions="*,Auto">
                        <Label Text="🛒 Magasins à proximité" Style="{StaticResource Title3}" />
                        <Label Grid.Column="1" Text="Voir tout →" TextColor="{StaticResource Primary}" FontSize="13" VerticalOptions="Center">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Command="{Binding GoToGroceryCommand}" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Grid>
                    <CollectionView ItemsSource="{Binding NearbyStores}" HeightRequest="100">
                        <CollectionView.ItemsLayout>
                            <LinearItemsLayout Orientation="Horizontal" ItemSpacing="12" />
                        </CollectionView.ItemsLayout>
                        <CollectionView.ItemTemplate>
                            <DataTemplate x:DataType="models:GroceryStoreListDto"
                                          xmlns:models="clr-namespace:MultiServices.Maui.Models">
                                <Frame WidthRequest="160" CornerRadius="16" Padding="12" HasShadow="True" BorderColor="Transparent" BackgroundColor="White">
                                    <VerticalStackLayout Spacing="6">
                                        <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="14" LineBreakMode="TailTruncation" />
                                        <Label Text="{Binding Brand}" FontSize="12" TextColor="{StaticResource Success}" />
                                        <Label Text="{Binding Rating, StringFormat='★ {0:F1}'}" FontSize="12" TextColor="#F59E0B" FontAttributes="Bold" />
                                    </VerticalStackLayout>
                                </Frame>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </VerticalStackLayout>

            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
EOF

cat > "$BASE/Views/Common/HomePage.xaml.cs" << 'EOF'
using MultiServices.Maui.ViewModels.Common;
namespace MultiServices.Maui.Views.Common;
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadHomeDataCommand.ExecuteAsync(null);
    }
}
EOF

echo "✅ Home page created"
