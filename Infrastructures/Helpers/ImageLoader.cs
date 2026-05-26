using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace DynamicFileExplorer.Helpers;

public static class ImageLoader
{
    public static Bitmap? LoadImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return null;

        try 
        {
            if (imagePath.StartsWith("avares://"))
            {
                var uri = new Uri(imagePath);
                using var assets = AssetLoader.Open(uri);
                return new Bitmap(assets);
            }
            else
            {
                if (!File.Exists(imagePath)) 
                {
                    Console.WriteLine($"Archivo no encontrado: {imagePath}");
                    return null;
                }

                using var fs = File.OpenRead(imagePath);
                return new Bitmap(fs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar '{imagePath}': {ex.Message}");
            return null; 
        }
    }
}