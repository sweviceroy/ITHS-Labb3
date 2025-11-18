using System.Windows;
using ITHSLab3.Models;

namespace ITHSLab3.Views
{
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog()
        {
            InitializeComponent();
        }

        // Convenient ctor: pass in the pack we want to edit
        public PackOptionsDialog(QuestionPack pack) : this()
        {
            DataContext = pack;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
