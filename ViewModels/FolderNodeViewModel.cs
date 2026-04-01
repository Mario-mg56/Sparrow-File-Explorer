using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

class FolderNodeViewModel{
    FolderItem Yo;
    List<FolderNodeViewModel> Childrens = [];
    public FolderNodeViewModel(FolderItem yo)
    {
        Yo = yo;

    }
    public FolderNodeViewModel(FolderItem yo, List<FolderNodeViewModel> childrens)
    {
        Yo = yo;
        Childrens = childrens;
    }

    public void AddChildren(FolderItem item)
    {
        Childrens.Add(new FolderNodeViewModel(item));
    }
    public void AddChildren(FolderNodeViewModel item)
    {
        Childrens.Add(item);
    }
    public void AddChildrens(List<FolderNodeViewModel> items)
    {
        items.Where(i=>!Childrens.Contains(i)).ToList().ForEach(Childrens.Add);
    }

    public List<FolderNodeViewModel> GetChildrens()
    {
        return Childrens;
    }

    public FolderItem GetNode()
    {
        return Yo;
    }
}