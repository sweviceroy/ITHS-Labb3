using ITHSLab3.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ITHSLab3.ViewModels
{
    // ViewModel for Config-screen: bygger/ändrar Question Packs
    // First part of the assignment to create  aview model that we
    // are gonna use to create questions. This could have been hard
    // -coded but we are gonna need a GUI to do it.

    public class ConfigurationViewModel : ViewModelBase
    {

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ PROPERTIES AND FIELDS     ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        
        public ObservableCollection<QuestionPack> QuestionPacks { get; } = new();

        private QuestionPack _selectedPack; // Dölj alla fields, sätt dem med property och databinding
        public QuestionPack SelectedPack
        {
            get => _selectedPack;
            set { _selectedPack = value; OnPropertyChanged(); }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set { _selectedQuestion = value; OnPropertyChanged(); }
        }


        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■ METHODS and other MEMBERS ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■


        // Commands 
        // TODO: Add commands for : 
        // Adding packs                                             [v]
        // adding questions                                         [v]
        // Options                                                  [V]
        // Start the game (StartPlayCom)                            [V]
        // TODO: Ändra dessa till *Command efteråt för MVVM skull   [V]
        public ICommand AddPackCommand { get; } 
        public ICommand RemovePackCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand RemoveQuestionCommand { get; }
        public ICommand OpenPackOptionsCommand { get; }
        public ICommand StartPlayCommand { get; }

        // _____________________________________________________________________ END COMMANDS
        //  Navigation/dialog hooks (Shell can subscribe)

        // Lets start the show *theme from lock stock starts playing
        // https://www.youtube.com/watch?v=suZIGmIhUsw&t=108s
        
        public event Action<QuestionPack> StartPlayRequested;
        
        // open settings for dialog
        public event Action<QuestionPack> PackOptionsRequested;     // open settings for dialog

        // simple id-counters for questions, questionpacks and options, 
        private int _nextQuestionId = 1;
        private int _nextPackId = 1;
        
        private int _nextOptionId = 1;

        public ConfigurationViewModel()
        {

            // Set all Commands
            // use _ => for LλBDA without input 
            AddPackCommand = new RelayCommand(_ => AddPack());                                                   //Execute: call AddPack() method. CanExecute: defaults = null → räknas som “always true,”. The button is always enabled.
            RemovePackCommand = new RelayCommand(_ => RemoveSelectedPack(), _ => SelectedPack != null);         // ONLY ENABLED if SelectedPack exists
            AddQuestionCommand = new RelayCommand(_ => AddQuestionToSelectedPack(), _ => SelectedPack != null); // same for these + Slected question if we gonna remove a question
            RemoveQuestionCommand = new RelayCommand(_ => RemoveSelectedQuestion(), _ => SelectedPack != null && SelectedQuestion != null);
            OpenPackOptionsCommand = new RelayCommand(_ => PackOptionsRequested?.Invoke(SelectedPack), _ => SelectedPack != null); // need nullable? so it wont crash
            StartPlayCommand = new RelayCommand(_ => StartPlayRequested?.Invoke(SelectedPack), _ => SelectedPack != null && SelectedPack.Questions.Any()); // Time to start if we have a selected pack and this is not empty. LINQ to get the avlible question

            //// Load example quiz pack so UI has something to show @ start 
            LoadSampleContent();
        }

        // --- Actions ---

        // TODO: 
        // AddPack                      [V]
        // RemoveSelectedPack           [V]
        //AddQuestionToSelectedPack     [V]
        // RemoveSelectedQuestion       [V]

        // add a simple startpack to test with
        private void AddPack()
        {
            var pack = new QuestionPack {
                Id = _nextPackId++,
                Name = $"QuestionPack {_nextPackId - 1}",
                Description = "MISSING DESCRIPTION ",
                TimePerQuestion = 30 // default 30s/question Change this MAGIC NUMBER LATER! 
                // Difficulty = Difficulty.Medium  // Later
            };

            // lägg in en startfråga med 4 alternativ (krav: alltid 4)
            var q = NewQuestion("Press f to ...");
            q.Options.Add(NewOption("Pay respects", true)); // multiple correct ones!
            q.Options.Add(NewOption("Open/Close door", true));
            q.Options.Add(NewOption("Get rekt", false));
            q.Options.Add(NewOption("delete System.32", false));
            pack.Questions.Add(q);

            QuestionPacks.Add(pack);
            SelectedPack = pack;
            SelectedQuestion = q;
        }

        // Remove SelectedPack
        private void RemoveSelectedPack()
        {
            if (SelectedPack == null) return;
            var toRemove = SelectedPack;
            SelectedPack = null;
            SelectedQuestion = null;
            QuestionPacks.Remove(toRemove);
        }

        private void AddQuestionToSelectedPack()
        {
            if (SelectedPack == null) return;

            var q = NewQuestion("New question...");
            // alltid exakt fyra alternativ (assignment-krav)
            q.Options.Add(NewOption("A", false));
            q.Options.Add(NewOption("B (this is true)", true));
            q.Options.Add(NewOption("C", false));
            q.Options.Add(NewOption("D", false));

            SelectedPack.Questions.Add(q);
            // meddela UI att listan ändrats
            OnPropertyChanged(nameof(SelectedPack));
            SelectedQuestion = q;
        }

        // This needs to fix the ID TOO! [V]
        private void RemoveSelectedQuestion()
        {
            if (SelectedPack == null || SelectedQuestion == null) return;

            var idx = SelectedPack.Questions.IndexOf(SelectedQuestion);
            if (idx >= 0)
            {
                SelectedPack.Questions.RemoveAt(idx);
                SelectedQuestion = GetNextQuestionAfterRemoval(idx);
                OnPropertyChanged(nameof(SelectedPack));
            }
        }

        // --- Helpers ---

        // Helper: find the next valid question to select after one has been removed
        private Question GetNextQuestionAfterRemoval(int removedIndex)
        {
            // If no pack or questions exist, return null
            if (SelectedPack == null || SelectedPack.Questions.Count == 0)
                return null;

            // If the removed index is now outside the list, clamp it to the last valid index
            if (removedIndex >= SelectedPack.Questions.Count)
                removedIndex = SelectedPack.Questions.Count - 1;

            // Return the next valid question (or null if index somehow invalid)
            return SelectedPack.Questions.ElementAtOrDefault(removedIndex);
        }

        private Question NewQuestion(string text)
        {
            return new Question
            {
                Id = _nextQuestionId++,
                QuestionText = text
            };
        }

        private QuestionOption NewOption(string text, bool isCorrect)
        {
            return new QuestionOption
            {
                Id = _nextOptionId++,
                AnswerText = text,
                IsCorrectAnswer = isCorrect
            };
        }

        private void LoadSampleContent()
        {
            var pack = new QuestionPack
            {
                Id = _nextPackId++,
                Name = "C# Basics",
                Description = "Intro questions for C# beginners.",
                TimePerQuestion = 25
                // Difficulty = Difficulty.Easy
            };

            // q1 – for the true 1337 dev gamerz
            var q1 = NewQuestion("When the code runs flawlessly first try, what do we call that?");
            q1.Options.Add(NewOption("Pure skill", true));
            q1.Options.Add(NewOption("Black magic", false));
            q1.Options.Add(NewOption("StackOverflow copy-paste luck", false));
            q1.Options.Add(NewOption("UwU divine intervention", false));

            // q2 – meme-tech logic vibes
            var q2 = NewQuestion("Which of these numbers is the most powerful in gamer lore?");
            q2.Options.Add(NewOption("420", false));
            q2.Options.Add(NewOption("67", false));
            q2.Options.Add(NewOption("69", false));
            q2.Options.Add(NewOption("1337", true));

            pack.Questions.Add(q1);
            pack.Questions.Add(q2);

            QuestionPacks.Add(pack);
            SelectedPack = pack;
            SelectedQuestion = q1;
        }
    }
}
