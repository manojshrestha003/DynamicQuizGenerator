namespace DynamicQuizGenerator.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }

        // Navigation
        public Student Student { get; set; }

    }
}
