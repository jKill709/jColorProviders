using System.Drawing;
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
}
