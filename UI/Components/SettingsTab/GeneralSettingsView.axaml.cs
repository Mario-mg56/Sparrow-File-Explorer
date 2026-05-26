using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.UI.Components;

public partial class GeneralSettingsView : UserControl
{
    public GeneralSettingsView()
    {
        InitializeComponent();
        LoadFromCache();
    }

    private void LoadFromCache()
    {
        var config = App.Current.Cache.Config;
        
        HiddenFilesCheckBox.IsChecked = config.DefaultIsHideItems;
        ExtensionsCheckBox.IsChecked = config.DefaultIsExtensionNameIncluded;
    }

    private void OnHiddenFilesCheckedChanged(object? sender, RoutedEventArgs e)
    {
        App.Current.Cache.Config.DefaultIsHideItems = HiddenFilesCheckBox.IsChecked ?? true;
        
        App.Current.FocusedTab?.fileManager?.ChangeHideItems(HiddenFilesCheckBox.IsChecked);
        
        SaveCache();
    }

    private void OnExtensionsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        App.Current.Cache.Config.DefaultIsExtensionNameIncluded = ExtensionsCheckBox.IsChecked ?? false;
        
        App.Current.FocusedTab?.fileManager?.CastWorkingDirChanged();
        
        SaveCache();
    }

    private void SaveCache()
    {
        PersistenceService.Save(App.Current.Cache);
    }
}