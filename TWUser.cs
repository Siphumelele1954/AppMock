using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TugwellApp.Models
{
    public class TWUser
    {
        [Required]
        [RegularExpression(@"^[A-Za-z]{6}\d{3}$", ErrorMessage = "Campus ID must be in the format ABCDEF000.")]
        public string UserID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First Name can only contain letters.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last Name can only contain letters.")]
        public string LastName { get; set; }
        public string FloorNo { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits.")]
        public string PhoneNo { get; set; }
        public string UserRole { get; set; }

        // Populate the 'Floor' and 'UserRole' dropdown lists.

        public IEnumerable<SelectListItem> FloorOptions { get; set; }
        public IEnumerable<SelectListItem> RoleOptions { get; set; }

    }
}
