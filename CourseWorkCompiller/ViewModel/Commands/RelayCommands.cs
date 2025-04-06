using System.Windows.Input;
namespace CourseWorkCompiller.ViewModel.Commands;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;

    public RelayCommand(Action<T> execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        if (parameter is T typedParameter)
        {
            _execute(typedParameter);
        }
    }

    public event EventHandler CanExecuteChanged;
}