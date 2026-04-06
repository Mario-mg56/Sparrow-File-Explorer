using System;
using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

class FolderNodeViewModel{
    readonly DirItem Yo;
    List<FolderNodeViewModel> Childrens = [];

    public Action? onHierarchyActualzied;
    public FolderNodeViewModel(DirItem yo)
    {
        Yo = yo;

    }
    public FolderNodeViewModel(DirItem yo, List<FolderNodeViewModel> childrens)
    {
        Yo = yo;
        Childrens = childrens;
    }

    public void AddChildren(DirItem item)
    {
        Childrens.Add(new FolderNodeViewModel(item));
        onHierarchyActualzied?.Invoke();
    }
    public void AddChildren(FolderNodeViewModel item)
    {
        Childrens.Add(item);
        onHierarchyActualzied?.Invoke();
    }
    public void AddChildrens(List<FolderNodeViewModel> items)
    {
        items.Where(i=>!Childrens.Contains(i)).ToList().ForEach(Childrens.Add);
        onHierarchyActualzied?.Invoke();
    }

    public List<FolderNodeViewModel> GetChildrens()
    {
        return Childrens;
    }

    public DirItem GetNode()
    {
        return Yo;
    }
}