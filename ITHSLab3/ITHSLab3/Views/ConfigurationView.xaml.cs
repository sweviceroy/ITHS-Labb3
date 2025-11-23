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

namespace ITHSLab3.Views
{
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void PackNameTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectListViewItemFromTextBox(sender);
        }

        private void QuestionTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectListViewItemFromTextBox(sender);
        }

        private void SelectListViewItemFromTextBox(object sender)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            var listViewItem = FindAncestor<ListViewItem>(textBox);
            if (listViewItem != null && !listViewItem.IsSelected)
            {
                listViewItem.IsSelected = true;
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T match)
                    return match;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
