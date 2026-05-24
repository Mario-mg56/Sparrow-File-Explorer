

using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DynamicFileExplorer.UI.Components;
public class Body : ThinScrollBarViewer
{
    protected override Type StyleKeyOverride => typeof(ScrollViewer);
    public readonly FilesGrid filesLayout;
    public Body()
    {
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        
        filesLayout = new FilesGrid() {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };
        Content = filesLayout;
        filesLayout.fileLayoutController.FileManager = App.Current.FocusedTab?.fileManager;

        App.Current.tabManager.TabFocused += tab =>
            filesLayout.fileLayoutController.SetFileManager(tab?.fileManager);
    }
}