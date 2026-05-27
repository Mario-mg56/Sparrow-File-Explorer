using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace DynamicFileExplorer.Models;

public class ContextSubMenu(List<ContextMenuItem>? items = null,
     IBrush? icon = null, string? name = null, Action<ContextMenuItem, Control?>? itemAction = null) : ContextMenuItem(icon, name, itemAction)
{
    public readonly List<ContextMenuItem> items = items ?? [];
}

public class ContextSubMenu<T>(List<ContextMenuItem<T>>? items = null, IBrush? icon = null, string? name = null,
     Action<ContextMenuItem<T>, T?, Control?>? itemAction = null)
 : ContextMenuItem<T>(icon, name, itemAction)
{
    public readonly List<ContextMenuItem<T>> items = items ?? [];
}



