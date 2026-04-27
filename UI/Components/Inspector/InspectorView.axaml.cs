using System;
using Avalonia.Controls;
using DynamicFileExplorer.UI.Components.Inspector;

namespace DynamicFileExplorer.UI.Components;

public partial class InspectorView : UserControl
{
    public InspectorView()
    {
        InitializeComponent();
        DataContext = new InspectorViewModel();
        App.Current.UIManager.OnTabsResized += b=>{
            if(b.Equals(Parent))
            {
                BlurBackground.RefreshBlur();
                
            }
        };
    }
}
