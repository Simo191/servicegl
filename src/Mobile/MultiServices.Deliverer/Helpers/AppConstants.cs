namespace MultiServices.Deliverer.Helpers;
public static class AppConstants
{
    public const string ApiBaseUrl = "https://api.multiservices.ma/api/v1";
    public const string LoginEndpoint = "/auth/login";
    public const string RegisterEndpoint = "/auth/register";
    public const string RefreshTokenEndpoint = "/auth/refresh";
    public const string DelivererProfileEndpoint = "/deliverer/profile";
    public const string DelivererRegisterEndpoint = "/deliverer/register";
    public const string DelivererDocumentsEndpoint = "/deliverer/documents";
    public const string DelivererTrainingEndpoint = "/deliverer/training";
    public const string DelivererQuizEndpoint = "/deliverer/quiz";
    public const string DelivererStatusEndpoint = "/deliverer/status";
    public const string DelivererLocationEndpoint = "/deliverer/location";
    public const string AvailableDeliveriesEndpoint = "/deliveries/available";
    public const string ActiveDeliveriesEndpoint = "/deliveries/active";
    public const string DeliveryHistoryEndpoint = "/deliveries/history";
    public const string DeliveryDetailEndpoint = "/deliveries/{0}";
    public const string AcceptDeliveryEndpoint = "/deliveries/{0}/accept";
    public const string RejectDeliveryEndpoint = "/deliveries/{0}/reject";
    public const string UpdateDeliveryStatusEndpoint = "/deliveries/{0}/status";
    public const string DeliveryProofEndpoint = "/deliveries/{0}/proof";
    public const string ReportProblemEndpoint = "/deliveries/{0}/report";
    public const string EarningsTodayEndpoint = "/deliverer/earnings/today";
    public const string EarningsWeekEndpoint = "/deliverer/earnings/week";
    public const string EarningsMonthEndpoint = "/deliverer/earnings/month";
    public const string EarningsHistoryEndpoint = "/deliverer/earnings/history";
    public const string PayoutRequestEndpoint = "/deliverer/payouts";
    public const string PayoutHistoryEndpoint = "/deliverer/payouts/history";
    public const string BonusesEndpoint = "/deliverer/bonuses";
    public const string DelivererStatsEndpoint = "/deliverer/stats";
    public const string SupportTicketEndpoint = "/support/tickets";
    public const string FaqEndpoint = "/support/faq";
    public const string SosEndpoint = "/deliverer/sos";
    public const string TokenKey = "auth_token";
    public const string RefreshTokenKey = "refresh_token";
    public const string UserIdKey = "user_id";
    public const string DelivererIdKey = "deliverer_id";
    public const string OnboardingCompletedKey = "onboarding_completed";
    public const string IsVerifiedKey = "is_verified";
    public const int LocationUpdateIntervalSeconds = 15;
    // ── Additional Endpoints ──
    public const string DelivererVehicleEndpoint = "/deliverer/vehicle";
    public const string EmergencyContactEndpoint = "/deliverer/emergency-contact";
    public const string DelivererStatsEndpoint = "/deliverer/stats";
    public const string ReportProblemEndpoint = "/deliveries/{0}/report";
    public const string SupportTicketsEndpoint = "/support/tickets";
    public const string FaqEndpoint = "/support/faq";
    public const string ActiveBonusesEndpoint = "/deliverer/bonuses";
    public const string EarningsSummaryEndpoint = "/deliverer/earnings/summary";
    public const string EarningsHistoryEndpoint = "/deliverer/earnings/history";
    public const string AvailableDeliveriesEndpoint = "/deliveries/available";
    public const string DelivererStatusEndpoint = "/deliverer/status";
    public const string DelivererLocationEndpoint = "/deliverer/location";
    public const string AcceptDeliveryEndpoint = "/deliveries/{0}/accept";
    public const string RejectDeliveryEndpoint = "/deliveries/{0}/reject";
    public const string SupportPhone = "+212522000000";

}
