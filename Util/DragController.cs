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
            _draggingTimeTrigger = value;
            draggingTimer.Interval = TimeSpan.FromMilliseconds(value);
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
            if (StartDraggingCondition?.Invoke(sender, e) ?? true) {
                _senderStartDraggingBuffer = sender;
                _eStartDraggingBuffer = e;
                if (_draggingTimeTrigger != 0) draggingTimer.Start();
                else OnStart();
            }
        };

        draggingTimer.Tick += (_, _) => OnStart();

        draggable.PointerReleased += (sender, e) =>
        {
            draggingTimer.Stop();
            if (!Dragging) return;
            App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved -= OnDrag);
            Dragging = false;
            StopDragging?.Invoke(draggable, sender, e);
        };
    }

    private void OnStart()
    {
        Dragging = true;
        StartDragging?.Invoke(draggable, _senderStartDraggingBuffer, _eStartDraggingBuffer!);
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.PointerMoved += OnDrag);
        draggingTimer.Stop();
    }

    private void OnDrag(object? sender, PointerEventArgs e) => Drag?.Invoke(draggable, sender, e);

    public void AbortDrag() => draggingTimer.Stop(); //Aborta un drag pendiente
}