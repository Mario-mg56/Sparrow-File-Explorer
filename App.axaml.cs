namespace DynamicFileExplorer;

using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Views.MainWindow;

public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;
    internal FileManager fileManager = null!;


    public override void Initialize()
    {
        fileManager = FileManager.Init(new DirItem(new Path(AppContext.BaseDirectory)));
        fileManager.SearchWorkingDir("Models");
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) 
            desktop.MainWindow = new MainWindow();
        base.OnFrameworkInitializationCompleted();
    }
}