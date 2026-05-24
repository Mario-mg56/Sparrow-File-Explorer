using System;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Config;

public static class Theme
{
    public static CustomTheme Current {get; private set;} = new();
    public static event Action<CustomTheme>? ThemeChanged;
    public static void Apply(CustomTheme theme)
    {
        var resources = App.Current.Resources;

        //Itemramos por las propiedades de CustomTheme
        foreach (var prop in typeof(CustomTheme).GetProperties())
        {
            if (prop.PropertyType == typeof(string))
                resources[prop.Name] = SolidColorBrush.Parse(prop.GetValue(theme) as string ?? "");
        }

        Current = theme;
        ThemeChanged?.Invoke(theme);
    }
}