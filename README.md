# jColorProviders

A simple interface for tracking System.Drawing.Color values in a range of ways

## Providers

- RegexColorProvider
    Returns colors based on matching Regex patterns loaded at runtime

        Basic usage
        ```csharp
        _colors = new RegexColorProvider();
        _colors.AddPattern(new Regex(".*Hello.*", RegexOptions.Compiled), Color.Green);
        _colors.AddPattern(new Regex(".*Goodbye.*", RegexOptions.Compiled), Color.Red);

        Color color1 = _colors.getColor("Hello, friend");
        Color color2 = _colors.getColor("Well then. Goodbye!");
        ```

- FixedIndexColorProvider
    Returns colors based on this fixed list:

            Index   |    Color
            --------|----------------
              0     |      Green
              1     |      Blue
              2     |      Red
              3     |      Magenta
              4     |      Cyan
              5     |      Yellow

    looping back if numbers higher than 5 are used

        Basic usage
        ```csharp
        _colors = new RegexColorProvider();
        _colors.AddPattern(new Regex(".*Hello.*", RegexOptions.Compiled), Color.Green);
        _colors.AddPattern(new Regex(".*Goodbye.*", RegexOptions.Compiled), Color.Red);

        Color color1 = _colors.getColor(1);     // Blue
        Color color2 = _colors.getColor(4);     // Cyan
        Color color3 = _colors.getColor(9);     // Magenta
        ```

- IndexedColorProvider
    Simular to the FixedColorProvider, but you may set the list of colors yourself
    
        Basic usage
        ```csharp
        _colors = new RegexColorProvider();
        _colors.AddColor(Color.Green);
        _colors.AddColor(Color.Red);
        _colors.AddColor(Color.Magenta);
        -colors.AddColor(Color.Green);

        Color color1 = _colors.getColor(1);             // Red
        Color color2 = _colors.getColor(4);             // Green

        int colorCount = _colors.CountOfColor(Color.Green)          // Will be 2, because green is entered twice
        int indexOfRed = _colors.GetIndexOfColor(Color.Red)         // Will return 1
        int indexOfRed = _colors.GetIndexOfColor(Color.Green, 2)    // Will return 3, because the first green was skipped in search

        Color color3 = _colors.getColor(9);             // Throws ArgumentOutOfRangeException
        
        _colors.Clear();
        Color color4 = _colors.getColor(indexOfRed)     // Throws InvalidOperationException
        ```