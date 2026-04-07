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

    readonly FileManager fileManager;
    public MainWindow() {
        InitializeComponent(); // SIEMPRE PRIMERO

        var grid = this.FindControl<Grid>("FilesGrid")
            ?? throw new Exception("No se encontró FilesGrid");

        fileManager = App.Current.fileManager;

        
        new ContentPaneview(grid, fileManager, BoundsProperty).Render();


       
    }
    
}