using Avalonia.Controls;
using Avalonia.Layout;

namespace DynamicFileExplorer.UI.Components;

public class FlexLayout : Border
{
    private readonly StackPanel panel;

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