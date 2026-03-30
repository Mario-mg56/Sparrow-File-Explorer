namespace DynamicFileExplorer.Models;

public struct PathFile
{
    public string Path { get; }

    public PathFile(string path)
    {
        Path = path;
    }
}