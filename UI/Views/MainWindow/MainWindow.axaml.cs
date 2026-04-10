namespace DynamicFileExplorer.UI.Views.MainWindow;

using Avalonia.Controls;

public partial class MainWindow : Window
{
    public MainWindow() {
        InitializeComponent(); // SIEMPRE PRIMERO

        var grid = this.FindControl<Grid>("FilesGrid")!;
        
        new FilesGridController(grid).Mount();
    }
    
}