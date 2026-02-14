using MultiServices.Deliverer.Views;
using MultiServices.Deliverer.Views.Onboarding;

namespace MultiServices.Deliverer;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ── Onboarding Routes ──
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("vehicleSelection", typeof(VehicleSelectionPage));
        Routing.RegisterRoute("documentUpload", typeof(DocumentUploadPage));
        Routing.RegisterRoute("training", typeof(TrainingPage));
        Routing.RegisterRoute("trainingQuiz", typeof(TrainingQuizPage));
        Routing.RegisterRoute("pendingVerification", typeof(PendingVerificationPage));

        // ── Delivery Routes ──
        Routing.RegisterRoute("activeDelivery", typeof(ActiveDeliveryPage));
        Routing.RegisterRoute("deliveryDetail", typeof(DeliveryDetailPage));
        Routing.RegisterRoute("reportProblem", typeof(ReportProblemPage));

        // ── Earnings Routes ──
        Routing.RegisterRoute("earningDetail", typeof(EarningDetailPage));
        Routing.RegisterRoute("payout", typeof(PayoutPage));

        // ── Profile Routes ──
        Routing.RegisterRoute("editProfile", typeof(EditProfilePage));
        Routing.RegisterRoute("editVehicle", typeof(EditVehiclePage));
        Routing.RegisterRoute("emergencyContact", typeof(EmergencyContactPage));
        Routing.RegisterRoute("stats", typeof(StatsPage));
        Routing.RegisterRoute("support", typeof(SupportPage));
        Routing.RegisterRoute("sos", typeof(SOSPage));
    }
}
