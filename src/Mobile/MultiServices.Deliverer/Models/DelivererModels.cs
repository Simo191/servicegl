namespace MultiServices.Deliverer.Models;

// Auth
public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
public class LoginResponse { public string Token { get; set; } = ""; public string RefreshToken { get; set; } = ""; public string UserId { get; set; } = ""; public string Role { get; set; } = ""; }
public class RegisterRequest { public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Password { get; set; } = ""; public string City { get; set; } = ""; }

// Profile (aligned with Deliverer entity)
public class DelivererProfile
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public string VehicleType { get; set; } = "Scooter";
    public string? VehiclePlateNumber { get; set; }
    public string? VehicleModel { get; set; }
    public string Status { get; set; } = "Offline";
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public string VerificationStatus { get; set; } = "Pending";
    public bool HasTrainingCompleted { get; set; }
    public double AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public decimal TotalEarnings { get; set; }
    public double AcceptanceRate { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public decimal DeliveryBaseFee { get; set; }
    public decimal PerKmFee { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}

// Documents
public class DocumentStatus { public string DocumentType { get; set; } = ""; public string Status { get; set; } = "NotUploaded"; public string? Url { get; set; }; public string? RejectionReason { get; set; }; }

// Training
public class TrainingVideo { public int Id { get; set; } public string Title { get; set; } = ""; public string Description { get; set; } = ""; public string VideoUrl { get; set; } = ""; public int DurationSeconds { get; set; } public bool IsWatched { get; set; } }
public class QuizQuestion { public int Id { get; set; } public string Question { get; set; } = ""; public List<string> Options { get; set; } = new(); public int CorrectOptionIndex { get; set; } public int? SelectedOptionIndex { get; set; } }
public class QuizResult { public int TotalQuestions { get; set; } public int CorrectAnswers { get; set; } public bool Passed { get; set; } public int PassingScore { get; set; } }

// Deliveries (aligned with AvailableDeliveryDto)
public class AvailableDelivery
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string PickupAddress { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public decimal EstimatedEarning { get; set; }
    public double DistanceKm { get; set; }
    public string? RestaurantName { get; set; }
    public string? StoreName { get; set; }
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public double DeliveryLat { get; set; }
    public double DeliveryLng { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DisplayName => RestaurantName ?? StoreName ?? "Service";
    public string DistanceDisplay => $"{DistanceKm:F1} km";
    public string EarningDisplay => $"{EstimatedEarning:F2} MAD";
    public string TypeIcon => Type switch { "Restaurant" => "🍔", "Grocery" => "🛒", _ => "🛠️" };
}

public class ActiveDeliveryInfo
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public string PickupAddress { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string? ClientName { get; set; }
    public string? ClientPhone { get; set; }
    public string? PickupName { get; set; }
    public string? PickupPhone { get; set; }
    public decimal EstimatedEarning { get; set; }
    public double DistanceKm { get; set; }
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public double DeliveryLat { get; set; }
    public double DeliveryLng { get; set; }
    public string? Notes { get; set; }
    public List<DeliveryItem> Items { get; set; } = new();
    public string StatusDisplay => Status switch { "ArrivedAtPickup" => "Arrivé au point de retrait", "PickedUp" => "Commande récupérée", "ArrivedAtCustomer" => "Arrivé chez le client", "Delivered" => "Livrée", _ => Status };
    public int StatusStep => Status switch { "ArrivedAtPickup" => 1, "PickedUp" => 2, "ArrivedAtCustomer" => 3, "Delivered" => 4, _ => 0 };
}

public class DeliveryItem { public string Name { get; set; } = ""; public int Quantity { get; set; } }

public class DeliveryHistoryItem
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string PickupName { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public decimal TotalEarning { get; set; }
    public double DistanceKm { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Status { get; set; } = "";
    public string DateDisplay => CompletedAt.ToString("dd/MM/yyyy HH:mm");
    public string EarningDisplay => $"+{TotalEarning:F2} MAD";
    public string TypeIcon => Type switch { "Restaurant" => "🍔", "Grocery" => "🛒", _ => "🛠️" };
}

public class UpdateDeliveryStatus { public string Status { get; set; } = ""; public string? ProofImageUrl { get; set; } }
public class ReportProblemRequest { public string Reason { get; set; } = ""; public string? Description { get; set; } public string? PhotoUrl { get; set; } }

// Earnings (aligned with DeliveryEarningDto)
public class EarningsSummary { public decimal TotalBase { get; set; } public decimal TotalDistance { get; set; } public decimal TotalTips { get; set; } public decimal TotalBonuses { get; set; } public decimal GrandTotal { get; set; } public int DeliveryCount { get; set; } }
public class EarningDetail { public Guid Id { get; set; } public string OrderType { get; set; } = ""; public Guid OrderId { get; set; } public decimal BaseFee { get; set; } public decimal DistanceFee { get; set; } public decimal TipAmount { get; set; } public decimal BonusAmount { get; set; } public decimal TotalEarning { get; set; } public DateTime Date { get; set; } public bool IsPaid { get; set; } public string DateDisplay => Date.ToString("dd/MM HH:mm"); public string TotalDisplay => $"+{TotalEarning:F2} MAD"; }
public class PayoutRequest { public decimal Amount { get; set; } public string BankAccount { get; set; } = ""; }
public class PayoutInfo { public Guid Id { get; set; } public decimal Amount { get; set; } public string Status { get; set; } = ""; public DateTime RequestedAt { get; set; } public string AmountDisplay => $"{Amount:F2} MAD"; public string StatusDisplay => Status switch { "Pending" => "En attente", "Processing" => "En cours", "Completed" => "Effectué", _ => Status }; }
public class BonusInfo { public Guid Id { get; set; } public string Title { get; set; } = ""; public string Description { get; set; } = ""; public decimal Amount { get; set; } public DateTime ExpiresAt { get; set; } public double Progress { get; set; } }

// Stats
public class DelivererStats { public int TotalDeliveries { get; set; } public double AverageRating { get; set; } public double AcceptanceRate { get; set; } public decimal TotalEarnings { get; set; } public double TotalDistanceKm { get; set; } public int DeliveriesToday { get; set; } public int DeliveriesThisWeek { get; set; } public int DeliveriesThisMonth { get; set; } }

// Support
public class SupportTicket { public string Subject { get; set; } = ""; public string Description { get; set; } = ""; public string Category { get; set; } = ""; }
public class FaqItem { public string Question { get; set; } = ""; public string Answer { get; set; } = ""; public bool IsExpanded { get; set; } }
public class LocationUpdate { public double Latitude { get; set; } public double Longitude { get; set; } }


// ── Additional Model Classes ──

public class VehicleOption
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class ProblemReason
{
    public string Code { get; set; }
    public string Label { get; set; }
    public string Icon { get; set; }
    public bool IsSelected { get; set; }
    public ProblemReason(string code, string label, string icon)
    {
        Code = code; Label = label; Icon = icon;
    }
}

public class EarningDetail
{
    public string Id { get; set; } = string.Empty;
    public decimal BaseFee { get; set; }
    public decimal DistanceFee { get; set; }
    public decimal Tip { get; set; }
    public decimal Bonus { get; set; }
    public decimal Total => BaseFee + DistanceFee + Tip + Bonus;
    public string PickupName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public double Distance { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class DelivererStats
{
    public int TotalDeliveries { get; set; }
    public int TotalInterventions { get; set; }
    public double AverageRating { get; set; }
    public double AcceptanceRate { get; set; }
    public decimal TotalEarnings { get; set; }
    public double TotalDistanceKm { get; set; }
    public double AverageDeliveryMinutes { get; set; }
    public int DeliveriesToday { get; set; }
    public int DeliveriesWeek { get; set; }
    public int DeliveriesMonth { get; set; }
}

public class EmergencyContact
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Relationship { get; set; }
}

public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class SupportTicket
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public string StatusLabel => Status switch { "Open" => "Ouvert", "InProgress" => "En cours", "Resolved" => "Résolu", "Closed" => "Fermé", _ => Status };
    public Color StatusColor => Status switch { "Open" => Colors.Orange, "InProgress" => Colors.Blue, "Resolved" => Colors.Green, "Closed" => Colors.Grey, _ => Colors.Grey };
    public DateTime CreatedAt { get; set; }
}

public class StatusUpdateRequest { public string Status { get; set; } = string.Empty; }
public class LocationUpdate { public double Latitude { get; set; } public double Longitude { get; set; } }

public class ActiveBonus
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CurrentProgress { get; set; }
    public int Target { get; set; }
    public double ProgressPercent => Target > 0 ? (double)CurrentProgress / Target : 0;
    public bool IsCompleted => CurrentProgress >= Target;
}
