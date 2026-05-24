using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.UI.Config;
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



    public Border inspectorBorder{set;get;} = null!;
  

    public Action<bool>? changingInspector;
    public bool inspectorVisibility
    {
        set
        {
            changingInspector?.Invoke(value);
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(inspectorVisibility)));
            inspectorWidth = value ? lastInspectorWidth :new GridLength(0, GridUnitType.Pixel);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(inspectorWidth)));
        }
        get;
    } = false;

    public GridLength inspectorWidth{get;set;} = new GridLength(0, GridUnitType.Star);
    public GridLength lastInspectorWidth{get;set;} = new GridLength(1,GridUnitType.Star);

    public GridLength resizableWidth{get;} = new GridLength(3,GridUnitType.Pixel);
    public IBrush resizableColor{get;} = Brushes.Black;
    
}