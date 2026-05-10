using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;

namespace DynamicFileExplorer.UI.Components;

public class ResizableGrid : Grid
{
    private readonly List<ResizableBorder> ResizableItems = [];

    public ResizableGrid()
    {
        Children.CollectionChanged += OnChangeChildren;
    }

    private void OnResize(ResizableBorder sender, Rect oldBounds, Rect newBounds)
    {
        int row = GetRow(sender), col = GetColumn(sender);
        
        //START 
        if (oldBounds.X != newBounds.X && col != 0) {
            var neighborCol = ColumnDefinitions[col-1];
            double delta = newBounds.X - oldBounds.X;

            double neighborWidth = neighborCol.ActualWidth + delta;
            double currentWidth = ColumnDefinitions[col].ActualWidth - delta;

            if (neighborWidth < GetMinWidth(col - 1) || currentWidth < GetMinWidth(col)) return;

            neighborCol.Width = new GridLength(neighborWidth);
            ColumnDefinitions[col].Width = new GridLength(currentWidth);
        }
        //END
        else if (oldBounds.Width != newBounds.Width && col != ColumnDefinitions.Count-1) {
            var neighborCol = ColumnDefinitions[col+1];
            double delta = newBounds.Width - oldBounds.Width;

            double neighborWidth = neighborCol.ActualWidth - delta;
            double currentWidth = ColumnDefinitions[col].ActualWidth + delta;

            if (neighborWidth < GetMinWidth(col + 1) || currentWidth < GetMinWidth(col)) return;

            neighborCol.Width = new GridLength(neighborWidth);
            ColumnDefinitions[col].Width = new GridLength(currentWidth);
        }
        //TOP
        if (oldBounds.Y != newBounds.Y && row != 0) {
            var neighborRow = RowDefinitions[row-1];
            double delta = newBounds.Y - oldBounds.Y;

            double neighborHeight = neighborRow.ActualHeight + delta;
            double currentHeight = RowDefinitions[row].ActualHeight - delta;

            if (neighborHeight < GetMinHeight(row - 1) || currentHeight < GetMinHeight(row)) return;

            neighborRow.Height = new GridLength(neighborHeight);
            RowDefinitions[row].Height = new GridLength(currentHeight);
        }
        //BOTTOM
        else if (oldBounds.Height != newBounds.Height && row != RowDefinitions.Count-1) {
            var neighborRow = RowDefinitions[row+1];
            double delta = newBounds.Height - oldBounds.Height;

            double neighborHeight = neighborRow.ActualHeight - delta;
            double currentHeight = RowDefinitions[row].ActualHeight + delta;

            if (neighborHeight < GetMinHeight(row + 1) || currentHeight < GetMinHeight(row)) return;

            neighborRow.Height = new GridLength(neighborHeight);
            RowDefinitions[row].Height = new GridLength(currentHeight);
        }
    }

    private void OnChangeChildren(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ResizableItems.ForEach(r => r.Resize -= OnResize);
        ResizableItems.Clear();
        foreach (Control c in Children)
        {
            c.MinWidth = Math.Max(c.MinWidth, 1);
            c.MinHeight = Math.Max(c.MinHeight, 1);
            if (c is ResizableBorder r) {
                ResizableItems.Add(r);
                r.Resize += OnResize;
            }
        }
    }

    private double GetMinWidth(int col)
    {
        double min = 0;
        foreach (Control c in Children) 
            if (GetColumn(c) == col) min = Math.Max(min, c.MinWidth);
        return min;
    }

    private double GetMinHeight(int row)
    {
        double min = 0;
        foreach (Control c in Children) 
            if (GetRow(c) == row) min = Math.Max(min, c.MinHeight);
        return min;
    }
}