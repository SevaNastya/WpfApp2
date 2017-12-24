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
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для AddChangeWindow.xaml
    /// </summary>
    public partial class AddChangeWindow : Window
    {
        public AddChangeWindow()
        {
            InitializeComponent();
            DataContext = new AddChangeViewModel();
        }
        public AddChangeWindow(Events a)
        {
            InitializeComponent();
            DataContext = new AddChangeViewModel(a);
        }
    }
}
