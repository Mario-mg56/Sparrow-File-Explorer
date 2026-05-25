using System;
using System.Diagnostics;

namespace DynamicFileExplorer.Infrastructures;

public static class AppInstanceLauncher
{
    public static void Open(AppMode mode)
    {
        var args = $"--mode={mode}";

        var psi = new ProcessStartInfo
        {
            FileName = Environment.ProcessPath,
            UseShellExecute = true,
            Arguments = args
        };

        Process.Start(psi);
    }
}