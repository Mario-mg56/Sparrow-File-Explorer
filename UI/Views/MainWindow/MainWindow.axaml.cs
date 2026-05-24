namespace DynamicFileExplorer.UI.Views.MainWindow;

using Avalonia.Controls;
using DynamicFileExplorer.ViewModels;

public partial class MainWindow : Window
{
    public readonly Canvas overlay;
    public MainWindow()
    {
        DataContext = new MainViewModel();

        InitializeComponent();
        overlay = this.FindControl<Canvas>("Overlay")!;
    }


}