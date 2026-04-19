using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public class FileView : Grid
{
    public readonly FileViewController controller;
    public string name;
    public readonly Image iconImg;
    public readonly TextBlock label;
    public static readonly int PADDING = 10, DRAGGING_TIME_TRIGGER = 20, ICON_SIZE = App.Styles.IconSize;
    public static readonly DispatcherTimer draggingTimer = new()
     {Interval = TimeSpan.FromMilliseconds(DRAGGING_TIME_TRIGGER)};

    public static readonly SolidColorBrush SELECTED_COLOR = new(Colors.LightBlue),
         TRANSPARENT = new(Colors.Transparent);

    public FileView(FileSystemItem file, bool uncontrolled = false)
    {
        controller = uncontrolled ? null! : new FileViewController(file, this);

        name = file is File f? App.Config.DefaultIsExtensionNameIncluded? f.path.name:f.NameWithoutExtension():file.path.name;
        Margin = new Thickness(PADDING);
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo

        RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
        RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        iconImg = new Image {
            
            Source = file is File? MainViewModel.LoadImage(App.Styles.FileImageIcon): MainViewModel.LoadImage(App.Styles.DirImageIcon),
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

    public static readonly ContextMenu<FileSystemItem> fileContextMenu = new ([
        new (name: "Open", itemAction: (i, file, _) => Console.WriteLine("wd " + file)),
        new (name: "Delete", itemAction: (i, file, _) => App.Current.fileManager.Delete(file!)),
        new (name: "Rename", itemAction: (i, file, _) => {
            var input = TextInputPopUp.getInstance();    
            input.Show();
            input.title = file?.path.name ?? "";
            input.Resolve = (s)=> App.Current.fileManager.Rename(file!, s);
            App.cacheService.UpdateBgImage("path");
        }),
        new (name: "Set as background image", itemAction: (_, file, _) => {
            if (file != null) {
                App.cacheService.UpdateBgImage(file.GetPath());
                App.Current.UIManager.AddOnMainWindowLoadedListener(mw => (mw.DataContext as MainViewModel)!.RefreshBackground());
            }
        })
    ]) {Background = Brushes.White};
}