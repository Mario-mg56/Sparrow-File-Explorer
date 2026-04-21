using System;
using System.Collections.Generic;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.Util;
using static DynamicFileExplorer.Util.Util;

namespace DynamicFileExplorer.UI.Components;

public class FileLayoutController
{
    private readonly List<ContextMenu<FileSystemItem>.ContextAttachement> attachements = [];
    public readonly Control fileLayout;
    public FileView? PointingFile {get; private set;}
    public readonly ContextMenu<DirItem> contextMenu;
    public event Action<FileView?>? OnChangePointingFile;
    public readonly DragController dragController;
    public readonly FileSelector fileSelector;
    private Point startPosFSBuffer;
    private readonly FileManager fileManager = App.Current.fileManager;
    public FileLayoutController(Control fileLayout)
    {
        App.Current.UIManager.FilesLayoutController = this;
        
        this.fileLayout = fileLayout;
        contextMenu = new (items:[
            new (name: "Crear Carpeta", itemAction: (i, wd, _) => {
                var input = TextInputPopUp.getInstance();    
                input.Show();   
                input.Show();   
                input.Show();   
                input.title = "";
                input.Resolve = (s)=> fileManager.CreateDir(wd!, s);
            }),
            new (name: "Crear Archivo", itemAction: (i, wd, _) => {
                var input = TextInputPopUp.getInstance();    
                input.Show();   
                input.title = "";
                input.Resolve = (s)=> fileManager.CreateFile(wd!, s);
            }),
            new (name: "Item 3", itemAction: (i, _, _) => Console.WriteLine(i.name + " selected"))
        ], attachements: [new ContextMenu<DirItem>.ContextAttachement(fileLayout, fileManager.WorkingDir)]) ;

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            mw.PointerMoved += (_, e) => SetPointingFile(BubbleSearchView<FileView>(mw, e))
        );

        fileSelector = new();
    
        dragController = new DragController(fileLayout)
            {StartDraggingCondition = (_, e) => PointingFile == null}; //No cuenta si arrastra un file
            
        dragController.StartDragging += (d, _, e) => {
            var pos = e.GetPosition(d);
            startPosFSBuffer = new Point((int) Math.Round(pos.X), (int) Math.Round(pos.Y));
            fileSelector.IsVisible = true;
        };
        dragController.Drag += (d, _, e) => {
            var pos = e.GetPosition(d);
            fileSelector.SetPosition(startPosFSBuffer, new Point((int) Math.Round(pos.X), (int) Math.Round(pos.Y)));
        };
        dragController.StopDragging += (d, _, e) => {
            fileSelector.IsVisible = false;
            fileSelector.Reset();
        };



        // fileLayout.PointerPressed += (_, e) => {
        //     BubbleSearchView<FileView>(fileLayout, e); //TODO: Si no hay ninguno settear los selected items a 0
        // };

        fileManager.WorkingDirChanged += ReloadFiles;
        ReloadFiles(fileManager.WorkingDir);
    }

    private void ReloadFiles(DirItem wd)
    {
        List<FileView> fvs = [];
        attachements.Clear();
        foreach(var f in fileManager.files)
        {
            var fv = new FileView(f);
            fvs.Add(fv);
            attachements.Add(new ContextMenu<FileSystemItem>.ContextAttachement(fv, f));
        }
        (fileLayout as IFileLayout)!.SetFileViews(fvs);
        FileView.fileContextMenu.SetAttachements(attachements);
        contextMenu.SetAttachements([new ContextMenu<DirItem>.ContextAttachement(fileLayout, wd)]);
    }

    private void SetPointingFile(FileView? pf)
    {
        PointingFile = pf;
        attachements.ForEach(a => {
            if(a.AttachedLayout is FileView fv)
                fv.Background = fv.controller.Selected ? FileView.SELECTED_COLOR : FileView.TRANSPARENT;
        });
        PointingFile?.Background = FileView.SELECTED_COLOR;
        OnChangePointingFile?.Invoke(PointingFile);
        System.Console.WriteLine(PointingFile);
    }
}