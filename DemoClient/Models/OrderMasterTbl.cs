using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class OrderMasterTbl
    {
        [Key]
        public int OrderMasterId { get; set; }
        public int? UserId { get; set; }
        [DataType(DataType.Date)]

        public DateTime? OrderDate { get; set; }
        public int? CardNo { get; set; }
        public int? Amount { get; set; }
        public int? Paid { get; set; }
        public virtual UserTbl? user { get; set; }

        public virtual ICollection<OrderDetailTbl>? Details { get; set; }
    }
}
