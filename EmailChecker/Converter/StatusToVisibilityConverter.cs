using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using EmailChecker.Model;

namespace EmailChecker.Converter
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MessageStatus status = (MessageStatus)value;
            switch (status)
            {
                case MessageStatus.Comapred:
                    return Visibility.Collapsed;
                    break;
                case MessageStatus.Acknowledged:
                    return Visibility.Collapsed;
                    break;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
