using System;
using System.Collections.ObjectModel;
using System.Linq; 
using System.Windows.Input;
using Avalonia.Threading;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.UI.Persistence;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.ViewModels;
    public class ContextSettingsViewModel
    {
        public ObservableCollection<ContextSettingItem> PrimeraColeccion { get; set; }
        public ObservableCollection<ContextSettingItem> SegundaColeccion { get; set; }
        
        public ICommand CrearCustomFileItemCommand { get; }
        public ICommand CrearCustomWDItemCommand { get; }
        public ICommand BorrarContextItemCommand { get; }

        public ContextSettingsViewModel()
        {
            var controller = App.Current.UIManager.FilesLayoutController;
            var config = App.Current.Cache?.Config;

            // --- CORRECCIÓN EN ARCHIVOS ---
            // Leemos los ítems que ya pasaron por el filtro y la inyección del JSON
            var itemsArchivos = FileLayoutController.contextMenuFile?.items ?? [];
            PrimeraColeccion = new ObservableCollection<ContextSettingItem>(
                itemsArchivos.Select(item => {
                    var guardado = config?.FileMenuStates.FirstOrDefault(x => x.Nombre == item.name);
                    return new ContextSettingItem(
                        item, 
                        guardado?.ExecutablePath ?? "", 
                        guardado?.RequiresUserInput ?? false, 
                        !string.IsNullOrEmpty(guardado?.ExecutablePath),
                        onChanged: OnCrearYGuardarContextItem
                    );
                })
            );

            // --- CORRECCIÓN CRÍTICA EN WORKING DIRECTORY ---
            // Si el controlador o su gestor de archivos no están listos, usamos una lista temporal base.
            // Al invocar directamente a FileLayoutController.MakeWDContextMenu aseguramos que se procesen
            // tanto los comandos nativos como los guardados previamente en el JSON.
            var currentFileManager = controller?.FileManager;
            var itemsDirectorio = currentFileManager != null 
                ? (FileLayoutController.MakeWDContextMenu(currentFileManager)?.items ?? [])
                : [];

            SegundaColeccion = new ObservableCollection<ContextSettingItem>(
                itemsDirectorio.Select(item => {
                    var guardado = config?.WDMenuStates.FirstOrDefault(x => x.Nombre == item.name);
                    return new ContextSettingItem(
                        item, 
                        guardado?.ExecutablePath ?? "", 
                        guardado?.RequiresUserInput ?? false, 
                        !string.IsNullOrEmpty(guardado?.ExecutablePath),
                        onChanged: OnCrearYGuardarContextItem
                    );
                })
            );

            CrearCustomFileItemCommand = new RelayCommand(() => LanzarSelectorDeEjecutable(esParaArchivo: true));
            CrearCustomWDItemCommand = new RelayCommand(() => LanzarSelectorDeEjecutable(esParaArchivo: false));
            BorrarContextItemCommand = new RelayCommand<ContextSettingItem>(OnBorrarContextItem);
        }

        private void OnBorrarContextItem(ContextSettingItem item)
        {
            if (item == null) return;

            if (PrimeraColeccion.Contains(item))
            {
                PrimeraColeccion.Remove(item);
            }
            else if (SegundaColeccion.Contains(item))
            {
                SegundaColeccion.Remove(item);
            }

            OnCrearYGuardarContextItem();
        }

        private void LanzarSelectorDeEjecutable(bool esParaArchivo)
        {
            var process = AppInstanceLauncher.Open(AppMode.MiniExplorer, AppUse.CreateContextMenu);

            AppInstanceLauncher.Listen(process, "OpenWith", msg =>
            {
                if (msg.Type != "apply") return;
                string executablePath = msg.Payload;

                Dispatcher.UIThread.Post(() =>
                {
                    var inputPopUp = TextInputPopUp.getInstance();
                    inputPopUp.Show(_resolve:
                        (nombreItem, requiereInput) =>
                        {
                            if (string.IsNullOrWhiteSpace(nombreItem)) return;

                            var baseItem = new ContextMenuItem(name: nombreItem) { isActive = true };
                            var nuevoItemConfigurado = new ContextSettingItem(
                                originalItem: baseItem,
                                execPath: executablePath,
                                requiresInput: requiereInput,
                                isCustom: true,
                                onChanged: OnCrearYGuardarContextItem
                            );

                            if (esParaArchivo) PrimeraColeccion.Add(nuevoItemConfigurado);
                            else SegundaColeccion.Add(nuevoItemConfigurado);

                            OnCrearYGuardarContextItem();
                        },
                        _title: "Configurar nuevo comando",
                        _watermark: "Nombre del ítem",
                        _checkboxText: "Requiere entrada de usuario"
                    );
                });
            });
        }

        private void OnCrearYGuardarContextItem()
        {
            var currentPersistence = App.Current.Cache;
            if (currentPersistence?.Config == null) return;

            // 1. Volcamos las colecciones de la UI a las listas de configuración estructuradas
            currentPersistence.Config.FileMenuStates = PrimeraColeccion.Select(x => new ContextMenuItemConfig {
                Nombre = x.Nombre,
                IsActive = x.DynamicValue,
                ExecutablePath = x.ExecutablePath,
                RequiresUserInput = x.RequiresUserInput
            }).ToList();

            currentPersistence.Config.WDMenuStates = SegundaColeccion.Select(x => new ContextMenuItemConfig {
                Nombre = x.Nombre,
                IsActive = x.DynamicValue,
                ExecutablePath = x.ExecutablePath,
                RequiresUserInput = x.RequiresUserInput
            }).ToList();

            // 2. Persistimos los datos físicamente en el JSON del disco
            PersistenceService.Save(currentPersistence);

            // 3. REFRESH EN TIEMPO REAL DEL MENÚ DEL EXPLORADOR
            // Forzamos al controlador del layout actual a reconstruir su menú contextual con los datos recién guardados.
            var controller = App.Current.UIManager.FilesLayoutController;
            if (controller != null && controller.FileManager != null)
            {
                Dispatcher.UIThread.Post(() => {
                    controller.contextMenu = FileLayoutController.MakeWDContextMenu(controller.FileManager);
                });
            }
        }
    }
public class ContextSettingItem
{
    // Cambiamos a la firma base que comparten las variantes genéricas y no genéricas
    public ContextMenuItem OriginalItem { get; }
    public string Nombre => OriginalItem.name;
    
    private readonly Action? _onChanged;

    public bool DynamicValue
    {
        get => OriginalItem.isActive;
        set
        {
            if (OriginalItem.isActive != value)
            {
                OriginalItem.isActive = value;
                _onChanged?.Invoke();
            }
        }
    }

    private string _executablePath = string.Empty;
    public string ExecutablePath 
    { 
        get => _executablePath; 
        set 
        {
            if (_executablePath != value)
            {
                _executablePath = value;
                _onChanged?.Invoke();
            }
        }
    }

    private bool _requiresUserInput = false;
    public bool RequiresUserInput 
    { 
        get => _requiresUserInput; 
        set 
        {
            if (_requiresUserInput != value)
            {
                _requiresUserInput = value;
                _onChanged?.Invoke();
            }
        }
    }

    public bool IsCustom { get; }

    // El parámetro originalItem ahora acepta implícitamente tanto ContextMenuItem como ContextMenuItem<DirItem>
    public ContextSettingItem(ContextMenuItem originalItem, string execPath = "", bool requiresInput = false, bool isCustom = false, Action? onChanged = null)
    {
        OriginalItem = originalItem;
        _executablePath = execPath;
        _requiresUserInput = requiresInput;
        IsCustom = isCustom;
        _onChanged = onChanged;
    }
}

    // RelayCommand básico sin parámetros
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
    }

    // NUEVO: Sobrecarga de RelayCommand genérico para soportar CommandParameter
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        public RelayCommand(Action<T> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute((T)parameter!);
        public event EventHandler? CanExecuteChanged;
    }