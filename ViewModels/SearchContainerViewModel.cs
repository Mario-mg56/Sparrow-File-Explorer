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
                    fm.WorkingDirChanged -=  LimpiarSearch;
                    return;
                }

                // 👉 Nueva búsqueda
                lastSearch = SearchText;
                BuscarText = "Limpiar";
                
                await fm.SearchWorkingDir(SearchText);
                fm.WorkingDirChanged +=  LimpiarSearch;
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

