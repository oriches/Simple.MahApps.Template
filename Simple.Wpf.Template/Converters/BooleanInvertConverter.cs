namespace Simple.Wpf.Template.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return false;
                }

                var boolValue = (bool) value;
                return !boolValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}