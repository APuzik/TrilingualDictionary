using System;
using System.Windows.Input;

namespace MultiDictionaryViewModel.Commands
{
    class RelayCommand : ICommand
    {
        public Action<object> ExecuteAction { get; set; }
        public Predicate<object> CanExecutePredicate { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate != null ? CanExecutePredicate(parameter) : true;
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }
    }
}
