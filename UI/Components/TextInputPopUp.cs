
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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
        get;}  
    public readonly TextBox input; 
    static readonly int PADDING = 10;

    private static TextInputPopUp instance;

    public Action<string>? Resolve {set;get;}
    public TextInputPopUp()
    {
        Margin = new Thickness(PADDING);
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo

        RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        input = new TextBox
        {
            Text = "",
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Background = Brushes.Transparent,

        };

        input.KeyDown += (p,s)=>{
            if(s.Key == Avalonia.Input.Key.Enter)
            {
                Resolve?.Invoke(input.Text);
                Resolve = null;
                Hide();
            }
        };

        SetRow(input, 0);
        SetColumn(input, 0);

        Children.Add(input);
        
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.overlay.Children.Add(this));
    }

    public virtual void Show() {
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            SetPosition((int) (mw.Bounds.Width/2 - Bounds.Width),
                (int) (mw.Bounds.Height/2 - Bounds.Height))
        );
        IsVisible = true;
    }
    public void Hide() => IsVisible = false;

    public void SetPosition(int x, int y) {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }

    public static TextInputPopUp getInstance()
    {
        instance ??= new();
        return instance;
    }
}