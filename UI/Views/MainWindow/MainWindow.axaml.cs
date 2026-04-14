namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.UI.Components;

public partial class MainWindow : Window
{

    readonly FileManager fileManager;

    public MainWindow()
    {
        InitializeComponent(); // SIEMPRE PRIMERO
        var grid = this.FindControl<Grid>("FilesGrid")
            ?? throw new Exception("No se encontró FilesGrid");
        var headerBorder = this.FindControl<Border>("HeaderContainer")
            ?? throw new Exception("No se encontró HeaderContainer");
        var hierarchyContainer = this.FindControl<Border>("HierarchyContainer")
            ?? throw new Exception("No se encontró HeaderContainer");
        var inspectorContainer = this.FindControl<Border>("InspectorContainer")
            ?? throw new Exception("No se encontró HeaderContainer");
        var mainGrid = this.FindControl<Grid>("MainGrid")
            ?? throw new Exception("No se encontró MainGrid");
        RowDefinition headerDefinition = mainGrid.RowDefinitions[0];
        ColumnDefinition hierarchyDefinition = mainGrid.ColumnDefinitions[0];
        ColumnDefinition inspectorDefinition = mainGrid.ColumnDefinitions[2];
        new ResizableContainerVertical(headerBorder,headerDefinition,false);
        new ResizableContainerHorizontal(hierarchyContainer,hierarchyDefinition,false);
        new ResizableContainerHorizontal(inspectorContainer,inspectorDefinition,true);
        fileManager = App.Current.fileManager;

        new ContentPaneview(grid, fileManager, BoundsProperty).Render();
    }


       


}