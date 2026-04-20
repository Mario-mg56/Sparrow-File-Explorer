using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace DynamicFileExplorer.Util;

public class DragController
{
    public readonly Control draggable;
    public bool Dragging {get; private set;} = false;
    private int _draggingTimeTrigger = 0;
    public int DraggingTimeTrigger {
        get => _draggingTimeTrigger;
        set {
            draggingTimer.Interval = TimeSpan.FromMilliseconds(value);
            _draggingTimeTrigger = value;
        }
    }
    public Func<object?, PointerPressedEventArgs, bool>? StartDraggingCondition {get; set;}
    public event Action<Control, object?, PointerPressedEventArgs>? StartDragging;
    public event Action<Control, object?, PointerEventArgs>? Drag;
    public event Action<Control, object?, PointerReleasedEventArgs>? StopDragging;
    public readonly DispatcherTimer draggingTimer;
    private object? _senderStartDraggingBuffer;
    private PointerPressedEventArgs? _eStartDraggingBuffer;
    public DragController(Control draggable)
    {
        this.draggable = draggable;
        draggingTimer = new();

        draggable.PointerPressed += (sender, e) =>
        {
            _senderStartDraggingBuffer = sender;
            _eStartDraggingBuffer = e;
            if (StartDraggingCondition?.Invoke(sender, e) ?? true) draggingTimer.Start();
        };

        draggingTimer.Tick += (_, _) =>
        {
            Dragging = true;
            App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved += OnDrag);
            StartDragging?.Invoke(draggable, _senderStartDraggingBuffer, _eStartDraggingBuffer!);
            draggingTimer.Stop();
        };

        draggable.PointerReleased += (sender, e) =>
        {
            App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
            Dragging = false;
            StopDragging?.Invoke(draggable, sender, e);
        };
    }

    private void OnDrag(object? sender, PointerEventArgs e) => Drag?.Invoke(draggable, sender, e);
}