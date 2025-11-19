using System.Collections.ObjectModel;

namespace ITHSLab3.Models
{
    public class QuestionPack
    {
        public int Id { get; set; }                 // unique id for the pack
        public string Name { get; set; } = string.Empty;   // e.g. "C# Basics"
        public string Description { get; set; } = string.Empty; // short text shown in config view

        // seconds per question. sätt 0 för oändligt
        public int TimePerQuestion { get; set; }

        // Difficulty of the pack (Easy/Medium/Hard) defaulta till medium. 
        public Difficulty Difficulty { get; set; } = Difficulty.Medium;

        // Use ObservableCollection so WPF updates UI on Add/Remove
        public ObservableCollection<Question> Questions { get; set; }

        public QuestionPack()
        {
            Questions = new ObservableCollection<Question>();
        }

        public QuestionPack(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            Questions = new ObservableCollection<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }
}
