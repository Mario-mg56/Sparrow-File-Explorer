using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting.Unicode;

namespace DynamicFileExplorer.UI.Helpers;
public static class AppResources
{
    public static bool miniExplorerMode=> AppArguments.IsThis(AppMode.MiniExplorer);
    public static GridLength widthLeftHeader =>
    miniExplorerMode ? new GridLength(10) : new GridLength(100);
    public static FontFamily Font =>  new(App.Styles.FontStyle);
    public static SolidColorBrush TextColor => new(Color.Parse(App.Styles.TextColor));

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
