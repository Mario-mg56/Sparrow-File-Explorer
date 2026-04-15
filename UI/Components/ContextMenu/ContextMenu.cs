namespace DynamicFileExplorer.UI.Components;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Views.MainWindow;

public class ContextMenu : FlexLayout
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public readonly List<ContextMenuItem> items;
    public event Action<ContextMenu>? OnShowMenu;
    public ContextMenu(Control attachedLayout, List<ContextMenuItem>? items = null) {
        this.items = items ?? [];
        IsVisible = false;
        Padding = new Thickness(0, 0, 0, ContextMenuItemView.MARGIN_TOP);
        this.items.ForEach(i => {
            i.AttachedLayout = attachedLayout;
            Children.Add(new ContextMenuItemView(i));
        });
        App.Current.UIManager.OnMainWindowLoaded += (mw) => SetUpContextMenu(mw, attachedLayout);
    }

    private void SetUpContextMenu(MainWindow mw, Control attachedLayout)
    {
        mw.PointerPressed += (_, _) => Hide();
        attachedLayout.PointerReleased += (sender, e) => {
            if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right)
            {
                var pos = e.GetPosition(mw);
                SetPosition(
                    (int) (pos.X > mw.Bounds.Width/2 ? pos.X - Bounds.Width : pos.X),
                    (int) (pos.Y > mw.Bounds.Height/2 ? pos.Y - Bounds.Height : pos.Y)
                );
                Show();
            }
        };

        mw.overlay.Children.Add(this);
    }

    public virtual void Show() {
        IsVisible = true;
        OnShowMenu?.Invoke(this);
    }
    public void Hide() => IsVisible = false;

    public void SetPosition(int x, int y) {
        X = x;
        Y = y;
        Canvas.SetLeft(this, X);
        Canvas.SetTop(this, Y);
    }
}

public class ContextMenu<T> : ContextMenu
{
    public new readonly List<ContextMenuItem<T>> items;

    public ContextMenu(Control attachedLayout, T attachedItem, List<ContextMenuItem<T>>? items = null) : base(attachedLayout)
    {
        this.items = items ?? [];
        this.items.ForEach(i => {
            i.AttachedLayout = attachedLayout;
            i.AttachedItem = attachedItem;
            Children.Add(new ContextMenuItemView(i));
        });
    }

    public void SetAttachedItem(T newItem) => items.ForEach(i => i.AttachedItem = newItem);
}
