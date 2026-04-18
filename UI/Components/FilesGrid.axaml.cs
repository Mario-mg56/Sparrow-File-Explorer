namespace DynamicFileExplorer.UI.Components;

using System;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;
using System.Reactive.Linq;     
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Media;
using DynamicFileExplorer.Models;
using System.Collections.Generic;

class FilesGrid : Grid
{
    public int iconSize;
    public int Cols {get; private set;}
    public int Rows {get; private set;}
    public readonly ContextMenu<DirItem> contextMenu;
    public readonly ContextMenu<FileSystemItem> fileContextMenu;
    private readonly FileManager fileManager = App.Current.fileManager;
    private readonly List<ContextMenu<FileSystemItem>.ContextAttachement> attachements = [];

    public FilesGrid()
    {
        iconSize = App.Styles.IconSize;
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo

        this.GetObservable(BoundsProperty).Subscribe(OnResize);

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
        ], attachements: [new ContextMenu<DirItem>.ContextAttachement(this, fileManager.WorkingDir)]) {Background = Brushes.White};

        fileContextMenu = new ([
            new (name: "Open", itemAction: (i, file, _) => Console.WriteLine("wd " + file)),
            new (name: "Delete", itemAction: (i, file, _) => fileManager.Delete(file!)),
            new (name: "Rename", itemAction: (i, file, _) => {
                var input = TextInputPopUp.getInstance();    
                input.Show();
                input.title = file?.path.name ?? "";
                input.Resolve = (s)=> fileManager.Rename(file!, s);
            })
        ]) {Background = Brushes.White};

        this.GetObservable(BoundsProperty).Subscribe(OnResize);

        fileManager.WorkingDirChanged += OnChangeWorkingDir;
        OnChangeWorkingDir(fileManager.WorkingDir);
    }

    private void OnChangeWorkingDir(DirItem wd)
    {
        SetUpFileViews([.. fileManager.files]);
        contextMenu.SetAttachements([new ContextMenu<DirItem>.ContextAttachement(this, wd)]);
        OnResize(Bounds);
    }

    private void SetUpFileViews(List<FileSystemItem> files)
    {
        attachements.Clear();

        files.ForEach(f => {
            var fv = new Border {Child = new FileView(f)};

            SetRow(fv, 0);
            SetColumn(fv, 0);
            Children.Add(fv);

            attachements.Add(new ContextMenu<FileSystemItem>.ContextAttachement(fv, f));
        });

        fileContextMenu.SetAttachements(attachements);
    }

    private void OnResize(Rect bounds)
    {
        if (bounds.Width == 0 || bounds.Height == 0) return;

        RowDefinitions.Clear();
        ColumnDefinitions.Clear();

        Cols = (int) Math.Floor((float) (bounds.Width/iconSize));
        Cols = Cols == 0 ? 1 : Cols;
        Rows = (int) Math.Ceiling(fileManager.files.Count/(float)Cols);

        for (int i = 0; i < Rows; i++)
            RowDefinitions.Add(new RowDefinition(new GridLength(iconSize)));
        for (int i = 0; i < Cols; i++) 
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        Children.Clear();

        for (int i = 0, r = 0; r < Rows; r++) {
            for (int c = 0; c < Cols && i < attachements.Count; c++, i++) {
                var fv = attachements[i].AttachedLayout;

                SetRow(fv, r);
                SetColumn(fv, c);
                Children.Add(fv);
            }
        }
    }

}