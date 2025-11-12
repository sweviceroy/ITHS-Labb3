using System;
using ITHSLab3.ViewModels;

namespace ITHSLab3.ViewModels
{
    // make it public so MainWindow can create it
    public class ShellViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(); // Notify the UI
            }
        }

        public ShellViewModel()
        {
            // Start with splash screen
            var splash = new SplashViewModel();
            splash.SplashCompleted += OnSplashCompleted;

            CurrentView = splash;
        }

        private void OnSplashCompleted()
        {
            // When splash says "done", switch to menu
            var menu = new MenuViewModel();
            menu.StartConfigurationRequested += OnStartConfiguration; // 🔹 subscribe to Menu event
            CurrentView = menu;
        }

        private void OnStartConfiguration()
        {
            // 🔹 Switch from Menu -> Configuration
            CurrentView = new ConfigurationViewModel();
        }
    }
}
