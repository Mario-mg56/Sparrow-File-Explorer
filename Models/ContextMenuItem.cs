using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace DynamicFileExplorer.Models;
public class ContextMenuItem(SolidColorBrush? icon = null, string? name = null, Action<ContextMenuItem>? itemAction = null)
{
    public readonly SolidColorBrush? icon = icon ?? GenerateIconPlaceholder();
    public readonly string name = name ?? "";
    public Action<ContextMenuItem>? itemAction = itemAction;

    public void Execute() => itemAction?.Invoke(this);
    
    private static SolidColorBrush GenerateIconPlaceholder()
    {   
        var rnd = new Random();
        return new SolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }
}

public class ContextSubMenu(List<ContextMenuItem>? items = null,
     SolidColorBrush? icon = null, string? name = null) : ContextMenuItem(icon, name)
{
    public readonly List<ContextMenuItem> items = items ?? [];
}
