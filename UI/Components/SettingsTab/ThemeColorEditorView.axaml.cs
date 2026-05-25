using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicFileExplorer.UI.Persistence;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DynamicFileExplorer.UI.Components;

public partial class ThemeColorEditorView : UserControl
{
    private readonly CustomTheme _theme;
    private readonly Action _onThemeRenamed;
    
    // Guardamos las referencias de todos los TextBoxes generados
    private readonly Dictionary<PropertyInfo, TextBox> _colorInputs = new();

    public ThemeColorEditorView(CustomTheme theme, Action onThemeRenamed)
    {
        InitializeComponent();
        _theme = theme;
        _onThemeRenamed = onThemeRenamed;

        InitializeHeaderControls();
        BuildDynamicForm();
        UpdateAllWatermarks(); // Inicializamos las marcas de agua
    }

    private void InitializeHeaderControls()
    {
        // Buscamos los controles manualmente de forma segura
        var themeNameTextBox = this.FindControl<TextBox>("ThemeNameTextBox");
        var activateThemeBtn = this.FindControl<Button>("ActivateThemeBtn");

        if (themeNameTextBox == null || activateThemeBtn == null) return;

        // 1. Asignamos el nombre actual
        themeNameTextBox.Text = _theme.Name;
        
        // 2. Validación y renombrado en tiempo real
        themeNameTextBox.TextChanged += (s, e) =>
        {
            string newName = themeNameTextBox.Text?.Trim() ?? "";

            // 🛑 EL CORTAFUEGOS: Si el nombre no ha cambiado realmente, ignorar
            if (newName == _theme.Name) return;

            bool isEmpty = string.IsNullOrEmpty(newName);
            bool isDuplicate = App.Current.Cache.Themes.Any(t => t != _theme && t.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));

            if (isEmpty || isDuplicate)
            {
                themeNameTextBox.Foreground = Brushes.Red;
                activateThemeBtn.IsEnabled = false; 
            }
            else
            {
                themeNameTextBox.ClearValue(ForegroundProperty);
                activateThemeBtn.IsEnabled = true;

                if (App.Current.Cache.Cache.CurrentTheme == _theme.Name)
                {
                    App.Current.Cache.Cache.CurrentTheme = newName;
                }

                _theme.Name = newName;
                _onThemeRenamed?.Invoke(); 
            }
        };

        // 3. Lógica del botón de activación
        activateThemeBtn.Click += (s, e) =>
        {
            App.Current.Cache.Cache.CurrentTheme = _theme.Name;
            Persistence.Theme.Apply(_theme);
        };
    }

    private void BuildDynamicForm()
    {
        var container = this.FindControl<StackPanel>("ColorsContainer");
        if (container == null) return;

        var properties = typeof(CustomTheme).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        string[] requiredBaseProperties = { "Primary", "Destructive", "Foreground", "IconBackground" };

        foreach (var prop in properties)
        {
            if (prop.Name == "Name" || prop.PropertyType != typeof(string)) continue;

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions("160, *"), Margin = new Avalonia.Thickness(0, 4) };
            var label = new TextBlock { Text = prop.Name, VerticalAlignment = VerticalAlignment.Center };
            
            var textBox = new TextBox { 
                Text = prop.GetValue(_theme) as string ?? "", 
                VerticalAlignment = VerticalAlignment.Center 
            };

            // Registramos el input en el diccionario
            _colorInputs[prop] = textBox;

            textBox.TextChanged += (s, e) =>
            {
                string inputText = textBox.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(inputText))
                {
                    prop.SetValue(_theme, null); // Guardamos como nulo para que el JSON quede limpio
                    
                    if (requiredBaseProperties.Contains(prop.Name))
                    {
                        textBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        textBox.ClearValue(ForegroundProperty);
                    }
                }
                else if (Color.TryParse(inputText, out _))
                {
                    textBox.ClearValue(ForegroundProperty);
                    prop.SetValue(_theme, inputText);
                }
                else
                {
                    textBox.Foreground = Brushes.Red;
                }

                // 🔥 UX MAGNÍFICA: Actualizamos las marcas de agua de los demás campos en tiempo real
                UpdateAllWatermarks();
            };

            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);
            Grid.SetRow(textBox, 0);
            Grid.SetColumn(textBox, 1);
            row.Children.Add(label);
            row.Children.Add(textBox);
            container.Children.Add(row);
        }
    }

    private void UpdateAllWatermarks()
    {
        string[] requiredBaseProperties = { "Primary", "Destructive", "Foreground", "IconBackground" };

        foreach (var kvp in _colorInputs)
        {
            var prop = kvp.Key;
            var textBox = kvp.Value;
            var currentValue = prop.GetValue(_theme) as string;

            // Solo mostramos el Watermark si el campo está vacío (es decir, usa el valor heredado)
            if (string.IsNullOrEmpty(currentValue))
            {
                if (requiredBaseProperties.Contains(prop.Name))
                {
                    textBox.Watermark = "Campo Base Obligatorio";
                }
                else
                {
                    // Le preguntamos al motor de generación cuál sería el color final actual
                    textBox.Watermark = ThemeGenerator.Resolve(_theme, prop.Name);
                }
            }
            else
            {
                textBox.Watermark = ""; // Si tiene valor escrito, limpiamos la marca de agua
            }
        }
    }
}