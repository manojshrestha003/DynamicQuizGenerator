using System.ComponentModel.DataAnnotations;

namespace DynamicQuizGenerator.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        [Required]
        public string Text { get; set; }

        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();  
    }
}
