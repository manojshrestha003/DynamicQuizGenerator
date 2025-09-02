using DynamicQuizGenerator.Models;
using DynamicQuizGenerator.Models.DTOs;
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
    public class AdminAuthController : ControllerBase
    {
        private readonly QuizDbContext _context;
        private readonly IConfiguration _config;

        public AdminAuthController(QuizDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Register Admin
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Admins.AnyAsync(a => a.Email == dto.Email))
                return BadRequest(new { message = "Email already registered" });

            var admin = new Admin
            {
                AdminName = dto.Username,
                Email = dto.Email,
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin registered successfully" });
        }

        // Login Admin
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == dto.Email);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(dto.Password, admin.hashedPassword))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateJwtToken(admin);

            return Ok(new
            {
                token,
                admin = new
                {
                    admin.AdminId,
                    admin.AdminName,
                    admin.Email
                }
            });
        }

        // Generate JWT for Admin
        private string GenerateJwtToken(Admin admin)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Name, admin.AdminName),
                
            };

            var jwtKey = _config["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
