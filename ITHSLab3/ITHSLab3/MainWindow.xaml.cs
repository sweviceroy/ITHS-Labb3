using System.Windows;
using ITHSLab3.ViewModels;

namespace ITHSLab3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // koppla fönstret till vår "root" viewmodel
            DataContext = new ShellViewModel();
        }
    }
}
