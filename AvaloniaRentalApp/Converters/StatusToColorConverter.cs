using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaRentalApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public static readonly StatusToColorConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.Contains("Connected")) return Brushes.Green;
                if (status.Contains("Failed")) return Brushes.Red;
            }
            return Brushes.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
