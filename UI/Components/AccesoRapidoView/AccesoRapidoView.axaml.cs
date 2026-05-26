using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Persistence;

namespace DynamicFileExplorer.UI.Components
{
    public partial class AccesoRapidoView : UserControl
    {
        private static AccesoRapidoView? _instance;

        public AccesoRapidoView()
        {
            InitializeComponent();
            _instance = this;
            CargarAccesosPersistentes();
        }

        public AccesoRapidoView(List<FileView> componentesFileView)
        {
            InitializeComponent();
            _instance = this;

            var listaContenedor = this.FindControl<StackPanel>("ListaAccesos");
            if (listaContenedor != null && componentesFileView != null)
            {
                foreach (var fileView in componentesFileView)
                {
                    fileView.PointerPressed += OnFileViewClicked;
                    listaContenedor.Children.Add(fileView);
                }
            }
        }

        public static void AddElement(FileSystemItem file)
        {
            if (file == null) return;

            if (_instance != null)
            {
                var listaContenedor = _instance.FindControl<StackPanel>("ListaAccesos");
                if (listaContenedor != null)
                {
                    FileView fileView = new FileView(file, false, true);
                    fileView.PointerPressed += _instance.OnFileViewClicked;
                    listaContenedor.Children.Add(fileView);

                    var config = App.Current.Cache?.Config;
                    var pathString = fileView.controller?.file?.GetPath();

                    if (config != null && !string.IsNullOrEmpty(pathString))
                    {
                        if (!config.AccesoRapidoPaths.Contains(pathString))
                        {
                            config.AccesoRapidoPaths.Add(pathString);
                            PersistenceService.Save(App.Current.Cache);
                        }
                    }
                }
            }
        }

        public static void CargarAccesosPersistentes()
        {
            var config = App.Current.Cache?.Config;
            if (config == null || config.AccesoRapidoPaths == null || _instance == null) return;

            var listaContenedor = _instance.FindControl<StackPanel>("ListaAccesos");
            if (listaContenedor == null) return;

            listaContenedor.Children.Clear();

            foreach (var pathStr in config.AccesoRapidoPaths)
            {
                try
                {
                    var pathModelo = new Models.Path(pathStr);
                    var dirItem = new DirItem(pathModelo);

                    var fileView = new FileView(dirItem, false, true); 
                    fileView.PointerPressed += _instance.OnFileViewClicked;
                    listaContenedor.Children.Add(fileView);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error cargando acceso rápido persistente para {pathStr}: {ex.Message}");
                }
            }
        }

        private void OnFileViewClicked(object? sender, PointerPressedEventArgs e)
        {
            if (sender is FileView fileViewClicado)
            {
                e.Handled = true;

                var pointerProperties = e.GetCurrentPoint(fileViewClicado).Properties;

                if (pointerProperties.IsLeftButtonPressed)
                {
                    MiFuncionDeAccion(fileViewClicado);
                }
                else if (pointerProperties.IsRightButtonPressed)
                {
                    EliminarAccesoRapido(fileViewClicado);
                }
            }
        }

        private void EliminarAccesoRapido(FileView view)
        {
            var listaContenedor = this.FindControl<StackPanel>("ListaAccesos");
            if (listaContenedor == null || view == null) return;

            if (listaContenedor.Children.Contains(view))
            {
                view.PointerPressed -= OnFileViewClicked;
                listaContenedor.Children.Remove(view);
            }

            var config = App.Current.Cache?.Config;
            var pathString = view.controller?.file?.GetPath();

            if (config != null && !string.IsNullOrEmpty(pathString))
            {
                if (config.AccesoRapidoPaths.Contains(pathString))
                {
                    config.AccesoRapidoPaths.Remove(pathString);
                    PersistenceService.Save(App.Current.Cache); 
                }
            }
        }

        private void MiFuncionDeAccion(FileView view)
        {
            if (view?.controller?.file is DirItem directorio)
            {
                App.Current.tabManager.FocusedTab.fileManager.ChangeDirectory(directorio);
            }
        }
    }
}