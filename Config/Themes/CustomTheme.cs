namespace DynamicFileExplorer.UI.Persistence;

public class CustomTheme
{
    public string Name { get; set; } = "default_theme";

    public string Primary { get; set; } = "#1994aa";
    public string Secondary { get; set; } = "#000000";
    public string Destructive { get; set; } = "#ff0000";
    public string Foreground { get; set; } = "#ffffff";
    public string IconBackground { get; set; } = "#00000000";
    public string? TextForeground { get; set; }
    
    public string? Selection { get; set; }
    public string? FileSelector { get; set; }
    public string? FileSelected { get; set; }
    public string? FileHover { get; set; }

    public string? IconForeground { get; set; }
    public string? IconHover { get; set; }
    public string? IconHoverForeground { get; set; }

    public string? TitleBarCloseIconHover { get; set; }
    public string? TitleBarIconHover { get; set; }

    public string? TabBackground { get; set; }
    public string? TabBorder { get; set; }
    public string? TabHover { get; set; }
    public string? TabFocus { get; set; }

    public string? ScrollThumb { get; set; }
    public string? ScrollThumbHover { get; set; }
    public string? ScrollThumbPressed { get; set; }
    public string? ScrollTrack { get; set; }
    public string? ScrollTrackHover { get; set; }
    public string? ScrollTrackPressed { get; set; }

    public string? SecondaryTransparent { get; set; }
    public string? NavBackground { get; set; }
}