using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace DynamicFileExplorer.Util;

public static class Util
{
    public static bool IsSquareTouchingSquare(Point startA, Point endA, Point startB, Point endB) => ( //Assuming origin bottom left
        endA.X >= startB.X && endA.Y >= startB.Y && endB.X >= startA.X && endB.Y >= startA.Y
    );
    
    public static T? BubbleSearchView<T>(Control pointedView, PointerEventArgs e) where T : Control
    {
        var visual = pointedView.InputHitTest(e.GetPosition(pointedView)) as Visual;
        while (visual != null && visual is not T) visual = visual.GetVisualParent();
        return visual as T;
    }
}
