using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.ViewModels;

class HierarchyViewModel
{
    FolderNodeViewModel? Root;

    public HierarchyViewModel()
    {
        List<FolderItem> fathersAndSon = FileManager.GetInstance().GetAllFathers();
        fathersAndSon.Reverse();
        FolderNodeViewModel? actualNode = null;
        foreach (FolderItem folder in fathersAndSon)
        {
            if (actualNode == null)
            {
                Root = new FolderNodeViewModel(folder);
                actualNode = Root;
            } else
            {
                FolderNodeViewModel tempNode = new (folder);
                actualNode.AddChildren(tempNode);
                actualNode = tempNode;
            }
            
        }
    }

    public void ExpandNode(FolderNodeViewModel folder)
    {
        List<FolderNodeViewModel> nodes = FileManager.GetInstance()
                            .ListAllDirectories(folder.GetNode())
                            .Select(i=>new FolderNodeViewModel(i))
                            .ToList();
        folder.AddChildrens(nodes);
    }

}