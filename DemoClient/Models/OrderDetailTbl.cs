using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class OrderDetailTbl
    {
        [Key]
        public int OrderDetailId { get; set; }

        public string? MovieName { get; set; }
        public int? UserId { get; set; }
        public int MovieId { get; set; }
        public int? OrderMasterId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? MovieDate { get; set; }
        public string? Slot { get; set; }
        public int NoOfTickets { get; set; }
        public string? SeatNo { get; set; }
        public int? Cost { get; set; }
        public virtual UserTbl? usertbl { get; set; }
        public virtual BookingTbl? bookingtbl { get; set; }
        public virtual OrderMasterTbl? OrderMasterTbl { get; set; }
    }
}
