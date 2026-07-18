# jColorProviders - v1.0.0 - Initial Release

jColorProviders provides a common `IColorProvider` interface for assigning 
`System.Drawing.Color` values using different strategies.

It is designed for applications that need consistent color selection logic,
such as log viewers, UI highlighting, syntax coloring, or visualization tools.

## IColorProvider

All providers implement:

```csharp
public interface IColorProvider<T>
{
    Color GetColor(T source);
}
```

## Providers

### Types of Providers

| Provider | Selection Method | Runtime Configurable |
|---|---|---|
| RegexColorProvider | Regex pattern matching | Yes |
| IndexedColorProvider | User-defined color sequence | Yes |
| FixedIndexColorProvider | Built-in color sequence | No |

### Choosing a Provider

Use `RegexColorProvider` when the color depends on text content.

Example:
- Log messages
- Search highlighting
- Syntax coloring

Use `IndexedColorProvider` when you need repeatable colors from a custom palette, indexed by with an int.

Example:
- Multiple cameras
- Data series
- Objects in a visualization

Use `FixedIndexColorProvider` when you need repeatable colors from a custom palette, but don't need specific colors.

### Provider Details
- RegexColorProvider
    Returns colors based on matching Regex patterns loaded at runtime

        Basic usage
        ```csharp
        colors = new RegexColorProvider();
        colors.AddPattern(new Regex(".*Hello.*", RegexOptions.Compiled), Color.Green);
        colors.AddPattern(new Regex(".*Goodbye.*", RegexOptions.Compiled), Color.Red);

        Color color1 = colors.GetColor("Hello, friend");
        Color color2 = colors.GetColor("Well then. Goodbye!");
        ```

- IndexedColorProvider
    Similar to the FixedColorProvider, but you may set the list of colors yourself
    
        Basic usage
        ```csharp
        colors = new IndexedColorProvider();
        colors.AddColor(Color.Green);
        colors.AddColor(Color.Red);
        colors.AddColor(Color.Magenta);
        colors.AddColor(Color.Green);

        Color color1 = colors.GetColor(1);             // Red
        Color color2 = colors.GetColor(4);             // Green

        int colorCount = colors.CountOfColor(Color.Green)          // Will be 2, because green is entered twice
        int indexOfRed = colors.GetIndexOfColor(Color.Red)         // Will return 1
        int indexOfGreen = colors.GetIndexOfColor(Color.Green, 2)    // Will return 3, because the first green was skipped in search

        Color color3 = colors.GetColor(9);             // Throws ArgumentOutOfRangeException
        
        colors.Clear();
        Color color4 = colors.GetColor(indexOfRed)     // Throws InvalidOperationException
        ```

- FixedIndexColorProvider
    Returns colors based on this fixed list:

            | Index | Color |
            |---|---|
            | 0 | Green |
            | 1 | Blue |
            | 2 | Red |
            | 3 | Magenta |
            | 4 | Cyan |
            | 5 | Yellow |

    looping back if numbers higher than 5 are used

        Basic usage
        ```csharp
        colors = new FixedIndexColorProvider();
        colors.AddPattern(new Regex(".*Hello.*", RegexOptions.Compiled), Color.Green);
        colors.AddPattern(new Regex(".*Goodbye.*", RegexOptions.Compiled), Color.Red);

        Color color1 = colors.GetColor(1);     // Blue
        Color color2 = colors.GetColor(4);     // Cyan
        Color color3 = colors.GetColor(9);     // Magenta
        ```

## Installation

Clone the repository:

```bash
git clone https://github.com/<user>/jColorProviders.git

```

## Requirements

- .NET 8.0 or later
- System.Drawing.Common

