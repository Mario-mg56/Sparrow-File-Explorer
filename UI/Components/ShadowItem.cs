using Avalonia.Controls;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Components;

public class ShadowItem : Border
{
    public static readonly float ALPHA = 0.5f;
    public ShadowItem(Control item)
    {
        Child = item;
        IsVisible = false;
        Background = new SolidColorBrush(Color.FromArgb((byte) (255*ALPHA), Colors.LightBlue.R, Colors.LightBlue.G, Colors.LightBlue.B));
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            mw.overlay.Children.Add(this)
        );
    }

    public void SetPosition(int x, int y) {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}