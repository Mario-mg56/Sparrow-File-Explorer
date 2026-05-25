using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Config;
using DynamicFileExplorer.UI.Helpers;
using DynamicFileExplorer.UI.Views.MainWindow;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public class FileView : Grid
{
    public readonly FileViewController controller;
    public string name;
    public readonly Image iconImg;
    public readonly TextBlock label;
    public static readonly int PADDING = 10, ICON_SIZE = App.Styles.IconSize;
    public static readonly SolidColorBrush TRANSPARENT = new(Colors.Transparent);
    public static SolidColorBrush SelectedColor {get; private set;} = new(Color.Parse(Config.Theme.Current.FileSelected));
    public static SolidColorBrush HoverColor {get; private set;} = new(Color.Parse(Config.Theme.Current.FileHover));

    static FileView() {
        Config.Theme.ThemeChanged += theme => (SelectedColor, HoverColor) =
            (new SolidColorBrush(Color.Parse(theme.FileSelected)), new SolidColorBrush(Color.Parse(theme.FileHover)));
    }

    public FileView(FileSystemItem file, bool uncontrolled = false)
    {
        controller = uncontrolled ? null! : new FileViewController(file, this);

        name = file is File f ? App.Config.DefaultIsExtensionNameIncluded ? f.path.name:f.NameWithoutExtension() : file.path.name;
        Margin = new Thickness(PADDING);
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo

        RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        Bitmap source = null!;
        if (file is File tipo)
        {
            foreach (var icon in App.Config.Icons)
            {
                if (icon.Uri.Contains(tipo.GetPath()))
                {
                    source = MainViewModel.LoadImage(icon.Image);
                } else if (icon.Formatos.Contains(tipo.extension))
                {
                    source = MainViewModel.LoadImage(icon.Image);
                    break;
                }
            }

            source ??= MainViewModel.LoadImage(App.Styles.FileImageIcon);
        }
        else
        {
             foreach (var icon in App.Config.Icons)
            {
                if (icon.Uri.Contains(file.GetPath()))
                {
                    source = MainViewModel.LoadImage(icon.Image);
                } 
            }
            source ??= MainViewModel.LoadImage(App.Styles.DirImageIcon);
            
        }
        iconImg = new Image {
            Source = source,
            Stretch = Stretch.Uniform
        };

        label = new TextBlock {
            Text = name,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        SetRow(iconImg, 0);
        SetColumn(iconImg, 0);
        SetRow(label, 1);
        SetColumn(label, 0);

        Children.Add(iconImg);
        Children.Add(label);
    }

    public static ContextMenu<FileSystemItem> MakeFileContextMenu(FileManager fm) => new ([
        new (name: "Open", itemAction: (i, file, _) => {
            if (file != null) fm.Open(file);
        }),
        new (name: "Delete", itemAction: (i, file, _) => fm.Delete(file!)),
        new (name: "Rename", itemAction: (i, file, _) => {
            var input = TextInputPopUp.getInstance();    
            input.Show((s,_)=> fm.Rename(file!, s),_title: file?.path.name ?? "");
            App.cacheService.UpdateBgImage("path");
        }),
        new(
                name: "Copy path",
                itemAction: async (i, file, c) =>
                {
                    await ClipboardHelper.CopyToClipboard(
                        TopLevel.GetTopLevel(c)!,
                        file?.GetPath()
                    );
                }
            )
            ,
        new(
            name: "Set icon",
            itemAction: async (i, file, c) =>
            {
                var input = TextInputPopUp.getInstance();    
                if (file == null) return;
                if(file is File f){
                    input.Show((s,b)=>{IconHelper.SetIcon(f,s,b);},_watermark:"Path to img",_checkboxText:"Establecer para todos los "+f.extension);   
                } else {
                    input.Show((s,b)=>{IconHelper.SetIconForDirs(file,s,b);},_watermark:"Path to img",_checkboxText:"Establecer para todas las carpetas ");   
                }
            }
        )
        ,
        new (name: "Set as background image", itemAction: (_, file, _) => {
            if (file != null) {
                App.cacheService.UpdateBgImage(file.GetPath());
                App.Current.UIManager.AddOnMainWindowLoadedListener(mw => (mw.DataContext as MainViewModel)!.RefreshBackground());
            }
        },whenAppears:(file)=>{
            if(file is File f){
                return f.CheckExtension(AppResources.ImageExtensions);
            }
            return false;
        }
        )
    ]) ;
    // {Background = new SolidColorBrush(Color.FromArgb(180, 30, 30, 30))};
}