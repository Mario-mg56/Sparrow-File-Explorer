using System;
using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
namespace DynamicFileExplorer.ViewModels;
public class MainViewModel : INotifyPropertyChanged
{
    public SettingsService SettingsService { get; } = App.Settings;

    public Styles Styles => SettingsService.CurrentStyle;
    public IImage BackgroundImage
    {
        // get => LoadImage(Styles.BackgroundImage);
        get => new Bitmap(App.Cache.BgImage);
    }
    public double BackgroundImageOpacity{get;} = 0.6;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshBackground() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundImage)));

    public static Bitmap LoadImage(string? image)
    {
        _ = image ?? throw new ArgumentNullException(nameof(image));

        var uri = new Uri(image);
        var assets = AssetLoader.Open(uri);
        return new Bitmap(assets);      
    }

}
