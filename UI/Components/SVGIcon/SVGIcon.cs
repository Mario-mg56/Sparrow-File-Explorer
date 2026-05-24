using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Components;

public class SVGIcon : Border
{
    public SVGIcon()
    {
        Child = new PathIcon();

    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconPathProperty) 
            Child.Data = Geometry.Parse(IconPath ?? "");

        if (change.Property == IconWidthProperty) 
            Child.Width = IconWidth;

        if (change.Property == IconHeightProperty) 
            Child.Height = IconHeight;

        if (change.Property == IconSizeProperty)
         {
             Child.Width = IconSize;
             Child.Height = IconSize;
         }

        if (change.Property == Padding_xProperty)
            Padding = new Thickness(Padding_x, Padding.Top, Padding_x, Padding.Bottom);
        
        if (change.Property == Padding_yProperty)
            Padding = new Thickness(Padding.Left, Padding_y, Padding.Right, Padding_y);
    }

    public static readonly StyledProperty<string?> IconPathProperty = AvaloniaProperty.Register<SVGIcon, string?>(nameof(IconPath));
    public static readonly StyledProperty<double> IconWidthProperty = AvaloniaProperty.Register<SVGIcon, double>(nameof(IconWidth));
    public static readonly StyledProperty<double> IconHeightProperty = AvaloniaProperty.Register<SVGIcon, double>(nameof(IconHeight));
    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<SVGIcon, double>(nameof(IconSize));
    public static readonly StyledProperty<double> Padding_xProperty = AvaloniaProperty.Register<SVGIcon, double>(nameof(Padding_x));
    public static readonly StyledProperty<double> Padding_yProperty = AvaloniaProperty.Register<SVGIcon, double>(nameof(Padding_y));
    public static readonly StyledProperty<IBrush> IconForegroundProperty = AvaloniaProperty.Register<SVGIcon, IBrush>(nameof(IconForeground));
    public static readonly StyledProperty<IBrush?> HoverBackgroundProperty = AvaloniaProperty.Register<SVGIcon, IBrush?>(nameof(HoverBackground));
    public static readonly StyledProperty<IBrush?> HoverForegroundProperty = AvaloniaProperty.Register<SVGIcon, IBrush?>(nameof(HoverForeground));

    public new PathIcon Child { 
        get => (PathIcon) base.Child!; 
        set => base.Child = value; 
    }

    public string? IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public double IconWidth {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public double IconSize {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public double Padding_x {
        get => GetValue(Padding_xProperty);
        set => SetValue(Padding_xProperty, value);
    }

    public double Padding_y {
        get => GetValue(Padding_yProperty);
        set => SetValue(Padding_yProperty, value);
    }

    public IBrush IconForeground {
        get => GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    public IBrush? HoverBackground {
        get => GetValue(HoverBackgroundProperty);
        set => SetValue(HoverBackgroundProperty, value);
    }

    public IBrush? HoverForeground {
        get => GetValue(HoverForegroundProperty);
        set => SetValue(HoverForegroundProperty, value);
    }
}