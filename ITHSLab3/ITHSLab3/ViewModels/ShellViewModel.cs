using ITHSLab3.Models;        // behövs för QuestionPack
using ITHSLab3.Services;
using ITHSLab3.ViewModels;
using System;
using ITHSLab3.Views;
using System.Windows;           // Added


namespace ITHSLab3.ViewModels
{
    // make it public so MainWindow can create it
    public class ShellViewModel : ViewModelBase
    {
        // audioService that we use
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

        // Victory! 
        private VictoryViewModel _victoryViewModel;

        private string? _lastPlayedPackName;


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

        // Handler för PackOptions
        private void OnPackOptionsRequested(QuestionPack pack)
        {
            if (pack == null)
                return;

            // ADDA using System.Windows; För att kunna öppna nya minifönster
            var dialog = new PackOptionsDialog(pack)
            {
                Owner = Application.Current.MainWindow
            };

            // We don't need the result right now; the pack is edited via bindings
            dialog.ShowDialog();
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

            // när menyn säger "öppna config", vi skapar bara en gång. 
            if (_configurationViewModel == null)
            {
                _configurationViewModel = new ConfigurationViewModel();

                // lyssna på när config vill starta spelet
                _configurationViewModel.StartPlayRequested += OnStartPlayRequested;

                // Hookat OnPackOptionsRequested
                _configurationViewModel.PackOptionsRequested += OnPackOptionsRequested;
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
                _playerViewModel.QuizFinished += OnQuizFinished;
            }

            _lastPlayedPackName = pack.Name;

            // låt PlayerViewModel ladda in valt pack
            _playerViewModel.LoadPack(pack);

            // byt vy till Player
            CurrentView = _playerViewModel;
        }

        private void OnQuizFinished(int score, int totalQuestions)
        {

            // Play victory sound
            // _audioService.PlayOneShot("Assets/SoundVictory.wav", 1.0); DOESNT PLAY! Something interrupts it. 

            string packName = _lastPlayedPackName ?? "NO PACKNAME FOUND";

            _victoryViewModel = new VictoryViewModel(packName, score, totalQuestions);
            _victoryViewModel.BackToMenuRequested += OnVictoryBackToMenu;

            CurrentView = _victoryViewModel;
        }

        private void OnVictoryBackToMenu()
        {
            CurrentView = _menuViewModel;
        }

    }
}
