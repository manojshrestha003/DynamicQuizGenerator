using DynamicQuizGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

namespace DynamicQuizGenerator.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuizDbContext _context;
        private readonly IConfiguration _config;
        public AccountController(QuizDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult  Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult AdminRegister()
        {
            return View();
        }

        public IActionResult AdminLogin()
        {
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null)
            {
                ViewBag.Message = "Student is not registered";
                return View();
            }

            // Generate Token 
            var token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                StudentId = student.StudentId, // ✅ ensure correct property name
                Token = token,
                ExpiryDate = DateTime.Now.AddMinutes(30)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = Url.Action("ResetPassword", "Account",
                new { token }, Request.Scheme);

            ViewBag.Message = "Password reset link: " + resetLink;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Quizzyfy Support", _config["EmailSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", student.Email));
            emailMessage.Subject = "Password Reset Request";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello {student.Username},\n\n" +
                       $"Click the link below to reset your password:\n{resetLink}\n\n" +
                       "If you didn't request this, ignore this email."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            ViewBag.Message = "Password reset link has been sent to your email!";

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token )
        {
            return View(model: token);

        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(rt => rt.Student)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.ExpiryDate > DateTime.Now);

            if (resetToken == null)
            {
                ViewBag.Message = "Invalid or expired token!";
                return View();
            }

            // Update password
            resetToken.Student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Invalidate token
            _context.PasswordResetTokens.Remove(resetToken);

            await _context.SaveChangesAsync();

            ViewBag.Message = "Password reset successful. You can now login!";
            return RedirectToAction("Login");
        }

    }
}
