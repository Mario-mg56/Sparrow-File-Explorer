using Avalonia.Controls;

namespace DynamicFileExplorer.UI.Components;

public class ShadowItem : Border
{
    public static readonly float ALPHA = 0.5f;
    public ShadowItem(Control item)
    {
        Child = item;
        IsHitTestVisible = false;
        IsVisible = false;
        Opacity = ALPHA;
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            mw.overlay.Children.Add(this)
        );
    }

    public void SetPosition(int x, int y) {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}