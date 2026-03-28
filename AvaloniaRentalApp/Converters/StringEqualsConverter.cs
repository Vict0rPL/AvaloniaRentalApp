using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaRentalApp.Converters
{
    /// <summary>
    /// Konwertuje nazwę sekcji na bool (czy dana sekcja jest aktywna).
    /// Użycie: Classes.nav-active="{Binding CurrentSection, Converter={StaticResource SectionEquals}, ConverterParameter=Dashboard}"
    /// </summary>
    public class StringEqualsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
