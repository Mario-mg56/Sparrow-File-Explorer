using System.Collections.Generic;

public class Config
{
    public bool DefaultIsHideItems = true;
    public bool DefaultIsExtensionNameIncluded = false;

    public string CurrentTheme = "LightTheme";
    public List<IconData> Icons { get; set; } = [];
    public List<OpenToolData> OpenTools { get; set; } = [];
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