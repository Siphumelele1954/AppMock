using System.ComponentModel.DataAnnotations;

namespace TugwellApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string Type { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
        public string StudentNo { get; set; }

        public Booking()
        {

        }
    }
}
