using System;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Persistence;

public static class Theme
{
    public static CustomTheme Current {get; private set;} = new();
    public static event Action<CustomTheme>? ThemeChanged;
    public static void Apply(CustomTheme theme)
    {
        var resources = App.Current.Resources;
        
        var resolvedTheme = new CustomTheme {Name = theme.Name};

        foreach (var prop in typeof(CustomTheme).GetProperties())
        {
            if (prop.PropertyType == typeof(string) && prop.Name != "Name")
            {
                string finalHexColor = ThemeGenerator.Resolve(theme, prop.Name);
                resources[prop.Name] = SolidColorBrush.Parse(finalHexColor);
                prop.SetValue(resolvedTheme, finalHexColor);
            }
        }
        Current = resolvedTheme;

        if (App.Current?.Cache != null)
        {
            App.Current.Cache.Cache.CurrentTheme = theme.Name;
            DynamicFileExplorer.Helpers.PersistenceService.Save(App.Current.Cache);
        }

        ThemeChanged?.Invoke(resolvedTheme);
    }
}