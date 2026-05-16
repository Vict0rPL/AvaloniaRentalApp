using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaRentalApp.Converters
{
    public static class NumberConverters
    {
        public static readonly IValueConverter NotZero =
            new FuncValueConverter<decimal, bool>(n => n > 0);
    }
}
