namespace DynamicFileExplorer.UI.Components;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using DynamicFileExplorer.Models;

public class ContextMenu : FlexLayout
{
    public int x = 0, y = 0;
    public readonly List<ContextMenuItem> items;
    public event Action<ContextMenu>? OnShowMenu;
    public ContextMenu(List<ContextMenuItem>? items = null) {
        IsVisible = false;
        IsHitTestVisible = true;
        this.items = items ?? [];
        Padding = new Thickness(0, 0, 0, ContextMenuItemView.MARGIN_TOP);
        this.items.ForEach(i => Children.Add(new ContextMenuItemView(i)));
    }

    public void Show() {
        IsVisible = true;
        OnShowMenu?.Invoke(this);
    }
    public void Hide() => IsVisible = false;

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}
