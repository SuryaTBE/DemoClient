using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class BookingTbl
    {
        [Key]
        public int BookingId { get; set; }

        public int MovieId{ get; set; }
        public string? MovieName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string? Slot { get; set; }
        public string? On { get; set; }
        public int? UserId { get; set; }
        //[Range(1, 5, ErrorMessage = "Sorry,You can book maximum 5 tickets at a time.")]
        public int NoOfTickets { get; set; }
        public string SeatNo { get; set; }
        public int? AmountTotal { get; set; }
        public virtual MovieTbl? Movie { get; set; }
        public virtual UserTbl? User { get; set; }
        public virtual ICollection<OrderDetailTbl>? OrderDetails { get; set; }
    }
}
