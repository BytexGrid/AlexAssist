using System;
using System.Globalization;
using System.Windows.Data;

namespace AlexAssist.UI.Converters
{
    public class BoolToTaskEditorTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Update Task" : "New Task";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 