using System;
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
            // DataContext should be the QuestionPack we are editing
            if (DataContext is QuestionPack pack)
            {
                int seconds;

                // Read user input, default to 60 if empty / invalid
                var text = TimeLimitTextBox.Text;

                if (!int.TryParse(text, out seconds))
                {
                    seconds = 60; // default 1 minute
                }

                // Clamp between 3 seconds and 120 seconds
                if (seconds < 3)
                    seconds = 3;
                else if (seconds > 120)
                    seconds = 120;

                pack.TimePerQuestion = seconds;
            }

            DialogResult = true;
            Close();
        }
    }
}
