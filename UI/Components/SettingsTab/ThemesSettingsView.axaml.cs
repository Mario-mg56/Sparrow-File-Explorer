using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicFileExplorer.UI.Persistence;
using System;
using System.Linq;

namespace DynamicFileExplorer.UI.Components;

public partial class ThemesSettingsView : UserControl
{
    // Guardamos la referencia real al tema que el usuario tiene abierto
    private CustomTheme? _currentEditingTheme;

    public ThemesSettingsView()
    {
        InitializeComponent();
        RefreshThemesList();
        ThemesList.SelectionChanged += OnThemeSelected;

        var addThemeBtn = this.FindControl<Button>("AddThemeBtn");
        if (addThemeBtn != null)
        {
            addThemeBtn.Click += (_, _) =>
            {
                var newThemeName = "New Theme " + (App.Current.Cache.Themes.Length + 1);
                var newTheme = new CustomTheme { Name = newThemeName };

                var themesList = App.Current.Cache.Themes.ToList();
                themesList.Add(newTheme);
                App.Current.Cache.Themes = themesList.ToArray();

                _currentEditingTheme = newTheme; // Marcamos el nuevo como editado
                RefreshThemesList();
            };
        }
    }

    public void RefreshThemesList()
    {
        // 1. DESENGANCHAR: Evita que el ListBox intente cerrar el editor mientras reconstruimos la lista
        ThemesList.SelectionChanged -= OnThemeSelected;

        // 2. Reconstruir la lista de strings
        ThemesList.ItemsSource = App.Current.Cache.Themes.Select(t => t.Name).ToList();

        // 3. Volver a seleccionar el tema usando su nombre (incluso si acaba de cambiar por una pulsación de tecla)
        if (_currentEditingTheme != null)
        {
            ThemesList.SelectedItem = _currentEditingTheme.Name;
        }

        // 4. REENGANCHAR: Volvemos a la normalidad
        ThemesList.SelectionChanged += OnThemeSelected;
    }

    private void OnThemeSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (ThemesList.SelectedItem is string themeName)
        {
            var selectedTheme = App.Current.Cache.Themes.FirstOrDefault(t => t.Name == themeName);
            if (selectedTheme != null)
            {
                _currentEditingTheme = selectedTheme;
                ThemeEditorContainer.Content = new ThemeColorEditorView(selectedTheme, RefreshThemesList);
                return;
            }
        }
        
        _currentEditingTheme = null;
        ThemeEditorContainer.Content = null;
    }

    public void OnDeleteThemeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is string themeName)
        {
            if (App.Current.Cache.Cache.CurrentTheme == themeName)
            {
                Console.WriteLine($"Bloqueado: No puedes eliminar el tema '{themeName}' porque está activo.");
                return;
            }

            var themesList = App.Current.Cache.Themes.ToList();
            var themeToRemove = themesList.FirstOrDefault(t => t.Name == themeName);
            
            if (themeToRemove != null)
            {
                themesList.Remove(themeToRemove);
                App.Current.Cache.Themes = themesList.ToArray();
                
                if (_currentEditingTheme?.Name == themeName)
                {
                    _currentEditingTheme = null;
                    ThemeEditorContainer.Content = null;
                }
                
                RefreshThemesList();
            }
        }
    }
}