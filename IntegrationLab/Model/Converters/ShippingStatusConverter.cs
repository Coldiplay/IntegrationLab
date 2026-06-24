using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BaseLibrary.Model.Enums;

namespace IntegrationLab.Model.Converters;

public class ShippingStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ShippingStatus status) //&& targetType == typeof(string))
            return status switch
            {
                ShippingStatus.InProcessing => "В обработке",
                ShippingStatus.ReadyToShip => "Готов к отправке",
                ShippingStatus.Shipping => "В работе",
                ShippingStatus.Delivered =>"Завершён",
                ShippingStatus.Incident => "Происшествие",
                _ => null
            };

        return null;
        //throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string status) //&& targetType == typeof(IncidentStatus))
            return status switch
            {
                "В обработке" => ShippingStatus.InProcessing,
                "Готов к отправке" => ShippingStatus.ReadyToShip,
                "В работе" => ShippingStatus.Shipping,
                "Завершён" => ShippingStatus.Delivered,
                "Происшествие" => ShippingStatus.Incident,
                _ => null
            };

        return null;
    }
}