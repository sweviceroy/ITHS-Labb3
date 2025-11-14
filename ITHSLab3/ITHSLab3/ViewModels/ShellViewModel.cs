using ITHSLab3.Models;        // behövs för QuestionPack
using ITHSLab3.Services;
using ITHSLab3.ViewModels;
using System;


namespace ITHSLab3.ViewModels
{
    // make it public so MainWindow can create it
    public class ShellViewModel : ViewModelBase
    {
        private readonly AudioService _audioService = new AudioService();


        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(); // notify UI att vi bytt vy
            }
        }

        // Håller referenser till våra huvud-vyer
        private SplashViewModel _splashViewModel;
        private MenuViewModel _menuViewModel;
        private ConfigurationViewModel _configurationViewModel;
        private PlayerViewModel _playerViewModel;

        public ShellViewModel()
        {
            // skapa splash först
            _splashViewModel = new SplashViewModel();
            _splashViewModel.SplashCompleted += OnSplashCompleted;

            // skapa meny direkt också (så vi kan återanvända den)
            _menuViewModel = new MenuViewModel();
            _menuViewModel.StartConfigurationRequested += OnStartConfiguration;

            // starta appen med splash
            CurrentView = _splashViewModel;
        }

        private void OnSplashCompleted()
        {
            // starta musikloop efter splash
            _audioService.PlayLoop("Assets/SoundMusic.wav");

            // när splash säger "klar" -> byt till meny
            CurrentView = _menuViewModel;
        }


        private void OnStartConfiguration()
        {
            // när menyn säger "öppna config"
            if (_configurationViewModel == null)
            {
                _configurationViewModel = new ConfigurationViewModel();

                // lyssna på när config vill starta spelet
                _configurationViewModel.StartPlayRequested += OnStartPlayRequested;
                // (senare kan vi även hooka PackOptionsRequested här)
            }

            CurrentView = _configurationViewModel;
        }

        private void OnStartPlayRequested(QuestionPack pack)
        {
            if (pack == null)
                return; // sanity check, borde inte hända men bättre safe

            // skapa eller återanvänd PlayerViewModel
            if (_playerViewModel == null)
            {
                _playerViewModel = new PlayerViewModel();
            }

            // låt PlayerViewModel ladda in valt pack
            _playerViewModel.LoadPack(pack); // 

            // byt vy till Player
            CurrentView = _playerViewModel;
        }
    }
}
