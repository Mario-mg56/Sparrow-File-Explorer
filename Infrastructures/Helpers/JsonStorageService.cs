using System.IO;
using System.Text.Json;

namespace DynamicFileExplorer.Helpers;
public class JsonStorageService
{
    protected static readonly JsonSerializerOptions serializer = new(){WriteIndented = true};
    protected static T? Load<T>(string filepath) where T : new()
    {
        if (!File.Exists(filepath)) return new T();

        var json = File.ReadAllText(filepath);
        return JsonSerializer.Deserialize<T>(json);
    }

    protected static void Save<T>(string filepath, T data)
    {
        var json = JsonSerializer.Serialize(data, serializer);
        File.WriteAllText(filepath, json);
    }
}
