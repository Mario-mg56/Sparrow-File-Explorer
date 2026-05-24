

using Avalonia.Controls;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public partial class FilterView : UserControl
{
    
    public FilterView()
    {
        DataContext = new FilterViewModel();
        InitializeComponent();
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw =>{ mw.overlay.Children.Add(this);});
        
    }
    public virtual void Show() {
        IsVisible = true;
    }
    public void Hide() => IsVisible = false;

    public void SetPosition(int x, int y) {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}