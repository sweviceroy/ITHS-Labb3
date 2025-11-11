using System.Collections.Generic;

namespace ITHSLab3.Models
{
    public class QuestionPack
    {
        public int Id { get; set; }                 // unique id for the pack
        public string Name { get; set; }            // e.g. "C# Basics"
        public string Description { get; set; }     // short text shown in config view

        // optional: use this if you created Difficulty.cs
        // public Difficulty Difficulty { get; set; }

        // optional: seconds per question, can be 0 = no limit

        public int TimePerQuestion { get; set; }

        public List<Question> Questions { get; set; }

        public QuestionPack()
        {
            Questions = new List<Question>();
        }

        public QuestionPack(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            Questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }
}
