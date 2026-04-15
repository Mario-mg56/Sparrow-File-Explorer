using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

public class SettingsService
{
    public string StylePath {get;}= Path.GetFullPath(Path.Combine(
    AppContext.BaseDirectory,
    "../../../Assets/Persistence/Styles.json"
    ));

    public Dictionary<string, Styles> AllStyles { get; private set; } = new();
    public Styles CurrentStyle { get; private set; } = new();
    public string ConfigPath {get;}= Path.GetFullPath(Path.Combine(
    AppContext.BaseDirectory,
    "../../../Assets/Persistence/Config.json"
    ));

    public Config Config { get; private set; } = new();

    

    public void Load()
    {
        AllStyles = Load(StylePath,AllStyles);
        Config = Load(ConfigPath,Config);
         if (AllStyles.TryGetValue(Config.CurrentTheme, out var theme))
        {
            CurrentStyle = theme;
        }
        else
        {
            Console.WriteLine($"Tema '{Config.CurrentTheme}' no encontrado, usando fallback");

            CurrentStyle = AllStyles.Values.FirstOrDefault() ?? new Styles();
        }
    }
    private T Load<T>(string filepath, T tipo) where T : new()
    {
        if (!File.Exists(filepath))
        {
            Console.WriteLine("no funciono");
            return new T();
        }

        var json = File.ReadAllText(filepath);


        using var doc = JsonDocument.Parse(json);
        var jsonProps = doc.RootElement.EnumerateObject()
            .Select(p => p.Name)
            .ToHashSet();

        // 2. Propiedades del modelo
        var modelProps = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet();

        // 3. Detectar diferencias
        var extra = jsonProps.Except(modelProps);
        var missing = modelProps.Except(jsonProps);

        foreach (var e in extra)
            Console.WriteLine($"⚠️ JSON tiene propiedad extra: {e}");

        foreach (var m in missing)
            Console.WriteLine($"⚠️ Falta propiedad en JSON: {m}");

        return JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    // public void SaveStyles()
    // {
    //     var json = JsonSerializer.Serialize(Styles, new JsonSerializerOptions
    //     {
    //         WriteIndented = true
    //     });

    //     File.WriteAllText(StylePath, json);
    // }
}
