using DynamicQuizGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicQuizGenerator.Controllers
{
    public class OptionsController : Controller
    {
        private readonly QuizDbContext _context;
        public OptionsController(QuizDbContext context)
        {
            _context = context;
        }

        // GET: Options for a question
        public IActionResult Index(int questionId)
        {
            var question = _context.Questions
                            .Include(q => q.Options)
                            .FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null) return NotFound();
            ViewBag.QuestionText = question.Text;
            ViewBag.QuestionId = question.QuestionId;
            return View(question.Options);
        }

        // GET: Options/Create
        public IActionResult Create(int questionId)
        {
            ViewBag.QuestionId = questionId;
            return View();
        }

        // POST: Options/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Option option)
        {
            if (ModelState.IsValid)
            {
                _context.Options.Add(option);
                _context.SaveChanges();
                return RedirectToAction("Index", new { questionId = option.QuestionId });
            }

           
            Console.WriteLine("Model is invalid:");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.QuestionId = option.QuestionId;
            return View(option);
        }


        // GET: Options/Edit/5
        public IActionResult Edit(int id)
        {
            var option = _context.Options.Find(id);
            if (option == null) return NotFound();

            
            ViewBag.QuestionId = option.QuestionId;
            return View(option);
        }

        // POST: Options/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Option option)
        {
            if (id != option.OptionId)
                return NotFound();

            if (!_context.Questions.Any(q => q.QuestionId == option.QuestionId))
            {
                Console.WriteLine($"Invalid QuestionId: {option.QuestionId}");
                ModelState.AddModelError("QuestionId", "Invalid Question selected.");
            }

            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model is invalid:");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

         
                ViewBag.QuestionId = option.QuestionId;
                return View(option);
            }

            _context.Update(option);
            _context.SaveChanges();
            return RedirectToAction("Index", new { questionId = option.QuestionId });
        }



        // GET: Options/Delete/5
        public IActionResult Delete(int id)
        {
            var option = _context.Options.Find(id);
            if (option == null) return NotFound();
            return View(option);
        }

        // POST: Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var option = _context.Options.Find(id);
            int questionId = option.QuestionId;
            _context.Options.Remove(option);
            _context.SaveChanges();
            return RedirectToAction("Index", new { questionId });
        }
    }
}
