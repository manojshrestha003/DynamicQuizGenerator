using System.ComponentModel.DataAnnotations;

namespace DynamicQuizGenerator.Models
{
    public class Option
    {
        public int OptionId { get; set; }

        [Required]
        public string? Text { get; set; }
        
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }
    }
}
