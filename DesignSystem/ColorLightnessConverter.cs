using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DesignSystem
{
    public class ColorLightnessConverter : IValueConverter
    {
        public ColorLightnessVariation LightnessVariation { get; set; }

        public ColorLightnessVariation InvertedLightnessVariation
        {
            get => LightnessVariation == ColorLightnessVariation.Lighten
                ? ColorLightnessVariation.Darken
                : ColorLightnessVariation.Lighten;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = TryCastToColor(value);
            var percentage = TryCastToPercentage(parameter);

            return ApplyLightnessVariation(color, percentage, LightnessVariation);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = TryCastToColor(value);
            var percentage = TryCastToPercentage(parameter);

            return ApplyLightnessVariation(color, percentage, InvertedLightnessVariation);
        }

        private Color TryCastToColor(object potentialColor)
        {
            if (potentialColor is Color)
                return (Color)potentialColor;

            throw new ArgumentException(nameof(potentialColor));
        }

        private double TryCastToPercentage(object potentialPercentage)
        {
            double percentage = -1.0;
            if (potentialPercentage is double)
                percentage = (double)potentialPercentage;
            else if (potentialPercentage is float)
                percentage = (float)potentialPercentage;
            else if (potentialPercentage is int)
                percentage = (int)potentialPercentage;

            if (percentage >= 0 && percentage <= 100)
                return percentage;

            throw new ArgumentException(nameof(potentialPercentage));
        }

        private static Color ApplyLightnessVariation(Color color, double percentage, ColorLightnessVariation variation)
        {
            var hsl = new HslColor(color);
            return variation == ColorLightnessVariation.Lighten
                ? hsl.Lighten(percentage).Rgba
                : hsl.Darken(percentage).Rgba;
        }

        public enum ColorLightnessVariation
        {
            Lighten,
            Darken
        }
    }
}