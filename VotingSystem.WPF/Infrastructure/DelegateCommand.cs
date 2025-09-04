using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VotingSystem.WPF.Infrastructure
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<Object?> _execute;
        private readonly Func<Object?, Boolean>? _canExecute;

        /// <summary>
        /// Az osztály konstruktora, amely csak végrehajtást definiál.
        /// </summary>
        /// <param name="execute">Az végrehajtandó művelet.</param>
        public DelegateCommand(Action<Object?> execute) : this(null, execute) { }

        /// <summary>
        /// Az osztály konstruktora, amely mind végrehajtást, mind pedig végrehajthatósági feltételt definiál.
        /// </summary>
        /// <param name="canExecute">A végrehajthatósági feltétel.</param>
        /// <param name="execute">Az végrehajtandó művelet.</param>
        public DelegateCommand(Func<Object?, Boolean>? canExecute, Action<Object?> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Esemény, amely jelzi, hogy a parancs végrehajthatósági feltétele megváltozott.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Megadja, hogy a parancs végrehajtható-e.
        /// </summary>
        /// <param name="parameter">A parancshoz tartozó paraméter.</param>
        public Boolean CanExecute(Object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Végrehajtja a parancsot.
        /// </summary>
        /// <param name="parameter">A parancshoz tartozó paraméter.</param>
        public void Execute(Object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        /// <summary>
        /// Kiváltja a CanExecuteChanged eseményt.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
