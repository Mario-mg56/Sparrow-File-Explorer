using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.Util;

namespace DynamicFileExplorer.UI.Components;

public class FileViewController
{
    public readonly FileSystemItem file;
    public readonly FileView view;
    public bool Selected {get; private set;} = false;
    public bool Dragging {get; private set;} = false;
    public ShadowItem shadowFile;
    public event Action<List<FileSystemItem>>? FilesDropped;
    public readonly DragController dragController;
    public static readonly int DRAGGING_TIME_TRIGGER = 100, OPEN_FILE_MAX_CLICK_INTERVAL = 500;
    public static readonly SolidColorBrush SELECTED_COLOR = new(Colors.LightBlue), TRANSPARENT = new(Colors.Transparent);

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
        Dragging = true;
        var pos = e.GetPosition(sender as Control);
        shadowFile.SetPosition((int) pos.X - FileView.ICON_SIZE/2, (int) pos.Y - FileView.ICON_SIZE/2);
        shadowFile.IsVisible = true;
    }
    private void OnStopDrag(Control _, object? sender, PointerReleasedEventArgs e)
    {
        Dragging = false;
        shadowFile.IsVisible = false;
        var flc = App.Current.UIManager.FilesLayoutController!;
        if (flc.PointingFile == null || flc.PointingFile.controller.file == file || flc.selectedFiles.Count == 0) return;
        flc.PointingFile.controller.DropFiles(App.Current.fileManager.SelectedItems);
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

    private void OnFilesDroppedDir(List<FileSystemItem> items)
    {
        if (file is DirItem folder)
        {

            FileManager.MoveItems(items,folder);
        }
    }
    private void OnFilesDroppedFile(List<FileSystemItem> items)
    {
        if (file is DirItem folder){
            var input = TextInputPopUp.getInstance();    
            input.Show();   
            input.title = "Nueva carpeta";
            input.Resolve = (s)=> {
            DirItem? newDir = App.Current.fileManager.CreateDir(App.Current.fileManager.WorkingDir, s);
                if(newDir==null)return;
                FileManager.MoveItems(items,newDir);

            
            };
        }

    }
}