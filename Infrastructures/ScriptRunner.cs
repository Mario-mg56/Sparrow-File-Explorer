using System.Diagnostics;

public static class ScriptRunner
{
    /// <summary>
    /// Ejecuta:
    /// script.sh -f argumento
    /// </summary>
    public static Process ExecuteWithFile(string scriptPath, string fileArgument)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = scriptPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("-f");
        psi.ArgumentList.Add(fileArgument);

        Process process = new Process();
        process.StartInfo = psi;

        process.Start();

        return process;
    }


    public static Process ExecuteWithFileAndString(
        string scriptPath,
        string fileArgument,
        string stringArgument)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = scriptPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("-f");
        psi.ArgumentList.Add(fileArgument);

        psi.ArgumentList.Add("-s");
        psi.ArgumentList.Add(stringArgument);

        Process process = new Process();
        process.StartInfo = psi;

        process.Start();

        return process;
    }
}