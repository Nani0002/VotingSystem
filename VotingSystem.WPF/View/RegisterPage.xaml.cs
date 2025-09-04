using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.View
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage(RegisterViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                switch (pb.Name)
                {
                    case "PasswordBox":
                        PasswordLabel.Visibility = Visibility.Hidden;
                        break;
                    case "PasswordConfirmBox":
                        PasswordConfirmLabel.Visibility = Visibility.Hidden;
                        break;
                }
            }
            else if (sender is TextBox tb)
            {
                switch (tb.Name)
                {
                    case "EmailBox":
                        EmailLabel.Visibility = Visibility.Hidden;
                        break;
                    case "NameBox":
                        NameLabel.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private void ElementLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                if (pb.Password == "")
                {
                    switch (pb.Name)
                    {
                        case "PasswordBox":
                            PasswordLabel.Visibility = Visibility.Visible;
                            break;
                        case "PasswordConfirmBox":
                            PasswordConfirmLabel.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
            else if (sender is TextBox tb)
            {
                if (tb.Text == "")
                {
                    switch (tb.Name)
                    {
                        case "EmailBox":
                            EmailLabel.Visibility = Visibility.Visible;
                            break;
                        case "NameBox":
                            NameLabel.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                PasswordBox pb = (PasswordBox)sender;
                switch (pb.Name)
                {
                    case "PasswordBox":
                        vm.Password = pb.Password;
                        break;
                    case "PasswordConfirmBox":
                        vm.PasswordConfirmation = pb.Password;
                        break;
                }
            }
        }
    }
}
