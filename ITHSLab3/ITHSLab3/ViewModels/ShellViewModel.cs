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
                OnPropertyChanged();        // Här sätter vi Current view
            }
        }

        public ShellViewModel()
        {
            // start with splashScreenen
            var splash = new SplashViewModel();
            splash.SplashCompleted += OnSplashCompleted;

            CurrentView = splash;
        }

        private void OnSplashCompleted()
        {
            // when splash says "done", switch to menu
            CurrentView = new MenuViewModel();
        }
    }
}
