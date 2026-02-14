namespace MultiServices.Domain.Enums;

// ==================== COMMON ====================
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    PickedUp = 4,
    InTransit = 5,
    Delivered = 6,
    Completed = 7,
    Cancelled = 8,
    Refunded = 9
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    ApplePay = 2,
    GooglePay = 3,
    PayPal = 4,
    CashOnDelivery = 5,
    Wallet = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5
}

public enum UserRole
{
    Client = 0,
    RestaurantOwner = 1,
    ServiceProvider = 2,
    GroceryManager = 3,
    Deliverer = 4,
    Intervenant = 5,
    Admin = 6,
    SuperAdmin = 7
}

public enum NotificationType
{
    Push = 0,
    Email = 1,
    SMS = 2,
    System = 3
}

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}

public enum Language
{
    French = 0,
    Arabic = 1,
    English = 2
}

public enum DocumentType
{
    IdentityCard = 0,
    DrivingLicense = 1,
    ProfessionalLicense = 2,
    Insurance = 3,
    Certificate = 4,
    VehicleRegistration = 5
}

public enum VerificationStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3
}

public enum ReportReason
{
    LateDelivery = 0,
    WrongOrder = 1,
    DamagedItems = 2,
    MissingItems = 3,
    PoorQuality = 4,
    RudeDeliverer = 5,
    PoorService = 6,
    Other = 7
}

// ==================== RESTAURANT ====================
public enum CuisineType
{
    Italian = 0,
    Asian = 1,
    Burger = 2,
    Pizza = 3,
    Moroccan = 4,
    French = 5,
    Indian = 6,
    Mexican = 7,
    Sushi = 8,
    Seafood = 9,
    Vegan = 10,
    FastFood = 11,
    Desserts = 12,
    Healthy = 13,
    Other = 14
}

public enum PriceRange
{
    Budget = 1,      // €
    Moderate = 2,    // €€
    Expensive = 3    // €€€
}

public enum DishSize
{
    Small = 0,
    Medium = 1,
    Large = 2,
    ExtraLarge = 3
}

public enum RestaurantOrderStatus
{
    Received = 0,
    Preparing = 1,
    Ready = 2,
    InTransit = 3,
    Delivered = 4,
    Cancelled = 5
}

// ==================== SERVICES A DOMICILE ====================
public enum ServiceCategory
{
    Plumbing = 0,         // Plomberie
    Electricity = 1,      // Électricité
    Cleaning = 2,         // Ménage & Nettoyage
    Painting = 3,         // Peinture
    Gardening = 4,        // Jardinage
    AirConditioning = 5,  // Climatisation
    Moving = 6,           // Déménagement
    Repair = 7            // Réparation
}

public enum ServiceSubCategory
{
    // Plumbing
    LeakRepair = 0,
    Unclogging = 1,
    PipeInstallation = 2,
    // Electricity
    PowerFailure = 10,
    ElectricalInstallation = 11,
    ElectricalPanel = 12,
    // Cleaning
    RegularCleaning = 20,
    DeepCleaning = 21,
    WindowCleaning = 22,
    // Painting
    InteriorPainting = 30,
    ExteriorPainting = 31,
    Wallpaper = 32,
    // Gardening
    LawnMowing = 40,
    HedgeTrimming = 41,
    GardenMaintenance = 42,
    // AC
    ACInstallation = 50,
    ACMaintenance = 51,
    ACRepair = 52,
    // Moving
    FullMoving = 60,
    FurnitureTransport = 61,
    Packing = 62,
    // Repair
    ApplianceRepair = 70,
    FurnitureRepair = 71,
    SmallJobs = 72
}

public enum PricingType
{
    Hourly = 0,
    FixedPrice = 1,
    Quote = 2
}

public enum InterventionStatus
{
    Reserved = 0,
    Confirmed = 1,
    EnRoute = 2,
    OnSite = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6,
    Disputed = 7
}

public enum RecurrenceType
{
    None = 0,
    Weekly = 1,
    BiWeekly = 2,
    Monthly = 3
}

// ==================== COURSES EN LIGNE ====================
public enum GroceryCategory
{
    FruitsVegetables = 0,
    MeatFish = 1,
    SavoryGrocery = 2,
    SweetGrocery = 3,
    Beverages = 4,
    Dairy = 5,
    Frozen = 6,
    HygieneBeauty = 7,
    HouseholdCleaning = 8,
    BabyChild = 9
}

public enum GroceryStore
{
    Marjane = 0,
    Carrefour = 1,
    AswakAssalam = 2,
    Acima = 3,
    LabelVie = 4
}

public enum GroceryOrderStatus
{
    Received = 0,
    Preparing = 1,
    ProductUnavailable = 2,
    Ready = 3,
    InTransit = 4,
    Delivered = 5,
    Cancelled = 6
}

public enum BagType
{
    Plastic = 0,
    Reusable = 1,
    Paper = 2,
    NoBag = 3
}

public enum DeliveryTimeSlot
{
    Morning_8_10 = 0,
    Morning_10_12 = 1,
    Afternoon_12_14 = 2,
    Afternoon_14_16 = 3,
    Afternoon_16_18 = 4,
    Evening_18_20 = 5,
    Evening_20_22 = 6
}

// ==================== DELIVERER ====================
public enum DelivererStatus
{
    Offline = 0,
    Online = 1,
    Busy = 2,
    OnBreak = 3
}

public enum VehicleType
{
    Bicycle = 0,
    Scooter = 1,
    Motorcycle = 2,
    Car = 3,
    Van = 4
}
