using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TugwellApp.Models;

namespace TugwellApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<TugwellApp.Models.Fine> Fine { get; set; } = default!;
        public DbSet<TugwellApp.Models.Booking> Booking { get; set; } = default!;
        public DbSet<TugwellApp.Models.DC> DC { get; set; } = default!;
    }
}
