using System;
using System.Linq;

public static class AppArguments
{
    public static AppMode Mode { get; private set; } = AppMode.Explorer;

    public static void Parse(string[] args)
    {
        var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode="));

        if (modeArg == null)
            return;

        var value = modeArg.Split('=')[1];

        if (Enum.TryParse<AppMode>(value, true, out var mode))
            Mode = mode;
    }
}