using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Components;
public class ResizableContainerHorizontal:ResizableContainer
{

    private ColumnDefinition topDefinition;
    private Styles Styles = App.Styles;
    public ResizableContainerHorizontal(Border top,ColumnDefinition topDefinition, Border thisBorder):base(top,thisBorder)
    {
        this.topDefinition = topDefinition;
        
    }
    

    public override void SetSize(Avalonia.Point pixels)
    {
        var x = pixels.X;

        var minSize = 5;
        var leftWidth = TopBorder.Bounds.Width+x;

        if (leftWidth < minSize )
            return;

        topDefinition.Width = new GridLength(leftWidth);
        lastSize = new GridLength(leftWidth);

        
    }
    public override void OnafterThingChange(bool yes)
    {

        if (yes)
        {
            SetPrevThingSize(lastSize);
        } else
        {
            lastSize = topDefinition.Width;
            SetPrevThingSize(new GridLength(2,GridUnitType.Star));
        }
    }
    public override void SetPrevThingSize(GridLength newSize)
    {
        // Console.WriteLine(newSize+"   "+topDefinition.Width);
        topDefinition.Width = newSize;

    }
}