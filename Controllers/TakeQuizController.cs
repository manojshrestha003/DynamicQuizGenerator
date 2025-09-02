using DynamicQuizGenerator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class QuizSubmissionDto
{
    public int QuizId { get; set; }
    public Dictionary<int, int> Answers { get; set; }
}

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
        public IActionResult Submit([FromBody] QuizSubmissionDto submission)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentId, out int studentIdInt))
                return Unauthorized();

            var attempt = new Attempt
            {
                QuizId = submission.QuizId,
                StudentId = studentIdInt,
                AttemptDate = DateTime.Now
            };

            _context.Attempts.Add(attempt);
            _context.SaveChanges();

            foreach (var ans in submission.Answers)
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

       
        public IActionResult History()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentId, out int studentIdInt))
                return Unauthorized();

            var attempts = _context.Attempts
                .Where(a => a.StudentId == studentIdInt)
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                    .ThenInclude(a => a.SelectedOption)
                .Include(a => a.Answers)
                    .ThenInclude(a => a.Question)
                .OrderByDescending(a => a.AttemptDate)
                .Select(a => new
                {
                    a.AttemptId,
                    a.AttemptDate,
                    Quiz = new
                    {
                        a.Quiz.QuizId,
                        a.Quiz.Title
                    },
                    Answers = a.Answers.Select(ans => new
                    {
                        ans.QuestionId,
                        QuestionText = ans.Question.Text,
                        SelectedOption = new
                        {
                            ans.SelectedOption.OptionId,
                            ans.SelectedOption.Text,
                            ans.SelectedOption.IsCorrect
                        }
                    }).ToList()
                })
                .ToList();

            return Json(attempts);
        }

    }
}
