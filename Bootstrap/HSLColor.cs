using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using Xunit;

namespace Bootstrap
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
            AssertDarken("#FF80E619", 20f, "#FF4D8A0F");
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void AssertRgbaToHsla(string hex, int h, int s, int l, int a)
        {
            var rgb = (Color)ColorConverter.ConvertFromString(hex);
            var hsl = new HslColor(rgb);
            Assert.Equal(
                new[] { h, s, l, a },
                new[] { hsl.Hue, hsl.Saturation, hsl.Luminance, hsl.Alpha }
            );
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void AssertRgbaToHslaToRgba(string hex)
        {
            var expected = (Color)ColorConverter.ConvertFromString(hex);
            var hsl = new HslColor(expected);
            var actual = hsl.Rgba;

            Assert.Equal(expected, actual);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void AssertDarken(string hexFrom, float percent, string hexTo)
        {
            var from = (Color)ColorConverter.ConvertFromString(hexFrom);
            var to = (Color)ColorConverter.ConvertFromString(hexTo);

            var hsl = new HslColor(from);
            var darken = hsl.Darken(percent);
            var actual = darken.Rgba;

            Assert.Equal(to, actual);
        }
    }

    [DebuggerDisplay("H:{Hue} S:{Saturation} L:{Luminance} A:{Alpha}")]
    public class HslColor
    {
        private const float FloatTolerance = 0.001f;

        private readonly float[] _hsl;
        private readonly float _alpha;

        public HslColor(Color rgb)
        {
            _hsl = FromRgb(rgb);
            _alpha = rgb.A / 255f;
        }

        public HslColor(float h, float s, float l, float a)
        {
            _hsl = new[] { h, s, l };
            _alpha = a;
        }

        public HslColor(float h, float s, float l)
            : this(h, s, l, 1f)
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

        public static float[] FromRgb(Color color)
        {
            //  Get RGB values in the range 0 - 1

            float[] rgb = { color.R / 255f, color.G / 255f, color.B / 255f };
            float r = rgb[0];
            float g = rgb[1];
            float b = rgb[2];

            //	Minimum and Maximum RGB values are used in the HSL calculations

            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));

            //  Calculate the Hue

            float h = 0;

            if (Math.Abs(max - min) < FloatTolerance)
                h = 0;
            else if (Math.Abs(max - r) < FloatTolerance)
                h = ((60 * (g - b) / (max - min)) + 360) % 360;
            else if (Math.Abs(max - g) < FloatTolerance)
                h = (60 * (b - r) / (max - min)) + 120;
            else if (Math.Abs(max - b) < FloatTolerance)
                h = (60 * (r - g) / (max - min)) + 240;

            //  Calculate the Luminance

            float l = (max + min) / 2;

            //  Calculate the Saturation

            float s = 0;

            if (Math.Abs(max - min) < FloatTolerance)
                s = 0;
            else if (l <= .5f)
                s = (max - min) / (max + min);
            else
                s = (max - min) / (2 - max - min);

            return new[]
            {
                (float)Math.Round(h, 0),
                (float)Math.Round(s * 100),
                (float)Math.Round(l * 100)
            };
        }

        public HslColor Darken(float percent)
        {
            _hsl[2] -= percent;
            _hsl[2] = Math.Min(100, Math.Max(0, _hsl[2]));

            return this;
        }

        public static Color ToRgb(float[] hsl, float alpha)
        {
            return ToRgb(hsl[0], hsl[1], hsl[2], alpha);
        }

        public static Color ToRgb(float h, float s, float l, float alpha)
        {
            if (s < 0.0f || s > 100.0f)
                throw new ArgumentOutOfRangeException(nameof(s));

            if (l < 0.0f || l > 100.0f)
                throw new ArgumentOutOfRangeException(nameof(l));

            if (alpha < 0.0f || alpha > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(alpha));

            //  Formula needs all values between 0 - 1.

            h = h % 360.0f;
            h /= 360f;
            s /= 100f;
            l /= 100f;

            float q = 0;

            if (l < 0.5)
                q = l * (1 + s);
            else
                q = (l + s) - (s * l);

            float p = 2 * l - q;

            float r = Math.Max(0, HueToRgb(p, q, h + (1.0f / 3.0f)));
            float g = Math.Max(0, HueToRgb(p, q, h));
            float b = Math.Max(0, HueToRgb(p, q, h - (1.0f / 3.0f)));

            r = Math.Min(r, 1.0f);
            g = Math.Min(g, 1.0f);
            b = Math.Min(b, 1.0f);

            // return Color.FromScRgb(alpha, r, g, b);
            return Color.FromArgb(
                (byte)(alpha * byte.MaxValue),
                (byte)(r * byte.MaxValue),
                (byte)(g * byte.MaxValue),
                (byte)(b * byte.MaxValue)
            );
        }

        private static float HueToRgb(float p, float q, float h)
        {
            if (h < 0) h += 1;
            if (h > 1) h -= 1;

            if (6 * h < 1)
                return p + ((q - p) * 6 * h);

            if (2 * h < 1)
                return q;

            if (3 * h < 2)
                return p + ((q - p) * 6 * ((2.0f / 3.0f) - h));

            return p;
        }
    }
}
