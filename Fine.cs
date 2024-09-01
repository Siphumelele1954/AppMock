using System.ComponentModel.DataAnnotations;

namespace TugwellApp.Models
{
    public class Fine
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCommitted { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public string StudentNo { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Fine amount must be a positive value.")]
        public decimal FineAmount { get; set; }

        public bool IsPaid { get; set; } = false;
    }
}
