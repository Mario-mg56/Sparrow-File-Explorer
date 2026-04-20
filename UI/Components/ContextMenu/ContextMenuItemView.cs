namespace DynamicFileExplorer.UI.Components;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.Models;

public class ContextMenuItemView : StackPanel
{
    public readonly ContextMenuItem item;
    public readonly static int ICON_SIZE = 16, GAP = 5, MARGIN_TOP = 5, MARGIN_X = 15;
    public readonly static IBrush
        background = Brushes.Transparent,
        foreground = Brushes.Black,
        hoverBackground = Brushes.LightGray;
    
    public ContextMenuItemView(ContextMenuItem item) {
        this.item = item;

        Orientation = Avalonia.Layout.Orientation.Horizontal;
        Background = background;
        Margin = new Thickness(MARGIN_X, MARGIN_TOP, MARGIN_X, 0);

        TextBlock label = new() {
            Text = item.name
        };
        Border iconImg = new() {
            Width = ICON_SIZE,
            Height = ICON_SIZE,
            Margin = new Thickness(0, 0, GAP, 0),
            Background = item.icon
        };
        Children.Add(iconImg);
        Children.Add(label);

        PointerPressed += (_, e) => {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) item.Execute();
        };

        PointerEntered += (_, _) => OnHoverEnter();
        PointerExited += (_, _) => OnHoverExit();
    }

    private void OnHoverEnter() => Background = hoverBackground;
    private void OnHoverExit() => Background = background;
}