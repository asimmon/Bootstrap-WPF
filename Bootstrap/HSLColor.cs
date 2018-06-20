using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using Xunit;

namespace Bootstrap
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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
                new[] { hsl.Hue, hsl.Saturation, hsl.Luminance, hsl.Alpha }
            );
        }

        private static void AssertRgbaToHslaToRgba(string hex)
        {
            var expected = (Color)ColorConverter.ConvertFromString(hex);
            var hsl = new HslColor(expected);
            var actual = hsl.Rgba;

            Assert.Equal(expected, actual);
        }

        private static void AssertDarken(string hexFrom, double percent, string hexTo)
        {
            var from = (Color)ColorConverter.ConvertFromString(hexFrom);
            var to = (Color)ColorConverter.ConvertFromString(hexTo);

            var hsl = new HslColor(from);
            var darken = hsl.Darken(percent);
            var actual = darken.Rgba;

            Assert.Equal(to, actual);
        }

        private static void AssertLighten(string hexFrom, double percent, string hexTo)
        {
            var from = (Color)ColorConverter.ConvertFromString(hexFrom);
            var to = (Color)ColorConverter.ConvertFromString(hexTo);

            var hsl = new HslColor(from);
            var darken = hsl.Lighten(percent);
            var actual = darken.Rgba;

            Assert.Equal(to, actual);
        }
    }

    [DebuggerDisplay("H:{Hue} S:{Saturation} L:{Luminance} A:{Alpha}")]
    public class HslColor
    {
        private const double Epsilon = 1E-7;

        private readonly double[] _hsl;
        private readonly double _alpha;

        public HslColor(Color rgb)
        {
            _hsl = FromRgb(rgb);
            _alpha = rgb.A / 255.0;
        }

        public HslColor(double h, double s, double l, double a)
        {
            _hsl = new[] { h, s, l };
            _alpha = a;
        }

        public HslColor(double h, double s, double l)
            : this(h, s, l, 1)
        { }

        public int Hue
        {
            get { return (int)Math.Round(_hsl[0], 0); }
        }

        public int Saturation
        {
            get { return (int)Math.Round(_hsl[1], 0); }
        }

        public int Luminance
        {
            get { return (int)Math.Round(_hsl[2], 0); }
        }

        public int Alpha
        {
            get { return (int)(_alpha * 100); }
        }

        public Color Rgba
        {
            get { return ToRgb(_hsl, _alpha); }
        }

        public static double[] FromRgb(Color color)
        {
            //  Get RGB values in the range 0 - 1

            double[] rgb = { color.R / 255.0, color.G / 255.0, color.B / 255.0 };
            double r = rgb[0];
            double g = rgb[1];
            double b = rgb[2];

            //	Minimum and Maximum RGB values are used in the HSL calculations

            double min = Math.Min(r, Math.Min(g, b));
            double max = Math.Max(r, Math.Max(g, b));

            //  Calculate the Hue

            double h = 0;

            if (Math.Abs(max - min) < Epsilon)
                h = 0;
            else if (Math.Abs(max - r) < Epsilon)
                h = ((60 * (g - b) / (max - min)) + 360) % 360;
            else if (Math.Abs(max - g) < Epsilon)
                h = (60 * (b - r) / (max - min)) + 120;
            else if (Math.Abs(max - b) < Epsilon)
                h = (60 * (r - g) / (max - min)) + 240;

            //  Calculate the Luminance

            double l = (max + min) / 2;

            //  Calculate the Saturation

            double s;

            if (Math.Abs(max - min) < Epsilon)
                s = 0;
            else if (l <= .5)
                s = (max - min) / (max + min);
            else
                s = (max - min) / (2 - max - min);

            return new[] { h, s * 100, l * 100 };
        }

        public HslColor Darken(double percent)
        {
            _hsl[2] -= percent;
            _hsl[2] = Math.Min(100, Math.Max(0, _hsl[2]));

            return this;
        }

        public HslColor Lighten(double percent)
        {
            _hsl[2] += percent;
            _hsl[2] = Math.Min(100, Math.Max(0, _hsl[2]));

            return this;
        }

        public static Color ToRgb(double[] hsl, double alpha)
        {
            return ToRgb(hsl[0], hsl[1], hsl[2], alpha);
        }

        public static Color ToRgb(double h, double s, double l, double alpha)
        {
            if (s < 0.0 || s > 100.0)
                throw new ArgumentOutOfRangeException(nameof(s));

            if (l < 0.0 || l > 100.0)
                throw new ArgumentOutOfRangeException(nameof(l));

            if (alpha < 0.0 || alpha > 1.0)
                throw new ArgumentOutOfRangeException(nameof(alpha));

            //  Formula needs all values between 0 - 1.

            h = h % 360.0;
            h /= 360.0;
            s /= 100.0;
            l /= 100.0;

            double q;

            if (l < 0.5)
                q = l * (1 + s);
            else
                q = (l + s) - (s * l);

            double p = 2 * l - q;

            double r = Math.Max(0, HueToRgb(p, q, h + (1.0 / 3.0)));
            double g = Math.Max(0, HueToRgb(p, q, h));
            double b = Math.Max(0, HueToRgb(p, q, h - (1.0 / 3.0)));

            r = Math.Min(r, 1.0);
            g = Math.Min(g, 1.0);
            b = Math.Min(b, 1.0);

            // return Color.FromScRgb(alpha, r, g, b);
            return Color.FromArgb(
                (byte)Math.Round(alpha * byte.MaxValue, 0),
                (byte)Math.Round(r * byte.MaxValue, 0),
                (byte)Math.Round(g * byte.MaxValue, 0),
                (byte)Math.Round(b * byte.MaxValue, 0)
            );
        }

        private static double HueToRgb(double p, double q, double h)
        {
            if (h < 0) h += 1;
            if (h > 1) h -= 1;

            if (6 * h < 1)
                return p + ((q - p) * 6 * h);

            if (2 * h < 1)
                return q;

            if (3 * h < 2)
                return p + ((q - p) * 6 * ((2.0 / 3.0) - h));

            return p;
        }
    }
}
