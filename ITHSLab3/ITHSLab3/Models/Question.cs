using System.Collections.Generic;

namespace ITHSLab3.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<QuestionOption> Options { get; set; }

        // public int PointsAwarded { get; set; }
        // public string Category { get; set; }

        public Question()
        {
            // for JSON + to avoid null lists
            Options = new List<QuestionOption>();
        }

        public Question(int id, string questionText)
        {
            Id = id;
            QuestionText = questionText;
            Options = new List<QuestionOption>();
        }

        public void AddOption(QuestionOption option)
        {
            Options.Add(option);
        }
    }
}
