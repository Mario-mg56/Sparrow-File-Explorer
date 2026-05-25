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

        if (AppArguments.Mode == AppMode.MiniExplorer)
        {
            Topmost = true;
            ShowInTaskbar = false;
            CanResize = false;
            Width = 400;
            Height = 300;
            SystemDecorations = SystemDecorations.BorderOnly;
            FileView.compact = true;
        }

        InitializeComponent();
        overlay = this.FindControl<Canvas>("Overlay")!;
    }


}