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

    // 1. Cargamos los datos del formulario a partir de la caché global
    private void LoadFromCache()
    {
        var config = App.Current.Cache.Config;
        
        HiddenFilesCheckBox.IsChecked = config.DefaultIsHideItems;
        ExtensionsCheckBox.IsChecked = config.DefaultIsExtensionNameIncluded;
    }

    // 2. Cuando el usuario hace click, actualizamos la caché y guardamos
    private void OnHiddenFilesCheckedChanged(object? sender, RoutedEventArgs e)
    {
        App.Current.Cache.Config.DefaultIsHideItems = HiddenFilesCheckBox.IsChecked ?? true;
        
        // Si el usuario cambia esto, le decimos al FileManager actual que recargue los archivos
        App.Current.FocusedTab?.fileManager?.ChangeHideItems(HiddenFilesCheckBox.IsChecked);
        
        SaveCache();
    }

    private void OnExtensionsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        App.Current.Cache.Config.DefaultIsExtensionNameIncluded = ExtensionsCheckBox.IsChecked ?? false;
        
        // Aquí podrías forzar un Reload de la UI para que se redibujen los nombres
        App.Current.FocusedTab?.fileManager?.CastWorkingDirChanged();
        
        SaveCache();
    }

    private void SaveCache()
    {
        PersistenceService.Save(App.Current.Cache);
    }
}