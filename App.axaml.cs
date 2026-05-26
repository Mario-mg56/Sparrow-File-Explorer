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
        var value = AppArguments.IsUse(AppUse.OpenWith)? new DirItem(new Path("/usr/share/applications")) :cacheService.ImportLastDir();
        Current.FocusedTab?.fileManager?.ChangeDirectory(value);

        UIManager = UIManager.Init();

        // DesktopLauncher.OpenWithDesktop("/usr/share/applications/vim.desktop","/home/diego/proyectos/interfaces/DynamicFileExplorer/Program.cs");

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var process = Process.GetCurrentProcess();

AppInstanceLauncher.AppRuntime.RootProcess = process;
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