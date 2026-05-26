using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.Models;
using static DynamicFileExplorer.Models.Path;
using DynamicFileExplorer.UI.Views.MainWindow;
using DynamicFileExplorer.UI.Persistence;
using static DynamicFileExplorer.Helpers.TabManager;
using System.Linq;
using DynamicFileExplorer.Infrastructures;
using System.Diagnostics;

namespace DynamicFileExplorer;

public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;
    public MainWindow MainWindow { get; private set; } = null!;
    public Persistence Cache { get; set; } = PersistenceService.Load();

    internal TabManager tabManager = null!;
    internal UIManager UIManager = null!;

    internal List<Tab> Tabs { get => tabManager.tabs; }
    internal Tab? FocusedTab { get => tabManager.FocusedTab; }

    public override void Initialize()
    {
        Resources["AppFont"] = new Avalonia.Media.FontFamily(Cache.Style.FontStyle ?? "Inter");
        Resources["AppFontSize"] = Cache.Style.FontSize > 0 ? Cache.Style.FontSize : 14.0;

        AppArguments.Parse(Environment.GetCommandLineArgs());

        Theme.Apply(
            Cache.Themes.FirstOrDefault(th => Cache.Cache.CurrentTheme == th.Name)
            ?? Cache.Themes.FirstOrDefault() ?? new()
        );

        tabManager = Init();
        tabManager.CreateTab(new DirItem(BasePath));

        var value = AppArguments.IsUse(AppUse.OpenWith)? new DirItem(new Path("/usr/share/applications")) :new DirItem(new Path(Cache.Cache.LastDir));
        Current.FocusedTab?.fileManager?.ChangeDirectory(value);

        UIManager = UIManager.Init();

        // DesktopLauncher.OpenWithDesktop("/usr/share/applications/vim.desktop","/home/diego/proyectos/interfaces/DynamicFileExplorer/Program.cs");

        Cache.Config.WDMenuStates.ForEach(s=>System.Console.WriteLine(s.Nombre));

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