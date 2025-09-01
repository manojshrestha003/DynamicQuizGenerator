using DynamicQuizGenerator.Models;
using DynamicQuizGenerator.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicQuizGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentAuthController : Controller
    {
        private readonly QuizDbContext _context;
        private readonly IConfiguration _config;

        public StudentAuthController(QuizDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Register new student
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Check if email already exists
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email))
                return BadRequest(new { message = "Email already registered" });

            var student = new Student
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student registered successfully" });
        }

        // Login student
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == dto.Email);

            if (student == null || !BCrypt.Net.BCrypt.Verify(dto.Password, student.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateJwtToken(student);

            return Ok(new
            {
                token,
                student = new
                {
                    student.StudentId,
                    student.Username,
                    student.Email
                }
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Get studentId claim
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentIdClaim))
                return Unauthorized(new { message = "Invalid token" });

            // Parse safely
            if (!int.TryParse(studentIdClaim, out int studentId))
                return Unauthorized(new { message = "Invalid token" });

            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(new
            {
                student.StudentId,
                student.Username,
                student.Email
            });
        }


        // Generate JWT safely
        private string GenerateJwtToken(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");

            // Claims for JWT
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, student.StudentId.ToString()),
        new Claim(ClaimTypes.Email, student.Email ?? throw new InvalidOperationException("Student email cannot be null.")),
        new Claim(ClaimTypes.Name, student.Username ?? student.Email)
    };

            // Get JWT key from configuration safely
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Get issuer and audience safely
            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is missing in configuration.");

            // Create token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
