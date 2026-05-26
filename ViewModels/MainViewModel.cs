using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.Infrastructures;
namespace DynamicFileExplorer.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    // public SettingsService SettingsService { get; } = App.Current.Settings;

    public bool TopBarIsVisible{get;} = !AppArguments.IsThis(AppMode.MiniExplorer);

    // public Styles Styles => SettingsService.CurrentStyle;
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
            if (AppArguments.IsThis(AppMode.MiniExplorer))
            {
                field = false;
                return;
            }
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


    public ICommand Aplicar { get; }
    public ICommand Cancelar { get; }

    public MainViewModel()
    {
        Aplicar = new RelayCommand(OnAplicar);
        Cancelar = new RelayCommand(OnCancelar);
    }

    private void OnAplicar()
    {
        if (AppArguments.IsUse(AppUse.OpenWith) || AppArguments.IsUse(AppUse.CreateContextMenu))
        {
            var file = App.Current.UIManager.FilesLayoutController?.selectedFiles[0].controller.file;
            if (file == null) return;
            AppInstanceLauncher.Send(
                "OpenWith",
                "apply",
                file.GetPath()
            );
            App.Current.UIManager.MainWindow?.Close();
        }
    }

    private void OnCancelar()
    {
        if (AppArguments.IsUse(AppUse.OpenWith) || AppArguments.IsUse(AppUse.CreateContextMenu))
        {
            var file = App.Current.FocusedTab?.fileManager?.SelectedItems.Count;
            AppInstanceLauncher.Send(
                "OpenWith",
                "cancel",
                ""
            );
        }
        App.Current.UIManager.MainWindow?.Close();
    }
}