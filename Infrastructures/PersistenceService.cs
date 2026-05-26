using System;
using System.Collections.Generic;
using System.IO;
using DynamicFileExplorer.UI.Persistence;
using static System.IO.Path;

namespace DynamicFileExplorer.Helpers;

public class PersistenceService : JsonStorageService
{
    public static readonly string
        PersistencePath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SparrowFileExplorer"),
        ThemesPath = Combine(PersistencePath, "Themes"),
        CachePath = Combine(PersistencePath, "cache.json"),
        StylesPath = Combine(PersistencePath, "styles.json"),
        ConfigPath = Combine(PersistencePath, "config.json"),
        KeyBindingsPath = Combine(PersistencePath, "bindings.json")
    ;

    static PersistenceService()
    {
        EnsureDirectories();
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(PersistencePath)) 
            Directory.CreateDirectory(PersistencePath);
        if (!Directory.Exists(ThemesPath)) 
            Directory.CreateDirectory(ThemesPath);
    }
    public static Persistence Load()
    {
        EnsureDirectories();

        return new Persistence  {
            Cache = Load<CacheData>(CachePath) ?? new(),
            Style = Load<Styles>(StylesPath) ?? new(),
            Config = Load<Config>(ConfigPath) ?? new(),
            KeyBindings = Load<KeyBindings>(KeyBindingsPath) ?? new(),
            Themes = [.. LoadThemes()]
        };

    }

    public static void Save(Persistence data)
    {
        System.Console.WriteLine(data.Config.WDMenuStates.Count);
        EnsureDirectories();

        Save(CachePath, data.Cache);
        Save(StylesPath, data.Style);
        Save(ConfigPath, data.Config);
        Save(KeyBindingsPath, data.KeyBindings);

        foreach (var theme in data.Themes) SaveTheme(theme);
    }

    private static List<CustomTheme> LoadThemes()
    {
        var themes = new List<CustomTheme>();
        foreach (var file in Directory.GetFiles(ThemesPath, "*.json")){
            var theme = Load<CustomTheme>(file);
            if (theme != null) themes.Add(theme);
        }
        if (themes.Count == 0) return [new()];
        return themes;
    }

    private static void SaveTheme(CustomTheme theme)
    {
        //Convertimos a _ carácteres inválidos
        string safeName = string.Join("_", theme.Name.Split(GetInvalidFileNameChars()));
        Save(Combine(ThemesPath, $"{safeName}.json"), theme);
    }
}