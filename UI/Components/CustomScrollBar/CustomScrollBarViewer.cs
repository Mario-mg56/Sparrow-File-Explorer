using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicFileExplorer.UI.Config;

namespace DynamicFileExplorer.UI.Components;

public class CustomScrollBarViewer : ScrollViewer
{
    protected override Type StyleKeyOverride => typeof(ScrollViewer);
    public static readonly StyledProperty<IBrush> RestColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(RestColor), Brushes.Gray);

    public static readonly StyledProperty<IBrush> HoverColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(HoverColor), Brushes.LightGray);

    public static readonly StyledProperty<IBrush> PressedColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(PressedColor), Brushes.DarkGray);
    public static readonly StyledProperty<IBrush> TrackRestColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(TrackRestColor), SolidColorBrush.Parse("#00000000"));

    public static readonly StyledProperty<IBrush> TrackHoverColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(TrackHoverColor), SolidColorBrush.Parse("#00000000"));

    public static readonly StyledProperty<IBrush> TrackPressedColorProperty =
        AvaloniaProperty.Register<CustomScrollBarViewer, IBrush>(nameof(TrackPressedColor), SolidColorBrush.Parse("#00000000"));


    public IBrush RestColor { get => GetValue(RestColorProperty); set => SetValue(RestColorProperty, value); }
    public IBrush HoverColor { get => GetValue(HoverColorProperty); set => SetValue(HoverColorProperty, value); }
    public IBrush PressedColor { get => GetValue(PressedColorProperty); set => SetValue(PressedColorProperty, value); }
    
    public IBrush TrackRestColor { get => GetValue(TrackRestColorProperty); set => SetValue(TrackRestColorProperty, value); }
    public IBrush TrackHoverColor { get => GetValue(TrackHoverColorProperty); set => SetValue(TrackHoverColorProperty, value); }
    public IBrush TrackPressedColor { get => GetValue(TrackPressedColorProperty); set => SetValue(TrackPressedColorProperty, value); }

    private Style? _thumbRestStyleH;
    private Style? _thumbRestStyleV;

    public CustomScrollBarViewer()
    {
        //Ocultamos las arrows
        var hideArrowsStyle = new Style(x => x.OfType<ScrollBar>().Descendant().OfType<RepeatButton>());
        hideArrowsStyle.Setters.Add(new Setter(IsVisibleProperty, false));
        Styles.Add(hideArrowsStyle);

        //No funciona cambiar estos estilos desde App.axaml, por lo que lo hago aquí
        Config.Theme.ThemeChanged += ApplyScrollTheme;
        ApplyScrollTheme(Config.Theme.Current);

        UpdateColorStyles();
    }

    private void ApplyScrollTheme(CustomTheme theme)
    {
        RestColor = new SolidColorBrush(Color.Parse(theme.ScrollThumb));
        HoverColor = new SolidColorBrush(Color.Parse(theme.ScrollThumbHover));
        PressedColor = new SolidColorBrush(Color.Parse(theme.ScrollThumbPressed));
        TrackRestColor = new SolidColorBrush(Color.Parse(theme.ScrollTrack));
        TrackHoverColor = new SolidColorBrush(Color.Parse(theme.ScrollTrackHover));
        TrackPressedColor = new SolidColorBrush(Color.Parse(theme.ScrollTrackPressed));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RestColorProperty || change.Property == HoverColorProperty || change.Property == PressedColorProperty ||
            change.Property == TrackRestColorProperty || change.Property == TrackHoverColorProperty || change.Property == TrackPressedColorProperty)
        {
            UpdateColorStyles();
        }
    }

    private void UpdateColorStyles()
    {
        Resources["ScrollBarTrackFill"] = TrackRestColor;
        Resources["ScrollBarTrackFillPointerOver"] = TrackHoverColor;
        Resources["ScrollBarTrackFillPressed"] = TrackPressedColor;

        Resources["ScrollBarThumbFill"] = RestColor;
        Resources["ScrollBarThumbFillPointerOver"] = HoverColor;
        Resources["ScrollBarThumbFillPressed"] = PressedColor;

        //Limpiamos estilos inmutables
        if (_thumbRestStyleH != null) Styles.Remove(_thumbRestStyleH);
        if (_thumbRestStyleV != null) Styles.Remove(_thumbRestStyleV);

        _thumbRestStyleH = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Horizontal)
            .Descendant().OfType<Thumb>());
        _thumbRestStyleH.Setters.Add(new Setter(BackgroundProperty, RestColor));

        _thumbRestStyleV = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Vertical)
            .Descendant().OfType<Thumb>());
        _thumbRestStyleV.Setters.Add(new Setter(BackgroundProperty, RestColor));

        Styles.Add(_thumbRestStyleH);
        Styles.Add(_thumbRestStyleV);
    }
}