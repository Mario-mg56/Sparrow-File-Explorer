using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Input;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;

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
        private FileManager? FM {get => App.Current.FocusedTab?.fileManager; }

        public SearchContainerViewModel()
        {
            SearchCommand = new RelayCommand(async () =>
            {
                if (FM == null) return;

                if (lastSearch == SearchText)
                {
                    SearchText = string.Empty;
                    lastSearch = string.Empty;

                    FM.CleanSearch();
                    BuscarText = "Buscar";
                    FM.WorkingDirChanged -=  LimpiarSearch;
                    return;
                }

                lastSearch = SearchText;
                BuscarText = "Limpiar";
                
                await FM.SearchWorkingDir(SearchText);
                FM.WorkingDirChanged +=  LimpiarSearch;
            });

        }

        private void LimpiarSearch(DirItem dir)
        {
            BuscarText = "buscar";
            SearchText = string.Empty;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

