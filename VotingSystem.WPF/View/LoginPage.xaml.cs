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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void LoginGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox)
            {
                PasswordLabel.Visibility = Visibility.Hidden;
            }
            else if (sender is TextBox)
            {
                UsernameLabel.Visibility = Visibility.Hidden;
            }
        }

        private void LoginLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox)
            {
                if (((PasswordBox)sender).Password == "")
                {
                    PasswordLabel.Visibility = Visibility.Visible;
                }
            }
            else if (sender is TextBox)
            {
                if (((TextBox)sender).Text == "")
                {
                    UsernameLabel.Visibility = Visibility.Visible;
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
