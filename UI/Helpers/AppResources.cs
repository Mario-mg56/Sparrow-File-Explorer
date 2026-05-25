using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace DynamicFileExplorer.UI.Helpers;
public static class AppResources
{
    public static FontFamily Font =>  new(App.Current.Cache.Style.FontStyle);
    public static SolidColorBrush TextColor => new(Color.Parse(App.Current.Cache.Style.TextColor));

        public static readonly HashSet<string> ImageExtensions =
    new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".gif",
        ".webp",
        ".tiff"
    };



}
