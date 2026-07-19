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

- Direct Value Providers
| Provider | Selection Method | Runtime Configurable |
|---|---|---|
| RegexColorProvider | Regex pattern matching | Yes |
| IndexedColorProvider | User-defined color sequence | Yes |
| FixedIndexColorProvider | Hard-Coded color sequence | No |
| GeneratedColorProvider | Deterministic, destinct sequence generated from an int | No |
| FixedHashColorProvider  | Deterministicly generated from a string | No |

- Color-Modifying Providers
| Provider | Intent |
|---|---|
| TextColorProvider | User provides backcolor, provider returns Black or white as text color |
| MostVisibleColorProvider | User provides a color, provider returns highest contrast color |

### Choosing a Provider

Use `RegexColorProvider` when the color depends on text content.

Example:
- Log messages
- Search highlighting
- Syntax coloring

Use `IndexedColorProvider` when you need repeatable colors from a custom palette, indexed with an int.

Example:
- Multiple cameras
- Data series
- Objects in a visualization

Use `FixedIndexColorProvider` when you need repeatable colors from standard CV palette: G, B, R, M, C, Y, then repeat.  Very Fast.

Use `GeneratedColorProvider` when you want many visually distinct colors, generated deterministicly at runtime.

Use `FixedHashColorProvider` when you want to use a string to deterministicly generate a Color based on a string.

Use Color-Modifying Providers to help select colors for text and background that will work with runtime-selected colors

### Provider Details
- RegexColorProvider
    Returns colors based on matching Regex patterns loaded at runtime

        Basic Usage
        ```csharp
        colors = new RegexColorProvider();
        colors.AddPattern(new Regex(".*Hello.*", RegexOptions.Compiled), Color.Green);
        colors.AddPattern(new Regex(".*Goodbye.*", RegexOptions.Compiled), Color.Red);

        Color color1 = colors.GetColor("Hello, friend");
        Color color2 = colors.GetColor("Well then. Goodbye!");
        ```

- IndexedColorProvider
    Similar to the FixedColorProvider, but you may set the list of colors yourself
    
        Basic Usage
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
    Returns colors based on this fixed, reapeating list:

| Index | Color |
|---|---|
| 0 | Green |
| 1 | Blue |
| 2 | Red |
| 3 | Magenta |
| 4 | Cyan |
| 5 | Yellow |
| 6 | Green (again) |
| 7 | Blue (again) |

        Basic Usage
        ```csharp
        colors = new FixedIndexColorProvider();

        Color color1 = colors.GetColor(1);     // Blue
        Color color2 = colors.GetColor(4);     // Cyan
        Color color3 = colors.GetColor(9);     // Magenta
        ```

- GeneratedColorProvider
   - Returns generated colors using golden ratio stepping through HSV color space.
   - Each index shall always return the same color, given the same contructor parameters
   - Sequential colors shall be disimilar from each other
   - Constructor includes parameters for Hue and Saturation to tune pallete.

        Basic Usage
        ```csharp
        colors = new GeneratedColorProvider(0.75, 0.95); // Default constructor values.  Not omitted for demonstration's sake.

        Color color1 = colors.GetColor(1);     // \ Disimilar from each
        Color color2 = colors.GetColor(2);     // /       other
        ```

- FixedHashColorProvider
    - Returns deterministicly generated colors from a string using a hash function.
    - Each string shall always return the same color, given the same contructor parameters
    - Constructor includes parameters for Hue and Saturation to tune pallete.

        Basic Usage
        ```csharp
        colors = new FixedHashColorProvider(0.75, 0.85); // Default constructor values.  Not omitted for demonstration's sake.

        Color color1 = colors.GetColor("MainModule");
        Color color2 = colors.GetColor("Server_Networking");
        Color color3 = colors.GetColor("MainModule");           // Same as color1
        ```

- Color-Modifying Providers
    - Returns colors meant to be readable, visible or otherwise contrast against the supplied color

        Basic Usage
        ```csharp
        textColors = new TextColorProvider();
        visibleColors = new MostVisibleColorProvider();
        
        textBox1.ForeColor = textColors.GetColor(textBox1.BackColor)
        panel.BackColor = visibleColors.GetColor(form.BackColor)
        ```

## Installation

Clone the repository:

```bash
git clone https://github.com/<user>/jColorProviders.git

```

## Requirements

- .NET 8.0 or later
- System.Drawing.Common

