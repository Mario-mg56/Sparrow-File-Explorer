using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace DynamicFileExplorer.UI.Components;

public class TextInputPopUp : Grid
{
    public string title
    {
        set
        {
            field = value;
            input.Text = value;
        }
        get;
    } = "";

    public readonly TextBox input; 
    static readonly int PADDING = 10;
    public readonly CheckBox checkBox;
    public readonly TextBlock checkLabel;

    private static TextInputPopUp? instance;

    public Action<string, bool>? Resolve { set; get; }

    public TextInputPopUp()
    {
        // Dimensions base del popup
        Width = 360; 
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        Margin = new Thickness(PADDING);
        Background = Brushes.Transparent;

        // Marco contenedor con esquinas redondeadas y sombras
        var cardBorder = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#2D2D30")), 
            BorderBrush = new SolidColorBrush(Color.Parse("#3F3F46")),      
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),                            
            Padding = new Thickness(16), // Padding controlado para evitar desbordamientos
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 15,
                OffsetX = 0,
                OffsetY = 5,
                Color = Color.Parse("#80000000") 
            })
        };

        // Cambiamos las filas a Auto y proporciones fijas para garantizar que el CheckBox siempre sea visible
        var internalGrid = new Grid();
        internalGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Fila para el TextBox
        internalGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Fila para el CheckBox
        internalGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        // TEXTBOX MODERNO
        input = new TextBox
        {
            Text = "",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch, 
            Height = 32,
            FontSize = 14,
            Watermark = "",
            Margin = new Thickness(0, 0, 0, 12) 
        };

        // CORRECCIÓN VISUAL DEL TEXTBOX:
        // En lugar de forzar Foreground/Background directamente (lo que rompe los estados de Avalonia),
        // sobreescribimos de manera segura los recursos internos del control para este TextBox.
        input.Resources.Add("TextBoxBackground", Color.Parse("#1E1E1F"));
        input.Resources.Add("TextBoxBackgroundFocused", Color.Parse("#1E1E1F"));
        input.Resources.Add("TextBoxBackgroundPointerOver", Color.Parse("#252526"));
        input.Resources.Add("TextBoxForeground", Colors.White);
        input.Resources.Add("TextBoxForegroundFocused", Colors.White);
        input.Resources.Add("TextBoxBorderBrush", Color.Parse("#515155"));
        input.Resources.Add("TextBoxBorderBrushFocused", Color.Parse("#007ACC")); // Azul sutil al hacer foco

        // CHECKBOX Y ETIQUETA
        checkBox = new CheckBox
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0)
        };

        checkLabel = new TextBlock
        {
            Text = "Option",
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.Parse("#CCCCCC")), 
            Margin = new Thickness(8, 0, 0, 0)
        };

        var checkPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(2, 0, 0, 4), // Margen de seguridad interno
            Children = { checkBox, checkLabel }
        };

        // Manteniendo tu misma lógica exacta de eventos de teclado
        input.KeyDown += (p, s) =>
        {
            if (s.Key == Avalonia.Input.Key.Enter)
            {
                System.Console.WriteLine(checkBox.IsEnabled);
                if (!string.IsNullOrEmpty(input.Text)) 
                {
                    Resolve?.Invoke(input.Text, checkBox.IsChecked ?? false);
                }
                Resolve = null;
                Hide();
            }
        };

        // Posicionamiento estricto en el Grid
        SetRow(input, 0);
        SetRow(checkPanel, 1);
        SetColumn(input, 0);
        SetColumn(checkPanel, 0);

        internalGrid.Children.Add(input);
        internalGrid.Children.Add(checkPanel);

        cardBorder.Child = internalGrid;
        Children.Add(cardBorder);

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw =>
            mw.overlay.Children.Add(this)
        );
    }

    public virtual void Show(Action<string, bool> _resolve, string _watermark = "", string _title = "", string _checkboxText = "") 
    {
        title = _title;
        Resolve = _resolve;
        input.Watermark = _watermark;
        input.Text = string.Empty; 

        if (!string.IsNullOrEmpty(_checkboxText))
        {
            SetCheckBoxText(_checkboxText);
            checkBox.IsVisible = true;
            checkLabel.IsVisible = true;
        } 
        else
        {
            checkBox.IsVisible = false;
            checkLabel.IsVisible = false;
        }

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            SetPosition((int)(mw.Bounds.Width / 2 - Width / 2),
                        (int)(mw.Bounds.Height / 2 - 50)) 
        );
        
        IsVisible = true;
        
        // Ejecutar el foco de manera segura tras asegurar que el control es visible
        Dispatcher.UIThread.Post(() => input.Focus());
    }

    public void SetCheckBoxText(string text)
    {
        checkLabel.Text = text;
    }

    public void Hide() => IsVisible = false;

    public void SetPosition(int x, int y) 
    {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }

    public static TextInputPopUp getInstance()
    {
        instance ??= new();
        return instance;
    }
}