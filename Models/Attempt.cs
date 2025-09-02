namespace DynamicQuizGenerator.Models
{
    public class Attempt
    {
        public int AttemptId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public int StudentId { get; set; } 
        public DateTime AttemptDate { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
