using DynamicQuizGenerator.Models;
using Microsoft.EntityFrameworkCore;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Attempt> Attempts { get; set; }
    public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Only cascade from Attempt -> Answers
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Attempt)
            .WithMany(at => at.Answers)
            .HasForeignKey(a => a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent cascade from Question -> Answers
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent cascade from Option -> Answers
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.SelectedOption)
            .WithMany()
            .HasForeignKey(a => a.OptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }



}
