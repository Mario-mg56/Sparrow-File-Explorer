namespace DynamicFileExplorer.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.UI.Views.MainWindow;

public class MainWindowViewModel : INotifyPropertyChanged
{
    
    private GridLength _headerHeight = new(1, GridUnitType.Star);
    public GridLength HeaderHeight
    {
        get => _headerHeight;
        set
        {
            if (_headerHeight != value)
            {
                _headerHeight = value;
                OnPropertyChanged();
            }
        }
    }


    // 🔹 Método que controla el tamañoprivate void ToggleSidebar(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    

    // 🔹 INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    public void SetHeaderHeight(double pixels)
    {
        HeaderHeight = new GridLength(pixels);
    }
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
