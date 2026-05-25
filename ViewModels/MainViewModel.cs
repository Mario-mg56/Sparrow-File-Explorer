using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
namespace DynamicFileExplorer.ViewModels;
public class MainViewModel : INotifyPropertyChanged
{
    public IImage? BackgroundImage
    {
        get => string.IsNullOrWhiteSpace(App.Current.Cache.Cache.BgImage) ? null 
            : LoadImage(App.Current.Cache.Cache.BgImage);
    }

    public double BackgroundImageOpacity{get;} = 0.6;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshBackground() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundImage)));

    public static Bitmap LoadImage(string? image)
    {
        if (string.IsNullOrWhiteSpace(image))
        throw new ArgumentNullException(nameof(image));

        if (System.IO.File.Exists(image))
        {
            return new Bitmap(image);
        }

        var uri = new Uri(image);

        if (uri.Scheme == "avares")
        {
            var assets = AssetLoader.Open(uri);
            return new Bitmap(assets);
        }

        throw new NotSupportedException($"Formato de imagen no soportado: {image}");
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