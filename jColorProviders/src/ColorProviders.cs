using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace jColorProviders
{
    // Stores corrolations between sources and the color for that source
    public class LogPattern
    {
        public Regex regex { get; }
        public Color? color { get; }

        public LogPattern(Regex pattern, Color color)
        {
            regex = pattern;
            this.color = color;
        }
    }

    public interface IColorProvider<T>
    {
        Color GetColor(T source);
    }

    // Direct Value Providers
    public class RegexColorProvider : IColorProvider<string>
    {
        private readonly List<LogPattern> _patterns = new();

        public void AddPattern(Regex pattern, Color color)
        {
            _patterns.Add(new LogPattern(pattern, color));
        }
        public void RemovePattern(Regex pattern)
        {
            _patterns.RemoveAll(p => p.regex.ToString() == pattern.ToString());
        }

        public Color GetColor(string source)
        {
            foreach (var pattern in _patterns)
            {
                if (pattern.regex.IsMatch(source))
                    return pattern.color ?? Color.Black;
            }

            return Color.Black;
        }
    }
    public class FixedIndexColorProvider : IColorProvider<int>
    {
        List<Color> _colors = new List<Color> { Color.Green,
                                                Color.Blue,
                                                Color.Red,
                                                Color.Magenta,
                                                Color.Cyan,
                                                Color.Yellow };
        public Color GetColor(int index)
        {
            return _colors[index % _colors.Count];
        }
    }
    public class IndexedColorProvider : IColorProvider<int>
    {
        private readonly List<Color> _colors = new();

        public void AddColor(Color color)
        {
            _colors.Add(color);
        }

        public void AddColors(params Color[] colors)
        {
            _colors.AddRange(colors);
        }

        public void Clear()
        {
            _colors.Clear();
        }

        public int Count => _colors.Count;

        public Color GetColor(int index)
        {
            if (_colors.Count == 0)
                throw new InvalidOperationException(
                    "No colors have been added to the provider.");

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _colors[index % _colors.Count];
        }

        public int GetIndexOfColor(Color color, int startingIndex = 0)
        {
            return _colors.IndexOf(color, startingIndex);
        }
        public int CountOfColor(Color color)
        {
            int count = 0;

            foreach (Color c in _colors)
            {
                if (c.Equals(color))
                    count++;
            }

            return count;
        }
    }
    public class GeneratedColorProvider : IColorProvider<int>
    {
        private const double GoldenRatio = 0.618033988749895;

        private readonly double _saturation;
        private readonly double _value;

        public GeneratedColorProvider(double saturation = 0.75, double value = 0.95)
        {
            _saturation = saturation;
            _value = value;
        }

        public Color GetColor(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            double hue = (index * GoldenRatio) % 1.0;

            return FromHSV(hue * 360.0, _saturation, _value);
        }


        private static Color FromHSV(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;

            double f = hue / 60 - Math.Floor(hue / 60);

            double v = value * 255;
            double p = v * (1 - saturation);
            double q = v * (1 - f * saturation);
            double t = v * (1 - (1 - f) * saturation);

            return hi switch
            {
                0 => Color.FromArgb(255, (int)v, (int)t, (int)p),
                1 => Color.FromArgb(255, (int)q, (int)v, (int)p),
                2 => Color.FromArgb(255, (int)p, (int)v, (int)t),
                3 => Color.FromArgb(255, (int)p, (int)q, (int)v),
                4 => Color.FromArgb(255, (int)t, (int)p, (int)v),
                _ => Color.FromArgb(255, (int)v, (int)p, (int)q)
            };
        }
    }
    public class FixedHashColorProvider : IColorProvider<string>
    {
        private const double GoldenRatio = 0.618033988749895;

        private readonly double _saturation;
        private readonly double _value;

        public FixedHashColorProvider(double saturation = 0.70, double value = 0.85)
        {
            if (saturation < 0.0 || saturation > 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(saturation),
                    saturation,
                    "Saturation must be between 0.0 and 1.0.");
            }

            if (value < 0.0 || value > 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Value must be between 0.0 and 1.0.");
            }

            _saturation = saturation;
            _value = value;
        }


        public Color GetColor(string source)
        {
            if (string.IsNullOrEmpty(source))
                return Color.Gray;

            ulong hash = HashString(source);

            // Convert hash into a stable hue
            double hue = ((hash % 1000000) / 1000000.0);

            // Spread similar hashes apart
            hue = (hue + GoldenRatio * (hash % 97)) % 1.0;

            return FromHSV(
                hue * 360.0,
                _saturation,
                _value);
        }


        private static ulong HashString(string input)
        {
            using SHA256 sha = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);

            return BitConverter.ToUInt64(hash, 0);
        }


        private static Color FromHSV(
            double hue,
            double saturation,
            double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;

            double f = hue / 60 - Math.Floor(hue / 60);

            double v = value * 255;
            double p = v * (1 - saturation);
            double q = v * (1 - f * saturation);
            double t = v * (1 - (1 - f) * saturation);

            return hi switch
            {
                0 => Color.FromArgb(255, (int)v, (int)t, (int)p),
                1 => Color.FromArgb(255, (int)q, (int)v, (int)p),
                2 => Color.FromArgb(255, (int)p, (int)v, (int)t),
                3 => Color.FromArgb(255, (int)p, (int)q, (int)v),
                4 => Color.FromArgb(255, (int)t, (int)p, (int)v),
                _ => Color.FromArgb(255, (int)v, (int)p, (int)q)
            };
        }
    }

    // Color-Modifying providers
    public class TextColorProvider : IColorProvider<Color>
    {
        public Color GetColor(Color background)
        {
            double luminance = GetRelativeLuminance(background);

            return luminance > 0.179
                ? Color.Black
                : Color.White;
        }

        protected static double GetRelativeLuminance(Color color)
        {
            double r = Linearize(color.R / 255.0);
            double g = Linearize(color.G / 255.0);
            double b = Linearize(color.B / 255.0);

            return
                0.2126 * r +
                0.7152 * g +
                0.0722 * b;
        }

        private static double Linearize(double value)
        {
            return value <= 0.03928
                ? value / 12.92
                : Math.Pow((value + 0.055) / 1.055, 2.4);
        }
    }
    public class MostVisibleColorProvider : TextColorProvider
    {
        public new Color GetColor(Color background)
        {
            double blackContrast = ContrastRatio(background, Color.Black);
            double whiteContrast = ContrastRatio(background, Color.White);

            return blackContrast >= whiteContrast
                ? Color.Black
                : Color.White;
        }

        private static double ContrastRatio(Color a, Color b)
        {
            double l1 = GetRelativeLuminance(a);
            double l2 = GetRelativeLuminance(b);

            double lighter = Math.Max(l1, l2);
            double darker = Math.Min(l1, l2);

            return (lighter + 0.05) / (darker + 0.05);
        }
    }
}
