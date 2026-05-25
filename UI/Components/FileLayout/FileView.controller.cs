using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.Util;

namespace DynamicFileExplorer.UI.Components;

public class FileViewController
{
    public readonly FileSystemItem file;
    public readonly FileView view;
    public bool Selected {get; private set;} = false;
    public bool Dragging {get => dragController.Dragging;}
    public ShadowItem shadowFile;
    public event Action<List<FileSystemItem>>? FilesDropped;
    public readonly DragController dragController;
    public static readonly int DRAGGING_TIME_TRIGGER = 100, OPEN_FILE_MAX_CLICK_INTERVAL = 500;
    public static readonly SolidColorBrush TRANSPARENT = new(Colors.Transparent);

    public FileViewController(FileSystemItem file, FileView view)
    {
        this.file = file;
        this.view = view;

        dragController = new DragController(view) {DraggingTimeTrigger = DRAGGING_TIME_TRIGGER};

        shadowFile = new ShadowItem(new FileView(file, true)) {
            Width = FileView.ICON_SIZE, Height = FileView.ICON_SIZE
        };

        dragController.Drag += OnDrag;
        dragController.StopDragging += OnStopDrag;
        
        FilesDropped += file is File ? OnFilesDroppedFile : OnFilesDroppedDir;
    }

    private void OnDrag(Control _, object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(sender as Control);
        shadowFile.SetPosition((int) pos.X - FileView.ICON_SIZE/2, (int) pos.Y - FileView.ICON_SIZE/2);
        shadowFile.IsVisible = true;
    }

    private void OnStopDrag(Control _, object? sender, PointerReleasedEventArgs e)
    {
        shadowFile.IsVisible = false;
        var flc = App.Current.UIManager.FilesLayoutController!;
        
        if (flc.PointingFile == null || flc.PointingFile.controller.file == file || flc.selectedFiles.Count == 0) return;
        
        var itemsToDrop = flc.selectedFiles.Select(fv => fv.controller.file).ToList();
        flc.PointingFile.controller.DropFiles(itemsToDrop);
    }

    public void DropFiles(List<FileSystemItem> droppedFiles)
    {
        Console.WriteLine(droppedFiles.Count + " items dropped in " + view.name);
        FilesDropped?.Invoke(droppedFiles);
    }

    public void SetSelected(bool selected){
        Selected = selected;
        view.Background = selected ? FileView.SelectedColor : TRANSPARENT;
    }

    private void OnFilesDroppedDir(List<FileSystemItem> items)
    {
        if (file is DirItem folder)
        {
            App.Current.FocusedTab?.fileManager?.MoveItems(items, folder);
        }
    }

    private void OnFilesDroppedFile(List<FileSystemItem> items)
    {
        if (file is File)
        {
            var input = TextInputPopUp.getInstance();   
            input.Show((s, _) => {
                var fm = App.Current.FocusedTab?.fileManager;
                if (fm == null) return;

                DirItem? newDir = fm.CreateDir(fm.WorkingDir, s);
                if (newDir == null) return;
                
                fm.MoveItems(items, newDir);
            }, _title:"Agrupar en Nueva Carpeta");   
        }
    }
}