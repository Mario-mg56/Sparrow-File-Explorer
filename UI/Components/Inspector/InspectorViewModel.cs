using System;
using System.ComponentModel;
using System.IO;
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

    public FileSystemItem? file
    {
        get;
        set
        {
            field = value;
            if (value == null) return;
            if(!file.Exists())return;

            fileName = value.path.name;
            tipo = value is Models.File ? "Archivo" : "Carpeta";
            ruta = value.path.path;

            // tamaño correcto (solo si es archivo)
            if (value is Models.File fi)
            {
                visibleTamanio=true;
                var size = new FileInfo(value.GetPath()).Length;
                tamanio = $"{size / (1024.0 * 1024.0):F2} MB";
                Console.WriteLine(fi.extension);
                var ext = fi.extension;
                if(AppResources.ImageExtensions.Contains(ext)){
                    imageSource = new Bitmap(file.GetPath());
                    imageVisibility=true;
                    } else{imageVisibility=false;};
                
            }
            else
            {
                visibleTamanio=false;
                tamanio = "-";
            }

            lastModification = new FileInfo(value.GetPath()).LastWriteTime+"";

            OnPropertyChanged(nameof(file));
        }
    }

    public InspectorViewModel()
    {
        instance = this;
    }
    private void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
