
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.Util;

namespace DynamicFileExplorer.UI.Components;

public class ResizableBorder : Border
{
    public List<ResizableSide> ResizableSides {get; set;} = [];
    public event Action<ResizableBorder, Rect, Rect>? Resize;
    public DragController? dragController;
    public float ResizingSpace {get; private set;} = 5;
    public ResizableBorder()
    {
        BorderBrush = new SolidColorBrush(Colors.Black);

        List<ResizableSide> dirs = [];
        Point lastPos = new(0, 0);

        dragController = new DragController(this) {
            StartDraggingCondition = (sender, e) => {
                var click = e.GetPosition(sender as Control);
                
                dirs = ResizableSides.Where(side =>
                    sideFuctions[side].resizingCondition(click, Bounds, ResizingSpace)
                ).ToList();

                return dirs.Count > 0;
            }
        };

        dragController.StartDragging += (draggable, sender, e) => lastPos = e.GetPosition(null);

        dragController.Drag += (draggable, sender, e) => {
            var pos = e.GetPosition(null);
            var delta = new Vector(pos.X - lastPos.X, pos.Y - lastPos.Y);
            Rect oldBounds = Bounds, newBounds = Bounds;
            
            dirs.ForEach(dir => newBounds = sideFuctions[dir].resizer(newBounds, delta));

            lastPos = pos;
            
            Resize?.Invoke(this, oldBounds, newBounds);
        };
    }

    public static readonly Dictionary<ResizableSide, ResizeFunctions> sideFuctions = new()
    {
        {ResizableSide.START, 
            new ResizeFunctions(
                (pos, bounds, space) => pos.X <= space,
                (oldBounds, delta) => 
                    new Rect(oldBounds.X + delta.X, oldBounds.Y, oldBounds.Width - delta.X, oldBounds.Height)
            )
        },
        {ResizableSide.TOP, 
            new ResizeFunctions(
                (pos, bounds, space) => pos.Y <= space,
                (oldBounds, delta) => 
                    new Rect(oldBounds.X, oldBounds.Y + delta.Y, oldBounds.Width, oldBounds.Height - delta.Y)
            )
        },
        {ResizableSide.END, 
            new ResizeFunctions(
                (pos, bounds, space) => pos.X >= bounds.Width - space,
                (oldBounds, delta) => 
                    new Rect(oldBounds.X, oldBounds.Y, oldBounds.Width + delta.X, oldBounds.Height)
            )
        },
        {ResizableSide.BOTTOM, 
            new ResizeFunctions(
                (pos, bounds, space) => pos.Y >= bounds.Height - space,
                (oldBounds, delta) => 
                    new Rect(oldBounds.X, oldBounds.Y, oldBounds.Width, oldBounds.Height + delta.Y)
            )
        }
    };
    
    public enum ResizableSide {TOP, START, BOTTOM, END}
    public struct ResizeFunctions (Func<Point, Rect, float, bool> resizingCondition, Func<Rect, Vector, Rect> resizer) {
        public Func<Point, Rect, float, bool> resizingCondition = resizingCondition;
        public Func<Rect, Vector, Rect> resizer = resizer;
    }

    public void SetResizingSpace(float rs)
    {
        if (rs < 0) return;
        ResizingSpace = rs;
        BorderThickness = new Thickness(
            StartResizing ? ResizingSpace : 0,
            TopResizing ? ResizingSpace : 0, 
            EndResizing ? ResizingSpace : 0,
            BottomResizing ? ResizingSpace : 0
        );
    }


    //Propiedades para usar desde AXAML

    public bool StartResizing
    {
        get => GetSide(ResizableSide.START);
        set => SetSide(ResizableSide.START, value);
    }

    public bool TopResizing
    {
        get => GetSide(ResizableSide.TOP);
        set => SetSide(ResizableSide.TOP, value);
    }

    public bool EndResizing
    {
        get => GetSide(ResizableSide.END);
        set => SetSide(ResizableSide.END, value);
    }

    public bool BottomResizing
    {
        get => GetSide(ResizableSide.BOTTOM);
        set => SetSide(ResizableSide.BOTTOM, value);
    }

    private bool GetSide(ResizableSide side) => ResizableSides.Contains(side);
    private void SetSide(ResizableSide side, bool value)
    {
        ResizableSides.Remove(side);
        if (value) ResizableSides.Add(side);
        SetResizingSpace(ResizingSpace);
        
    }
}