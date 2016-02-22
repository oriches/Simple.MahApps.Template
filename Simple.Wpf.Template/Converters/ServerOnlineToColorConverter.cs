namespace Simple.Wpf.Template.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public sealed class ServerOnlineToColorConverter : IValueConverter
    {
        public SolidColorBrush Online { get; set; }

        public SolidColorBrush Offline { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return Offline;
                }

                var isOnline = (bool) value;
                return isOnline ? Online : Offline;
            }
            catch (Exception)
            {
                return Offline;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
