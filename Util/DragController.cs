using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace DynamicFileExplorer.Util;

public class DragController
{
    public readonly Control draggable;
    public bool Dragging {get; private set;} = false;
    public bool TryingToDrag {get; private set;} = false; //Entre el start y el trigger del timer
    private int _draggingTimeTrigger = 0;
    public int DraggingTimeTrigger {
        get => _draggingTimeTrigger;
        set {
            _draggingTimeTrigger = value;
            draggingTimer.Interval = TimeSpan.FromMilliseconds(value);
        }
    }
    public Func<object?, PointerPressedEventArgs, bool>? StartDraggingCondition {get; set;}
    public event Action<Control, object?, PointerPressedEventArgs>? StartDragging;
    public event Action<Control, object?, PointerEventArgs>? Drag;
    public event Action<Control, object?, PointerReleasedEventArgs>? StopDragging;
    public event Action<Control>? DragFailed;
    public readonly DispatcherTimer draggingTimer;
    private object? _senderStartDraggingBuffer;
    private PointerPressedEventArgs? _eStartDraggingBuffer;
    public DragController(Control draggable)
    {
        this.draggable = draggable;
        draggingTimer = new();

        draggable.PointerPressed += (sender, e) =>
        {
            if (StartDraggingCondition?.Invoke(sender, e) ?? true) {
                _senderStartDraggingBuffer = sender;
                _eStartDraggingBuffer = e;
                draggingTimer.Start();
                TryingToDrag = true;
            }
        };

        draggingTimer.Tick += (_, _) => OnStart();

        draggable.PointerReleased += (sender, e) =>
        {
            draggingTimer.Stop();
            if (!Dragging){
                TryingToDrag = false;
                DragFailed?.Invoke(draggable);
                return;
            }
            OnStop(sender, e);
        };
    }

    private void OnStart()
    {
        TryingToDrag = false;
        Dragging = true;
        StartDragging?.Invoke(draggable, _senderStartDraggingBuffer, _eStartDraggingBuffer!);
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved += OnDrag);
        draggingTimer.Stop();
    }

    private void OnDrag(object? sender, PointerEventArgs e) => Drag?.Invoke(draggable, sender, e);

    private void OnStop(object? sender, PointerReleasedEventArgs e)
    {
        Dragging = false;
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
        StopDragging?.Invoke(draggable, sender, e);
    }

    public void AbortDrag() { //Aborta un drag pendiente
        draggingTimer.Stop();
        Dragging = false;
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
    }
}