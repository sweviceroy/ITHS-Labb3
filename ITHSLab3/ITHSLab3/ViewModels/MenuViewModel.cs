using ITHSLab3.Services;
using System;
using System.Windows.Input;
using ITHSLab3.Services;

namespace ITHSLab3.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        // 🔹 Event for ShellViewModel to listen to
        public event Action StartConfigurationRequested;

        // 🔹 Command bound to a button in MenuView
        public ICommand StartConfigurationCommand { get; }

        public string Title => "QuizForge – Menu";

        public MenuViewModel()
        {
            StartConfigurationCommand = new RelayCommand(StartConfiguration);
        }

        private void StartConfiguration(object? _)
        {
            AudioService _audioService = new AudioService();
            _audioService.PlayOneShot("Assets/SoundStartConfig.wav", 5.0); // Går ej! Clampat till 1.0 :/ FML
            Thread.Sleep(3000);
            StartConfigurationRequested?.Invoke();
        }
    }
}
