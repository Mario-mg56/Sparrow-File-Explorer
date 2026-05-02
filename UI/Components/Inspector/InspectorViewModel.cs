using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Avalonia.Media.Imaging;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Helpers;

namespace DynamicFileExplorer.UI.Components.Inspector;

public class InspectorViewModel : INotifyPropertyChanged
{


    public static InspectorViewModel? instance;
    public event PropertyChangedEventHandler? PropertyChanged;



    public bool visibleTamanio
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(visibleTamanio));
        }
    } = true;
    public string fileName
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(fileName));
        }
    } = "";

    public string tipo
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(tipo));
        }
    } = "";

    public string tamanio
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(tamanio));
        }
    } = "";

    public string ruta
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ruta));
        }
    } = "";

    public string lastModification
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(lastModification));
        }
    } = "";
    public bool imageVisibility
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(imageVisibility));
        }
    } = false;

    public Bitmap? imageSource
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(imageSource));
        }
    }
    
    public List<FileSystemItem>? files
    {
        get;
        set
        {
            field = value;
            if(value==null)return;
            if (value?.Count == 0) return;


            if(value.Count == 1)
            {
                var file = value[0];
                fileName = file.path.name;
                tipo = file is Models.File ? "Archivo" : "Carpeta";
                ruta = file.path.path;

                // tamaño correcto (solo si es archivo)
                if (file is Models.File fi)
                {
                    visibleTamanio=true;
                    var size = new FileInfo(file.GetPath()).Length;
                    tamanio = FormatSize(size);
                    var ext = fi.extension;
                    if(AppResources.ImageExtensions.Contains(ext)){
                        imageSource = new Bitmap(file.GetPath());
                        imageVisibility=true;
                        } else{imageVisibility=false;};
                    
                } else if (file is DirItem di)
                {
                    visibleTamanio=true;
                    tamanio = FormatSize(GetDirectorySize(file.GetPath()));
                }

                lastModification = new FileInfo(file.GetPath()).LastWriteTime+"";
            } else
            {
                imageVisibility=false;
                lastModification = "";
                visibleTamanio=true;
                string finalNames = "";
                long finalSize = 0;
                int[] cantidad = [0,0];
                foreach(var file in files)
                {
                    finalNames+=file.path.name+",";
                    cantidad[0] += file is Models.File ? 1:0;
                    cantidad[1] += file is DirItem ? 1:0;

                    finalSize+= file is Models.File? new FileInfo(file.GetPath()).Length:GetDirectorySize(file.GetPath());
                }



                fileName = finalNames;
                tipo = "Archivo: " +cantidad[0] +" Carpeta: " + cantidad[1];
                tamanio = FormatSize(finalSize);
                // ruta = file.path.path; cambiar si se quiere a futuro 
                // tamaño correcto (solo si es archivo)

            }
            OnPropertyChanged(nameof(files));
        }
    }
    public static long GetDirectorySize(string path)
    {
        try
        {
            DirectoryInfo dir = new(path);

            long size = 0;

            foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length;
            }
            return size;

        } catch (UnauthorizedAccessException ){
        }
        return 0;
        

    }
    public static string FormatSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };

        double size = bytes;
        int unit = 0;

        while (size < 1 && unit > 0)
        {
            size *= 1024;
            unit--;
        }

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        while (size < 0.1 && unit > 0)
        {
            size *= 1024;
            unit--;
        }

        return $"{size:F1} {units[unit]}";
    }


    public InspectorViewModel()
    {
        instance = this;
    }
    private void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
