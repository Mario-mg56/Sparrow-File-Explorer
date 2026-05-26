using System.Collections.Generic;

namespace DynamicFileExplorer.UI.Persistence;

public class Config
{
    public bool DefaultIsHideItems { get; set; } = true;
    public bool DefaultIsExtensionNameIncluded { get; set; } = false;
    public List<IconData> Icons { get; set; } = [];
    public List<OpenToolData> OpenTools { get; set; } = [];
    public List<ContextMenuItemConfig> FileMenuStates { get; set; } = [];
    public List<ContextMenuItemConfig> WDMenuStates { get; set; } = [];
}
public class ContextMenuItemConfig
{
    public string Nombre { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public string ExecutablePath { get; set; } = string.Empty;
    public bool RequiresUserInput { get; set; } = false; 
}
public class IconData
{
    public List<string> Formatos { get; set; } = [];
    public List<string> Uri { get; set; } = [];
    public string Image { get; set; } = "";
}

public class OpenToolData
{
    public List<string> Extensiones { get; set; } = [];

    public List<string> Uri { get; set; } = [];

    public string DesktopApp { get; set; } = "";
}