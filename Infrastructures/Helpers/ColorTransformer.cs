using System;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Helpers;

public static class ColorTransformer
{
    public static IBrush Glassify(this Color baseColor, double alpha)
    {
        alpha = Math.Clamp(alpha, 0.0, 1.0);
        byte alphaByte = (byte)Math.Round(alpha * 255);
        return new SolidColorBrush(Color.FromArgb(alphaByte, baseColor.R, baseColor.G, baseColor.B));
    }
    public static IBrush Glassify(string hexColor, double alpha) => Glassify(Color.Parse(hexColor), alpha);
    public static string ToHex(this IBrush brush)
    {
        if (brush is SolidColorBrush solidBrush)
        {
            Color c = solidBrush.Color;
            return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        }
        return "#00000000"; 
    }
}