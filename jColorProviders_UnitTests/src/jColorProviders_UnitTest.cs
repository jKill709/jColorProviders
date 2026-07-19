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

public class FixedHashColorProviderTests
{
    [Fact]
    public void Same_String_Returns_Same_Color()
    {
        var provider = new FixedHashColorProvider();

        Color first = provider.GetColor("MainModule");

        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(first, provider.GetColor("MainModule"));
        }
    }

    [Fact]
    public void Different_Instances_With_Default_Settings_Return_Same_Color()
    {
        var provider1 = new FixedHashColorProvider();
        var provider2 = new FixedHashColorProvider();

        Assert.Equal(
            provider1.GetColor("Networking"),
            provider2.GetColor("Networking"));
    }

    [Fact]
    public void Different_Instances_With_Same_Constructor_Parameters_Return_Same_Color()
    {
        var provider1 = new FixedHashColorProvider(0.60, 0.90);
        var provider2 = new FixedHashColorProvider(0.60, 0.90);

        Assert.Equal(
            provider1.GetColor("Logger"),
            provider2.GetColor("Logger"));
    }

    [Fact]
    public void Changing_Saturation_Changes_Output_Color()
    {
        var provider1 = new FixedHashColorProvider(0.30, 0.85);
        var provider2 = new FixedHashColorProvider(0.90, 0.85);

        Assert.NotEqual(
            provider1.GetColor("Renderer"),
            provider2.GetColor("Renderer"));
    }

    [Fact]
    public void Changing_Brightness_Changes_Output_Color()
    {
        var provider1 = new FixedHashColorProvider(0.75, 0.40);
        var provider2 = new FixedHashColorProvider(0.75, 1.00);

        Assert.NotEqual(
            provider1.GetColor("Renderer"),
            provider2.GetColor("Renderer"));
    }

    [Fact]
    public void Returned_Color_Is_Always_Fully_Opaque()
    {
        var provider = new FixedHashColorProvider();

        for (int i = 0; i < 500; i++)
        {
            Color color = provider.GetColor($"Item{i}");

            Assert.Equal(255, color.A);
        }
    }

    [Fact]
    public void Large_Number_Of_Calls_Remain_Deterministic()
    {
        var provider = new FixedHashColorProvider();

        Dictionary<string, Color> expected = new();

        for (int i = 0; i < 1000; i++)
        {
            string key = $"Key{i}";
            expected[key] = provider.GetColor(key);
        }

        foreach (var pair in expected)
        {
            Assert.Equal(pair.Value, provider.GetColor(pair.Key));
        }
    }

    [Fact]
    public void Similar_Strings_Do_Not_Produce_The_Same_Color()
    {
        var provider = new FixedHashColorProvider();

        Assert.NotEqual(
            provider.GetColor("Module1"),
            provider.GetColor("Module2"));
    }

    [Fact]
    public void Case_Changes_Affect_Color()
    {
        var provider = new FixedHashColorProvider();

        Assert.NotEqual(
            provider.GetColor("Logger"),
            provider.GetColor("logger"));
    }

    [Fact]
    public void Leading_Whitespace_Affects_Color()
    {
        var provider = new FixedHashColorProvider();

        Assert.NotEqual(
            provider.GetColor("Module"),
            provider.GetColor(" Module"));
    }

    [Fact]
    public void Trailing_Whitespace_Affects_Color()
    {
        var provider = new FixedHashColorProvider();

        Assert.NotEqual(
            provider.GetColor("Module"),
            provider.GetColor("Module "));
    }

    [Fact]
    public void Empty_String_Is_Deterministic()
    {
        var provider = new FixedHashColorProvider();

        Color first = provider.GetColor(string.Empty);
        Color second = provider.GetColor(string.Empty);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Unicode_Strings_Are_Deterministic()
    {
        var provider = new FixedHashColorProvider();

        const string text = "こんにちは世界";

        Assert.Equal(
            provider.GetColor(text),
            provider.GetColor(text));
    }

    [Fact]
    public void Long_Strings_Are_Deterministic()
    {
        var provider = new FixedHashColorProvider();

        string text = new string('X', 10000);

        Assert.Equal(
            provider.GetColor(text),
            provider.GetColor(text));
    }

    [Fact]
    public void Provider_Can_Handle_Many_Unique_Inputs()
    {
        var provider = new FixedHashColorProvider();

        HashSet<Color> colors = new();

        for (int i = 0; i < 500; i++)
        {
            colors.Add(provider.GetColor($"Input_{i}"));
        }

        // We don't require every color to be unique,
        // but excessive collisions would indicate a poor hash.
        Assert.True(colors.Count > 300);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(1.0, 1.0)]
    [InlineData(0.5, 0.5)]
    public void Valid_Constructor_Values_Do_Not_Throw(
        double saturation,
        double brightness)
    {
        var provider = new FixedHashColorProvider(saturation, brightness);

        Color color = provider.GetColor("Test");

        Assert.Equal(255, color.A);
    }

    [Theory]
    [InlineData(-0.01, 0.5)]
    [InlineData(1.01, 0.5)]
    [InlineData(0.5, -0.01)]
    [InlineData(0.5, 1.01)]
    public void Invalid_Constructor_Values_Throw(
        double saturation,
        double brightness)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new FixedHashColorProvider(saturation, brightness));
    }

    [Fact]
    public void Repeated_Calls_Do_Not_Create_Different_Results()
    {
        var provider = new FixedHashColorProvider();

        Color expected = provider.GetColor("PerformanceTest");

        for (int i = 0; i < 10000; i++)
        {
            Assert.Equal(expected, provider.GetColor("PerformanceTest"));
        }
    }
}

public abstract class ColorModifyingProviderTests
{
    /// <summary>
    /// Implemented by derived test classes.
    /// </summary>
    protected abstract IColorProvider<Color> CreateProvider();

    [Fact]
    public void GetColor_Should_NotThrow_ForBlack()
    {
        var provider = CreateProvider();

        var result = provider.GetColor(Color.Black);

        Assert.True(result.IsKnownColor || result.IsNamedColor || result.A != 0);
    }

    [Fact]
    public void GetColor_Should_NotThrow_ForWhite()
    {
        var provider = CreateProvider();

        var result = provider.GetColor(Color.White);

        Assert.True(result.IsKnownColor || result.IsNamedColor || result.A != 0);
    }

    [Fact]
    public void GetColor_Should_NotThrow_ForTransparent()
    {
        var provider = CreateProvider();

        var result = provider.GetColor(Color.Transparent);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(255, 255, 255)]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(255, 255, 0)]
    [InlineData(255, 0, 255)]
    [InlineData(0, 255, 255)]
    [InlineData(127, 127, 127)]
    [InlineData(10, 40, 200)]
    [InlineData(200, 100, 20)]
    public void GetColor_Should_ReturnValidColor(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color result = provider.GetColor(Color.FromArgb(r, g, b));

        Assert.InRange(result.A, 0, 255);
        Assert.InRange(result.R, 0, 255);
        Assert.InRange(result.G, 0, 255);
        Assert.InRange(result.B, 0, 255);
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(100, 150, 200)]
    [InlineData(5, 10, 15)]
    public void GetColor_Should_BeDeterministic(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color input = Color.FromArgb(r, g, b);

        Color first = provider.GetColor(input);
        Color second = provider.GetColor(input);

        Assert.Equal(first, second);
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(127, 127, 127)]
    [InlineData(30, 80, 200)]
    public void GetColor_Should_ReturnOpaqueColor(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color result = provider.GetColor(Color.FromArgb(r, g, b));

        Assert.Equal(255, result.A);
    }

    [Fact]
    public void GetColor_Should_WorkForEveryGrayLevel()
    {
        var provider = CreateProvider();

        for (int i = 0; i <= 255; i++)
        {
            Color result = provider.GetColor(Color.FromArgb(i, i, i));

            Assert.InRange(result.R, 0, 255);
            Assert.InRange(result.G, 0, 255);
            Assert.InRange(result.B, 0, 255);
        }
    }

    [Fact]
    public void MultipleCalls_Should_NotChangeBehavior()
    {
        var provider = CreateProvider();

        Color input = Color.CornflowerBlue;

        Color expected = provider.GetColor(input);

        for (int i = 0; i < 1000; i++)
        {
            Assert.Equal(expected, provider.GetColor(input));
        }
    }

    [Fact]
    public void Provider_Should_HandleManyColors()
    {
        var provider = CreateProvider();

        for (int r = 0; r <= 255; r += 31)
        {
            for (int g = 0; g <= 255; g += 31)
            {
                for (int b = 0; b <= 255; b += 31)
                {
                    Color result = provider.GetColor(Color.FromArgb(r, g, b));

                    Assert.InRange(result.A, 0, 255);
                }
            }
        }
    }
}
public class TextColorProviderTests : ColorModifyingProviderTests
{
    protected override IColorProvider<Color> CreateProvider()
        => new TextColorProvider();

    [Fact]
    public void BlackBackground_ReturnsWhite()
    {
        var provider = CreateProvider();

        Assert.Equal(Color.White, provider.GetColor(Color.Black));
    }

    [Fact]
    public void WhiteBackground_ReturnsBlack()
    {
        var provider = CreateProvider();

        Assert.Equal(Color.Black, provider.GetColor(Color.White));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(25, 25, 25)]
    [InlineData(64, 64, 64)]
    [InlineData(40, 90, 200)]
    [InlineData(120, 30, 40)]
    public void DarkColors_ReturnWhiteText(int r, int g, int b)
    {
        var provider = CreateProvider();

        Assert.Equal(Color.White, provider.GetColor(Color.FromArgb(r, g, b)));
    }

    [Theory]
    [InlineData(255, 255, 255)]
    [InlineData(240, 240, 240)]
    [InlineData(230, 220, 210)]
    [InlineData(255, 255, 128)]
    [InlineData(220, 180, 150)]
    public void LightColors_ReturnBlackText(int r, int g, int b)
    {
        var provider = CreateProvider();

        Assert.Equal(Color.Black, provider.GetColor(Color.FromArgb(r, g, b)));
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(120, 120, 120)]
    [InlineData(200, 100, 50)]
    public void ReturnedColor_IsAlwaysBlackOrWhite(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color result = provider.GetColor(Color.FromArgb(r, g, b));

        Assert.True(result == Color.Black || result == Color.White);
    }
}
public class MostVisibleColorProviderTests : ColorModifyingProviderTests
{
    protected override IColorProvider<Color> CreateProvider()
        => new MostVisibleColorProvider();

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(128, 128, 128)]
    [InlineData(10, 50, 200)]
    public void ReturnedColor_IsNotIdenticalToInput(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color input = Color.FromArgb(r, g, b);

        Assert.NotEqual(input, provider.GetColor(input));
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(80, 80, 80)]
    public void ReturnedColor_HasGreaterContrastThanInput(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color input = Color.FromArgb(r, g, b);
        Color result = provider.GetColor(input);

        double selfContrast = ContrastRatio(input, input);
        double resultContrast = ContrastRatio(input, result);

        Assert.True(resultContrast > selfContrast);
    }

    [Fact]
    public void ComplementaryColors_HaveHighContrast()
    {
        var provider = CreateProvider();

        Color result = provider.GetColor(Color.Red);

        Assert.True(ContrastRatio(Color.Red, result) >= 4.5);
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(128, 128, 128)]
    [InlineData(192, 192, 192)]
    public void HandlesSaturatedAndDesaturatedColors(int r, int g, int b)
    {
        var provider = CreateProvider();

        Color result = provider.GetColor(Color.FromArgb(r, g, b));

        Assert.NotEqual(Color.Empty, result);
    }

    private static double ContrastRatio(Color a, Color b)
    {
        double l1 = RelativeLuminance(a);
        double l2 = RelativeLuminance(b);

        if (l1 < l2)
            (l1, l2) = (l2, l1);

        return (l1 + .05) / (l2 + .05);
    }

    private static double RelativeLuminance(Color c)
    {
        static double Channel(double v)
        {
            v /= 255.0;
            return v <= 0.03928
                ? v / 12.92
                : Math.Pow((v + .055) / 1.055, 2.4);
        }

        return
            0.2126 * Channel(c.R) +
            0.7152 * Channel(c.G) +
            0.0722 * Channel(c.B);
    }
}