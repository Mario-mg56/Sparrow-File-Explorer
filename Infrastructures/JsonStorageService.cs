using System.IO;
using System.Text.Json;

namespace DynamicFileExplorer.Infrastructures;
public class JsonStorageService
{
    protected T Load<T>(string filepath) where T : new()
    {
        if (!File.Exists(filepath))
            return new T();

        var json = File.ReadAllText(filepath);
        return JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    protected void Save<T>(string filepath, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filepath, json);
    }
}
