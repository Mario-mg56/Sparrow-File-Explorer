using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Components;

public class FlexLayout : Border
{
    protected readonly StackPanel panel;

    public FlexLayout()
    {
        panel = new StackPanel();
        Child = panel;
    }

    public Orientation Orientation
    {
        get => panel.Orientation;
        set => panel.Orientation = value;
    }

    public Controls Children => panel.Children;
}