using Avalonia.Controls;

namespace DynamicFileExplorer.UI.Components;

public partial class SettingsTab : UserControl
{
    private readonly GeneralSettingsView _generalView;
    private readonly AppearanceSettingsView _appearanceView;
    private readonly ThemesSettingsView _themesView;
    private readonly ContextSettingsView _contextMenuView;

    public SettingsTab()
    {
        InitializeComponent();
        _generalView = new GeneralSettingsView();
        _appearanceView = new AppearanceSettingsView();
        _themesView = new ThemesSettingsView();
        _contextMenuView = new ContextSettingsView();
    }

    private void OnCategoryChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoriesList.SelectedItem is ListBoxItem selectedItem)
        {
            string category = selectedItem.Content?.ToString() ?? "";
            FormContainer.Content = category switch
            {
                "General" => _generalView,
                "Appearance" => _appearanceView,
                "Themes" => _themesView,
                "ContextMenu" => _contextMenuView,
                _ => null
            };
        }
    }
}