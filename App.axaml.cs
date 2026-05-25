using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using static DynamicFileExplorer.Models.Path;
using DynamicFileExplorer.UI.Views.MainWindow;
using DynamicFileExplorer.UI.Config;
using static DynamicFileExplorer.Infrastructures.TabManager;

namespace DynamicFileExplorer;

public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;
    public MainWindow MainWindow { get; private set; } = null!;
    internal TabManager tabManager = null!;
    internal UIManager UIManager = null!;
    internal static SettingsService Settings = null!;
    internal static CacheData Cache = null!;
    internal static CacheService cacheService = null!;
    internal static new Styles Styles = null!;
    internal static Config Config = null!;
    internal List<Tab> Tabs { get => tabManager.tabs; }
    internal Tab? FocusedTab { get => tabManager.FocusedTab; }

    public override void Initialize()
    {
        AppArguments.Parse(Environment.GetCommandLineArgs());
        
                Theme.Apply(new CustomTheme());

        Settings = new SettingsService();
        Settings.Load();
        Styles = Settings.CurrentStyle;
        Config = Settings.Config;

        tabManager = Init();
        tabManager.CreateTab(new DirItem(BasePath));

        cacheService = new CacheService().Load();
        Cache = cacheService.Cache;
        cacheService.ImportLastDir();

        UIManager = UIManager.Init();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 🔥 SOLO el primer proceso lanza el segundo
        // if (!Environment.GetCommandLineArgs().Contains("--child"))
        // {
        //     Process.Start(new ProcessStartInfo
        //     {
        //         FileName = Environment.ProcessPath,
        //         Arguments = "--child",
        //         UseShellExecute = true
        //     });
        // }

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