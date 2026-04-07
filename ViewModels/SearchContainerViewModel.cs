using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.ViewModels
{
    public class SearchContainerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _BuscarText = "Buscar";
        public string BuscarText
        {
            get => _BuscarText;
            set
            {
                if (_BuscarText == value) return;
                _BuscarText = value;
                OnPropertyChanged();
            }
        }
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }

        private string lastSearch = string.Empty;
        private readonly FileManager fm;

        public SearchContainerViewModel()
        {
            fm = App.Current.fileManager;

            SearchCommand = new RelayCommand(async () =>
            {
                if (fm == null) return;

                if (lastSearch == SearchText)
                {
                    SearchText = string.Empty;
                    lastSearch = string.Empty;

                    fm.CleanSearch();
                    BuscarText = "Buscar";
                    return;
                }

                // 👉 Nueva búsqueda
                lastSearch = SearchText;
                BuscarText = "Limpiar";

                await fm.SearchWorkingDir(SearchText);
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
