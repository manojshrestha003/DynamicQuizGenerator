using DynamicQuizGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class QuestionsController : Controller
{
    private readonly QuizDbContext _context;
    public QuestionsController(QuizDbContext context)
    {
        _context = context;
    }

    // GET: Questions for a quiz
    [HttpGet("Questions/Quiz/{quizId}")]
    public IActionResult Index(int quizId)
    {
        var quiz = _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                    .FirstOrDefault(q => q.QuizId == quizId);
        if (quiz == null) return NotFound();
        ViewBag.QuizTitle = quiz.Title;
        ViewBag.QuizId = quiz.QuizId;
        return View(quiz.Questions);
    }

    // GET: Questions/Create
    public IActionResult Create(int quizId)
    {
        var question = new Question { QuizId = quizId };
        return View(question);
    }

    // POST: Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Question question)
    {
        if (ModelState.IsValid)
        {
            _context.Questions.Add(question);
            _context.SaveChanges();
            return RedirectToAction("Index", new { quizId = question.QuizId });
        }

        // Log validation errors to the console
        Console.WriteLine(" Model is invalid:");
        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }
        }

        return View(question);
    }



    // GET: Questions/Edit/5
    public IActionResult Edit(int id)
    {
        var question = _context.Questions.Find(id);
        if (question == null) return NotFound();

        // Pass QuizId to the view
        ViewBag.QuizId = question.QuizId;

        return View(question);
    }

    // POST: Questions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Question question)
    {
        if (id != question.QuestionId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(question);
            _context.SaveChanges();
            return RedirectToAction("Index", new { quizId = question.QuizId });
        }

        // Keep quizId if validation fails
        ViewBag.QuizId = question.QuizId;
        return View(question);
    }

    // GET: Questions/Delete/5
    public IActionResult Delete(int id)
    {
        var question = _context.Questions.Find(id);
        if (question == null) return NotFound();
        return View(question);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null) return NotFound();

        int quizId = question.QuizId;
        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", new { quizId });
    }

}
