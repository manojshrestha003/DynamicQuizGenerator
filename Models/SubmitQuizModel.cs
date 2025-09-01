using System.Collections.Generic;

namespace DynamicQuizGenerator.Models
{
    public class SubmitQuizModel
    {
        public int QuizId { get; set; } 
        public Dictionary<int, int> Answers { get; set; }
    }
}
