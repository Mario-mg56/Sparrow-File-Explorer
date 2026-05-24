using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace DynamicFileExplorer.UI.Components;

public class ThinScrollBarViewer : CustomScrollBarViewer
{
    protected override Type StyleKeyOverride => typeof(ScrollViewer);

    public static readonly StyledProperty<double> ScrollBarThicknessProperty =
        AvaloniaProperty.Register<ThinScrollBarViewer, double>(nameof(ScrollBarThickness), 2.0);

    public double ScrollBarThickness
    {
        get => GetValue(ScrollBarThicknessProperty);
        set => SetValue(ScrollBarThicknessProperty, value);
    }

    private Style? _blockExpandStyleH;
    private Style? _normalStyleH;
    private Style? _blockExpandStyleV;
    private Style? _normalStyleV;

    public ThinScrollBarViewer()
    {
        AllowAutoHide = false;

        TrackRestColor = Brushes.Transparent;
        TrackHoverColor = Brushes.Transparent;
        TrackPressedColor = Brushes.Transparent;

        ScrollBarThickness = 5.0; 
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ScrollBarThicknessProperty) UpdateThicknessStyles();
        
    }

    private void UpdateThicknessStyles()
    {
        if (_blockExpandStyleH != null) Styles.Remove(_blockExpandStyleH);
        if (_normalStyleH != null) Styles.Remove(_normalStyleH);
        if (_blockExpandStyleV != null) Styles.Remove(_blockExpandStyleV);
        if (_normalStyleV != null) Styles.Remove(_normalStyleV);

        //Grosor horizontal
        _blockExpandStyleH = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Horizontal)
            .PropertyEquals(ScrollBar.IsExpandedProperty, true));
        _blockExpandStyleH.Setters.Add(new Setter(HeightProperty, ScrollBarThickness));
        _blockExpandStyleH.Setters.Add(new Setter(MaxHeightProperty, ScrollBarThickness));
        _blockExpandStyleH.Setters.Add(new Setter(MinHeightProperty, ScrollBarThickness));

        _normalStyleH = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Horizontal));
        _normalStyleH.Setters.Add(new Setter(HeightProperty, ScrollBarThickness));

        //Grosor vertical
        _blockExpandStyleV = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Vertical)
            .PropertyEquals(ScrollBar.IsExpandedProperty, true));
        _blockExpandStyleV.Setters.Add(new Setter(WidthProperty, ScrollBarThickness));
        _blockExpandStyleV.Setters.Add(new Setter(MaxWidthProperty, ScrollBarThickness));
        _blockExpandStyleV.Setters.Add(new Setter(MinWidthProperty, ScrollBarThickness));

        _normalStyleV = new Style(x => x.OfType<ScrollBar>()
            .PropertyEquals(ScrollBar.OrientationProperty, Orientation.Vertical));
        _normalStyleV.Setters.Add(new Setter(WidthProperty, ScrollBarThickness));

        Styles.Add(_blockExpandStyleH);
        Styles.Add(_normalStyleH);
        Styles.Add(_blockExpandStyleV);
        Styles.Add(_normalStyleV);
    }
}