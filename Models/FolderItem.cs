using System;

namespace DynamicFileExplorer.Models;

class FolderItem : FileSystemItem
{
    public FolderItem(PathFile name) : base(name)
    {
    }
    public FolderItem(string localPath):base(localPath)
    {
    }
}