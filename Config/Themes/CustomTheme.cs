using DynamicFileExplorer.UI.Helpers;

namespace DynamicFileExplorer.UI.Config;

public class CustomTheme
{
    private string? _textFg, _iconFg, _iconHv, _iconHvFg, _selection, _fileSelector, _fileSelected, _fileHv, _titleBarIconHv, _titleBarCloseIconHv,
        _tabBg, _tabBd, _tabHv, _tabFc, _scrollThumb, _scrollThumbHv, _scrollThumbPressed;

    public string Primary { get; set; } = "#1994aa";
    public string Destructive {get; set;} = "#ff0000";
    public string Foreground {get; set;} = "#ffffff";
    public string TextForeground {get => _textFg ?? Foreground; set => _textFg = value;}

    public string Selection {get => _selection ?? Glassify(Primary, 0.6f); set => _selection = value;}
    public string FileSelector {get => _fileSelector ?? Selection; set => _fileSelector = value;}
    public string FileSelected {get => _fileSelected ?? Selection; set => _fileSelected = value;}
    public string FileHover {get => _fileHv ?? Glassify(Primary, 0.4f); set => _fileHv = value;}

    public string IconBackground {get; set;} = TRANSPARENT;
    public string IconForeground {get => _iconFg ?? Foreground; set => _iconFg = value;}
    public string IconHover {get => _iconHv ?? Glassify(Primary, 0.6f); set => _iconHv = value;}
    public string IconHoverForeground {get => _iconHvFg ?? Foreground; set => _iconHvFg = value;}

    public string TitleBarCloseIconHover {get => _titleBarCloseIconHv ?? Glassify(Destructive, 0.6f); set => _titleBarCloseIconHv = value;}
    public string TitleBarIconHover {get => _titleBarIconHv ?? IconHover; set => _titleBarIconHv = value;}

    public string TabBackground {get => _tabBg ?? Glassify(Primary, 0.3f); set => _tabBg = value;}
    public string TabBorder {get => _tabBd ?? Glassify(Primary, 0.6f); set => _tabBd = value;}
    public string TabHover {get => _tabHv ?? Glassify(Primary, 0.5f); set => _tabHv = value;}
    public string TabFocus {get => _tabFc ?? TabHover; set => _tabFc = value;}

    public string ScrollThumb {get => _scrollThumb ?? Primary; set => _scrollThumb = value;}
    public string ScrollThumbHover {get => _scrollThumbHv ?? Glassify(Primary, 0.8f); set => _scrollThumbHv = value;}
    public string ScrollThumbPressed {get => _scrollThumbPressed ?? Glassify(Primary, 0.6f); set => _scrollThumbPressed = value;}
    public string ScrollTrack {get; set;} = TRANSPARENT;
    public string ScrollTrackHover {get; set;} = TRANSPARENT;
    public string ScrollTrackPressed {get; set;} = TRANSPARENT;


    private static string Glassify(string color, float alpha) => ColorTransformer.Glassify(color, alpha).ToHex();

    private static readonly string TRANSPARENT = "#ffffff00";
}