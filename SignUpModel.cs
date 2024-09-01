using System.ComponentModel.DataAnnotations;

namespace TugwellApp.Models
{
    public class SignUpModel
    {

        public string WelcomeMessage { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(8)]  // Password length validation
        public string Password { get; set; }
    }
}
