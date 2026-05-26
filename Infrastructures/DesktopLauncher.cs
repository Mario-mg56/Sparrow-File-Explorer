using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace DynamicFileExplorer.Infrastructures;

public static class DesktopLauncher
{
    public static void OpenWithDesktop(string desktopFilePath, string targetFilePath)
    {
        if (!File.Exists(desktopFilePath))
            throw new FileNotFoundException("Desktop file not found", desktopFilePath);

        if (!File.Exists(targetFilePath))
            throw new FileNotFoundException("Target file not found", targetFilePath);

        var execLine = GetExecLine(desktopFilePath);

        if (string.IsNullOrWhiteSpace(execLine))
            throw new Exception("Exec not found in .desktop file");

        var command = BuildCommand(execLine, targetFilePath);

        Run(command);
    }

    private static string GetExecLine(string desktopPath)
    {
        foreach (var line in File.ReadAllLines(desktopPath))
        {
            if (line.StartsWith("Exec=", StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring("Exec=".Length).Trim();
            }
        }
        return null!;
    }

    private static string BuildCommand(string exec, string filePath)
    {
        exec = exec.Replace("%F", $"\"{filePath}\"");
        exec = exec.Replace("%f", $"\"{filePath}\"");
        exec = exec.Replace("%U", $"\"{filePath}\"");
        exec = exec.Replace("%u", $"\"{filePath}\"");
        exec = exec.Replace("%%", "%");

        exec = Regex.Replace(exec, "%[iucdk]", "");

        return exec;
    }

    private static void Run(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{Escape(command)}\"",
            UseShellExecute = false
        };

        Process.Start(psi);
    }

    private static string Escape(string cmd)
    {
        return cmd.Replace("\"", "\\\"");
    }
}