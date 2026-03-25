using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BaseLibrary.Model;

namespace IntegrationLab.Model.Converters;

public class IncidentStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IncidentStatus status )//&& targetType == typeof(string))
        {
            return status switch
            {
                IncidentStatus.InProgress => "В обработке",
                IncidentStatus.Resolved => "Завершён",
                _ => null
            };
        }
        return null;
        //throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string status )//&& targetType == typeof(IncidentStatus))
        {
            return status switch
            {
                "В обработке" => IncidentStatus.InProgress,
                "Завершён" => IncidentStatus.Resolved,
                _ => null
            };
        }
        
        return null;
        //return DependencyProperty
        //if (value)
    }
}