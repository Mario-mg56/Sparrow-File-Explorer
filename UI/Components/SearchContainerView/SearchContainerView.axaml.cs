using Avalonia.Controls;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public partial class SearchContainerView : UserControl
{
    public SearchContainerView()
    {
        InitializeComponent();
        
        DataContext = new SearchContainerViewModel();
       
    }

}
