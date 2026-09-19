using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicIsland.UI.Converters;

public class PlayPauseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isPlaying && isPlaying)
        {
            return "⏸"; // Pause symbol
        }
        return "▶"; // Play symbol
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
