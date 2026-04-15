namespace DynamicFileExplorer.UI.Views.MainWindow;

using Avalonia.Controls;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.ViewModels;

public partial class MainWindow : Window
{
    public readonly Canvas overlay;

    public MainWindow()
    {
        DataContext = new MainViewModel();
        InitializeComponent(); // SIEMPRE PRIMERO
        var headerBorder = this.FindControl<Border>("HeaderContainer")!;
        var hierarchyContainer = this.FindControl<Border>("HierarchyContainer")!;
        var inspectorContainer = this.FindControl<Border>("InspectorContainer")!;
        var mainGrid = this.FindControl<Grid>("MainGrid")!;
        overlay = this.FindControl<Canvas>("Overlay")!;
        RowDefinition headerDefinition = mainGrid.RowDefinitions[0];
        ColumnDefinition hierarchyDefinition = mainGrid.ColumnDefinitions[0];
        ColumnDefinition inspectorDefinition = mainGrid.ColumnDefinitions[2];
        new ResizableContainerVertical(headerBorder, headerDefinition, false);
        new ResizableContainerHorizontal(hierarchyContainer, hierarchyDefinition, false);
        new ResizableContainerHorizontal(inspectorContainer, inspectorDefinition, true);
    }





}