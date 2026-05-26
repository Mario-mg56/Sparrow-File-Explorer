using System;
using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.Infrastructures.Helpers;

public static class OpenToolHelper
{
    public static void SetTool(FileSystemItem file, string desktopPath, bool isForPath)
    {
        if (isForPath)
            SetToolForPath(file, desktopPath);
        else
            SetToolForExtension(file, desktopPath);

        App.Settings.SaveConfig();
    }

    public static void SetToolForPath(FileSystemItem file, string desktopPath)
    {
        var fullPath = file.GetPath();
        var tools = App.Config.OpenTools;

        OpenToolData? target = null;

        foreach (var tool in tools)
        {
            // quitar override anterior del path
            tool.Uri.Remove(fullPath);

            if (tool.DesktopApp == desktopPath)
                target = tool;
        }

        if (target != null)
        {
            if (!target.Uri.Contains(fullPath))
                target.Uri.Add(fullPath);
        }
        else
        {
            tools.Add(new OpenToolData
            {
                DesktopApp = desktopPath,
                Uri = new List<string> { fullPath },
                Extensiones = new List<string>()
            });
        }
    }

    public static void SetToolForExtension(FileSystemItem file, string desktopPath)
    {
        var ext = file is File f ? f.extension : null;
        if (string.IsNullOrEmpty(ext)) return;

        var tools = App.Config.OpenTools;

        OpenToolData? target = null;

        foreach (var tool in tools)
        {
            tool.Extensiones.Remove(ext);

            if (tool.DesktopApp == desktopPath)
                target = tool;
        }

        if (target == null)
        {
            tools.Add(new OpenToolData
            {
                DesktopApp = desktopPath,
                Extensiones = new List<string> { ext },
                Uri = new List<string>()
            });
        }
        else if (!target.Extensiones.Contains(ext))
        {
            target.Extensiones.Add(ext);
        }
    }

    public static string? ResolveTool(FileSystemItem file)
    {
        var path = file.GetPath();

        var byPath = App.Config.OpenTools
            .FirstOrDefault(t => t.Uri.Contains(path));

        if (byPath != null)
            return byPath.DesktopApp;

        var ext = file is File f ? f.extension : null;

        if (!string.IsNullOrEmpty(ext))
        {
            var byExt = App.Config.OpenTools
                .FirstOrDefault(t => t.Extensiones.Contains(ext));

            if (byExt != null)
                return byExt.DesktopApp;
        }

        return null;
    }
}