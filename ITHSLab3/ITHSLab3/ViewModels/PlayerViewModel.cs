using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ITHSLab3.Models;

namespace ITHSLab3.ViewModels
{
    // Handles quiz gameplay: iterates through questions, checking answers and keeps the score
    public class PlayerViewModel : ViewModelBase
    {
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ PROPERTIES AND FIELDS  ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private QuestionPack _currentPack;
        public QuestionPack CurrentPack
        {
            get => _currentPack;
            private set { _currentPack = value; OnPropertyChanged(); }
        }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            private set { _currentQuestion = value; OnPropertyChanged(); }
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            private set { _currentQuestionIndex = value; OnPropertyChanged(); }
        }

        private int _score;
        public int Score
        {
            get => _score;
            private set { _score = value; OnPropertyChanged(); }
        }

        private bool _isQuizFinished;
        public bool IsQuizFinished
        {
            get => _isQuizFinished;
            private set
            {
                _isQuizFinished = value;
                OnPropertyChanged();
                // här kan vi senare trigga om NextQuestionCommand kan köra osv
            }
        }

        // this updates with just Options.Add(new Option);
        public ObservableCollection<QuestionOption> Options { get; } = new();

        // Fired when the quiz ends → ShellViewModel can swap to results view
        public event Action<int, int> QuizFinished;

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ COMMANDS              ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        // no sets for these ICommands
        public ICommand CheckAnswerCommand { get; }
        public ICommand NextQuestionCommand { get; }

        // _____________________________________________________________________ END COMMANDS

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ CONSTRUCTOR           ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        // tom ctor – Shell skapar denna en gång och anropar LoadPack() när vi ska spela
        public PlayerViewModel()
        {
            // skapa commands en gång
            CheckAnswerCommand = new RelayCommand(AnswerQuestion);
            NextQuestionCommand = new RelayCommand(
                _ => NextQuestion(),
                _ => !IsQuizFinished
            );
        }

        // Shell kallar denna när användaren klickar "Start" i ConfigurationView
        public void LoadPack(QuestionPack pack)
        {
            if (pack == null || pack.Questions == null || pack.Questions.Count == 0)
            {
                // inget att spela – markera som klart direkt
                CurrentPack = pack;
                CurrentQuestion = null;
                CurrentQuestionIndex = 0;
                Score = 0;
                IsQuizFinished = true;
                QuizFinished?.Invoke(0, 0);
                return;
            }

            CurrentPack = pack;
            CurrentQuestionIndex = 0;
            Score = 0;
            IsQuizFinished = false;

            LoadCurrentQuestion();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ METHODS               ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private void LoadCurrentQuestion()
        {
            if (CurrentPack == null || CurrentPack.Questions == null)
            {
                FinishQuiz();
                return;
            }

            if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < CurrentPack.Questions.Count)
            {
                CurrentQuestion = CurrentPack.Questions[CurrentQuestionIndex];

                // clear and repopulate Options for binding (needed for ObservableCollection updates)
                Options.Clear();
                foreach (var opt in CurrentQuestion.Options)
                    Options.Add(opt);
            }
            else
            {
                // if no more questions → finish quiz
                FinishQuiz();
            }
        }

        private void AnswerQuestion(object parameter)
        {
            if (IsQuizFinished)
                return;

            if (parameter is not QuestionOption selectedOption)
                return;

            // check if correct
            if (selectedOption.IsCorrectAnswer)
                Score++;

            // increaseScore(); // vi kör direkt i koden istället
            NextQuestion();
        }

        private void NextQuestion()
        {
            if (CurrentPack == null || CurrentPack.Questions == null)
            {
                FinishQuiz();
                return;
            }

            CurrentQuestionIndex++;

            if (CurrentQuestionIndex < CurrentPack.Questions.Count)
                LoadCurrentQuestion();
            else
                FinishQuiz();
        }

        private void FinishQuiz()
        {
            IsQuizFinished = true;

            var totalQuestions = CurrentPack?.Questions?.Count ?? 0;
            QuizFinished?.Invoke(Score, totalQuestions);
        }
    }
}
