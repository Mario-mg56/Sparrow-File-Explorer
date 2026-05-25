using System;
using System.Collections.Generic;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Persistence;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.UI.Helpers;

public class IconHelper
{
    public static void SetIcon(FileSystemItem path, string icon, Boolean isForPath)
    {
        if (isForPath)
            SetIconForPath(path, icon);
        else
            SetIconForExtension(path, icon);
            
        PersistenceService.Save(App.Current.Cache);
    }
    
    public static void SetIconForDirs(FileSystemItem path, string icon, Boolean isForPath)
    {
        if (isForPath) SetIconForPath(path, icon);
        if (!isForPath) SetIconForDirs(icon);
    }

    public static void SetIconForDirs(string icon)
    {
        // mario trabaja tonto
    }
    
    public static void SetIconForPath(FileSystemItem file, string iconPath)
    {
        var fullPath = file.GetPath();
        var icons = App.Current.Cache.Config.Icons;
        IconData? targetIcon = null;

        foreach (var icon in icons)
        {
            if (icon.Uri.Contains(fullPath)) icon.Uri.Remove(fullPath);
            if (icon.Image == iconPath) targetIcon = icon;
        }

        if (targetIcon != null)
        {
            if (!targetIcon.Uri.Contains(fullPath)) targetIcon.Uri.Add(fullPath);
        }
        else
        {
            icons.Add(new IconData { Image = iconPath, Uri = [fullPath], Formatos = [] });
        }
    }
    
    public static void SetIconForExtension(FileSystemItem file, string iconPath)
    {
        var ext = file is File f ? f.extension : null; 
        if (string.IsNullOrEmpty(ext)) return;

        var icons = App.Current.Cache.Config.Icons;
        IconData? target = null;

        foreach (var icon in icons)
        {
            icon.Formatos.Remove(ext);
            if (icon.Image == iconPath) target = icon;
        }

        if (target == null)
            icons.Add(new IconData { Image = iconPath, Formatos = [ext] });
        else if (!target.Formatos.Contains(ext))
            target.Formatos.Add(ext);
    }
}