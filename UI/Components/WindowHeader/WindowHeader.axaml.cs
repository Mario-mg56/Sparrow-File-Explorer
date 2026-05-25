using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicFileExplorer.UI.Persistence;

namespace DynamicFileExplorer.UI.Components;

public partial class WindowHeader : UserControl
{
    public WindowHeader()
    {
        InitializeComponent();

        App.Current.UIManager.AddOnMainWindowLoadedListener( mw => {
            Minimize.PointerPressed += (s, e) => {
                if (e.GetCurrentPoint(Minimize).Properties.IsLeftButtonPressed)
                    mw.WindowState = WindowState.Minimized;
            };
            Maximize.PointerPressed += (s, e) => {
                if (e.GetCurrentPoint(Maximize).Properties.IsLeftButtonPressed)
                    mw.WindowState =  mw.WindowState == WindowState.Maximized ? 
                        WindowState.Normal : WindowState.Maximized;
            };
            Close.PointerPressed += (s, e) => {
                if (e.GetCurrentPoint(Close).Properties.IsLeftButtonPressed)
                    mw.Close();
            };
            mw.PropertyChanged += (s, e) => {
                if (e.Property == Window.WindowStateProperty){
                    if (mw.WindowState == WindowState.Maximized)
                        Maximize.IconPath = Icons.RESTORE_SVG_PATH;
                    else Maximize.IconPath = Icons.MAXIMIZE_SVG_PATH;
                }
            };
        });
    }
}

public class WindowHeaderLayout : StackPanel
{
    public bool Reversed
    {
        get => FlowDirection == FlowDirection.LeftToRight;
        set => FlowDirection = value ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
    }

    public WindowHeaderLayout()
    {
        Reversed = false;
        Orientation = Orientation.Horizontal;

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => PointerPressed += (s, e) =>  {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)  {
                if (e.ClickCount == 2) mw.WindowState = //Si es doble click maximizamos o restauramos
                    mw.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                else  mw.BeginMoveDrag(e);     
            }
        });

        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }
}