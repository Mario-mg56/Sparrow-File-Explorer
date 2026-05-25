namespace DynamicFileExplorer.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action? _executeSync;
    private readonly Func<Task>? _executeAsync;

    public RelayCommand(Action execute)
    {
        _executeSync = execute;
    }

    public RelayCommand(Func<Task> execute)
    {
        _executeAsync = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        if (_executeSync != null)
            _executeSync();
        else if (_executeAsync != null)
            await _executeAsync();
    }
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
