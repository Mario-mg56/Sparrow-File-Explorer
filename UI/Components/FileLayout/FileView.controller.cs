using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Views.MainWindow;

namespace DynamicFileExplorer.UI.Components;

public class FileViewController
{
    private readonly FileSystemItem file;
    public readonly FileView view;
    public bool Selected {get; private set;} = false;
    public bool Dragging {get; private set;} = false;
    public ShadowItem shadowFile;
    public event Action<FileView>? StartDragging, StopDragging;
    private DirItem workingDir;
    public event Action<List<FileSystemItem>>? FilesDropped;
    public readonly DispatcherTimer draggingTimer = new()
         {Interval = TimeSpan.FromMilliseconds(DRAGGING_TIME_TRIGGER)};
    public static readonly int PADDING = 10, DRAGGING_TIME_TRIGGER = 100;
    public static readonly SolidColorBrush SELECTED_COLOR = new(Colors.LightBlue), TRANSPARENT = new(Colors.Transparent);

    public FileViewController(FileSystemItem file, FileView view)
    {
        this.file = file;
        this.view = view;

        shadowFile = new ShadowItem(new FileView(file, true)) {
            Width = FileView.ICON_SIZE, Height = FileView.ICON_SIZE
        };

        view.PointerPressed += OnLeftClick;

        draggingTimer.Tick += (_, _) =>
        {
            Dragging = true;
            workingDir = App.Current.fileManager.WorkingDir;
            App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved += OnDrag);
            StartDragging?.Invoke(view);
            draggingTimer.Stop();
        };
        
        view.PointerReleased += (_, e) => {
            if (e.InitialPressMouseButton == MouseButton.Left) stopDrag();
                
        };

        App.Current.fileManager.FocusChanged += (focusedItems) => {
            foreach (var f in focusedItems) {
                if (f == file) {
                    SetSelected(true);
                    return;
                }
            }
            SetSelected(false);
        };
        FilesDropped +=OnFilesDroppedDir;
    }
    private void OnLeftClick(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(view).Properties.IsLeftButtonPressed)
        {
            FileManager fm = App.Current.fileManager;
            if (file is File fi) fm.Select(fi);
            else if(file is DirItem fo) fm.Select(fo);
            draggingTimer.Start();
        }
    }


    private void stopDrag()
    {
        Console.WriteLine("eo");
        draggingTimer.Stop();
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
        shadowFile.IsVisible = false;
        App.Current.UIManager.FilesLayoutController!
            .PointingFile?.controller.DropFiles(App.Current.fileManager.SelectedItems);
        Dragging = false;
        StopDragging?.Invoke(view);

    }

    private void OnFilesDroppedDir(List<FileSystemItem> items)
    {
        if (file is DirItem folder)
        {
            items.OfType<File>()
                .ToList()
                .ForEach(f => App.Current.fileManager.Move(f, folder.path));

            items.OfType<DirItem>()
                .ToList()
                .ForEach(d => App.Current.fileManager.Move(d, folder.path));
        }
    }

    private void OnDrag(object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(sender as MainWindow);
        shadowFile.SetPosition((int) pos.X - FileView.ICON_SIZE/2, (int) pos.Y - FileView.ICON_SIZE/2);
        shadowFile.IsVisible = true;


        //HARDCODED BUG FIX IT 
        if(workingDir.GetPath().Equals(file.GetPath()))stopDrag();
    }

    public void DropFiles(List<FileSystemItem> droppedFiles)
    {
        Console.WriteLine(droppedFiles  + " dropped in " + view.name);
        FilesDropped?.Invoke(droppedFiles);
    }

    public void SetSelected(bool selected){
        Selected = selected;
        view.Background = selected ? SELECTED_COLOR : TRANSPARENT;
    }
}