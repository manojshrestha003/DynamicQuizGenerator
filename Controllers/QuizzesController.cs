using Microsoft.AspNetCore.Mvc;
using DynamicQuizGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicQuizGenerator.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly QuizDbContext _context;

        public QuizzesController(QuizDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

    
        public IActionResult Create() => View();

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            if (!ModelState.IsValid) return View(quiz);

            quiz.CreatedDate = DateTime.Now;
            await _context.Quizzes.AddAsync(quiz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Quizzes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            return quiz == null ? NotFound() : View(quiz);
        }

        // POST: Quizzes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quiz quiz)
        {
            if (id != quiz.QuizId) return NotFound();
            if (!ModelState.IsValid) return View(quiz);

            try
            {
                _context.Update(quiz);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await QuizExists(quiz.QuizId))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Quizzes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            return quiz == null ? NotFound() : View(quiz);
        }

        // POST: Quizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Quizzes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.QuizId == id);
            return quiz == null ? NotFound() : View(quiz);
        }

        private async Task<bool> QuizExists(int id) =>
            await _context.Quizzes.AnyAsync(e => e.QuizId == id);
    }
}
