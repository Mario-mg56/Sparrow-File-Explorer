using DynamicFileExplorer.UI.Helpers; // Asumiendo que aquí está tu ColorTransformer

namespace DynamicFileExplorer.UI.Persistence;

public static class ThemeGenerator
{
    private static string Glassify(string color, float alpha) => ColorTransformer.Glassify(color, alpha).ToHex();
    private static readonly string TRANSPARENT = "#00000000";

    public static string Resolve(CustomTheme theme, string propertyName)
    {
        var userValue = typeof(CustomTheme).GetProperty(propertyName)?.GetValue(theme) as string;
        if (!string.IsNullOrWhiteSpace(userValue)) return userValue;

        return propertyName switch
        {
            nameof(CustomTheme.TextForeground) => Resolve(theme, nameof(CustomTheme.Foreground)),
            nameof(CustomTheme.IconForeground) => Resolve(theme, nameof(CustomTheme.Foreground)),
            
            nameof(CustomTheme.Selection) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.6f),
            nameof(CustomTheme.FileSelector) => Resolve(theme, nameof(CustomTheme.Selection)),
            nameof(CustomTheme.FileSelected) => Resolve(theme, nameof(CustomTheme.Selection)),
            
            nameof(CustomTheme.FileHover) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.4f),
            nameof(CustomTheme.IconHover) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.6f),
            nameof(CustomTheme.IconHoverForeground) => Resolve(theme, nameof(CustomTheme.Foreground)),
            nameof(CustomTheme.TitleBarIconHover) => Resolve(theme, nameof(CustomTheme.IconHover)),
            nameof(CustomTheme.TitleBarCloseIconHover) => Glassify(Resolve(theme, nameof(CustomTheme.Destructive)), 0.6f),

            nameof(CustomTheme.TabBackground) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.3f),
            nameof(CustomTheme.TabBorder) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.6f),
            nameof(CustomTheme.TabHover) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.5f),
            nameof(CustomTheme.TabFocus) => Resolve(theme, nameof(CustomTheme.TabHover)),

            nameof(CustomTheme.ScrollThumb) => Resolve(theme, nameof(CustomTheme.Primary)),
            nameof(CustomTheme.ScrollThumbHover) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.8f),
            nameof(CustomTheme.ScrollThumbPressed) => Glassify(Resolve(theme, nameof(CustomTheme.Primary)), 0.6f),
            nameof(CustomTheme.ScrollTrack) => TRANSPARENT,
            nameof(CustomTheme.ScrollTrackHover) => TRANSPARENT,
            nameof(CustomTheme.ScrollTrackPressed) => TRANSPARENT,

            nameof(CustomTheme.NavBackground) => Glassify(Resolve(theme, nameof(CustomTheme.Secondary)), 0.5f),
            nameof(CustomTheme.SecondaryTransparent) => Glassify(Resolve(theme, nameof(CustomTheme.Secondary)), 0.1f),

            //Fallback
            nameof(CustomTheme.Primary) => "#1994aa",
            nameof(CustomTheme.Destructive) => "#ff0000",
            nameof(CustomTheme.Secondary) => "#000000",
            nameof(CustomTheme.Foreground) => "#ffffff",
            nameof(CustomTheme.IconBackground) => TRANSPARENT,

            _ => "#ff00ff"
        };
    }
}