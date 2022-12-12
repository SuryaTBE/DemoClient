using System.ComponentModel.DataAnnotations;

namespace DemoClient.Models
{
    public class UserTbl
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        //[Compare("Password",ErrorMessage ="Both Passwords doesnot match!please type again correctly...")]
        //public string CPassword { get; set; }
        public virtual ICollection<BookingTbl>? BookingTbls { get; set; }
        public virtual ICollection<OrderMasterTbl>? OrderMasterTbls { get; set; }
        public virtual ICollection<OrderDetailTbl>? Details { get; set; }
    }
}
