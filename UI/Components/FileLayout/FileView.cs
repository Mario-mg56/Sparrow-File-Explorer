using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Infrastructures.Helpers;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Helpers;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public class FileView : Grid
{
    public static bool compact = false;
    public readonly FileViewController controller;
    public string name;
    public readonly Image iconImg;
    public readonly TextBlock label;
    public static readonly int PADDING = 10;
    public static int ICON_SIZE => App.Current.Cache?.Style?.IconSize ?? 100;
    public static readonly SolidColorBrush TRANSPARENT = new(Colors.Transparent);
    public static SolidColorBrush SelectedColor {get; private set;} = new(Color.Parse(Persistence.Theme.Current.FileSelected!));
    public static SolidColorBrush HoverColor {get; private set;} = new(Color.Parse(Persistence.Theme.Current.FileHover!));

    static FileView() {
        Persistence.Theme.ThemeChanged += theme => (SelectedColor, HoverColor) =
            (new SolidColorBrush(Color.Parse(theme.FileSelected!)), new SolidColorBrush(Color.Parse(theme.FileHover!)));
    }

    public FileView(FileSystemItem file, bool uncontrolled = false)
    {
        controller = uncontrolled ? null! : new FileViewController(file, this);

        name = file is File f ? App.Current.Cache.Config.DefaultIsExtensionNameIncluded ? f.path.name : f.NameWithoutExtension() : file.path.name;
        Margin = new Thickness(PADDING);
        Background = new SolidColorBrush(Colors.Transparent);

        Bitmap source = null!;
        
        if (file is File tipo)
        {
            foreach (var icon in App.Current.Cache.Config.Icons)
            {
                if (icon.Uri.Contains(tipo.GetPath()) || icon.Formatos.Contains(tipo.extension))
                {
                    source = MainViewModel.LoadImage(icon.Image);
                    break;
                }
            }
            source ??= MainViewModel.LoadImage(App.Current.Cache.Style.FileImageIcon);
        }
        else
        {
             foreach (var icon in App.Current.Cache.Config.Icons)
            {
                if (icon.Uri.Contains(file.GetPath()))
                {
                    source = MainViewModel.LoadImage(icon.Image);
                    break;
                } 
            }
            source ??= MainViewModel.LoadImage(App.Current.Cache.Style.DirImageIcon);
        }

        if (compact)
        {
            Height = 24;
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            iconImg = new Image
            {
                Source = source,
                Width = 24,
                Height = 24,
                Margin = new Thickness(5),
                Stretch = Stretch.Uniform
            };
            label = new TextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center
            };
            SetColumn(iconImg, 0);
            SetColumn(label, 1);
        }
        else
        {
            RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
            RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            iconImg = new Image
            {
                Source = source,
                Stretch = Stretch.Uniform
            };
            label = new TextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            SetRow(iconImg, 0);
            SetRow(label, 1);
        }

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
            input.Show((s, _) => fm.Rename(file!, s), _title: file?.path.name ?? "");
        }),
                new(
    name: "Open with",
    itemAction: async (i, file, c) =>
    {
        var process = AppInstanceLauncher.Open(
            AppMode.MiniExplorer,
            AppUse.OpenWith
        );


        AppInstanceLauncher.Listen(process, "OpenWith", msg =>
        {
            Console.WriteLine(
                "holaaa " + msg.Type + " payload: " + msg.Payload
            );

            if (msg.Type != "apply")
                return;

            if (file == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                if (file is File f)
                {
                    var inputForPipe = TextInputPopUp.getInstance();

                    inputForPipe.Show(_resolve:
                        (s, b) =>
                        {
                            System.Console.WriteLine("asdasdas"+ file.GetPath());
                            OpenToolHelper.SetTool(file, s, b);

                            App.Current
                                .FocusedTab?
                                .fileManager?
                                .Open(file);
                        },
                        _title: msg.Payload,
                        _checkboxText:
                            "Establecer para todos los " + f.extension
                    );
                }
            });
        });
    }
),
        new(
                name: "Copy path",
                itemAction: async (i, file, c) =>
                {
                    await ClipboardHelper.CopyToClipboard(
                        TopLevel.GetTopLevel(c)!,
                        file?.GetPath()?? ""
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
                    input.Show((s,b)=>{IconHelper.SetIcon(f,s,b);}, _watermark:"Path to img", _checkboxText:"Establecer para todos los "+f.extension);   
                } else {
                    input.Show((s,b)=>{IconHelper.SetIconForDirs(file,s,b);}, _watermark:"Path to img", _checkboxText:"Establecer para todas las carpetas ");   
                }
            }
        ),
        new (name: "Set as background image", itemAction: (_, file, _) => {
            if (file != null) {
                App.Current.Cache.Cache.BgImage = file.GetPath();
                App.Current.UIManager.AddOnMainWindowLoadedListener(mw => (mw.DataContext as MainViewModel)!.RefreshBackground());
            }
        }, whenAppears: (file) => {
            if (file is File f) {
                return f.CheckExtension(AppResources.ImageExtensions);
            }
            return false;
        })
    ]);
}