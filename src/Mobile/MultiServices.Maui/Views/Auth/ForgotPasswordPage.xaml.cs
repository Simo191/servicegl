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
