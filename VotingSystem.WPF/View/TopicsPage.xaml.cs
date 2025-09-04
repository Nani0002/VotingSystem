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
    /// Interaction logic for TopicsPage.xaml
    /// </summary>
    public partial class TopicsPage : Page
    {
        public TopicsPage(TopicsListViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadTopicsAsync();
        }
    }
}
