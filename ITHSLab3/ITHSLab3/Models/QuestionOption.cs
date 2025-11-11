using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHSLab3.Models
{
    public class QuestionOption
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }

        public bool IsCorrectAnswer { get; set; }

        // public Cathegory QuestionCathegory {get; set;}

        public QuestionOption()
        {

        } // gonna need for JSON

        public QuestionOption(int id, string text, bool isCorrect)
        {
            this.Id = id;
            this.AnswerText = text;
            this.IsCorrectAnswer = isCorrect;
        }

    }
}
