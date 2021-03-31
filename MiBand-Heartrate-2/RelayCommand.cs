using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MiBand_Heartrate_2
{
    public abstract class RelayCommand : ICommand
    {
        public static Dictionary<string, RelayCommand> Commands { get; internal set; }
            = new Dictionary<string, RelayCommand>();

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual event EventHandler CanExecuteChanged;


        public RelayCommand(string name, string description)
        {
            Name = name;
            Description = description;
            Commands[name] = this;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    public class RelayCommand<T> : RelayCommand
    {
        readonly Action<T> _execute = null;

        readonly Predicate<T> _canExecute = null;

        public RelayCommand(string name, string description, Action<T> execute)
            : this(name, description, execute, null) { }

        public RelayCommand(string name, string description, Action<T> execute, Predicate<T> canExecute)
            : base(name, description)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object param)
        {
            return _canExecute == null ? true : _canExecute((T)param);
        }

        public override event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public override void Execute(object param)
        {
            _execute((T)param);
        }
    }
}