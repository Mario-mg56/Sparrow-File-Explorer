namespace DynamicFileExplorer.UI.Persistence;

public class Persistence() {
    public required CacheData Cache {get; init;}
    public required Styles Style {get; init;}
    public required Config Config {get; init;}
    public required KeyBindings KeyBindings {get; init;}
    public required CustomTheme[] Themes {get; set;}
    
}