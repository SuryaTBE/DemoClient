using Microsoft.EntityFrameworkCore;

namespace DemoClient.Models
{
    public class DemoContext:DbContext
    {
        public DemoContext() { }
        public DemoContext(DbContextOptions opt) : base(opt) { }
        public DbSet<UserTbl> UserTbls { get; set; }
        public DbSet<AdminTbl> AdminTbls { get; set; }
        public DbSet<MovieTbl> MovieTbls { get; set; }
        public DbSet<BookingTbl> BookingTbl { get; set; }
        public DbSet<OrderDetailTbl> OrderDetails { get; set; }
        public DbSet<OrderMasterTbl> OrderMasterTbls { get; set; }
    }
}
