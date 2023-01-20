using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<CompanyGroup> CompanyGroups { get; set; }
        public DbSet<HotelInstance> HotelInstances { get; set; }
        public DbSet<EmployeeInstance> EmployeeInstances { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Award> Awards { get; set; }

        public DbSet<ReserveOrder> ReserveOrders { get; set; }
        public DbSet<GuestRoom> GuestRooms { get; set; }
        public DbSet<LaundryRoom> LaundryRooms { get; set; }
        public DbSet<GymRoom> GymRooms { get; set; }
        public DbSet<MeetingRoom> OtherGuestRooms { get; set; }
        public DbSet<StaffRoom> StaffRooms { get; set; }

        public DbSet<CheckInRecord> CheckInRecords { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Administration> Administrations { get; set; }


        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HotelInstance>()
                .HasOne(hotel => hotel.CompanyGroup)
                .WithMany(company => company.HotelInstances)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.GuestRoom)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
