using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Simple.Wpf.Template.Converters
{
    public sealed class ServerOnlineToColorConverter : IValueConverter
    {
        public SolidColorBrush Online { get; set; }

        public SolidColorBrush Offline { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return Offline;

                var isOnline = (bool)value;
                return isOnline ? Online : Offline;
            }
            catch (Exception)
            {
                return Offline;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
}