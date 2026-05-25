using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using static DynamicFileExplorer.Util.Util;

namespace DynamicFileExplorer.UI.Components;

public class FileLayoutController
{
    private readonly List<ContextMenu<FileSystemItem>.ContextAttachement> attachements = [];
    public readonly Control fileLayout;
    private FileManager? _fm;
    public FileManager? FileManager {get => _fm; set => SetFileManager(value);}
    public FileView? PointingFile {get; private set;}
    public readonly List<FileView> fileViews = [];
    public readonly List<FileView> selectedFiles = [];
    public ContextMenu<DirItem>? contextMenu;
    public readonly FileSelector fileSelector;
    public readonly DispatcherTimer openFileTimer = new() //Controla el intervalo de tiempo que tiene un usuario para abrir un archivo
         {Interval = TimeSpan.FromMilliseconds(FileViewController.OPEN_FILE_MAX_CLICK_INTERVAL)};
    public event Action<FileView?>? PointingFileChanged;
    public event Action? SelectedFilesChanged;
    private FileView? lastFileSelected;
    public static readonly Key MULTIPLE_SELECTION_KEY = Key.LeftCtrl;
    public FileLayoutController(Control fileLayout)
    {
        App.Current.UIManager.FilesLayoutController = this;
        this.fileLayout = fileLayout;

        fileLayout.PointerPressed += (sender, e) => {
            if(PointingFile == null) {
                selectedFiles.Clear();
                SelectedFilesChanged?.Invoke();
            }
        };

        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => 
            mw.PointerMoved += (sender, e) => {
                var clickPoint = e.GetPosition(fileLayout);
                FileView? pf = null;
                fileViews.ForEach((fv) => {
                    if (fv.Bounds.Contains(clickPoint)) {
                        pf = fv;
                        return;
                    }
                });
                SetPointingFile(pf);
            }
        );

        fileSelector = new(fileLayout)
            {StartSelectingCondition = (_, e) => PointingFile == null}; //No cuenta si arrastra un file;
        fileSelector.Selecting += OnDragSelection;

        SelectedFilesChanged += RefreshFileSelection;
        SelectedFilesChanged += OpenInspector;

        openFileTimer.Tick += (_, _) => {
            lastFileSelected = null;
            openFileTimer.Stop();
        };
    }
    
    
    private Action<DirItem>? WDChangedBuffer;
    public void SetFileManager(FileManager? newFileManager)
    {
        if(FileManager != null && WDChangedBuffer != null)
            FileManager.WorkingDirChanged -= WDChangedBuffer;

        _fm = newFileManager;

        if (FileManager == null)
        {
            fileViews.Clear();
            attachements.Clear();
            selectedFiles.Clear();
            (fileLayout as IFileLayout)!.SetFileViews([]);
            return;
        }

        contextMenu = MakeWDContextMenu(FileManager);
        
        WDChangedBuffer = _ => ReloadFiles(FileManager);
        FileManager.WorkingDirChanged += WDChangedBuffer;
        ReloadFiles(FileManager);
    }

    private void ReloadFiles(FileManager fm)
    {
        fileViews.Clear();
        attachements.Clear();
        selectedFiles.Clear();
        var fileContextMenu = FileView.MakeFileContextMenu(fm);
        foreach(var f in fm.files)
        {
            var fv = new FileView(f);
            SetUpFileView(fv);
            fileViews.Add(fv);
        }
        (fileLayout as IFileLayout)!.SetFileViews(fileViews);
        fileContextMenu.SetAttachements(attachements);
        contextMenu!.SetAttachements([new ContextMenu<DirItem>.ContextAttachement(fileLayout, fm.WorkingDir)]);
    }

    private void SetUpFileView(FileView fv)
    {
        fv.PointerPressed += (sender, e) =>
        {
            if (e.GetCurrentPoint((Visual)sender!).Properties.
                PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                OnSelect((sender as FileView)!, App.Current.UIManager.GetKeyEvent(MULTIPLE_SELECTION_KEY).IsPressed);
        };
        fv.PointerReleased += (sender, e) =>
        {//Solo abrimos al soltar para que no interfiera con el Drag
            if (e.GetCurrentPoint(sender as Visual).Properties.
                PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                TryOpenFile((sender as FileView)!);
        };
        attachements.Add(new ContextMenu<FileSystemItem>.ContextAttachement(fv, fv.controller.file));
    }

    private void OnSelect(FileView fv, bool multipleSelection = false)
    {
        if (!multipleSelection) selectedFiles.Clear();
        else if(selectedFiles.Contains(fv)) { //Selección multiple de un ya seleccionado
            selectedFiles.Remove(fv);
            SelectedFilesChanged?.Invoke();
            return;
        }
        selectedFiles.Add(fv);
        SelectedFilesChanged?.Invoke();
    }

    private void TryOpenFile(FileView fv)
    {
        openFileTimer.Stop();
        if (lastFileSelected != null && lastFileSelected == fv)
        {
            lastFileSelected = null;
            if (fv.controller.Dragging) return;
            fv.controller.dragController.AbortDrag(); //En caso de que DRAGGING_TIME_TRIGGER > OPEN_FILE_MAX_CLICK_INTERVAL
            FileManager?.Open(fv.controller.file);
        }
        else {
            lastFileSelected = fv;
            openFileTimer.Start();
        }
    }

    private void OnDragSelection(Point _, Point _2)
    {  
        if(App.Current.UIManager.MainWindow == null) return;

        Point fsStart, fsEnd;
        (fsStart, fsEnd) = (fileSelector.Bounds.TopLeft, fileSelector.Bounds.BottomRight);

        var selected = new HashSet<FileView>(fileViews.Where(fv => {
            var absPos = fv.TranslatePoint(new Point(0, 0), App.Current.UIManager.MainWindow) ?? new(0, 0);
            return IsSquareTouchingSquareNormalizedPos(fsStart, fsEnd, absPos,
                 new Point(absPos.X+fv.Bounds.Width, absPos.Y+fv.Bounds.Height));
        }));

        if (new HashSet<FileView>(selectedFiles).SetEquals(selected)) return; //No se ha modificado la selección
        selectedFiles.Clear();
        selectedFiles.AddRange(selected);
        SelectedFilesChanged?.Invoke();
    }

    private void SetPointingFile(FileView? pf)
    {
        if (PointingFile == pf) return;
        PointingFile = pf;
        PointingFileChanged?.Invoke(PointingFile);
        RefreshFileSelection();
        PointingFile?.Background = FileView.HoverColor;
    }
    
    public static int i = 1 ;

    public void RefreshFileSelection() {
        fileViews.ForEach(fv => fv.controller.SetSelected(false));
        selectedFiles.ForEach(fv => fv.controller.SetSelected(true));
    }

    private void OpenInspector()
    {
        var draggingFile = selectedFiles.Find(f =>
            f.controller.dragController.TryingToDrag || f.controller.dragController.Dragging
        );
        void ShowInspector() =>
                App.Current.UIManager.LoadFileInfo(selectedFiles.Select(f => f.controller.file).ToList());

        if (draggingFile == null) {
            ShowInspector();
            return;
        }

        void ShowAndClean()
        {
            ShowInspector();
            draggingFile.controller.dragController.DragFailed -= ShowInspectorIfDragFails;
            draggingFile.controller.dragController.StopDragging -= ShowInspectorOnStopDragging;
        }

        void ShowInspectorIfDragFails(object _) => ShowAndClean();
        void ShowInspectorOnStopDragging(object _, object? _1, object _2) => ShowAndClean();

        draggingFile.controller.dragController.DragFailed += ShowInspectorIfDragFails;
        draggingFile.controller.dragController.StopDragging += ShowInspectorOnStopDragging;
    }

    public static ContextMenu<DirItem> MakeWDContextMenu(FileManager fm) => new ([
        new (name: "Crear Carpeta", itemAction: (i, wd, _) => {
            var input = TextInputPopUp.getInstance();    
            input.Show((s,_)=>fm.CreateDir(wd!, s));   
        }),
        new (name: "Crear Archivo", itemAction: (i, wd, _) => {
            var input = TextInputPopUp.getInstance();    
            input.Show((s,_)=>fm.CreateFile(wd!, s));   
        }),
        new (name: "Item 3", itemAction: (i, _, _) => Console.WriteLine(i.name + " selected"))
    ]);
}