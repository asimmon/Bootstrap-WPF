using System;
using System.Globalization;
using System.Windows.Media;

namespace DesignSystem
{
    public class SolidColorBrushLightnessConverter : ColorLightnessConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = TryParseBrush(value);
            var newColor = Convert(brush.Color, parameter);
            return new SolidColorBrush(newColor);
        }

        private static SolidColorBrush TryParseBrush(object potentialBrush)
        {
            if (potentialBrush is SolidColorBrush)
                return (SolidColorBrush)potentialBrush;

            throw new ArgumentException(nameof(potentialBrush));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = TryParseBrush(value);
            var newColor = ConvertBack(brush.Color, parameter);
            return new SolidColorBrush(newColor);
        }
    }
}