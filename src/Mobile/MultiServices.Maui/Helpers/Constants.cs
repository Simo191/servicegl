namespace MultiServices.Maui.Helpers;

public static class AppConstants
{
    public const string ApiBaseUrl = "https://api.multiservices.ma/api/v1";
    public const string SignalRHubUrl = "https://api.multiservices.ma/hubs/tracking";

    public static class Colors
    {
        public const string Primary = "#6366F1";
        public const string Secondary = "#8B5CF6";
        public const string Success = "#10B981";
        public const string Warning = "#F59E0B";
        public const string Danger = "#EF4444";
        public const string Info = "#3B82F6";
        public const string Dark = "#1F2937";
        public const string Light = "#F9FAFB";
        public const string Restaurant = "#F59E0B";
        public const string Service = "#3B82F6";
        public const string Grocery = "#10B981";
    }

    public static class CuisineTypes
    {
        public static readonly string[] All = { "Marocain", "Italien", "Asiatique", "Burger", "Pizza", "Tacos", "Sushi", "Indien", "Turc", "Français" };
    }

    public static class ServiceCategories
    {
        public static readonly (string Key, string Label, string Icon)[] All =
        {
            ("Plomberie", "Plomberie", "🔧"),
            ("Electricite", "Électricité", "⚡"),
            ("Menage", "Ménage", "🧹"),
            ("Peinture", "Peinture", "🎨"),
            ("Jardinage", "Jardinage", "🌱"),
            ("Climatisation", "Climatisation", "❄️"),
            ("Demenagement", "Déménagement", "📦"),
            ("Reparation", "Réparation", "🔨")
        };
    }

    public static class GroceryBrands
    {
        public static readonly string[] All = { "Marjane", "Carrefour", "Aswak Assalam", "Acima", "Label'Vie" };
    }
}
