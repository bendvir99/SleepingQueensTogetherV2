using SleepingQueensTogether.Models;
using System.Globalization;

namespace SleepingQueensTogether.Converters
{
	internal class VisibilityBoolToIcon : IValueConverter
	{
        #region Public Methods
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			string icon = Icons.Visibility_off;
            if (value != null)
				icon =  (bool)value ? Icons.Visibility_off : Icons.Visibility_on;
			return icon;
		}

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
