using Avalonia.Media;
using DynamicFileExplorer;

namespace DynamicFileExplorer.UI.Helpers;
public static class AppResources
{
   public static FontFamily Font =>
        new(App.Styles.FontStyle);
        public static SolidColorBrush TextColor =>
        new(Color.Parse(App.Styles.TextColor));


}
