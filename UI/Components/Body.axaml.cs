using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using static DynamicFileExplorer.Helpers.TabManager;

namespace DynamicFileExplorer.UI.Components;
public class Body : ThinScrollBarViewer
{
    protected override Type StyleKeyOverride => typeof(ScrollViewer);
    public readonly FilesGrid filesLayout;
    public readonly SettingsTab settings;
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

        settings = new();

        App.Current.tabManager.TabFocused += tab => {
            switch(tab?.Type)
            {
                case TabType.FILES:
                    Content = filesLayout;
                    filesLayout.fileLayoutController.SetFileManager(tab?.fileManager);     
                    break;
                case TabType.SETTINGS:
                    Content = settings;
                    break;
                default:
                    Content = null;     
                    break;
            }
        };
    }
}