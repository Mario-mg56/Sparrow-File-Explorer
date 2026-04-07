using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
