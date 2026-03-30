using System;

namespace DynamicFileExplorer.Models;

class FileItem : FileSystemItem
{
    String Extension;
    public FileItem(PathFile name) : base(name)
    {
        Extension = System.IO.Path.GetExtension(name.Path);
    }

    public override string GetFullPath()
    {
        return Path+Name+Extension;
    }
}