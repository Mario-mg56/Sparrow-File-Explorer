using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

class FileView : Grid
{
    public string name;
    public readonly Image iconImg;
    public readonly TextBlock label;
    static readonly int PADDING = 10;
    private readonly FileSystemItem file;
    public FileView(FileSystemItem file)
    {
        this.file = file;
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

        PointerPressed += (_, e) => {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) OnLeftClick();
        };

        App.Current.fileManager.FocusChanged += (focusedItems) => {
            foreach (var f in focusedItems) {
                if (f == file) {
                    Background = new SolidColorBrush(Colors.LightBlue);
                    return;
                }
            }
            Background = new SolidColorBrush(Colors.Transparent);
        };
    }
    private void OnLeftClick()
    {
        FileManager fm = App.Current.fileManager;

        if (file is File fi) fm.Select(fi);
        else if(file is DirItem fo) fm.Select(fo);
    }
}