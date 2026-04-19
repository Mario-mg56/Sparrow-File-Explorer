using System;
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
    public static readonly int PADDING = 10, DRAGGING_TIME_TRIGGER = 200;
    public static readonly DispatcherTimer draggingTimer = new() {Interval = TimeSpan.FromMilliseconds(DRAGGING_TIME_TRIGGER)};
    public static readonly SolidColorBrush SELECTED_COLOR = new(Colors.LightBlue), TRANSPARENT = new(Colors.Transparent);

    public FileViewController(FileSystemItem file, FileView view)
    {
        this.file = file;
        this.view = view;

        shadowFile = new ShadowItem(new FileView(file, true));

        view.PointerPressed += OnLeftClick;

        draggingTimer.Tick += (_, _) =>
        {
            draggingTimer.Stop();
            Dragging = true;
            OnStartDragging();
            StartDragging?.Invoke(view);
        };
        
        view.PointerReleased += (_, e) => {
            if (e.InitialPressMouseButton == MouseButton.Left) {
                draggingTimer.Stop();
                Dragging = false;
                OnStopDragging();
                StartDragging?.Invoke(view);
            }
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

    private void OnStartDragging()
    {
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved += OnDrag);
        shadowFile.IsVisible = true;
    }

    private void OnDrag(object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(sender as MainWindow);
        shadowFile.SetPosition((int) pos.X, (int) pos.Y);
    }

    private void OnStopDragging()
    {
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
        shadowFile.IsVisible = false;
    }

    public void SetSelected(bool selected){
        Selected = selected;
        view.Background = selected ? SELECTED_COLOR : TRANSPARENT;
    }
}