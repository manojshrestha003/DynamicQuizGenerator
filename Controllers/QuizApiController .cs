using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DynamicQuizGenerator.Models;

namespace DynamicQuizGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class QuizApiController : Controller
    {
        private readonly QuizDbContext _context;
        public QuizApiController(QuizDbContext context) { _context = context; }

        [Authorize]
        [HttpGet("all")]
        public IActionResult GetAllQuizzes()
        {
            var quizzes = _context.Quizzes.ToList();
            return Ok(quizzes);
        }


        [Authorize]
    [HttpPost("Submit")]
    public IActionResult Submit([FromBody] SubmitQuizModel model)
    {
        
        return Ok(new { message = "Quiz submitted successfully!" });
    }
    }
}
