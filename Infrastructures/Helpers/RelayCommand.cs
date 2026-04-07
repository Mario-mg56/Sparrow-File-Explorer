using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicFileExplorer.Infrastructures;
public class RelayCommand(Func<Task> execute) : ICommand
{
    private readonly Func<Task> _execute = execute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        await _execute();
    }
}
