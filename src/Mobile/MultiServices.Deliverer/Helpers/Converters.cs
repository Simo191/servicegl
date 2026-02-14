using System.Globalization;

namespace MultiServices.Deliverer.Helpers;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is true ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#F5F5F5");
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => false;
}

public class BoolToCheckConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is true ? "✅" : "📎";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => false;
}

public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) => value != null;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => false;
}

public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) => value is bool b && !b;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => value is bool b && !b;
}

public class PercentToDoubleConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is double d ? d / 100.0 : 0.0;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => 0.0;
}

public class BoolToEmojiConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is true ? "🎉" : "😔";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => false;
}

public class BoolToResultConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is true ? "Bravo ! Quiz réussi !" : "Quiz échoué...";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => "";
}

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        !string.IsNullOrEmpty(value as string);
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => "";
}

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is int i && i > 0;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => 0;
}

public class BoolToOnlineTextConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value is true ? "Activez pour recevoir des commandes" : "Passez en ligne pour commencer";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => false;
}