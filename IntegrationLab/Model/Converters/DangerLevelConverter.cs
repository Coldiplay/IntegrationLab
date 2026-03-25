using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BaseLibrary.Model;

namespace IntegrationLab.Model.Converters;

public class DangerLevelConverter : IValueConverter
{
    private const string SpecificString = "Уровень опасности: ";
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DangerLevel dangerLevel) return null;
        
        var stringValue = dangerLevel switch
        {
            DangerLevel.Low => "Малый",
            DangerLevel.Medium => "Средний",
            DangerLevel.High => "Высокий",
            DangerLevel.VeryHigh => "Чрезвычайный",
            _ => null
        }; 
            
        if ((parameter is true 
             || parameter is string param 
             && param.Equals("true", StringComparison.OrdinalIgnoreCase)) 
            && stringValue is not null)
        {
            stringValue = SpecificString + stringValue;
        }

        return stringValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string dangerLevelString) return null;
        
        if (dangerLevelString.Contains(SpecificString))
            dangerLevelString = dangerLevelString.Replace(SpecificString, "");
        
        return dangerLevelString switch
        {
            "Малый" => DangerLevel.Low,
            "Средний" => DangerLevel.Medium,
            "Высокий" => DangerLevel.High,
            "Чрезвычайный" => DangerLevel.VeryHigh,
            _ => null
        };

    }
}