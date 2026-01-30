using SleepingQueensTogether.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepingQueensTogether.Converters
{
    internal class BiometricBoolToIcon : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string icon = Icons.Fingerprint_off;
            if (value != null)
                icon = (bool)value ? Icons.Fingerprint : Icons.Fingerprint_off;
            return icon;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
