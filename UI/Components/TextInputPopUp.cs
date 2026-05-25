
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
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
        get;}  = "";
    public readonly TextBox input; 
    static readonly int PADDING = 10;
    public readonly CheckBox checkBox;
    public readonly TextBlock checkLabel;

    private static TextInputPopUp? instance;

    public Action<string, bool>? Resolve {set;get;}
    public TextInputPopUp()
    {
        Margin = new Thickness(PADDING);
        Background = new SolidColorBrush(Colors.Transparent);

        RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star))); // input
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star))); // checkbox row

        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        input = new TextBox
        {
            Text = "",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = Brushes.Transparent,
        };

        checkBox = new CheckBox
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        checkLabel = new TextBlock
        {
            Text = "Option",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5,0,0,0)
        };

        var checkPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                checkBox,
                checkLabel
            }
        };

        input.KeyDown += (p, s) =>
        {
            if (s.Key == Avalonia.Input.Key.Enter)
            {
                if (input.Text.Equals("")) Resolve?.Invoke(input.Text,checkBox.IsEnabled);
                Resolve = null;
                Hide();
            }
        };

        SetRow(input, 0);
        SetRow(checkPanel, 1);

        SetColumn(input, 0);
        SetColumn(checkPanel, 0);

        Children.Add(input);
        Children.Add(checkPanel);

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw =>
            mw.overlay.Children.Add(this)
        );
    }

    public virtual void Show(Action<string, bool> _resolve,string _watermark = "",string _title = "", string _checkboxText = "") {
        title  = _title;
        Resolve  = _resolve;
        input.Watermark = _watermark;
        if (!_checkboxText.Equals(""))
        {
            SetCheckBoxText(_checkboxText);
            checkBox.IsVisible = true;
            checkLabel.IsVisible = true;
        } else
        {
            checkBox.IsVisible = false;
            checkLabel.IsVisible = false;
        }
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            SetPosition((int) (mw.Bounds.Width/2 - Bounds.Width),
                (int) (mw.Bounds.Height/2 - Bounds.Height))
        );
        IsVisible = true;
    }

    public void SetCheckBoxText(string text)
    {
        checkLabel.Text = text;
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