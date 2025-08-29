namespace DynamicQuizGenerator.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public int AttemptId { get; set; }
        public Attempt Attempt { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int OptionId { get; set; }
        public Option SelectedOption { get; set; }
    }
}
