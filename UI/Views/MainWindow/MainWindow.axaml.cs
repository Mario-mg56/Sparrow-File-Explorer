namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;
using DynamicFileExplorer.UI.Components;
using Avalonia.Interactivity;
using System.Linq;
using DynamicFileExplorer.Infrastructures;
using System.ComponentModel;
using DynamicFileExplorer.ViewModels;
using System.Numerics;
using Avalonia.Input;

public partial class MainWindow : Window
{

    readonly FileManager fileManager;

    private bool isDraggin;
    public MainWindow()
    {
        InitializeComponent(); // SIEMPRE PRIMERO
        DataContext = new MainWindowViewModel();
        var grid = this.FindControl<Grid>("FilesGrid")
            ?? throw new Exception("No se encontró FilesGrid");

        fileManager = App.Current.fileManager;

        var vm = (MainWindowViewModel)DataContext!;
        vm.SetHeaderHeight(60);
        new ContentPaneview(grid, fileManager, BoundsProperty).Render();
    }


    private void HeaderBorder_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!isDraggin) return;
        var pos = e.GetPosition(this);

        var vm = (MainWindowViewModel)DataContext!;
        vm.SetHeaderHeight(pos.Y);
    }
    private void OnPressed(object? sender, PointerPressedEventArgs e) 
    {
        isDraggin = true;
    }
    private void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        isDraggin = false;
    }


}