using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.Helpers;
using System.Collections.Generic;

namespace DynamicFileExplorer.UI.Components;

public partial class AppearanceSettingsView : UserControl
{
    private bool _isLoaded = false;
    
    private readonly List<string> _availableFonts = [
        "Inter", "Arial", "Segoe UI", "Consolas", "JetBrains Mono Nerd Font"
    ];

    public AppearanceSettingsView()
    {
        InitializeComponent();
        LoadFromCache();

        IconSizeSlider.PropertyChanged += OnIconSizePropertyChanged;
        FontSizeSlider.PropertyChanged += OnFontSizePropertyChanged;
        FontComboBox.SelectionChanged += OnFontSelectionChanged;
        
        _isLoaded = true;
    }

    private void LoadFromCache()
    {
        var style = App.Current.Cache.Style;
        
        IconSizeSlider.Value = style.IconSize;
        
        if (style.FontSize == 0) style.FontSize = 14; 
        FontSizeSlider.Value = style.FontSize;

        if (!_availableFonts.Contains(style.FontStyle ?? "Inter"))
        {
            _availableFonts.Add(style.FontStyle!);
        }
        FontComboBox.ItemsSource = _availableFonts;
        FontComboBox.SelectedItem = style.FontStyle ?? "Inter";
    }

    private void OnIconSizePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_isLoaded) return;
        if (e.Property == Slider.ValueProperty && e.NewValue is double newValue)
        {
            App.Current.Cache.Style.IconSize = (int)newValue;
            SaveCache();
            
            App.Current.FocusedTab?.fileManager?.CastWorkingDirChanged();
        }
    }

    private void OnFontSizePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_isLoaded) return;
        if (e.Property == Slider.ValueProperty && e.NewValue is double newValue)
        {
            App.Current.Cache.Style.FontSize = newValue;
            
            App.Current.Resources["AppFontSize"] = newValue;
            SaveCache();
        }
    }

    private void OnFontSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded) return;
        if (FontComboBox.SelectedItem is string selectedFont)
        {
            App.Current.Cache.Style.FontStyle = selectedFont;
            
            App.Current.Resources["AppFont"] = new FontFamily(selectedFont);
            SaveCache();
        }
    }

    private void SaveCache()
    {
        PersistenceService.Save(App.Current.Cache);
    }
}