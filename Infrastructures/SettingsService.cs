using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DynamicFileExplorer.Infrastructures;

public class SettingsService : JsonStorageService
{
    private static string PersistencePath{get;} = "../../../Config";
    public string StylePath {get;}= Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, PersistencePath,"Styles.json"
    ));

    public Dictionary<string, Styles> AllStyles { get; private set; } = new();
    public Styles CurrentStyle { get; private set; } = new();
    public string ConfigPath {get;}= Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, PersistencePath,"Config.json"
    ));

    public Config Config { get; private set; } = new();

    public string KeyBindingsPath {get;}= Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, PersistencePath,"KeyBindings.json"
    ));

    public KeyBindings KeyBindings { get; private set; } = new();

    public void Load()
    {
        AllStyles = Load<Dictionary<string, Styles>>(StylePath);
        Config = Load<Config>(ConfigPath);
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


    // public void SaveStyles()
    // {
    //     var json = JsonSerializer.Serialize(Styles, new JsonSerializerOptions
    //     {
    //         WriteIndented = true
    //     });

    //     File.WriteAllText(StylePath, json);
    // }
}
