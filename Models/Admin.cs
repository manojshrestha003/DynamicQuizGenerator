using System.ComponentModel.DataAnnotations;

namespace DynamicQuizGenerator.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AdminName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string hashedPassword { get; set; }

    }
}
