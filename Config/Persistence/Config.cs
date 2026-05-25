using System.Collections.Generic;

namespace DynamicFileExplorer.UI.Persistence;

public class Config
{
    public bool DefaultIsHideItems { get; set; } = true;
    public bool DefaultIsExtensionNameIncluded { get; set; } = false;
    public List<IconData> Icons { get; set; } = [];
}

public class IconData
{
    public List<string> Formatos { get; set; } = [];
    public List<string> Uri { get; set; } = [];
    public string Image { get; set; } = "";
}