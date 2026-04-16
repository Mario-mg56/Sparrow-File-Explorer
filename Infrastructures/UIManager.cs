using System;
using Avalonia.Controls;
using DynamicFileExplorer.UI.Views.MainWindow;
using ContextMenu = DynamicFileExplorer.UI.Components.ContextMenu;

namespace DynamicFileExplorer.Infrastructures;

class UIManager
{
    private static UIManager? instance;
    public MainWindow? MainWindow { get; private set; }
    public event Action<MainWindow>? OnMainWindowLoaded;
    public event Action<Border>? OnTabsResized;

    public UIManager()
    {
        
    }

    public static UIManager Init()
    {
        instance ??= new UIManager();
        return instance;
    }

    public void LoadMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        OnMainWindowLoaded?.Invoke(mainWindow);
    }
    public void CallResizeTabs(Border border)
    {
        OnTabsResized?.Invoke(border);
    }

    public double? GetRelativeWidth(Border border)
    {
        
        if (MainWindow is null)return null;
        return border.Bounds.Width/MainWindow.Bounds.Width;
    }

    public void AddContextMenu(Control control, ContextMenu menu)
    {
        if (MainWindow == null) OnMainWindowLoaded += (mw) => SetUpContextMenu(mw, control, menu);
        else SetUpContextMenu(MainWindow, control, menu);
    }

    private static void SetUpContextMenu(MainWindow mw, Control control, ContextMenu menu)
    {
        mw.PointerPressed += (_, _) => menu.Hide();
        control.PointerReleased += (sender, e) => {
            if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right)
            {
                var pos = e.GetPosition(mw);
                menu.SetPosition(
                    (int) (pos.X > mw.Bounds.Width/2 ? pos.X - menu.Bounds.Width : pos.X),
                    (int) (pos.Y > mw.Bounds.Height/2 ? pos.Y - menu.Bounds.Height : pos.Y)
                );
                menu.Show();
            }
        };

        mw.overlay.Children.Add(menu);
    }
}