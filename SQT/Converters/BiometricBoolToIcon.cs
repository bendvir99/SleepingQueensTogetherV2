using SleepingQueensTogether.Models;
using System.Globalization;

namespace SleepingQueensTogether.Converters
{
    internal class BiometricBoolToIcon : IValueConverter
    {
        #region Public Methods
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
        #endregion
    }
}
