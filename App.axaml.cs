namespace DynamicFileExplorer;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Views.MainWindow;

public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;
    internal FileManager fileManager = null!;
    internal UIManager UIManager = null!;
    public override void Initialize()
    {
        fileManager = FileManager.Init(new DirItem(new Path(AppContext.BaseDirectory)));
        UIManager = UIManager.Init();
        
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mw = new();
            desktop.MainWindow = mw;
            UIManager.LoadMainWindow(mw);
        }
        base.OnFrameworkInitializationCompleted();
    }

    private void RegisterGlobalClicks(Window window) =>
         window.AddHandler(InputElement.PointerPressedEvent, OnGlobalPointerPressed, RoutingStrategies.Tunnel);
    

    private void OnGlobalPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(null);

        if (point.Properties.IsLeftButtonPressed)
        {
            Console.WriteLine("Click izquierdo global");
        }

        if (point.Properties.IsRightButtonPressed)
        {
            Console.WriteLine("Click derecho global");
        }
    }
}