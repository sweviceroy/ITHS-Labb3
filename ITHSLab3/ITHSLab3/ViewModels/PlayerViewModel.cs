using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;   // behövs för List<T>
using System.Windows.Input;
using ITHSLab3.Models;
using System.Threading.Tasks;


namespace ITHSLab3.ViewModels
{
    // Handles quiz gameplay: iterates through questions, checking answers and keeps the score
    public class PlayerViewModel : ViewModelBase
    {
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ PROPERTIES AND FIELDS  ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private string _feedbackImageSource;
        public string FeedbackImageSource
        {
            get => _feedbackImageSource;
            private set { _feedbackImageSource = value; OnPropertyChanged(); }
        }

        private bool _feedbackVisible;
        public bool FeedbackVisible
        {
            get => _feedbackVisible;
            private set { _feedbackVisible = value; OnPropertyChanged(); }
        }


        private QuestionPack _currentPack;
        public QuestionPack CurrentPack
        {
            get => _currentPack;
            private set
            {
                _currentPack = value;
                OnPropertyChanged();                     // CurrentPack
                OnPropertyChanged(nameof(ProgressText)); // uppdatera header-text
            }
        }

        // lista med frågor bara för denna spelomgång (shufflad)
        private List<Question> _sessionQuestions;

        // enkel Random för shuffling
        private readonly Random _random = new Random();

        // beräknad text för "Question X out of Y"
        public string ProgressText
        {
            get
            {
                int totalQuestions = 0;

                if (_sessionQuestions != null)
                    totalQuestions = _sessionQuestions.Count;
                else if (CurrentPack != null && CurrentPack.Questions != null)
                    totalQuestions = CurrentPack.Questions.Count;

                if (totalQuestions == 0)
                    return "NO QUESTIONS";

                // om vi har gått förbi sista frågan → visa sluttext
                if (CurrentQuestionIndex >= totalQuestions)
                    return $"Quiz Finished! Score: {Score}/{totalQuestions}";

                return $"Question {CurrentQuestionIndex + 1} out of {totalQuestions}";
            }
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
            private set
            {
                _currentQuestionIndex = value;
                OnPropertyChanged();                     // CurrentQuestionIndex
                OnPropertyChanged(nameof(ProgressText)); // uppdatera header-text
            }
        }

        private int _score;
        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressText)); // score påverkar sluttexten
            }
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
                _sessionQuestions = null;
                CurrentQuestion = null;
                CurrentQuestionIndex = 0;
                Score = 0;
                IsQuizFinished = true;
                OnPropertyChanged(nameof(ProgressText));
                QuizFinished?.Invoke(0, 0);
                return;
            }

            CurrentPack = pack;
            Score = 0;
            IsQuizFinished = false;

            // skapa en sessionslista med frågor och shuffle den
            _sessionQuestions = new List<Question>(pack.Questions);
            ShuffleQuestions();

            CurrentQuestionIndex = 0;
            LoadCurrentQuestion();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ METHODS               ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        // enkel Fisher–Yates shuffle – inga LINQ eller konstigheter
        private void ShuffleQuestions()
        {
            if (_sessionQuestions == null)
                return;

            for (int i = _sessionQuestions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1); // random index 0..i
                Question temp = _sessionQuestions[i];
                _sessionQuestions[i] = _sessionQuestions[j];
                _sessionQuestions[j] = temp;
            }
        }

        private void LoadCurrentQuestion()
        {
            // kolla först att vi har något att spela
            if (_sessionQuestions == null || _sessionQuestions.Count == 0)
            {
                FinishQuiz();
                return;
            }

            // kolla att indexet är inom listans gränser
            if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < _sessionQuestions.Count)
            {
                // plocka ut den aktuella frågan från vår shufflade lista
                CurrentQuestion = _sessionQuestions[CurrentQuestionIndex];

                // om frågan saknar alternativ: töm listan och baila
                if (CurrentQuestion.Options == null || CurrentQuestion.Options.Count == 0)
                {
                    Options.Clear();
                    return;
                }

                // skapa en KOPIA av alternativen – vi vill INTE röra originalet i modellen
                List<QuestionOption> shuffledOptions = new List<QuestionOption>(CurrentQuestion.Options);

                // enkel Fisher–Yates shuffle
                for (int i = shuffledOptions.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    QuestionOption temp = shuffledOptions[i];
                    shuffledOptions[i] = shuffledOptions[j];
                    shuffledOptions[j] = temp;
                }

                // fyll ObservableCollection för UI
                Options.Clear();
                foreach (QuestionOption opt in shuffledOptions)
                {
                    Options.Add(opt);
                }
            }
            else
            {
                // om indexet är utanför → avsluta quizet
                FinishQuiz();
            }
        }


        private async void AnswerQuestion(object parameter)
        {
            if (IsQuizFinished)
                return;

            QuestionOption selectedOption = parameter as QuestionOption;
            if (selectedOption == null)
                return;

            // check correctness
            bool correct = selectedOption.IsCorrectAnswer;
            if (correct)
                Score++;

            // set image
            if (correct)
                FeedbackImageSource = "/Assets/ImageCORRECT.png";
            else
                FeedbackImageSource = "/Assets/ImageERROR.png";

            // show it
            FeedbackVisible = true;

            // wait 3 seconds
            await Task.Delay(3000);

            // hide again
            FeedbackVisible = false;

            // go next
            NextQuestion();
        }


        private void NextQuestion()
        {
            if (_sessionQuestions == null || _sessionQuestions.Count == 0)
            {
                FinishQuiz();
                return;
            }

            CurrentQuestionIndex++;

            if (CurrentQuestionIndex < _sessionQuestions.Count)
                LoadCurrentQuestion();
            else
                FinishQuiz();
        }

        private void FinishQuiz()
        {
            IsQuizFinished = true;

            int totalQuestions = 0;
            if (_sessionQuestions != null)
                totalQuestions = _sessionQuestions.Count;
            else if (CurrentPack != null && CurrentPack.Questions != null)
                totalQuestions = CurrentPack.Questions.Count;

            QuizFinished?.Invoke(Score, totalQuestions);
            OnPropertyChanged(nameof(ProgressText)); // visa sluttext i headern
        }
    }
}
