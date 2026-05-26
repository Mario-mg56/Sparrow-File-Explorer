using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Components;

public class ResizableContainerVertical : ResizableContainer
{
    private RowDefinition topDefinition;

    public ResizableContainerVertical(
        Border top,
        RowDefinition topDefinition,
        Border thisBorder
    ) : base(top, thisBorder)
    {
        this.topDefinition = topDefinition;

    }

    public override void SetSize(Point pixels)
    {
        var y = pixels.Y;
        var minSize = 5;

        var topHeight = topDefinition.ActualHeight + y;
        // var botHeight = botDefinition.ActualHeight - y;

        if (topHeight < minSize)
            return;

        topDefinition.Height = new GridLength(topHeight);
        lastSize = new GridLength(topHeight);
        // botDefinition.Height = new GridLength(botHeight);
    }

    public override void OnafterThingChange(bool yes)
    {

        if (yes)
        {
            SetPrevThingSize(lastSize);
        } else
        {
            lastSize = topDefinition.Height;
            SetPrevThingSize(new GridLength(2,GridUnitType.Star));
        }
    }
    public override void SetPrevThingSize(GridLength newSize)
    {
        topDefinition.Height = newSize;

    }
}
