using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicFileExplorer;
namespace DynamicFileExplorer.ViewModels;
public class MainViewModel
{
    public SettingsService SettingsService { get; } = App.Settings;

    public Styles Styles => SettingsService.CurrentStyle;
    public IImage BackgroundImage
    {
        get
        {
            return LoadImage(Styles.BackgroundImage);
        }
    }
    public double BackgroundImageOpacity{get;} = 0.6;

    public static Bitmap LoadImage(string? image)
    {
        _ = image ?? throw new ArgumentNullException(nameof(image));

        var uri = new Uri(image);
        var assets = AssetLoader.Open(uri);
        return new Bitmap(assets);      
    }

}
