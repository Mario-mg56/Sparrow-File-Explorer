using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace DynamicFileExplorer.Infrastructures;

public static class AppInstanceLauncher
{
    public static Process Open(AppMode mode, AppUse use = AppUse.Normal)
    {
        var args = $"--mode={mode} --use={use}";

        var psi = new ProcessStartInfo
        {
            FileName = Environment.ProcessPath!,
            Arguments = args,

            UseShellExecute = false,

            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,

            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.Start();
        AppRuntime.RootInput = process.StandardInput;
        return process;
    }

    public static void Send(string tag, string type, string payload)
    {
        var msg = new PipeMessage(tag, type, payload);
        var json = JsonSerializer.Serialize(msg);

        Console.WriteLine(json);
        Console.Out.Flush();
    }

    public static void Listen(Process process, string? tagFilter, Action<PipeMessage> onMessage)
    {
        process.OutputDataReceived += (_, e) =>
        {
            if (string.IsNullOrWhiteSpace(e.Data))
                return;

            try
            {
                var msg = JsonSerializer.Deserialize<PipeMessage>(e.Data);
                if (msg == null) return;

                if (tagFilter != null && msg.Tag != tagFilter)
                    return;

                onMessage(msg);
            }
            catch
            {
                // ignorar ruido no válido
            }
        };

        process.BeginOutputReadLine();
    }
    public static class AppRuntime
    {
        public static StreamWriter? RootInput { get; set; }
        public static Process? RootProcess { get; set; }
    }
}
