using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace DynamicFileExplorer.Models;
public class ContextMenuItem(SolidColorBrush? icon = null, string? name = null, Action<ContextMenuItem, Control?>? itemAction = null)
{
    public Control? AttachedLayout { get; internal set; }
    public readonly SolidColorBrush? icon = icon ?? GenerateIconPlaceholder();
    public readonly string name = name ?? "";
    public Action<ContextMenuItem, Control?>? itemAction = itemAction;

    public bool isActive = true;

    public virtual void Execute() => itemAction?.Invoke(this, AttachedLayout);
    

    private static SolidColorBrush GenerateIconPlaceholder()
    {   
        var rnd = new Random();
        return new SolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }
    public virtual bool Render()
    {
        System.Console.WriteLine("Deberia estar " + isActive);
        return isActive;
    }
}

public class ContextMenuItem<T> (SolidColorBrush? icon = null, string? name = null, Action<ContextMenuItem<T>, T?, Control?>? itemAction = null, Predicate<T>? whenAppears = null)
 : ContextMenuItem(icon, name, null)
{
    public T? AttachedItem { get; internal set; }
    public override bool Render()
    {
        if(isActive==false)return false;
        if(whenAppears==null)return true;
        if(AttachedItem==null)return true;
        return whenAppears.Invoke(AttachedItem);
    }
    public new Action<ContextMenuItem<T>, T?, Control?>? itemAction = itemAction;

    public override void Execute()
    {
        itemAction?.Invoke(this, AttachedItem, AttachedLayout);
        App.Current.tabManager.FocusedTab?.fileManager?.CastWorkingDirChanged();
    } 
}


