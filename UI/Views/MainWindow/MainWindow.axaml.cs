namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.ViewModels;

public partial class MainWindow : Window
{
    public readonly Canvas overlay;
    private ResizableContainerHorizontal resizableInspector;
    public MainWindow()
    {
        DataContext = new MainViewModel();

        InitializeComponent(); // SIEMPRE PRIMERO
        var separador1 = this.FindControl<Border>("Separador1")!;
        var separador2 = this.FindControl<Border>("Separador2")!;
        var headerBorder = this.FindControl<Border>("HeaderContainer")!;
        var hierarchyContainer = this.FindControl<Border>("HierarchyContainer")!;
        var filesContainer = this.FindControl<Border>("FilesContainer")!;
        var inspectorContainer = this.FindControl<Border>("InspectorContainer")!;
        ((MainViewModel)DataContext).inspectorBorder = inspectorContainer;
        var separadorV = this.FindControl<Border>("SeparadorV")!;
        var mainGrid = this.FindControl<Grid>("MainGrid")!;
        overlay = this.FindControl<Canvas>("Overlay")!;
        RowDefinition headerDefinition = mainGrid.RowDefinitions[0];
        ColumnDefinition hierarchyDefinition = mainGrid.ColumnDefinitions[0];
        ColumnDefinition mainDefinition = mainGrid.ColumnDefinitions[2];
        ColumnDefinition inspectorDefinition = mainGrid.ColumnDefinitions[4];
        new ResizableContainerVertical(headerBorder, headerDefinition, separadorV);
        new ResizableContainerHorizontal(hierarchyContainer,hierarchyDefinition,separador1);
        resizableInspector = new ResizableContainerHorizontal(filesContainer,mainDefinition,separador2);
        ((MainViewModel)DataContext).changingInspector += resizableInspector.OnafterThingChange;

        
    }


       


}