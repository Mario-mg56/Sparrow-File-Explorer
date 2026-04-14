using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;

namespace DynamicFileExplorer.UI.Components;
public class ResizableContainerHorizontal:ResizableContainer
{

    private ColumnDefinition definition;
    readonly bool invert;
    public ResizableContainerHorizontal(Border border, ColumnDefinition definition, bool invert):base(border)
    {
        this.invert = invert;
        this.definition = definition;
        RebuildBorder(border);
        
    }
    public override void RebuildBorder(Border border)
    {
        // 1. Sacar el contenido actual del Border
        if (border.Child is not Control oldRoot)
            return;

        // 2. Desenganchar del Border (IMPORTANTE)
        border.Child = null;

        // 3. Crear nuevo Grid
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, 5"),
            Name = "HeaderBorder"
        };

        grid.PointerMoved += HeaderBorder_PointerMoved;

        // 4. Reasignar el antiguo contenido a Row 0
        Grid.SetColumn(oldRoot, invert?1:0);
        grid.Children.Add(oldRoot);

        // 5. Crear botón
        var button = new Button
        {
            Background = Avalonia.Media.Brushes.Transparent,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
            Cursor = new Cursor(StandardCursorType.SizeWestEast)
        };

        button.PointerPressed += OnPressed;
        button.PointerReleased += OnReleased;

        Grid.SetColumn(button, invert?0:1);
        grid.Children.Add(button);

        // 6. Reemplazar contenido del Border
        border.Child = grid;
    }

    public override void SetSize(Avalonia.Point pixels)
    {
        var width = border.Bounds.Width;
        
        double value = invert? width-pixels.X:pixels.X;
        if (value<5)
            return;

        definition.Width = new GridLength(value);
        
    }
}