namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using DynamicFileExplorer.UI.Components;
using Avalonia.Interactivity;
using System.Linq;
using DynamicFileExplorer.Infrastructures;

public partial class MainWindow : Window
{

    readonly Button backButton, forwardButton;
    readonly FileManager fileManager;
    public MainWindow() {
        InitializeComponent(); // SIEMPRE PRIMERO

        var grid = this.FindControl<Grid>("FilesGrid")
            ?? throw new Exception("No se encontró FilesGrid");

        fileManager = App.Current.fileManager;

        backButton = this.FindControl<Button>("BackButton")
            ?? throw new Exception("No se encontró BackButton");

        forwardButton = this.FindControl<Button>("ForwardButton")
            ?? throw new Exception("No se encontró ForwardButton");

        new ContentPaneview(grid, fileManager, BoundsProperty).Render();

        backButton.Click += (_, _) => fileManager.GoBack();

        var host = this.FindControl<Grid>("HierarchyHost");
        host?.Children.Add(new HierarchyView());
       
    }
    
}