using Avalonia.Controls;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components
{
    public partial class ContextSettingsView : UserControl
    {
        public ContextSettingsView()
        {
            InitializeComponent();
            
            // Asignamos el DataContext para que los {Binding} funcionen
            DataContext = new ContextSettingsViewModel();
        }
    }
}