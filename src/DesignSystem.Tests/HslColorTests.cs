using System.Windows.Media;
using Xunit;

namespace DesignSystem.Tests
{
    public class HslColorTests
    {
        [Fact]
        public void RgbaToHsla()
        {
            AssertRgbaToHsla("#FF69ECC7", h: 163, s: 78, l: 67, a: 100);
            AssertRgbaToHsla("#DC69ECC7", h: 163, s: 78, l: 67, a: 86);
        }

        [Fact]
        public void RgbaToHslaThenBackToRgba()
        {
            AssertRgbaToHslaToRgba("#FF69ECC7");
        }

        [Fact]
        public void Darken()
        {
            AssertDarken("#FF80E619", 20.0, "#FF4D8A0F");

            AssertDarken("#428BCA", 6.5, "#337AB7");
        }

        [Fact]
        public void Lighten()
        {
            AssertLighten("#000000", 13.5, "#222222");
            AssertLighten("#000000", 20.0, "#333333");
            AssertLighten("#000000", 33.5, "#555555");
            AssertLighten("#000000", 46.7, "#777777");
            AssertLighten("#000000", 93.5, "#EEEEEE");

            AssertLighten("#FF80E619", 20.0, "#FFB3F075");
        }

        private static void AssertRgbaToHsla(string hex, int h, int s, int l, int a)
        {
            var rgb = (Color)ColorConverter.ConvertFromString(hex);
            var hsl = new HslColor(rgb);
            Assert.Equal(
                new[] { h, s, l, a },
                new[] { hsl.Hue, hsl.Saturation, hsl.Lightness, hsl.Alpha }
            );
        }

        private static void AssertRgbaToHslaToRgba(string hex)
        {
            var expected = (Color)ColorConverter.ConvertFromString(hex);
            var hsl = new HslColor(expected);
            var actual = hsl.Rgba;

            Assert.Equal(expected, actual);
        }

        private static void AssertDarken(string hexFrom, double percentage, string hexTo)
        {
            var from = (Color)ColorConverter.ConvertFromString(hexFrom);
            var to = (Color)ColorConverter.ConvertFromString(hexTo);

            var hsl = new HslColor(from);
            var darker = hsl.Darken(percentage);
            var actual = darker.Rgba;

            Assert.Equal(to, actual);
        }

        private static void AssertLighten(string hexFrom, double percentage, string hexTo)
        {
            var from = (Color)ColorConverter.ConvertFromString(hexFrom);
            var to = (Color)ColorConverter.ConvertFromString(hexTo);

            var hsl = new HslColor(from);
            var lighter = hsl.Lighten(percentage);
            var actual = lighter.Rgba;

            Assert.Equal(to, actual);
        }
    }
}
