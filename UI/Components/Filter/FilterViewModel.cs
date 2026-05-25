using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.ViewModels;

public class FilterViewModel : INotifyPropertyChanged
{
    private bool _isAbc = true;
    private bool _isLarge;
    private bool _isAscCreation = true;
    private bool _isAscModify = true;


    public int gridNameIndex{get;set{field=value;OnPropertyChanged();}} = 0;
    public int gridSizeIndex{get;set{field=value;OnPropertyChanged();}} = 1;
    public int gridCreationIndex{get;set{field=value;OnPropertyChanged();}} = 2;
    public int gridModifyIndex{get;set{field=value;OnPropertyChanged();}}= 3;


    public bool IsAbc
    {
        get => _isAbc;
        set
        {
            if (_isAbc != value)
            {
                _isAbc = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OrderName));
            }
        }
    }

    public string OrderName => IsAbc ? "Abc" : "Cba";

    public bool IsLarge
    {
        get => _isLarge;
        set
        {
            if (_isLarge != value)
            {
                _isLarge = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SizeName));
            }
        }
    }

    public string SizeName => IsLarge ? "Large" : "Small";

    public bool IsAscCreation
    {
        get => _isAscCreation;
        set
        {
            if (_isAscCreation != value)
            {
                _isAscCreation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CreationName));
            }
        }
    }

    public string CreationName => IsAscCreation ? "01-30" : "30-01";

    public bool IsAscModify
    {
        get => _isAscModify;
        set
        {
            if (_isAscModify != value)
            {
                _isAscModify = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModifyName));
            }
        }
    }

    public string ModifyName => IsAscModify ? "01-30" : "30-01";

    // ===== Commands =====

    public ICommand ToggleOrderCommand { get; }
    public ICommand ToggleSizeCommand { get; }
    public ICommand ToggleCreationCommand { get; }
    public ICommand ToggleModifyCommand { get; }

    public FilterViewModel()
    {
        ToggleOrderCommand = new RelayCommand(() => IsAbc = !IsAbc);
        ToggleSizeCommand = new RelayCommand(() => IsLarge = !IsLarge);
        ToggleCreationCommand = new RelayCommand(() => IsAscCreation = !IsAscCreation);
        ToggleModifyCommand = new RelayCommand(() => IsAscModify = !IsAscModify);
    }

    // ===== INotifyPropertyChanged =====

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
