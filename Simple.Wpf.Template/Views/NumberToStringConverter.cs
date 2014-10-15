namespace Simple.Wpf.Template.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return System.Convert.ToInt32(value);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
