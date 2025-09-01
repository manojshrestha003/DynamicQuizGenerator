using DynamicQuizGenerator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DynamicQuizGenerator.Controllers
{
    
    public class TakeQuizController : Controller
    {
        private readonly QuizDbContext _context;
        public TakeQuizController(QuizDbContext context)
        {
            _context = context;
        }

        // GET: List of quizzes
        public IActionResult Index()
        {
            var quizzes = _context.Quizzes.ToList();
            return View(quizzes);
        }

        // GET: Take a quiz
        public IActionResult Start(int quizId)
        {
            var quiz = _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault(q => q.QuizId == quizId);

            if (quiz == null) return NotFound();
            return View(quiz);
        }

        // POST: Submit quiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int quizId, Dictionary<int, int> answers)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(studentId, out int studentIdInt))
            {
                return Unauthorized(); 
            }

            var attempt = new Attempt
            {
                QuizId = quizId,
                StudentId = studentIdInt,
                AttemptDate = DateTime.Now
            };


            _context.Attempts.Add(attempt);
            _context.SaveChanges();

            foreach (var ans in answers)
            {
                _context.Answers.Add(new Answer
                {
                    AttemptId = attempt.AttemptId,
                    QuestionId = ans.Key,
                    OptionId = ans.Value
                });
            }
            _context.SaveChanges();

            return RedirectToAction("Result", new { attemptId = attempt.AttemptId });
        }

        // GET: Show result
        public IActionResult Result(int attemptId)
        {
            var attempt = _context.Attempts
    .Include(a => a.Quiz)
    .Include(a => a.Answers)
        .ThenInclude(a => a.SelectedOption)
    .Include(a => a.Answers)
        .ThenInclude(a => a.Question) 
            .ThenInclude(q => q.Options) 
    .FirstOrDefault(a => a.AttemptId == attemptId);


            if (attempt == null) return NotFound();

            int totalQuestions = attempt.Answers.Count;
            int correctAnswers = attempt.Answers.Count(a => a.SelectedOption.IsCorrect);

            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.CorrectAnswers = correctAnswers;

            return View(attempt);
        }
    }
}
