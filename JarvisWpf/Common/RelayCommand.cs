using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace JarvisWpf.Common
{
    public class RelayCommand<T> : ICommand
    {
        Action<T> ExecuteDelegate;
        Func<bool> CanExecuteDelegate;

        public RelayCommand(Action<T> CommandMethod)
        {
            this.ExecuteDelegate = CommandMethod;
        }

        public RelayCommand(Action<T> CommandMethod, Func<bool> CanCommandExecuteMethod)
        {
            CanExecuteDelegate = CanCommandExecuteMethod;
            ExecuteDelegate = CommandMethod;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate == null)
                return true;
            return CanExecuteDelegate();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged.Invoke(this, new EventArgs());
        }
    }
}
