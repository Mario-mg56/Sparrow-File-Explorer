using System;
using System.Linq;

public static class AppArguments
{
    public static AppMode Mode { get; private set; } = AppMode.Explorer;
    public static AppUse Use { get; private set; } = AppUse.Normal;

    public static void Parse(string[] args)
    {
        ParseMode(args);
        ParseUse(args);
    }

    private static void ParseMode(string[] args)
    {
        var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode=", StringComparison.OrdinalIgnoreCase));

        if (modeArg == null) return;

        var value = modeArg.Split('=', 2)[1];

        if (Enum.TryParse<AppMode>(value, true, out var mode))
            Mode = mode;
    }

    private static void ParseUse(string[] args)
    {
        var useArg = args.FirstOrDefault(a => a.StartsWith("--use=", StringComparison.OrdinalIgnoreCase));

        if (useArg == null) return;

        var value = useArg.Split('=', 2)[1];

        if (Enum.TryParse<AppUse>(value, true, out var use))
            Use = use;
    }

    public static bool IsThis(AppMode mode) => Mode == mode;
    public static bool IsUse(AppUse use) => Use == use;
}