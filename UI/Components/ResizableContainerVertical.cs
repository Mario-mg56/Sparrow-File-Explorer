using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;

namespace DynamicFileExplorer.UI.Components;
public class ResizableContainerVertical:ResizableContainer
{

    private RowDefinition definition;
    private Styles Styles = App.Styles;
    private bool invert;
    public ResizableContainerVertical(Border border, RowDefinition definition,bool invert):base(border)
    {
        this.invert = invert;
        RebuildBorder(border);
        this.definition = definition;
        
    }
    public override void RebuildBorder(Border border)
    {
        border.BorderBrush = new SolidColorBrush(Color.Parse(Styles.SecondaryColor));
        border.BorderThickness = new Thickness(0, invert?2:0, 0, invert?0:2);
        // 1. Sacar el contenido actual del Border
        if (border.Child is not Control oldRoot)
            return;

        // 2. Desenganchar del Border (IMPORTANTE)
        border.Child = null;

        // 3. Crear nuevo Grid
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("*, 5"),
            Name = "HeaderBorder"
        };

        grid.PointerMoved += HeaderBorder_PointerMoved;

        // 4. Reasignar el antiguo contenido a Row 0
        Grid.SetRow(oldRoot,invert?1:0);
        grid.Children.Add(oldRoot);

        // 5. Crear botón
        var button = new Button
        {
            Background = Brushes.Transparent,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            Cursor = new Cursor(StandardCursorType.SizeNorthSouth),
            BorderBrush = Brushes.Transparent,
        };

        button.PointerPressed += OnPressed;
        button.PointerReleased += OnReleased;

        Grid.SetRow(button, invert?0:1);
        grid.Children.Add(button);

        // 6. Reemplazar contenido del Border
        border.Child = grid;
    }

    public override void SetSize(Avalonia.Point pixels)
    {
        var height = border.Bounds.Height;
        double value = invert? height-pixels.Y:pixels.Y;
        if (value <5)
            return;
        definition.Height = new GridLength(value);
    }
}