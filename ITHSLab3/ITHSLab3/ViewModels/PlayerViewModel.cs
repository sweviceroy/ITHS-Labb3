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
            set { _currentPack = value; OnPropertyChanged(); }
        }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; OnPropertyChanged(); }
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set { _currentQuestionIndex = value; OnPropertyChanged(); }
        }

        private int _score;
        public int Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(); }
        }

        private bool _isQuizFinished;
        public bool IsQuizFinished
        {
            get => _isQuizFinished;
            set { _isQuizFinished = value; OnPropertyChanged(); }
        }

        /*
            public List<QuestionOption> Options { get; set; } = new();

            private List<QuestionOption> _options;
            public List<QuestionOption> Options
            {
            get => _options;
            set { _options = value; OnPropertyChanged(); }
            }

            Options.Add(newOption);
            var newList = new List<QuestionOption>(Options);
            newList.Add(newOption);
            Options = newList;           

         */

        // this updates with just Options.Add(new Option);
        public ObservableCollection<QuestionOption> Options { get; set; } = new();

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

        public PlayerViewModel(QuestionPack pack)
        {
            CurrentPack = pack;
            CurrentQuestionIndex = 0;
            Score = 0;
            IsQuizFinished = false;

            // set the question
            LoadCurrentQuestion();

            // create commands
            CheckAnswerCommand = new RelayCommand(AnswerQuestion);
            NextQuestionCommand = new RelayCommand(_ => NextQuestion(), _ => !IsQuizFinished);
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ METHODS               ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private void LoadCurrentQuestion()
        {
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
            if (parameter is not QuestionOption selectedOption)
                return;

            // check if correct
            if (selectedOption.IsCorrectAnswer)
                Score++;
            //increaseScore()
            NextQuestion();
        }

        private void NextQuestion()
        {
            CurrentQuestionIndex++;

            if (CurrentQuestionIndex < CurrentPack.Questions.Count)
                LoadCurrentQuestion();
            else
                FinishQuiz();
        }

        private void FinishQuiz()
        {
            IsQuizFinished = true;
            QuizFinished?.Invoke(Score, CurrentPack.Questions.Count);
        }

        /*
        public void increaseScore() {
        score++;
        }
         * 
         */
    }
}
