using System;
using System.Globalization;
using System.Windows.Data;

namespace AdminPanel
{
    public class SelectedItemToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "#CDD6F4"; // Default foreground color
            }
            else
            {
                return "Black"; // Foreground color when an item is selected
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}