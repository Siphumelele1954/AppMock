using System.ComponentModel.DataAnnotations;

namespace TugwellApp.Models
{
    public class DC
    {
        public int Id { get; set; }

        [Required]
        public string StudentNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCommitted { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string PresidingWarden { get; set; }

        [DataType(DataType.Date)]
        public DateTime HearingDate { get; set; }

        public bool Iscompleted { get; set; } = false;

        public DC()
        {

        }
    }
}
