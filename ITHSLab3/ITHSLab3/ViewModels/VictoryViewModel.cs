using System;
using System.Windows.Input;

namespace ITHSLab3.ViewModels 
{
    public class VictoryViewModel : ViewModelBase
    {
        public event Action? BackToMenuRequested;

        public string PackName { get; }
        public int Score { get; }
        public int TotalQuestions { get; }

        public string Heading => "Congratulations!";
        public string ResultText =>
            $"You scored {Score} out of {TotalQuestions} questions.";

        public ICommand BackToMenuCommand { get; }

        public VictoryViewModel(string packName, int score, int totalQuestions)
        {
            PackName = packName;
            Score = score;
            TotalQuestions = totalQuestions;

            BackToMenuCommand = new RelayCommand(_ => BackToMenuRequested?.Invoke());
        }
    }
}
