using System.Globalization;
using System.Windows.Media;
using Xunit;

namespace DesignSystem.Tests
{
    public class ColorLightnessConverterTests
    {
        [Fact]
        public void ConvertLighten()
        {
            var from = (Color)ColorConverter.ConvertFromString("#337AB7");
            var expected = (Color)ColorConverter.ConvertFromString("#4F93CE");
            var converter = new ColorLightnessConverter
            {
                LightnessVariation = ColorLightnessConverter.ColorLightnessVariation.Lighten
            };

            Assert.Equal(expected, converter.Convert(from, typeof(Color), 10.0, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void ConvertDarken()
        {
            var from = (Color)ColorConverter.ConvertFromString("#428BCA");
            var expected = (Color)ColorConverter.ConvertFromString("#3071A9");
            var converter = new ColorLightnessConverter
            {
                LightnessVariation = ColorLightnessConverter.ColorLightnessVariation.Darken
            };

            Assert.Equal(expected, converter.Convert(from, typeof(Color), 10.0, CultureInfo.CurrentCulture));
        }
    }
}
