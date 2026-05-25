using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using DynamicFileExplorer.UI.Persistence;

namespace DynamicFileExplorer.UI.Components;

public partial class TabView : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TabView, string>(nameof(Title), "New Tab");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly StyledProperty<string> CloseIconPathProperty =
        AvaloniaProperty.Register<TabView, string>(nameof(CloseIconPath), Icons.CLOSE_SVG_PATH);

    public string CloseIconPath
    {
        get => GetValue(CloseIconPathProperty);
        set => SetValue(CloseIconPathProperty, value);
    }

    public event Action<TabView>? ClickedTab, ClosedTab;
    public static SolidColorBrush FocusColor {get; private set;} = new(Color.Parse(Persistence.Theme.Current.TabFocus));

    static TabView() {
        Persistence.Theme.ThemeChanged += theme => FocusColor =
            new SolidColorBrush(Color.Parse(theme.TabFocus));
    }

    public TabView()
    {
        InitializeComponent();
        CloseButton.PointerPressed += OnCloseButtonPressed;
        PointerPressed += OnClickTab;
    }

    private void OnClickTab(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) ClickedTab?.Invoke(this);
    }

    private void OnCloseButtonPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Handled = true;
            ClosedTab?.Invoke(this);
        }
    }

    public void FocusTab(bool focus)
    {
        Background = focus ? FocusColor
         : new SolidColorBrush(Color.Parse(Persistence.Theme.Current.TabBackground));
    }
}