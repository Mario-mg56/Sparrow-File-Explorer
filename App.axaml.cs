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
    internal UIManager UIManager = null!;
    internal static SettingsService Settings = null!;
    internal static CacheData Cache = null!;
    internal static CacheService cacheService = null!;
    internal static new Styles Styles = null!;
    internal static Config Config = null!;
    public MainWindow MainWindow { get; private set; } = null!;

    public override void Initialize()
    {
        
        Settings = new SettingsService();
        Settings.Load();
        Styles = Settings.CurrentStyle;
        Config = Settings.Config;
        fileManager = FileManager.Init(new DirItem(new Path(AppContext.BaseDirectory)));
        cacheService = new CacheService().Load();
        Cache = cacheService.Cache;
        cacheService.ImportLastDir();
        UIManager = UIManager.Init();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mw = new();
            desktop.MainWindow = mw;
            MainWindow = mw;
            UIManager.LoadMainWindow(mw);
        }
        base.OnFrameworkInitializationCompleted();
    }
}