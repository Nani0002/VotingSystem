using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace VotingSystem.WPF.View
{
    public static class EnterKeyBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(EnterKeyBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, ICommand value) =>
            element.SetValue(CommandProperty, value);

        public static ICommand GetCommand(UIElement element) =>
            (ICommand)element.GetValue(CommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                uiElement.KeyDown -= UiElement_KeyDown;
                if (e.NewValue != null)
                    uiElement.KeyDown += UiElement_KeyDown;
            }
        }

        private static void UiElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var uiElement = sender as UIElement;
                var command = GetCommand(uiElement!);
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
