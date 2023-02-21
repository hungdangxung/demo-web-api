using System.ComponentModel.DataAnnotations;

namespace DemoWebAPI.Models
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
