using System.Drawing;
using System.Text.RegularExpressions;
using Xunit;
using jColorProviders;

namespace jColorProviders.Tests;

public class RegexColorProviderTests
{
    [Fact]
    public void GetColor_NoPatterns_ReturnsBlack()
    {
        var provider = new RegexColorProvider();

        Color result = provider.GetColor("Anything");

        Assert.Equal(Color.Black, result);
    }

    [Fact]
    public void GetColor_MatchingPattern_ReturnsAssociatedColor()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("^Error$"), Color.Red);

        Color result = provider.GetColor("Error");

        Assert.Equal(Color.Red, result);
    }

    [Fact]
    public void GetColor_NonMatchingPattern_ReturnsBlack()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("^Warning$"), Color.Yellow);

        Color result = provider.GetColor("Error");

        Assert.Equal(Color.Black, result);
    }

    [Fact]
    public void GetColor_FirstMatchingPatternWins()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex(".*"), Color.Blue);
        provider.AddPattern(new Regex("^Error$"), Color.Red);

        Color result = provider.GetColor("Error");

        Assert.Equal(Color.Blue, result);
    }

    [Fact]
    public void GetColor_SecondPatternMatches_ReturnsSecondColor()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("^Warning$"), Color.Yellow);
        provider.AddPattern(new Regex("^Error$"), Color.Red);

        Color result = provider.GetColor("Error");

        Assert.Equal(Color.Red, result);
    }

    [Fact]
    public void GetColor_RegexMatchingSubstring_ReturnsColor()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("Error"), Color.Red);

        Color result = provider.GetColor("Critical Error occurred");

        Assert.Equal(Color.Red, result);
    }

    [Fact]
    public void GetColor_CaseSensitiveRegex_BehavesAsExpected()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("^Error$"), Color.Red);

        Assert.Equal(Color.Red, provider.GetColor("Error"));
        Assert.Equal(Color.Black, provider.GetColor("error"));
    }

    [Fact]
    public void GetColor_CaseInsensitiveRegex_BehavesAsExpected()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(
            new Regex("^Error$", RegexOptions.IgnoreCase),
            Color.Red);

        Assert.Equal(Color.Red, provider.GetColor("Error"));
        Assert.Equal(Color.Red, provider.GetColor("error"));
        Assert.Equal(Color.Red, provider.GetColor("ERROR"));
    }

    [Fact]
    public void GetColor_MultipleCalls_AreConsistent()
    {
        var provider = new RegexColorProvider();

        provider.AddPattern(new Regex("^Error$"), Color.Red);

        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(Color.Red, provider.GetColor("Error"));
        }
    }
}

public class FixedIndexColorProviderTests
{
    private readonly FixedIndexColorProvider _provider = new();

    [Theory]
    [InlineData(0, KnownColor.Green)]
    [InlineData(1, KnownColor.Blue)]
    [InlineData(2, KnownColor.Red)]
    [InlineData(3, KnownColor.Magenta)]
    [InlineData(4, KnownColor.Cyan)]
    [InlineData(5, KnownColor.Yellow)]
    public void GetColor_FirstCycle_ReturnsExpectedColor(
        int index,
        KnownColor expected)
    {
        Color result = _provider.GetColor(index);

        Assert.Equal(Color.FromKnownColor(expected), result);
    }

    [Theory]
    [InlineData(6, KnownColor.Green)]
    [InlineData(7, KnownColor.Blue)]
    [InlineData(8, KnownColor.Red)]
    [InlineData(9, KnownColor.Magenta)]
    [InlineData(10, KnownColor.Cyan)]
    [InlineData(11, KnownColor.Yellow)]
    [InlineData(12, KnownColor.Green)]
    public void GetColor_WrapsAroundList(
        int index,
        KnownColor expected)
    {
        Color result = _provider.GetColor(index);

        Assert.Equal(Color.FromKnownColor(expected), result);
    }

    [Fact]
    public void GetColor_LargeIndex_WrapsCorrectly()
    {
        Color result = _provider.GetColor(1000);

        Assert.Equal(Color.Cyan, result); // 1000 % 6 == 4
    }

    [Fact]
    public void GetColor_NegativeIndex_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _provider.GetColor(-1));
    }
}

public class LogPatternTests
{
    [Fact]
    public void Constructor_StoresRegex()
    {
        Regex regex = new("^abc$");

        var pattern = new LogPattern(regex, Color.Red);

        Assert.Same(regex, pattern.regex);
    }

    [Fact]
    public void Constructor_StoresColor()
    {
        Regex regex = new("^abc$");

        var pattern = new LogPattern(regex, Color.Red);

        Assert.Equal(Color.Red, pattern.color);
    }
}

public class IndexedColorProviderTests
{
    #region AddColor / Count

    [Fact]
    public void AddColor_ShouldIncreaseCount()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        Assert.Equal(1, provider.Count);
    }

    [Fact]
    public void AddColor_ShouldStoreColor()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        Assert.Equal(Color.Red, provider.GetColor(0));
    }

    #endregion

    #region AddColors

    [Fact]
    public void AddColors_ShouldAddAllColors()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Blue);

        Assert.Equal(3, provider.Count);
    }

    [Fact]
    public void AddColors_ShouldPreserveOrder()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Blue);

        Assert.Equal(Color.Red, provider.GetColor(0));
        Assert.Equal(Color.Green, provider.GetColor(1));
        Assert.Equal(Color.Blue, provider.GetColor(2));
    }

    [Fact]
    public void AddColors_WithNoArguments_ShouldDoNothing()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors();

        Assert.Equal(0, provider.Count);
    }

    #endregion

    #region Clear

    [Fact]
    public void Clear_ShouldRemoveAllColors()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(Color.Red, Color.Blue);

        provider.Clear();

        Assert.Equal(0, provider.Count);
    }

    [Fact]
    public void GetColor_AfterClear_ShouldThrow()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        provider.Clear();

        Assert.Throws<InvalidOperationException>(() => provider.GetColor(0));
    }

    #endregion

    #region GetColor

    [Fact]
    public void GetColor_ShouldReturnCorrectColor()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Blue);

        Assert.Equal(Color.Green, provider.GetColor(1));
    }

    [Theory]
    [InlineData(3, KnownColor.Red)]
    [InlineData(4, KnownColor.Green)]
    [InlineData(5, KnownColor.Blue)]
    [InlineData(6, KnownColor.Red)]
    [InlineData(100, KnownColor.Green)] //100 % 3 == 1
    public void GetColor_ShouldWrapAround(
        int index,
        KnownColor expected)
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Blue);

        Assert.Equal(
            Color.FromKnownColor(expected),
            provider.GetColor(index));
    }

    [Fact]
    public void GetColor_WithNegativeIndex_ShouldThrow()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => provider.GetColor(-1));
    }

    [Fact]
    public void GetColor_WithNoColors_ShouldThrow()
    {
        var provider = new IndexedColorProvider();

        Assert.Throws<InvalidOperationException>(
            () => provider.GetColor(0));
    }

    #endregion

    #region GetIndexOfColor

    [Fact]
    public void GetIndexOfColor_ShouldReturnFirstOccurrence()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Red,
            Color.Blue);

        Assert.Equal(0, provider.GetIndexOfColor(Color.Red));
    }

    [Fact]
    public void GetIndexOfColor_WithStartingIndex_ShouldContinueSearching()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Red,
            Color.Blue);

        Assert.Equal(2,
            provider.GetIndexOfColor(Color.Red, 1));
    }

    [Fact]
    public void GetIndexOfColor_WhenColorMissing_ShouldReturnMinusOne()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green);

        Assert.Equal(-1,
            provider.GetIndexOfColor(Color.Blue));
    }

    [Fact]
    public void GetIndexOfColor_WhenStartingAtEnd_ShouldReturnMinusOne()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green);

        Assert.Equal(-1,
            provider.GetIndexOfColor(Color.Red, 2));
    }

    [Fact]
    public void GetIndexOfColor_WithNegativeStartingIndex_ShouldThrow()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => provider.GetIndexOfColor(Color.Red, -1));
    }

    [Fact]
    public void GetIndexOfColor_WithStartingIndexPastEnd_ShouldThrow()
    {
        var provider = new IndexedColorProvider();

        provider.AddColor(Color.Red);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => provider.GetIndexOfColor(Color.Red, 2));
    }

    #endregion

    #region CountOfColor

    [Fact]
    public void CountOfColor_WhenColorOccursOnce_ShouldReturnOne()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Blue);

        Assert.Equal(1,
            provider.CountOfColor(Color.Green));
    }

    [Fact]
    public void CountOfColor_WhenColorOccursMultipleTimes_ShouldReturnCount()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green,
            Color.Red,
            Color.Blue,
            Color.Red);

        Assert.Equal(3,
            provider.CountOfColor(Color.Red));
    }

    [Fact]
    public void CountOfColor_WhenColorMissing_ShouldReturnZero()
    {
        var provider = new IndexedColorProvider();

        provider.AddColors(
            Color.Red,
            Color.Green);

        Assert.Equal(0,
            provider.CountOfColor(Color.Blue));
    }

    [Fact]
    public void CountOfColor_OnEmptyProvider_ShouldReturnZero()
    {
        var provider = new IndexedColorProvider();

        Assert.Equal(0,
            provider.CountOfColor(Color.Red));
    }

    #endregion
}

public class GeneratedColorProviderTests
{
    [Fact]
    public void SameIndexAlwaysReturnsSameColor()
    {
        var provider = new GeneratedColorProvider();

        Color first = provider.GetColor(42);

        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(first, provider.GetColor(42));
        }
    }

    [Fact]
    public void DifferentInstancesWithSameParametersReturnSameColors()
    {
        var provider1 = new GeneratedColorProvider();
        var provider2 = new GeneratedColorProvider();

        for (int i = 0; i < 500; i++)
        {
            Assert.Equal(provider1.GetColor(i), provider2.GetColor(i));
        }
    }

    [Fact]
    public void DifferentConstructorParametersProduceDifferentPalette()
    {
        var provider1 = new GeneratedColorProvider(0.75, 0.95);
        var provider2 = new GeneratedColorProvider(0.15, 0.95);

        bool foundDifference = false;

        for (int i = 0; i < 50; i++)
        {
            if (provider1.GetColor(i) != provider2.GetColor(i))
            {
                foundDifference = true;
                break;
            }
        }

        Assert.True(foundDifference);
    }

    [Fact]
    public void SaturationParameterChangesPalette()
    {
        var provider1 = new GeneratedColorProvider(0.75, 0.95);
        var provider2 = new GeneratedColorProvider(0.75, 0.40);

        bool foundDifference = false;

        for (int i = 0; i < 50; i++)
        {
            if (provider1.GetColor(i) != provider2.GetColor(i))
            {
                foundDifference = true;
                break;
            }
        }

        Assert.True(foundDifference);
    }

    [Fact]
    public void SequentialColorsAreNotEqual()
    {
        var provider = new GeneratedColorProvider();

        for (int i = 0; i < 1000; i++)
        {
            Assert.NotEqual(
                provider.GetColor(i),
                provider.GetColor(i + 1));
        }
    }

    [Fact]
    public void LargeNumberOfGeneratedColorsAreUnique()
    {
        var provider = new GeneratedColorProvider();

        var colors = new HashSet<Color>();

        for (int i = 0; i < 500; i++)
        {
            Assert.True(colors.Add(provider.GetColor(i)),
                $"Duplicate color generated at index {i}");
        }
    }

    [Fact]
    public void ColorComponentsRemainWithinValidRange()
    {
        var provider = new GeneratedColorProvider();

        for (int i = 0; i < 1000; i++)
        {
            Color c = provider.GetColor(i);

            Assert.InRange(c.A, 0, 255);
            Assert.InRange(c.R, 0, 255);
            Assert.InRange(c.G, 0, 255);
            Assert.InRange(c.B, 0, 255);
        }
    }

    [Fact]
    public void ColorsAreFullyOpaque()
    {
        var provider = new GeneratedColorProvider();

        for (int i = 0; i < 1000; i++)
        {
            Assert.Equal(255, provider.GetColor(i).A);
        }
    }

    [Fact]
    public void NegativeIndexesThrowArgumentOutOfRangeException()
    {
        var provider = new GeneratedColorProvider();

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.GetColor(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => provider.GetColor(-123));
        Assert.Throws<ArgumentOutOfRangeException>(() => provider.GetColor(int.MinValue));
    }

    [Fact]
    public void LargeIndexesRemainDeterministic()
    {
        var provider = new GeneratedColorProvider();

        int[] indexes =
        {
                100000,
                500000,
                1000000,
                int.MaxValue
            };

        foreach (int index in indexes)
        {
            Color expected = provider.GetColor(index);

            Assert.Equal(expected, provider.GetColor(index));
        }
    }

    [Fact]
    public void FirstHundredSequentialColorsContainNoDuplicates()
    {
        var provider = new GeneratedColorProvider();

        var set = new HashSet<Color>();

        for (int i = 0; i < 100; i++)
        {
            Assert.True(set.Add(provider.GetColor(i)));
        }
    }

    [Fact]
    public void FirstFiveHundredSequentialColorsContainNoDuplicates()
    {
        var provider = new GeneratedColorProvider();

        var set = new HashSet<Color>();

        for (int i = 0; i < 500; i++)
        {
            Assert.True(set.Add(provider.GetColor(i)));
        }
    }

    [Fact]
    public void RequestingColorsOutOfOrderProducesSameResults()
    {
        var provider = new GeneratedColorProvider();

        Color a = provider.GetColor(100);
        Color b = provider.GetColor(5);
        Color c = provider.GetColor(999);
        Color d = provider.GetColor(42);

        Assert.Equal(a, provider.GetColor(100));
        Assert.Equal(b, provider.GetColor(5));
        Assert.Equal(c, provider.GetColor(999));
        Assert.Equal(d, provider.GetColor(42));
    }

    [Fact]
    public void RepeatedCallsDoNotChangePreviouslyGeneratedColors()
    {
        var provider = new GeneratedColorProvider();

        Color expected = provider.GetColor(7);

        for (int i = 0; i < 10000; i++)
        {
            provider.GetColor(i);
        }

        Assert.Equal(expected, provider.GetColor(7));
    }

    [Theory]
    [InlineData(0.00, 0.10)]
    [InlineData(0.00, 1.00)]
    [InlineData(0.25, 0.50)]
    [InlineData(0.50, 0.75)]
    [InlineData(0.75, 0.95)]
    [InlineData(1.00, 1.00)]
    public void VariousConstructorParametersProduceValidColors(double hue, double saturation)
    {
        var provider = new GeneratedColorProvider(hue, saturation);

        for (int i = 0; i < 100; i++)
        {
            Color color = provider.GetColor(i);

            Assert.InRange(color.R, 0, 255);
            Assert.InRange(color.G, 0, 255);
            Assert.InRange(color.B, 0, 255);
            Assert.Equal(255, color.A);
        }
    }

    [Fact]
    public void AdjacentColorsHaveDifferentArgbValues()
    {
        var provider = new GeneratedColorProvider();

        for (int i = 0; i < 500; i++)
        {
            Assert.NotEqual(
                provider.GetColor(i).ToArgb(),
                provider.GetColor(i + 1).ToArgb());
        }
    }
}