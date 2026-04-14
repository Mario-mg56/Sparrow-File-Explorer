using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace DynamicFileExplorer.UI.Components;

public class ResizableContainer : ContentControl
{
    private bool _isDragging;
    private double _startY;
    private double _startHeight;

    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<ResizableContainer, double>(nameof(HeaderHeight), 150);

    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    // public override void ApplyTemplate()
    // {
    //     base.ApplyTemplate();

    //     this.PointerPressed += OnPressed;
    //     this.PointerReleased += OnReleased;
    //     this.PointerMoved += OnMoved;
    // }

    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(this);

        // Solo zona inferior (resize bar)
        if (p.Y >= Bounds.Height - 10)
        {
            _isDragging = true;
            _startY = e.GetPosition(null).Y;
            _startHeight = HeaderHeight;

            e.Pointer.Capture(this);
        }
    }

    private void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    private void OnMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;

        var currentY = e.GetPosition(null).Y;
        var delta = currentY - _startY;

        HeaderHeight = Math.Max(50, _startHeight + delta);
    }
}
