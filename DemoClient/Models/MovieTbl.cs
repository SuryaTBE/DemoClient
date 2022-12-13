using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class MovieTbl
    {
        public MovieTbl()
        {
            Bookings = new HashSet<BookingTbl>();
        }
        [Key]
        public int MovieId { get; set; }
        public string? MovieName { get; set; }
        public string? Image { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string? Slot { get; set; }
        public int Cost { get; set; }
        public int capacity { get; set; }

        public virtual ICollection<BookingTbl>? Bookings { get; set; }
    }
}
