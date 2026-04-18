namespace DynamicFileExplorer.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Views.MainWindow;

public class ContextMenu : FlexLayout
{
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public List<Control> AttachedLayouts { get; protected set; }
    public readonly List<ContextMenuItem> items;
    public event Action<ContextMenu>? OnShowMenu;
    protected EventHandler<Avalonia.Input.PointerReleasedEventArgs>? OnReleaseAttachedLayoutHandler;
    public ContextMenu(List<ContextMenuItem> items, List<Control>? attachedLayouts = null) : base(){
        this.items = items;
        AttachedLayouts = attachedLayouts ?? [];
        IsVisible = false;
        Padding = new Thickness(0, 0, 0, ContextMenuItemView.MARGIN_TOP);

        this.items.ForEach(i =>  Children.Add(new ContextMenuItemView(i)));
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => {
            mw.PointerPressed += (_, _) => Hide();
            mw.overlay.Children.Add(this);
        });
        OnReleaseAttachedLayoutHandler = (sender, e) => {
            App.Current.UIManager.AddOnMainWindowLoadedListener(mw => {
                if (e.InitialPressMouseButton != Avalonia.Input.MouseButton.Right) return;
                OnReleaseAttachedLayout(mw, sender as Control, e);
                e.Handled = true;
            });
        };
    }

    public void SetAttachedLayouts(List<Control> attachedLayouts)
    {
        AttachedLayouts.ForEach(al => al.PointerReleased -= OnReleaseAttachedLayoutHandler);
        attachedLayouts.ForEach(al => al.PointerReleased += OnReleaseAttachedLayoutHandler);

        AttachedLayouts = attachedLayouts;
    }

    protected virtual void OnReleaseAttachedLayout(MainWindow mw, Control? sender, Avalonia.Input.PointerReleasedEventArgs e) {
        var pos = e.GetPosition(mw);
        SetPosition(
            (int) (pos.X > mw.Bounds.Width/2 ? pos.X - Bounds.Width : pos.X),
            (int) (pos.Y > mw.Bounds.Height/2 ? pos.Y - Bounds.Height : pos.Y)
        );
        Show();
        items.ForEach(i =>  i.AttachedLayout = sender);
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
    public List<ContextAttachement> Attachements { get; private set; }
    public ContextMenu(List<ContextMenuItem<T>> items, List<ContextAttachement>? attachements = null) : base([.. items.Cast<ContextMenuItem>()], ConvertAttachments(attachements))
    {
        Attachements = attachements ?? [];
    }

    public struct ContextAttachement(Control attachedLayout, T? attachedItem)
    {
        public Control AttachedLayout = attachedLayout;
        public T? AttachedItem = attachedItem;
    }

    public void SetAttachements(List<ContextAttachement>? attachements)
    {
        SetAttachedLayouts(ConvertAttachments(attachements));
        Attachements = attachements ?? [];
    }

    protected override void OnReleaseAttachedLayout(MainWindow mw, Control? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        base.OnReleaseAttachedLayout(mw, sender, e);
        var attachement = Attachements?.Find(a => a.AttachedLayout == sender);
        if (attachement == null) {Console.WriteLine("Attachement not found"); return;}
        items.OfType<ContextMenuItem<T>>().ToList().ForEach(i => {
            i.AttachedLayout = sender;
            i.AttachedItem = attachement.Value.AttachedItem;
        });
    }

    private static List<Control> ConvertAttachments(List<ContextAttachement>? items) {
        if (items == null) return [];
        List<Control> convertedItems = [];
        items.ForEach(i => convertedItems.Add(i.AttachedLayout));
        return convertedItems;
    }
}
