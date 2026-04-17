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
    public static readonly int COLS = 5;
    public int Rows => (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
    public readonly ContextMenu<DirItem> contextMenu;
    public readonly ContextMenu<FileSystemItem> fileContextMenu;
    private readonly FileManager fileManager = App.Current.fileManager;
    private readonly List<ContextMenu<FileSystemItem>.ContextAttachement> attachements = [];

    public FilesGrid()
    {
        
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo
        
 
 

        this.GetObservable(BoundsProperty).Subscribe(OnResize);

        contextMenu = new (items:[
            new (name: "Crear Carpeta", itemAction: (i, wd, _) => {
                var input = TextInputPopUp.getInstance();    
                input.Show();   
                input.Show();   
                input.Show();   
                input.title = "";
                input.Resolve = (s)=> fileManager.CreateDir(wd,s);
            }),
            new (name: "Crear Archivo", itemAction: (i, wd, _) => {
                var input = TextInputPopUp.getInstance();    
                input.Show();   
                input.title = "";
                input.Resolve = (s)=> fileManager.CreateFile(wd,s);
            }),
            new (name: "Item 3", itemAction: (i, _, _) => Console.WriteLine(i.name + " selected"))
        ]) {Background = Brushes.White};

        fileContextMenu = new ([
            new (name: "Open", itemAction: (i, file, _) => Console.WriteLine("wd " + file)),
            new (name: "Delete", itemAction: (i, file, _) => fileManager.Delete(file)),
            new (name: "Rename", itemAction: (i, file, _) => {
            var input = TextInputPopUp.getInstance();    
            input.Show();
            input.title = file?.path.name;
            input.Resolve = (s)=> fileManager.Rename(file,s);
            }
            )
        ]) {Background = Brushes.White};

        fileManager.WorkingDirChanged += (dir) => {
            RebuildGrid();
            ReloadContextAttachements();
        };
        RebuildGrid();
        ReloadContextAttachements();

    }
    
    private void RebuildGrid()
    {
        Children.Clear();
        OnResize(Bounds);
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        attachements.Clear();

        for (int i = 0, r = 0; r < rows; r++) {
            for (int c = 0; c < COLS && i < fileManager.files.Count; c++, i++) {
                var file = fileManager.files[i];
                var fv = new FileView(file);
                var border = new Border {Child = fv};

                SetRow(border, r);
                SetColumn(border, c);
                Children.Add(border);

                attachements.Add(new ContextMenu<FileSystemItem>.ContextAttachement(fv, fileManager.files[i]));
            }
        }
    }

    private void OnResize (Rect bounds)
    {
        RowDefinitions.Clear();
        ColumnDefinitions.Clear();

        int rows = Rows;
        int cellSize =  (int) Math.Round(bounds.Width/COLS);

        for (int i = 0; i < rows; i++)
            RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
        for (int i = 0; i < COLS; i++) 
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
    }

    private void ReloadContextAttachements()
    {
        contextMenu.SetAttachements([new ContextMenu<DirItem>.ContextAttachement(this, fileManager.WorkingDir)]);
        fileContextMenu.SetAttachements(attachements);
    }

}