using System.Globalization;

namespace MultiServices.Maui.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return Color.FromArgb("#10B981");
        return Color.FromArgb("#EF4444");
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Pending" or "Reserved" => Color.FromArgb("#F59E0B"),
            "Confirmed" => Color.FromArgb("#3B82F6"),
            "Preparing" or "InProgress" => Color.FromArgb("#8B5CF6"),
            "Ready" => Color.FromArgb("#06B6D4"),
            "InTransit" or "EnRoute" => Color.FromArgb("#F97316"),
            "Delivered" or "Completed" => Color.FromArgb("#10B981"),
            "Cancelled" => Color.FromArgb("#EF4444"),
            "Refunded" => Color.FromArgb("#6B7280"),
            _ => Color.FromArgb("#6B7280")
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Pending" => "En attente",
            "Confirmed" => "Confirmée",
            "Preparing" => "En préparation",
            "Ready" => "Prête",
            "InTransit" or "EnRoute" => "En route",
            "OnSite" => "Sur place",
            "InProgress" => "En cours",
            "Delivered" or "Completed" => "Terminée",
            "Cancelled" => "Annulée",
            "Refunded" => "Remboursée",
            "Reserved" => "Réservée",
            _ => value?.ToString() ?? ""
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class RatingToStarsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double rating)
        {
            int full = (int)rating;
            return new string('★', full) + new string('☆', 5 - full);
        }
        return "☆☆☆☆☆";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class CurrencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal d) return $"{d:N2} DH";
        return "0.00 DH";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class DateTimeToRelativeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 1) return "À l'instant";
            if (diff.TotalMinutes < 60) return $"Il y a {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"Il y a {(int)diff.TotalHours}h";
            if (diff.TotalDays < 7) return $"Il y a {(int)diff.TotalDays}j";
            return dt.ToString("dd/MM/yyyy");
        }
        return "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class NullToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value != null;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
