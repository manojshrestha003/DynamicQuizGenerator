using System.ComponentModel.DataAnnotations;

namespace DynamicQuizGenerator.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Question>? Questions { get; set; }

    }
}
