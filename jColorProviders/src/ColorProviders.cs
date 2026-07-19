using System.Drawing;
using System.Text.RegularExpressions;

namespace jColorProviders
{
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
}
